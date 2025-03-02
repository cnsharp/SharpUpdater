using CnSharp.Updater.CLI.CommandHandlers;
using System.CommandLine;

namespace CnSharp.Updater.CLI
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand($"This is a packaging & deployment tool of {Constants.ProductName}.");
            rootCommand.AddCommand(NewSetGlobalSourceCommand());
            rootCommand.AddCommand(NewRemoveGlobalSourceCommand());
            rootCommand.AddCommand(NewInitManifestFileCommand());
            rootCommand.AddCommand(NewInitIgnoreFileCommand());
            rootCommand.AddCommand(NewPackCommand());
            rootCommand.AddCommand(NewPushCommand());
            return await rootCommand.InvokeAsync(args);
        }

        private static Command NewInitManifestFileCommand()
        {
            return new Command("init", $"Generate a {Constants.ManifestFileName} file in the current directory")
            {
                Handler = new InitManifestFileCommandHandler()
            };
        }

        private static Command NewInitIgnoreFileCommand()
        {
            return new Command("ignore", $"Generate a {Constants.IgnoreFileName} file in the current directory")
            {
                Handler = new InitIgnoreFileCommandHandler()
            };
        }

        private static Command NewSetGlobalSourceCommand()
        {
            var sourceOption = new Option<string>("--source", "Set the global SharpUpdater.Server source URL")
            {
                IsRequired = true
            };
            sourceOption.AddAlias("-s");

            var command = new Command("global", "Set the global settings")
            {
                sourceOption
            };
            command.SetHandler(GlobalSourceCommandHandler.Set, sourceOption);
            return command;
        }

        private static Command NewRemoveGlobalSourceCommand()
        {
            var command = new Command("RemoveSource", "Remove the global source");
            command.SetHandler(GlobalSourceCommandHandler.Remove);
            return command;
        }

        private static Command NewPackCommand()
        {
            var sourceOption = new Option<string>("--source", "Specify the SharpUpdater.Server source URL");
            sourceOption.AddAlias("-s");

            var projectDirOption = new Option<string>("--project", "Specify the project directory");
            projectDirOption.AddAlias("-p");

            var outputDirOption = new Option<string>("--output", () => $"bin\\{Constants.ProductName}\\", "Specify the output directory");
            outputDirOption.AddAlias("-o");

            var versionOption = new Option<string>("--version", "Specify the package version");
            versionOption.AddAlias("-v");

            var minVersionOption = new Option<string>("--MinimumVersion", "Specify the minimum version must be updated");
            minVersionOption.AddAlias("-mv");

            var releaseNotesOption = new Option<string>("--ReleaseNotes", "Input release notes");
            releaseNotesOption.AddAlias("-rn");

            var noBuildOption = new Option<bool>("--no-build", "Skip build");

            var command = new Command("pack", "Pack the project")
            {
                sourceOption,
               projectDirOption,
               outputDirOption,
               versionOption,
               minVersionOption,
               releaseNotesOption,
               noBuildOption
            };
            command.SetHandler(PackCommandHandler.Invoke,
                sourceOption,
                projectDirOption,
               outputDirOption,
               versionOption,
               minVersionOption,
               releaseNotesOption,
               noBuildOption);
            return command;
        }

        private static Command NewPushCommand()
        {
            var packageOption = new Option<string>("--package", "Specify the .sp file path")
            {
                IsRequired = true
            };
            packageOption.AddAlias("-p");

            var sourceOption = new Option<string>("--source", "Specify the SharpUpdater.Server source URL");
            sourceOption.AddAlias("-s");

            var keyOption = new Option<string>("--apikey", "Specify the ApiKey of SharpUpdater.Server")
            {
                IsRequired = true
            };
            keyOption.AddAlias("-k");

            var pushCommand = new Command("push", "Push .sp package to SharpUpdater.Server")
            {
               packageOption,
               sourceOption,
               keyOption
            };
            pushCommand.SetHandler(PushCommandHandler.Invoke,
                packageOption,
               sourceOption,
               keyOption);
            return pushCommand;
        }
    }

}