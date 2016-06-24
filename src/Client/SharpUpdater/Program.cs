using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CnSharp.Windows.Updater
{
    internal static class Program
    {
        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// 0 temp .sp file downloaded / package feed url
        /// 1 process name
        /// 2 package feed file downloaded (optional)
        /// </param>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Console.SetOut(new StreamWriter(Path.Combine(Application.StartupPath, "update.log")));

            if (args != null && args.Length > 0)
            {
                string packagePath = args[0];
                string manifestPath = "";
            
                if (args.Length > 1)
                {
                    var processName = args[1];
                    Process.GetProcessesByName(processName).ToList().ForEach(p => p.Kill());
                }

                string feedFile = "";
                if (args.Length > 2)
                {
                    feedFile = args[2];
                }
                var installForm = new InstallForm(packagePath);
                Application.Run(installForm);
                return;
            }
#if !DEBUG
            //Application.ThreadException += Application_ThreadException;
#endif
            var connForm = new ConnectionForm();
            Application.Run(connForm);

            
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }

}