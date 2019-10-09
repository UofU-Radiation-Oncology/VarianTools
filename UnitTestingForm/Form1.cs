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

namespace UnitTestingForm
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
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
      TestFunction();
    }
  }
}
