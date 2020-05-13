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
