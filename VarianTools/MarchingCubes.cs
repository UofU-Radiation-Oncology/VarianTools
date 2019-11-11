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
      public void MarchingCubeAlgorithm(double z)
      {
        List<VVector> segment = new List<VVector>(); // individual segement of contour on image plane z
        
        
        // get fist triangle
        var t1 = FirstTriangleToIntersectPlane(z);

        // get e1
        var e1 = FirstEdgeToIntersectPlane(t1, z);

        // add point
        segment.Add(EdgePointAtZ(e1, z));

        // get e2

        // add point


        // get triangle adjoining last edge

        // get next edge

        // add point 







      }




      public void MarchingCubeAlgorithmDEP(double z)
      {

        // get first edge
        var e1 =





        // Get first triangle
        int ti = FirstTriangleToIntersectPlane(z);




        // WHAT IF THERE ARE MULTIPLE CONTOURS?

        //VVector[][] contour;
        //int ci = 0;  // contour index
        //int pi = 0;  // point index
        List<VVector> contourlist = new List<VVector>();

        // Get first triangle
        int ti = FirstTriangleToIntersectPlane(z);



        // -- Add intersection points to ordered point array -- //

        int edge_number = 0; // number of intersecting edges that have currently been encountered and added to the point collection. 
        int last_edge = 0; // edge containing final intersecting point to be added for triangle ti

        for (int e = 0; e < 3; e++)
        {
          if (EdgeContainsZ(Triangles[ti].Edge(e), z))
          {
            // Add intersection point 
            contourlist.Add(EdgePointAtZ(Triangles[ti].Edge(e), z));
            edge_number++;
            if (edge_number == 2)
            {
              last_edge = edge_number;
              break;
            }
          }
        }

        // Get next triangle 
        TriangleSharingEdge(ti, last_edge);





      }




    }// public partial class XMesh
  
  }

}