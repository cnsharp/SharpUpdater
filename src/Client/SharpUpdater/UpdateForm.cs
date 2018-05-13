using System;
using System.Windows.Forms;
using CnSharp.Updater;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public partial class UpdateForm : BaseForm
    {
        #region Constants and Fields

        private static readonly string FinishText = Common.GetLocalText("Completed");
        private static readonly string RetryText = Common.GetLocalText("Retry");
        private Manifest _localManifest;
        private readonly PackageFeedSummary _packageFeedSummary;


        private bool _downloaded;
        private string _totalSizeDesc;


        private Installer _installer;

        #endregion

        #region Constructors and Destructors

        public UpdateForm(Manifest localManifest, PackageFeedSummary packageFeedSummary)
        {
            InitializeComponent();

            btnUpgrade.Focus();

            _localManifest = localManifest;
            _packageFeedSummary = packageFeedSummary;

            _installer = new Installer(packageFeedSummary.PackageUrl, Application.StartupPath, Common.IgnoreFiles,
                packageFeedSummary.Updated);
            _installer.DownloadProgressChanged += (sender, e) => { progressBar.Value = e.ProgressPercentage; };
            _installer.DownloadCompleted += (sender, args) => { Finish(); };
            _installer.UnzipCompleted += file =>
            {
                var manifest = FileUtil.ReadManifest(file);
                _localManifest = manifest;
                CreateShortcut(manifest);
            };
        }


        private void Finish()
        {
            btnUpgrade.Enabled = true;
            btnUpgrade.Text = FinishText;
            _downloaded = true;

            Application.Exit();
            Common.Start(_localManifest);
        }

        #endregion

        #region Properties

        private bool OptionalUpdate
        {
            set { isEnableCloseButton = btnUpgrade.Enabled = btnCancel.Enabled = value; }
        }

        #endregion

        #region Methods

        private void DoUpgrade()
        {
            _downloaded = false;
            progressBar.Value = 0;
            _installer.Install();
        }

        private void FormUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormUpdate_Load(object sender, EventArgs e)
        {
            lblSize.Text = string.Empty;
            Init();
        }

        private void Init()
        {
            OptionalUpdate = _localManifest.CompareTo(_packageFeedSummary.Version) >= 0;
            lblTitle.Text = string.Format(
                lblTitle.Text,
                _localManifest.Version,
                _packageFeedSummary.Version,
                _packageFeedSummary.Updated);

            boxDes.Text = _packageFeedSummary.ReleaseNotes;

            progressBar.Maximum = 100;
            if (!btnUpgrade.Enabled)
            {
                DoUpgrade();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Directory.Delete(tempPath, true);
            Application.Exit();
            Common.Start(_localManifest);
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            btnUpgrade.Enabled = false;
            btnCancel.Enabled = false;
            if (!_downloaded)
            {
                DoUpgrade();
            }
            else
            {
                Application.Exit();
                Common.Start(_localManifest);
            }
        }

        #endregion
    }
}