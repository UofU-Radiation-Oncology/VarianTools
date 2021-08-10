namespace VarianTools
{
  partial class MultiContourSelect
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.ItemListBox = new System.Windows.Forms.ListBox();
      this.OkayButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // ItemListBox
      // 
      this.ItemListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ItemListBox.FormattingEnabled = true;
      this.ItemListBox.ItemHeight = 31;
      this.ItemListBox.Location = new System.Drawing.Point(12, 43);
      this.ItemListBox.Name = "ItemListBox";
      this.ItemListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.ItemListBox.Size = new System.Drawing.Size(429, 810);
      this.ItemListBox.TabIndex = 0;
      // 
      // OkayButton
      // 
      this.OkayButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.OkayButton.Location = new System.Drawing.Point(147, 873);
      this.OkayButton.Name = "OkayButton";
      this.OkayButton.Size = new System.Drawing.Size(146, 75);
      this.OkayButton.TabIndex = 1;
      this.OkayButton.Text = "Okay";
      this.OkayButton.UseVisualStyleBackColor = true;
      this.OkayButton.Click += new System.EventHandler(this.OkayButton_Click);
      // 
      // MultiContourSelect
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(442, 960);
      this.Controls.Add(this.OkayButton);
      this.Controls.Add(this.ItemListBox);
      this.Name = "MultiContourSelect";
      this.Text = "MultiContourSelect";
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button OkayButton;
    public System.Windows.Forms.ListBox ItemListBox;
  }
}