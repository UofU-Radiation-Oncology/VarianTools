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
        Triangles = new List<MeshTriangle>();
        Points = new List<Point3D>();

        // copy triangle data
        for (int i = 0; i < m.TriangleIndices.Count / 3; i++)
        {
          Triangles.Add(new MeshTriangle(m.TriangleIndices[i * 3], m.TriangleIndices[i * 3 + 1], m.TriangleIndices[(i * 3) + 2]));
        }
        // copy point data
        for (int i = 0; i < m.Positions.Count; i++)
          Points.Add(m.Positions[i]);
      }

      public List<MeshTriangle> Triangles; // list of MeshTriangles objects which each contain three indices of points contained in Points list
      public List<Point3D> Points;


      public double MeshVolume()
      {
        //https://stackoverflow.com/questions/16460897/calculate-the-volume-of-a-geometrymodel3d

        Point3D v1 = new Point3D();
        Point3D v2 = new Point3D();
        Point3D v3 = new Point3D();
        double volume = 0.0;

        // foreach triangle 
        for (int i = 0; i < Triangles.Count; i++)
        {
          v1 = Points[Triangles[i].Points[0]];
          v2 = Points[Triangles[i].Points[1]];
          v3 = Points[Triangles[i].Points[2]];
          
          volume += (((v2.Y - v1.Y) * (v3.Z - v1.Z) - (v2.Z - v1.Z) * (v3.Y - v1.Y)) * (v1.X + v2.X + v3.X)) / 6;
        }

        return volume;
      }

      public void RotatePointsToMaximizeVolume(double adeg, double bdeg, double gdeg)
      {
        for (int i = 0; i < Points.Count; i++)
          RotatePointToMaximizeVolume(i, adeg, bdeg, gdeg);
      }

      public void RotatePointToMaximizeVolume(int pi, double adeg, double bdeg, double gdeg)
      {

        //? does the maximum angular deviation result in the maximum volume increase 
        List<double> dir = new List<double>();
        dir.Add(1.0);
        dir.Add(-1.0);

        foreach (var d in dir)
        {
          // Get original point and volume
          Point3D p0 = Points[pi];
          double v0 = MeshVolume();

          // Rotate Point
          Points[pi] = RotateMeshPoint(p0, adeg*d, bdeg*d, gdeg*d);

          // Get new volume
          double vprime = MeshVolume();

          if (vprime < v0)
            Points[pi] = p0;
        }
      }

      public void RotateMeshPoints(EulerAngles ea)
      {
        
        //double alpha = adeg * Math.PI / 180.0; // convert to radians
        //double beta = bdeg * Math.PI / 180.0;  // convert to radians
        //double gamma = gdeg * Math.PI / 180.0; // convert to radians

        //EulerAngles ea = new EulerAngles("XY'Z", alpha, beta, gamma); //specifies the angles and the convention for rotation

        for (int i = 0; i < Points.Count; i++)
        {
          Point3D p = Points[i];
          var ep = Rotation.RotatePoint(new EulerPoint(p.X, p.Y, p.Z), ea);
          Points[i] = new Point3D(ep.x, ep.y, ep.z);
        }

      }


      public Point3D RotateMeshPoint(Point3D p, double adeg, double bdeg, double gdeg)
      {
        double alpha = adeg * Math.PI / 180.0; // convert to radians
        double beta = bdeg * Math.PI / 180.0;  // convert to radians
        double gamma = gdeg * Math.PI / 180.0; // convert to radians

        EulerAngles ea = new EulerAngles("XY'Z", alpha, beta, gamma); //specifies the angles and the convention for rotation
        var ep = Rotation.RotatePoint(new EulerPoint(p.X, p.Y, p.Z), ea);
        return new Point3D(ep.x, ep.y, ep.z);
      }
      
    } // end of XMesh class

  }
}
