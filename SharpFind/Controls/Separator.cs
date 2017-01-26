/* Separator.cs
** This file is part #Find.
** 
** Copyright 2017 by Jad Altahan <hello@exr.be>
** Licensed under MIT
** <https://github.com/ei/SharpFind/blob/master/LICENSE>
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

        private _Orientation _Shape;
        [Browsable(true)]
        [Description("Indicates whether the control should be drawn vertically or horizontally.")]
        public _Orientation Orientation
        {
            get { return _Shape; }
            set
            {
                _Shape = value;
                if (value == _Orientation.Horizontal)
                {
                    Width = Height;
                    Height = 10;
                }
                else if (value == _Orientation.Vertical)
                {
                    Height = Width;
                    Width = 10;
                }
                Invalidate();
            }
        }

        public Separator()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            Size = new Size(120, 10);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            switch (_Shape)
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
            using (Pen pen = new Pen(ColorTranslator.FromHtml("#B4B4B4"), 1.0F))
            {
                switch (_Shape)
                {
                    case _Orientation.Horizontal:
                        e.Graphics.DrawLine(pen, 0, 5, Width, 5);
                        e.Graphics.DrawLine(Pens.White, 0, 6, Width, 6);
                        break;
                    case _Orientation.Vertical:
                        e.Graphics.DrawLine(pen, 5, 0, 5, Height);
                        e.Graphics.DrawLine(Pens.White, 6, 0, 6, Height);
                        break;
                }
            }
        }
    }
}