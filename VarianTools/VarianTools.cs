using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VarianTools
{

  public static class Eclipse
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

    /// <summary>
    /// returns structure set for PlanningItem plan
    /// </summary>
    /// <param name="plan"></param>
    /// <returns></returns>
    public static StructureSet GetStructureSet(PlanningItem pitem)
    {
        StructureSet structures = null; 

        // Get Structure set
        try
        {
            structures = ((PlanSetup)pitem).StructureSet;

        }
        catch
        {
            structures = ((PlanSum)pitem).StructureSet;

        }
        return structures;

    }

    public static Structure GetStructure(StructureSet ss, string query)
    {
      foreach(var s in ss.Structures)
      {
        if (s.Id == query)
          return s;
      }

      return null;
    }

    public static Nullable<int> GetStructureIndex(StructureSet ss, string query)
    {
      for(int i = 0; i < StructureCount(ss); i++)
      {
        if (ss.Structures.ElementAt(i).Id == query)
          return i;
      }

      return null;
    }

    public static int StructureCount(StructureSet ss)
    {
      int count = 0;
      foreach (var s in ss.Structures)
      {
        count++;
      }
      return count;
    }


    /// <summary>
    /// Presents a dialgoue for selecting a structure from a list of structures
    /// </summary>
    /// <param name="ss">structure set</param>
    /// <returns>selected structure object</returns>
    public static Structure StructureSelectDialogue(StructureSet ss)
    {
      StructureSelectorForm ssf = new StructureSelectorForm(ss);
      if (ssf.ShowDialog() == DialogResult.OK)
      {
        //ssf.StructureComboBox.Text
        return GetStructure(ss, ssf.StructureComboBox.Text);
      }
      else
        return null;
    }
        
    

    }
}
