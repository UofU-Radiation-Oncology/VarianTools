using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using Euler;

namespace VarianTools
{
  public static partial class Structures
  {
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
        var contour = xm.MarchingCubeAlgorithmSingleSegment(zDicom);

        if (contour != null)
        {
          //MessageBox.Show("index: " + i.ToString() + "\nContour length: " + contour.Length.ToString());
          foreach (var segment in contour)
            s.AddContourOnImagePlane(segment, i);
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


    public static void GenerateRotationalEnvelope(StructureSet ss, string sID)
    {
      // Get original structure s0
      var s0 = ss.Structures.First(s => s.Id == sID);

      // Create copy of s0 and assign to sPre
      var sPre = ss.AddStructure("CONTROL", "GRETEMPsPre");
      sPre.SegmentVolume = s0.Margin(0.0);

      // Get list of Euler angles
      string convention = "XY'Z";
      List<EulerAngles> Angles = new List<EulerAngles>();
      Angles.Add(new EulerAngles(convention, 1 / 180 * Math.PI, 2 / 180 * Math.PI, 3 / 180 * Math.PI));
      //List<Euler.EulerAngles> Angles = Euler.RelatedFunctions.RandomRotationsWithBounds(convention, 1, 1, 1, 2);

      foreach (var ea in Angles)
      {

        // Get Original Mesh
        var sXm = new Structures.XMesh(s0.MeshGeometry);

        // Rotate Mesh 
        sXm.RotateMeshPoints(ea);

        // Get rotated structure (sRot) from mesh
        string rsId = "GRETEMPsRot";
        //StructureFromXmesh(sXm, rsId);
        var sRot = GetStructure(ss, rsId);

        // Add sRot to sPre
        var sPst = ss.AddStructure("CONTROL", "GRETEMPsPst");
        sPst.SegmentVolume = sPre.Or(sRot);

        // Remove sRot and SPre
        ss.RemoveStructure(sRot);
        ss.RemoveStructure(sPre);

        // Set sPre to sPst and remove sPst
        sPre = ss.AddStructure("CONTROL", "GRETEMPsPre");
        sPre.SegmentVolume = sPst.SegmentVolume;

        // Remove sPst
        ss.RemoveStructure(sPst);
      }
    }
  }

}