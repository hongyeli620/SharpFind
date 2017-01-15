using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace SharpFind.Classes
{
    internal static class NativeMethods
    {
        #region Constants

        // RedrawWindow flags
        /// <summary>
        /// Invalidate lprcUpdate or hrgnUpdate (only one may be not NULL). If
        /// both are NULL, the entire window is invalidated.
        /// </summary>
        public const int RDW_INVALIDATE = 0x1;
        /// <summary>
        ///  Includes child windows, if any, in the repainting operation.
        /// </summary>
        public const int RDW_ALLCHILDREN = 0x80;
        /// <summary>
        ///  Causes the affected windows (as specified by the RDW_ALLCHILDREN flag)
        ///  to receive WM_NCPAINT, WM_ERASEBKGND, and WM_PAINT messages, if
        ///  necessary, before the function returns.
        /// </summary>
        public const int RDW_UPDATENOW = 0x100;
        /// <summary>
        /// Causes any part of the nonclient area of the window that intersects the
        /// update region to receive a WM_NCPAINT message.
        /// </summary>
        public const int RDW_FRAME = 0x400;


        // SetWindowPos flags
        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function.
        /// </summary>
        public const int SWP_FRAMECHANGED = 0x20;
        /// <summary>
        /// Does not change the owner window's position in the Z order.
        /// </summary>
        public const int SWP_NOOWNERZORDER = 0x200;


        // GetWindowLong flags
        /// <summary>
        /// Retrieves a handle to the application instance.
        /// </summary>
        public const int GWL_HINSTANCE = -6;
        /// <summary>
        /// Retrieves the identifier of the window.
        /// </summary>
        public const int GWL_ID = -12;
        /// <summary>
        /// Retrieves the window styles.
        /// </summary>
        public const int GWL_STYLE = -16;
        /// <summary>
        /// Retrieves the extended window styles.
        /// </summary>
        public const int GWL_EXSTYLE = -20;
        /// <summary>
        /// Retrieves the user data associated with the window.
        /// </summary>
        public const int GWL_USERDATA = -21;


        // GetClassLong flags
        /// <summary>
        /// Retrieves a handle to the background brush associated with the class.
        /// </summary>
        public const int GCL_HBRBACKGROUND = -10;
        /// <summary>
        /// Retrieves a handle to the cursor associated with the class.
        /// </summary>
        public const int GCL_HCURSOR = -12;
        /// <summary>
        /// Retrieves a handle to the icon associated with the class.
        /// </summary>
        public const int GCL_HICON = -14;
        /// <summary>
        /// Retrieves the size, in bytes, of the extra window memory associated with
        /// each window in the class.
        /// </summary>
        public const int GCL_CBWNDEXTRA = (-18);
        /// <summary>
        /// Retrieves the size, in bytes, of the extra memory associated with the class.
        /// </summary>
        public const int GCL_CBCLSEXTRA = -20;
        /// <summary>
        /// Retrieves the address of the window procedure associated with the class.
        /// </summary>
        public const int GCL_WNDPROC = -24;
        /// <summary>
        /// Retrieves the window-class style bits.
        /// </summary>
        public const int GCL_STYLE = -26;
        /// <summary>
        /// Retrieves an ATOM value that uniquely identifies the window class.
        /// </summary>
        public const int GCW_ATOM = -32;
        /// <summary>
        /// Retrieves a handle to the small icon associated with the class.
        /// </summary>
        public const int GCL_HICONSM = -34;


        // Menu flags
        /// <summary>
        /// A window receives this message when the user chooses a command from
        /// the Window menu or when the user chooses the maximize button, minimize
        /// button, restore button, or close button.
        /// </summary>
        public const int WM_SYSCOMMAND = 0x112;
        /// <summary>
        /// Specifies that the menu item opens a drop-down menu or submenu.
        /// </summary>
        public const int MF_POPUP = 0x010;
        /// <summary>
        /// Indicates that uPosition gives the zero-based relative position of the
        /// menu item.
        /// </summary>
        public const int MF_BYPOSITION = 0x400;
        /// <summary>
        /// Draws a horizontal dividing line.
        /// </summary>
        public const int MF_SEPARATOR = 0x800;

        #endregion
        #region Enumerations

        // The list is too big. I didn't want to waste bytes. I only added the ones
        // used by the program
        internal enum WindowsMessages : uint
        {
            NULL      = 0x00,
            MOUSEMOVE = 0x200,
            LBUTTONUP = 0x202
        }

        internal enum RasterOperations : uint
        {
            BLACKNESS   = 0x00000042,
            CAPTUREBLT  = 0x40000000,
            DSTINVERT   = 0x00550009,
            MERGECOPY   = 0x00C000CA,
            MERGEPAINT  = 0x00BB0226,
            NOTSRCCOPY  = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            PATCOPY     = 0x00F00021,
            PATINVERT   = 0x005A0049,
            PATPAINT    = 0x00FB0A09,
            SRCAND      = 0x008800C6,
            SRCCOPY     = 0x00CC0020,
            SRCERASE    = 0x00440328,
            SRCINVERT   = 0x00660046,
            SRCPAINT    = 0x00EE0086,
            WHITENESS   = 0x00FF0062
        }

        #endregion
        #region RECT Structure

        /// <summary>
        /// The RECT structure defines the coordinates of the upper-left and
        /// lower-right corners of a rectangle.
        /// </summary>
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #endregion
        #region user32.dll

        /// <summary>
        /// Retrieves the specified value from the WNDCLASSEX structure associated
        /// with the specified window.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window and, indirectly, the class to which the window
        /// belongs.
        /// </param>
        /// 
        /// <param name="nIndex">
        /// The value to be retrieved.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is the requested value.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        internal static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Same as GetClassLongPtr32, but for x64 systems.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        internal static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Retrieves the name of the class to which the specified window belongs.
        /// </summary>
        ///
        /// <param name="hWnd">
        /// A handle to the window and, indirectly, the class to which the window
        /// belongs.
        /// </param>
        ///
        /// <param name="lpClassName">
        /// The class name string
        /// </param>
        ///
        /// <param name="nMaxCount">
        /// The length of the lpClassName buffer, in characters
        /// </param>
        ///
        /// <returns>
        /// If the function succeeds, the return value is the number of characters
        /// copied to the buffer.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// Retrieves the coordinates of a window's client area.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window whose client coordinates are to be retrieved.
        /// </param>
        /// 
        /// <param name="lpRect">
        /// A pointer to a RECT structure that receives the client coordinates.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Enables the application to access the window menu for copying and
        /// modifying.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window that will own a copy of the window menu.
        /// </param>
        /// 
        /// <param name="bRevert">
        /// The action to be taken. If this parameter is FALSE, GetSystemMenu
        /// returns a handle to the copy of the window menu currently in use.
        /// The copy is initially identical to the window menu, but it can be
        /// modified. If this parameter is TRUE, GetSystemMenu resets the window
        /// menu back to the default state. The previous window menu, if any, is
        /// destroyed.
        /// </param>
        /// 
        /// <returns>
        /// If the bRevert parameter is FALSE, the return value is a handle to a
        /// copy of the window menu. If the bRevert parameter is TRUE, the return
        /// value is NULL.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        /// <summary>
        /// Retrieves the device context (DC) for the entire window, including
        /// title bar, menus, and scroll bars. A window device context permits
        /// painting anywhere in a window, because the origin of the device
        /// context is the upper-left corner of the window instead of the client
        /// area.
        /// 
        /// GetWindowDC assigns default attributes to the window device context
        /// each time it retrieves the device context. Previous attributes are lost.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window with a device context that is to be retrieved.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is a handle to a device
        /// context for the specified window.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(IntPtr hWnd);

        /// <summary>
        /// Retrieves information about the specified window. The function also
        /// retrieves the 32-bit (DWORD) value at the specified offset into the
        /// extra window memory.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window and, indirectly, the class to which the window
        /// belongs.
        /// </param>
        /// 
        /// <param name="nIndex">
        /// The zero-based offset to the value to be retrieved.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is the requested value.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Same as GetWindowLong, but for compatibility with both 32-bit and
        /// 64-bit versions of Windows
        /// </summary>
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongA")]
        internal static extern int GetWindowLongPtr(IntPtr hwnd, int nIndex);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified
        /// window. The dimensions are given in screen coordinates that are
        /// relative to the upper-left corner of the screen.
        /// </summary>
        /// 
        /// <param name="hwnd">
        /// A handle to the window.
        /// </param>
        /// 
        /// <param name="lpRect">
        /// A pointer to a RECT structure that receives the screen coordinates of
        /// the upper-left and lower-right corners of the window.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one)
        /// into a buffer.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window or control containing the text.
        /// </param>
        /// 
        /// <param name="lpString">
        /// The buffer that will receive the text.
        /// </param>
        /// 
        /// <param name="nMaxCount">
        /// The maximum number of characters to copy to the buffer, including the
        /// null character.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is the length, in
        /// characters, of the copied string.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified
        /// window.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// 
        /// <param name="lpdwProcessId">
        /// A pointer to a variable that receives the process identifier.
        /// </param>
        /// 
        /// <returns>
        /// The return value is the identifier of the thread that created the window.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

        /// <summary>
        /// Inserts a new menu item into a menu, moving other items down the menu.
        /// </summary>
        /// 
        /// <param name="hMenu">
        /// A handle to the menu to be changed.
        /// </param>
        /// 
        /// <param name="uPosition">
        /// The menu item before which the new menu item is to be inserted, as
        /// determined by the uFlags parameter.
        /// </param>
        /// 
        /// <param name="uFlags">
        /// Controls the interpretation of the uPosition parameter and the
        /// content, appearance, and behavior of the new menu item. This parameter
        /// must include one of the following required values.
        /// </param>
        /// 
        /// <param name="uIDNewItem">
        /// The identifier of the new menu item or, if the uFlags parameter has 
        /// the MF_POPUP flag set, a handle to the drop-down menu or submenu.
        /// </param>
        /// 
        /// <param name="lpNewItem">
        /// The content of the new menu item.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, uint uIDNewItem, [MarshalAs(UnmanagedType.LPTStr)]string lpNewItem);

        /// <summary>
        /// The InvalidateRect function adds a rectangle to the specified window's
        /// update region.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window whose update region has changed.
        /// </param>
        /// 
        /// <param name="lpRect">
        /// A pointer to a RECT structure that contains the client coordinates of
        /// the rectangle to be added to the update region.
        /// </param>
        /// 
        /// <param name="bErase">
        /// Specifies whether the background within the update region is to be
        /// erased when the update region is processed.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern int InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        /// <summary>
        /// Determines whether the specified rectangle is empty.
        /// </summary>
        /// 
        /// <param name="lprc">
        /// Pointer to a RECT structure that contains the logical coordinates of
        /// the rectangle.
        /// </param>
        /// 
        /// <returns>
        /// If the rectangle is empty, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool IsRectEmpty(ref RECT lprc);

        /// <summary>
        /// Determines whether the specified window handle identifies an existing
        /// window.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window to be tested.
        /// </param>
        /// 
        /// <returns>
        /// If the window handle identifies an existing window, the return value
        /// is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// Determines whether the specified window is a native Unicode window.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window to be tested.
        /// </param>
        /// If the window is a native Unicode window, the return value is nonzero.
        /// <returns></returns>
        [DllImport("user32.dll")]
        internal static extern bool IsWindowUnicode(IntPtr hWnd);

        /// <summary>
        /// The OffsetRect function moves the specified rectangle by the specified
        /// offsets.
        /// </summary>
        /// 
        /// <param name="lprc">
        /// Pointer to a RECT structure that contains the logical coordinates of
        /// the rectangle to be moved.
        /// </param>
        /// 
        /// <param name="dx">
        /// Specifies the amount to move the rectangle left or right. This parameter
        /// must be a negative value to move the rectangle to the left.
        /// </param>
        /// 
        /// <param name="dy">
        /// Specifies the amount to move the rectangle up or down. This parameter
        /// must be a negative value to move the rectangle up.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool OffsetRect(ref RECT lprc, int dx, int dy);

        /// <summary>
        /// The RedrawWindow function updates the specified rectangle or region
        /// in a window's client area.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window to be redrawn. If this parameter is NULL, the
        /// desktop window is updated.
        /// </param>
        /// 
        /// <param name="lprcUpdate">
        /// A pointer to a RECT structure containing the coordinates, in device
        /// units, of the update rectangle.
        /// </param>
        /// 
        /// <param name="hrgnUpdate">
        /// A handle to the update region.
        /// </param>
        /// 
        /// <param name="flags">
        /// One or more redraw flags.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        /// <summary>
        /// Releases the mouse capture from a window in the current thread and
        /// restores normal mouse input processing.
        /// </summary>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();

        /// <summary>
        /// The ReleaseDC function releases a device context (DC), freeing it
        /// for use by other applications. The effect of the ReleaseDC function
        /// depends on the type of DC. It frees only common and window DCs. It has
        /// no effect on class or private DCs.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window whose DC is to be released.
        /// </param>
        /// 
        /// <param name="hDC">
        /// A handle to the DC to be released.
        /// </param>
        /// 
        /// <returns>
        /// The return value indicates whether the DC was released. If the DC was
        /// released, the return value is 1
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// Sets the mouse capture to the specified window belonging to the
        /// current thread.SetCapture captures mouse input either when the mouse
        /// is over the capturing window, or when the mouse button was pressed
        /// while the mouse was over the capturing window and the button is still
        /// down. Only one window at a time can capture the mouse.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window in the current thread that is to capture the
        /// mouse.
        /// </param>
        /// 
        /// <returns>
        /// The return value is a handle to the window that had previously
        /// captured the mouse.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr SetCapture(IntPtr hWnd);

        /// <summary>
        /// Updates the client area of the specified window by sending a WM_PAINT
        /// message to the window if the window's update region is not empty. 
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// Handle to the window to be updated.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool UpdateWindow(IntPtr hWnd);

        /// <summary>
        /// Retrieves a handle to the window that contains the specified point.
        /// </summary>
        /// 
        /// <param name="point">
        /// The point to be checked.
        /// </param>
        /// 
        /// <returns>
        /// The return value is a handle to the window that contains the point.
        /// If no window exists at the given point, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(Point point);

        #endregion
        #region gdi32.dll

        /// <summary>
        /// Paints the specified rectangle using the brush that is currently
        /// selected into the specified device context. The brush color and the
        /// surface color or colors are combined by using the specified raster
        /// operation.
        /// </summary>
        /// 
        /// <param name="hdc">
        /// A handle to the device context.
        /// </param>
        /// 
        /// <param name="nXLeft">
        /// The x-coordinate, in logical units, of the upper-left corner of the
        /// rectangle to be filled.
        /// </param>
        /// 
        /// <param name="nYLeft">
        /// The y-coordinate, in logical units, of the upper-left corner of the
        /// rectangle to be filled.
        /// </param>
        /// 
        /// <param name="nWidth">
        /// The width, in logical units, of the rectangle.
        /// </param>
        /// 
        /// <param name="nHeight">
        /// The height, in logical units, of the rectangle.
        /// </param>
        /// 
        /// <param name="dwRop">
        /// The raster operation code. This code can be one of the following values.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("gdi32.dll")]
        internal static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft, int nWidth, int nHeight, RasterOperations dwRop);

        #endregion
        #region kernel32.dll

        /// <summary>
        /// Retrieves the process identifier of the calling process.
        /// </summary>
        /// 
        /// <returns>
        /// The return value is the process identifier of the calling process.
        /// </returns>
        [DllImport("kernel32.dll")]
        internal static extern uint GetCurrentProcessId();

        /// <summary>
        /// Retrieves the priority class for the specified process. This value,
        /// together with the priority value of each thread of the process,
        /// determines each thread's base priority level.
        /// </summary>
        /// 
        /// <param name="hProcess">
        /// A handle to the process.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is the priority class of
        /// the specified process.
        /// </returns>
        [DllImport("kernel32.dll")]
        internal static extern uint GetPriorityClass(IntPtr hProcess);

        #endregion
        #region uxtheme.dll

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
        internal static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        #endregion
        #region WinUser.h & CommCtrl.h Definitions
        
        /// <summary>
        /// Based on Windows Kits\8.1\Include\um\WinUser.h
        /// </summary>
        internal static class ClassStyles
        {
            internal static readonly int
            CS_VREDRAW         = 0x0001,
            CS_HREDRAW         = 0x0002,
            CS_DBLCLKS         = 0x0008,
            CS_OWNDC           = 0x0020,
            CS_CLASSDC         = 0x0040,
            CS_PARENTDC        = 0x0080,
            CS_NOCLOSE         = 0x0200,
            CS_SAVEBITS        = 0x0800,
            CS_BYTEALIGNCLIENT = 0x1000,
            CS_BYTEALIGNWINDOW = 0x2000,
            CS_GLOBALCLASS     = 0x4000,
            CS_IME             = 0x00010000,
            CS_DROPSHADOW      = 0x00020000;
        }

        /// <summary>
        /// Based on Windows Kits\8.1\Include\um\WinUser.h
        /// </summary>
        internal static class WindowStyles
        {
            internal static readonly long
            WS_OVERLAPPED       = 0x00000000L,
            WS_POPUP            = 0x80000000L,
            WS_CHILD            = 0x40000000L,
            WS_MINIMIZE         = 0x20000000L,
            WS_VISIBLE          = 0x10000000L,
            WS_DISABLED         = 0x08000000L,
            WS_CLIPSIBLINGS     = 0x04000000L,
            WS_CLIPCHILDREN     = 0x02000000L,
            WS_MAXIMIZE         = 0x01000000L,
            WS_CAPTION          = 0x00C00000L,
            WS_BORDER           = 0x00800000L,
            WS_DLGFRAME         = 0x00400000L,
            WS_VSCROLL          = 0x00200000L,
            WS_HSCROLL          = 0x00100000L,
            WS_SYSMENU          = 0x00080000L,
            WS_THICKFRAME       = 0x00040000L,
//          WS_GROUP            = 0x00020000L,
//          WS_TABSTOP          = 0x00010000L,
            WS_MINIMIZEBOX      = 0x00020000L,
            WS_MAXIMIZEBOX      = 0x00010000L,
//          WS_TILED            = WS_OVERLAPPED,
//          WS_ICONIC           = WS_MINIMIZE,
//          WS_SIZEBOX          = WS_THICKFRAME,
//          WS_TILEDWINDOW      = WS_OVERLAPPEDWINDOW,
            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW      = WS_POPUP | WS_BORDER | WS_SYSMENU;
//          WS_CHILDWINDOW      = WS_CHILD;
        }

        /// <summary>
        /// Based on Windows Kits\8.1\Include\um\WinUser.h
        /// </summary>
        internal static class ExtendedWindowStyles
        {
            internal static readonly long
            WS_EX_DLGMODALFRAME       = 0x00000001L,
            WS_EX_NOPARENTNOTIFY      = 0x00000004L,
            WS_EX_TOPMOST             = 0x00000008L,
            WS_EX_ACCEPTFILES         = 0x00000010L,
            WS_EX_TRANSPARENT         = 0x00000020L,
            WS_EX_MDICHILD            = 0x00000040L,
            WS_EX_TOOLWINDOW          = 0x00000080L,
            WS_EX_WINDOWEDGE          = 0x00000100L,
            WS_EX_CLIENTEDGE          = 0x00000200L,
            WS_EX_CONTEXTHELP         = 0x00000400L,
            WS_EX_RIGHT               = 0x00001000L,
            WS_EX_LEFT                = 0x00000000L,
            WS_EX_RTLREADING          = 0x00002000L,
            WS_EX_LTRREADING          = 0x00000000L,
            WS_EX_LEFTSCROLLBAR       = 0x00004000L,
            WS_EX_RIGHTSCROLLBAR      = 0x00000000L,
            WS_EX_CONTROLPARENT       = 0x00010000L,
            WS_EX_STATICEDGE          = 0x00020000L,
            WS_EX_APPWINDOW           = 0x00040000L,
//          WS_EX_OVERLAPPEDWINDOW    = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
//          WS_EX_PALETTEWINDOW       = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_LAYERED             = 0x00080000,
            WS_EX_NOINHERITLAYOUT     = 0x00100000L,
            WS_EX_NOREDIRECTIONBITMAP = 0x00200000L,
            WS_EX_LAYOUTRTL           = 0x00400000L,
            WS_EX_COMPOSITED          = 0x02000000L,
            WS_EX_NOACTIVATE          = 0x08000000L;
        }

        /// <summary>
        /// Based on Windows Kits\8.1\WinUser.h
        /// </summary>
        internal static class ButtonControlStyles
        {
            internal static readonly long
            BS_PUSHBUTTON      = 0x00000000L,
            BS_DEFPUSHBUTTON   = 0x00000001L,
            BS_CHECKBOX        = 0x00000002L,
            BS_AUTOCHECKBOX    = 0x00000003L,
            BS_RADIOBUTTON     = 0x00000004L,
            BS_3STATE          = 0x00000005L,
            BS_AUTO3STATE      = 0x00000006L,
            BS_GROUPBOX        = 0x00000007L,
            BS_USERBUTTON      = 0x00000008L,
            BS_AUTORADIOBUTTON = 0x00000009L,
            BS_PUSHBOX         = 0x0000000AL,
            BS_OWNERDRAW       = 0x0000000BL,
            BS_TYPEMASK        = 0x0000000FL,
            BS_LEFTTEXT        = 0x00000020L,
            BS_TEXT            = 0x00000000L,
            BS_ICON            = 0x00000040L,
            BS_BITMAP          = 0x00000080L,
            BS_LEFT            = 0x00000100L,
            BS_RIGHT           = 0x00000200L,
            BS_CENTER          = 0x00000300L,
            BS_TOP             = 0x00000400L,
            BS_BOTTOM          = 0x00000800L,
            BS_VCENTER         = 0x00000C00L,
            BS_PUSHLIKE        = 0x00001000L,
            BS_MULTILINE       = 0x00002000L,
            BS_NOTIFY          = 0x00004000L,
            BS_FLAT            = 0x00008000L,
            BS_RIGHTBUTTON     = BS_LEFTTEXT;
        }

        /// <summary>
        /// Based on Windows Kits\8.1\CommCtrl.h
        /// </summary>
        internal static class CommonControlStyles
        {
            internal static readonly long
            CCS_TOP           = 0x00000001L,
            CCS_NOMOVEY       = 0x00000002L,
            CCS_BOTTOM        = 0x00000003L,
            CCS_NORESIZE      = 0x00000004L,
            CCS_NOPARENTALIGN = 0x00000008L,
            CCS_ADJUSTABLE    = 0x00000020L,
            CCS_NODIVIDER     = 0x00000040L,
            CCS_VERT          = 0x00000080L,
            CCS_LEFT          = CCS_VERT | CCS_TOP,
            CCS_RIGHT         = CCS_VERT | CCS_BOTTOM,
            CCS_NOMOVEX       = CCS_VERT | CCS_NOMOVEY;
        }

        /// <summary>
        /// Based on Windows Kits\8.1\WinUser.h
        /// </summary>
        {
            internal static readonly long
            DS_ABSALIGN      = 0x01L,
            DS_SYSMODAL      = 0x02L,
            DS_LOCALEDIT     = 0x20L,
            DS_SETFONT       = 0x40L,
            DS_MODALFRAME    = 0x80L,
            DS_NOIDLEMSG     = 0x100L,
            DS_SETFOREGROUND = 0x200L,
            DS_3DLOOK        = 0x0004L,
            DS_FIXEDSYS      = 0x0008L,
            DS_NOFAILCREATE  = 0x0010L,
            DS_CONTROL       = 0x0400L,
            DS_CENTER        = 0x0800L,
            DS_CENTERMOUSE   = 0x1000L,
            DS_CONTEXTHELP   = 0x2000L,
            DS_USEPIXELS     = 0x8000L,
            DS_SHELLFONT     = DS_SETFONT | DS_FIXEDSYS;
        }

        #endregion
        #region Process Priority Definitions

        /// <summary>
        /// As documented on:
        /// https://msdn.microsoft.com/en-us/library/windows/desktop/ms683211(v=vs.85).aspx
        /// </summary>
        internal static class PriorityClass
        {
            internal static readonly uint
            NORMAL_PRIORITY_CLASS         = 0x20,
            IDLE_PRIORITY_CLASS           = 0x40,
            HIGH_PRIORITY_CLASS           = 0x80,
            REALTIME_PRIORITY_CLASS       = 0x100,
            BELOW_NORMAL_PRIORITY_CLASS   = 0x4000,
            ABOVE_NORMAL_PRIORITY_CLASS   = 0x8000,
            PROCESS_MODE_BACKGROUND_BEGIN = 0x100000,
            PROCESS_MODE_BACKGROUND_END   = 0x200000;
        }

        #endregion
    }
}