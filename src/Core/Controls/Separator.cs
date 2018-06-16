/* Separator.cs
** This file is part #Find.
** 
** Copyright 2017 by Jad Altahan <xviyy@aol.com>
** Licensed under MIT
** <https://github.com/xv/SharpFind/blob/master/LICENSE>
*/

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace SharpFind.Controls
{
    public class Separator : Control
    {
        public enum _Orientation { Horizontal, Vertical }
        private _Orientation shape;

        /// <summary>
        /// Depending on the OS version, this property decides whether an "edge"
        /// line should be drawn underneath the separator line.
        /// </summary>
        private bool DrawEdge { get; set; }
        private string color;

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Indicates whether the control should be drawn vertically or horizontally.")]
        public _Orientation Orientation
        {
            get { return shape; }
            set
            {
                shape = value;
                switch (shape)
                {
                    case _Orientation.Horizontal:
                        Width  = Height;
                        Height = 10;
                        break;
                    case _Orientation.Vertical:
                        Height = Width;
                        Width  = 10;
                        break;
                }
                Invalidate();
            }
        }

        public Separator()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            Size = new Size(120, 10);

            SetControlStyle();
        }

        /// <summary>
        /// Sets the control style based on the operating system.
        /// </summary>
        private void SetControlStyle()
        {
//          Console.WriteLine(Environment.OSVersion.Version.Major.ToString() + "." + 
//                            Environment.OSVersion.Version.Minor.ToString());

            switch (Environment.OSVersion.Version.Major)
            {
                case 5:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0: // Windows 2000
                            color = "#808080";
                            DrawEdge = true;
                            break;
                        case 1: // Windows XP
                        case 2: // Windows XP 64-bit & Windows Server 2003
                            color = "#D0D0BF";
                            DrawEdge = false;
                            break;
                    }
                    break;
                case 6:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0: // Windows Vista & Windows Server 2008
                        case 1: // Windows 7 & Windows Server 2008 R2
                            color = "#D5DFE5";
                            DrawEdge = true;
                            break;
                        case 2: // Windows 8 & Windows Server 2012
                        case 3: // Windows 8.1 & Windows Server 2012 R2
                            color = "#DCDCDC";
                            DrawEdge = false;
                            break;
                    }
                    break;
                case 10: // Windows 10 and beyond
                    color = "#DCDCDC";
                    DrawEdge = false;
                    break;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            switch (shape)
            {
                case _Orientation.Horizontal:
                    Height = 10;
                    break;
                case _Orientation.Vertical:
                    Width = 10;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(ColorTranslator.FromHtml(color), 1.0F))
            {
                switch (shape)
                {
                    case _Orientation.Horizontal:
                                      e.Graphics.DrawLine(pen, 0, 5, Width, 5);
                        if (DrawEdge) e.Graphics.DrawLine(Pens.White, 0, 6, Width, 6);
                        break;
                    case _Orientation.Vertical:
                                      e.Graphics.DrawLine(pen, 5, 0, 5, Height);
                        if (DrawEdge) e.Graphics.DrawLine(Pens.White, 6, 0, 6, Height);
                        break;
                }
            }
        }
    }
}