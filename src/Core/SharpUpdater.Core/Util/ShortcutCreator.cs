using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CnSharp.Updater.Util
{
    public class ShortcutCreator
    {
        private readonly string _appName;
        private readonly string _companyName;
        private readonly string _iconLocation;
        private readonly string _targetLocation;
        private readonly string _workingDirectory;

        public ShortcutCreator(string appName, string targetLocation, string iconLocation, string companyName)
        {
            _appName = appName;
            _targetLocation = targetLocation;
            _workingDirectory = Path.GetDirectoryName(_targetLocation);
            _iconLocation = iconLocation;
            _companyName = companyName;
        }

        private const string DesktopVbs = @"
        set shell = WScript.CreateObject(""WScript.Shell"")
strDesktop = shell.SpecialFolders(""Desktop"")
set shortcut = shell.CreateShortcut(strDesktop & ""\{0}.lnk"")
shortcut.TargetPath = ""{1}""
shortcut.WindowStyle = 1
shortcut.Description = ""{0}""
shortcut.WorkingDirectory = ""{2}""
shortcut.IconLocation = ""{3}""
shortcut.Save
        ";
        public void CreateOnDesktop()
        {
            var path = string.Format("{0}\\{1}.lnk", Environment.GetFolderPath(
                Environment.SpecialFolder.DesktopDirectory), _appName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var vbs = string.Format(DesktopVbs,  _appName, _targetLocation, _workingDirectory,_iconLocation);
            Run(vbs);
        }

        private const string StartMenuVbs = @"
        set shell = WScript.CreateObject(""WScript.Shell"")
Set fso = CreateObject(""Scripting.FileSystemObject"")
strStartMenu = shell.SpecialFolders(""StartMenu"")&""\Programs\{0}\""
If  not fso.FolderExists(strStartMenu)   then
    fso.CreateFolder(strStartMenu)
end if
set shortcut = shell.CreateShortcut(strStartMenu & ""\{1}.lnk"")
shortcut.TargetPath = ""{2}""
shortcut.WindowStyle = 1
shortcut.Description = ""{1}""
shortcut.WorkingDirectory = ""{3}""
shortcut.IconLocation = ""{4}""
shortcut.Save
        ";

        public void CreateOnStartMenu()
        {
            var path = string.Format("{0}\\Programs\\{1}\\{2}.lnk", Environment.GetFolderPath(
                Environment.SpecialFolder.StartMenu), _companyName, _appName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var vbs = string.Format(StartMenuVbs, _companyName, _appName, _targetLocation, _workingDirectory,
                                    _iconLocation);
           Run(vbs);
        }


        /// 写入临时文件
        private void Run(string vbs)
        {
            if (string.IsNullOrEmpty(vbs)) return;
            //临时文件 
            var tempFile = string.Format("{0}\\{1}.vbs",Path.GetTempPath(), Guid.NewGuid());
            //写入文件 
            using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                //这里必须用UnicodeEncoding. 因为用UTF-8或ASCII会造成VBS乱码 
                var uni = new UnicodeEncoding();
                var b = uni.GetBytes(vbs);
                fs.Write(b, 0, b.Length);
                fs.Flush();
                fs.Close();
            }
            Process.Start(tempFile);
        }

    }
}