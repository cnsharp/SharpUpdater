using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CnSharp.Windows.Updater
{
	partial class UpdateForm
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
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(UpdateForm));
            panel1 = new Panel();
            boxDes = new RichTextBox();
            btnCancel = new Button();
            btnUpgrade = new Button();
            lblSize = new Label();
            progressBar = new ProgressBar();
            lblTitle = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(boxDes);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnUpgrade);
            panel1.Controls.Add(lblSize);
            panel1.Controls.Add(progressBar);
            panel1.Controls.Add(lblTitle);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // boxDes
            // 
            boxDes.BackColor = SystemColors.Window;
            boxDes.BorderStyle = BorderStyle.FixedSingle;
            resources.ApplyResources(boxDes, "boxDes");
            boxDes.Name = "boxDes";
            boxDes.ReadOnly = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(btnCancel, "btnCancel");
            btnCancel.Name = "btnCancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnUpgrade
            // 
            resources.ApplyResources(btnUpgrade, "btnUpgrade");
            btnUpgrade.Name = "btnUpgrade";
            btnUpgrade.UseVisualStyleBackColor = true;
            btnUpgrade.Click += btnUpgrade_Click;
            // 
            // lblSize
            // 
            resources.ApplyResources(lblSize, "lblSize");
            lblSize.Name = "lblSize";
            // 
            // progressBar
            // 
            resources.ApplyResources(progressBar, "progressBar");
            progressBar.Name = "progressBar";
            // 
            // lblTitle
            // 
            resources.ApplyResources(lblTitle, "lblTitle");
            lblTitle.Name = "lblTitle";
            // 
            // UpdateForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UpdateForm";
            FormClosing += FormUpdate_FormClosing;
            Load += FormUpdate_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Button btnUpgrade;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.RichTextBox boxDes;
		private System.Windows.Forms.Panel panel1;
	}
}