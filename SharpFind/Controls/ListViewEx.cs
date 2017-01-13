using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SharpFind.Controls
{
    public class ListViewEx : ListView
    {
        /// <summary>
        /// Causes a window to use a different set of visual style information
        /// than its class normally uses. In this context, it will be used to
        /// apply the native Window ListView theme.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// Handle to the window whose visual style information is to be changed.
        /// </param>
        /// 
        /// <param name="pszSubAppName">
        /// Pointer to a string that contains the application name to use in place
        /// of the calling application's name.
        /// </param>
        /// 
        /// <param name="pszSubIdList">
        /// Pointer to a string that contains a semicolon-separated list of CLSID
        /// names to use in place of the actual list passed by the window's class.
        /// </param>
        /// 
        /// <returns>
        /// If this function succeeds, it returns S_OK. Otherwise, it returns an
        /// HRESULT error code.
        /// </returns>
        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

        public ListViewEx()
        {
            // Eliminate the annoying flicker
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.EnableNotifyMessage, true);
        }
    }
}
