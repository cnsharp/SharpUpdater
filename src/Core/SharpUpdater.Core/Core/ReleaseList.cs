using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace CnSharp.Windows.Updater.Util
{
    /// <summary>
    /// release info of app
    /// </summary>
    /// <see cref="Manifest"/>
    [Obsolete("replaced by Manifest")]
    public class ReleaseList
    {
        private List<ReleaseFile> _files;
        private string _updateDescription;
        public string ApplicationStart { get; set; }
        public string AppName { get; set; }
        public string Company { get; set; }
        public string MinVersion { get; set; }
        public string ReleaseDate { get; set; }
        public string ReleaseUrl { get; set; }
        public string ReleaseVersion { get; set; }
        public string ShortcutIcon { get; set; }
        public bool Packaged { get; set; }
        public string WebSite { get; set; }

        [XmlIgnore]
        public string UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; }
        }

        [XmlElement("UpdateDescription")] //注意生成的节点名要同XmlIgnore的名称一致
        public XmlNode TextCData //不要使用XmlCDataSection,否则反序列化会出异常
        {
            get
            {
                return new XmlDocument().CreateCDataSection(_updateDescription);
                //注意此处不能写成 this.Description，否则将会得到null
            }
            set { _updateDescription = value.Value; }
        }

        public List<ReleaseFile> Files
        {
            get { return _files ?? (_files = new List<ReleaseFile>()); }
            set { _files = value; }
        }

        public int Compare(string version)
        {
            return CompareVersion(ReleaseVersion, version);
        }

        /// <summary>
        ///     比较版本号
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <param name="newVersion"></param>
        /// <returns></returns>
        public static int CompareVersion(string currentVersion, string newVersion)
        {
            if (string.IsNullOrEmpty(currentVersion) || string.IsNullOrEmpty(newVersion))
                return 1;
            var myVersion = currentVersion.Split('.');
            var otherVersion = newVersion.Split('.');
            var size = Math.Max(myVersion.Length, otherVersion.Length);
            for (int i = 0; i < size; i++)
            {
                int myNumber = GetIntFromArray(myVersion, i);
                int otherNumber = GetIntFromArray(otherVersion, i);
                if (myNumber != otherNumber)
                    return myNumber - otherNumber;
            }
            return 0;
        }

        private static int GetIntFromArray(string[] array, int i)
        {
            if (array.Length <= i)
                return 0;
            int value;
            if (!int.TryParse(array[i], out value) || value < 0)
                throw new ArgumentException("invalid version number");
            return value;
        }

        public int Compare(ReleaseList otherList)
        {
            if (otherList == null)
                throw new ArgumentNullException("otherList");
            int diff = Compare(otherList.ReleaseVersion);
            if (diff != 0)
                return diff;
            return (ReleaseDate == otherList.ReleaseDate)
                       ? 0
                       : (DateTime.Parse(ReleaseDate) > DateTime.Parse(otherList.ReleaseDate) ? 1 : -1);
        }

        public ReleaseFile[] GetDifferences(ReleaseList otherList, out long fileSize)
        {
            if (Packaged)
            {
                fileSize = otherList.Files[0].FileSize;
                return new[] {otherList.Files[0]};
            }
            fileSize = 0;
            if (otherList == null || Compare(otherList) == 0)
                return null;
            var ht = new Hashtable();
            foreach (ReleaseFile file in _files)
            {
                ht.Add(file.FileName, file.Version);
            }
            var diffrences = new List<ReleaseFile>();
            foreach (ReleaseFile file in otherList._files)
            {
                if ((!ht.ContainsKey(file.FileName)) || ht[file.FileName] == null ||
                    CompareVersion(file.Version, ht[file.FileName].ToString()) != 0)
                {
                    diffrences.Add(file);
                    fileSize += file.FileSize;
                }
            }
            return diffrences.ToArray();
        }
    }
}