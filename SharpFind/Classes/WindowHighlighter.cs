/* WindowHighlighter.cs
** This file is part #Find.
** 
** Copyright 2017 by Jad Altahan <hello@exr.be>
** Licensed under MIT
** <https://github.com/ei/SharpFind/blob/master/LICENSE>
*/

using System;
using static SharpFind.Classes.NativeMethods;

namespace SharpFind.Classes
{
    public class WindowHighlighter
    {
        /// <summary>
        /// Highlight the designated window by drawing a colored rectangle around
        /// it, just like Microsoft Spy++.
        /// </summary>
        public static void Highlight(IntPtr hWnd)
        {
            var rect = new RECT();
            var hDC = GetWindowDC(hWnd);

            if (hWnd == IntPtr.Zero || !IsWindow(hWnd))
                return;

            GetWindowRect(hWnd, out rect);
            OffsetRect(ref rect, -rect.left, -rect.top);

            // The thickness of the frame
            const int width = 3;

            if (hDC == IntPtr.Zero)
                return;

            if (!IsRectEmpty(ref rect))
            {
                // An alternative way of highlighting without using the native <PatBlt>
                //
                // const float penWidth = 4F;
                // using (Pen pen = new Pen(ColorTranslator.FromHtml("#FF0000"), penWidth))
                // {
                //     using (Graphics g = Graphics.FromHdc(hDC))
                //     {
                //         g.DrawRectangle(pen, 0, 0, rect.right - rect.left, rect.bottom - rect.top);
                //     }
                // }

                // Top
                PatBlt(hDC, rect.left, rect.top, rect.right - rect.left, width, RasterOperations.PATINVERT);
                // Left
                PatBlt(hDC, rect.left, rect.bottom - width, width, -(rect.bottom - rect.top - 2 * width), RasterOperations.PATINVERT);
                // Right
                PatBlt(hDC, rect.right - width, rect.top + width, width, rect.bottom - rect.top - 2 * width, RasterOperations.PATINVERT);
                // Bottom
                PatBlt(hDC, rect.right, rect.bottom - width, -(rect.right - rect.left), width, RasterOperations.PATINVERT);
            }

            ReleaseDC(hWnd, hDC);
        }

        /// <summary>
        /// Refreshes the window to get rid of the previously drawn rectangles.
        /// </summary>
        public static void Refresh(IntPtr hWnd)
        {
            InvalidateRect(hWnd, IntPtr.Zero, true);
            UpdateWindow(hWnd);
            RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, RDW_FRAME |
                                                         RDW_INVALIDATE |
                                                         RDW_UPDATENOW |
                                                         RDW_ALLCHILDREN);              
        }
    }
}