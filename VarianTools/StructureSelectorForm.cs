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
using VMS.TPS.Common.Model.Types;

namespace VarianTools
{
  public partial class StructureSelectorForm : Form
  {
    public StructureSelectorForm(StructureSet structures)
    {
      InitializeComponent();

      //Populate combo box 
      foreach (var s in structures.Structures)
      {
        StructureComboBox.Items.Add(s.Id);
      }
    }

    private void OkayButton_Click(object sender, EventArgs e)
    {
      if (StructureComboBox.SelectedItem != null)
      {
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
      else
        MessageBox.Show("Please select a structure from the drop down", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

  }
}
