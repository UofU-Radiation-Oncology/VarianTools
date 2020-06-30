using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Media.Media3D;
using System.Reflection;
using EvilDICOM.Network;
using EvilDICOM.Core.Helpers;
using Microsoft.Extensions.Options;

namespace VarianTools
{
  public static class DICOM
  {
    // NOTE THESE FUNCTIONS RELY ON ESTABLISHING A DICOM APPLICATION ENTITY THAT ASSUMES THE IP OF THE RESEARCH BOX

    public static void StoreDoseToFile(string basepath, string UID)
    {
      // Store the details of the daemon (Ae Title , IP , port )
      var daemon = new Entity("ECLIPSEDB", "10.104.90.5", 51402);

      // Store the details of the client (Ae Title , port ) -> IP address isdetermined by CreateLocal() method
      var local = Entity.CreateLocal("RELAY_AE", 9999);

      // Set up a client ( DICOM SCU = Service Class User )
      var client = new DICOMSCU(local);

      // Set up a receiver to catch the files as they come in
      var receiver = new DICOMSCP(local);

      // Let the daemon know we can take anything it sends
      receiver.SupportedAbstractSyntaxes = AbstractSyntax.ALL_RADIOTHERAPY_STORAGE;

      // Create base path if it doesn't exist
      if (!Directory.Exists(basepath))
      {
        try
        {
          Directory.CreateDirectory(basepath);
        }
        catch
        {
          var orphaned = @"C:\Temp";
          General.CMsg($"StoreDICOMToFile: FAILED TO CREATE DIR {basepath} DATA WILL BE STORED IN {orphaned}");
          basepath = orphaned;
        }
      }


      // Set the action when a DICOM files comes in
      receiver.DIMSEService.CStoreService.CStorePayloadAction = (dcm, asc) =>
      {
        var path = Path.Combine(basepath, dcm.GetSelector().SOPInstanceUID.Data + ".dcm");
        General.CMsg($"StoreDICOMToFile: Writing DICOM data to path: {path}");
        dcm.Write(path);
        return true; // Lets daemom know if you successfully wrote to drive
      };

      receiver.ListenForIncomingAssociations(true);

      // Build a finder class to help with C- FIND operations

      var mover = client.GetCMover(daemon);
      var sIOD = new EvilDICOM.Network.DIMSE.IOD.CFindInstanceIOD();
      sIOD.SOPClassUID = SOPClassUID.RTDoseStorage;
      sIOD.SOPInstanceUID = UID;

      General.CMsg("StoreDICOMToFile: Sending C-MOVE message for UID: " + UID.ToString());
      ushort msgId = 1;
      var response = mover.SendCMove(sIOD, local.AeTitle, ref msgId);
      General.CMsg("StoreDICOMToFile: C-MOVE result status: " + response.Status.ToString());
    }
    
    public static void StoreCTImagesToFile(string basepath, string patId, Series imgSeries)
    {
      
    
      // Store the details of the daemon (Ae Title , IP , port )
      var daemon = new Entity("ECLIPSEDB", "10.104.90.5", 51402);

      // Store the details of the client (Ae Title , port ) -> IP address isdetermined by CreateLocal() method
      var local = Entity.CreateLocal("RELAY_AE", 9999);

      // Set up a client ( DICOM SCU = Service Class User )
      var client = new DICOMSCU(local);

      // Set up a receiver to catch the files as they come in
      var receiver = new DICOMSCP(local);

      // Let the daemon know we can take anything it sends
      receiver.SupportedAbstractSyntaxes = AbstractSyntax.ALL_RADIOTHERAPY_STORAGE;

      // Create base path if it doesn't exist
      if (!Directory.Exists(basepath))
      {
        try
        {
          Directory.CreateDirectory(basepath);
        }
        catch
        {
          var orphaned = @"C:\Temp";
          General.CMsg($"StoreDICOMToFile: FAILED TO CREATE DIR {basepath} DATA WILL BE STORED IN {orphaned}");
          basepath = orphaned;
        }
      }


      // Set the action when a DICOM files comes in
      receiver.DIMSEService.CStoreService.CStorePayloadAction = (dcm, asc) =>
      {
        var path = Path.Combine(basepath, dcm.GetSelector().SOPInstanceUID.Data + ".dcm");
        General.CMsg($"StoreDICOMToFile: Writing DICOM data to path: {path}");
        dcm.Write(path);
        return true; // Lets daemom know if you successfully wrote to drive
      };

      receiver.ListenForIncomingAssociations(true);

      // Finder
      var finder = client.GetCFinder(daemon);
      var studies = finder.FindStudies(patId);
      var series = finder.FindSeries(studies);
      var ct = series.Where(s => s.SeriesInstanceUID == imgSeries.UID).SelectMany(ser => finder.FindImages(ser));

      // Mover
      var mover = client.GetCMover(daemon);
      ushort msgId = 1;
      foreach (var im in ct)
      {
        General.CMsg("StoreDICOMToFile: Sending C-MOVE message for UID: " + im.SOPInstanceUID.ToString());
        var response = mover.SendCMove(im, local.AeTitle, ref msgId);
        General.CMsg("StoreDICOMToFile: C-MOVE result status: " + response.Status.ToString());
      }

    }

  }
}





