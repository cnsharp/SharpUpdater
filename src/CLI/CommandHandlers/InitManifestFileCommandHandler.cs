using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;

namespace CnSharp.Updater.CLI.CommandHandlers;

internal class InitManifestFileCommandHandler : ICommandHandler
{

    public int Invoke(InvocationContext context)
    {
        var dir = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(dir, Constants.ManifestFileName);
        if (File.Exists(filePath))
        {
            ConsoleExtensions.WriteError($"File '{filePath}' is already exists.");
            return -1;
        }
        File.WriteAllText(filePath, Templates.ManifestXml, Encoding.UTF8);
        context.Console.WriteLine($"File '{filePath}' created.");
        return 0;
    }
    
    public Task<int> InvokeAsync(InvocationContext context)
    {
        Invoke(context);
        return Task.FromResult(0);
    }
}