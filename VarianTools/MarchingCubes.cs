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
      public VVector[] MarchingCubeAlgorithm(double z)
      {
        // CURRENTLY DOES NOT ACCOMMODATE MULTIPLE SEGMENTS
        // NEED TO DETERMINE HOW TO HANDLE COINCIDENT EDGES

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



        // -- get second point -- //
                
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

        
        return segment.ToArray();
      }




    }// public partial class XMesh
  
  }

}