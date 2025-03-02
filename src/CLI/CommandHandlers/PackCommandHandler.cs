using System.Diagnostics;
using CnSharp.Updater.Packaging;
using CnSharp.Updater.Util;
using NuGet.Versioning;

namespace CnSharp.Updater.CLI.CommandHandlers;

internal class PackCommandHandler
{
    public static async Task Invoke(string source, string? project, string output, string? version, string? minimumVersion, string releaseNotes, bool noBuild)
    {
        SemanticVersion? v = null;
        if (!string.IsNullOrWhiteSpace(version) && !SemanticVersion.TryParse(version, out v))
        {
            ConsoleExtensions.WriteError("Version number is not semantic.");
            return;
        }

        SemanticVersion? mv = null;
        if (!string.IsNullOrWhiteSpace(minimumVersion) && !SemanticVersion.TryParse(minimumVersion, out mv))
        {
            ConsoleExtensions.WriteError("Minimum version number is not semantic.");
            return;
        }
        var dir = project ?? Directory.GetCurrentDirectory();
        string projectFile;
        if (ProjectHolder.CheckExistsIfProject(dir))
        {
            projectFile = dir;
            dir = Path.GetDirectoryName(dir);
        }
        else
        {
            projectFile = ProjectHolder.GetProjectFile(dir);
            if (projectFile == null)
            {
                ConsoleExtensions.WriteError("Project file not found.");
                return;
            }
        }

        var manifest = GetManifest(dir);
        var globalSource = Settings.Load()?.GlobalSource;
        var currentSource = !string.IsNullOrWhiteSpace(source) ? source : globalSource;
        if (string.IsNullOrWhiteSpace(manifest.ReleaseUrl) && string.IsNullOrWhiteSpace(currentSource))
        {
            ConsoleExtensions.WriteError("--source is required.");
            return;
        }

        var projectHelper = ProjectHolder.GetProjectHelper(projectFile);
        if (!noBuild)
        {
            Console.WriteLine("Building...");
            if (!Build(projectHelper))
            {
                return;
            }
        }

        var releaseDir = projectHelper.GetReleaseDir();
        var exeFile = projectHelper.GetExeFileReleased();
        UpdateManifest(manifest, exeFile, releaseDir, dir, currentSource, v, mv, releaseNotes);
        var outputPath = output;
        if (string.IsNullOrWhiteSpace(outputPath))
            outputPath = $"bin\\{Constants.ProductName}";
        var outputDir = EnsureOutputDir(dir, outputPath);
        var zipName = $"{manifest.AppName}_{manifest.Version}{Manifest.PackageFileExt}";
        var zipPath = $"{outputDir}\\{zipName}";
        var pb = new PackageBuilder(manifest, releaseDir);
        pb.CreatePackage(zipPath);
        Console.WriteLine($"Package file generated in {zipPath}.");
    }

    protected static string EnsureOutputDir(string baseDir, string outputDir)
    {
        var dir = outputDir.Trim().Replace('/', Path.DirectorySeparatorChar);
        dir = FileUtil.JoinFilePath(baseDir, dir);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return dir;
    }

    private static Manifest GetManifest(string projectDir)
    {
        var manifestFilePath = Path.Combine(projectDir, Constants.ManifestFileName);
        return File.Exists(manifestFilePath) ? XmlSerializerHelper.LoadObjectFromXml<Manifest>(manifestFilePath) : new Manifest();
    }

    private static void UpdateManifest(Manifest manifest, string exeFile, string releaseDir, string projectDir, string? source, SemanticVersion? version, SemanticVersion? minVersion, string releaseNotes)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(exeFile);
        var fileVersion = fileVersionInfo.FileVersion ?? fileVersionInfo.ProductVersion;
        var packageVersion = version?.ToString() ?? fileVersion.ToSemanticVersion();
        var fileName = Path.GetFileNameWithoutExtension(fileVersionInfo.FileName);
        manifest.SetIdIfBlank(fileName);
        manifest.SetAppNameIfBlank(!string.IsNullOrWhiteSpace(fileVersionInfo.ProductName) ? fileVersionInfo.ProductName : fileName);
        manifest.SetCopyrightIfBlank(fileVersionInfo.LegalCopyright);
        manifest.SetDescriptionIfBlank(fileVersionInfo.FileDescription);
        manifest.SetEntryPointIfBlank(Path.GetFileName(exeFile));
        manifest.SetIdIfBlank(fileVersionInfo.CompanyName);
        manifest.SetVersionIfBlank(packageVersion);
        if (!string.IsNullOrWhiteSpace(source))
            manifest.ReleaseUrl = source;
        if (!string.IsNullOrWhiteSpace(releaseNotes))
            manifest.ReleaseNotes = releaseNotes;
        manifest.MinVersion = minVersion?.ToString() ?? manifest.Version;
        manifest.Files = new ManifestGatherer(releaseDir, projectDir).GatherFiles(true);
        var manifestFilePath = Path.Combine(releaseDir, Manifest.ManifestFileName);
        manifest.Save(manifestFilePath);
    }

    private static bool Build(ProjectHelper projectHelper)
    {
        if (projectHelper.IsDotnetFramework)
            return CmdHelper.Run("msbuild", $"\"{projectHelper.ProjectFile}\" /p:Configuration=Release", Console.WriteLine, Console.WriteLine);
        return CmdHelper.Run("dotnet", $" build \"{projectHelper.ProjectFile}\" --configuration Release", Console.WriteLine, Console.WriteLine);
    }
}
