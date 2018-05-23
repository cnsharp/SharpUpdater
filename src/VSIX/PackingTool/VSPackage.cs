using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CnSharp.VisualStudio.Extensions;
using CnSharp.VisualStudio.SharpDeploy.Commands;
using CnSharp.VisualStudio.SharpDeploy.Util;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace CnSharp.VisualStudio.SharpDeploy
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class VSPackage : AsyncPackage
    {
        /// <summary>
        ///     VSPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "85ecc0d6-9c3a-432f-ac95-7a9583195cf5";

        #region Overrides of AsyncPackage

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
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

            SharpPackCommand.Initialize(this);
            AddIgnoreFileCommand.Initialize(this);
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

    }
}