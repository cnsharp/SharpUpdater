using System.ComponentModel;
using System.IO;

namespace CnSharp.Updater.Util
{
    public class Monitor : BackgroundWorker
    {
        private readonly string _manifestPath;
        private readonly string _tempDir;

        public Monitor(string manifestPath, string tempDir)
        {
            _manifestPath = manifestPath;
            _tempDir = tempDir;
        }

        public Manifest LocalManifest { get; private set; }
        public Manifest RemoteManifest { get; private set; }
        public bool HasNewVersion { get; private set; }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            base.OnDoWork(e);
            LocalManifest = FileUtil.ReadManifest(_manifestPath);
            RemoteManifest = FileUtil.ReadManifest(LocalManifest.ReleaseUrl + "/sp/FindPackagesById()?id="+LocalManifest.Id);


            if (LocalManifest.CompareTo(RemoteManifest) != 0)
            {
                HasNewVersion = true;
                if (!string.IsNullOrEmpty(_tempDir))
                {
                    if (!Directory.Exists(_tempDir))
                        Directory.CreateDirectory(_tempDir);
                    RemoteManifest.Save(Path.Combine(_tempDir, Manifest.ManifestFileName));
                    //download update files silently
                    var loader = new Downloader(LocalManifest, RemoteManifest, _tempDir);
                    loader.RunWorkerAsync();
                }
            }
        }

    }
}