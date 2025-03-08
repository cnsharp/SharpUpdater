# SharpUpdater
A windows application automatic update solution,includes updater clients,update pack repository,CLI and VSIX packaging tool.

## Quick start

### Build your desktop application
Take winforms for example.

Fork this repo and clone it to your local machine.Then you can find the clients source code in the location [\src\Clients](src/Clients).

The clients are not pulished as NuGet package so far, so you can customize the updater as you need.

Build your updater.exe,publish the client in a single file.

Create a new winforms project, and integrate the updater.exe by adding it to the main project and set its property 'Copy to Output Directory' as 'Copy Always'.

Start the updater.exe in the main program.
```csharp
static void Main(string[] args)
{
    ApplicationConfiguration.Initialize();

    // Check if the application is running from the updater, if not, run the updater.
    // The updater.exe will pass the args to the application after updating to avoid checking again.
    if (args.Length == 0)
    {
        var updaterPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe");
        if (File.Exists(updaterPath))
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "Updater.exe"
            };
            var proc = Process.Start(processStartInfo);
            proc?.WaitForExit();
            return;
        }
    }

    Application.Run(new MainForm());
}
```

### Publish your application 
This solution focuses on On-premises deployment, so you can publish your application as a pure exe file or make a installer using Windows Application Packaging Project,
Visual Studio Install Project,Inno Setup, etc.

### Create a update pack
There are two approaches to create a update pack.
1. Use the [VSIX packaging tool](https://marketplace.visualstudio.com/items?itemName=CnSharpStudio.SharpUpdater) of SharpUpdater.
2. Use [SharpUpdater.CLI](https://www.nuget.org/packages/SharpUpdater.CLI).
```
dotnet tool install --global SharpUpdater.CLI
```

### Deploy the update pack
First, you need to deploy a [SharpUpdater.Server](https://github.com/cnsharp/SharpUpdater.Server) to host the update packs.
Then, you can deploy the update pack to the server by using the SharpUpdater.CLI or the VSIX.
The command like below:
```
su push -p C:\path\to\your\updatepack.sp  -s http://YOUR_SERVER_HOST/sp -k YOUR_KEY
```