using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using CnSharp.Updater;

namespace CnSharp.Windows.Updater
{
    public class Common
    {
        public const string AppName = "SharpUpdater";


        private static  string[] _ignoreFiles;

        public static string[] IgnoreFiles = 
        {
             "Updater.exe",
             "[Content_Types].xml"
        };

        public static string[] IgnoreFolders =  {"_rels"};

        #region Public Methods

        public static string GetLocalText(string key)
        {
            string resourceNameSpace = typeof(Common).Namespace+".Language";
            CultureInfo ci = CultureInfo.CurrentCulture;
            string cultureName = ci.Name;
            if (cultureName.Contains("zh-CN"))
            {
                cultureName = "zh-Hans";
            }
            var rm = new ResourceManager($"{resourceNameSpace}.{cultureName}", Assembly.GetExecutingAssembly());
            //if (rm == null)
            //     rm = new ResourceManager(string.Format("{0}.en-us", resourceNameSpace), assembly);
            return rm.GetString(key, ci);
        }

        public static void Start(Manifest manifest)
        {
            Start(manifest, false);
        }
         
        public static void Start(Manifest manifest,bool exception)
        {
            Process.Start(manifest.EntryPoint, (exception ? "exception" : "ok"));
        }


        #endregion
    }
}