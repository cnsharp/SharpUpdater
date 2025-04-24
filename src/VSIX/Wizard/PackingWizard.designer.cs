﻿namespace CnSharp.VisualStudio.SharpUpdater.Wizard
{
    partial class PackingWizard
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PackingWizard));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.sourceBox = new System.Windows.Forms.ComboBox();
            this.stepWizardControl = new AeroWizard.StepWizardControl();
            this.wizardPageVersion = new AeroWizard.WizardPage();
            this.chkNoBuild = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.projectGrid = new System.Windows.Forms.DataGridView();
            this.chkSame = new System.Windows.Forms.CheckBox();
            this.wizardPageManifest = new AeroWizard.WizardPage();
            this.targetBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.exeBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gbFiles = new System.Windows.Forms.GroupBox();
            this.manifestGrid = new CnSharp.VisualStudio.SharpUpdater.Wizard.ManifestGrid();
            this.wizardPagePackage = new AeroWizard.WizardPage();
            this._productInfoControl = new CnSharp.VisualStudio.SharpUpdater.Wizard.ProductInfoControl();
            this.wizardPageDeploy = new AeroWizard.WizardPage();
            this.chkOpenDir = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtOutputDir = new System.Windows.Forms.TextBox();
            this.btnOpenOutputDir = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.projectBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.ColProductName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColVersionHidden = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.stepWizardControl)).BeginInit();
            this.wizardPageVersion.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectGrid)).BeginInit();
            this.wizardPageManifest.SuspendLayout();
            this.gbFiles.SuspendLayout();
            this.wizardPagePackage.SuspendLayout();
            this.wizardPageDeploy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 199);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Deploy Server";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 285);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "API Key";
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(42, 333);
            this.txtKey.Name = "txtKey";
            this.txtKey.PasswordChar = '*';
            this.txtKey.Size = new System.Drawing.Size(429, 23);
            this.txtKey.TabIndex = 5;
            this.txtKey.Validating += new System.ComponentModel.CancelEventHandler(this.txtKey_Validating);
            this.txtKey.Validated += new System.EventHandler(this.txtKey_Validated);
            // 
            // sourceBox
            // 
            this.sourceBox.FormattingEnabled = true;
            this.sourceBox.Location = new System.Drawing.Point(42, 229);
            this.sourceBox.Name = "sourceBox";
            this.sourceBox.Size = new System.Drawing.Size(429, 25);
            this.sourceBox.TabIndex = 4;
            // 
            // stepWizardControl
            // 
            this.stepWizardControl.Location = new System.Drawing.Point(0, 0);
            this.stepWizardControl.Name = "stepWizardControl";
            this.stepWizardControl.Pages.Add(this.wizardPageVersion);
            this.stepWizardControl.Pages.Add(this.wizardPageManifest);
            this.stepWizardControl.Pages.Add(this.wizardPagePackage);
            this.stepWizardControl.Pages.Add(this.wizardPageDeploy);
            this.stepWizardControl.Size = new System.Drawing.Size(942, 672);
            this.stepWizardControl.StepListFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.stepWizardControl.TabIndex = 27;
            this.stepWizardControl.Title = "Pack / Deploy";
            this.stepWizardControl.TitleIcon = ((System.Drawing.Icon)(resources.GetObject("stepWizardControl.TitleIcon")));
            // 
            // wizardPageVersion
            // 
            this.wizardPageVersion.AllowBack = false;
            this.wizardPageVersion.Controls.Add(this.chkNoBuild);
            this.wizardPageVersion.Controls.Add(this.groupBox1);
            this.wizardPageVersion.Controls.Add(this.chkSame);
            this.wizardPageVersion.Name = "wizardPageVersion";
            this.wizardPageVersion.NextPage = this.wizardPageManifest;
            this.wizardPageVersion.Size = new System.Drawing.Size(744, 516);
            this.stepWizardControl.SetStepText(this.wizardPageVersion, "Version Number");
            this.wizardPageVersion.TabIndex = 2;
            this.wizardPageVersion.Text = "Version Number";
            // 
            // chkNoBuild
            // 
            this.chkNoBuild.AutoSize = true;
            this.chkNoBuild.Location = new System.Drawing.Point(30, 442);
            this.chkNoBuild.Name = "chkNoBuild";
            this.chkNoBuild.Size = new System.Drawing.Size(78, 21);
            this.chkNoBuild.TabIndex = 11;
            this.chkNoBuild.Text = "No Build";
            this.chkNoBuild.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.projectGrid);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(744, 424);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assemblies";
            // 
            // projectGrid
            // 
            this.projectGrid.AllowUserToAddRows = false;
            this.projectGrid.AllowUserToDeleteRows = false;
            this.projectGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.projectGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.projectGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColProductName,
            this.ColVersion,
            this.ColVersionHidden});
            this.projectGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectGrid.Location = new System.Drawing.Point(3, 19);
            this.projectGrid.Name = "projectGrid";
            this.projectGrid.RowTemplate.Height = 23;
            this.projectGrid.Size = new System.Drawing.Size(738, 402);
            this.projectGrid.TabIndex = 0;
            // 
            // chkSame
            // 
            this.chkSame.AutoSize = true;
            this.chkSame.Location = new System.Drawing.Point(146, 442);
            this.chkSame.Name = "chkSame";
            this.chkSame.Size = new System.Drawing.Size(218, 21);
            this.chkSame.TabIndex = 9;
            this.chkSame.Text = "Same Version as the First Project";
            this.chkSame.UseVisualStyleBackColor = true;
            this.chkSame.CheckedChanged += new System.EventHandler(this.chkSame_CheckedChanged);
            // 
            // wizardPageManifest
            // 
            this.wizardPageManifest.Controls.Add(this.targetBox);
            this.wizardPageManifest.Controls.Add(this.label4);
            this.wizardPageManifest.Controls.Add(this.exeBox);
            this.wizardPageManifest.Controls.Add(this.label3);
            this.wizardPageManifest.Controls.Add(this.gbFiles);
            this.wizardPageManifest.Name = "wizardPageManifest";
            this.wizardPageManifest.NextPage = this.wizardPagePackage;
            this.wizardPageManifest.Size = new System.Drawing.Size(744, 518);
            this.stepWizardControl.SetStepText(this.wizardPageManifest, "Manifest");
            this.wizardPageManifest.TabIndex = 5;
            this.wizardPageManifest.Text = "Manifest";
            // 
            // targetBox
            // 
            this.targetBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetBox.FormattingEnabled = true;
            this.targetBox.Location = new System.Drawing.Point(143, 443);
            this.targetBox.Name = "targetBox";
            this.targetBox.Size = new System.Drawing.Size(184, 25);
            this.targetBox.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 446);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(115, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "Target Framework";
            // 
            // exeBox
            // 
            this.exeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exeBox.FormattingEnabled = true;
            this.exeBox.Location = new System.Drawing.Point(521, 444);
            this.exeBox.Name = "exeBox";
            this.exeBox.Size = new System.Drawing.Size(184, 25);
            this.exeBox.TabIndex = 1;
            this.exeBox.Validating += new System.ComponentModel.CancelEventHandler(this.exeBox_Validating);
            this.exeBox.Validated += new System.EventHandler(this.exeBox_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(387, 449);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Application Entry(*)";
            // 
            // gbFiles
            // 
            this.gbFiles.Controls.Add(this.manifestGrid);
            this.gbFiles.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbFiles.Location = new System.Drawing.Point(0, 0);
            this.gbFiles.Name = "gbFiles";
            this.gbFiles.Size = new System.Drawing.Size(744, 429);
            this.gbFiles.TabIndex = 1;
            this.gbFiles.TabStop = false;
            this.gbFiles.Text = "Files";
            // 
            // manifestGrid
            // 
            this.manifestGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manifestGrid.Location = new System.Drawing.Point(3, 19);
            this.manifestGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.manifestGrid.Name = "manifestGrid";
            this.manifestGrid.Size = new System.Drawing.Size(738, 407);
            this.manifestGrid.TabIndex = 0;
            // 
            // wizardPagePackage
            // 
            this.wizardPagePackage.Controls.Add(this._productInfoControl);
            this.wizardPagePackage.Name = "wizardPagePackage";
            this.wizardPagePackage.NextPage = this.wizardPageDeploy;
            this.wizardPagePackage.Size = new System.Drawing.Size(744, 518);
            this.stepWizardControl.SetStepText(this.wizardPagePackage, "Product Info");
            this.wizardPagePackage.TabIndex = 3;
            this.wizardPagePackage.Text = "Product Info";
            // 
            // _productInfoControl
            // 
            this._productInfoControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._productInfoControl.Location = new System.Drawing.Point(0, 0);
            this._productInfoControl.Manifest = null;
            this._productInfoControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._productInfoControl.Name = "_productInfoControl";
            this._productInfoControl.Size = new System.Drawing.Size(744, 518);
            this._productInfoControl.TabIndex = 0;
            // 
            // wizardPageDeploy
            // 
            this.wizardPageDeploy.Controls.Add(this.chkOpenDir);
            this.wizardPageDeploy.Controls.Add(this.label10);
            this.wizardPageDeploy.Controls.Add(this.txtOutputDir);
            this.wizardPageDeploy.Controls.Add(this.btnOpenOutputDir);
            this.wizardPageDeploy.Controls.Add(this.label1);
            this.wizardPageDeploy.Controls.Add(this.sourceBox);
            this.wizardPageDeploy.Controls.Add(this.label2);
            this.wizardPageDeploy.Controls.Add(this.txtKey);
            this.wizardPageDeploy.IsFinishPage = true;
            this.wizardPageDeploy.Name = "wizardPageDeploy";
            this.wizardPageDeploy.Size = new System.Drawing.Size(744, 518);
            this.stepWizardControl.SetStepText(this.wizardPageDeploy, "Deploy");
            this.wizardPageDeploy.TabIndex = 4;
            this.wizardPageDeploy.Text = "Deploy";
            // 
            // chkOpenDir
            // 
            this.chkOpenDir.AutoSize = true;
            this.chkOpenDir.Checked = true;
            this.chkOpenDir.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOpenDir.Location = new System.Drawing.Point(42, 134);
            this.chkOpenDir.Name = "chkOpenDir";
            this.chkOpenDir.Size = new System.Drawing.Size(221, 21);
            this.chkOpenDir.TabIndex = 24;
            this.chkOpenDir.Text = "Open output directory after build";
            this.chkOpenDir.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(39, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(158, 17);
            this.label10.TabIndex = 22;
            this.label10.Text = "Package Output Directory";
            // 
            // txtOutputDir
            // 
            this.txtOutputDir.Location = new System.Drawing.Point(42, 74);
            this.txtOutputDir.Name = "txtOutputDir";
            this.txtOutputDir.Size = new System.Drawing.Size(429, 23);
            this.txtOutputDir.TabIndex = 21;
            // 
            // btnOpenOutputDir
            // 
            this.btnOpenOutputDir.FlatAppearance.BorderSize = 0;
            this.btnOpenOutputDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenOutputDir.Location = new System.Drawing.Point(493, 74);
            this.btnOpenOutputDir.Name = "btnOpenOutputDir";
            this.btnOpenOutputDir.Size = new System.Drawing.Size(23, 23);
            this.btnOpenOutputDir.TabIndex = 23;
            this.btnOpenOutputDir.UseVisualStyleBackColor = true;
            this.btnOpenOutputDir.Click += new System.EventHandler(this.btnOpenOutputDir_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // ColProductName
            // 
            this.ColProductName.FillWeight = 200F;
            this.ColProductName.HeaderText = "Project Name";
            this.ColProductName.Name = "ColProductName";
            this.ColProductName.ReadOnly = true;
            // 
            // ColVersion
            // 
            this.ColVersion.HeaderText = "Version";
            this.ColVersion.Name = "ColVersion";
            // 
            // ColVersionHidden
            // 
            this.ColVersionHidden.HeaderText = "ColVersionHidden";
            this.ColVersionHidden.Name = "ColVersionHidden";
            this.ColVersionHidden.Visible = false;
            // 
            // PackingWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(942, 672);
            this.Controls.Add(this.stepWizardControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PackingWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PackingWizard";
            this.Load += new System.EventHandler(this.DeployWizard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.stepWizardControl)).EndInit();
            this.wizardPageVersion.ResumeLayout(false);
            this.wizardPageVersion.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.projectGrid)).EndInit();
            this.wizardPageManifest.ResumeLayout(false);
            this.wizardPageManifest.PerformLayout();
            this.gbFiles.ResumeLayout(false);
            this.wizardPagePackage.ResumeLayout(false);
            this.wizardPageDeploy.ResumeLayout(false);
            this.wizardPageDeploy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.projectBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.ComboBox sourceBox;
        private AeroWizard.StepWizardControl stepWizardControl;
        private AeroWizard.WizardPage wizardPageVersion;
        private AeroWizard.WizardPage wizardPagePackage;
        private AeroWizard.WizardPage wizardPageDeploy;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private AeroWizard.WizardPage wizardPageManifest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView projectGrid;
        private System.Windows.Forms.CheckBox chkSame;
        private System.Windows.Forms.GroupBox gbFiles;
        private System.Windows.Forms.CheckBox chkOpenDir;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtOutputDir;
        private System.Windows.Forms.Button btnOpenOutputDir;
        private System.Windows.Forms.BindingSource projectBindingSource;
        private ProductInfoControl _productInfoControl;
        private ManifestGrid manifestGrid;
        private System.Windows.Forms.ComboBox exeBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox targetBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkNoBuild;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColProductName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColVersionHidden;
    }
}