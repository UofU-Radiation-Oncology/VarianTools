using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;
using System.Windows.Media.Media3D;
using System.Windows.Forms;

namespace VarianTools
{
  public static partial class Structures
  {
    public partial class XMesh
    {


      public bool EdgeIntersectsPlane(int ti, double z)
      {
        for (int i = 0; i < 3; i++)
        {
          if (EdgeContainsZ(Triangles[ti].Edge(i), z))
            return true;
        }
        return false;
      }

      /// <summary>
      /// returns index of triangle that shares an edge with egde ei of Triangle ti else it returns ti
      /// </summary>
      /// <param name="ti"></param>
      /// <param name="ei"></param>
      /// <returns></returns>
      public int TriangleSharingEdge(int ti, int ei)
      {
        var e = Triangles[ti].Edge(ei);

        for (int i = 0; i < Triangles.Count; i++)
        {
          if (i != ti)
          {
            for (int j = 0; j < 3; j++)
            {
              var ej = Triangles[i].Edge(j);
              if (EdgesMatch(e, ej))
                return i;
            }
          }
        }

        return ti;
      }

      public bool EdgesMatch(List<int> e1, List<int> e2)
      {
        int m = 0;
        foreach (var p in e2)
          if (e1.Contains(p))
            m++;
        if (m == e2.Count)
          return true;
        else
          return false;

      }

      /// <summary>
      /// return index of first triangle encountered to interesect plane defined by z;
      /// returns nummber of triangles in mesh if one is not encounter (this value will be out of range of triangles)
      /// </summary>
      /// <param name="z">dicom z coordinate of image plane</param>
      /// <returns></returns>
      public int FirstTriangleToIntersectPlane(double z)
      {
        int count = Triangles.Count;
        for (int ti = 0; ti < count; ti++)
        {
          if (TriangleIntersectsPlane(ti, z))
            return ti;
        }
        return count;
      }

      /// <summary>
      /// returns the first edge of triangle ti to intersect plane z
      /// </summary>
      /// <param name="ti"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      public List<int> FirstEdgeToIntersectPlane(int ti, double z)
      {
        if (EdgeContainsZ(Triangles[ti].Edge(0), z))
          return Triangles[ti].Edge(0);

        else if (EdgeContainsZ(Triangles[ti].Edge(1), z))
          return Triangles[ti].Edge(1);

        else if (EdgeContainsZ(Triangles[ti].Edge(2), z))
          return Triangles[ti].Edge(2);

        else
          return null;
      }

      public bool TriangleIntersectsPlane(int ti, double z)
      {
        for (int i = 0; i < 3; i++)
        {
          if (EdgeContainsZ(Triangles[ti].Edge(i), z))
            return true;
        }
        return false;
      }

      public bool EdgeContainsZ(List<int> edge, double z)
      {
        // WHAT IF LINE RUNS ALONG EDGE???

        if (edge.Count == 2)
        {
          double az = Points[edge[0]].Z;
          double bz = Points[edge[1]].Z;

          if (az > bz)
          {
            if (z < az && z > bz)
              return true;
          }
          else if (bz > az)
          {
            if (z < bz && z > az)
              return true;
          }
          else if (az == bz && z == az)
          {
            MessageBox.Show("Warning plane is coincident with mesh edge\n Algorithms dependent on edge intersections may be ill behaved");
          }
        }

        return false;
      }

      /// <summary>
      /// retruns a point (VVector) along edge at z
      /// </summary>
      /// <param name="edge"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      public VVector EdgePointAtZ(List<int> edge, double z)
      {
        VVector point = new VVector();

        int i1 = edge[0];  // point index 1
        int i2 = edge[1];  // point index 2

        double d = (z - Points[i1].Z) / (Points[i2].Z - Points[i1].Z);  // intersection parameter d (relative distance when travelling from p1 to p2)

        point.x = Points[i1].X + d * (Points[i2].X - Points[i1].X);
        point.y = Points[i1].Y + d * (Points[i2].Y - Points[i1].Y);
        point.z = z;

        return point;
      }





    }
  }
}