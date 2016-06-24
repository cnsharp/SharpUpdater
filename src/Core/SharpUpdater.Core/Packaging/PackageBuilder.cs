using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using CnSharp.Updater.Util;

namespace CnSharp.Updater.Packaging
{
    public class PackageBuilder
    {
        private readonly Manifest _manifest;
        private readonly string _baseDir;
        private const string DefaultContentType = "application/octet";
        public const string ManifestRelationType = "manifest";
        public const string PackageRelationshipNamespace = "http://schemas.cnsharp.com/sharpupdater/2016/06/";

        public PackageBuilder(Manifest manifest,string baseDir)
        {
            _manifest = manifest;
            _baseDir = baseDir;
        }

        public Package CreatePackage(string targetPath)
        {
            using (Package package = Package.Open(targetPath, FileMode.Create))
            {
                WriteManifest(package,1);
                WriteFiles(package);

                package.PackageProperties.Creator = _manifest.Owner;
                package.PackageProperties.Description = _manifest.Description;
                package.PackageProperties.Identifier = _manifest.Id;
                package.PackageProperties.Version = _manifest.Version;
                package.PackageProperties.Language = _manifest.Language;
                package.PackageProperties.Keywords = _manifest.Tags;
                package.PackageProperties.Title = _manifest.AppName;
                package.PackageProperties.LastModifiedBy = CreatorInfo();

                return package;
            }
        }


        private static string CreatorInfo()
        {
            List<string> creatorInfo = new List<string>();
            var assembly = typeof(PackageBuilder).Assembly;
            creatorInfo.Add(assembly.FullName);
            creatorInfo.Add(Environment.OSVersion.ToString());

            return String.Join(";", creatorInfo.ToArray());
        }



        private void WriteManifest(Package package, int minimumManifestVersion)
        {
            Uri uri = UriUtility.CreatePartUri(Manifest.ManifestFileName);

            // Create the manifest relationship
            package.CreateRelationship(uri, TargetMode.Internal, PackageRelationshipNamespace + ManifestRelationType);

            // Create the part
            PackagePart packagePart = package.CreatePart(uri, DefaultContentType, CompressionOption.Maximum);

            using (Stream stream = packagePart.GetStream())
            {
                _manifest.Save(stream, minimumManifestVersion);
            }
        }

        private void WriteFiles(Package package)
        {
            var files = Directory.GetFiles(_baseDir, @"*.*", SearchOption.AllDirectories);
            var dirLen = _baseDir.Length;
            foreach (var each in files)
            {

                var relPath = each.Substring(dirLen).Replace("\\", "/");
                if (relPath.StartsWith("/"))
                    relPath = relPath.TrimStart('/');
                string destFilename = "/" + relPath;
                Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                if (package.PartExists(uri))
                {
                    package.DeletePart(uri);
                }
                PackagePart part = package.CreatePart(uri, "", CompressionOption.Normal);
                using (FileStream fileStream = new FileStream(each, FileMode.Open, FileAccess.Read))
                {
                    using (Stream dest = part.GetStream())
                    {
                        fileStream.CopyTo(dest);
                    }
                }
            }
        }
    }


    public static class UriUtility
    {
        /// <summary>
        /// Converts a uri to a path. Only used for local paths.
        /// </summary>
        public static string GetPath(Uri uri)
        {
            string path = uri.OriginalString;
            if (path.StartsWith("/", StringComparison.Ordinal))
            {
                path = path.Substring(1);
            }

            // Bug 483: We need the unescaped uri string to ensure that all characters are valid for a path.
            // Change the direction of the slashes to match the filesystem.
            return Uri.UnescapeDataString(path.Replace('/', Path.DirectorySeparatorChar));
        }

        public static Uri CreatePartUri(string path)
        {
            // Only the segments between the path separators should be escaped
            var segments = path.Split(new[] { '/', Path.DirectorySeparatorChar }, StringSplitOptions.None)
                               .Select(Uri.EscapeDataString);
            var escapedPath = String.Join("/", segments.ToArray());
            return PackUriHelper.CreatePartUri(new Uri(escapedPath, UriKind.Relative));
        }

        // Bug 2379: SettingsCredentialProvider does not work
        private static Uri CreateODataAgnosticUri(string uri)
        {
            if (uri.EndsWith("$metadata", StringComparison.OrdinalIgnoreCase))
            {
                uri = uri.Substring(0, uri.Length - 9).TrimEnd('/');
            }
            return new Uri(uri);
        }

        /// <summary>
        /// Determines if the scheme, server and path of two Uris are identical.
        /// </summary>
        public static bool UriEquals(Uri uri1, Uri uri2)
        {
            uri1 = CreateODataAgnosticUri(uri1.OriginalString.TrimEnd('/'));
            uri2 = CreateODataAgnosticUri(uri2.OriginalString.TrimEnd('/'));

            return Uri.Compare(uri1, uri2, UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    
}
