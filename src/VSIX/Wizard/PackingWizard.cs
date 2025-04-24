﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using AeroWizard;
using CnSharp.Updater;
using CnSharp.Updater.Packaging;
using CnSharp.Updater.Util;
using CnSharp.VisualStudio.Extensions;
using CnSharp.VisualStudio.Extensions.Projects;
using CnSharp.VisualStudio.SharpUpdater.Util;
using EnvDTE;
using Constants = CnSharp.Updater.Constants;
using ManifestGatherer = CnSharp.VisualStudio.SharpUpdater.Util.ManifestGatherer;
using Process = System.Diagnostics.Process;
using StatusBar = System.Windows.Forms.StatusBar;

namespace CnSharp.VisualStudio.SharpUpdater.Wizard
{
    public partial class PackingWizard : Form
    {
        private Manifest _manifest;
        private ManifestGatherer _manifestGatherer;
        private NuPackSettings _settings;
        private Project _startProject;
        private ProjectAssemblyInfo _assemblyInfo;
        private ProjectProperties _projectProperties;
        private readonly StatusBar statusBar;
        private string _releaseDir;
        private string _exeDir;
        private string _manifestFile;
        private string _outputDir;
        private string _projectDir;
        private string _targetFramework;
        private string _zipPath;

        public PackingWizard()
        {
            InitializeComponent();

            statusBar = new StatusBar();
            Controls.Add(statusBar);

            ActiveControl = projectGrid;
            BindTextBoxEvents();
            projectGrid.AutoGenerateColumns = false;
            
            LoadManifest();
            BindProjects();
            BindSettings();

            stepWizardControl.SelectedPageChanged += StepWizardControl_SelectedPageChanged;
            stepWizardControl.Finished += StepWizardControl_Finished;
            wizardPageVersion.Commit += WizardPageCommit;
            wizardPageManifest.Commit += WizardPageCommit;
            wizardPagePackage.Commit += WizardPageCommit;
        }

       private  void BindSettings()
        {
            var cacheHelper = new CacheHelper<NuPackSettings>();
            var settingFile = Paths.GetSettingsFileLocation(_startProject);
            _settings = File.Exists(settingFile) ? cacheHelper.Get(settingFile) : new NuPackSettings();
            txtOutputDir.Text = _settings.PackageOutputDirectory;
            chkOpenDir.Checked = _settings.OpenPackageOutputDirectoryAfterBuild;
        }

        private void LoadManifest()
        {
            var dte2 = Host.Instance.Dte2;
            _startProject = dte2.GetActiveProject();
            _projectDir = _startProject.GetDirectory();
            _releaseDir = $"{_projectDir}\\bin\\Release\\";
            _assemblyInfo = _startProject.GetProjectAssemblyInfo();
            _projectProperties = _startProject.GetProjectProperties();
            _targetFramework = _projectProperties?.TargetFramework;
            _manifestFile = $"{_projectDir}\\{Manifest.ManifestFileName}";

            string id = !string.IsNullOrWhiteSpace(_assemblyInfo.Title)
                ? _assemblyInfo.Title
                : (!string.IsNullOrWhiteSpace(_assemblyInfo.Product)
                    ? _assemblyInfo.Product 
                    : _assemblyInfo.ProjectName
                ); 
            if (File.Exists(_manifestFile))
            {
                var doc = new XmlDocument();
                doc.Load(_manifestFile);
                var xml = doc.InnerXml;
                if (_assemblyInfo != null)
                {
                    xml = xml.Replace("$id$",id)
                        .Replace("$owner$", _assemblyInfo.Company)
                        .Replace("$description$", _assemblyInfo.Description)
                        .Replace("$copyright$", _assemblyInfo.Copyright);
                }

                _manifest = XmlSerializerHelper.LoadObjectFromXmlString<Manifest>(xml);
            }
            else
            {
                _manifest = new Manifest();
                if (_assemblyInfo != null)
                {
                    _manifest.Id = id;
                    _manifest.AppName = !string.IsNullOrWhiteSpace(_assemblyInfo.Title) ? _assemblyInfo.Title : id;
                    _manifest.Owner = _assemblyInfo.Company;
                    _manifest.Description = _assemblyInfo.Description;
                    _manifest.Copyright = _assemblyInfo.Copyright;
                }
            }

            _manifest.EntryPoint = _startProject.Properties.Item("AssemblyName").Value + ".exe";
            exeBox.Text = _manifest.EntryPoint;
            targetBox.Text = _targetFramework;
        }

