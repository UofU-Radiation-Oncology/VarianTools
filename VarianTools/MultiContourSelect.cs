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
  public partial class MultiContourSelect : Form
  {
    public MultiContourSelect(StructureSet ss)
    {
      InitializeComponent();
      foreach(var s in ss.Structures)
        ItemListBox.Items.Add(s.Id);
    }

    private void OkayButton_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }
  }
}
