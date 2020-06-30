using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Media.Media3D;
using System.Reflection;



namespace VarianTools
{

  public static partial class Eclipse
  {

    /// <summary>
    /// returns the currently selected plan or plansum.  If currently selected item is a plansum and multiple plansums exist prompt user for plan sum to use
    /// </summary>
    /// <param name="context"></param>
    /// <returns>currently selected plan or plansum</returns>
    public static PlanningItem GetCurrentPlanningItem(ScriptContext context)
    {
      // attempt to get plan 
      PlanSetup plan = context.PlanSetup;
      if (plan != null)
        return (PlanningItem)plan;

      // if plan not selected attempt to get plan sum
      int n = context.PlanSumsInScope.Count();
      //MessageBox.Show("n: " + n.ToString());
      if (n > 0)
      {
        //MessageBox.Show("Plan Sums");
        // if more than one prompt user to select plan sum 
        if (n > 1)
        {
          PlanSumSelectorForm ps = new PlanSumSelectorForm(context);
          if (ps.ShowDialog() == DialogResult.OK)
          {
            int i = ps.PlanSumComboBox.SelectedIndex;
            return (PlanningItem)context.PlanSumsInScope.ToList()[i];
          }
          else
            return null;

        }
        // if only one return plansum in scope
        else
          return (PlanningItem)context.PlanSumsInScope.FirstOrDefault();
      }
      else
      {
        //MessageBox.Show("Error in loading planning item", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        var msg = "Error in loading planning item";
        General.CMsg(msg);
        return null;

      }
    }

    public static PlanSetup GetPlan(Course c, string planId)
    {
      foreach (var plan in c.PlanSetups)
        if (plan.Id == planId)
          return plan;

      return null;
    }

