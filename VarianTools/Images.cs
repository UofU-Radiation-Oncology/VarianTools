using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using Euler;
using System.Windows.Media.Media3D;
using System.Windows.Forms;

namespace VarianTools
{

  // -- Img Related Functions -- // 
  public static partial class Images
  {

    // Img plane to z
    public static double ImgPlaneToZDicom(Image img, int i)
    {
      if (img.ImagingOrientation != PatientOrientation.HeadFirstSupine)
      {
        string msg = "Warning: ImgPlaneToZDicom has not been validated for orientations other than HFS";
        msg += "\nCurrent image orientation: " + img.ImagingOrientation.ToString();
        MessageBox.Show(msg);
      }

      return img.Origin.z + (Convert.ToDouble(i) * img.ZRes);

    }







  }
}
