using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Text;

namespace CnSharp.Windows.Updater.Util
{

    public class VersionChecker 
    {
        private BackgroundWorker _worker;
        private readonly string _versionFileUrl;
        private readonly string _updateLogUrl;
        private UpdateInfo _updateInfo;
        public event VersionEventHandler CheckCompleted;

        public VersionChecker(string versionFileUrl, string updateLogUrl)
        {
            this._versionFileUrl = versionFileUrl;
            this._updateLogUrl = updateLogUrl;
            _updateInfo = new UpdateInfo();
        }

     
        public void Check()
        {
            if(_worker != null && _worker.IsBusy)
                return;
             _worker = new BackgroundWorker();
            _worker.DoWork += WorkerDoWork;
            _worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
            _worker.RunWorkerAsync(new[] { _versionFileUrl, _updateLogUrl });
        }


        private static string GetCurrentVersion(string versionFileUrl)
        {
            var wc = new WebClient();
            //return DownloadTools.DownloadUTFString(wc, versionFileUrl, Encoding.UTF8);
            wc.Encoding = Encoding.UTF8;
            return wc.DownloadString(versionFileUrl);
        }

        private static string GetUpdateLog(string updateLogUrl)
        {
            try
            {
                var wc = new WebClient();
                return wc.DownloadString(updateLogUrl);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            var array = (string[]) e.Argument;
            _updateInfo.Version = GetCurrentVersion(array[0]);
            var diff = FileUtil.CompareVersion(_updateInfo.Version,Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (diff > 0)
            {
                _updateInfo.Success = true;
                _updateInfo.NewVersionFound = true;
                _updateInfo.UpdateLog = GetUpdateLog(array[1]);
            }
        }

        void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                _updateInfo.Exception = e.Error;
            }
            if (CheckCompleted != null)
                CheckCompleted(this, new VersionEventArgs {UpdateInfo = _updateInfo});
        }

    }


    public delegate void VersionEventHandler(object sender, VersionEventArgs args);

    public class VersionEventArgs : EventArgs
    {
        public UpdateInfo UpdateInfo { get; set; }
    }

}