        private void BindProjects()
        {
            var dte2 = Host.Instance.Dte2;
            var refProjects = SolutionDataCache.Instance.GetSolutionProperties(dte2.Solution.FileName).Projects;
            projectGrid.Rows.Clear();
            BindProject(_startProject);
            foreach (var project in refProjects)
            {
                if(project == _startProject) continue;
                BindProject(project);
            }

            chkSame.Visible = refProjects.Count > 1;
        }

        private void BindProject(Project project)
        {
            var asm = project.GetProjectAssemblyInfo();
            projectGrid.Rows.Add(
                project.Name,
                asm != null ? asm.Version : string.Empty,
                asm != null ? asm.Version : string.Empty
                );
            projectGrid.Rows[projectGrid.Rows.Count - 1].Tag = project;
            if (project == _startProject)
            {
                projectGrid.Rows[projectGrid.Rows.Count - 1].DefaultCellStyle.Font = new Font(
                    projectGrid.Font.FontFamily,
                    projectGrid.Font.Size,
                    FontStyle.Bold
                    );
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
                if (!chkNoBuild.Checked)
                {
                    if (!Build())
                    {
                        DialogResult = DialogResult.Abort;
                        e.Cancel = true;
                        return;
                    }

                    statusBar.Text = string.Empty;
                }

                _exeDir = _targetFramework == null ? _releaseDir : Path.Combine(_releaseDir, _targetFramework);
                _manifestGatherer = new ManifestGatherer(_exeDir, _projectDir, _settings);
                var items = _manifestGatherer.GatherFiles();
                manifestGrid.Bind(_manifest, _exeDir, items);
                BindBoxes();
                exeBox.Text = _startProject.Properties.Item("AssemblyName").Value + ".exe";
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
                if(!string.IsNullOrWhiteSpace(_settings.DeployServer) && string.IsNullOrWhiteSpace(_settings.DeployKey))
                    txtKey.Focus();
                else
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
                return;
            }
            
            Task.Run(() => HttpUtil.CheckUrl(box.Text)).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    var msg = t.Exception.InnerExceptions.Count > 0 ? t.Exception.InnerExceptions[0].Message : t.Exception.Message;
                    errorProvider.SetError(box, msg);
                    e.Cancel = true;
                }
            });
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
                dir.StartsWith(_releaseDir, StringComparison.InvariantCultureIgnoreCase))
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

        public async Task PackingAsync()
        {
            ActiveOutputWindow();
            OutputMessage("Start..." + Environment.NewLine);
            _outputDir = EnsureOutputDir(_projectDir, txtOutputDir.Text);

            SaveManifest();

            //packing
            var zipName = $"{_manifest.AppName}_{_manifest.Version}{Manifest.PackageFileExt}";
            _zipPath = Path.Combine(_outputDir, zipName);
            if (!chkNoBuild.Checked)
            {
                if (File.Exists(_zipPath))
                    File.Delete(_zipPath);
                OutputMessage("Packaging..." + Environment.NewLine);
                var pb = new PackageBuilder(_manifest, _exeDir);
                pb.CreatePackage(_zipPath);
            }

            OutputMessage("Save project settings..." + Environment.NewLine);
            SaveProjectSettings();

            var deployed = await Deploy();
            if (!deployed)
                return;

            if (chkOpenDir.Checked)
                Process.Start(_outputDir);

            OutputMessage("All done!" + Environment.NewLine);
        }

        private void ActiveOutputWindow()
        {
            Host.Instance.Dte2.ToolWindows.OutputWindow.Parent.Activate();
        }



        private void OutputMessage(string message)
        {
            Host.Instance.Dte2.OutputMessage(Constants.ProductName, message);
        }

        private async Task<bool> Deploy()
        {
            var url = sourceBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
                return true;
            try
            {
                OutputMessage("Push the package..." + Environment.NewLine);
                await PackageUploader.Upload( _zipPath, url, txtKey.Text);
                OutputMessage($"Upload package to {url} succeeded.");
                return true;
            }
            catch(Exception ex)
            {
                var msg = $"Upload package to {url} failed.";
                msg += $"Exception:{ex.Message}{Environment.NewLine}";
                Host.Instance.Dte2.OutputMessage(Constants.ProductName,msg);
                return false;
            }
        }

        private void SaveManifest()
        {
            _manifest = _productInfoControl.Manifest;
            _manifest.EntryPoint = exeBox.Text;
            //_manifest.ShortcutIcon = targetBox.Text;
            _manifest.ReleaseDate = DateTimeOffset.Now;
            _manifest.Files = manifestGrid.GetReleaseFiles().ToList();

            var exeManifestFilePath = string.Concat(_exeDir, "\\", Manifest.ManifestFileName);
            _manifest.Save(exeManifestFilePath);

            var projectManifestFilePath = string.Concat(_projectDir, "\\", Manifest.ManifestFileName);
          
            if (File.Exists(projectManifestFilePath))
            {
                _manifest.Merge(projectManifestFilePath);
            }
            else
            {
                File.Copy(exeManifestFilePath, projectManifestFilePath);
                try
                {
                    _startProject.ProjectItems.AddFromFile(projectManifestFilePath);
                }
                catch (Exception ignored)
                {

                }
            }

            File.Copy(exeManifestFilePath, Path.Combine(_outputDir, _manifest.Id+Constants.ManifestExtension), true);
        }

        private void SaveProjectSettings()
        {
            _settings.UnselectedFolders = manifestGrid.GetExcludedFolders().ToList();
            _settings.UnselectedFiles = manifestGrid.GetExcludedFiles().ToList();
            _settings.DeployServer = sourceBox.Text;
            //_settings.DeployKey = txtKey.Text;
            _settings.PackageOutputDirectory = txtOutputDir.Text.Contains(":") ? _outputDir : txtOutputDir.Text;
            _settings.OpenPackageOutputDirectoryAfterBuild = chkOpenDir.Checked;

            var cacheDir = Paths.GetSettingsFileLocation(_startProject);
            var cacheHelper = new CacheHelper<NuPackSettings>();
            cacheHelper.Save(_settings, cacheDir);
        }

        protected string EnsureOutputDir(string baseDir, string outputDir)
        {
            var dir = outputDir.Trim().Replace('/', Path.DirectorySeparatorChar);
            dir = dir.Replace("{version}", _manifest.Version);
            dir = FileUtil.JoinFilePath(baseDir, dir);
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
                var project = row.Tag as Project;
                var assemblyInfo = project.GetProjectAssemblyInfo();
                var newVersion = row.Cells[1].Value?.ToString().Trim();
                //if (string.IsNullOrEmpty(newVersion) && i == 0)
                //    return;

                if (assemblyInfo != null)
                {
                    if (assemblyInfo.Version != newVersion)
                    {
                        if (project.HasAssemblyInfo())
                        {
                            assemblyInfo.Version = newVersion;
                            assemblyInfo.FileVersion = newVersion;
                            assemblyInfo.Save();
                        }
                        else
                        {
                            project.Properties.Item("Version").Value =
                                project.Properties.Item("FileVersion").Value =
                                 project.Properties.Item("AssemblyVersion").Value = newVersion;
                        }

                        if (i == 0)
                        {
                            _manifest.Version = _manifest.MinVersion = newVersion.ToSemanticVersion();
                        }
                    }
                }

                i++;
                row.DefaultCellStyle.ForeColor = Color.Green;
            }
        }


        private bool Build()
        {
            statusBar.Text = "Building...";
           return Host.Instance.Solution2.BuildRelease();
        }

        private void chkSame_CheckedChanged(object sender, EventArgs e)
        {
            var i = 0;
            var v = projectGrid.Rows[0].Cells[1].Value.ToString();
            foreach (DataGridViewRow row in projectGrid.Rows)
            {
                if (i > 0)
                {
                    row.Cells[1].Value = chkSame.Checked ? v : row.Cells[2].Value;
                }
                i++;
            }
        }


        private void BindBoxes()
        {
            BindTargetBox();
            foreach (var fileType in Common.SupportedFileTypes)
            {
                if (BindBox(exeBox, fileType))
                    break;
            }
        }

        private bool BindBox(ComboBox box, string ext)
        {
            var current = box.Text;
            var files = manifestGrid.GetSelectedFiles(ext).ToList();
            box.Items.Clear();
            box.Items.AddRange(files.ToArray());
            if (!string.IsNullOrWhiteSpace(current) && files.Contains(current))
                return true;
            if (files.Count > 0)
            {
                box.Text = files[0];
                return true;
            }

            return false;
        }

        private void BindTargetBox()
        {
            var current = targetBox.Text;
            var frameworks = _targetFramework != null ? new List<string> { _targetFramework } : new List<string> { "-" };
            targetBox.Items.Clear();
            targetBox.Items.AddRange(frameworks.ToArray());
            if (!string.IsNullOrWhiteSpace(current) && frameworks.Contains(current))
                return;
            if (frameworks.Count > 0)
            {
                targetBox.Text = frameworks[0];
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

        private void txtKey_Validating(object sender, CancelEventArgs e)
        {
            if (NeedApiKey())
            {
                errorProvider.SetError(txtKey, "ApiKey is required.");
                e.Cancel = true;
            }
        }

        private bool NeedApiKey()
        {
            return !string.IsNullOrWhiteSpace(sourceBox.Text) && string.IsNullOrWhiteSpace(txtKey.Text);
        }

        private void txtKey_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(txtKey, null);
        }
    }
}