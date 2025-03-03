using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CnSharp.Updater.Util;
using CnSharp.VisualStudio.Extensions;
using EnvDTE;
using Constants = CnSharp.Updater.Constants;

namespace CnSharp.VisualStudio.SharpUpdater
{
    public class Common
    {
        public static readonly string IgnoreFileName = $"{Constants.ProductName}.ignore";
        public static readonly string[] SupportedProjectTypes = { ".csproj", ".vbproj", ".fsproj" };

        public static bool Contains(Enum keys, Enum flag)
        {
            ulong keysVal = Convert.ToUInt64(keys);
            ulong flagVal = Convert.ToUInt64(flag);

            return (keysVal & flagVal) == flagVal;
        }

        public static DialogResult ShowError(string message)
        {
            return MessageBox.Show(message, Constants.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public class ProductInfo
    {
        public string Name { get; set; }

        public string CompanyName { get; set; }

        public string Version { get; set; }
    }

    public class Paths
    {
        public static string AddinRoot =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");

        public static string GetSettingsFileLocation(Project project)
        {
            var dir = Path.GetDirectoryName(Host.Instance.DTE.Solution.FileName);
            dir = Path.Combine(dir, $".{Constants.ProductName}");

            if (!Directory.Exists(dir))
            {
                DirectoryInfo di = Directory.CreateDirectory(dir);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            return Path.Combine(dir, "settings.xml");
        }
    }

    public class NuPackSettings
    {
        public NuPackSettings()
        {
            UnselectedFiles = new List<string>();
            UnselectedFolders = new List<string>();
        }
        public List<string> UnselectedFolders { get; set; }
        public List<string> UnselectedFiles { get; set; }

        public string PackageOutputDirectory { get; set; } = "bin\\{version}\\";
        public bool OpenPackageOutputDirectoryAfterBuild { get; set; }
        public string DeployServer { get; set; }
        public string DeployKey { get; set; }

    }

    class Validation
    {
        public static bool HasValidationErrors(System.Windows.Forms.Control.ControlCollection controls)
        {
            bool hasError = false;

            // Now we need to loop through the controls and deterime if any of them have errors
            foreach (Control control in controls)
            {
                // check the control and see what it returns
                bool validControl = IsValid(control);
                // If it's not valid then set the flag and keep going.  We want to get through all
                // the validators so they will display on the screen if errorProviders were used.
                if (!validControl)
                    hasError = true;

                // If its a container control then it may have children that need to be checked
                if (control.HasChildren)
                {
                    if (HasValidationErrors(control.Controls))
                        hasError = true;
                }
            }
            return hasError;
        }

        // Here, let's determine if the control has a validating method attached to it
        // and if it does, let's execute it and return the result
        private static bool IsValid(object eventSource)
        {
            string name = "EventValidating";

            Type targetType = eventSource.GetType();

            do
            {
                FieldInfo[] fields = targetType.GetFields(
                     BindingFlags.Static |
                     BindingFlags.Instance |
                     BindingFlags.NonPublic);

                foreach (FieldInfo field in fields)
                {
                    if (field.Name == name)
                    {
                        EventHandlerList eventHandlers = ((EventHandlerList)(eventSource.GetType().GetProperty("Events",
                            (BindingFlags.FlattenHierarchy |
                            (BindingFlags.NonPublic | BindingFlags.Instance))).GetValue(eventSource, null)));

                        Delegate d = eventHandlers[field.GetValue(eventSource)];

                        if (d != null)
                        {
                            Delegate[] subscribers = d.GetInvocationList();

                            // ok we found the validation event,  let's get the event method and call it
                            foreach (Delegate d1 in subscribers)
                            {
                                // create the parameters
                                object sender = eventSource;
                                CancelEventArgs eventArgs = new CancelEventArgs();
                                eventArgs.Cancel = false;
                                object[] parameters = new object[2];
                                parameters[0] = sender;
                                parameters[1] = eventArgs;
                                // call the method
                                d1.DynamicInvoke(parameters);
                                // if the validation failed we need to return that failure
                                return !eventArgs.Cancel;
                            }
                        }
                    }
                }

                targetType = targetType.BaseType;

            } while (targetType != null);

            return true;
        }

    }
}