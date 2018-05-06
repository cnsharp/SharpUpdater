//------------------------------------------------------------------------------
// <copyright file="VSPackage.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using CnSharp.VisualStudio.Extensions;
using CnSharp.VisualStudio.SharpDeploy.Commands;
using CnSharp.VisualStudio.SharpDeploy.Util;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace CnSharp.VisualStudio.SharpDeploy
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The minimum requirement for a class to be considered a valid package for Visual Studio
    ///         is to implement the IVsPackage interface and register itself with the shell.
    ///         This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///         to do it: it derives from the Package class that provides the implementation of the
    ///         IVsPackage interface and uses the registration attributes defined in the framework to
    ///         register itself and its components with the shell. These attributes tell the pkgdef creation
    ///         utility what data to put into .pkgdef file.
    ///     </para>
    ///     <para>
    ///         To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
    ///         &gt; in .vsixmanifest file.
    ///     </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists)]
    public sealed class VSPackage : Package
    {
        /// <summary>
        ///     VSPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "85ecc0d6-9c3a-432f-ac95-7a9583195cf5";

        #region Package Members

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();


            var dte = GetGlobalService(typeof (DTE)) as DTE2;
            Host.Instance.DTE = dte;
            Host.Instance.SolutionOpendAction = () =>
            {
                var sln = Host.Instance.Solution2;
                var projects = dte.GetSolutionProjects().Where(p => !string.IsNullOrWhiteSpace(p.FileName) && Common.SupportedProjectTypes.Any(t => p.FileName.EndsWith(t, StringComparison.OrdinalIgnoreCase))).ToList();
                var sp = new SolutionProperties
                {
                    Projects = projects
                };
                SolutionDataCache.Instance.AddOrUpdate(sln.FileName, sp, (k, v) =>
                {
                    v = sp;
                    return v;
                });
            };

            dte.Events.SolutionEvents.ProjectAdded += p =>
            {
                if (string.IsNullOrWhiteSpace(p.FileName) ||
                    !Common.SupportedProjectTypes.Any(t => p.FileName.EndsWith(t, StringComparison.OrdinalIgnoreCase)))
                     return;
                var sln = Host.Instance.Solution2;
                SolutionDataCache.Instance.TryGetValue(sln.FileName, out var sp);
                sp?.Projects.Add(p);
            };
            dte.Events.SolutionEvents.ProjectRemoved += project =>
            {
                var sln = Host.Instance.Solution2;
                SolutionDataCache.Instance.TryGetValue(sln.FileName, out var sp);
                sp?.Projects.Remove(project);
            };
            //dte.Events.SolutionEvents.ProjectRenamed += (project, name) =>
            //{
            //    var sln = Host.Instance.Solution2;
            //    SolutionDataCache.Instance.TryGetValue(sln.FileName, out var sp);
            //    if (sp == null) return;
            //    var prj = 
            //}

            SharpPackCommand.Initialize(this);
            AddIgnoreFileCommand.Initialize(this);
        }

        #endregion
    }
}