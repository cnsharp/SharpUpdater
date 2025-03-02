using CnSharp.Updater;
using CnSharp.Updater.Util;

namespace CnSharp.Updater.CLI.CommandHandlers;

internal class PushCommandHandler
{
    public static async Task Invoke(string packageFile, string source, string apiKey)
    {
        var globalSource = Settings.Load()?.GlobalSource;
        var currentSource = !string.IsNullOrWhiteSpace(source) ? source : globalSource;
        if (string.IsNullOrWhiteSpace(currentSource))
        {
            ConsoleExtensions.WriteError("--source is required.");
            return;
        }
        Console.WriteLine("Start...");
        try
        {
            await PackageUploader.Upload(packageFile, currentSource, apiKey);
        }
        catch(Exception e)
        {
            ConsoleExtensions.WriteError(e.Message);
            return;
        }

        Console.WriteLine("Push successfully.");
    }
}