namespace VarianTools
{
    partial class PlanSumSelectorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlanSumSelectorForm));
            this.label1 = new System.Windows.Forms.Label();
            this.PlanSumComboBox = new System.Windows.Forms.ComboBox();
            this.OkayButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(39, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(343, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Multiple plan sums detected: please select planning item";
            // 
            // PlanSumComboBox
            // 
            this.PlanSumComboBox.FormattingEnabled = true;
            this.PlanSumComboBox.Location = new System.Drawing.Point(83, 36);
            this.PlanSumComboBox.Name = "PlanSumComboBox";
            this.PlanSumComboBox.Size = new System.Drawing.Size(247, 21);
            this.PlanSumComboBox.TabIndex = 1;
            // 
            // OkayButton
            // 
            this.OkayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OkayButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OkayButton.Location = new System.Drawing.Point(208, 66);
            this.OkayButton.Name = "OkayButton";
            this.OkayButton.Size = new System.Drawing.Size(75, 31);
            this.OkayButton.TabIndex = 2;
            this.OkayButton.Text = "okay";
            this.OkayButton.UseVisualStyleBackColor = true;
            this.OkayButton.Click += new System.EventHandler(this.OkayButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.Location = new System.Drawing.Point(127, 66);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 31);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(17, 17);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // PlanSumSelectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(413, 101);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OkayButton);
            this.Controls.Add(this.PlanSumComboBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PlanSumSelectorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "      ";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button OkayButton;
        private System.Windows.Forms.Button CancelButton;
        public System.Windows.Forms.ComboBox PlanSumComboBox;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}