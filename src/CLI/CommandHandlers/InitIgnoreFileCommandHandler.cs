using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text;

namespace CnSharp.Updater.CLI.CommandHandlers;

internal class InitIgnoreFileCommandHandler : ICommandHandler
{

    public int Invoke(InvocationContext context)
    {
        var dir = Directory.GetCurrentDirectory();
        string ignoreFilePath = Path.Combine(dir, Constants.IgnoreFileName);
        if (File.Exists(ignoreFilePath))
        {
            ConsoleExtensions.WriteError($"File '{ignoreFilePath}' is already exists.");
            return -1;
        }
        File.WriteAllText(ignoreFilePath, Templates.IgnoreFiles, Encoding.UTF8);
        context.Console.WriteLine($"File '{ignoreFilePath}' created.");
        return 0;
    }
    
    public Task<int> InvokeAsync(InvocationContext context)
    {
        Invoke(context);
        return Task.FromResult(0);
    }
}