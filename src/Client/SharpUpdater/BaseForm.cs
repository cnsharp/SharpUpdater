using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CnSharp.Updater;
using CnSharp.Updater.Util;

namespace CnSharp.Windows.Updater
{
    public class BaseForm : Form
    {
        #region Constants and Fields

        protected bool isEnableCloseButton;

        #endregion

        #region Properties

        protected override CreateParams CreateParams
        {
            get
            {
                if (isEnableCloseButton)
                {
                    CreateParams parameters = base.CreateParams;
                    return parameters;
                }
                else
                {
                    int CS_NOCLOSE = 0x200;
                    CreateParams parameters = base.CreateParams;
                    parameters.ClassStyle |= CS_NOCLOSE;
                    return parameters;
                }
            }
        }

        #endregion

        #region Public Methods

        public DialogResult CheckProcessing(string exeName)
        {
            if (Process.GetProcessesByName(exeName).Length > 0)
            {
                DialogResult rs = MessageBox.Show(
                    string.Format(Common.GetLocalText("CloseRunning"), exeName),
                    Common.GetLocalText("Warning"),
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
                if (rs == DialogResult.Retry)
                {
                    return CheckProcessing(exeName);
                }
                return rs;
            }
            return DialogResult.OK;
        }

        #endregion

        #region Methods

        protected void CreateShortcut(Manifest remoteRelease)
        {
            if (string.IsNullOrEmpty(remoteRelease.ShortcutIcon) || remoteRelease.ShortcutIcon.Trim().Length == 0)
            {
                return;
            }
            string iconLocation = AppDomain.CurrentDomain.BaseDirectory + remoteRelease.ShortcutIcon;
            string appName = remoteRelease.AppName;
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                appName = appName.Replace(invalidChar, '_');
            }
            string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + appName +
                                  ".lnk";
            if (File.Exists(shortcutPath))
            {
                return;
            }

            var exeName = Assembly.GetExecutingAssembly().GetName().Name + ".exe";
            var creator = new ShortcutCreator(
                appName, AppDomain.CurrentDomain.BaseDirectory + exeName, iconLocation,
                remoteRelease.Owner);
            creator.CreateOnDesktop();
            creator.CreateOnStartMenu();
        }

        #endregion

        private void InitializeComponent()
        {
            SingleAssemblyComponentResourceManager resources = new SingleAssemblyComponentResourceManager(typeof(BaseForm));
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "BaseForm";
            this.ResumeLayout(false);

        }
    }
}