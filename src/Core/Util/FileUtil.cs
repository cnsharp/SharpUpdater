using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace CnSharp.Updater.Util
{
    public static class FileUtil
    {
        
        public static Manifest ReadManifest(string fileName)
        {
            return XmlSerializerHelper.LoadObjectFromXml<Manifest>(fileName);
        }

        public static void Save(this Manifest manifest, string fileName)
        {
            if (File.Exists(fileName))
            {
                var current = XmlSerializerHelper.LoadObjectFromXml<Manifest>(fileName);
                var props = typeof(Manifest).GetProperties()
                    .Where(p => p.CanRead && p.CanWrite && p.GetType() != typeof(XmlNode))
                    .ToList();
                foreach (var prop in props)
                {
                    var currentValue = prop.GetValue(current)?.ToString();
                    var newValue = prop.GetValue(manifest);
                    if (currentValue != null && currentValue.StartsWith("$") && currentValue.EndsWith("$")) 
                        continue;
                    if (newValue != null)
                    {
                        prop.SetValue(current, newValue);
                    }
                }
                XmlSerializerHelper.SerializeToXmlFile(current, fileName);
                return;
            }

            XmlSerializerHelper.SerializeToXmlFile(manifest, fileName);
        }


        public static void CopyFiles(string sourceDirectory, string targetDirectory)
        {
            if (!Directory.Exists(sourceDirectory)) return;
            if (!Directory.Exists(targetDirectory))
                Directory.CreateDirectory(targetDirectory);

            string[] directories = Directory.GetDirectories(sourceDirectory);
            if (directories.Length > 0)
            {
                foreach (string d in directories)
                {
                    CopyFiles(d, targetDirectory + d.Substring(d.LastIndexOf("\\")));
                }
            }

            string[] files = Directory.GetFiles(sourceDirectory);
            if (files.Length > 0)
            {
                foreach (string s in files)
                {
                    File.Copy(s, targetDirectory + s.Substring(s.LastIndexOf("\\")), true);
                }
            }
        }

        public static void DeleteFolder(string path)
        {
            DeleteDirectory(new DirectoryInfo(path));
        }

        public static void DeleteDirectory(DirectoryInfo dir)
        {
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    DeleteDirectory(child);
                }
                dir.Delete(true);
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5(this string input)
        {
            var md5 = new MD5CryptoServiceProvider();

            byte[] inBytes = Encoding.Default.GetBytes(input);

            byte[] outBytes = md5.ComputeHash(inBytes);

            var output = new StringBuilder();

            for (int i = 0; i < outBytes.Length; i++)
            {
                output.Append(outBytes[i].ToString("x2"));
            }

            return output.ToString();
        }

        public static string FormatFileSizeDescription(long bytes)
        {
            if (bytes > 1024 * 1024)
                return string.Format("{0}M", Math.Round((double)bytes / (1024 * 1024), 2, MidpointRounding.AwayFromZero));
            if (bytes > 1024)
                return string.Format("{0}K", Math.Round((double)bytes / 1024, 2, MidpointRounding.AwayFromZero));
            return string.Format("{0}B", bytes);
        }


        public static long CopyTo(this Stream source, Stream target)
        {
            const int bufSize = 0x1000;

            var buf = new byte[bufSize];

            long totalBytes = 0;

            var bytesRead = 0;

            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
                totalBytes += bytesRead;
            }

            return totalBytes;
        }

        public static string JoinFilePath(string baseDir, string targetDir)
        {
            var dir = targetDir.Trim().Replace('/', Path.DirectorySeparatorChar);
            if (!dir.Contains(":" + Path.DirectorySeparatorChar))
            {
                dir = Path.Combine(baseDir, dir);
            }
            return dir;
        }
    }
}