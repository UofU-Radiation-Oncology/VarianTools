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

    
    // ---- Structure Related Functions ---------------------//


        
    

    }
}
