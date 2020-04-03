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
      /*public void RotatePointsToMaximizeVolume(double adeg, double bdeg, double gdeg)
      {
        for (int i = 0; i < Points.Count; i++)
          RotatePointToMaximizeVolume(i, adeg, bdeg, gdeg);
      }*/
      /*public void RotatePointToMaximizeVolume(int pi, double adeg, double bdeg, double gdeg)
      {

        //? does the maximum angular deviation result in the maximum volume increase 
        List<double> dir = new List<double>();
        dir.Add(1.0);
        dir.Add(-1.0);

        foreach (var d in dir)
        {
          // Get original point and volume
          VVector p0 = Points[pi];
          double v0 = MeshVolume();

          // Rotate Point
          Points[pi] = RotateMeshPoint(p0, adeg*d, bdeg*d, gdeg*d);

          // Get new volume
          double vprime = MeshVolume();

          if (vprime < v0)
            Points[pi] = p0;
        }
      }*/
      /*public VVector RotateMeshPoint(VVector p, double adeg, double bdeg, double gdeg)
{
  double alpha = adeg * Math.PI / 180.0; // convert to radians
  double beta = bdeg * Math.PI / 180.0;  // convert to radians
  double gamma = gdeg * Math.PI / 180.0; // convert to radians

  EulerAngles ea = new EulerAngles("XY'Z", alpha, beta, gamma); //specifies the angles and the convention for rotation
  var ep = Rotation.RotatePoint(new EulerPoint(p.x, p.y, p.z), ea);
  return new VVector(ep.x, ep.y, ep.z);
}*/

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


    } // end of XMesh class

  }
}
