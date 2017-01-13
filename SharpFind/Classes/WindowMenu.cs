using System;
using System.Runtime.InteropServices;

namespace SharpFind.Classes
{
    public class WindowMenu
    {
        #region Constants

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

        // Single window menu items
        public const int MNU_ABOUT = 1000;
        public const int MNU_LICENSE = 1001;
        public const int MNU_CHANGELOG = 1002;

        #endregion
        #region user32.dll Functions

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
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool InsertMenu(IntPtr hMenu, uint uPosition, uint uFlags, uint uIDNewItem, [MarshalAs(UnmanagedType.LPTStr)]string lpNewItem);

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

        #endregion
    }
}