using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;
using System.Windows.Media;

namespace VarianTools
{
  public static partial class Structures
  {


    public static bool StructureExists(StructureSet ss, string sId)
    {
      bool exists = false;
      foreach (var s in ss.Structures)
      {
        if (s.Id == sId)
          exists = true;
      }
      return exists;
    }


    /// <summary>
    /// returns structure set for PlanningItem plan
    /// </summary>
    /// <param name="plan"></param>
    /// <returns></returns>
    public static StructureSet GetStructureSet(PlanningItem pitem)
    {
      StructureSet structures = null;

      // Get Structure set
      try
      {
        structures = ((PlanSetup)pitem).StructureSet;

      }
      catch
      {
        structures = ((PlanSum)pitem).StructureSet;

      }
      return structures;

    }

    /// <summary>
    /// Retrieves structure object based on an exact match query
    /// </summary>
    /// <param name="ss">StructureSet object</param>
    /// <param name="query">name of the structure (structure id)</param>
    /// <returns>Structure or null  if not found</returns>
    public static Structure GetStructure(StructureSet ss, string query)
    {
      foreach (var s in ss.Structures)
      {
        if (s.Id == query)
          return s;
      }

      return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ss"></param>
    /// <param name="query"></param>
    public static void DeleteStructure(StructureSet ss, string query)
    {
      var s = GetStructure(ss, query);
      if (s != null)
        ss.RemoveStructure(s);
    }
    public static Nullable<int> GetStructureIndex(StructureSet ss, string query)
    {
      for (int i = 0; i < StructureCount(ss); i++)
      {
        if (ss.Structures.ElementAt(i).Id == query)
          return i;
      }

      return null;
    }

    /// <summary>
    /// Presents a dialgoue for selecting a structure from a list of structures
    /// </summary>
    /// <param name="ss">structure set</param>
    /// <returns>selected structure object</returns>
    public static Structure StructureSelectDialogue(StructureSet ss)
    {
      StructureSelectorForm ssf = new StructureSelectorForm(ss);
      if (ssf.ShowDialog() == DialogResult.OK)
      {
        //ssf.StructureComboBox.Text
        return GetStructure(ss, ssf.StructureComboBox.Text);
      }
      else
        return null;
    }

    public static int StructureCount(StructureSet ss)
    {
      int count = 0;
      foreach (var s in ss.Structures)
      {
        count++;
      }
      return count;
    }

    public static void DuplicateStructure(StructureSet ss, string baseId, string copyId)
    {
      // Duplicate Strcutre s
      var s = GetStructure(ss, baseId);
      var ds = ss.AddStructure(s.DicomType, copyId);

      // --  Copy segment volume  -- //
      ds.SegmentVolume = s.SegmentVolume;

    }

    public static void ConvertToHighResolution(Structure s)
    {
      if (!s.IsHighResolution && s.CanConvertToHighResolution())
        s.ConvertToHighResolution();
    }

    public static void HRAndDefault(StructureSet ss, Structure s0, Structure s1)
    {
      if (s0.IsHighResolution && !s1.IsHighResolution)
      {
        var s1HR = ss.AddStructure("CONTROL", "s1HRTemp");
        s1HR.SegmentVolume = s1.SegmentVolume;
        if (s1HR.CanConvertToHighResolution())
        {
          s1HR.ConvertToHighResolution();
          s0.SegmentVolume = s0.And(s1HR);
          DeleteStructure(ss, s1HR.Id);
        }
        else
        {
          DeleteStructure(ss, s1HR.Id);
          MessageBox.Show("Boolean And could not be completed\nStructure could not be converted to high resoulution to match primary structure", "Boolean Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }

    }

    /// <summary>
    /// Adds a structure to structure set ss (if the structure already exists then it overwrites it)
    /// </summary>
    /// <param name="ss"></param>
    /// <param name="dType"></param>
    /// <param name="sId"></param>
    /// <returns></returns>
    public static Structure ADDStructure(StructureSet ss, string dType, string sId)
    {
      if (StructureExists(ss, sId))
        DeleteStructure(ss, sId);
      return ss.AddStructure(dType, sId);
    }

    /// <summary>
    /// returns a segment volume that is the result of a boolean And operation despite differences in structure resolution e.g. high vs default accuracy
    /// a temporary high resolution structure is created for the operation and subsequently deleted. 
    /// </summary>
    /// <param name="ss"></param>
    /// <param name="s0"></param>
    /// <param name="s1"></param>
    /// <returns></returns>
    public static SegmentVolume BooleanAnd(StructureSet ss, Structure s0, Structure s1)
    {
      if ((s0.IsHighResolution && !s1.IsHighResolution) || (!s0.IsHighResolution && s1.IsHighResolution))
      {
        // Create temp HR structure and return segment volume
        if (!s0.IsHighResolution)
        {
          var s0HR = ss.AddStructure("CONTROL", "s0_HR_TEMP");
          s0HR.SegmentVolume = s0.SegmentVolume;
          s0HR.ConvertToHighResolution();
          var sv = s0HR.And(s1.SegmentVolume);
          Structures.DeleteStructure(ss, s0HR.Id);
          return sv;
        }
        else
        {
          var s1HR = ss.AddStructure("CONTROL", "s1_HR_TEMP");
          s1HR.SegmentVolume = s1.SegmentVolume;
          s1HR.ConvertToHighResolution();
          var sv = s0.And(s1HR.SegmentVolume);
          Structures.DeleteStructure(ss, s1HR.Id);
          return sv;
        }
      }
      else
        return s0.And(s1.SegmentVolume);
    }

    public static SegmentVolume BooleanOr(StructureSet ss, Structure s0, Structure s1)
    {
      if ((s0.IsHighResolution && !s1.IsHighResolution) || (!s0.IsHighResolution && s1.IsHighResolution))
      {
        // Create temp HR structure and return segment volume
        if (!s0.IsHighResolution)
        {
          var s0HR = ss.AddStructure("CONTROL", "s0_HR_TEMP");
          s0HR.SegmentVolume = s0.SegmentVolume;
          s0HR.ConvertToHighResolution();
          var sv = s0HR.Or(s1.SegmentVolume);
          Structures.DeleteStructure(ss, s0HR.Id);
          return sv;
        }
        else
        {
          var s1HR = ss.AddStructure("CONTROL", "s1_HR_TEMP");
          s1HR.SegmentVolume = s1.SegmentVolume;
          s1HR.ConvertToHighResolution();
          var sv = s0.Or(s1HR.SegmentVolume);
          Structures.DeleteStructure(ss, s1HR.Id);
          return sv;
        }
      }
      else
        return s0.Or(s1.SegmentVolume);
    }

    public static SegmentVolume BooleanSub(StructureSet ss, Structure s0, Structure s1)
    {
      if ((s0.IsHighResolution && !s1.IsHighResolution) || (!s0.IsHighResolution && s1.IsHighResolution))
      {
        // Create temp HR structure and return segment volume
        if (!s0.IsHighResolution)
        {
          var s0HR = ss.AddStructure("CONTROL", "s0_HR_TEMP");
          s0HR.SegmentVolume = s0.SegmentVolume;
          s0HR.ConvertToHighResolution();
          var sv = s0HR.Sub(s1.SegmentVolume);
          Structures.DeleteStructure(ss, s0HR.Id);
          return sv;
        }
        else
        {
          var s1HR = ss.AddStructure("CONTROL", "s1_HR_TEMP");
          s1HR.SegmentVolume = s1.SegmentVolume;
          s1HR.ConvertToHighResolution();
          var sv = s0.Sub(s1HR.SegmentVolume);
          Structures.DeleteStructure(ss, s1HR.Id);
          return sv;
        }
      }
      else
        return s0.Sub(s1.SegmentVolume);


    }

    /// <summary>
    ///  returns the hausdorf distance in mm between two surfaces defined as the maximum value and adversary would be reguired to travel from surface 2 to surface 1
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static double HausdorfDistance(StructureSet ss, Structure s1, Structure s2)
    {

      var s2Points = GetContourPositions(ss, s2);
      var s1Points = GetContourPositions(ss, s1);

      double hd = 0.0;
      foreach (var s2P in s2Points)
      {
        double md = 9999999.0;
        foreach (var s1P in s1Points)
        {
          var x = s2P.x - s1P.x;
          var y = s2P.y - s1P.y;
          var z = s2P.z - s1P.z;
          var r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));
          // find min distance between s2P and surface s1
          if (r < md)
            md = r;
        }
        if (md > hd)
          hd = md;
      }
      return hd;
    }

    /// <summary>
    /// returns a PointCollection with 3D positions of all points in contour
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static List<VVector> GetContourPositions(StructureSet ss, Structure s)
    {
      List<VVector> points = new List<VVector>();

      for (int i = 0; i < ss.Image.ZSize; i++)
      {
        var contours = s.GetContoursOnImagePlane(i);
        for (int j = 0; j < contours.Length; j++)
          foreach (var p in contours[j])
            points.Add(p);
      }
      return points;      
    }

    public static void StructureFromXmesh(Image img, XMesh xm, Structure s)
    {

      // CURRENTLY ONLY SAMPLES SINGLE SEGMENT OF CONTOUR ON PLANE
      // TASK: NEED TO VERIFY HOW TO BEST HANDLE PARTIAL VOLUME EFFECTS WITH ZDICOMTOIMGPLANE ETC.
      // REVIEW SUB FUNCTIONS - CURRENT STATUS IS TESTING CONTOUR EXTRACTION AND ROTATION //

      // Get max and min z 
      var maxZ = xm.Points.Max(p => p.z);
      var minZ = xm.Points.Min(p => p.z);
      var maxIndex = Images.ZDicomToImgPlane(img, maxZ);
      var minIndex = Images.ZDicomToImgPlane(img, minZ);

      /*string msg = "";
      msg += "maxZ: " + maxZ.ToString();
      msg += "\nminZ: " + minZ.ToString();
      msg += "\nmaxIndex: " + maxIndex.ToString();
      msg += "\nminIndex: " + minIndex.ToString();
      MessageBox.Show(msg);*/

      for (int i = minIndex; i <= maxIndex; i++)
      {
        var zDicom = Images.ImgPlaneToZDicom(img, i);
        //MessageBox.Show(zDicom.ToString());
        try
        {
          var contour = xm.MarchingCubeAlgorithmSingleSegment(zDicom);
          //var contour = xm.MarchingCubeAlgorithm(zDicom);
          if (contour != null)
          {
            //MessageBox.Show("index: " + i.ToString() + "\nContour length: " + contour.Length.ToString());
            foreach (var segment in contour)
              s.AddContourOnImagePlane(segment, i);
          }
        }
        catch(Exception exc)
        {
          General.CMsg("\t**Marching cubes algorithm failed for i: " + i.ToString());
          //var path = (@"C:\Temp\" + s.Id + "_Z" + zDicom.ToString().Replace(".","_") +".msh").Replace("-","_");
          //General.CMsg("STRUCTURE FROM XMESH ERROR: " + exc.Message + "\nTrace: " + exc.StackTrace);
          //General.CMsg("Writing XMesh to file: "+ path);
          //General.SaveObject<XMesh>(xm, path);
    
        }
      }

      //xm.MarchingCubeAlgorithm()
      /*
      string msg = "";

      msg += "Image origin: " + img.Origin.z.ToString();
      msg += "\nZres: " + img.ZRes.ToString();
      msg += "\nmaxZ: " + maxZ.ToString();
      msg += "\nminZ: " + minZ.ToString();
      msg += "\nmaxIndex: " + maxIndex.ToString();
      msg += "\nminIndex: " + minIndex.ToString();
      
      MessageBox.Show(msg);
      */


    }

    public class RotationalEnvelopeConfig
    {
      
      // Private members 
      private int _n;          // number of hardcoded iterations
      private bool _binomial;  // use binomial cuttof?
      private int _nbin;    // number of binomial iterations without observing a change
      private bool _extrema;   // use extrema set?

      // Publics members
      
      // number of hardcoded iterations
      public int n    
      {
        get { return _n; }
        set { _n = value; _nbin = 0; _binomial = false; }
      }
            
      // number of iterations observed without change
      public int nBinomial
      { 
        get {return _nbin;}
        set { _nbin = value; _n = 0; _binomial = true; }
      }
            
      // bool - binomial cutoff being used
      public bool useBinomialCutoff
      { 
        get { return _binomial; }
      }
      
      // bool - numerical cutoff being used
      public bool useNumericalCutoff
      { 
        get { return !_binomial; }
      }


      // bool - use set of standard Euler angles for making convergance more efficient
      public bool useExtrema
      {
        get { return _extrema; }
        set { _extrema = value; }
      }

      // bool - fully random vs using standard Euler angle set. 
      public bool fullyRandom
      {
        get { return !_extrema; }
        set { _extrema = !value; }
      }

      /// <summary>
      /// Sets a default configuration for Envelope generation
      /// </summary>
      public RotationalEnvelopeConfig()
      {
        n = 27;
        useExtrema = true;
      }

      public void SetNBinomial(double coverage, double undersample_probability)
      {
        var c = coverage;
        var usp = undersample_probability;
        double n = Math.Log(usp) / Math.Log(c);
        nBinomial = (int)Math.Ceiling(n);
      }


      public override string ToString()
      {
        string msg = "n: " + n.ToString();
        msg += "\nnBinomial: " + nBinomial.ToString();
        msg += "\nuseBinomialCutoff: " + useBinomialCutoff.ToString();
        msg += "\nuseNuericalCutoff: " + useNumericalCutoff.ToString();
        msg += "\nuseExtrema: " + useExtrema.ToString();
        msg += "\nfullyRandom: " + fullyRandom.ToString();
        return msg;
      }

      public int GetN()
      {
        if (useBinomialCutoff)
          return nBinomial;
        else
          return n;
      }
    }

    /// <summary>
    /// Genrates a rotational envelope that encompases all potential rotations of struture s given a magnitude and convention of rotation specified by ea
    /// </summary>
    /// <param name="ss">structure set</param>
    /// <param name="s">structure used for generating rotational envelope</param>
    /// <param name="p">point around which rotation occurs</param>
    /// <param name="ea">Euler convention and magnitude of rotations</param>
    /// /// <param name="i">number of iterations in brute force approach</param>
    public static List<double> GenerateRotationalEnvelope(Image img, StructureSet ss, Structure s0, string sRotEnv, VVector p, EulerAngles ea, RotationalEnvelopeConfig rec)

    {

      //var msgpre = "VARIANTOOLS.STRUCTURES.GENERATEROTATIONALENVELOPE: "; // prefix used in console messages
      List<double> vols = new List<double>();

      //string convention should = "XY'Z"

      // Convert to high resolution structure
      if (!s0.IsHighResolution && s0.CanConvertToHighResolution())
        s0.ConvertToHighResolution();
      
      // Add vol to iterative volume list
      vols.Add(s0.Volume);

      // Create copy of s0 and store as pre
      var sPre = ss.AddStructure("CONTROL", sRotEnv);
      sPre.SegmentVolume = s0.Margin(0.0);
      
      // random number generator is declared here and passed to sampling function to avoid potential deterministic effects of redclaring generator for each sample
      Random rn_gen = new Random();

      // Get max set of EulerAngles
      List<EulerAngles> ea_max_list = new List<EulerAngles>();
      if(rec.useExtrema)
        ea_max_list = Euler.RandomGenerator.GetMaxEulerAngles(ea);

      //for (int i = 0; i < n; i++)
      int n = 0;

      //while(!VolumeIncreaseStangnant(vols))
      while(!StopCriteriaMet(rec,vols))
      {
        // Get Original Mesh
        var sXm = new Structures.XMesh(s0.MeshGeometry);

        // Get random angles
        //General.CMsg(msgpre + "sampling euler angles");
        EulerAngles rea;
        if (n < ea_max_list.Count)
        {
          //General.CMsg("stnd euler: " + n.ToString());
          rea = ea_max_list[n];
        }
        else
          rea = Euler.RandomGenerator.SampleEulerAngles(rn_gen, ea);

        // Rotate Mesh 
        //General.CMsg(msgpre + "rotating mesh");
        sXm.RotateMeshPoints(rea,p);

        // Create contour from rotated mesh - search for unique name - it seems 
        string sRotId = General.AppendConstrianedString(s0.Id, "R" + n.ToString(), General.EclipseStringType.StructureId);
        if (StructureExists(ss, sRotId))
          DeleteStructure(ss, sRotId);
        var sRot = ss.AddStructure("CONTROL", sRotId);
        //General.CMsg(msgpre + "extracting contour from mesh");

        Structures.StructureFromXmesh(img, sXm, sRot); 
                
        if (!sRot.IsHighResolution && sRot.CanConvertToHighResolution())
          sRot.ConvertToHighResolution();

        // add rotated structure to composite structure
        string sPstId = General.AppendConstrianedString(s0.Id, "P" + n.ToString(), General.EclipseStringType.StructureId);
        if (StructureExists(ss, sPstId))
          DeleteStructure(ss, sPstId);
        var sPst = ss.AddStructure("CONTROL", sPstId);
        //General.CMsg(msgpre + "adding contour to composite");

        if (!sPre.IsHighResolution && sPre.CanConvertToHighResolution())
          sPre.ConvertToHighResolution();
        if (!sPre.IsHighResolution || !sRot.IsHighResolution)
          General.CMsg("Resolution Mismatch: " + sPre.Id + ":" + sPre.IsHighResolution.ToString() + "  " + sRot.Id + ":" + sRot.IsHighResolution.ToString());
        sPst.SegmentVolume = sPre.Or(sRot);
        if (!sPst.IsHighResolution && sPst.CanConvertToHighResolution())
          sPst.ConvertToHighResolution();

        // set pre to post, remove pre and rot 
        sPre.SegmentVolume = sPst.SegmentVolume;
        vols.Add(sPre.Volume);
        ss.RemoveStructure(sRot);
        ss.RemoveStructure(sPst);
        n++;
      }
      return vols;
    }

    private static bool StopCriteriaMet(RotationalEnvelopeConfig rec, List<double> volumes)
    {
      if (rec.useNumericalCutoff && volumes.Count < rec.n)
        return false;
      if (rec.useBinomialCutoff)
      {
        if (volumes.Count <= rec.nBinomial  || (volumes.Count <= 27 && rec.useExtrema))
          return false;
        else
        {
          bool icheck = true;
          for (int i = volumes.Count - rec.nBinomial; i < volumes.Count; i++)
            if (VolumeChanged(volumes[i-1],volumes[i]))
              icheck = false;
          return icheck;
        }
      }
      
      return false;
    }

    private static bool VolumeChanged(double a, double b)
    {
      ///For a 4mm spherical lesion a change in 0.001cc corresponds to a 0.05mm change in diameter.  As size of lesion increases,
      ///the change of diameter decreases for a given change in volume. 
      double delta = 0.001; 
      double up = a + delta;
      double low = a - delta;
      
      if (b <= up && b >= low)
        return false;
      else
        return true;
    }
    
    /// <summary>
    /// Deprecated
    /// </summary>
    /// <param name="volumes"></param>
    /// <returns></returns>
    private static bool VolumeIncreaseStangnant(List<double> volumes)
    {
      int n = 100;
      int c = volumes.Count;
      //General.CMsg(c.ToString());
      
      if (c > 100)
        return true;
      else
        return false;
      
      /*
      if (c > n)
      {
        var b = volumes.Last();
        var a = volumes[c - n];
        if (a > 0.0 && b/a < 1.001)
          return true;
        else
          return false;
      }
      else
        return false;
        */
    }
  
  }

}