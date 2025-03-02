using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using CnSharp.Updater;
using CnSharp.Updater.Packaging;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public partial class UpdateForm : BaseForm
    {
        #region Constants and Fields

        private static readonly string FinishText = Common.GetLocalText("Completed");
        private static readonly string RetryText = Common.GetLocalText("Retry");
        private readonly PackageFeedSummary _packageFeedSummary;
        private Manifest _localManifest;
        private bool _downloaded;
        private readonly Installer _installer;

        #endregion

        #region Constructors and Destructors

        public UpdateForm(Manifest localManifest, PackageFeedSummary packageFeedSummary)
        {
            InitializeComponent();

            btnUpgrade.Focus();

            _localManifest = localManifest;
            _packageFeedSummary = packageFeedSummary;

            _installer = new Installer(packageFeedSummary.PackageUrl, Application.StartupPath, Common.IgnoreFiles);
            _installer.DownloadProgressChanged += (sender, e) => { progressBar.Value = e.ProgressPercentage; };
            _installer.DownloadCompleted += (sender, args) => { Finish(); };
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
            set => isEnableCloseButton = btnUpgrade.Enabled = btnCancel.Enabled = value;
        }

        #endregion

        #region Methods

        private async Task DoUpgrade()
        {
            _downloaded = false;
            progressBar.Value = 0;
            await  _installer.InstallAsync();
        }

        private void FormUpdate_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private async void FormUpdate_Load(object sender, EventArgs e)
        {
            lblSize.Text = string.Empty;
            await Init();
        }

        private async Task Init()
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
               await DoUpgrade();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Directory.Delete(tempPath, true);
            Application.Exit();
            Common.Start(_localManifest);
        }

        private async void btnUpgrade_Click(object sender, EventArgs e)
        {
            btnUpgrade.Enabled = false;
            btnCancel.Enabled = false;
            if (!_downloaded)
            {
              await DoUpgrade();
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