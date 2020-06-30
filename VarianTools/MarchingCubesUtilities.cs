using System;
using System.Linq;
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

      public List<int> GetNextTriangles(VVector p0, VVector p1, double z)
      {
        List<int> nextTriangles = new List<int>();

        for (int i = 0; i < Triangles.Count; i++)
        {

          if (!TriangleContainsPoint(i, p0) && TriangleContainsPoint(i, p1))
          {
            var r = GetTriangleIntesection(i, z);

            if (r == TriangleIntersection.EdgeEdge)
              nextTriangles.Add(i);
            if (r == TriangleIntersection.Coincident)
              nextTriangles.Add(i);
            if (r == TriangleIntersection.PointEdge)
              nextTriangles.Add(i);
          }

        }
        
        return nextTriangles;
      }

      public List<VVector> GetIntersectionPoints(int ti, double z)
      {
        List<VVector> iPoints = new List<VVector>();

        var tPoints = GetTrianglePoints(ti);
        var v0 = tPoints[0];
        var v1 = tPoints[1];
        var v2 = tPoints[2];

        // add any coincident verticies 
        if (Equal(v0.z, z)) 
          iPoints.Add(v0);
        if (Equal(v1.z, z)) 
          iPoints.Add(v1);
        if (Equal(v2.z, z)) 
          iPoints.Add(v2);

        // get edge points

        // cases
        bool c1 = false;
        bool c2 = false;
        bool c3 = false;


        // case 1
        var a = v0;
        var b = v1;
        if (EdgeIntersectedByPlane(a, b, z))
        {
          iPoints.Add(EdgePointOnPlane(a, b, z));
          c1 = true;
        }

        // case 2
        a = v1;
        b = v2;
        if (EdgeIntersectedByPlane(a, b, z))
        {
          iPoints.Add(EdgePointOnPlane(a, b, z));
          c2 = true;
        }

        // case 3
        a = v2;
        b = v0;
        if (EdgeIntersectedByPlane(a, b, z))
        {
          iPoints.Add(EdgePointOnPlane(a, b, z));
          c3 = true;
        }

        if (iPoints.Count > 0 && iPoints.Count < 3)
          return iPoints;
        else
        {
          string msg = "Warning: 0 or >2 intersection points identified in triangle: " + ti.ToString();
          msg += "\nAs a plane may only intersect twice (or be coincident with a face or edge, this likely indicates an error in the alorgirthm or mesh ";
          msg += "\n\nDetails\n";
          msg += "\nv0:\n" + General.VVectorMessage(v0);
          msg += "\n\nv1:\n" + General.VVectorMessage(v1);
          msg += "\n\nv2:\n" + General.VVectorMessage(v2);
          msg += "\n\nz: " + z.ToString();
          msg += "\n\ncase1:" + c1.ToString() + " case2:" + c2.ToString() + " case3:" + c3.ToString();
          msg += "\n\ncount: " + iPoints.Count.ToString();
          //MessageBox.Show(msg, "Marching Cubes Algorithm Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          General.CMsg(msg);
          return null;
        }
      }

      /// <summary>
      /// returns triangle index of first encountered triangle that intersects the plane as long as it has not previously been associated with a segment
      /// </summary>
      /// <param name="z"></param>
      /// <param name="previous_segment"></param>
      /// <returns></returns>
      public int GetFirstTriangle(double z, List<int> previous_segment)
      {
        int count = Triangles.Count;
        for (int ti = 0; ti < count; ti++)
        {
          if (!previous_segment.Contains(ti))
          {
            var r = GetTriangleIntesection(ti, z);
            if (r == TriangleIntersection.EdgeEdge || r == TriangleIntersection.PointEdge || r == TriangleIntersection.Coincident)
              return ti;
          }
        }
        return count; // ti always less than count returning count indicates intersection not found
      }

      public TriangleIntersection GetTriangleIntesection(int ti, double z)
      {
        var tPoints = GetTrianglePoints(ti);
        var v0 = tPoints[0];
        var v1 = tPoints[1];
        var v2 = tPoints[2];

        // count verticies that lie on z plane 
        int vonz = 0;
        foreach (var v in tPoints)
        {
          if (Equal(v.z, z))
            vonz++; 
        }

        // count the number of edges that are intersected by the plane
        int ebyz = 0;

        if (EdgeIntersectedByPlane(v0, v1, z))
          ebyz++;

        if (EdgeIntersectedByPlane(v1, v2, z))
          ebyz++;

        if (EdgeIntersectedByPlane(v2, v0, z))
          ebyz++;


        // Face Check 
        if (vonz == 3)
          return TriangleIntersection.Face;

        // Coincident Check
        if (vonz == 2)
          return TriangleIntersection.Coincident;

        // Point Edge Check
        if (vonz == 1 && ebyz == 1)
          return TriangleIntersection.PointEdge;

        // Point Check 
        if (vonz == 1 && ebyz == 0)
          return TriangleIntersection.Point;

        // Edge Edge Check
        if (vonz == 0 && ebyz == 2)
          return TriangleIntersection.EdgeEdge;
        
        return TriangleIntersection.NoIntersection;
      }

      public bool TriangleContainsPoint(int ti, VVector point)
      {
        var tPoints = GetTrianglePoints(ti);
        var v0 = tPoints[0];
        var v1 = tPoints[1];
        var v2 = tPoints[2];

        if (EdgeContainsPoint(v0, v1, point))
          return true;

        if (EdgeContainsPoint(v1, v2, point))
          return true;

        if (EdgeContainsPoint(v2, v0, point))
          return true;

        return false; 
      }

      /// <summary>
      /// returns true if p lies on line defined by points p0 and p1
      /// </summary>
      /// <param name="p0"></param>
      /// <param name="p1"></param>
      /// <param name="p"></param>
      /// <returns></returns>
      public bool EdgeContainsPoint(VVector p0, VVector p1, VVector p)
      {

        // -- check if point lies at either end -- //

        if (Equal(p0.x, p.x) && Equal(p0.y, p.y) && Equal(p0.z, p.z)) 
          return true;

        if (Equal(p1.x, p.x) && Equal(p1.y, p.y) && Equal(p1.z, p.z)) 
          return true;

        
        // -- check if it lies on line segment -- // 

        double a = p1.x - p0.x;
        double b = p1.y - p0.y;
        double c = p1.z - p0.z;

        // calculate parameter t
        double tx;
        double ty;
        double tz;

        if (!Equal(a, 0.0) && !Equal(b, 0.0) && !Equal(c, 0.0)) 
        {
          tx = (p.x - p0.x) / a;
          ty = (p.y - p0.y) / b;
          tz = (p.z - p0.z) / c;

          if (Equal(tx, ty) && Equal(ty, tz)) 
          {
            var t = tx;
            if (t > 0.0 && t < 1.0)
              return true;
          }
        }


        //if (a == 0.0 && b != 0.0 && c != 0.0)
        if (Equal(a, 0.0) && !Equal(b, 0.0) && !Equal(c, 0.0))
        {
          ty = (p.y - p0.y) / b;
          tz = (p.z - p0.z) / c;

          if (Equal(ty,tz) && Equal(p.x, p0.x))
          {
            var t = ty;
            if (t > 0.0 && t < 1.0)
              return true;
          }

        }

        //if (a != 0.0 && b == 0.0 && c != 0.0)
        if (!Equal(a, 0.0) && Equal(b, 0.0) && !Equal(c, 0.0))
        {
          tx = (p.x - p0.x) / a;
          tz = (p.z - p0.z) / c;

          if (Equal(tx, tz) && Equal(p.y, p0.y)) 
          {
            var t = tx;
            if (t > 0.0 && t < 1.0)
              return true;
          }

        }

        //if (a != 0.0 && b != 0.0 && c == 0.0)
        if (!Equal(a, 0.0) && !Equal(b, 0.0) && Equal(c, 0.0))
        {
          tx = (p.x - p0.x) / a;
          ty = (p.y - p0.y) / b;

          if (Equal(tx, ty) && Equal(p.z, p0.z)) 
          {
            var t = tx;
            if (t > 0.0 && t < 1.0)
              return true;
          }

        }

        //if (a == 0 && b == 0 && c != 0)
        if (Equal(a, 0.0) && Equal(b, 0.0) && !Equal(c, 0.0))
        {
          tz = (p.z - p0.z) / c;

          if (Equal(p.x, p0.x) && Equal(p.y, p0.y))
          {
            var t = tz;
            if (t > 0.0 && t < 1.0)
              return true;
          }
        }

        //if (a == 0 && b != 0 && c == 0)
        if (Equal(a, 0.0) && !Equal(b, 0.0) && Equal(c, 0.0))
        {
          ty = (p.y - p0.y) / c;

          if (Equal(p.x, p0.x) && Equal(p.z, p0.z))
          {
            var t = ty;
            if (t > 0.0 && t < 1.0)
              return true;
          }
        }

        //if (a != 0 && b == 0 && c == 0)
        if (!Equal(a, 0.0) && Equal(b, 0.0) && Equal(c, 0.0))
        {
          tx = (p.x - p0.x) / c;

          if (Equal(p.y, p0.y) && Equal(p.z, p0.z))
          {
            var t = tx;
            if (t > 0.0 && t < 1.0)
              return true;
          }
        }


        return false;

      }

      public bool EdgeIntersectedByPlane(VVector v0, VVector v1, double z)
      {

        double az = v0.z;
        double bz = v1.z;

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

        return false;
      }

      public VVector EdgePointOnPlane(VVector v0, VVector v1, double z)
      {

        VVector point = new VVector();

        double t = (z - v0.z) / (v1.z - v0.z);  // intersection parameter t (relative distance when travelling from p1 to p2)

        point.x = v0.x + t * (v1.x - v0.x);
        point.y = v0.y + t * (v1.y - v0.y);
        point.z = z;

        return point;

      }

      /// <summary>
      /// Evaluates equality based on absolute difference specified by epsion
      /// </summary>
      /// <param name="a"></param>
      /// <param name="b"></param>
      /// <returns></returns>
      private bool Equal(double a, double b)
      {
        double epsilon = 0.000001;
        double d = Math.Abs(a - b);

        if (d < epsilon) 
          return true;
        else
          return false;
      }


    } // public partial class XMesh


    public enum TriangleIntersection
    {
      NoIntersection,
      EdgeEdge,
      Coincident,
      Point,
      PointEdge,
      Face
    }

  } // public static partial class Structures

} // namespace VarianTools