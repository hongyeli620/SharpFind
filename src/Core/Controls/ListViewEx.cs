/* ListViewEx.cs
** This file is part #Find.
** 
** Copyright 2018 by Jad Altahan <xviyy@aol.com>
** Licensed under MIT
** <https://github.com/xv/SharpFind/blob/master/LICENSE>
*/

using System.Windows.Forms;
using System;

namespace SharpFind.Controls
{
    public class ListViewEx : ListView
    {
        public ListViewEx()
        {
            // Eliminate the annoying flicker
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                Win32.SetWindowTheme(Handle, "explorer", null);

            base.OnHandleCreated(e);
        }
    }
}