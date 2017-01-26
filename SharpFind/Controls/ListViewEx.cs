/* ListViewEx.cs
** This file is part #Find.
** 
** Copyright 2017 by Jad Altahan <hello@exr.be>
** Licensed under MIT
** <https://github.com/ei/SharpFind/blob/master/LICENSE>
*/

using System.Windows.Forms;
using System;
using SharpFind.Classes;

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
            NativeMethods.SetWindowTheme(Handle, "explorer", null);
            base.OnHandleCreated(e);
        }
    }
}