using System;
using System.IO;
using CnSharp.Updater.Util;

namespace CnSharp.Updater
{
    public class Settings
    {
        public const string FileName = "settings.xml";
        static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.ProductName, FileName);

        public static Settings Load()
        {
            if (!File.Exists(FilePath)) return null;
            return XmlSerializerHelper.LoadObjectFromXml<Settings>(FilePath);
        }

        public string GlobalSource { get; set; }


        public void Save()
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            XmlSerializerHelper.SerializeToXmlFile(this, FilePath);
        }
    }
}