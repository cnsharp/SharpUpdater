using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace ZipUpdater.Monitor
{
   public class FileDownloader 
   {
       private readonly string fileUrl;
       private readonly string storePath;
       public long TotalBytes { get; set; }
       public long LoadedBytes { get; set; }
       private BackgroundWorker worker;
       public event DownloadEventHandler DownloadCompleted;

       public FileDownloader(string fileUrl,string storePath)
       {
           this.fileUrl = fileUrl;
           this.storePath = storePath;

       }
       public void Download()
       {
           if (worker != null && worker.IsBusy)
               return;
           worker = new BackgroundWorker();
           worker.DoWork += WorkerDoWork;
           worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
           worker.RunWorkerAsync(new[] { fileUrl, storePath });
       }

       private void WorkerDoWork(object sender, DoWorkEventArgs e)
       {
           var array = (string[]) e.Argument;
            Load(array[0],array[1]);
       }

       private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
       {
          if(DownloadCompleted != null)
          {
              DownloadCompleted(this, new DownEventArgs { Exception = e.Error });
          }
       }

       private void Load(string url,string path)
       {
           string dir = Path.GetDirectoryName(path);
           if (!Directory.Exists(dir))
               Directory.CreateDirectory(dir);

           WebRequest req = WebRequest.Create(url);
           WebResponse res = req.GetResponse();
           if (res.ContentLength == 0)
               return;

           long fileLength = res.ContentLength;
           TotalBytes = fileLength;
           LoadedBytes = 0;
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

                  var loadedBytes = LoadedBytes + downByte;
                  LoadedBytes = Math.Min(loadedBytes, TotalBytes);
               }

               var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
               fs.Write(bufferbyte, 0, bufferbyte.Length);
               srm.Close();
               srmReader.Close();
               fs.Close();
           }
       }
   }

   public delegate void DownloadEventHandler(object sender, DownEventArgs args);

   public class DownEventArgs : EventArgs
   {
       public bool Success 
       { 
           get
           {
               return Exception == null;
           }
       }
       public Exception Exception { get; set; }
   }
}
