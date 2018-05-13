using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using CnSharp.Updater;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public partial class ConnectionForm : BaseForm
    {
        #region Constants and Fields

        private readonly BackgroundWorker _worker;

        #endregion

        #region Constructors and Destructors

        public ConnectionForm()
        {
            InitializeComponent();

            var manifestFileName = Path.Combine(Application.StartupPath, Manifest.ManifestFileName);
            if (!File.Exists(manifestFileName))
            {
                return;
            }

            LocalManifest = FileUtil.ReadManifest(manifestFileName);
            _worker = new BackgroundWorker();
            _worker.DoWork += DoWork;
            _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            //var feedUrl = $"{LocalManifest.ReleaseUrl}/sp/GetUpdates()?packageIds='{LocalManifest.Id}'&versions='{LocalManifest.Version}'&includePrerelease=false";
            var feedUrl = $"{LocalManifest.ReleaseUrl}sp/Search()?$filter=IsLatestVersion&searchTerm='{LocalManifest.Id}'&includePrerelease=false";
            var doc = new XmlDocument();
            doc.Load(feedUrl);
            var feedResolver = new PackageFeedResolver(doc);
            var sum = feedResolver.GetSummary();
            e.Result = sum;
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Application.Exit();
                Common.Start(LocalManifest, true);
                return;
            }

            var sum = e.Result as PackageFeedSummary;

            if( !sum.IsNew(LocalManifest))
            {
                Application.Exit();
                Common.Start(LocalManifest, false);
                return;
            }
            //Hide();

            if (CheckProcessing() != DialogResult.OK)
            {
                Application.Exit();
                return;
            }
            var form = new UpdateForm(LocalManifest, sum);
            form.ShowDialog();
        }

        #endregion

        public Manifest LocalManifest { get; }

        #region Methods

        private DialogResult CheckProcessing()
        {
            string exeName = LocalManifest.EntryPoint.Substring(0, LocalManifest.EntryPoint.Length - 4);
            if (Process.GetProcessesByName(exeName).Length > 0)
            {
                DialogResult rs = MessageBox.Show(
                    string.Format(Common.GetLocalText("CloseRunning"), exeName),
                    Common.GetLocalText("Warning"),
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                if (rs == DialogResult.Retry)
                {
                    return CheckProcessing();
                }
                return rs;
            }
            return DialogResult.OK;
        }

        private void ConnectionFormLoad(object sender, EventArgs e)
        {
            _worker.RunWorkerAsync();
        }

        #endregion

        private void ConnectionForm_Shown(object sender, EventArgs e)
        {
          
        }

    }
}