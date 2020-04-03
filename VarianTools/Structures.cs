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


    /// <summary>
    /// Genrates a rotational envelope that encompases all potential rotations of struture s given a magnitude and convention of rotation specified by ea
    /// </summary>
    /// <param name="ss">structure set</param>
    /// <param name="s">structure used for generating rotational envelope</param>
    /// <param name="p">point around which rotation occurs</param>
    /// <param name="ea">Euler convention and magnitude of rotations</param>
    /// /// <param name="i">number of iterations in brute force approach</param>
    public static void GenerateRotationalEnvelope(Image img, StructureSet ss, Structure s0, string sRotEnv, VVector p, EulerAngles ea, int n)
    {

      //string convention should = "XY'Z"

      // Create copy of s0 and store as pre
      var sPre = ss.AddStructure("CONTROL", sRotEnv);
      sPre.SegmentVolume = s0.Margin(0.0);
      
      // random number generator is declared here and passed to sampling function to avoid potential deterministic effects of redclaring generator for each sample
      Random rn_gen = new Random();

      for (int i = 0; i < n; i++)
      {
        // Get Original Mesh
        var sXm = new Structures.XMesh(s0.MeshGeometry);

        // Get random angles
        var rea = Euler.RandomGenerator.SampleEulerAngles(rn_gen,ea);

        // Rotate Mesh 
        sXm.RotateMeshPoints(rea,p);

        // Create contour from rotated mesh - search for unique name - it seems 
        string sRotId = General.AppendConstrianedString(s0.Id, "R" + n.ToString(), General.EclipseStringType.StructureId);
        if (StructureExists(ss, sRotId))
          DeleteStructure(ss, sRotId);
        var sRot = ss.AddStructure("CONTROL", sRotId);
        Structures.StructureFromXmesh(img, sXm, sRot);

        // add rotated structure to composite structure
        string sPstId = General.AppendConstrianedString(s0.Id, "P" + n.ToString(), General.EclipseStringType.StructureId);
        if (StructureExists(ss, sPstId))
          DeleteStructure(ss, sPstId);
        var sPst = ss.AddStructure("CONTROL", sPstId);
        sPst.SegmentVolume = sPre.Or(sRot);
        
        // set pre to post, remove pre and rot 
        sPre.SegmentVolume = sPst.SegmentVolume;
        ss.RemoveStructure(sRot);
        ss.RemoveStructure(sPst);
      }
        



    }
  }

}