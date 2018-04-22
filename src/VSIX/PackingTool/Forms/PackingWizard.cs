using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AeroWizard;
using CnSharp.Updater;
using CnSharp.Updater.Packaging;
using CnSharp.Updater.Util;
using CnSharp.VisualStudio.Extensions;
using CnSharp.VisualStudio.Extensions.Projects;
using CnSharp.VisualStudio.SharpDeploy.Util;
using EnvDTE;
using EnvDTE80;
using Process = System.Diagnostics.Process;

namespace CnSharp.VisualStudio.SharpDeploy.Forms
{
    public partial class PackingWizard : Form
    {
        private string _manifestFile;
        private NuPackSettings _settings;
        private Project _startProject;
        private ProjectAssemblyInfo _assemblyInfo;
        private string _buildDir;
        private Manifest _manifest;
        private ManifestGatherer _manifestGatherer;
        private string _outputDir;
        private string _projectDir;
        private string _versionFolder;
        private string _zipPath;


        public PackingWizard()
        {
            InitializeComponent();

            ActiveControl = projectGrid;
            BindTextBoxEvents();
            projectGrid.AutoGenerateColumns = false;
            
            BindProjects();
            BindSettings();

            stepWizardControl.SelectedPageChanged += StepWizardControl_SelectedPageChanged;
            stepWizardControl.Finished += StepWizardControl_Finished;
            wizardPageVersion.Commit += WizardPageCommit;
            wizardPageManifest.Commit += WizardPageCommit;
            wizardPagePackage.Commit += WizardPageCommit;
        }

        void BindSettings()
        {
            var cacheHelper = new CacheHelper<NuPackSettings>();
            var settingFile = Paths.GetNuPackSettingsLocation(_startProject);
            if (File.Exists(settingFile))
                _settings = cacheHelper.Get(settingFile);
            else
            {
                _settings = new NuPackSettings();
            }
            txtOutputDir.Text = _settings.PackageOutputDirectory;
            chkOpenDir.Checked = _settings.OpenPackageOutputDirectoryAfterBuild;

        }

        void BindProjects()
        {
            var dte2 = Host.Instance.Dte2;
            _startProject = dte2.GetActiveProejct();
            _projectDir = _startProject.GetDirectory();

            _manifestFile = $"{_projectDir}\\{Manifest.ManifestFileName}";
            _manifest = new Manifest();
            _manifest.EntryPoint = _startProject.Properties.Item("OutputFileName").Value.ToString();
            exeBox.Text = _manifest.EntryPoint;
            if (File.Exists(_manifestFile))
            {
                _manifest = FileUtil.ReadManifest(_manifestFile);
            }

            var refProjects = dte2.GetSolutionProjects().ToList();
            refProjects.Remove(_startProject);
            projectGrid.Rows.Clear();
            BindProject(_startProject);
            foreach (var project in refProjects)
            {
                BindProject(project);
            }

            chkSame.Visible = refProjects.Any();

        }

        private void BindProject(Project project)
        {
            var ass = project.GetProjectAssemblyInfo();
            projectGrid.Rows.Add(
                project.Name,
                ass != null ? ass.Version : string.Empty
                );
            projectGrid.Rows[projectGrid.Rows.Count - 1].Tag = ass;
            if (project == _startProject)
            {
                projectGrid.Rows[projectGrid.Rows.Count - 1].DefaultCellStyle.Font = new Font(
                    projectGrid.Font.FontFamily,
                    projectGrid.Font.Size,
                    FontStyle.Bold
                    );
                if (ass != null)
                {
                    _manifest.Id = ass.Title;
                    _manifest.AppName = ass.Title;
                    _manifest.Owner = ass.Company;
                    _manifest.Description = ass.Description;
                    _manifest.Copyright = ass.Copyright;
                }
                _assemblyInfo = ass;
            }
        }


        private void WizardPageCommit(object sender, WizardPageConfirmEventArgs e)
        {
            var wp = sender as WizardPage;
            if (Validation.HasValidationErrors(wp.Controls))
            {
                e.Cancel = true;
            }
            if (e.Page == wizardPageVersion)
            {
                SaveAssemblyInfo();
                if (!Build())
                {
                    DialogResult = DialogResult.Abort;
                    e.Cancel = true;
                    return;
                }

                _manifestGatherer = new ManifestGatherer(_buildDir, _projectDir, _settings);
                var items = _manifestGatherer.GatherFiles();
                manifestGrid.Bind(_manifest, _buildDir, items);
                BindBox();
            }
            else if (e.Page == wizardPageManifest)
            {
                if (manifestGrid.ValidateSelection() == false)
                {
                    e.Cancel = true;
                    return;
                }
                _productInfoControl.Manifest = _manifest;
            }
        }


