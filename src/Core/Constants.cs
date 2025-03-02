namespace CnSharp.Updater
{
    public static class Constants
    {
        public const string ApiKeyHeader = "X-SHARPUPDATER-APIKEY";
        public const string HashAlgorithm = "SHA512";
        public const string ProductName = "SharpUpdater";
        public const string ManifestRelationType = "manifest";
        public const string ManifestExtension = Manifest.ManifestExt;
        public const string ManifestFileName = Manifest.ManifestFileName;
        public const string IgnoreFileName = ProductName+".ignore";
        public const string PackageRelationshipNamespace = "http://schemas.cnsharp.com/sharpupdater/2016/06/";
        public const string UriId = "sp";
        public const string UpdaterFileName = "updater.exe";

        /// <summary>
        /// Represents the ".sp" extension.
        /// </summary>
        public const string PackageExtension = Manifest.PackageFileExt;

        /// <summary>
        /// Represents the ".sp.sha512" extension.
        /// </summary>
        public const string HashFileExtension = PackageExtension + ".sha512";
    }
}