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