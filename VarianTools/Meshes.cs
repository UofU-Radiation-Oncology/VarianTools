using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;
using System.Windows.Media.Media3D;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MeshViewer3D;
using System.Linq;

namespace VarianTools
{

  // -- Mesh Related Functions -- // 
  public static partial class Structures
  {


    public static Point3D GetMesh3DGeometryCentroid(Point3DCollection points)
    {
      // get X y and Z lists
      List<double> x = new List<double>();
      List<double> y = new List<double>();
      List<double> z = new List<double>();

      foreach (var p in points)
      {
        x.Add(p.X);
        y.Add(p.Y);
        z.Add(p.Z);
      }

      return new Point3D(x.Average(), y.Average(), z.Average());

    }
    

  }


}








