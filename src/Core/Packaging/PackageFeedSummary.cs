using System;

namespace CnSharp.Updater.Packaging
{
    public class PackageFeedSummary
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public DateTimeOffset Updated { get; set; }
        public string ReleaseNotes { get; set; }
        public string PackageUrl { get; set; }
        public long PackageSize { get; set; }

        public bool IsNew(Manifest manifest)
        {
            return manifest.Version != Version;
        }
    }
}