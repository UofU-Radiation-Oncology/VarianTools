using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace UnitTestingVarianTools
{
  class Program
  {
    static void Main(string[] args)
    {
      VVector[][][] rpoints = new VVector[10][][]; // vector for storing new points 
      rpoints[0][0][0].x = 0.34;
      rpoints[0][0][0].y = 0.56;
      rpoints[0][0][0].z = 0.78;
      MessageBox.Show(rpoints[0][0][0].x.ToString());
    }
  }
}
