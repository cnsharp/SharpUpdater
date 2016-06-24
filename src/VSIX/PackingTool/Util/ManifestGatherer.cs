using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CnSharp.VisualStudio.SharpDeploy.Util
{
    public  class ManifestGatherer
    {

        private int _rootLength;
        private string _projectDir;

        public List<FileListItem> GatherFiles(string dir, string projectDir,NuPackSettings settings)
        {
            _rootLength = dir.Length;
            _projectDir = projectDir;
            return GatherFilesInFolder(dir, true,settings.UnselectedFolders,settings.UnselectedFiles);
        }

        private List<FileListItem> GatherFilesInFolder(string dir, bool isFirst,IList<string> unselectedFolders,IList<string> unselectedFiles )
        {
            var list = new List<FileListItem>();

            if (IsExcluded(dir))
                return list;

            string dirShortName = dir.Substring(_rootLength);
            bool folderUnselected = unselectedFolders?.Contains(dirShortName) ?? false;
            if (!isFirst)
            {
                var folderItem = new FileListItem { Dir = dir, Selected = !folderUnselected };
                list.Add(folderItem);
            }

            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                if (IsExcluded(file))
                    continue;
                string shortName = dir.Substring(_rootLength);
                bool selected = !folderUnselected &&
                                (unselectedFiles == null || !unselectedFiles.Contains(shortName))
                                && !IsFileUnselected(file);
                list.Add(new FileListItem
                {
                    Dir = file,
                    IsFile = true,
                    Selected = selected
                });
            }

            string[] folders = Directory.GetDirectories(dir);
            foreach (string folder in folders)
            {
                list.AddRange(GatherFilesInFolder(folder, false,unselectedFolders,unselectedFiles).ToArray());
            }
            return list;
        }

        private ManifestFilter _filter;

        private void GetManifestFilter()
        {
            if (_filter != null)
                return;
            var ignoreFile = Path.Combine(_projectDir,Common.IgnoreFileName);
            _filter = new ManifestFilter( File.Exists(ignoreFile) ? ignoreFile : null);
        }

        protected virtual bool IsExcluded(string file)
        {
            GetManifestFilter();
            return _filter.IsExcluded(file);
        }

        protected virtual bool IsFileUnselected(string file)
        {
            var ext = Path.GetExtension(file).ToLower();
            return (ext == ".xml" && File.Exists(file.Substring(0, file.Length - 4) + ".dll"));
        }
    }

    public class ManifestFilter
    {
      
        private readonly string _ignoreFile;
        private List<string> _files = new List<string> { "updater.exe" };
        private List<string> _regex = new List<string>();

        public ManifestFilter(string ignoreFile)
        {
            if (string.IsNullOrWhiteSpace(ignoreFile))
                return;

            if (!File.Exists(ignoreFile))
                throw new FileNotFoundException(".ignore file not found.",ignoreFile);
         
            _ignoreFile = ignoreFile;
            using (var sr = new StreamReader(_ignoreFile))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    if (wildCardRegex.IsMatch(line))
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

        static readonly Regex wildCardRegex = new Regex("[.$^{\\[(|)*+?\\\\]");

        /// <summary>
        /// 将通配符字符串转换成等价的正则表达式
        /// 这可以用正则表达式来实现通配符匹配
        /// </summary>
        /// <see cref="http://dearymz.blog.163.com/blog/static/205657420081122103757583/">《C#正则转义》</see>
        /// <see cref="http://dearymz.blog.163.com/blog/static/2056574200722355218155/">《C#实现DOS通配符“*”和“/”的识别》</see>
        static string GetWildcardRegexString(string wildcardStr)
        {

            return wildCardRegex.Replace(wildcardStr,

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
