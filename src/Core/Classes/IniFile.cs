﻿/* IniFile.cs
** This file is part #Find.
** 
** Copyright 2018 by Jad Altahan <xviyy@aol.com>
** Licensed under MIT
** <https://github.com/xv/SharpFind/blob/master/LICENSE>
*/

using System.Windows.Forms;
using System;

namespace SharpFind.Classes
{
    /// <summary>
    /// A class used to read and write application settings via .ini
    /// configuration file.
    /// </summary>
    public class IniFile
    {
        /// <summary>
        /// Path to the settings file.
        /// </summary>
        public static string SettingsPath()
        {
            return Application.StartupPath + "\\settings.ini";
        }

        #region Reading

        /// <summary>
        /// Reads <c>string</c> type keys.
        /// </summary>
        public static string ReadINI(string path, string section, string key)
        {
            return ReadData(path, section, key, string.Empty);
        }

        /// <summary>
        /// Reads <c>integer</c> type keys
        /// </summary>
        public static int ReadINI(string path, string section, string key, int defaultValue)
        {
            return int.Parse(ReadData(path, section, key, string.Empty));
        }

        /// <summary>
        /// Reads <c>boolean</c> type keys.
        /// </summary>
        public static bool ReadINI(string path, string section, string key, bool defaultValue)
        {
            return bool.Parse(ReadData(path, section, key, defaultValue.ToString()));
        }

        public static string ReadData(string path, string section, string key, string defaultValue)
        {
            var sData = new string(' ', 1024);
            var i = Convert.ToInt32(Win32.GetPrivateProfileString(section, key, defaultValue, sData, sData.Length, path));
            var value = i > 0 ? sData.Substring(0, i) : string.Empty;
            return value;
        }

        #endregion
        #region Writing

        /// <summary>
        /// Writes <c>string</c> type keys.
        /// </summary>
        public static void WriteINI(string path, string section, string key, string value)
        {
            Win32.WritePrivateProfileString(section, key, value, path);
        }

        /// <summary>
        /// Writes <c>integer</c> type keys.
        /// </summary>
        public static void WriteINI(string path, string section, string key, int value)
        {
            Win32.WritePrivateProfileString(section, key, value.ToString(), path);
        }

        /// <summary>
        /// Writes <c>boolean</c> type keys.
        /// </summary>
        public static void WriteINI(string path, string section, string key, bool value)
        {
            Win32.WritePrivateProfileString(section, key, value.ToString().ToLower(), path);
        }

        #endregion
    }
}
