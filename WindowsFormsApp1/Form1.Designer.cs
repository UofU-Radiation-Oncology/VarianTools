namespace WindowsFormsApp1
{
  partial class Form1
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.FileDialogButton = new System.Windows.Forms.PictureBox();
      this.label5 = new System.Windows.Forms.Label();
      this.FileTextBox = new System.Windows.Forms.TextBox();
      this.ActionButton = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.FileDialogButton)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.ActionButton)).BeginInit();
      this.SuspendLayout();
      // 
      // FileDialogButton
      // 
      this.FileDialogButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("FileDialogButton.BackgroundImage")));
      this.FileDialogButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.FileDialogButton.Location = new System.Drawing.Point(381, 42);
      this.FileDialogButton.Name = "FileDialogButton";
      this.FileDialogButton.Size = new System.Drawing.Size(30, 25);
      this.FileDialogButton.TabIndex = 146;
      this.FileDialogButton.TabStop = false;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
      this.label5.Location = new System.Drawing.Point(76, 48);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(30, 16);
      this.label5.TabIndex = 145;
      this.label5.Text = "File";
      // 
      // FileTextBox
      // 
      this.FileTextBox.Location = new System.Drawing.Point(112, 47);
      this.FileTextBox.Name = "FileTextBox";
      this.FileTextBox.Size = new System.Drawing.Size(263, 20);
      this.FileTextBox.TabIndex = 144;
      // 
      // ActionButton
      // 
      this.ActionButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ActionButton.BackgroundImage")));
      this.ActionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.ActionButton.Location = new System.Drawing.Point(224, 85);
      this.ActionButton.Name = "ActionButton";
      this.ActionButton.Size = new System.Drawing.Size(30, 30);
      this.ActionButton.TabIndex = 154;
      this.ActionButton.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
      this.label1.Location = new System.Drawing.Point(109, 23);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(284, 16);
      this.label1.TabIndex = 155;
      this.label1.Text = "Choose an Xmesh to extract from and press go";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
      this.ClientSize = new System.Drawing.Size(486, 140);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.ActionButton);
      this.Controls.Add(this.FileDialogButton);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.FileTextBox);
      this.Name = "Form1";
      this.Text = "Form1";
      ((System.ComponentModel.ISupportInitialize)(this.FileDialogButton)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.ActionButton)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox FileDialogButton;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox FileTextBox;
    private System.Windows.Forms.PictureBox ActionButton;
    private System.Windows.Forms.Label label1;
  }
}

