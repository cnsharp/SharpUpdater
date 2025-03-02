using CnSharp.Updater;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CnSharp.Windows.Updater
{
    public class Bootstrapper
    {
        private static StreamWriter sw;
        public static void Start()
        {
            var logFile = Path.Combine(Application.StartupPath, "_logs",
              $"updater_{DateTime.Today:yyyyMMdd}.log");
            var dir = Path.GetDirectoryName(logFile);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            sw = new StreamWriter(logFile);
            Console.SetOut(sw);
            var manifestFile = Path.Combine(Application.StartupPath, Manifest.ManifestFileName);
            if (!File.Exists(manifestFile))
            {
                Console.WriteLine("manifest not found.");
                return;
            }
            Application.ThreadException += Application_ThreadException;
            Application.ApplicationExit += ApplicationOnApplicationExit;
            var connForm = new ConnectionForm();
            Application.Run(connForm);
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs e)
        {
            sw.Flush();
            sw.Close();
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            Console.WriteLine(e.Exception.StackTrace);
        }

    }
}