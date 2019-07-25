using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;

namespace VarianTools
{
    public partial class PlanSelectorForm : Form
    {
        public PlanSelectorForm(IEnumerable<PlanSetup> plans)
        {
            InitializeComponent();
            for(int i = 0; i<plans.Count(); i++)
                PlanComboBox.Items.Add(plans.ElementAt(i).Id);

        }

        public PlanSelectorForm(IEnumerable<PlanSetup> plans, string prompt)
        {
            InitializeComponent();
            PromptLabel.Text = prompt;
            for (int i = 0; i < plans.Count(); i++)
                PlanComboBox.Items.Add(plans.ElementAt(i).Id);
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            if (PlanComboBox.SelectedItem != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show("Please select a plan from the drop down", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


    }
}
