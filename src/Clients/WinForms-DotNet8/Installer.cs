using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public class Installer
    {
        private readonly string _packagePath;
        private readonly string _appDir;
        private readonly string[] _ignoreFiles;
        private string _tempDir;
        private string _tempFile;

        public event ProgressChangedEventHandler DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadCompleted;

        public Installer(string packagePath, string appDir, string[] ignoreFiles)
        {
            _packagePath = packagePath;
            _appDir = appDir;
            _ignoreFiles = ignoreFiles;
        }

        public async Task InstallAsync()
        {
           
            var client = new HttpClient();
            _tempDir = Path.Combine(Path.GetTempPath(), Common.AppName);
            _tempFile = Path.Combine(_tempDir, Guid.NewGuid() + ".zip");

            var response = await client.GetAsync(_packagePath, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var canReportProgress = totalBytes != -1;

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(_tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                var buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    totalRead += bytesRead;
                    if (canReportProgress)
                    {
                        DownloadProgressChanged?.Invoke(this, new ProgressChangedEventArgs((int)(totalRead * 100 / totalBytes), null));
                    }
                }
            }

            Unzip();
            DownloadCompleted.Invoke(this, new AsyncCompletedEventArgs(null, false, null));
           
        }

        private void Unzip()
        {
            var dir = Path.GetDirectoryName(_tempFile);
            dir += "\\" + Guid.NewGuid();
            ZipHelper.Unzip(_tempFile, dir, true);
            ClearIgnoreFiles(dir, _ignoreFiles);
            FileUtil.CopyFiles(dir, _appDir);
            Directory.Delete(dir, true);
        }

        private void ClearIgnoreFiles(string dir, string[] ignoreFiles)
        {
            if (ignoreFiles == null) return;
            foreach (var ignoreFile in ignoreFiles)
            {
                var delFile = Path.Combine(dir, ignoreFile);
                if (File.Exists(delFile))
                    File.Delete(delFile);
            }
        }
    }
}