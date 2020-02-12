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

      // These functions are related to the first iteration of the algorithm (Edge Search) Likely can be deleted. 
      public EdgeIntersection EdgeIntersectsPlane(int ti, double z)
      {
        for (int i = 0; i < 3; i++)
        {
          var r = EdgeIntersectedByZ(Triangles[ti].Edge(i), z);
          if (r==EdgeIntersection.Bisects || r == EdgeIntersection.Coincident)
            return r;
        }
        return EdgeIntersection.NoIntersection;
      }
      /// <summary>
      /// returns index of triangle that shares an edge with egde ei of Triangle ti else it returns ti
      /// </summary>
      /// <param name="ti"></param>
      /// <param name="ei"></param>
      /// <returns></returns>
      public int TriangleSharingEdge(int ti, List<int> e)
      {
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
          var r = TriangleIntersectsPlane(ti, z);
          if (r == EdgeIntersection.Coincident || r == EdgeIntersection.Bisects)
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
        try
        {
          //string msg = "Checking triangle: " + ti.ToString();
          //msg += "  Edge 0   for intersection with z: " + z.ToString();
          //MessageBox.Show(msg, "FirstEdgeToIntersectPlane", MessageBoxButtons.OK, MessageBoxIcon.Information);

          List<int> edges = new List<int>(){ 0, 1, 2 };

          foreach (var e in edges)
          {
            var r = EdgeIntersectedByZ(Triangles[ti].Edge(e), z);
            if (r == EdgeIntersection.Bisects || r == EdgeIntersection.Coincident)
              return Triangles[ti].Edge(e);
          }
        }
        catch
        {
          MessageBox.Show("Error finding first edge to intersect plane\nTriangle: " + ti.ToString() + "\nZ: ");
        }
          
            
        return null;
        
      }
      /// <summary>
      /// returns the second edge to instersect the plane defined by z for triangle ti and first edge e1  
      /// if plane is coincident with first edge returns null
      /// </summary>
      /// <param name="ti"></param>
      /// <param name="e1"></param>
      /// <param name="z"></param>
      /// <returns></returns>
      public List<int> SecondEdgeToIntersectPlane(int ti, List<int> e1, double z)
      {

        for (int e = 0; e < 3; e++)
        {
          var edge = Triangles[ti].Edge(e);
          if (!EdgesMatch(e1, edge))
          {
            var r = EdgeIntersectedByZ(edge, z);
            if (r == EdgeIntersection.Bisects)
              return edge;
          }
        }
        
        return null;
      
      }
      public EdgeIntersection TriangleIntersectsPlane(int ti, double z)
      {
        for (int i = 0; i < 3; i++)
        {
          var result = EdgeIntersectedByZ(Triangles[ti].Edge(i), z);
          if (result == EdgeIntersection.Bisects || result == EdgeIntersection.Coincident)
            return result;
        }
        return EdgeIntersection.NoIntersection;
      }
      public EdgeIntersection EdgeIntersectedByZ(List<int> edge, double z)
      {
        // WHAT IF LINE RUNS ALONG EDGE???

        if (edge.Count == 2)
        {
          double az = Points[edge[0]].z;
          double bz = Points[edge[1]].z;

          if (az > bz)
          {
            if (z < az && z > bz)
              return EdgeIntersection.Bisects;
          }
          else if (bz > az)
          {
            if (z < bz && z > az)
              return EdgeIntersection.Bisects;
          }
          else if (az == bz && z == az)
          {
            return EdgeIntersection.Coincident;
            
            /*string msg = "Warning:\npublic bool EdgeIntersectedByZ(List<int> edge, double z)\n";
            msg += "\nz: " + z.ToString();
            msg += "\nv1 z: " + az.ToString();
            msg += "\nv2 z: " + bz.ToString();

            msg += "\n\nPlane is coincident with mesh edge - algorithms dependent on edge intersections may be ill behaved";
            MessageBox.Show(msg,"Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);*/

          }
        }

        return EdgeIntersection.NoIntersection;
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

        double d = (z - Points[i1].z) / (Points[i2].z - Points[i1].z);  // intersection parameter d (relative distance when travelling from p1 to p2)

        point.x = Points[i1].x + d * (Points[i2].x - Points[i1].x);
        point.y = Points[i1].y + d * (Points[i2].y - Points[i1].y);
        point.z = z;

        return point;
      }

    }

    public enum EdgeIntersection
    { 
      NoIntersection,
      Bisects,
      Coincident
    }




  }


}