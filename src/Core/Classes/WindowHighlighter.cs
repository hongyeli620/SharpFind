/* WindowHighlighter.cs
** This file is part #Find.
** 
** Copyright 2018 by Jad Altahan <xviyy@aol.com>
** Licensed under MIT
** <https://github.com/xv/SharpFind/blob/master/LICENSE>
*/

using System.Drawing;
using System;

namespace SharpFind.Classes
{
    public class WindowHighlighter
    {
        /// <summary>
        /// Highlights the designated window by drawing a rectangle around
        /// it, just like Microsoft Spy++.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// Handle to the object to be highlighted.
        /// </param>
        /// 
        /// <param name="useNativeHighlighter">
        /// If true, the rectangle will be drawn using the native PatBlt
        /// function.
        /// </param>
        public static void Highlight(IntPtr hWnd, bool useNativeHighlighter)
        {
            var rect = new Win32.RECT();
            var hDC = Win32.GetWindowDC(hWnd);

            if (hWnd == IntPtr.Zero || !Win32.IsWindow(hWnd))
                return;

            Win32.GetWindowRect(hWnd, out rect);
            Win32.OffsetRect(ref rect, -rect.left, -rect.top);

            // The thickness of the frame
            const int width = 3;

            if (hDC == IntPtr.Zero)
                return;

            if (!Win32.IsRectEmpty(ref rect))
            {
                if (useNativeHighlighter)
                {
                    // Top side
                    Win32.PatBlt(hDC,
                                 rect.left,
                                 rect.top,
                                 rect.right - rect.left,
                                 width,
                                 Win32.RasterOperations.PATINVERT);

                    // Left side
                    Win32.PatBlt(hDC,
                                 rect.left,
                                 rect.bottom - width,
                                 width,
                                 -(rect.bottom - rect.top - 2 * width),
                                 Win32.RasterOperations.PATINVERT);

                    // Right side
                    Win32.PatBlt(hDC,
                                 rect.right - width,
                                 rect.top + width,
                                 width,
                                 rect.bottom - rect.top - 2 * width,
                                 Win32.RasterOperations.PATINVERT);

                    // Bottom side
                    Win32.PatBlt(hDC,
                                 rect.right,
                                 rect.bottom - width,
                                 -(rect.right - rect.left),
                                 width,
                                 Win32.RasterOperations.PATINVERT);
                }
                else
                {
                    // Simple GDI+ rectangle drawing
                    using (var pen = new Pen(ColorTranslator.FromHtml("#FF0000"), 4F))
                    {
                        using (var g = Graphics.FromHdc(hDC))
                            g.DrawRectangle(pen, 0, 0, rect.right - rect.left,
                                                       rect.bottom - rect.top);
                    }
                }
            }

            Win32.ReleaseDC(hWnd, hDC);
        }

        /// <summary>
        /// Refreshes the window to get rid of the previously drawn rectangle.
        /// </summary>
        public static void Refresh(IntPtr hWnd)
        {
            Win32.InvalidateRect(hWnd, IntPtr.Zero, true);
            Win32.UpdateWindow(hWnd);

            const Win32.RedrawWindowFlags flags = Win32.RedrawWindowFlags.RDW_FRAME      | 
                                                  Win32.RedrawWindowFlags.RDW_INVALIDATE | 
                                                  Win32.RedrawWindowFlags.RDW_UPDATENOW  |
                                                  Win32.RedrawWindowFlags.RDW_ALLCHILDREN;

            Win32.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, flags);
        }
    }
}