        private void StepWizardControl_SelectedPageChanged(object sender, EventArgs e)
        {
            if (stepWizardControl.SelectedPage == wizardPageVersion)
            {
                projectGrid.Focus();
            }
            else if (stepWizardControl.SelectedPage == wizardPageManifest)
            {
                manifestGrid.Focus();
            }
            else if (stepWizardControl.SelectedPage == wizardPagePackage)
            {
                _productInfoControl.Focus();
            }
            else if (stepWizardControl.SelectedPage == wizardPageDeploy)
            {
                txtOutputDir.Focus();
                txtOutputDir.Text = _settings.PackageOutputDirectory;
                sourceBox.Text = _settings.DeployServer;
                txtKey.Text = _settings.DeployKey;
                chkOpenDir.Checked = _settings.OpenPackageOutputDirectoryAfterBuild;
            }
        }

        private void StepWizardControl_Finished(object sender, EventArgs e)
        {
            _manifest.ReleaseUrl = sourceBox.Text.Trim();
            DialogResult = DialogResult.OK;
        }


        private void BindTextBoxEvents()
        {
            txtOutputDir.Validating += TxtOutputDir_Validating;
            txtOutputDir.Validated += TextBoxValidated;
            sourceBox.Validating += SourceBoxOnValidating;
            sourceBox.Validated += TextBoxValidated;
        }

