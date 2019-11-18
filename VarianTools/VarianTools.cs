﻿using System;
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

namespace VarianTools
{

  public static partial class Eclipse
  {
    
    /// <summary>
    /// returns the currently selected plan or plansum.  If currently selected item is a plansum and multiple plansums exist prompt user for plan sum to use
    /// </summary>
    /// <param name="context"></param>
    /// <returns>currently selected plan or plansum</returns>
    public static PlanningItem GetCurrentPlanningItem(ScriptContext context)
    {
        // attempt to get plan 
        PlanSetup plan = context.PlanSetup;
        if (plan != null)
            return (PlanningItem)plan;

        // if plan not selected attempt to get plan sum
        int n = context.PlanSumsInScope.Count();
        //MessageBox.Show("n: " + n.ToString());
        if (n > 0)
        {
            //MessageBox.Show("Plan Sums");
            // if more than one prompt user to select plan sum 
            if (n > 1)
            {
                PlanSumSelectorForm ps = new PlanSumSelectorForm(context);
                if (ps.ShowDialog() == DialogResult.OK)
                {
                    int i = ps.PlanSumComboBox.SelectedIndex;
                    return (PlanningItem)context.PlanSumsInScope.ToList()[i];
                }
                else
                    return null;

            }
            // if only one return plansum in scope
            else
                return (PlanningItem)context.PlanSumsInScope.FirstOrDefault();
        }
        else
        {
            MessageBox.Show("Error in loading planning item","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            return null;
                
        }
    }

    public static PlanSetup GetPlan(Course c, string planId)
    {
      foreach (var plan in c.PlanSetups)
        if (plan.Id == planId)
          return plan;

      return null;
    }






  }

  public static partial class General
  {
    public static void WriteLinesToFile(string path, List<string> lines)
    {

      using (System.IO.StreamWriter file = new System.IO.StreamWriter(path)) //@"C:\Users\Public\TestFolder\WriteLines2.txt"))
      {
        foreach (string line in lines)
        {
          file.WriteLine(line);
        }
      }

    }

    /// <summary>
    /// Deserializes an xml file into an object list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static T LoadObject<T>(string fileName)
    {
      //MessageBox.Show(fileName);
      if (string.IsNullOrEmpty(fileName)) { return default(T); }

      T objectOut = default(T);

      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(fileName);
        string xmlString = xmlDocument.OuterXml;

        using (StringReader read = new StringReader(xmlString))
        {
          Type outType = typeof(T);

          XmlSerializer serializer = new XmlSerializer(outType);
          using (XmlReader reader = new XmlTextReader(read))
          {
            objectOut = (T)serializer.Deserialize(reader);
            reader.Close();
          }

          read.Close();
        }
      }
      catch
      {
        MessageBox.Show("Error Loading Object");
      }

      return objectOut;
    }

    /// <summary>
    /// Serializes an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializableObject"></param>
    /// <param name="fileName"></param>
    public static void SaveObject<T>(T serializableObject, string fileName)
    {

      if (serializableObject == null)
      {

        return;
      }

      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
        using (MemoryStream stream = new MemoryStream())
        {
          serializer.Serialize(stream, serializableObject);
          stream.Position = 0;
          xmlDocument.Load(stream);
          xmlDocument.Save(fileName);
          stream.Close();
        }
      }
      catch (Exception ex)
      {
        //Log exception here
        MessageBox.Show("An error occured while trying to save to: " + fileName);
      }
    }

    public static string AppendConstrianedString(string orig, string append, EclipseStringType type)
    {
      int allowed_length = (int)type;
      int n = orig.Length + append.Length - allowed_length;
      orig.Remove(orig.Length - n, n);
      return orig + append;
    }

    public enum EclipseStringType
    {
      StructureId = 16
    }

  }
}
