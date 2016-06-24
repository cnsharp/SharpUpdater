using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace CnSharp.Windows.Updater.Util
{
    public class DownloadHelper
    {
        #region Public Methods

        public static void DownloadFile(
           string localFolder, string remoteFolder, string fileName, BackgroundWorker worker, long totalLength, ref long downloaded)
        {
            string url = remoteFolder + "/" + fileName;
            string path = localFolder + "/" + fileName;
            DownloadFile(path, url, worker, totalLength, ref downloaded);
        }

        public static void DownloadFile(string path, string url, BackgroundWorker worker, long totalLength, ref long downloaded)
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            //using (var wc = new WebClient())
            //{
            //    wc.DownloadFile(url,path);
            //    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            //}

            WebRequest req = WebRequest.Create(url);
            WebResponse res = req.GetResponse();
            if (res.ContentLength == 0)
            {
                return;
            }

            long fileLength = res.ContentLength;

            using (Stream srm = res.GetResponseStream())
            {
                var srmReader = new StreamReader(srm);
                var bufferbyte = new byte[fileLength];
                int allByte = bufferbyte.Length;
                int startByte = 0;
                while (fileLength > 0)
                {
                    int downByte = srm.Read(bufferbyte, startByte, allByte);
                    if (downByte == 0)
                    {
                        break;
                    }
                    startByte += downByte;
                    allByte -= downByte;

                    downloaded += downByte;
                    var progress = Math.Min(downloaded, totalLength);
                    var percentage =  (int)((Math.Round(Convert.ToDecimal(progress)/Convert.ToDecimal(totalLength)*1.0M,2))*100);
                    worker.ReportProgress(percentage);

                }

                var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(bufferbyte, 0, bufferbyte.Length);
                srm.Close();
                srmReader.Close();
                fs.Close();
            }
        }

        static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}