using CnSharp.Updater.Util;
using System.Collections.Generic;
using System.IO;

namespace CnSharp.VisualStudio.SharpUpdater.Util
{
    public  class ManifestGatherer
    {

        private int _rootLength;
        private readonly string _dir;
        private string _projectDir;
        private readonly NuPackSettings _settings;

        public ManifestGatherer(string dir, string projectDir, NuPackSettings settings)
        {
            _rootLength = dir.Length;
            _dir = dir;
            _projectDir = projectDir;
            _settings = settings;
        }

        public List<FileListItem> GatherFiles()
        {
          
            return GatherFilesInFolder(_dir, true,_settings.UnselectedFolders,_settings.UnselectedFiles);
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

        private IgnoreFileParser _filter;

        private void GetManifestFilter()
        {
            if (_filter != null)
                return;
            var ignoreFile = Path.Combine(_projectDir,Common.IgnoreFileName);
            _filter = new IgnoreFileParser(File.Exists(ignoreFile) ? ignoreFile : null);
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
}