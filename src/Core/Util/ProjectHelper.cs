using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CnSharp.Updater.Util
{
    public class ProjectHelper
    {
        private readonly XDocument _doc;
        private readonly string _projectFile;

        public ProjectHelper(string projectFile)
        {
            _doc = XDocument.Load(projectFile);
            _projectFile = projectFile;
        }

        public string ProjectFile => _projectFile;

        public bool IsDotnetFramework
        {
            get
            {
                var ns = _doc.Root.GetDefaultNamespace();
                return _doc.Descendants(ns + "TargetFrameworkVersion").FirstOrDefault() != null;
            }
        }

        public string GetTargetFramework()
        {
            return _doc.Element("Project")
                 ?.Element("PropertyGroup")
                 ?.Element("TargetFramework")
                 ?.Value;
        }

        public string GetAssemblyName()
        {
            return _doc.Element("Project")
                ?.Element("PropertyGroup")
                ?.Element("AssemblyName")
                ?.Value;
        }

        public string GetReleaseDir()
        {
            var dir = Path.GetDirectoryName(_projectFile);
            var path = Path.Combine(dir, "bin", "Release");
            var targetFramework = GetTargetFramework();
            if (!string.IsNullOrEmpty(targetFramework))
                path = Path.Combine(path, targetFramework);
            return path;
        }

        public string GetBinDir()
        {
            var dir = Path.GetDirectoryName(_projectFile);
            return Path.Combine(dir, "bin");
        }

        public string GetExeFileReleased()
        {
            var dir = GetReleaseDir();
            if (!Directory.Exists(dir))
                throw new DirectoryNotFoundException("Release directory not found.");
            string exe;
            var assemblyName = GetAssemblyName();
            if (assemblyName != null)
            {
                exe = Path.Combine(dir, assemblyName, ".exe");
                if (!File.Exists(exe))
                    throw new FileNotFoundException("None of .exe file found in Release directory.");
                return exe;
            }
            var files = Directory.GetFiles(dir, "*.exe", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                throw new FileNotFoundException("None of .exe file found in Release directory.");
            exe = files.Where(f => !Path.GetFileName(f).Equals(Constants.UpdaterFileName, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (exe == null)
                throw new FileNotFoundException("None of valid .exe file found in Release directory.");
            return exe;
        }
    }
}
