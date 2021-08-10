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
    [Serializable]
    public partial class XMesh
    {
      public XMesh(MeshGeometry3D m)
      {
        Triangles = new List<MeshTriangle>();
        Points = new List<VVector>();
        
        // copy triangle data
        for (int i = 0; i < m.TriangleIndices.Count / 3; i++)
        {
          Triangles.Add(new MeshTriangle(m.TriangleIndices[i * 3], m.TriangleIndices[i * 3 + 1], m.TriangleIndices[(i * 3) + 2]));
        }
        // copy point data
        for (int i = 0; i < m.Positions.Count; i++)
          Points.Add(General.Point3dToVVector(m.Positions[i]));
      }

      public List<MeshTriangle> Triangles; // list of MeshTriangles objects which each contain three indices of points contained in Points list
      public List<VVector> Points;

      /// <summary>
      /// samples additional points inside of mesh to create a dense point cloud suitable for calculations e.g. hausdorf distance
      /// </summary>
      /// <param name="resolution">sampling resolution in mm</param>
      /// <returns></returns>
      public List<VVector> VerticiesToDensePointCloud(double resolution)
      {
        List<VVector> SampledVertices = new List<VVector>();
        General.CMsg("\tSampling XMesh");
        foreach (var t in Triangles)
        {
          try
          {
            var i0 = t.PointIndices[0];
            var i1 = t.PointIndices[1];
            var i2 = t.PointIndices[2];
            var v0 = Points[i0];
            var v1 = Points[i1];
            var v2 = Points[i2];

            // Add original points
            SampledVertices.Add(v0);
            SampledVertices.Add(v1);
            SampledVertices.Add(v2);

            // Sample additional points on plane and within mesh triangle
            SampledVertices.AddRange(SampleMeshTriangle(v0, v1, v2, resolution));
          }
          catch (Exception e)
          {
            General.CMsg("Error sampling mesh triangle\n" + e.Message);
          }
        }
        return SampledVertices;
      }

      /*
      List<VVector> GenerateGrid(VVector P, VVector Q, VVector R)
      {
        List<VVector> grid = new List<VVector>();

        double res = 1.0; // sampling resolution in mm

        var PQ = VectorSubtract(Q, P);
        var PR = VectorSubtract(R, P);
        var n = VectorProduct(PQ, PR);
        var d = PlaneConstant(n, P);


        return grid;
      }

      // calculates constant assuming normal vector n and point p residing on plane
      double PlaneConstant(VVector n, VVector p)
      {
        return ((n.x * (p.x * -1.0)) + (n.y * (p.y * -1.0)) + (n.z * (p.z * -1.0))) * -1.0;
      }

      // Calculates vector product assuming x y z normal vectors - returns corresponding coefficients
      VVector VectorProduct(VVector A, VVector B)
      {
        VVector P = new VVector();

        // Calculate normal coefficients 
        P.x = (A.y * B.z) - (A.z * B.y);
        P.y = -((A.x * B.z) - (A.z * B.x));
        P.z = (A.x * B.y) - (A.y * B.x);

        return P;

      }
      //*/
      
        
      /// <summary>
      /// Returns points sampled on mesh surface with resolution r (does not maintain topography)
      /// </summary>
      /// <param name="v1">vertex 1</param>
      /// <param name="v2">vertex 2</param>
      /// <param name="v3">vertex 3</param>
      /// <param name="res">sampling resolution (mm)</param>
      /// <returns></returns>
      public List<VVector> SampleMeshTriangle(VVector v0, VVector v1, VVector v2, double res)
      {
        List<VVector> points = new List<VVector>();

        var v0v1 = VectorSubtract(v1, v0);
        var v0v2 = VectorSubtract(v2, v0);

        // determine number of samples for each basis vector based on resolution
        var n01 = Math.Floor(VectorMagnitude(v0v1) / res);
        var n02 = Math.Floor(VectorMagnitude(v0v2) / res);

        for (int j = 0; j <= n02; j++)
        {
          double b;
          if (n02 != 0)
            b = (double)j / (double)n02;
          else
            b = 0.0;

          double a;
          if (n01 != 0)
          {
            for (int i = 1; i <= n01; i++)
            {
              a = (double)i / (double)n01;
              // if point is in triangle calculate and add to points
              if (a + b < 1)
                points.Add(VectorAdd(v0, VectorAdd(ScaleVector(a, v0v1), ScaleVector(b, v0v2))));
            }
          }
          else
          {
            if (b > 0.0 && b < 1.0)
              points.Add(VectorAdd(v0, ScaleVector(b, v0v2)));
          }
        }
        return points;
      }

      /// <summary>
      /// calculates the magnitued of vector r
      /// </summary>
      /// <param name="r">VVector</param>
      /// <returns>double</returns>
      public double VectorMagnitude(VVector r)
      {
        return Math.Sqrt(Math.Pow(r.x, 2) + Math.Pow(r.y, 2) + Math.Pow(r.z, 2));
      }

      /// <summary>
      /// Scales vector r by s
      /// </summary>
      /// <param name="s"></param>
      /// <param name="r"></param>
      /// <returns></returns>
      public VVector ScaleVector(double s, VVector A)
      {
        VVector sA = new VVector();
        sA.x = A.x * s;
        sA.y = A.y * s;
        sA.z = A.z * s;
        return sA;
      }

      /// <summary>
      /// performs vector subtraction
      /// </summary>
      /// <param name="A"></param>
      /// <param name="B"></param>
      /// <returns></returns>
      public VVector VectorSubtract(VVector A, VVector B)
      {
        VVector Sub = new VVector();
        Sub.x = A.x - B.x;
        Sub.y = A.y - B.y;
        Sub.z = A.z - B.z;
        return Sub;
      }

      /// <summary>
      /// performs vector addition
      /// </summary>
      /// <param name="A"></param>
      /// <param name="B"></param>
      /// <returns></returns>
      public VVector VectorAdd(VVector A, VVector B)
      {
        VVector Sum = new VVector();
        Sum.x = A.x + B.x;
        Sum.y = A.y + B.y;
        Sum.z = A.z + B.z;
        return Sum;
      }

      public double MeshVolume()
      {
        //https://stackoverflow.com/questions/16460897/calculate-the-volume-of-a-geometrymodel3d

        VVector v1 = new VVector();
        VVector v2 = new VVector();
        VVector v3 = new VVector();
        double volume = 0.0;

        // foreach triangle 
        for (int i = 0; i < Triangles.Count; i++)
        {
          v1 = Points[Triangles[i].PointIndices[0]];
          v2 = Points[Triangles[i].PointIndices[1]];
          v3 = Points[Triangles[i].PointIndices[2]];
          
          volume += (((v2.y - v1.y) * (v3.z - v1.z) - (v2.z - v1.z) * (v3.y - v1.y)) * (v1.x + v2.x + v3.x)) / 6;
        }

        return volume;
      }

      public List<VVector> GetTrianglePoints(int ti)
      {
        List<VVector> plist = new List<VVector>();
        foreach (var pi in Triangles[ti].PointIndices)
          plist.Add(Points[pi]);
        return plist;
      }


      public void RotateMeshPoints(EulerAngles ea, VVector r)
      {

        //double alpha = adeg * Math.PI / 180.0; // convert to radians
        //double beta = bdeg * Math.PI / 180.0;  // convert to radians
        //double gamma = gdeg * Math.PI / 180.0; // convert to radians

        //EulerAngles ea = new EulerAngles("XY'Z", alpha, beta, gamma); //specifies the angles and the convention for rotation

        // Coordinate transform such that origin is r
        RebaseMeshPoints(r);

        // rotate
        for (int i = 0; i < Points.Count; i++)
        {
          VVector p = Points[i];
          var ep = Rotation.RotatePoint(new EulerPoint(p.x, p.y, p.z), ea);
          Points[i] = new VVector(ep.x, ep.y, ep.z);
        }

        // Coordinate transform back to DICOM origin
        VVector r_inv = new VVector(r.x * -1.0, r.y * -1.0, r.z * -1.0);
        RebaseMeshPoints(r_inv);
      }


      /// <summary>
      /// Shifts coordinate system origin of XMesh points to point p
      /// </summary>
      /// <param name="p"></param>
      public void RebaseMeshPoints(VVector p)
      {
        for (int i = 0; i < Points.Count; i++)
        {
          VVector rp = new VVector();
          rp.x = Points[i].x - p.x;
          rp.y = Points[i].y - p.y;
          rp.z = Points[i].z - p.z;
          Points[i] = rp;
        }
      }

      public void WriteMeshToMacFile()
      {
        List<string> lines = new List<string>();
        
        foreach (var p in Points)
        {
          var x = p.x - Points[0].x;
          var y = p.y - Points[0].y;
          var z = p.z - Points[0].z;
          lines.Add(x.ToString() + "\t" + y.ToString() + "\t" + z.ToString());
       
        }
        General.WriteLinesToFile(@"O:\MacMesh.txt", lines);      
      }
    } // end of XMesh class

  }
}
