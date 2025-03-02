using System;
using System.Diagnostics;
using CnSharp.Updater;

namespace CnSharp.Windows.Updater
{
    public class Common
    {
        public const string AppName = "SharpUpdater";

        public static string[] IgnoreFiles = 
        {
             "Updater.exe",
             "[Content_Types].xml"
        };

        public static string[] IgnoreFolders =  {"_rels"};

        public static string GetLocalText(string key)
        {
            return Properties.Resources.ResourceManager.GetString(key);
        }


        public static void Start(Manifest manifest,Exception e = null)
        {
            if (e != null)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Process.Start(manifest.EntryPoint, e != null ? "exception" : "ok");
        }
    }
}