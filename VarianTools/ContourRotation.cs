using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;


namespace VarianTools
{

  public static partial class Structures
  {


  }


  // ---- VolumeContourPoint Class Definition ----- DEPRACATED //

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