using System;
using System.IO;
using System.Net;

namespace CnSharp.Windows.Updater.Util
{
    public class DownloadUtil
    {
        public static void DownloadFile(string localFolder, string remoteFolder, string fileName)
        {
            //if (!System.IO.Directory.Exists(localFolder))
            //    System.IO.Directory.CreateDirectory(localFolder);
            string url = remoteFolder + "/" + fileName;
            string path = localFolder + "/" + fileName;
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var wc = new WebClient();
            wc.DownloadFile(url, path);
        }

        [Obsolete("moved to FileUtil")]
        public static string FormatFileSizeDescription(long bytes)
        {
            if (bytes > 1024 * 1024)
                return string.Format("{0}M", Math.Round((double)bytes / (1024 * 1024), 2, MidpointRounding.AwayFromZero));
            if (bytes > 1024)
                return string.Format("{0}K", Math.Round((double)bytes / 1024, 2, MidpointRounding.AwayFromZero));
            return string.Format("{0}B", bytes);
        }
    }
}