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
    /// <summary>
    /// Container class for MeshGeometry 3D that adds a methods and attributes for rotating meshes and extracting contours. 
    /// 
    /// </summary>
    public partial class XMesh
    {
      public XMesh(MeshGeometry3D m)
      {
        // copy triangle data
        for (int i = 0; i < m.TriangleIndices.Count / 3; i++)
          Triangles.Add(new MeshTriangle(m.TriangleIndices[i * 3], m.TriangleIndices[i * 3 + 1], m.TriangleIndices[(i * 3) + 2]));

        // copy point data
        for (int i = 0; i < m.Positions.Count; i++)
          Points.Add(m.Positions[i]);
      }

      public List<MeshTriangle> Triangles;
      public List<Point3D> Points;





      // ------  EDGE AND TRIANGLE FUNCTIONS ------- //


    } // end of XMesh class

  }
}
