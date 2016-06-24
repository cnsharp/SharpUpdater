using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CnSharp.Updater.Util;

namespace CnSharp.Updater
{
    /// <summary>
    /// manifest of APP
    /// </summary>
    public class Manifest
    {
        public const string ManifestFileName = "manifest.xml";
        public const string ManifestExt = ".manifest";
        public const string PackageFileExt = ".sp";

        private List<ReleaseFile> _files;
     
        /// <summary>
        /// entry point of app,old name 'ApplicationStart' (V3.0+)
        /// </summary>
        public string EntryPoint { get; set; }

        /// <summary>
        /// APP's name or title
        /// </summary>
        public string AppName { get; set; }
        public string Owner { get; set; }
        public string MinVersion { get; set; }
        public string ReleaseDate { get; set; }
        public string ReleaseUrl { get; set; }
        public string Version { get; set; }
        public string ShortcutIcon { get; set; }

        /// <summary>
        /// ID of APP (V4.0+)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Icon web url  (V4.0+)
        /// </summary>
        public string IconUrl { get; set; }
        //public bool Packaged { get; set; }
        public string WebSite { get; set; }

        /// <summary>
        /// tags (V4.0+)
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Language (V4.0+)
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Copyright (V4.0+)
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Updated date time string in package feed,like '2016-06-21T06:54:11Z'
        /// </summary>
        public string PackageUpdated { get; set; }

        private string _releaseNotes;
        /// <summary>
        /// release note of app,old name 'UpdateDescription' (V3.0+)
        /// </summary>
        [XmlIgnore]
        public string ReleaseNotes
        {
            get { return _releaseNotes; }
            set { _releaseNotes = value; }
        }

        [XmlElement("ReleaseNotes")] 
        public XmlNode ReleaseNoteCData 
        {
            get
            {
                return new XmlDocument().CreateCDataSection(_releaseNotes);
            }
            set { _releaseNotes = value.Value; }
        }

        private string _description;
        /// <summary>
        /// App Description (V4.0+)
        /// </summary>
        [XmlIgnore]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [XmlElement("Description")]
        public XmlNode DescriptionCData
        {
            get
            {
                return new XmlDocument().CreateCDataSection(_description);
            }
            set { _description = value.Value; }
        }

        public List<ReleaseFile> Files
        {
            get { return _files ?? (_files = new List<ReleaseFile>()); }
            set { _files = value; }
        }

        public int CompareTo(string version)
        {
            return Version.CompareVersion(version);
        }

    
        public int CompareTo(Manifest otherList)
        {
            if (otherList == null)
                throw new ArgumentNullException(nameof(otherList));
            int diff = CompareTo(otherList.Version);
            if (diff != 0)
                return diff;
            return (ReleaseDate == otherList.ReleaseDate)
                       ? 0
                       : (DateTime.Parse(ReleaseDate) > DateTime.Parse(otherList.ReleaseDate) ? 1 : -1);
        }

        public ReleaseFile[] GetDifferences(Manifest otherList, out long fileSize)
        {
            fileSize = 0;
            if (CompareTo(otherList) == 0)
                return null;
            var ht = new Hashtable();
            foreach (var file in _files)
            {
                ht.Add(file.FileName, file.Version);
            }
            var diffrences = new List<ReleaseFile>();
            foreach (var file in otherList.Files)
            {
                if ((!ht.ContainsKey(file.FileName)) || ht[file.FileName] == null ||
                    file.Version.CompareVersion(ht[file.FileName].ToString()) != 0
                    )
                {
                    diffrences.Add(file);
                    fileSize += file.FileSize;
                }
            }
            return diffrences.ToArray();
        }


        public string ZipUrl
        {
            get
            {
                var url = ReleaseUrl;
                if (!url.EndsWith("/"))
                    url += "/";
                return $"{url}{AppName}_{Version}{PackageFileExt}";
            }
        }

        public string GetReleaseFileUrl(string fileName)
        {
            var url = ReleaseUrl;
            if (!url.EndsWith("/"))
                url += "/";
            return $"{url}{Version}/{fileName}";
        }

        public string VersionRoot
        {
            get
            {
                var url = ReleaseUrl;
                if (!url.EndsWith("/"))
                    url += "/";
                return $"{url}{Version}/";
            }
        }


        public void Save(Stream stream)
        {
            Save(stream, validate: true, minimumManifestVersion: 1);
        }

        /// <summary>
        /// Saves the current manifest to the specified stream.
        /// </summary>
        /// <param name="stream">The target stream.</param>
        /// <param name="minimumManifestVersion">The minimum manifest version that this class must use when saving.</param>
        public void Save(Stream stream, int minimumManifestVersion)
        {
            Save(stream, validate: true, minimumManifestVersion: minimumManifestVersion);
        }

        public void Save(Stream stream, bool validate)
        {
            Save(stream, validate, minimumManifestVersion: 1);
        }

        public void Save(Stream stream, bool validate, int minimumManifestVersion)
        {
            //if (validate)
            //{
            //    // Validate before saving
            //    Validate(this);
            //}

            //int version = Math.Max(minimumManifestVersion, ManifestVersionUtility.GetManifestVersion(Metadata));
            //string schemaNamespace = ManifestSchemaUtility.GetSchemaNamespace(version);

            // Define the namespaces to use when serializing
            var ns = new XmlSerializerNamespaces();
            //ns.Add("", schemaNamespace);

            // Need to force the namespace here again as the default in order to get the XML output clean
            var serializer = new XmlSerializer(typeof(Manifest));//, schemaNamespace);
            serializer.Serialize(stream, this, ns);
        }
    }
}