        private void SourceBoxOnValidating(object sender, CancelEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null)
                return;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                errorProvider.SetError(box, "*");
                e.Cancel = true;
                return;
            }
            var regex = @"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?";
            if (!Regex.IsMatch(box.Text, regex))
            {
                errorProvider.SetError(box, "Invalid URL");
                e.Cancel = true;
            }
        }

        private void TxtOutputDir_Validating(object sender, CancelEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null)
                return;
            if (box.Text.Contains(":") && !Directory.Exists(box.Text.Trim()))
            {
                errorProvider.SetError(box, "Directory not found.");
                e.Cancel = true;
                return;
            }
            var dir = box.Text.Replace("/", "\\");
            if (!dir.EndsWith("\\"))
                dir += "\\";
            if (dir.StartsWith("bin\\release\\", StringComparison.InvariantCultureIgnoreCase) ||
                dir.StartsWith(_buildDir, StringComparison.InvariantCultureIgnoreCase))
            {
                errorProvider.SetError(box,"Cannot be a sub directory under build directory.");
                e.Cancel = true;
            }
        }


        private void TextBoxValidated(object sender, EventArgs e)
        {
            var box = sender as TextBox;
            if (box == null)
                return;
            errorProvider.SetError(box, null);
        }


        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            var box = sender as TextBox;
            if (box != null && box.BackColor == SystemColors.Info)
                box.BackColor = SystemColors.Window;
        }

        private void DeployWizard_Load(object sender, EventArgs e)
        {
            SetControls();
        }


        private void SetControls()
        {
            _productInfoControl.ProjectAssemblyInfo = _assemblyInfo;
        }


        private void btnOpenOutputDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                txtOutputDir.Text = folderBrowserDialog.SelectedPath;
        }

        public void Packing()
        {
            _outputDir = EnsureOutputDir(_projectDir, txtOutputDir.Text);
            var manifestFile = SaveManifest();
            CopyFiles();
            File.Copy(manifestFile, string.Concat(_versionFolder, "\\", Manifest.ManifestFileName));


            var zipName = $"{_manifest.AppName}_{_manifest.Version}{Manifest.PackageFileExt}";
            _zipPath = $"{_outputDir}\\{zipName}";

            //if(File.Exists(_zipPath))
            //    File.Delete(_zipPath);
            //ZipFile.CreateFromDirectory(_versionFolder,_zipPath);


            var pb = new PackageBuilder(_manifest, _versionFolder);
            pb.CreatePackage(_zipPath);

            SaveProjectSettings();

            Deploy();

            if (chkOpenDir.Checked)
                Process.Start(_outputDir);
        }

        private void Deploy()
        {
            var url = sourceBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
                return;
            using (var wc = new WebClient())
            {
                try
                {
                    wc.Headers.Add(Common.ApiKeyHeader, txtKey.Text);
                    wc.UploadFile(url, "PUT", _zipPath);
                }
                catch(Exception ex)
                {
                    var msg = $"Upload package to {url} failed.";
                    Common.ShowError(msg);
                    msg += $"Exception:{ex.Message}";
                    Host.Instance.Dte2.OutputMessage(Common.ProductName,msg);
                }
            }
        }


        private string SaveManifest()
        {
            _manifest = _productInfoControl.Manifest;
            _manifest.EntryPoint = exeBox.Text;
            _manifest.ShortcutIcon = icoBox.Text;
            _manifest.ReleaseDate = DateTime.Now.ToString();
            _manifest.Files = manifestGrid.GetReleaseFiles().ToList();

            var file = string.Concat(_outputDir, "\\", Manifest.ManifestFileName);
            _manifest.Save(file);

            var projectFile = string.Concat(_projectDir, "\\", Manifest.ManifestFileName);
          
            if (File.Exists(projectFile))
            {
                if(Common.CheckTfs(_startProject))
                    Host.Instance.SourceControl.CheckOut(Path.GetDirectoryName(Host.Instance.DTE.Solution.FullName),projectFile);
                File.Copy(file, projectFile, true);
            }
            else
            {
                _startProject.ProjectItems.AddFromFileCopy(file);
            }
            return file;
        }

        private void SaveProjectSettings()
        {
            _settings.UnselectedFolders = manifestGrid.GetExcludedFolders().ToList();
            _settings.UnselectedFiles = manifestGrid.GetExcludedFiles().ToList();
            _settings.DeployServer = sourceBox.Text;
            _settings.DeployKey = txtKey.Text;
            _settings.PackageOutputDirectory = txtOutputDir.Text.Contains(":") ? _outputDir : txtOutputDir.Text;
            _settings.OpenPackageOutputDirectoryAfterBuild = chkOpenDir.Checked;

            var cacheDir = Paths.GetNuPackSettingsLocation(_startProject);
            var cacheHelper = new CacheHelper<NuPackSettings>();
            cacheHelper.Save(_settings, cacheDir);
        }

        private void CopyFiles()
        {
            _versionFolder = string.Concat(_outputDir, "\\", _manifest.Version);
            if (Directory.Exists(_versionFolder))
            {
                FileUtil.DeleteFolder(_versionFolder);
            }
            Directory.CreateDirectory(_versionFolder);
            foreach (var item in manifestGrid.GetSelectedItems())
            {
                var sourceFile = item.Dir;
                var targetFile = string.Concat(_versionFolder, "\\", item.RelativeFileName);
                var targetDir = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
                File.Copy(sourceFile, targetFile);
            }
        }

        protected string EnsureOutputDir(string baseDir, string outputDir)
        {
            var dir = outputDir.Trim().Replace("/", "\\");
            if (!dir.Contains(":\\"))
            {
                dir = baseDir + "\\" + dir + "\\";
            }

            dir = dir.Replace("{version}", _manifest.Version);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        private void SaveAssemblyInfo()
        {
            projectGrid.EndEdit();
            var i = 0;
            foreach (DataGridViewRow row in projectGrid.Rows)
            {
                var assemblyInfo = row.Tag as ProjectAssemblyInfo;
                var newVersion = row.Cells[1].Value?.ToString().Trim();
                if (string.IsNullOrEmpty(newVersion) && i == 0)
                    return;

                if (assemblyInfo != null)
                {
                    if (assemblyInfo.Version != newVersion)
                    {
                        assemblyInfo.Version = newVersion;
                        assemblyInfo.FileVersion = newVersion;
                        assemblyInfo.Save();
                    }

                    if (i == 0)
                    {
                        _manifest.Version = _manifest.MinVersion = newVersion;
                    }
                }

                i++;
                row.DefaultCellStyle.ForeColor = Color.Green;
            }
        }


        private bool Build()
        {
            var solution = (Solution2) Host.Instance.DTE.Solution;
            var solutionBuild = (SolutionBuild2) solution.SolutionBuild;
            solutionBuild.SolutionConfigurations.Item("Release").Activate();
            _buildDir =
                $"{Path.GetDirectoryName(_startProject.FullName)}\\bin\\{solutionBuild.ActiveConfiguration.Name}\\";
            solutionBuild.Build(true);
            if (solutionBuild.LastBuildInfo != 0)
            {
                return false;
            }
            return true;
        }

        private void chkSame_CheckedChanged(object sender, EventArgs e)
        {
            var i = 0;
            var v = projectGrid.Rows[0].Cells[1].Value.ToString();
            foreach (DataGridViewRow row in projectGrid.Rows)
            {
                if (i > 0)
                {
                    row.Cells[1].Value = chkSame.Checked ? v : (row.Tag as ProjectAssemblyInfo).Version;
                }
                i++;
            }
        }

        private void manifestGrid_OnSelectedRowsChanged(object sender, EventArgs e)
        {
            BindBox();
        }

        private void BindBox()
        {
            BindBox(exeBox, ".exe");
            BindBox(icoBox, ".ico");
        }

        private void BindBox(ComboBox box, string ext)
        {
            var current = box.Text;
            var files = manifestGrid.GetSelectedFiles(ext).ToList();
            box.Items.Clear();
            box.Items.AddRange(files.ToArray());
            if (!string.IsNullOrWhiteSpace(current) && files.Contains(current))
                return;
            if (files.Count > 0)
            {
                box.Text = files[0];
            }
        }

        private void exeBox_Validating(object sender, CancelEventArgs e)
        {
            var box = sender as ComboBox;
            if (box == null)
                return;
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                errorProvider.SetError(box, "Application entry is required.");
                e.Cancel = true;
            }
        }

        private void exeBox_Validated(object sender, EventArgs e)
        {
            var box = sender as ComboBox;
            if (box == null)
                return;
            errorProvider.SetError(box, null);
        }

     
    }
}