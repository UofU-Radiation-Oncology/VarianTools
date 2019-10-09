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


    public static void RotateMesh(Structure s, Image img, double adeg, double bdeg, double gdeg)
    {
      //s.MeshGeometry.Normals.
      //s.MeshGeometry = new System.Windows.Media.Media3D.MeshGeometry3D();
      //s.SegmentVolume.
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
      //VVector[][][] rpoints = new VVector[img.ZSize][][]; // vector for storing new points 
      VolumeContourPoints cvol = new VolumeContourPoints();

      // -- Rotate Contour and store in rpoints -- // 
      
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
              //MessageBox.Show(i.ToString() + "\n" + j.ToString() + "\n" + k.ToString());

              // Rotate point
              VVector pprime = RotatePoint(contours[j][k], adeg, bdeg, gdeg);
              
              // Add pprime to new contour representation
              cvol.allpoints.Add(pprime);

              //MessageBox.Show("Point Rotated");
              /*
              try
              {

                // Add point to rotated contour VVector List (rpslist)
                int ip = ZDicomToImgPlane(img, pprime.z);
                //MessageBox.Show(ip.ToString());
                
                rpoints[ip][j][k].x = pprime.x;

                string msg = "";
                msg += "img plane: " + i.ToString();
                msg += "\ncontour: " + j.ToString();
                msg += "\npoint:   " + k.ToString();
                MessageBox.Show(msg);

                rpoints[ip][j][k].y = pprime.y;
                rpoints[ip][j][k].z = ImgPlaneToZDicom(img, ip);
              }
              catch(Exception e)
              {
                
                string msg = "";
                //msg += e.Message.ToString() += "\n\n";
                msg += "img plane: " + i.ToString();
                msg += "\ncontour: " + j.ToString();
                msg += "\npoint:   " + k.ToString();
                msg += "\n\nOriginal Point";
                msg += "\npoint x: " + contours[j][k].x.ToString();
                msg += "\npoint y: " + contours[j][k].y.ToString();
                msg += "\npoint z: " + contours[j][k].z.ToString();

                msg += "\n\nRotated Point";
                msg += "\npoint x: " + pprime.x.ToString();
                msg += "\npoint y: " + pprime.y.ToString();
                msg += "\npoint z: " + pprime.z.ToString();

                MessageBox.Show(msg);
               }
               */
            }
          }
        }
      }

      // -- Clear current contours and add rotated img contour for all image planes -- // 

      for (int i = 0; i < img.ZSize; i++)
      {
        s.ClearAllContoursOnImagePlane(i);
        double imgz = ImgPlaneToZDicom(img, i);
        double range = img.ZRes;
        VVector[] contour = cvol.ContourOnImage(imgz, range);
        if(contour.Length > 0)
          s.AddContourOnImagePlane(contour, i);
      }

    }


    
    public static VVector RotatePoint(VVector p, double adeg, double bdeg, double gdeg)
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

    
    public static double ImgPlaneToZDicom(Image img, int ip)
    {
      double z = (img.ZDirection.z * img.Origin.z) + ((double)ip * img.ZRes);

      /*
      string msg = "";
      msg += "dicom origin z: " + img.Origin.z.ToString();
      msg += "\nz resolution:   " + img.ZRes.ToString();
      msg += "\nZDirection.x:   " + img.ZDirection.x.ToString();
      msg += "\nZDirection.y:   " + img.ZDirection.y.ToString();
      msg += "\nZDirection.z:   " + img.ZDirection.z.ToString();
      msg += "\n\nimage plane: " + ip.ToString();
      msg += "\nZ: " + z.ToString();
      
      MessageBox.Show(msg);
      */
      return z;

    }

    public static int ZDicomToImgPlane(Image img, double Zdicom)
    {
      // Need to consider how to handle partial volume effects
      double ip = (Zdicom - (img.ZDirection.z * img.Origin.z)) / img.ZRes;
      return (int)ip;
    }
  }

  public class VolumeContourPoints
  {

    public VolumeContourPoints()
    {
      allpoints = new List<VVector>();
    }
    public List<VVector> allpoints;

    /// <summary>
    /// Retrieves contour on image plane from allpoints sorted by closest point 
    /// </summary>
    /// <param name="imgz"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public VVector[] ContourOnImage(double imgz, double range)
    {
      List<VVector> points = new List<VVector>();

      foreach (var p in allpoints)
        if (p.z < (imgz + range) && p.z > (imgz - range))
          points.Add(p);

      // Sort Contour 
      var sortedpoints = SortContourPointsByNearNeighbor(points);

      // Format as VVector[] contour on image plane
      VVector[] contour = new VVector[sortedpoints.Count];
      for (int i = 0; i < points.Count; i++)
        contour[i] = sortedpoints[i];

      return contour;
    }

    private List<VVector> SortContourPointsByNearNeighbor(List<VVector> unSortedPoints)
    {
      // ** MAY WANT TO WRITE FUNCTION TO PREVENT ZIGZAGGING. The real question is how to order points in a way to maximize area 
      List<VVector> sortedPoints = new List<VVector>();

      //Get starting point
      var start = unSortedPoints[0];
      unSortedPoints.Remove(start);

      var current = start;
      var remaining = unSortedPoints;
      var path = new List<VVector>();
      path.Add(start);

      while (remaining.Count > 0)
      {
        var next = ClosestPoint(current, remaining);
        path.Add(next);
        remaining.Remove(next);
        current = next;
      }

      return path;

    }

    private VVector ClosestPoint(VVector a, List<VVector> remaining)
    {
      double d = DistanceBetweenPoints(a, remaining[0]);
      int i_close = 0; // index of element closest to point a 

      for (int i = 1; i < remaining.Count; i++) 
      {
        double d_compare = DistanceBetweenPoints(a, remaining[i]); // compare distance 
        if (d_compare < d)
        {
          i_close = i;
          d = d_compare;
        }
      }

      return remaining[i_close];
    }


    private double DistanceBetweenPoints(VVector a, VVector b)
    {
      double x = b.x-a.x;
      double y = b.y-a.y;
      return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }


  }
}