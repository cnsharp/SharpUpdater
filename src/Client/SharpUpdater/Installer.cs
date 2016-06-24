using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using CnSharp.Updater;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public class Installer
    {
        private readonly string _packagePath;
        private readonly string _appDir;
        private readonly string[] _ignoreFiles;
        private readonly string _packageUpdatedDate;
        private string _tempDir;
        private string _tempFile;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadCompleted;

        public delegate void UnzippedEventHandler(string manifestFile);

        public event UnzippedEventHandler UnzipCompleted;

        public Installer(string packagePath,string appDir, string[] ignoreFiles,string packageUpdatedDate = null)
        {
            _packagePath = packagePath;
            _appDir = appDir;
            _ignoreFiles = ignoreFiles;
            _packageUpdatedDate = packageUpdatedDate;
        }

        public  void Install()
        {
            if (_packagePath.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                var wc = new WebClient();
                _tempDir = Path.Combine(Path.GetTempPath(), Common.AppName);
               _tempFile = _tempDir +"\\"+ Guid.NewGuid() + ".zip";

                wc.DownloadProgressChanged += (sender, args) =>
                {
                    DownloadProgressChanged?.Invoke(sender, args);
                };
                wc.DownloadFileCompleted += (sender, args) =>
                {
                    Unzip();
                    DownloadCompleted?.Invoke(sender, args);
                };

                wc.DownloadFileAsync(new Uri(_packagePath), _tempFile);
                return;
            }
            Unzip();

            if (!string.IsNullOrEmpty(_packageUpdatedDate))
            {
                var mf = Path.Combine(_appDir, Manifest.ManifestFileName);
                var manifest = FileUtil.ReadManifest(mf);
                manifest.PackageUpdated = _packageUpdatedDate;
                manifest.Save(mf);
            }
        }

        private void Unzip()
        {
            var zipFile = _tempFile ?? _packagePath;
            if (_ignoreFiles == null || _ignoreFiles.Length == 0)
            {
                ZipHelper.Unzip(zipFile, _appDir,true);
                return;
            }
            var dir = Path.GetDirectoryName(zipFile);
            dir += "\\" + Guid.NewGuid();
            ZipHelper.Unzip(zipFile, dir,true);
            UnzipCompleted?.Invoke(Path.Combine(dir,Manifest.ManifestFileName));
            ClearIgnoreFiles(dir, _ignoreFiles);
            FileUtil.CopyFiles(dir,_appDir);
            Directory.Delete(dir,true);
        }

        private void ClearIgnoreFiles(string dir, string[] ignoreFiles)
        {
            if (ignoreFiles == null)
                return;
            foreach (var ignoreFile in ignoreFiles)
            {
                var delFile = Path.Combine(dir, ignoreFile);
                if (File.Exists(delFile))
                    File.Delete(delFile);
            }
        }

    }
}