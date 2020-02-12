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
        PointIndices = new List<int>(); // indices of points that make up the triangle

        PointIndices.Add(a);
        PointIndices.Add(b);
        PointIndices.Add(c);
      }

      public List<int> PointIndices;

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
          edge.Add(PointIndices[i]);

          if (i < 2)
            edge.Add(PointIndices[i + 1]);
          else
            edge.Add(PointIndices[0]);
        }
        else
          throw new ArgumentException("Edge index out of range (0, 1, or 2)", "i: " + i.ToString());
        return edge;
      }




    }





  }
}