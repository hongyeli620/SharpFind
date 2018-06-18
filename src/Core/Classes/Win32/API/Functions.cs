using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace SharpFind
{
    public partial class Win32
    {
        #region gdi32.dll

        /// <summary>
        /// The GetDeviceCaps function retrieves device-specific information for
        /// the specified device.
        /// </summary>
        /// 
        /// <param name="hdc">
        /// A handle to the DC.
        /// </param>
        /// 
        /// <param name="nIndex">
        /// The item to be returned. This parameter can be one of the following
        /// values.
        /// </param>
        /// 
        /// <returns>
        /// The return value specifies the value of the desired item.
        /// When nIndex is BITSPIXEL and the device has 15bpp or 16bpp, the
        /// return value is 16.
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

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
        public static extern bool PatBlt(IntPtr hdc, int nXLeft, int nYLeft,
                                                     int nWidth, int nHeight,
                                                     RasterOperations dwRop);

        #endregion
        #region kernel32.dll

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// 
        /// <param name="hObject">
        /// A valid handle to an open object.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Takes a snapshot of the specified processes, as well as the heaps,
        /// modules, and threads used by these processes.
        /// </summary>
        /// 
        /// <param name="dwFlags">
        /// The portions of the system to be included in the snapshot.
        /// The parameters are defined in <c>SnapshotFlags</c>.
        /// </param>
        /// 
        /// <param name="th32ProcessID">
        /// The process identifier of the process to be included in the snapshot.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, it returns an open handle to the specified
        /// snapshot.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, int th32ProcessID);

        /// <summary>
        /// Retrieves the process identifier of the calling process.
        /// </summary>
        /// 
        /// <returns>
        /// The return value is the process identifier of the calling process.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentProcessId();

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
        public static extern uint GetPriorityClass(IntPtr hProcess);

        /// <summary>
        /// Retrieves a string from the specified section in an initialization file.
        /// </summary>
        /// 
        /// <param name="lpAppName">
        /// The name of the section containing the key name. If this parameter is
        /// NULL, the GetPrivateProfileString function copies all section names
        /// in the file to the supplied buffer.
        /// </param>
        /// 
        /// <param name="lpKeyName">
        /// The name of the key whose associated string is to be retrieved. If this
        /// parameter is NULL, all key names in the section specified by the lpAppName
        /// parameter are copied to the buffer specified by the lpReturnedString
        /// parameter.
        /// </param>
        /// 
        /// <param name="lpDefault">
        /// A default string. If the lpKeyName key cannot be found in the
        /// initialization file, GetPrivateProfileString copies the default string to
        /// the lpReturnedString buffer. If this parameter is NULL, the default is an
        /// empty string, "".
        /// </param>
        /// 
        /// <param name="lpReturnedString">
        /// A pointer to the buffer that receives the retrieved string.
        /// </param>
        /// 
        /// <param name="nSize">
        /// The size of the buffer pointed to by the lpReturnedString parameter, in
        /// characters.
        /// </param>
        /// 
        /// <param name="lpFileName">
        /// The name of the initialization file.
        /// </param>
        /// 
        /// <returns>
        /// The return value is the number of characters copied to the buffer, not
        /// including the terminating null character.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint GetPrivateProfileString(string lpAppName, 
                                                          string lpKeyName, 
                                                          string lpDefault,
                                                          string lpReturnedString, 
                                                          int nSize,
                                                          string lpFileName);

        /// <summary>
        /// Retrieves information about the first module associated with a
        /// process.
        /// </summary>
        /// 
        /// <param name="hSnapshot">
        /// A handle to the snapshot returned from a previous call to the
        /// <c>CreateToolhelp32Snapshot</c> function.
        /// </param>
        /// 
        /// <param name="lpme">
        /// A pointer to a <c>MODULEENTRY32</c> structure.
        /// </param>
        /// 
        /// <returns>
        /// Returns TRUE if the first entry of the module list has been copied
        /// to the buffer or FALSE otherwise.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        /// <summary>
        /// Retrieves information about the next module associated with a
        /// process.
        /// </summary>
        /// 
        /// <param name="hSnapshot">
        /// A handle to the snapshot returned from a previous call to the
        /// <c>CreateToolhelp32Snapshot</c> function.
        /// </param>
        /// 
        /// <param name="lpme">
        /// A pointer to a <c>MODULEENTRY32</c> structure.
        /// </param>
        /// 
        /// <returns>
        /// Returns TRUE if the first entry of the module list has been copied
        /// to the buffer or FALSE otherwise.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        /// <summary>
        /// Opens an existing thread object.
        /// </summary>
        /// 
        /// <param name="dwDesiredAccess">
        /// The access to the thread object. This access right is checked
        /// against the security descriptor for the thread. This parameter can
        /// be one or more of the thread access rights.
        /// </param>
        /// 
        /// <param name="bInheritHandle">
        /// If this value is TRUE, processes created by this process will
        /// inherit the handle. Otherwise, the processes do not inherit this
        /// handle.
        /// </param>
        /// 
        /// <param name="dwThreadId">
        /// The identifier of the thread to be opened.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is an open handle to the
        /// specified thread.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        /// <summary>
        /// Retrieves information about the first thread of any process
        /// encountered in a system snapshot.
        /// </summary>
        /// 
        /// <param name="hSnapshot">
        /// A handle to the snapshot returned from a previous call to the
        /// <c>CreateToolhelp32Snapshot</c> function.
        /// </param>
        /// 
        /// <param name="lpte">
        /// A pointer to a <c>THREADENTRY32</c> structure.
        /// </param>
        /// 
        /// <returns>
        /// Returns TRUE if the first entry of the module list has been copied
        /// to the buffer or FALSE otherwise.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern bool Thread32First(IntPtr hSnapshot, ref THREADENTRY32 lpte);

        /// <summary>
        /// Retrieves information about the next thread of any process
        /// encountered in a system snapshot.
        /// </summary>
        /// 
        /// <param name="hSnapshot">
        /// A handle to the snapshot returned from a previous call to the
        /// <c>CreateToolhelp32Snapshot</c> function.
        /// </param>
        /// 
        /// <param name="lpte">
        /// A pointer to a <c>THREADENTRY32</c> structure.
        /// </param>
        /// 
        /// <returns>
        /// Returns TRUE if the first entry of the module list has been copied
        /// to the buffer or FALSE otherwise.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern bool Thread32Next(IntPtr hSnapshot, ref THREADENTRY32 lpte);

        /// <summary>
        /// Copies a string into the specified section of an initialization file.
        /// </summary>
        /// 
        /// <param name="lpAppName">
        /// The name of the section to which the string will be copied. If the
        /// section does not exist, it is created. The name of the section is
        /// case-independent; the string can be any combination of uppercase and
        /// lowercase letters.
        /// </param>
        /// 
        /// <param name="lpKeyName">
        /// The name of the key to be associated with a string. If the key does
        /// not exist in the specified section, it is created. If this parameter
        /// is NULL, the entire section, including all entries within the
        /// section, is deleted.
        /// </param>
        /// 
        /// <param name="lpString">
        /// A null-terminated string to be written to the file. If this parameter
        /// is NULL, the key pointed to by the lpKeyName parameter is deleted.
        /// </param>
        /// <param name="lpFileName">
        /// The name of the initialization file.
        /// </param>
        /// 
        /// <returns>
        /// If the function successfully copies the string to the initialization
        /// file, the return value is nonzero.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WritePrivateProfileString(string lpAppName, 
                                                           string lpKeyName,
                                                           string lpString,
                                                           string lpFileName);

        #endregion
        #region ntdll.dll

        /// <summary>
        /// Retrieves information about the specified thread.
        /// </summary>
        /// 
        /// <param name="threadHandle">
        /// A handle to the thread about which information is being requested.
        /// </param>
        /// 
        /// <param name="threadInformationClass">
        /// If this parameter is the ThreadQuerySetWin32StartAddress value of
        /// the THREADINFOCLASS enumeration, the function returns the start
        /// address of the thread.
        /// </param>
        /// 
        /// <param name="threadInformation">
        /// A pointer to a buffer in which the function writes the requested
        /// information.
        /// </param>
        /// <param name="threadInformationLength">
        /// The size of the buffer pointed to by the <c>ThreadInformation</c>
        /// parameter, in bytes.
        /// </param>
        /// 
        /// <param name="returnLength">
        /// A pointer to a variable in which the function returns the size of
        /// the requested information.
        /// </param>
        /// 
        /// <returns>
        /// Returns an NTSTATUS success or error code.
        /// </returns>
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtQueryInformationThread(IntPtr threadHandle,
                                                          THREADINFOCLASS threadInformationClass, 
                                                          IntPtr threadInformation, 
                                                          int threadInformationLength, 
                                                          IntPtr returnLength);

        #endregion
        #region shell32.dll

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        #endregion
        #region shlwapi.dll

        /// <summary>
        /// Converts a numeric value into a string that represents the number
        /// expressed as a size value in bytes, kilobytes, megabytes, or
        /// gigabytes, depending on the size.
        /// </summary>
        /// 
        /// <param name="fileSize">
        /// The numeric value to be converted.
        /// </param>
        /// 
        /// <param name="buffer">
        /// A pointer to a buffer that receives the converted string.
        /// </param>
        /// 
        /// <param name="bufferSize">
        /// The size of the buffer pointed to by <c>buffer</c>.
        /// </param>
        /// 
        /// <returns>
        /// Returns a pointer to the converted string, or NULL if the conversion fails.
        /// </returns>
        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern long StrFormatByteSize(long fileSize, StringBuilder buffer, int bufferSize);

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
        public static extern uint GetClassLongPtr32(IntPtr hWnd, ClassLongIndex nIndex);

        /// <summary>
        /// Same as GetClassLongPtr32, but for x64 systems.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, ClassLongIndex nIndex);

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
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

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
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Retrieves a handle to a device context (DC) for the client area of
        /// a specified window or for the entire screen.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window whose DC is to be retrieved. If this value
        /// is NULL, GetDC retrieves the DC for the entire screen.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC
        /// for the specified window's client area.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

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
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

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
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

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
        public static extern int GetWindowLong(IntPtr hWnd, WindowLongIndex nIndex);

        /// <summary>
        /// Same as GetWindowLong, but for compatibility with both 32-bit and
        /// 64-bit versions of Windows
        /// </summary>
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongA")]
        public static extern int GetWindowLongPtr(IntPtr hwnd, WindowLongIndex nIndex);

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized
        /// positions of the specified window.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// 
        /// <param name="lpwndpl">
        /// A pointer to the WINDOWPLACEMENT structure that receives the show
        /// state and position information.
        /// </param>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

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
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

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
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

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
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);

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
        public static extern bool InsertMenu(IntPtr hMenu, uint uPosition,
                                                           InsertMenuFlags uFlags, 
                                                           uint uIDNewItem,
                                                           [MarshalAs(UnmanagedType.LPTStr)]string lpNewItem);

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
        public static extern int InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

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
        public static extern bool IsRectEmpty(ref RECT lprc);

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
        public static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// Determines whether the specified window is enabled for mouse and
        /// keyboard input.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window to be tested.
        /// </param>
        /// 
        /// <returns>
        /// If the window is enabled, the return value is nonzero.
        /// If the window is not enabled, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        /// <summary>
        /// Determines whether the specified window is a native Unicode window.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window to be tested.
        /// </param>
        /// 
        /// <returns>
        /// If the window is a native Unicode window, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindowUnicode(IntPtr hWnd);

        /// <summary>
        /// Determines the visibility state of the specified window.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window to be tested.
        /// </param>
        /// 
        /// <returns>
        /// If the specified window, its parent window, its parent's parent
        /// window, and so forth, have the WS_VISIBLE style, the return value is
        /// nonzero. Otherwise, the return value is zero. 
        /// 
        /// Because the return value specifies whether the window has the
        /// WS_VISIBLE style, it may be nonzero even if the window is totally
        /// obscured by other windows. 
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// Determines whether a window is maximized.
        /// </summary>
        /// 
        /// <param name="hWnd">
        /// A handle to the window to be tested.
        /// </param>
        /// 
        /// <returns>
        /// If the window is zoomed, the return value is nonzero.
        /// If the window is not zoomed, the return value is zero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// Loads the specified cursor resource from the executable (.EXE) file
        /// associated with an application instance.
        /// </summary>
        ///
        /// <param name="hInstance">
        /// A handle to an instance of the module whose executable file contains the
        /// cursor to be loaded.
        /// </param>
        ///
        /// <param name="lpCursorName">
        /// The name of the cursor resource to be loaded.
        /// </param>
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        /// <summary>
        /// The OffsetRect function moves the specified rectangle by the
        /// specified offsets.
        /// </summary>
        /// 
        /// <param name="lprc">
        /// Pointer to a RECT structure that contains the logical coordinates of
        /// the rectangle to be moved.
        /// </param>
        /// 
        /// <param name="dx">
        /// Specifies the amount to move the rectangle left or right. This
        /// parameter must be a negative value to move the rectangle to the left.
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
        public static extern bool OffsetRect(ref RECT lprc, int dx, int dy);

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
        public static extern bool RedrawWindow(IntPtr hWnd,
                                               IntPtr lprcUpdate,
                                               IntPtr hrgnUpdate,
                                               RedrawWindowFlags flags);

        /// <summary>
        /// Releases the mouse capture from a window in the current thread and
        /// restores normal mouse input processing.
        /// </summary>
        /// 
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// The ReleaseDC function releases a device context (DC), freeing it
        /// for use by other applications. The effect of the ReleaseDC function
        /// depends on the type of DC. It frees only common and window DCs. It
        /// has no effect on class or private DCs.
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
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

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
        public static extern IntPtr SetCapture(IntPtr hWnd);

        /// <summary>
        /// Sets the cursor shape.
        /// </summary>
        ///
        /// <param name="hCursor">
        /// A handle to the cursor.
        /// </param>
        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr hCursor);

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
        public static extern bool UpdateWindow(IntPtr hWnd);

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
        public static extern IntPtr WindowFromPoint(Point point);

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
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, 
                                                             string pszSubIdList);

        #endregion
    }
}