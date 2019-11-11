using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;


namespace VarianTools
{

  public static partial class Structures
  {
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
        if (contour.Length > 0)
          s.AddContourOnImagePlane(contour, i);
      }

    }

    public static VVector RotatePoint(VVector p, double adeg, double bdeg, double gdeg)
    {
      double alpha = adeg * Math.PI / 180.0;
      double beta = bdeg * Math.PI / 180.0;
      double gamma = gdeg * Math.PI / 180.0;

      EulerAngles ea = new EulerAngles("XY'Z", alpha, beta, gamma);
      var ep = Rotation.RotatePoint(new EulerPoint(p.x, p.y, p.z), ea);
      return new VVector(ep.x, ep.y, ep.z);
    }



  }


  // ---- VolumeContourPoint Class Definition ----- //

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
      double x = b.x - a.x;
      double y = b.y - a.y;
      return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }


  }

}