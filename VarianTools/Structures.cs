using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;

namespace VarianTools
{

  public static partial class Eclipse
  {
    public static string version = "1.0.0";
    
    // ---- Structure related functions ---- //

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

    public static int StructureCount(StructureSet ss)
    {
      int count = 0;
      foreach (var s in ss.Structures)
      {
        count++;
      }
      return count;
    }

    /*public static Tuple<int?,int?> StructureImgPlaneRange(Image img, Structure s)
    {
      int? a;
      int? b;

      if (s.IsEmpty)
        return Tuple.Create<int?, int?>(null, null);
      else
      {
        // img length
        int l = img.ZSize;
        for (int i = 0; i < l; i++)
        {
          
        }
      }


  

    }*/


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

    /// <summary>
    /// Rotates all points in a given structure according to provided angles alpha, beta, and gamma 
    /// </summary>
    /// <param name="s">Structure </param>
    /// <param name="img">Image</param>
    /// <param name="adeg">angle alpha in degrees</param>
    /// <param name="bdeg">angle beta  in degrees</param>
    /// <param name="gdeg">angle gamma in degrees</param>
    public static void RotateStructure(Structure s, Image img, double adeg, double bdeg, double gdeg)
    {
      // foreach image plane (i)
      for (int i = 0; i < 2/*img.ZSize*/; i++)
      {
        var contours = s.GetContoursOnImagePlane(i);
        if (contours.Length > 0)
        {
          // foreach contour (j) on img plane i
          for (int j = 0; j < contours.Length; j++)
          {
            // foreach point (k) of contour j
            for (int k = 0; k < contours[j].Length; k++)
            {

              string msg = "";
              msg += "img plane: " + i.ToString();
              msg += "\ncontour: " + j.ToString();
              msg += "\npoint z: " + contours[j][k].z.ToString();
              MessageBox.Show(msg);

            }
          }
        }
      }

      
      
    }

    public static VVector RotatePoint(VVector p,double adeg, double bdeg, double gdeg)
    {
      double alpha = adeg * Math.PI / 180.0;
      double beta  = bdeg * Math.PI / 180.0;
      double gamma = gdeg * Math.PI / 180.0;

      EulerAngles ea = new EulerAngles("XY'Z", alpha, beta, gamma);
      var ep = Rotation.RotatePoint(new EulerPoint(p.x, p.y, p.z), ea);
      return new VVector(ep.x, ep.y, ep.z);
    }

    public static void DuplicateStructure(StructureSet ss, string baseId, string copyId)
    {
      // Duplicate Strcutre s
      var s = GetStructure(ss, baseId);
      var ds = ss.AddStructure(s.DicomType, copyId);

      // --  Copy segment volume  -- //
      ds.SegmentVolume = s.SegmentVolume;
      
    }
  }
}