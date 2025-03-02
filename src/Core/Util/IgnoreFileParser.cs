using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CnSharp.Updater.Util
{
    public class IgnoreFileParser
    {
        private readonly List<string> _files = new List<string> { "updater.exe", Manifest.ManifestFileName, ".pdb" };
        private readonly List<string> _regex = new List<string>();

        public IgnoreFileParser(string ignoreFile)
        {
            if (string.IsNullOrWhiteSpace(ignoreFile))
                return;

            if (!File.Exists(ignoreFile))
                throw new FileNotFoundException(".ignore file not found.", ignoreFile);

            using (var sr = new StreamReader(ignoreFile))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    if (WildCardRegex.IsMatch(line))
                    {
                        var regex = GetWildcardRegexString(line);
                        _regex.Add(regex);
                    }
                    else
                    {
                        _files.Add(line);
                    }
                }
            }
        }

        public bool IsExcluded(string dir)
        {
            if (_files.Any(f => dir.EndsWith(f, StringComparison.CurrentCultureIgnoreCase)))
                return true;
            if (_regex.Any(r => Regex.IsMatch(dir, r, RegexOptions.Compiled | RegexOptions.IgnoreCase)))
                return true;
            return false;
        }

        static readonly Regex WildCardRegex = new Regex("[.$^{\\[(|)*+?\\\\]");

        /// <summary>
        /// 将通配符字符串转换成等价的正则表达式
        /// 这可以用正则表达式来实现通配符匹配
        /// </summary>
        static string GetWildcardRegexString(string wildcardStr)
        {

            return WildCardRegex.Replace(wildcardStr,
                delegate (Match m)
                {
                    switch (m.Value)
                    {

                        case "?":
                            return ".?";

                        case "*":
                            return ".*";

                        default:
                            return "\\" + m.Value;

                    }
                });
        }
    }
}
