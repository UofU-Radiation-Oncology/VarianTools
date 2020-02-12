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
      public VVector[] MarchingCubeAlgorithmEdgeSearch(double z)
      {
        // CURRENTLY DOES NOT ACCOMMODATE MULTIPLE SEGMENTS
        // NEED TO DETERMINE HOW TO HANDLE COINCIDENT EDGES
        MessageBox.Show("Marching Cubes Working");
        List<VVector> segment = new List<VVector>(); // individual segement of contour on image plane z
        MeshGeometry3D mg3d = new MeshGeometry3D();
       
        // -- get first point -- //
        
        // get first triangle
        var t = FirstTriangleToIntersectPlane(z);
        var t1 = t; // first triangle 

        // get e1
        var e1 = FirstEdgeToIntersectPlane(t, z);

        // add point
        segment.Add(EdgePointAtZ(e1, z));

        // get e2
        var e2 = SecondEdgeToIntersectPlane(t, e1, z);

        // add point
        segment.Add(EdgePointAtZ(e2, z));

        // move on to next triangle
        t = TriangleSharingEdge(t, e2);
        e1 = e2;



        // -- get remaining points until  we get back to first triangle -- // 

        int c = 0;
        while (t != t1 && c < Triangles.Count)
        {
          // get e2
          e2 = SecondEdgeToIntersectPlane(t, e1, z);

          // add point
          segment.Add(EdgePointAtZ(e2, z));

          // move on to next triangle
          t = TriangleSharingEdge(t, e2);
          e1 = e2;
          c++;
        }

        if (c >= Triangles.Count)
          MessageBox.Show("Marching cube algorithm Warning: \nstarting point not reached - exceeded max triangle constraint in marching algorithm\n ");

        MessageBox.Show("Marching Cubes Finished");
        return segment.ToArray();
      }

      public VVector[][] MarchingCubeAlgorithm(double z)
      {
        List<VVector[]> segments = new List<VVector[]>();


        bool unscanned_segments_exist = true;

        while (unscanned_segments_exist)
        {
          List<VVector> segment = new List<VVector>(); // individual segement of a contour on image plane z
          List<int> scanned_triangles = new List<int>(); // list of triangle indices that have been scanned and included in a contour - used to identify multiple segments in GetFirstTriangle()

          // -- get first point -- //

          // get first triangle
          var t = GetFirstTriangle(z, scanned_triangles);
          if (t == Triangles.Count)
            unscanned_segments_exist = false;

          if (unscanned_segments_exist)
          {
            var t1 = t; // first triangle 
            var tcheck = t; // assigned to triangles that share a coincident edge if exists or t if not used in conjunction with t1 to indicate we are back at start 
            scanned_triangles.Add(t);

            // get first intersection points
            var iPoints = GetIntersectionPoints(t, z);
            var p0 = iPoints[0];
            var p1 = iPoints[1];
            var p0s = p0; // saved for future reference as a check to see if duplicate points are a result of simply circling back 
            var p1s = p1;  // saved for future reference as a check to see if duplicate points are a result of simply circling back 
            segment.Add(p0);
            segment.Add(p1);

            // move on to next triangle
            var nextTriangles = GetNextTriangles(p0, p1, z);
            scanned_triangles.AddRange(nextTriangles);
            t = nextTriangles[0];
            if (nextTriangles.Count == 1)
              tcheck = t;
            else if (nextTriangles.Count == 2)
              tcheck = nextTriangles[1];
            else
            {
              tcheck = t;
              string msg = "Warning: > 2 triangles located in mesh that contain p1 and intersect plane z (exluding previously encountered triangle).";
              msg += "\nAs only two triangles can share an edge, this likely indicates an error in the alorgirthm or mesh ";
              MessageBox.Show(msg, "Marching Cubes Algorithm Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            int c = 0;
            while (t != t1 && tcheck != t1 && c < Triangles.Count)
            {

              //--  Set old p1 to p0 and get next p1 -- //

              p0 = p1; // set p0 to old p1
              iPoints = GetIntersectionPoints(t, z);
              foreach (var p in iPoints)
              {
                if (!Equal(p.x, p0.x) || !Equal(p.y, p0.y) || !Equal(p.z, p0.z))
                  p1 = p;
              }

              // Add p1 
              if (!segment.Contains(p1))
                segment.Add(p1);
              else
              {
                if (!General.VVectorEqual(p1, p0s) && !General.VVectorEqual(p1, p1s))
                {
                  string msg = "Warning: intersection point already in segment \np:\n" + General.VVectorMessage(p1) + "\ninitial p0:\n" + General.VVectorMessage(p0s) + "\ninitial p1:\n" + General.VVectorMessage(p1s);
                  msg += "\nAs points should only be encountered once, this likely indicates an error in the alorgirthm or mesh";
                  MessageBox.Show(msg, "Marching Cubes Algorithm Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
              }


              // move on to next triangle
              nextTriangles = GetNextTriangles(p0, p1, z);
              scanned_triangles.AddRange(nextTriangles);
              t = nextTriangles[0];
              if (nextTriangles.Count == 1)
                tcheck = t;
              else if (nextTriangles.Count == 2)
                tcheck = nextTriangles[1];
              else
              {
                tcheck = t;
                string msg = "Warning: > 2 triangles located in mesh that contain p1 and intersect plane z (exluding previously encountered triangle).";
                msg += "\nAs only two triangles can share an edge, this likely indicates an error in the alorgirthm or mesh ";
                MessageBox.Show(msg, "Marching Cubes Algorithm Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
              }
              c++;

            }

            if (c >= Triangles.Count)
              MessageBox.Show("Marching cube algorithm Warning: \nstarting point not reached - exceeded max triangle constraint in marching algorithm\n ");

            segments.Add(segment.ToArray());

          } // if unscanned segments exist
        
        } // while unscanned segments exist

        if (segments.Count > 0)
          return segments.ToArray();
        else
          return null;

      } // public VVector[][] MarchingCubeAlgorithm(double z)

      /// <summary>
      /// returns contour at z or null if no contour can be generated (e.g. vetex of triangles in plane but no appreciable contour. 
      /// </summary>
      /// <param name="z"></param>
      /// <returns></returns>
      public VVector[][] MarchingCubeAlgorithmSingleSegment(double z)
      {
        List<VVector[]> segments = new List<VVector[]>();


        List<VVector> segment = new List<VVector>(); // individual segement of a contour on image plane z
        List<int> scanned_triangles = new List<int>(); // list of triangle indices that have been scanned and included in a contour - used to identify multiple segments in GetFirstTriangle()

        // -- get first point -- //

        // get first triangle
        var t = GetFirstTriangle(z, scanned_triangles);
        if (t != Triangles.Count)
        {
          var t1 = t; // first triangle 
          var tcheck = t; // assigned to triangles that share a coincident edge if exists or t if not used in conjunction with t1 to indicate we are back at start 
          scanned_triangles.Add(t);

          // get first intersection points
          var iPoints = GetIntersectionPoints(t, z);
          var p0 = iPoints[0];
          var p1 = iPoints[1];
          var p0s = p0; // saved for future reference as a check to see if duplicate points are a result of simply circling back 
          var p1s = p1;  // saved for future reference as a check to see if duplicate points are a result of simply circling back 
          segment.Add(p0);
          segment.Add(p1);

          // move on to next triangle
          var nextTriangles = GetNextTriangles(p0, p1, z);
          scanned_triangles.AddRange(nextTriangles);
          t = nextTriangles[0];
          if (nextTriangles.Count == 1)
            tcheck = t;
          else if (nextTriangles.Count == 2)
            tcheck = nextTriangles[1];
          else
          {
            tcheck = t;
            string msg = "Warning: > 2 triangles located in mesh that contain p1 and intersect plane z (excluding previously encountered triangle).";
            msg += "\nAs only two triangles can share an edge, this likely indicates an error in the alorgirthm or mesh ";
            MessageBox.Show(msg, "Marching Cubes Algorithm Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }

          int c = 0;
          while (t != t1 && tcheck != t1 && c < Triangles.Count)
          {

            //--  Set old p1 to p0 and get next p1 -- //

            p0 = p1; // set p0 to old p1
            iPoints = GetIntersectionPoints(t, z);
            foreach (var p in iPoints)
            {
              if (!Equal(p.x, p0.x) || !Equal(p.y, p0.y) || !Equal(p.z, p0.z))
                p1 = p;
            }

            // Add p1 
            if (!segment.Contains(p1))
              segment.Add(p1);
            else
            {
              if (!General.VVectorEqual(p1, p0s) && !General.VVectorEqual(p1, p1s))
              {
                string msg = "Warning: intersection point already in segment \np:\n" + General.VVectorMessage(p1) + "\ninitial p0:\n" + General.VVectorMessage(p0s) + "\ninitial p1:\n" + General.VVectorMessage(p1s);
                msg += "\nAs points should only be encountered once, this likely indicates an error in the alorgirthm or mesh";
                MessageBox.Show(msg, "Marching Cubes Algorithm Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
              }
            }


            // move on to next triangle
            nextTriangles = GetNextTriangles(p0, p1, z);
            scanned_triangles.AddRange(nextTriangles);
            t = nextTriangles[0];
            if (nextTriangles.Count == 1)
              tcheck = t;
            else if (nextTriangles.Count == 2)
              tcheck = nextTriangles[1];
            else
            {
              tcheck = t;
              string msg = "Warning: > 2 triangles located in mesh that contain p1 and intersect plane z (exluding previously encountered triangle).";
              msg += "\nAs only two triangles can share an edge, this likely indicates an error in the alorgirthm or mesh ";
              MessageBox.Show(msg, "Marching Cubes Algorithm Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            c++;

          }

          if (c >= Triangles.Count)
            MessageBox.Show("Marching cube algorithm Warning: \nstarting point not reached - exceeded max triangle constraint in marching algorithm\n ");

          segments.Add(segment.ToArray());

        }


        if (segments.Count > 0)
          return segments.ToArray();
        else
          return null;

      } // public VVector[][] MarchingCubeAlgorithm(double z)

    }// public partial class XMesh
  
  }

}