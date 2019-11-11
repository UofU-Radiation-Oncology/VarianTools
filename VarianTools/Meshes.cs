using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;
using System.Windows.Media.Media3D;
using System.Windows.Forms;

namespace VarianTools
{

  // -- Mesh Related Functions -- // 
  public static partial class Structures
  {

    public static void RotateMesh()
    {



    }

    public static void ContourFromMesh(MeshGeometry3D m)
    {
      // Save a Mesh Geometry and use for testing 


    }

    // -- helper functions for accessing triangles of a mesh -- // 

    /*

    public static int TCount(MeshGeometry3D m)
    {
      return m.TriangleIndices.Count / 3;
    }

    /// <summary>
    /// returns the starting index in the MeshGeometry3D.TriangleIndices array for a reindexed triangle 
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static int MeshIndex(int TriangleIndex)
    {
      return TriangleIndex * 3;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="TriangleIndex"></param>
    /// <param name="FaceIndex"></param>
    /// <returns></returns>
    public static (int, int) Edge(MeshGeometry3D m, int TriangleIndex, int EdgeIndex)
    {
      int start = TriangleIndex * 3;

      if (EdgeIndex == 2)
        return (start+2,start);
      if (EdgeIndex == 1)
        return (start + 1, start + 2);
      else
        return (start, start + 1);

    }
    
      */
  }








}