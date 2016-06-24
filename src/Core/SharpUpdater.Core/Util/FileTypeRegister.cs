using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace CnSharp.Updater.Util
{
	public class FileTypeRegister
	{
		private const int HWND_BROADCAST = 0xffff;
		private const int INI_INTL = 1;
		private const int WM_SETTINGCHANGE = WM_WININICHANGE;
		private const int WM_WININICHANGE = 0x001a;

		public static void RegisterFileType(string extension, string relationName, string exePath, string icon,
		                                  string description, bool registerAlways)
		{
			if (FileTypeRegistered(extension))
			{
				UnRegistFileType(extension, relationName);
			}


			string fileName = Path.GetFileName(exePath);
			string installPath = Path.GetDirectoryName(exePath);
			if (!extension.StartsWith("."))
				extension = "." + extension;
			RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(extension);
			extKey.SetValue("", relationName);
			extKey.Close();

			RegistryKey relationKey = Registry.ClassesRoot.CreateSubKey(relationName);
			relationKey.SetValue("", description);
			relationKey.CreateSubKey("DefaultIcon").SetValue("", icon); //Í¼±ê
			relationKey.Close();

			Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + extension).SetValue("", relationName);
			RegistryKey fileExtKey = Registry.CurrentUser.CreateSubKey(
				@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extension);
			fileExtKey.SetValue("", relationName);

			RegistryKey openWithKey = fileExtKey.CreateSubKey("OpenWithList");
			openWithKey.SetValue("a", fileName);
			openWithKey.SetValue("MRUList", "a");
			openWithKey.SetValue(relationName, new byte[] {0}, RegistryValueKind.Binary);
			fileExtKey.Close();

			RegistryKey cmdKey = Registry.ClassesRoot.CreateSubKey(relationName + @"\shell\open\command");
			cmdKey.SetValue("", "\"" + exePath + "\" \"%1\"");
			cmdKey.SetValue("(Default)", "\"" + exePath + "\",\"%1\"");
			cmdKey.SetValue("InstallPath", installPath);
			cmdKey.Close();

			cmdKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + relationName + @"\shell\open\command");
			cmdKey.SetValue("", "\"" + exePath + "\" \"%1\"");
			cmdKey.SetValue("(Default)", "\"" + exePath + "\",\"%1\"");
			cmdKey.SetValue("InstallPath", installPath);
			cmdKey.Close();

			string usersPath = "";
			string pathExt;
			string[] arinfo = Registry.Users.GetSubKeyNames(); 
			for (int keyid = 0; keyid < arinfo.Length; keyid++) 
			{
				if (arinfo[keyid].Length > 12)
				{
					pathExt = arinfo[keyid].Substring(arinfo[keyid].Length - 12, 12); 
					if (string.Compare(pathExt, "1000_Classes") == 0) 
					{
						usersPath = arinfo[keyid]; // This subkey is the key we¡¯re looking for
						break; 
					}
				}
			}
			if (usersPath.Length > 0) 
			{
				Registry.Users.CreateSubKey(usersPath + @"\" + extension).SetValue("", relationName);
				RegistryKey cmdKey2 = Registry.Users.CreateSubKey(usersPath + @"\" + extension + @"\shell\open\command");
				cmdKey2.SetValue("", "\"" + exePath + "\" \"%1\"");
				cmdKey2.SetValue("(Default)", "\"" + exePath + "\",\"%1\"");
				cmdKey2.SetValue("InstallPath", installPath);
			}

			usersPath = "";
			arinfo = Registry.Users.GetSubKeyNames(); 
			for (int keyid = 0; keyid < arinfo.Length; keyid++) 
			{
				if (arinfo[keyid].Length > 4) 
				{
					pathExt = arinfo[keyid].Substring(arinfo[keyid].Length - 4, 4); 
					if (string.Compare(pathExt, "1000") == 0) // Does it end in 1000?
					{
						usersPath = arinfo[keyid]; 
						break; 
					}
				}
			}
			if (usersPath.Length > 0) // If we have a valid SubKey name, do the create the following Keys
			{
				RegistryKey classKey = Registry.Users.CreateSubKey(string.Format(@"{0}\Software\Classes\{1}", usersPath, extension));
				classKey.SetValue("", relationName);
				classKey.Close();

				RegistryKey userOpenWithKey = Registry.Users.CreateSubKey(
					string.Format(@"{0}\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{1}\OpenWithList",
					              usersPath, extension));
				userOpenWithKey.SetValue("a", fileName);
				userOpenWithKey.SetValue("MRUList", "a");
				userOpenWithKey.Close();

				RegistryKey userExtKey = Registry.Users.CreateSubKey(
					string.Format(@"{0}\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\{1}\OpenWithProgids",
					              usersPath, extension));
				userExtKey.SetValue(relationName, new byte[] {0}, RegistryValueKind.Binary);
				userExtKey.Close();

				RegistryKey userCmdKey = Registry.Users.CreateSubKey(
					string.Format(@"{0}\Software\Classes\{1}\shell\open\command", usersPath, extension));
				userCmdKey.SetValue("", string.Format("\"{0}\" \"%1\"", exePath));
				userCmdKey.SetValue("(Default)", string.Format("\"{0}\",\"%1\"", exePath));
				userCmdKey.SetValue("InstallPath", installPath);
				userCmdKey.Close();
			}

			// Refresh Registry ,force Windows to Re-Load the Registry
			SendMessage(HWND_BROADCAST, WM_SETTINGCHANGE, 0, INI_INTL); 
		}

		private static bool FileTypeRegistered(string extension)
		{
			RegistryKey sluKey = Registry.ClassesRoot.OpenSubKey(extension);
			if (sluKey != null)
				return true;
			return false;
		}

		private static void UnRegistFileType(string extension, string relationName)
		{
			if (FileTypeRegistered(extension))
			{
				try
				{
					Registry.ClassesRoot.DeleteSubKeyTree(extension);
					Registry.ClassesRoot.DeleteSubKeyTree(relationName);
					Registry.ClassesRoot.Close();
					Registry.CurrentUser.DeleteSubKey(@"Software\Classes\" + relationName);
					Registry.CurrentUser.Close();
				}
				catch
				{
				}
			}
		}

		[DllImport("user32.dll")]
		private static extern int SendMessage(int hWnd, uint wMsg, uint wParam, uint lParam);
	}
}