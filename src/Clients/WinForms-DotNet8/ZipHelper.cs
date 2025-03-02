using System.IO;
using System.IO.Packaging;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public class ZipHelper
    {
        public static void Unzip(string compressedFileName, string folderName, bool overrideExisting)
        {
            var directoryInfo = new DirectoryInfo(folderName);
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            using (var package = Package.Open(compressedFileName, FileMode.Open, FileAccess.Read))
            {
                foreach (var packagePart in package.GetParts())
                {
                    ExtractPart(packagePart, folderName, overrideExisting);
                }
            }
        }

        private static void ExtractPart(PackagePart packagePart, string targetDirectory, bool overrideExisting)
        {
            var stringPart = targetDirectory + packagePart.Uri.ToString().Replace('\\', '/');

            if (!Directory.Exists(Path.GetDirectoryName(stringPart)))
                Directory.CreateDirectory(Path.GetDirectoryName(stringPart));

            if (!overrideExisting && File.Exists(stringPart))
                return;
            using (var fileStream = new FileStream(stringPart, FileMode.Create))
            {
                packagePart.GetStream().CopyTo(fileStream);
            }
        }

       
    }
}