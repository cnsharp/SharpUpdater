using CnSharp.Updater.Util;

namespace CnSharp.Updater.CLI
{
    public class ProjectHolder
    {
        static readonly List<string> ProjectFileExtensions = [".csproj", ".vbproj"];
        static readonly List<string> ProjectFilePatterns = ProjectFileExtensions.Select(e => "*" + e).ToList();
        private static readonly Dictionary<string, string?> ProjectFileDict = new();
        private static readonly Dictionary<string, ProjectHelper> ProjectHelperDict = new();

        public static bool CheckExistsIfProject(string file)
        {
            if (ProjectFileExtensions.Any(file.EndsWith))
            {
                if (!File.Exists(file))
                {
                    throw new FileNotFoundException($"File {file} does not exist.");
                }
                return true;
            }
            if (!Directory.Exists(file))
            {
                throw new DirectoryNotFoundException($"Directory {file} does not exist.");
            }
            return false;
        }

        public static string? GetProjectFile(string dir)
        {
            if (!ProjectFileDict.ContainsKey(dir))
            {
                foreach (var pattern in ProjectFilePatterns)
                {
                    var projectFiles = Directory.GetFiles(dir, pattern, SearchOption.TopDirectoryOnly);
                    if (projectFiles.Length > 0)
                    {
                        ProjectFileDict.Add(dir, projectFiles[0]);
                        return projectFiles[0];
                    }
                }
               
                ProjectFileDict.Add(dir, null);
                return null;
            }
            return ProjectFileDict[dir];
        }

        public static ProjectHelper GetProjectHelper(string projectFile)
        {
            if (!ProjectHelperDict.ContainsKey(projectFile))
            {
                var helper = new ProjectHelper(projectFile);
                ProjectHelperDict.Add(projectFile, helper);
                return helper;
            }
            return ProjectHelperDict[projectFile];
        }
    }
}
