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

  /// <summary>
  /// Container class for MeshGeometry 3D that adds a methods and attributes for rotating meshes and extracting contours. 
  /// 
  /// </summary>
  public class XMesh
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
    /// return index of first triangle encountered to interesect plane defined by z 
    /// returns count if one is not found
    /// </summary>
    /// <param name="z"></param>
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
  
  
  } // end of XMesh class

  public class MeshTriangle
  {
   /// <summary>
   /// constructor for MeshTriangle class.  Principle feature of the class is a list of integtegers that represent indicies for different points that make up the triangle 
   /// and that are members of the XMesh Points list. 
   /// </summary>
   /// <param name="a"></param>
   /// <param name="b"></param>
   /// <param name="c"></param>
    public MeshTriangle(int a, int b, int c)
    {
      Points = new List<int>(); // indices of points that make up the triangle

      Points.Add(a);
      Points.Add(b);
      Points.Add(c);
    }

    public List<int> Points;

    /// <summary>
    /// return indices of points that correspond to a given edge (0,1, or 2) 
    /// </summary>
    /// <param name="TriangleIndex"></param>
    /// <returns></returns>
    public List<int> Edge(int i)
    {
      List<int> edge = new List<int>();
      if (i < 3)
      {
        edge.Add(Points[i]);

        if (i < 2)
          edge.Add(Points[i + 1]);
        else
          edge.Add(Points[0]);
      }
      else 
        throw new ArgumentException("Edge index out of range (0, 1, or 2)", "i: " + i.ToString());
      return edge;
    }




  }
}