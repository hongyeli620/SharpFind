/* LinkLabelEx.cs
** This file is part #Find.
** 
** Copyright 2018 by Jad Altahan <xviyy@aol.com>
** Licensed under MIT
** <https://github.com/xv/SharpFind/blob/master/LICENSE>
*/

using System.Drawing;
using System.Windows.Forms;
using System;

namespace SharpFind.Controls
{
    public class LinkLabelEx : LinkLabel
    {
        #region Fields

        private readonly Color linkColor       = ColorTranslator.FromHtml("#0066CC");
        private readonly Color activeLinkColor = ColorTranslator.FromHtml("#00509F");

        private const int WM_SETCURSOR = 0x0020;

        private const int IDC_HAND = 32649;
        private static readonly Cursor NativeHand = new Cursor(Win32.LoadCursor(IntPtr.Zero, IDC_HAND));

        #endregion

        public LinkLabelEx()
        {
            LinkColor = linkColor;
            ActiveLinkColor = activeLinkColor;
            LinkBehavior = LinkBehavior.HoverUnderline;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (OverrideCursor == Cursors.Hand)
                OverrideCursor = NativeHand;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            OverrideCursor = null;
        }
    }
}