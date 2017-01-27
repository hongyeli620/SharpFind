using System.Windows.Forms;
using System;
using static SharpFind.Classes.NativeMethods;

namespace SharpFind.Classes
{
    public class INIReadWrite
    {
        public static string SettingsPath()
        {
            return Application.StartupPath + "\\Settings.ini";
        }

        public static string INIRead(string path, string section, string key)
        {
            return INIRead(path, section, key, string.Empty);
        }

        public static string INIRead(string path, string section, string key, string defaultValue)
        {
            var returnValue = string.Empty;
            var sData = string.Empty;

            sData = new string(' ', 1024);
            var i = Convert.ToInt32(GetPrivateProfileString(section, key, defaultValue, sData, sData.Length, path));
            returnValue = i > 0 ? sData.Substring(0, i) : string.Empty;

            return returnValue;
        }

        public static void INIWrite(string path, string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, path);
        }
    }
}