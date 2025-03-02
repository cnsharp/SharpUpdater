using System;
using System.Text.RegularExpressions;

namespace CnSharp.Updater.Util
{
    public static class VersionUtil
    {

        public const string SemanticVersionPattern =
            "^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$";

        private static readonly Regex SemanticVersionRegex = new Regex(SemanticVersionPattern, RegexOptions.Compiled);
        public static int CompareVersion(this string version, string otherVersion)
        {
            return CompareVersions(version, otherVersion);
        }

        public static int CompareVersions(string version1, string version2)
        {
            // Split version number and pre-release identifier
            var v1Parts = version1.Split('-');
            var v2Parts = version2.Split('-');

            // Parse the main version number
            Version v1 = Version.Parse(v1Parts[0]);
            Version v2 = Version.Parse(v2Parts[0]);

            // Compare the main version numbers
            int mainComparison = v1.CompareTo(v2);
            if (mainComparison != 0)
                return mainComparison;

            // If the main version numbers are the same, compare the pre-release identifiers
            string preRelease1 = v1Parts.Length > 1 ? v1Parts[1] : string.Empty;
            string preRelease2 = v2Parts.Length > 1 ? v2Parts[1] : string.Empty;

            return ComparePreRelease(preRelease1, preRelease2);
        }

        private static int ComparePreRelease(string preRelease1, string preRelease2)
        {
            // If neither has a pre-release identifier, they are equal
            if (string.IsNullOrEmpty(preRelease1) && string.IsNullOrEmpty(preRelease2))
                return 0;

            // If one version has a pre-release identifier, it is considered smaller
            if (string.IsNullOrEmpty(preRelease1))
                return 1; // version1 is a stable release, greater than version2
            if (string.IsNullOrEmpty(preRelease2))
                return -1; // version2 is a stable release, greater than version1

            // Compare pre-release identifiers
            return string.Compare(preRelease1, preRelease2, StringComparison.OrdinalIgnoreCase);
        }

        public static string ToSemanticVersion(this string versionNumber)
        {
            if (versionNumber.IsSemanticVersion())
                return versionNumber;
            // Split the version number
            string[] parts = versionNumber.Split('.');

            // Ensure at least MAJOR.MINOR.PATCH
            if (parts.Length < 3)
            {
                throw new ArgumentException("Version number must have at least 3 parts (MAJOR.MINOR.PATCH).");
            }

            // Extract MAJOR, MINOR, PATCH
            string major = parts[0];
            string minor = parts[1];
            string patch = parts[2];

            // Build the Semantic Version
            return $"{major}.{minor}.{patch}";
        }

       public static bool IsSemanticVersion(this string version)
        {
            return SemanticVersionRegex.IsMatch(version);
        }
    }
}
