using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CnSharp.Updater;

namespace CnSharp.VisualStudio.SharpDeploy.Forms
{
    public partial class ManifestGrid : UserControl
    {
        public ManifestGrid()
        {
            InitializeComponent();
        }

        private Manifest _manifest;
        private int _rootLength;
        private bool _fileVersionGot;

        public event EventHandler OnSelectedRowsChanged;

        public void Bind(Manifest manifest,string baseDir,IEnumerable<FileListItem> items)
        {
            _manifest = manifest;
            _rootLength = baseDir.Length;
            items.ToList().ForEach(item =>
            {
                if (Directory.Exists(item.Dir))
                {
                    var folderName = "[" + item.Dir.Substring(_rootLength) + "]";
                    gridFileList.Rows.Add(item.Selected, folderName, "-", "-");
                    var folderRow = gridFileList.Rows[gridFileList.Rows.Count - 1];
                    folderRow.Cells[ColFileVersion.Name].ReadOnly = true;
                    folderRow.Tag = item;
                }
                else
                {
                    var shortName = item.Dir.Substring(_rootLength);

                    var fi = new FileInfo(item.Dir);
                    var ext = fi.Extension.ToLower();
                    var version = string.Empty;
                    if (ext == ".exe" || ext == ".dll")
                    {
                        version = FileVersionInfo.GetVersionInfo(item.Dir).FileVersion;
                    }
                    gridFileList.Rows.Add(item.Selected,shortName, fi.Length, version);
              

                    var lastRow = gridFileList.Rows[gridFileList.Rows.Count - 1];
                    lastRow.Tag = item;

                    if (!string.IsNullOrEmpty(version))
                    {
                        lastRow.Cells[ColFileVersion.Name].ReadOnly = true;
                        lastRow.DefaultCellStyle.ForeColor = Color.Gray;
                    }
                }
            });

                GetFilesVersion();
        }

        private void GetFilesVersion()
        {
            if (_fileVersionGot)
                return;
            foreach (DataGridViewRow gr in gridFileList.Rows)
            {
                var fileName = gr.Cells[ColFileName.Name].Value.ToString().ToLower();
                if (fileName.EndsWith(".exe") || fileName.EndsWith(".dll") || fileName.StartsWith("["))
                    continue;
                var got = false;
              
                foreach (var file in _manifest.Files)
                {
                    if (string.Compare(file.FileName, fileName, true) == 0 && !string.IsNullOrEmpty(file.Version))
                    {
                        gr.Cells[ColFileVersion.Name].Value = file.Version;
                        got = true;
                        break;
                    }
                }
                if (!got)
                {
                    gr.Cells[ColFileVersion.Name].Value = _manifest.Version;
                }
            }
            _fileVersionGot = true;
        }


