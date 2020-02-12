using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using VarianTools;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Microsoft.VisualBasic;


namespace UnitTestingForm
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    public void HelixTestingFunction()
    {

      string fname = @"O:\Software\SampleMeshGeometry.mgo";
      MeshGeometry3D m3d = General.LoadObject<MeshGeometry3D>(fname);
      Structures.XMesh mX = new Structures.XMesh(m3d);


      //double z = mX.Points.Max(p => p.Z) - ((mX.Points.Max(p => p.Z) - mX.Points.Min(p => p.Z)) / 2.0);
      double z = 32.5;
      Point3D plane_origin = new Point3D(0.0, 0.0, z);
      Vector3D plane_normal = new Vector3D(0.0, 0.0, 1.0);

      var contour = MeshGeometryHelper.GetContourSegments(m3d, plane_origin, plane_normal);



      for (int i = 0; i < contour.Count; i++)
      {
        string msg = "";
        msg += "\ni: " + i.ToString();
        msg += "\nx: " + (contour[i].X / 10.0).ToString();
        msg += "\ny: " + (contour[i].Y / 10.0).ToString();
        msg += "\nz: " + (contour[i].Z / 10.0).ToString();
        MessageBox.Show(msg);
      }


    }
    public string PointMessage(Point3D p)
    {
      string msg = "";
      msg += "x: " + p.X.ToString();
      msg += "\ny: " + p.Y.ToString();
      msg += "\nz: " + p.Z.ToString();
      return msg;
    }
    public string VVectorMessage(VVector p)
    {
      string msg = "";
      msg += "x: " + p.x.ToString();
      msg += "\ny: " + p.y.ToString();
      msg += "\nz: " + p.z.ToString();
      return msg;
    }
    public void StructFromXMeshTesting()
    {
      //string fname = @"O:\Software\SampleMeshGeometry.mgo";
      string fname = @"O:\Software\Testing\SimpleMesh.mgo";
      MeshGeometry3D m3d = General.LoadObject<MeshGeometry3D>(fname);
      Structures.XMesh mX = new Structures.XMesh(m3d);
      
      // Get places to sample along mesh
      List<double> sample_planes = new List<double>();
      // Get first location
      var max = mX.Points.Max(p => p.z);
      var min = mX.Points.Min(p => p.z);
      string m1 = "";
      m1 += "min: " + min.ToString();
      m1 += "\nmax: " + max.ToString();

      MessageBox.Show(m1);

      for (int i = 0; min + Convert.ToDouble(i) < max; i++)
        sample_planes.Add(min + Convert.ToDouble(i));
      sample_planes.Add(max);

      // sample planes
      foreach(var zd in sample_planes)
      {
        var contour = mX.MarchingCubeAlgorithmSingleSegment(zd);
        if (contour != null)
        {
          string msg = "Zdicom: " + zd.ToString();
          msg += "\nLength: " + contour[0].Length.ToString();
          MessageBox.Show(msg);
        }
        else 
        {
          for(int i = 0; i < mX.Triangles.Count; i++)
          {

            // check if triangle contains zd
            bool tcontainszd = false;
            var tpoints = mX.GetTrianglePoints(i);
            foreach (var p in tpoints)
            {
              if (General.DoubleEqual(p.z, zd))
                tcontainszd = true;
            }
            if (tcontainszd)
            {
              string m2 = "Triangle: " + i.ToString();
              m2 += "\n";
              foreach (var p in tpoints)
              {
                m2 += General.VVectorMessage(p);
                m2 += "\n\n";
              }
              MessageBox.Show(m2);

            }

          }
        }
        
      }

    }
    public void TriangleContainsPointTesting()
    {
      string fname = @"O:\Software\Testing\SimpleMesh.mgo";
      MeshGeometry3D m3d = General.LoadObject<MeshGeometry3D>(fname);
      Structures.XMesh mX = new Structures.XMesh(m3d);
      for (int i = 0; i < mX.Points.Count; i++)
        mX.Points[i] = new VVector(mX.Points[i].x - 200.0, mX.Points[i].y - 300.0, mX.Points[i].z);

      //var response = Interaction.InputBox("z: ");
      double z = 0.02;
      int t = 23;
      var tpoints = mX.GetIntersectionPoints(t, z);
      var p0 = tpoints[0];
      var p1 = tpoints[1];

      string msg = "";
      var v0 = mX.Points[mX.Triangles[t].PointIndices[0]];
      var v1 = mX.Points[mX.Triangles[t].PointIndices[1]];
      var v2 = mX.Points[mX.Triangles[t].PointIndices[2]];


      msg += "Triangle: " + t.ToString();
      msg += "\n\nv0:\n" + General.VVectorMessage(v0);
      msg += "\n\nv1:\n" + General.VVectorMessage(v1);
      msg += "\n\nv2:\n" + General.VVectorMessage(v2);

      msg += "\n\np0:\n" + General.VVectorMessage(p0);
      msg += "\n\np1:\n" + General.VVectorMessage(p1);

      msg += "\n\nTriangle contains p0: " + mX.TriangleContainsPoint(t, p0).ToString();
      msg += "\nTriangle contains p1: " + mX.TriangleContainsPoint(t, p1).ToString();

      msg += "\n\nIntersectionStatus: " + mX.GetTriangleIntesection(t, z).ToString();
      MessageBox.Show(msg);
    

    }

    public void MarchingCubesHandlesCoincidencesAndFacesTest()
    {
      string fname = @"O:\Software\Testing\SimpleMesh.mgo";
      MeshGeometry3D m3d = General.LoadObject<MeshGeometry3D>(fname);
      Structures.XMesh mX = new Structures.XMesh(m3d);
      for (int i = 0; i < mX.Points.Count; i++)
        mX.Points[i] = new VVector(mX.Points[i].x - 200.0, mX.Points[i].y - 300.0, mX.Points[i].z);      

        
      string msg = "max: " +  mX.Points.Max(p => p.z).ToString();
      msg += "\nmin: " + mX.Points.Min(p => p.z).ToString();
      MessageBox.Show(msg);
      var response = Interaction.InputBox("z: ");
      double z = Convert.ToDouble(response);

      msg = "";
      msg += "SimpleMesh";
      msg += "\nPoint Count: " + mX.Points.Count.ToString();
      msg += "\nTriangle Count: " + mX.Triangles.Count.ToString();

      MessageBox.Show(msg);
      var contour = mX.MarchingCubeAlgorithmSingleSegment(z);
      MessageBox.Show(contour[0].Length.ToString());
      /*
      List<int> scanned_triangles = new List<int>(); // list of triangle indices that have been scanned and included in a contour - used to identify multiple segments in GetFirstTriangle()

      List<VVector> segment = new List<VVector>(); // individual segement of contour on image plane z


      // -- get first point -- //

      // get first triangle
      var t = mX.GetFirstTriangle(z, scanned_triangles);
      var t1 = t; // first triangle 
      var tcheck = t; // assigned to triangles that share a coincident edge if exists or t if not used in conjunction with t1 to indicate we are back at start 
      scanned_triangles.Add(t);

      // get first intersection points
      var iPoints = mX.GetIntersectionPoints(t, z);
      var p0 = iPoints[0];
      var p1 = iPoints[1];
      var p0s = p0; // saved for future reference as a check to see if duplicate points are a result of simply circling back 
      var p1s = p1;  // saved for future reference as a check to see if duplicate points are a result of simply circling back 
      segment.Add(p0);
      segment.Add(p1);

      msg = "";
      foreach (var v in mX.Triangles[t].PointIndices)
      {
        msg += "Vertext: " + v.ToString();
        msg += "\n" + General.VVectorMessage(mX.Points[v]);
        msg += "\n\n";

      }

      msg += "p0:\n" + General.VVectorMessage(p0);
      msg += "\n\np1:\n" + General.VVectorMessage(p1);
      
      msg += "\n\n" + mX.TriangleContainsPoint(t ,p0).ToString();
      msg += "\n\n" + mX.TriangleContainsPoint(t, p1).ToString();

      MessageBox.Show(msg);

      */
      //var contour = mX.MarchingCubeAlgorithm(z);

      //msg = contour.Length.ToString();

      //MessageBox.Show(msg);


    }
    public void MeshLoadAndVolumeTest()
    {
      string fname = @"O:\Software\SampleMeshGeometry.mgo";
      MeshGeometry3D m3d = General.LoadObject<MeshGeometry3D>(fname);
      //MessageBox.Show(m3d.TriangleIndices.Count.ToString());
      Structures.XMesh mX = new Structures.XMesh(m3d);
      MessageBox.Show(mX.MeshVolume().ToString());
      MessageBox.Show(mX.Points[0].x.ToString());
      mX.RotatePointsToMaximizeVolume(1.0, 1.5, 2.0);
      MessageBox.Show(mX.Points[0].x.ToString());
      MessageBox.Show(mX.MeshVolume().ToString());
      //MessageBox.Show(mX.MeshVolume().ToString());

    }
    public void TestFunction()
    {
      
      

      VVector[][][] vv = new VVector[10][][]; // vector for storing new points
      vv[0] = new VVector[10][];
      vv[0][0] = new VVector[10];
      vv[0][0][0] = new VVector(0.34, 0.560, 0.78);
      MessageBox.Show("x: " + vv[0][0][0].x.ToString());
    }
    public void PointOnEdgeTesting()
    {


      
      
      MeshGeometry3D m3d = new MeshGeometry3D();
      Structures.XMesh xm = new Structures.XMesh(m3d);
      VVector v0 = new VVector(0.0, 0.0, 0.0);
      VVector v1 = new VVector(-2.0, -2.0, -1.0);

      var ep = xm.EdgePointOnPlane(v0, v1, 0.5);
      MessageBox.Show(General.VVectorMessage(ep));
      MessageBox.Show(xm.EdgeContainsPoint(v0, v1, ep).ToString());
      

    }
    public void TestSortingFunction()
    {
      VolumeContourPoints vcp = new VolumeContourPoints();
      vcp.allpoints.Add(new VVector(1.0, 1.0, 1.0));
      vcp.allpoints.Add(new VVector(2.0, 2.0, 1.0));
      vcp.allpoints.Add(new VVector(1.5, 1.5, 1.0));
      vcp.allpoints.Add(new VVector(3.0, 3.0, 1.0));
      vcp.allpoints.Add(new VVector(0.0, 0.5, 1.0));
      vcp.allpoints.Add(new VVector(1.0, 1.0, 1.0));
    }
    private void Button1_Click(object sender, EventArgs e)
    {
      StructFromXMeshTesting();
      //MarchingCubesHandlesCoincidencesAndFacesTest();
      //TriangleContainsPointTesting();
      //PointOnEdgeTesting();
    }

    
  }
}
