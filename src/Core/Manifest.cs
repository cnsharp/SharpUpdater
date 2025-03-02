using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public const string ManifestFileName = "SharpUpdater.manifest";
        public const string ManifestExt = ".manifest";
        public const string PackageFileExt = ".sp";
        public const string FileVersionNumberRegex = @"^[1-9]\d*(\.\d+){1,3}$";
        public const string PackageVersionNumberRegex = @"^\d+(\.\d+){2,3}(-(\w+)\.?\d*)?$";


        /// <summary>
        /// entry point of app
        /// </summary>
        public string EntryPoint { get; set; }

        /// <summary>
        /// APP name
        /// </summary>
        public string AppName { get; set; }
        public string Owner { get; set; }
        public string MinVersion { get; set; }
        /// <summary>
        /// Release Date of package in UTC format,like '2018-05-17T14:48:56+00:00'
        /// </summary>
        [XmlIgnore]
        public DateTimeOffset ReleaseDate { get; set; } = DateTimeOffset.UtcNow;
        [XmlElement("ReleaseDate")]
        public string ReleaseDateString
        {
            get => ReleaseDate.ToString("o");
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    ReleaseDate = DateTimeOffset.Parse(value);
            }
        }
        public string ReleaseUrl { get; set; }
        public string Version { get; set; }
        public string Icon { get; set; }

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
        /// release notes of the app
        /// </summary>
        [XmlIgnore]
        public string ReleaseNotes { get; set; }

        [XmlElement("ReleaseNotes")]
        public XmlNode ReleaseNoteCData
        {
            get => new XmlDocument().CreateCDataSection(ReleaseNotes);
            set => ReleaseNotes = value?.Value ?? string.Empty;
        }

        /// <summary>
        /// App Description (V4.0+)
        /// </summary>
        [XmlIgnore]
        public string Description { get; set; }

        [XmlElement("Description")]
        public XmlNode DescriptionCData
        {
            get => new XmlDocument().CreateCDataSection(Description);
            set => Description = value?.Value ?? string.Empty;
        }

        public List<ReleaseFile> Files { get; set; } = new List<ReleaseFile>();

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
            return (ReleaseDate - otherList.ReleaseDate).Seconds;
        }

        public ReleaseFile[] GetDifferences(Manifest otherList, out long fileSize)
        {
            fileSize = 0;
            if (CompareTo(otherList) == 0)
                return null;
            var ht = new Hashtable();
            foreach (var file in Files)
            {
                ht.Add(file.FileName, file.Version);
            }
            var differences = new List<ReleaseFile>();
            foreach (var file in otherList.Files)
            {
                if (!ht.ContainsKey(file.FileName) || ht[file.FileName] == null ||
                    file.Version.CompareVersion(ht[file.FileName].ToString()) != 0
                    )
                {
                    differences.Add(file);
                    fileSize += file.FileSize;
                }
            }
            return differences.ToArray();
        }


        public string PackageUrl
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
            if (validate)
            {
                // Validate before saving
                Validate(this);
            }

            //int version = Math.Max(minimumManifestVersion, ManifestVersionUtility.GetManifestVersion(Metadata));
            //string schemaNamespace = ManifestSchemaUtility.GetSchemaNamespace(version);

            // Define the namespaces to use when serializing
            var ns = new XmlSerializerNamespaces();
            //ns.Add("", schemaNamespace);

            // Need to force the namespace here again as the default in order to get the XML output clean
            var serializer = new XmlSerializer(typeof(Manifest));//, schemaNamespace);
            serializer.Serialize(stream, this, ns);
        }


        public bool Validate(Manifest manifest)
        {
            if (manifest == null) throw new ArgumentNullException("manifest", "manifest cannot be null.");
            if (string.IsNullOrWhiteSpace(manifest.Id)) return false;
            if (string.IsNullOrWhiteSpace(manifest.AppName)) return false;
            if (string.IsNullOrWhiteSpace(manifest.ReleaseUrl)) return false;
            if (manifest.Files == null || !manifest.Files.Any()) return false;
            if (string.IsNullOrWhiteSpace(manifest.EntryPoint)) return false;
            if (!manifest.Files.Any(f => f.FileName == manifest.EntryPoint)) return false;
            if (string.IsNullOrWhiteSpace(manifest.Version)) return false;
            if (!Regex.IsMatch(manifest.Version, PackageVersionNumberRegex)) return false;
            if (string.IsNullOrWhiteSpace(manifest.MinVersion))
                manifest.MinVersion = manifest.Version;
            else
            {
                if (!Regex.IsMatch(manifest.MinVersion, PackageVersionNumberRegex))
                    return false;
            }
            return true;
        }

        #region set field if it is blank

        private static bool IsPlaceHolder(string value)
        {
            return value.StartsWith("$") && value.EndsWith("$");
        }

        public Manifest SetIdIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Id) || IsPlaceHolder(Id)) && !string.IsNullOrWhiteSpace(value))
                Id = value;
            return this;
        }


        public Manifest SetAppNameIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(AppName) || IsPlaceHolder(AppName)) && !string.IsNullOrWhiteSpace(value))
                AppName = value;
            return this;
        }

        public Manifest SetOwnerIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Owner) || IsPlaceHolder(Owner)) && !string.IsNullOrWhiteSpace(value))
                Owner = value;
            return this;
        }

        public Manifest SetMinVersionIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(MinVersion) || IsPlaceHolder(MinVersion)) && !string.IsNullOrWhiteSpace(value))
                MinVersion = value;
            return this;
        }

        public Manifest SetReleaseUrlIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(ReleaseUrl) || IsPlaceHolder(ReleaseUrl)) && !string.IsNullOrWhiteSpace(value))
                ReleaseUrl = value;
            return this;
        }

        public Manifest SetVersionIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Version) || IsPlaceHolder(Version)) && !string.IsNullOrWhiteSpace(value))
                Version = value;
            return this;
        }

        public Manifest SetIconIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Icon) || IsPlaceHolder(Icon)) && !string.IsNullOrWhiteSpace(value))
                Icon = value;
            return this;
        }

        public Manifest SetIconUrlIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(IconUrl) || IsPlaceHolder(IconUrl)) && !string.IsNullOrWhiteSpace(value))
                IconUrl = value;
            return this;
        }

        public Manifest SetWebSiteIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(WebSite) || IsPlaceHolder(WebSite)) && !string.IsNullOrWhiteSpace(value))
                WebSite = value;
            return this;
        }

        public Manifest SetTagsIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Tags) || IsPlaceHolder(Tags)) && !string.IsNullOrWhiteSpace(value))
                Tags = value;
            return this;
        }

        public Manifest SetLanguageIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Language) || IsPlaceHolder(Language)) && !string.IsNullOrWhiteSpace(value))
                Language = value;
            return this;
        }

        public Manifest SetCopyrightIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Copyright) || IsPlaceHolder(Copyright)) && !string.IsNullOrWhiteSpace(value))
                Copyright = value;
            return this;
        }

        public Manifest SetReleaseNotesIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(ReleaseNotes) || IsPlaceHolder(ReleaseNotes)) && !string.IsNullOrWhiteSpace(value))
                ReleaseNotes = value;
            return this;
        }

        public Manifest SetDescriptionIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(Description) || IsPlaceHolder(Description)) && !string.IsNullOrWhiteSpace(value))
                Description = value;
            return this;
        }

        public Manifest SetEntryPointIfBlank(string value)
        {
            if ((string.IsNullOrWhiteSpace(EntryPoint) || IsPlaceHolder(EntryPoint)) && !string.IsNullOrWhiteSpace(value))
                EntryPoint = value;
            return this;
        }
        #endregion

    }
}