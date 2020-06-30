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

    public static double ImgPlaneToZDicom(Image img, int ip)
    {
    
      if (img.ImagingOrientation != PatientOrientation.HeadFirstSupine)
      {
        string msg = "Warning: ImgPlaneToZDicom has not been validated for orientations other than HFS";
        msg += "\nCurrent image orientation: " + img.ImagingOrientation.ToString();
        //MessageBox.Show(msg);
        General.CMsg(msg);
      }
      
      return (img.ZDirection.z * img.Origin.z) + ((double)ip * img.ZRes);
    
    }

    public static int ZDicomToImgPlane(Image img, double Zdicom)
    {
      // Need to consider how to handle partial volume effects
      double ip = (Zdicom - (img.ZDirection.z * img.Origin.z)) / img.ZRes;
      return (int)ip;
    }




  }
}
