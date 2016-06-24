using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace CnSharp.Updater.Util
{
    public class Downloader : BackgroundWorker
    {
        private readonly Manifest _remoteManifest;
        private readonly string _tempDir;
        private readonly long _totalSize;
        private readonly ReleaseFile[] _diff;

        public long TotalBytes
        {
            get { return _totalSize; }
        }

        public long ReceivedBytes { get; private set; }


        public Downloader(Manifest localManifest, Manifest remoteManifest, string tempDir)
        {
            _remoteManifest = remoteManifest;
            _tempDir = tempDir;
            _diff = localManifest.GetDifferences(_remoteManifest, out _totalSize);
            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);
        }


        public Downloader(Manifest remoteManifest,ReleaseFile[] files,long totalSize, string tempDir)
        {
            _remoteManifest = remoteManifest;
            _tempDir = tempDir;
            _diff = files;
            _totalSize = totalSize;
            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);
        }

       public new void RunWorkerAsync()
        {
            if (_diff == null || _diff.Length == 0)
                return;
            var di = new DownloadInfo
            {
                WebRoot =  _remoteManifest.VersionRoot,//todo
                TempDir = _tempDir,
                Files = _diff
            };

            base.RunWorkerAsync(di);
        }



        //public event DownloadProgressChangedEventHandler DownloadProgressChanged;


        protected override void OnDoWork(DoWorkEventArgs e)
        {
            base.OnDoWork(e);
            var info = e.Argument as DownloadInfo;
            if (info == null)
                return;
            foreach (var file in info.Files)
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        var fileName = Path.Combine(info.TempDir, file.FileName);
                        var dir = Path.GetDirectoryName(fileName);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                            wc.DownloadFile(new Uri(info.WebRoot + "/" + file.FileName),
                                fileName);
                            if (base.WorkerReportsProgress)
                            {
                                SendProgress(file.FileSize);
                            }
                    }
                }
                catch
                {
                    e.Result = file.FileName;
                    throw;
                }
            }
        }

        private void SendProgress(long bytes)
        {
            ReceivedBytes += bytes;

            var progress = Math.Min(ReceivedBytes, _totalSize);
            var percentage = (int)((Math.Round(Convert.ToDecimal(progress) / Convert.ToDecimal(_totalSize) * 1.0M, 2)) * 100);
            base.ReportProgress(percentage);
        }



        public class DownloadInfo
        {
            public string WebRoot { get; set; }
            public string TempDir { get; set; }
            public ReleaseFile[] Files { get; set; }
        }
    }
}