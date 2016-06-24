﻿//------------------------------------------------------------------------------
// <copyright file="AddIgnoreFileCommand.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using CnSharp.VisualStudio.Extensions;
using Microsoft.VisualStudio.Shell;

namespace CnSharp.VisualStudio.SharpDeploy.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AddIgnoreFileCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4129;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("307E403C-3431-45B9-86D6-3DFF0810D838");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddIgnoreFileCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private AddIgnoreFileCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AddIgnoreFileCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new AddIgnoreFileCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = Host.Instance.Dte2;
            var project = dte.GetActiveProejct();
            Common.CheckTfs(project);
            var file = Path.Combine(project.GetDirectory(), Common.IgnoreFileName);
            if (File.Exists(file))
                return;

            using (var sw = new StreamWriter(file, false, Encoding.UTF8))
            {
                var temp = Resource.IgnoreFile;
                sw.Write(temp);
                sw.Flush();
                sw.Close();
            }

            project.ProjectItems.AddFromFile(file);
            dte.ItemOperations.OpenFile(file);
        }
    }
}
