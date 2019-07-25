namespace VarianTools
{
  partial class StructureSelectorForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StructureSelectorForm));
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.CancelButton = new System.Windows.Forms.Button();
      this.OkayButton = new System.Windows.Forms.Button();
      this.StructureComboBox = new System.Windows.Forms.ComboBox();
      this.PromptLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
      this.pictureBox1.Location = new System.Drawing.Point(30, 8);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(17, 17);
      this.pictureBox1.TabIndex = 14;
      this.pictureBox1.TabStop = false;
      // 
      // CancelButton
      // 
      this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CancelButton.Location = new System.Drawing.Point(33, 59);
      this.CancelButton.Name = "CancelButton";
      this.CancelButton.Size = new System.Drawing.Size(75, 31);
      this.CancelButton.TabIndex = 13;
      this.CancelButton.Text = "cancel";
      this.CancelButton.UseVisualStyleBackColor = true;
      this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
      // 
      // OkayButton
      // 
      this.OkayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.OkayButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.OkayButton.Location = new System.Drawing.Point(114, 59);
      this.OkayButton.Name = "OkayButton";
      this.OkayButton.Size = new System.Drawing.Size(75, 31);
      this.OkayButton.TabIndex = 12;
      this.OkayButton.Text = "okay";
      this.OkayButton.UseVisualStyleBackColor = true;
      this.OkayButton.Click += new System.EventHandler(this.OkayButton_Click);
      // 
      // StructureComboBox
      // 
      this.StructureComboBox.FormattingEnabled = true;
      this.StructureComboBox.Location = new System.Drawing.Point(18, 32);
      this.StructureComboBox.Name = "StructureComboBox";
      this.StructureComboBox.Size = new System.Drawing.Size(200, 21);
      this.StructureComboBox.TabIndex = 11;
      // 
      // PromptLabel
      // 
      this.PromptLabel.AutoSize = true;
      this.PromptLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.PromptLabel.Location = new System.Drawing.Point(47, 9);
      this.PromptLabel.Name = "PromptLabel";
      this.PromptLabel.Size = new System.Drawing.Size(154, 16);
      this.PromptLabel.TabIndex = 10;
      this.PromptLabel.Text = "Please select a structure";
      // 
      // StructureSelectorForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(231, 106);
      this.Controls.Add(this.pictureBox1);
      this.Controls.Add(this.CancelButton);
      this.Controls.Add(this.OkayButton);
      this.Controls.Add(this.StructureComboBox);
      this.Controls.Add(this.PromptLabel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "StructureSelectorForm";
      this.Text = "StructureSelectorForm";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button CancelButton;
    private System.Windows.Forms.Button OkayButton;
    public System.Windows.Forms.ComboBox StructureComboBox;
    public System.Windows.Forms.Label PromptLabel;
  }
}