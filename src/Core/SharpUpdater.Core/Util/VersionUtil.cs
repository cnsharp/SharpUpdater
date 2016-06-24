using System;

namespace CnSharp.Updater.Util
{
    public static class VersionUtil
    {

        public static int CompareVersion(this string version, string otherVersion)
        {
            var v = new Version(version);
            var ov = new Version(otherVersion);
            return v.CompareTo(ov);
        }


        public static string Shorten(this string version,string trimEndString = ".*")
        {
            var chars = trimEndString.ToCharArray();
            var num = version;
            while (num.EndsWith(trimEndString))
            {
                for (var i = chars.Length - 1; i >= 0; i--)
                {
                    num = num.TrimEnd(chars[i]);
                }
               if (trimEndString == ".0" && num.IndexOf(".", StringComparison.Ordinal) < 0)//.0 keep one at least
                    break;
            }
            return version;
        }

        public const string VersionNumberRegex = @"^[1-9]\d*(\.\d+){1,3}$";
    }
}
