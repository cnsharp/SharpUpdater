using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CnSharp.Updater.Util
{
    public  class ManifestGatherer
    {
        private readonly string _releaseDir;
        private readonly string _projectDir;

        public ManifestGatherer(string releaseDir, string projectDir)
        {
            _releaseDir = releaseDir;
            _projectDir = projectDir;
        }

        public List<ReleaseFile> GatherFiles(bool deleteExclusions)
        {
            return GatherFilesInFolder(_releaseDir, deleteExclusions);
        }

        private List<ReleaseFile> GatherFilesInFolder(string dir, bool deleteExclusions)
        {
            var list = new List<ReleaseFile>();

            if (IsExcluded(dir))
            {
                if (deleteExclusions)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Delete directory {dir} failed: {ex.Message}");
                    }
                }
                return list;
            }

            string[] files = Directory.GetFiles(dir);
            foreach (string file in files)
            {
                if (IsExcluded(file))
                {
                    if (deleteExclusions)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Delete file {file} failed: {ex.Message}");
                        }
                    }
                    continue;
                }
                list.Add(new ReleaseFile
                {
                    FileName = file.Substring(dir.Length),
                    FileSize = new FileInfo(file).Length,
                    Version = FileVersionInfo.GetVersionInfo(file).FileVersion ?? "-"
                });
            }

            string[] folders = Directory.GetDirectories(dir);
            foreach (string folder in folders)
            {
                list.AddRange(GatherFilesInFolder(folder, deleteExclusions).ToArray());
            }
            return list;
        }

        private IgnoreFileParser _ignoreFileParser;

        private void GetIgnoreFileParser()
        {
            if (_ignoreFileParser != null)
                return;
            var ignoreFile = Path.Combine(_projectDir, Constants.IgnoreFileName);
            _ignoreFileParser = new IgnoreFileParser(File.Exists(ignoreFile) ? ignoreFile : null);
        }

        protected virtual bool IsExcluded(string file)
        {
            GetIgnoreFileParser();
            return _ignoreFileParser.IsExcluded(file);
        }
    }
}