        private void gridFileList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridFileList.CurrentCell != null && gridFileList.CurrentCell.ReadOnly)
                return;
            if (e.ColumnIndex != 0) return;
            var dir = GetDir(gridFileList.CurrentRow.Index);
            var selected = !Convert.ToBoolean(gridFileList.CurrentCell.Value);
            if (!Directory.Exists(dir)) //find folder
            {
                var dirName = Path.GetDirectoryName(dir);
                var folderIndex = -1;
                var i = e.RowIndex;
                var otherSelected = false;
                while (i > 0)
                {
                    i--;
                    var folder = GetDir(i);
                    if (!Directory.Exists(folder))
                    {
                        if (Path.GetDirectoryName(folder) != dirName)
                            return;
                        if (!otherSelected &&
                            Convert.ToBoolean(gridFileList.Rows[i].Cells[ColSelect.Name].Value))
                            otherSelected = true;
                        continue;
                    }
                    if (folder == dirName)
                    {
                        folderIndex = i;
                        break;
                    }
                }
                if (folderIndex < 0)
                    return;
                if (selected || otherSelected)
                {
                    gridFileList.Rows[folderIndex].Cells[ColSelect.Name].Value = true;
                    return;
                }
                i = e.RowIndex;
                while (i < gridFileList.Rows.Count - 1)
                {
                    i++;
                    var subling = GetDir(i);
                    if (Directory.Exists(subling) || Path.GetDirectoryName(subling) != dirName)
                    {
                        break;
                    }
                    if (Convert.ToBoolean(gridFileList.Rows[i].Cells[ColSelect.Name].Value))
                    {
                        gridFileList.Rows[folderIndex].Cells[ColSelect.Name].Value = true;
                        return;
                    }
                }
                gridFileList.Rows[folderIndex].Cells[ColSelect.Name].Value = false;
            }
            else //find files
            {
                for (var i = e.RowIndex + 1; i < gridFileList.Rows.Count; i++)
                {
                    var fileDir = GetDir(i);
                    if (!fileDir.StartsWith(dir))
                        break;
                    gridFileList.Rows[i].Cells[0].Value = selected;
                }
            }
           OnSelectedRowsChanged?.Invoke(this,EventArgs.Empty);
        }

        private string GetDir(int i)
        {
            return (gridFileList.Rows[i].Tag as FileListItem).Dir;
        }

        public bool ValidateSelection()
        {
            var i = 0;
            foreach (DataGridViewRow gr in gridFileList.Rows)
            {
                gr.DefaultCellStyle.BackColor = SystemColors.Window;
                var selected = Convert.ToBoolean(gr.Cells[ColSelect.Name].Value);
                if (!selected)
                    continue;
                var fileName = gr.Cells[ColFileName.Name].Value.ToString();
                if (!IsFile(fileName))
                    continue;

                i++;
                if ((gr.Cells[ColFileVersion.Name].Value == null ||
                     gr.Cells[ColFileVersion.Name].Value.ToString().Trim().Length == 0))
                {
                    Common.ShowError(gr.Cells[ColFileName.Name].Value + "  version number missed.");
                    gridFileList.FirstDisplayedScrollingRowIndex = gr.Index;
                    gr.DefaultCellStyle.BackColor = Color.Yellow;
                    return false;
                }
            }
            if (i == 0)
            {
                Common.ShowError("None of the files is selected.");
                return false;
            }
            return true;
        }

        public IEnumerable<string> GetExcludedFiles()
        {
            gridFileList.EndEdit();
            foreach (DataGridViewRow gr in gridFileList.Rows)
            {
                var fileName = gr.Cells[ColFileName.Name].Value.ToString();
                if (!IsFile(fileName))
                    continue;
                if (!Convert.ToBoolean(gr.Cells[ColSelect.Name].Value))
                    yield return fileName;
            }
        }

        public IEnumerable<string> GetExcludedFolders()
        {
            gridFileList.EndEdit();
            foreach (DataGridViewRow gr in gridFileList.Rows)
            {
                var fileName = gr.Cells[ColFileName.Name].Value.ToString();
                if (IsFile(fileName))
                    continue;
                if (!Convert.ToBoolean(gr.Cells[ColSelect.Name].Value))
                    yield return fileName.TrimStart('[').TrimEnd(']');
            }
        }

        protected bool IsFile(string cellText)
        {
            return !cellText.StartsWith("[");
        }

        public IEnumerable<string> GetSelectedFiles(string ext = null)
        {
            gridFileList.EndEdit();
            foreach (DataGridViewRow gr in gridFileList.Rows)
            {
                if (Convert.ToBoolean(gr.Cells[ColSelect.Name].Value))
                {
                    var file = gr.Cells[ColFileName.Name].Value.ToString();

                    if (IsFile(file) && (string.IsNullOrWhiteSpace(ext) || file.EndsWith(ext, StringComparison.CurrentCultureIgnoreCase)))
                        yield return file;
                }
            }
        }

        public IEnumerable<ReleaseFile> GetReleaseFiles()
        {
            gridFileList.EndEdit();
            foreach (DataGridViewRow gr in gridFileList.Rows)
            {
                if (Convert.ToBoolean(gr.Cells[ColSelect.Name].Value))
                {
                    var file = gr.Cells[ColFileName.Name].Value.ToString();

                    if (IsFile(file))
                        yield return new ReleaseFile
                        {
                            FileName = file,
                            FileSize = long.Parse(gr.Cells[ColSize.Name].Value.ToString()),
                            Version = gr.Cells[ColFileVersion.Name].Value.ToString()
                        };
                }
            }
        }

        public IEnumerable<FileListItem> GetSelectedItems()
        {
            gridFileList.EndEdit();
            foreach (DataGridViewRow gr in gridFileList.Rows)
            {
                if (Convert.ToBoolean(gr.Cells[ColSelect.Name].Value))
                {
                    var file = gr.Cells[ColFileName.Name].Value.ToString();

                    if (IsFile(file))
                    {
                        var item = gr.Tag as FileListItem;
                        item.RelativeFileName = file;
                        yield return item;
                    }
                }
            }
        }
    }
}
