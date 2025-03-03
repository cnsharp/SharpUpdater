using CnSharp.VisualStudio.Extensions;
using CnSharp.VisualStudio.SharpUpdater.Util;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CnSharp.VisualStudio.SharpUpdater.Commands;
using Task = System.Threading.Tasks.Task;

namespace CnSharp.VisualStudio.SharpUpdater
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(SharpUpdaterPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class SharpUpdaterPackage : AsyncPackage
    {
        /// <summary>
        /// SharpUpdaterPackage GUID string.
        /// </summary>
        public const string PackageGuidString = PackageGuids.guidSharpUpdaterPackageString;

        public SharpUpdaterPackage()
        {
            RedirectAssembly("System.IO.Packaging", new Version("9.0.0.2"), "b03f5f7f11d50a3a");
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var dte = GetGlobalService(typeof(DTE)) as DTE2;
            Host.Instance.DTE = dte;

            bool isSolutionLoaded = await IsSolutionLoadedAsync();

            if (isSolutionLoaded)
            {
                HandleOpenSolution();
            }

            // Listen for subsequent solution events
            Microsoft.VisualStudio.Shell.Events.SolutionEvents.OnAfterOpenSolution += HandleOpenSolution;


            dte.Events.SolutionEvents.ProjectAdded += p =>
            {
                if (string.IsNullOrWhiteSpace(p.FileName) ||
                    !Common.SupportedProjectTypes.Any(t => p.FileName.EndsWith(t, StringComparison.OrdinalIgnoreCase)))
                    return;
                var sln = Host.Instance.Solution2;
                SolutionDataCache.Instance.TryGetValue(sln.FileName, out var sp);
                sp?.AddProject(p);
            };
            dte.Events.SolutionEvents.ProjectRemoved += p =>
            {
                var sln = Host.Instance.Solution2;
                SolutionDataCache.Instance.TryGetValue(sln.FileName, out var sp);
                sp?.RemoveProject(p);
            };



            await AddIgnoreFileCommand.InitializeAsync(this);
            await AddManifestFileCommand.InitializeAsync(this);
            await SharpPackCommand.InitializeAsync(this);

        }

        private async Task<bool> IsSolutionLoadedAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();
            var solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object value));

            return value is bool isSolOpen && isSolOpen;
        }

        private void HandleOpenSolution(object sender = null, EventArgs e = null)
        {
            var sln = Host.Instance.Solution2;
            var projects = Host.Instance.DTE.GetSolutionProjects()
                .Where(
                    p =>
                        !string.IsNullOrWhiteSpace(p.FileName) &&
                        Common.SupportedProjectTypes.Any(
                            t => p.FileName.EndsWith(t, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            var sp = new SolutionProperties
            {
                Projects = projects
            };
            SolutionDataCache.Instance.AddOrUpdate(sln.FileName, sp, (k, v) =>
            {
                v = sp;
                return v;
            });
        }


        #endregion

        public static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            ResolveEventHandler handler = null;

            handler = (sender, args) => {
                // Use latest strong name & version when trying to load SDK assemblies
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;

                Debug.WriteLine("Redirecting assembly load of " + args.Name
                                                                + ",\tloaded by " + (args.RequestingAssembly == null ? "(unknown)" : args.RequestingAssembly.FullName));

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName(shortName + ", PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= handler;

                return Assembly.Load(requestedAssembly);
            };
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }
    }
}
