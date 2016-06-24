using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using CnSharp.Updater;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public partial class InstallForm : BaseForm
    {
        private readonly string _feedFile;

        #region Constants and Fields

        //private readonly string _configFile;
        private readonly string _tempDir;
        private Manifest _manifest;
        private BackgroundWorker _worker;
        private string _spFile;
        private string _feedUrl;
        private Installer _installer;
        private PackageFeedSummary _summary;

        #endregion

        #region Constructors and Destructors

        public InstallForm(string packagePath,string feedFile = null)
        {
            _feedFile = feedFile;
            InitializeComponent();
            if (packagePath.EndsWith(Manifest.PackageFileExt, StringComparison.InvariantCultureIgnoreCase))
                _spFile = packagePath;
            else
            {
                _feedUrl = packagePath;
            }
        }

        #endregion

        #region Methods

        private void ConnectionFormLoad(object sender, EventArgs e)
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            if (!string.IsNullOrEmpty(_feedUrl))
            {
                var doc = new XmlDocument();
                doc.Load(_feedUrl);
                var resolver = new PackageFeedResolver(doc);
                _summary = resolver.GetSummary();
                lblStatus.Text = Common.GetLocalText("UpdateTo") + "V" + _summary.Version;

                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Maximum = 100;
                _installer = new Installer(_summary.PackageUrl, Application.StartupPath, Common.IgnoreFiles,_summary.Updated);
                _installer.DownloadProgressChanged += (o, args) =>
                {
                    progressBar.Value = args.ProgressPercentage;
                };
                _installer.DownloadCompleted += (o, args) =>
                {
                    if (args.Error != null)
                    {
                        Console.WriteLine(args.Error.Message);
                        Console.WriteLine(args.Error.StackTrace);
                    }
                    progressBar.Value = progressBar.Maximum;
                };
            }
            else
            {
                _installer = new Installer(_spFile, Application.StartupPath, Common.IgnoreFiles);
                if (!string.IsNullOrEmpty(_feedFile) && File.Exists(_feedFile))
                {
                    var doc = new XmlDocument();
                    doc.Load(_feedFile);
                    var resolver = new PackageFeedResolver(doc);
                    _summary = resolver.GetSummary();
                    lblStatus.Text = Common.GetLocalText("UpdateTo") + "V" + _summary.Version;
                }
                else
                {
                    _installer.UnzipCompleted += file =>
                    {
                        _manifest = FileUtil.ReadManifest(file);
                        lblStatus.Text = Common.GetLocalText("UpdateTo") + "V" + _manifest.Version;
                    };
                }
            }

            _installer.Install();

        
           
                DialogResult = DialogResult.No;
          
        }

        #endregion
    }
}