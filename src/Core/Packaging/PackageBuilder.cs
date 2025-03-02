using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using CnSharp.Updater.Util;

namespace CnSharp.Updater.Packaging
{
    public class PackageBuilder
    {
        private readonly Manifest _manifest;
        private readonly string _baseDir;
        private const string DefaultContentType = "application/octet";

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

            return string.Join(";", creatorInfo.ToArray());
        }



        private void WriteManifest(Package package, int minimumManifestVersion)
        {
            Uri uri = UriUtility.CreatePartUri(Manifest.ManifestFileName);

            // Create the manifest relationship
            package.CreateRelationship(uri, TargetMode.Internal, Constants.PackageRelationshipNamespace + Constants.ManifestRelationType);

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

    
}
