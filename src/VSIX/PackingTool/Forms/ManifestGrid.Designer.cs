namespace CnSharp.VisualStudio.SharpDeploy.Forms
{
    partial class ManifestGrid
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridFileList = new System.Windows.Forms.DataGridView();
            this.ColSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColFileVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridFileList)).BeginInit();
            this.SuspendLayout();
            // 
            // gridFileList
            // 
            this.gridFileList.AllowUserToAddRows = false;
            this.gridFileList.AllowUserToDeleteRows = false;
            this.gridFileList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridFileList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFileList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColSelect,
            this.ColFileName,
            this.ColSize,
            this.ColFileVersion});
            this.gridFileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridFileList.Location = new System.Drawing.Point(0, 0);
            this.gridFileList.Name = "gridFileList";
            this.gridFileList.RowHeadersVisible = false;
            this.gridFileList.RowTemplate.Height = 23;
            this.gridFileList.Size = new System.Drawing.Size(734, 494);
            this.gridFileList.TabIndex = 17;
            this.gridFileList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridFileList_CellClick);
            // 
            // ColSelect
            // 
            this.ColSelect.FillWeight = 10F;
            this.ColSelect.HeaderText = "Select";
            this.ColSelect.Name = "ColSelect";
            this.ColSelect.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColSelect.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ColFileName
            // 
            this.ColFileName.FillWeight = 50F;
            this.ColFileName.HeaderText = "Name";
            this.ColFileName.Name = "ColFileName";
            this.ColFileName.ReadOnly = true;
            // 
            // ColSize
            // 
            this.ColSize.FillWeight = 20F;
            this.ColSize.HeaderText = "File Size(K)";
            this.ColSize.Name = "ColSize";
            this.ColSize.ReadOnly = true;
            // 
            // ColFileVersion
            // 
            this.ColFileVersion.FillWeight = 20F;
            this.ColFileVersion.HeaderText = "Version";
            this.ColFileVersion.Name = "ColFileVersion";
            // 
            // ManifestGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridFileList);
            this.Name = "ManifestGrid";
            this.Size = new System.Drawing.Size(734, 494);
            ((System.ComponentModel.ISupportInitialize)(this.gridFileList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridFileList;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColFileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColFileVersion;
    }
}
