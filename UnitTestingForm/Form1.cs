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

namespace UnitTestingForm
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    public void StructFromXMeshTesting()
    {
      string fname = @"O:\Software\SampleMeshGeometry.mgo";
      MeshGeometry3D m3d = General.LoadObject<MeshGeometry3D>(fname);
      Structures.XMesh mX = new Structures.XMesh(m3d);
      //double z = mX.Points.Max(p => p.Z) - ( ( mX.Points.Max(p=> p.Z) - mX.Points.Min(p => p.Z) ) / 2.0 );
      double z = 2.5;
      
      int ti = mX.FirstTriangleToIntersectPlane(z);

      string msg = "";
      msg += "z: " + z.ToString();
      msg += "\nti: " + ti.ToString();
      msg += "\nt count: " + mX.Triangles.Count.ToString();
      
      MessageBox.Show(msg);

      var e = mX.FirstEdgeToIntersectPlane(ti, z);

      msg = "";
      msg += "edge p1 z: " + mX.Points[e[0]].Z.ToString();
      msg += "\nedge p2 z: " + mX.Points[e[1]].Z.ToString();
      MessageBox.Show(msg);
    }


    public void MeshLoadAndVolumeTest()
    {
      string fname = @"O:\Software\SampleMeshGeometry.mgo";
      MeshGeometry3D m3d = General.LoadObject<MeshGeometry3D>(fname);
      //MessageBox.Show(m3d.TriangleIndices.Count.ToString());
      Structures.XMesh mX = new Structures.XMesh(m3d);
      MessageBox.Show(mX.MeshVolume().ToString());
      MessageBox.Show(mX.Points[0].X.ToString());
      mX.RotatePointsToMaximizeVolume(1.0, 1.5, 2.0);
      MessageBox.Show(mX.Points[0].X.ToString());
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
    }

    
  }
}