    /// <summary>
    /// STANDALONE: checks if course c exists for patient p
    /// </summary>
    /// <param name="p">patient</param>
    /// <param name="cid">course id</param>
    /// <returns></returns>
    public static bool CourseExists(Patient p, string cid)
    {
      foreach (var c in p.Courses)
        if (c.Id == cid)
          return true;

      return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="op"></param>
    /// <param name="np"></param>
    /// <param name="iso">VVector isocenter position in new image to which beam will be copied</param>
    public static void CopyBeamsToNewPlan(ExternalPlanSetup op, ExternalPlanSetup np, VVector iso)
    {
      foreach (var beam in op.Beams)
      {
        if (!beam.IsSetupField)
        {
          General.CMsg("\nattempting to copy: " + beam.Id + " from plan " + op.Id + " to plan " + np.Id);
          DuplicateBeam(np, beam, iso);  // copy beam to new plan
          //CopyBeam(np, beam, iso);  // copy beam to new plan
          General.CMsg("\nsuccessful copy");
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="plan"></param>
    /// <param name="beam"></param>
    /// <param name="iso">VVector isocenter position in new image to which beam will be copied</param> 
    public static void DuplicateBeam(ExternalPlanSetup plan, Beam beam, VVector iso)
    {
      // PRIMARY FLUENCE MODEL ID????

      General.EclipseBeamType btype = BeamType(beam);

      // machine parameters
      string mID = beam.TreatmentUnit.Id; ;  // machine ID
      General.CMsg(mID);
      string eID = beam.EnergyModeDisplayName;  // energy ID
      General.CMsg(eID);
      int dr = beam.DoseRate;     // dose rate
      General.CMsg(dr.ToString());
      string tID = beam.Technique.Id;  // technique ID
      General.CMsg(tID);
      ExternalBeamMachineParameters mp = new ExternalBeamMachineParameters(mID, eID, dr, tID, null); // machine paramters
      
      VRect<double> jp = beam.ControlPoints.First().JawPositions; // jaw positions
      double col = beam.ControlPoints.First().CollimatorAngle; // collimator angle 
      double g_start = beam.ControlPoints.First().GantryAngle; // gantry start
      double g_stop = beam.ControlPoints.Last().GantryAngle; // gantry stop
      double psa = beam.ControlPoints.First().PatientSupportAngle; // couch
      int cpn = beam.ControlPoints.Count(); // number of controlpoints
      float [,] lp = beam.ControlPoints.First().LeafPositions;
      List<double> msw = new List<double>(); // meterset weights
      foreach (var cp in beam.ControlPoints)
        msw.Add(cp.MetersetWeight);

      var ep = beam.GetEditableParameters();
      General.CMsg("BeamType: " + btype.ToString());
      ep.Isocenter = iso;

      if (btype == General.EclipseBeamType.ArcBeam)
      {
        var db = plan.AddArcBeam(mp, jp, col, g_start, g_stop, beam.GantryDirection, psa, iso); // create beam in ExternalPlanSetup plan
        db.ApplyParameters(ep);
      }

      else if (btype == General.EclipseBeamType.ConformalArcBeam)
      {
        var db = plan.AddConformalArcBeam(mp, col, cpn, g_start, g_stop, beam.GantryDirection, psa, iso);
        db.ApplyParameters(ep);
      }

      else if (btype == General.EclipseBeamType.MLCArcBeam)
      {
        var db = plan.AddMLCArcBeam(mp, lp, jp, col, g_start, g_stop, beam.GantryDirection, psa, iso);
        db.ApplyParameters(ep);
      }

      else if (btype == General.EclipseBeamType.MLCBeam)
      {
        var db = plan.AddMLCBeam(mp, lp, jp, col, g_start, psa, iso);
        db.ApplyParameters(ep);
      }

      else if (btype == General.EclipseBeamType.MultipleStaticSegmentBeam)
      {
        var db = plan.AddMultipleStaticSegmentBeam(mp, msw, col, g_start, psa, iso);
        db.ApplyParameters(ep);
      }

      else if (btype == General.EclipseBeamType.SlidingWindowBeam)
      {
        var db = plan.AddSlidingWindowBeam(mp, msw, col, g_start, psa, iso);
        db.ApplyParameters(ep);
      }

      else if (btype == General.EclipseBeamType.StaticBeam)
      {
        var db = plan.AddStaticBeam(mp, jp, col, g_start, psa, iso);
        db.ApplyParameters(ep);
      }

      else if (btype == General.EclipseBeamType.VMATBeam)
      {
        var db = plan.AddVMATBeam(mp, msw, col, g_start, g_stop, beam.GantryDirection, psa, iso);
        db.ApplyParameters(ep);
      }

    }

    /// <summary>
    /// Create a copy of an existing beam (beams are unique to plans).
    /// (from Varian Code Samples)
    /// </summary>
    public static Beam CopyBeam(ExternalPlanSetup plan, Beam b, VVector iso) 
    {
      
      // machine parameters
      string mID = b.TreatmentUnit.Id; ;  // machine ID
      string eID = b.EnergyModeDisplayName;  // energy ID
      int dr = b.DoseRate;     // dose rate
      string tID = b.Technique.Id;  // technique ID
      ExternalBeamMachineParameters mp = new ExternalBeamMachineParameters(mID, eID, dr, tID, null); // machine paramters

      // Create a new beam
      var collimatorAngle = b.ControlPoints.First().CollimatorAngle;
      var gantryAngle = b.ControlPoints.First().GantryAngle;
      var metersetWeights = b.ControlPoints.Select(cp => cp.MetersetWeight);
      var PatientSupportAngle = b.ControlPoints.First().PatientSupportAngle;
      var nb = plan.AddSlidingWindowBeam(mp, metersetWeights, collimatorAngle, gantryAngle, PatientSupportAngle, iso);

      // Copy control points from the original beam.
      var editableParams = nb.GetEditableParameters();
      for (var i = 0; i < editableParams.ControlPoints.Count(); i++)
      {
        editableParams.ControlPoints.ElementAt(i).LeafPositions = b.ControlPoints.ElementAt(i).LeafPositions;
        editableParams.ControlPoints.ElementAt(i).JawPositions = b.ControlPoints.ElementAt(i).JawPositions;
      }
      nb.ApplyParameters(editableParams);
      return nb;
    }

    public static General.EclipseBeamType BeamType(Beam beam)
    {
      // --  Determine existing beam type and create new beam  -- // 
      bool isArc;
      if (beam.GantryDirection == GantryDirection.None)
        isArc = false;
      else
        isArc = true;

      // ArcBeam
      if (beam.MLCPlanType == MLCPlanType.NotDefined && isArc)
        return General.EclipseBeamType.ArcBeam;


      // ConformalArcBeam
      else if (beam.MLCPlanType == MLCPlanType.ArcDynamic && isArc)
        return General.EclipseBeamType.ConformalArcBeam;

      // MLCArcBeam
      else if (beam.MLCPlanType == MLCPlanType.Static && isArc)
        return General.EclipseBeamType.MLCArcBeam;

      // MLCBeam
      else if (beam.MLCPlanType == MLCPlanType.Static && !isArc)
        return General.EclipseBeamType.MLCBeam;

      // NEEDS WORK
      // MultipleStaticSegmentBeam or SlidingwindowBeam
      else if (beam.MLCPlanType == MLCPlanType.DoseDynamic && !isArc)
      {
        return General.EclipseBeamType.MultipleStaticSegmentBeam;
      }

      // StaticBeam
      else if (beam.MLCPlanType == MLCPlanType.NotDefined && !isArc)
        return General.EclipseBeamType.StaticBeam;

      // AddVMATBeam
      else if (beam.MLCPlanType == MLCPlanType.VMAT && isArc)
        return General.EclipseBeamType.VMATBeam;

      else
        return General.EclipseBeamType.None;

    }

  } // public static partial class Eclipse

  public static partial class General
  {
    public static void WriteLinesToFile(string path, List<string> lines)
    {

      using (System.IO.StreamWriter file = new System.IO.StreamWriter(path)) //@"C:\Users\Public\TestFolder\WriteLines2.txt"))
      {
        foreach (string line in lines)
        {
          file.WriteLine(line);
        }
      }

    }

    /// <summary>
    /// Deserializes an xml file into an object list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static T LoadObject<T>(string fileName)
    {
      //MessageBox.Show(fileName);
      if (string.IsNullOrEmpty(fileName)) { return default(T); }

      T objectOut = default(T);

      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(fileName);
        string xmlString = xmlDocument.OuterXml;

        using (StringReader read = new StringReader(xmlString))
        {
          Type outType = typeof(T);

          XmlSerializer serializer = new XmlSerializer(outType);
          using (XmlReader reader = new XmlTextReader(read))
          {
            objectOut = (T)serializer.Deserialize(reader);
            reader.Close();
          }

          read.Close();
        }
      }
      catch
      {
        //MessageBox.Show("Error Loading Object");
        General.CMsg("Error Loading Object");
      }

      return objectOut;
    }

    /// <summary>
    /// Serializes an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializableObject"></param>
    /// <param name="fileName"></param>
    public static void SaveObject<T>(T serializableObject, string fileName)
    {

      if (serializableObject == null)
      {

        return;
      }

      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
        using (MemoryStream stream = new MemoryStream())
        {
          serializer.Serialize(stream, serializableObject);
          stream.Position = 0;
          xmlDocument.Load(stream);
          xmlDocument.Save(fileName);
          stream.Close();
        }
      }
      catch (Exception ex)
      {
        //Log exception here
        //MessageBox.Show("An error occured while trying to save to: " + fileName);
        General.CMsg("An error occured while trying to save to: " + fileName);
      }
    }

    public static void WriteXMLForPatient(Patient p)
    {

      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.IndentChars = ("\t");
      System.IO.MemoryStream mStream = new System.IO.MemoryStream();
      using (XmlWriter writer = XmlWriter.Create(mStream, settings))
      {
        writer.WriteStartDocument(true);
        writer.WriteStartElement("Patient");
        p.WriteXml(writer);       // add the Patient XML to the writer stream
        writer.WriteEndElement(); // </Patient>
        writer.WriteEndDocument();

        // done writing to the memory stream
        writer.Flush();
        mStream.Flush();

        // create the XML file.
        string temp = System.Environment.GetEnvironmentVariable("TEMP");
        string sXMLPath = string.Format("{0}\\{1}({2})-patient.xml", temp, p.LastName, p.Id);
        General.CMsg(sXMLPath);
        using (System.IO.FileStream file = new System.IO.FileStream(sXMLPath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
        {
          // Have to rewind the MemoryStream in order to read its contents. 
          mStream.Position = 0;
          mStream.CopyTo(file);
          file.Flush();
          file.Close();
        }
        // 'Start' generated XML file to launch browser window
        System.Diagnostics.Process.Start(sXMLPath);
        // Sleep for a few seconds to let internet browser window to start
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
      }

    }
    public static string AppendConstrianedString(string orig, string append, EclipseStringType type)
    {
      int allowed_length = (int)type;
      int n = orig.Length + append.Length - allowed_length;

      if (n > 0)
        orig = orig.Remove(orig.Length - n, n);
      
      return orig + append;
    }

    public static void CMsg(string txt)
    {
      System.Console.WriteLine(txt);
    }

    public static VVector Point3dToVVector(Point3D p)
    {
      VVector vvp = new VVector();
      vvp.x = p.X;
      vvp.y = p.Y;
      vvp.z = p.Z;

      return vvp;
    }

    public static Point3D VVectorToPoint3D(VVector vv)
    {
      Point3D p3d = new Point3D();
      p3d.X = vv.x;
      p3d.Y = vv.y;
      p3d.Z = vv.z;

      return p3d;
    }

    public static string VVectorMessage(VVector p)
    {
      string msg = "";
      msg += "x: " + p.x.ToString();
      msg += "\ny: " + p.y.ToString();
      msg += "\nz: " + p.z.ToString();
      return msg;
    }

    public static bool VVectorEqual(VVector a, VVector b)
    {
      bool x = DoubleEqual(a.x, b.x);
      bool y = DoubleEqual(a.y, b.y);
      bool z = DoubleEqual(a.z, b.z);
      if (x && y && z)
        return true;
      else
        return false;
    }

    public static bool DoubleEqual(double a, double b)
    {
        double epsilon = 0.000001;
        double d = Math.Abs(a - b);
        if (d < epsilon)
          return true;
        else
          return false;
    }
    public enum EclipseStringType
    {
      StructureId = 16,
      CourseId = 16,
      StructureSetId = 16,
      ImageSeriesId = 16,
      ImageId = 16,
      PlanId = 13
    }
    public enum EclipseBeamType
    {
      ArcBeam,
      ConformalArcBeam,
      MLCArcBeam,
      MLCBeam,
      MultipleStaticSegmentBeam,
      SlidingWindowBeam,
      StaticBeam,
      VMATBeam,
      None

    }
  }
}
