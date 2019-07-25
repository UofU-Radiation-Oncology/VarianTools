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

namespace VarianTools
{
    public partial class PlanSumSelectorForm : Form
    {
        public PlanSumSelectorForm(ScriptContext context)
        {
            InitializeComponent();

            //Populate combo box
            foreach (var p in context.PlanSumsInScope)
                PlanSumComboBox.Items.Add(p.Id);

        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            if (PlanSumComboBox.SelectedItem != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show("Please select a plan sum from the drop down", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
