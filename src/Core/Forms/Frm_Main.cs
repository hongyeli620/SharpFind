﻿/* Frm_Main.cs
** This file is part #Find.
** 
** Copyright 2018 by Jad Altahan <xviyy@aol.com>
** Licensed under MIT
** <https://github.com/xv/SharpFind/blob/master/LICENSE>
*/

using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System;

using SharpFind.Classes;
using SharpFind.Forms;
using SharpFind.Properties;

// <using static> is a C#6 feature. See:
// https://blogs.msdn.microsoft.com/csharpfaq/2014/11/20/new-features-in-c-6/
// C#6 IDE support starts at Visual Studio 2013 and up
using static SharpFind.Classes.NativeMethods.Styles.ButtonControlStyles;
using static SharpFind.Classes.NativeMethods.Styles.ComboBoxStyles;
using static SharpFind.Classes.NativeMethods.Styles.DateTimeControlStyles;
using static SharpFind.Classes.NativeMethods.Styles.DialogBoxStyles;
using static SharpFind.Classes.NativeMethods.Styles.EditControlStyles;
using static SharpFind.Classes.NativeMethods.Styles.HeaderControlStyles;
using static SharpFind.Classes.NativeMethods.Styles.ListBoxStyles;
using static SharpFind.Classes.NativeMethods.Styles.ListViewStyles;
using static SharpFind.Classes.NativeMethods.Styles.MDIClientStyles;
using static SharpFind.Classes.NativeMethods.Styles.TreeViewControlStyles;
using static SharpFind.Classes.NativeMethods.Styles.WindowStyles;
using static SharpFind.Classes.NativeMethods.Styles.WindowStylesEx;

namespace SharpFind
{
    public partial class Frm_Main : Form
    {
        public Frm_Main()
        {
            InitializeComponent();

            // Form appearance
            appName = IsRunningAsAdmin() ? Text = Application.ProductName + " (admin)" :
                                           Text = Application.ProductName;
            Text = appName;

            ControlBox      = true;
            MinimizeBox     = true;
            MaximizeBox     = false;
            ShowIcon        = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition   = FormStartPosition.CenterScreen;

            SetSizeBasedOnDpi();

            using (var ms = new MemoryStream(Resources.finder))
            {
                _cursorDefault = Cursor.Current;
                _cursorFinder = new Cursor(ms);
            }
        }

        #region Fields

        private readonly string appName;

        private int formHeightCollapsed = 100;
        private int formHeightExtended = 215;

        // Cursors to be used
        private readonly Cursor _cursorDefault;
        private readonly Cursor _cursorFinder;

        private bool isHandleNull;
        private bool isCapturing;
        private IntPtr hPreviousWindow;
        private IntPtr hWnd;
        private IntPtr hWndOld;
        private int targetPID;

        // Hexadecimal prefix and format
        private static string hPrefix = "";
        private static string hFormat = "X8";

        // Single window menu items
        private const int MNU_ABOUT = 1000;
        private const int MNU_LICENSE = 1001;
        private const int MNU_CHANGELOG = 1002;
        private const int MNU_ADMIN = 1003;

        #endregion
        #region Window Procedure

        protected override void WndProc(ref Message m)
        {
            // Handle the Window Menu item click events
            if (m.Msg == (int)Win32.WindowsMessages.WM_SYSCOMMAND)
            {
                switch (m.WParam.ToInt32())
                {
                    case MNU_ABOUT:
                        ShowAboutDialog();
                        break;
                    case MNU_CHANGELOG:
                        ShowChangelog();
                        break;
                    case MNU_LICENSE:
                        ShowLicense();
                        break;
                    case MNU_ADMIN:
                        RunAsAdministrator();
                        break;
                }
            }

            // Handle the Finder Tool drag & release
            switch (m.Msg)
            {
                case (int)Win32.WindowsMessages.WM_LBUTTONUP:
                    CaptureMouse(false);
                    break;
                case (int)Win32.WindowsMessages.WM_MOUSEMOVE:
                    HandleMouseMovement();
                    break;
                case (int)Win32.WindowsMessages.WM_PAINT:
                    if (LV_WindowStyles.View == View.Details && LV_WindowStyles.Columns.Count > 0)
                        LV_WindowStyles.Columns[LV_WindowStyles.Columns.Count - 1].Width = -2;

                    if (LV_ExtendedStyles.View == View.Details && LV_ExtendedStyles.Columns.Count > 0)
                        LV_ExtendedStyles.Columns[LV_ExtendedStyles.Columns.Count - 1].Width = -2;
                    break;
            }

            base.WndProc(ref m);
        }

        #endregion
        #region Functions

        #region DPI Check

        private static Point GetSystemDpi()
        {
            var result = new Point();
            var hDC = Win32.GetDC(IntPtr.Zero);

            result.X = Win32.GetDeviceCaps(hDC, 88);
            result.Y = Win32.GetDeviceCaps(hDC, 90);

            Win32.ReleaseDC(IntPtr.Zero, hDC);

            return result;
        }

        private static bool IsDpi96()
        {
            var result = GetSystemDpi();
            return result.X == 96 || result.Y == 96;
        }

        #endregion

        /// <summary>
        /// Checks if the the program is running as administrator.
        /// </summary>
        private static bool IsRunningAsAdmin()
        {
            var winIdentity = WindowsIdentity.GetCurrent();
            var adminRole   = WindowsBuiltInRole.Administrator;

            return Environment.OSVersion.Version.Major >= 6 && 
                   new WindowsPrincipal(winIdentity).IsInRole(adminRole);
        }

        #region General

        private string GetWindowText(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            Win32.GetWindowText(hWnd, sb, 256);
            return sb.ToString();
        }

        private string GetWindowClass(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            Win32.GetClassName(hWnd, sb, 256);
        
            var value = sb.ToString();
            if (Win32.IsWindowUnicode(hWnd))
                value = value + " (unicode)";

            return value;
        }

        private string GetWindowRect(IntPtr hWnd)
        {
            Win32.RECT wRect;
            Win32.GetWindowRect(hWnd, out wRect);
            var winState = Win32.IsZoomed(hWnd) ? " (maximized)" : string.Empty;

            return string.Format("({0},{1}) - ({2},{3}), {4} x {5}{6}", 
                                 wRect.left,
                                 wRect.top,
                                 wRect.right,
                                 wRect.bottom,
                                 wRect.right - wRect.left,
                                 wRect.bottom - wRect.top,
                                 winState);
        }

        private string GetRestoredRect(IntPtr hWnd)
        {
            var wp = new Win32.WINDOWPLACEMENT();
            Win32.GetWindowPlacement(hWnd, ref wp);
            return string.Format("({0},{1}) - ({2},{3}), {4} x {5}", 
                                 wp.rcNormalPosition.left,
                                 wp.rcNormalPosition.top,
                                 wp.rcNormalPosition.right,
                                 wp.rcNormalPosition.bottom,
                                 wp.rcNormalPosition.right  - wp.rcNormalPosition.left,
                                 wp.rcNormalPosition.bottom - wp.rcNormalPosition.top);
        }

        private string GetClientRect(IntPtr hWnd)
        {
            Win32.RECT cRect, wRect;
            Win32.POINT pt;

            Win32.GetClientRect(hWnd, out cRect);

            pt.X = cRect.left;
            pt.Y = cRect.top;

            Win32.MapWindowPoints(hWnd, IntPtr.Zero, ref pt, 2);

            Win32.GetWindowRect(hWnd, out wRect);
            var absoluteX = pt.X - wRect.left;
            var absoluteY = pt.Y - wRect.top;

            return string.Format("({0},{1}) - ({2},{3}), {4} x {5}", 
                                 absoluteX,
                                 absoluteY,
                                 cRect.right + absoluteX,
                                 cRect.bottom + absoluteY,
                                 cRect.right,
                                 cRect.bottom);
        }

        private string GetInstanceHandle(IntPtr hWnd)
        {
            return hPrefix + Win32.GetWindowLong(hWnd, Win32.WindowLongIndex.GWL_HINSTANCE).ToString(hFormat);
        }

        private string GetControlId(IntPtr hWnd)
        {
            return hPrefix + Win32.GetWindowLong(hWnd, Win32.WindowLongIndex.GWL_ID).ToString(hFormat);
        }

        private string GetUserData(IntPtr hWnd)
        {
            return hPrefix + Win32.GetWindowLong(hWnd, Win32.WindowLongIndex.GWL_USERDATA).ToString(hFormat);
        }

        private void GetWindowBytesCombo(IntPtr hWnd)
        {
            var value = (long)GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCL_CBWNDEXTRA);
            var i = 0;

            CMB_WindowBytes.Items.Clear();
            CMB_WindowBytes.Enabled = value != 0;

            while (value != 0)
            {
                if (value >= 4)
                {
                    // <GetWindowLongPtr> is used here, otherwise it won't work right
                    // Dealing with x68/x64 compatibility is really a pain in the ass
                    var bytes = Win32.GetWindowLongPtr(hWnd, (Win32.WindowLongIndex)i).ToString(hFormat);
                    CMB_WindowBytes.Items.Add("+" + i + "       " + hPrefix + bytes);
                }
                else
                    CMB_WindowBytes.Items.Add("+" + i + "       " + "(Unavailable)");

                i += 4; // 0, 4, 8, 12, 16, etc
                value = Math.Max(value -4, 0);
            }

            // Select the first index, if any
            if (CMB_WindowBytes.Items.Count != 0)
                CMB_WindowBytes.SelectedIndex = 0;
        }

        #endregion
        #region Styles

        private void DumpStyle(string style, string styleValue)
        {
            var item = LV_WindowStyles.Items.Add(style);
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(styleValue).ForeColor = Color.DimGray;
            item.SubItems[1].Font = new Font("Lucida Sans Typewriter", 8f, FontStyle.Regular);
        }

        private void DumpStyleEx(string style, string styleValue)
        {
            var item = LV_ExtendedStyles.Items.Add(style);
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(styleValue).ForeColor = Color.DimGray;
            item.SubItems[1].Font = new Font("Lucida Sans Typewriter", 8f, FontStyle.Regular);
        }

        private string GetWindowStyles(IntPtr hWnd)
        {
            var i = Win32.GetWindowLongPtr(hWnd, Win32.WindowLongIndex.GWL_STYLE);
            LV_WindowStyles.Items.Clear();

            if (i != 0)
            {
                if ((i & WS_BORDER) != 0) DumpStyle("WS_BORDER", WS_BORDER.ToString("X8"));
                if ((i & WS_CAPTION) == WS_CAPTION)
                {
                    DumpStyle("WS_CAPTION", WS_CAPTION.ToString("X8"));

                    if ((i & WS_SYSMENU) != 0)
                        DumpStyle("WS_SYSMENU", WS_SYSMENU.ToString("X8"));
                }
                if ((i & WS_CHILD) != 0)
                {
                    DumpStyle("WS_CHILD", WS_CHILD.ToString("X8"));

                    if ((i & WS_TABSTOP) != 0)
                        DumpStyle("WS_TABSTOP", WS_TABSTOP.ToString("X8"));
                    if ((i & WS_GROUP) != 0)
                        DumpStyle("WS_GROUP", WS_GROUP.ToString("X8"));
                }
                else
                {
                    if ((i & WS_POPUP) != 0)
                        DumpStyle("WS_POPUP", WS_POPUP.ToString("X8"));

                    if ((i & WS_SYSMENU) != 0)
                    {
                        if ((i & WS_MINIMIZEBOX) != 0)
                            DumpStyle("WS_MINIMIZEBOX", WS_MINIMIZEBOX.ToString("X8"));
                        if ((i & WS_MAXIMIZEBOX) != 0)
                            DumpStyle("WS_MAXIMIZEBOX", WS_MAXIMIZEBOX.ToString("X8"));
                    }
                }
                if ((i & WS_CLIPCHILDREN) != 0) DumpStyle("WS_CLIPCHILDREN", WS_CLIPCHILDREN.ToString("X8"));
                if ((i & WS_CLIPSIBLINGS) != 0) DumpStyle("WS_CLIPSIBLINGS", WS_CLIPSIBLINGS.ToString("X8"));
                if ((i & WS_DISABLED)     != 0) DumpStyle("WS_DISABLED",     WS_DISABLED.ToString("X8"));
                if ((i & WS_DLGFRAME)     != 0) DumpStyle("WS_DLGFRAME",     WS_DLGFRAME.ToString("X8"));
                if ((i & WS_HSCROLL)      != 0) DumpStyle("WS_HSCROLL",      WS_HSCROLL.ToString("X8"));
                if ((i & WS_MAXIMIZE)     != 0) DumpStyle("WS_MAXIMIZE",     WS_MAXIMIZE.ToString("X8"));
                if ((i & WS_MINIMIZE)     != 0) DumpStyle("WS_MINIMIZE",     WS_MINIMIZE.ToString("X8"));
                if ((i & WS_OVERLAPPED)   != 0) DumpStyle("WS_OVERLAPPED",   WS_OVERLAPPED.ToString("X8"));
                if ((i & WS_THICKFRAME)   != 0) DumpStyle("WS_THICKFRAME",   WS_THICKFRAME.ToString("X8"));
                if ((i & WS_VISIBLE)      != 0) DumpStyle("WS_VISIBLE",      WS_VISIBLE.ToString("X8"));
                if ((i & WS_VSCROLL)      != 0) DumpStyle("WS_VSCROLL",      WS_VSCROLL.ToString("X8"));

                if ((i & WS_OVERLAPPEDWINDOW) == WS_OVERLAPPEDWINDOW)
                    DumpStyle("WS_OVERLAPPEDWINDOW", WS_OVERLAPPEDWINDOW.ToString("X8"));
                if ((i & WS_POPUPWINDOW) == WS_POPUPWINDOW)
                    DumpStyle("WS_POPUPWINDOW", WS_POPUPWINDOW.ToString("X8"));

                if (TB_Class.Text.StartsWith("Button"))
                {
                    if ((i & BS_BITMAP)      != 0)       DumpStyle("BS_BITMAP",          BS_BITMAP.ToString("X8"));
                    if ((i & BS_FLAT)        != 0)       DumpStyle("BS_FLAT",            BS_FLAT.ToString("X8"));
                    if ((i & BS_ICON)        != 0)       DumpStyle("BS_ICON",            BS_ICON.ToString("X8"));
                    if ((i & BS_LEFTTEXT)    != 0)       DumpStyle("BS_LEFTTEXT",        BS_LEFTTEXT.ToString("X8"));
                    if ((i & BS_MULTILINE)   != 0)       DumpStyle("BS_MULTILINE",       BS_MULTILINE.ToString("X8"));
                    if ((i & BS_NOTIFY)      != 0)       DumpStyle("BS_NOTIFY",          BS_NOTIFY.ToString("X8"));
                    if ((i & BS_PUSHLIKE)    != 0)       DumpStyle("BS_PUSHLIKE",        BS_PUSHLIKE.ToString("X8"));
                    if ((i & BS_RIGHTBUTTON) != 0)       DumpStyle("BS_RIGHTBUTTON",     BS_RIGHTBUTTON.ToString("X8"));
                    if ((i & BS_TEXT)        != 0)       DumpStyle("BS_TEXT",            BS_TEXT.ToString("X8"));

                    if ((i & 0xf) == BS_PUSHBUTTON)      DumpStyle("BS_PUSHBUTTON",      BS_PUSHBUTTON.ToString("X8"));
                    if ((i & 0xf) == BS_DEFPUSHBUTTON)   DumpStyle("BS_DEFPUSHBUTTON",   BS_DEFPUSHBUTTON.ToString("X8"));
                    if ((i & 0xf) == BS_CHECKBOX)        DumpStyle("BS_CHECKBOX",        BS_CHECKBOX.ToString("X8"));
                    if ((i & 0xf) == BS_AUTOCHECKBOX)    DumpStyle("BS_AUTOCHECKBOX",    BS_AUTOCHECKBOX.ToString("X8"));
                    if ((i & 0xf) == BS_RADIOBUTTON)     DumpStyle("BS_RADIOBUTTON",     BS_RADIOBUTTON.ToString("X8"));
                    if ((i & 0xf) == BS_3STATE)          DumpStyle("BS_3STATE",          BS_3STATE.ToString("X8"));
                    if ((i & 0xf) == BS_AUTO3STATE)      DumpStyle("BS_AUTO3STATE",      BS_AUTO3STATE.ToString("X8"));
                    if ((i & 0xf) == BS_GROUPBOX)        DumpStyle("BS_GROUPBOX",        BS_GROUPBOX.ToString("X8"));
                    if ((i & 0xf) == BS_USERBUTTON)      DumpStyle("BS_USERBUTTON",      BS_USERBUTTON.ToString("X8"));
                    if ((i & 0xf) == BS_AUTORADIOBUTTON) DumpStyle("BS_AUTORADIOBUTTON", BS_AUTORADIOBUTTON.ToString("X8"));
                    if ((i & 0xf) == BS_OWNERDRAW)       DumpStyle("BS_OWNERDRAW",       BS_OWNERDRAW.ToString("X8"));
                                                         
                    if ((i & BS_BOTTOM)  == BS_BOTTOM)   DumpStyle("BS_BOTTOM",          BS_RIGHT.ToString("X8"));
                    if ((i & BS_CENTER)  == BS_CENTER)   DumpStyle("BS_CENTER",          BS_CENTER.ToString("X8"));
                    if ((i & BS_LEFT)    == BS_LEFT)     DumpStyle("BS_LEFT",            BS_LEFT.ToString("X8"));
                    if ((i & BS_RIGHT)   == BS_RIGHT)    DumpStyle("BS_RIGHT",           BS_RIGHT.ToString("X8"));
                    if ((i & BS_TOP)     == BS_TOP)      DumpStyle("BS_TOP",             BS_LEFT.ToString("X8"));
                    if ((i & BS_VCENTER) == BS_VCENTER)  DumpStyle("BS_VCENTER",         BS_CENTER.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("ComboBox"))
                {
                    if ((i & CBS_AUTOHSCROLL)       != 0) DumpStyle("CBS_AUTOHSCROLL",       CBS_AUTOHSCROLL.ToString("X8"));
                    if ((i & CBS_DISABLENOSCROLL)   != 0) DumpStyle("CBS_DISABLENOSCROLL",   CBS_DISABLENOSCROLL.ToString("X8"));
                    if ((i & CBS_HASSTRINGS)        != 0) DumpStyle("CBS_HASSTRINGS",        CBS_HASSTRINGS.ToString("X8"));
                    if ((i & CBS_LOWERCASE)         != 0) DumpStyle("CBS_LOWERCASE",         CBS_LOWERCASE.ToString("X8"));
                    if ((i & CBS_NOINTEGRALHEIGHT)  != 0) DumpStyle("CBS_NOINTEGRALHEIGHT",  CBS_NOINTEGRALHEIGHT.ToString("X8"));
                    if ((i & CBS_OEMCONVERT)        != 0) DumpStyle("CBS_OEMCONVERT",        CBS_OEMCONVERT.ToString("X8"));
                    if ((i & CBS_OWNERDRAWFIXED)    != 0) DumpStyle("CBS_OWNERDRAWFIXED",    CBS_OWNERDRAWFIXED.ToString("X8"));
                    if ((i & CBS_OWNERDRAWVARIABLE) != 0) DumpStyle("CBS_OWNERDRAWVARIABLE", CBS_OWNERDRAWVARIABLE.ToString("X8"));
                    if ((i & CBS_SORT)              != 0) DumpStyle("CBS_SORT",              CBS_SORT.ToString("X8"));
                    if ((i & CBS_UPPERCASE)         != 0) DumpStyle("CBS_UPPERCASE",         CBS_UPPERCASE.ToString("X8"));

                    if ((i & 0x3) == CBS_SIMPLE)          DumpStyle("CBS_SIMPLE",            CBS_SIMPLE.ToString("X8"));
                    if ((i & 0x3) == CBS_DROPDOWN)        DumpStyle("CBS_DROPDOWN",          CBS_DROPDOWN.ToString("X8"));
                    if ((i & 0x3) == CBS_DROPDOWNLIST)    DumpStyle("CBS_DROPDOWNLIST",      CBS_DROPDOWNLIST.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("SysDateTimePick32"))
                {
                    if ((i & DTS_UPDOWN) != 0)
                    {
                        DumpStyle("DTS_UPDOWN", DTS_UPDOWN.ToString("X8"));

                        if ((i & DTS_TIMEFORMAT) == DTS_TIMEFORMAT)
                            DumpStyle("DTS_TIMEFORMAT", DTS_TIMEFORMAT.ToString("X8"));
                    }

                    if ((i & DTS_APPCANPARSE)     != 0) DumpStyle("DTS_APPCANPARSE",     DTS_APPCANPARSE.ToString("X8"));
                    if ((i & DTS_RIGHTALIGN)      != 0) DumpStyle("DTS_RIGHTALIGN",      DTS_RIGHTALIGN.ToString("X8"));
                    if ((i & DTS_SHOWNONE)        != 0) DumpStyle("DTS_SHOWNONE",        DTS_SHOWNONE.ToString("X8"));
                    if ((i & DTS_SHORTDATEFORMAT) != 0) DumpStyle("DTS_SHORTDATEFORMAT", DTS_SHORTDATEFORMAT.ToString("X8"));
                    if ((i & DTS_LONGDATEFORMAT)  != 0) DumpStyle("DTS_LONGDATEFORMAT",  DTS_LONGDATEFORMAT.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("#32770"))
                {
                    if ((i & DS_ABSALIGN)      != 0) DumpStyle("DS_ABSALIGN",      DS_ABSALIGN.ToString("X8"));
                    if ((i & DS_SYSMODAL)      != 0) DumpStyle("DS_SYSMODAL",      DS_SYSMODAL.ToString("X8"));
                    if ((i & DS_LOCALEDIT)     != 0) DumpStyle("DS_LOCALEDIT",     DS_LOCALEDIT.ToString("X8"));
                    if ((i & DS_SETFONT)       != 0) DumpStyle("DS_SETFONT",       DS_SETFONT.ToString("X8"));
                    if ((i & DS_MODALFRAME)    != 0) DumpStyle("DS_MODALFRAME",    DS_MODALFRAME.ToString("X8"));
                    if ((i & DS_NOIDLEMSG )    != 0) DumpStyle("DS_NOIDLEMSG",     DS_NOIDLEMSG.ToString("X8"));
                    if ((i & DS_SETFOREGROUND) != 0) DumpStyle("DS_SETFOREGROUND", DS_SETFOREGROUND.ToString("X8"));
                    if ((i & DS_3DLOOK)        != 0) DumpStyle("DS_3DLOOK",        DS_3DLOOK.ToString("X8"));
                    if ((i & DS_FIXEDSYS)      != 0) DumpStyle("DS_FIXEDSYS",      DS_FIXEDSYS.ToString("X8"));
                    if ((i & DS_NOFAILCREATE)  != 0) DumpStyle("DS_NOFAILCREATE",  DS_NOFAILCREATE.ToString("X8"));
                    if ((i & DS_CONTROL)       != 0) DumpStyle("DS_CONTROL",       DS_CONTROL.ToString("X8"));
                    if ((i & DS_CENTER)        != 0) DumpStyle("DS_CENTER",        DS_CENTER.ToString("X8"));
                    if ((i & DS_CENTERMOUSE)   != 0) DumpStyle("DS_CENTERMOUSE",   DS_CENTERMOUSE.ToString("X8"));
                    if ((i & DS_CONTEXTHELP)   != 0) DumpStyle("DS_CONTEXTHELP",   DS_CONTEXTHELP.ToString("X8"));
                    if ((i & DS_USEPIXELS)     != 0) DumpStyle("DS_USEPIXELS",     DS_USEPIXELS.ToString("X8"));

                    if ((i & DS_SHELLFONT) == DS_SHELLFONT)
                        DumpStyle("DS_SHELLFONT", DS_SHELLFONT.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("Edit"))
                {
                    if ((i & ES_LEFT)        != 0) DumpStyle("ES_LEFT",        ES_LEFT.ToString("X8"));
                    if ((i & ES_CENTER)      != 0) DumpStyle("ES_CENTER",      ES_CENTER.ToString("X8"));
                    if ((i & ES_RIGHT)       != 0) DumpStyle("ES_RIGHT",       ES_RIGHT.ToString("X8"));
                    if ((i & ES_MULTILINE)   != 0) DumpStyle("ES_MULTILINE",   ES_MULTILINE.ToString("X8"));
                    if ((i & ES_UPPERCASE)   != 0) DumpStyle("ES_UPPERCASE",   ES_UPPERCASE.ToString("X8"));
                    if ((i & ES_LOWERCASE)   != 0) DumpStyle("ES_LOWERCASE",   ES_LOWERCASE.ToString("X8"));
                    if ((i & ES_PASSWORD)    != 0) DumpStyle("ES_PASSWORD",    ES_PASSWORD.ToString("X8"));
                    if ((i & ES_AUTOVSCROLL) != 0) DumpStyle("ES_AUTOVSCROLL", ES_AUTOVSCROLL.ToString("X8"));
                    if ((i & ES_AUTOHSCROLL) != 0) DumpStyle("ES_AUTOHSCROLL", ES_AUTOHSCROLL.ToString("X8"));
                    if ((i & ES_NOHIDESEL)   != 0) DumpStyle("ES_NOHIDESEL",   ES_NOHIDESEL.ToString("X8"));
                    if ((i & ES_OEMCONVERT)  != 0) DumpStyle("ES_OEMCONVERT",  ES_OEMCONVERT.ToString("X8"));
                    if ((i & ES_READONLY)    != 0) DumpStyle("ES_READONLY",    ES_READONLY.ToString("X8"));
                    if ((i & ES_WANTRETURN)  != 0) DumpStyle("ES_WANTRETURN",  ES_WANTRETURN.ToString("X8"));
                    if ((i & ES_NUMBER)      != 0) DumpStyle("ES_NUMBER",      ES_NUMBER.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("SysHeader32"))
                {
                    if ((i & HDS_HORZ)       != 0) DumpStyle("HDS_HORZ",       HDS_HORZ.ToString("X8"));
                    if ((i & HDS_BUTTONS)    != 0) DumpStyle("HDS_BUTTONS",    HDS_BUTTONS.ToString("X8"));
                    if ((i & HDS_HOTTRACK)   != 0) DumpStyle("HDS_HOTTRACK",   HDS_HOTTRACK.ToString("X8"));
                    if ((i & HDS_HIDDEN)     != 0) DumpStyle("HDS_HIDDEN",     HDS_HIDDEN.ToString("X8"));
                    if ((i & HDS_DRAGDROP)   != 0) DumpStyle("HDS_DRAGDROP",   HDS_DRAGDROP.ToString("X8"));
                    if ((i & HDS_FULLDRAG)   != 0) DumpStyle("HDS_FULLDRAG",   HDS_FULLDRAG.ToString("X8"));
                    if ((i & HDS_FILTERBAR)  != 0) DumpStyle("HDS_FILTERBAR",  HDS_FILTERBAR.ToString("X8"));
                    if ((i & HDS_FLAT)       != 0) DumpStyle("HDS_FLAT",       HDS_FLAT.ToString("X8"));
                    if ((i & HDS_CHECKBOXES) != 0) DumpStyle("HDS_CHECKBOXES", HDS_CHECKBOXES.ToString("X8"));
                    if ((i & HDS_NOSIZING)   != 0) DumpStyle("HDS_NOSIZING",   HDS_NOSIZING.ToString("X8"));
                    if ((i & HDS_OVERFLOW)   != 0) DumpStyle("HDS_OVERFLOW",   HDS_OVERFLOW.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("ListBox"))
                {
                    if ((i & LBS_NOTIFY)            != 0) DumpStyle("LBS_NOTIFY",            LBS_NOTIFY.ToString("X8"));
                    if ((i & LBS_SORT)              != 0) DumpStyle("LBS_SORT",              LBS_SORT.ToString("X8"));
                    if ((i & LBS_NOREDRAW)          != 0) DumpStyle("LBS_NOREDRAW",          LBS_NOREDRAW.ToString("X8"));
                    if ((i & LBS_MULTIPLESEL)       != 0) DumpStyle("LBS_MULTIPLESEL",       LBS_MULTIPLESEL.ToString("X8"));
                    if ((i & LBS_OWNERDRAWFIXED)    != 0) DumpStyle("LBS_OWNERDRAWFIXED",    LBS_OWNERDRAWFIXED.ToString("X8"));
                    if ((i & LBS_OWNERDRAWVARIABLE) != 0) DumpStyle("LBS_OWNERDRAWVARIABLE", LBS_OWNERDRAWVARIABLE.ToString("X8"));
                    if ((i & LBS_HASSTRINGS)        != 0) DumpStyle("LBS_HASSTRINGS",        LBS_HASSTRINGS.ToString("X8"));
                    if ((i & LBS_USETABSTOPS)       != 0) DumpStyle("LBS_USETABSTOPS",       LBS_USETABSTOPS.ToString("X8"));
                    if ((i & LBS_NOINTEGRALHEIGHT)  != 0) DumpStyle("LBS_NOINTEGRALHEIGHT",  LBS_NOINTEGRALHEIGHT.ToString("X8"));
                    if ((i & LBS_MULTICOLUMN)       != 0) DumpStyle("LBS_MULTICOLUMN",       LBS_MULTICOLUMN.ToString("X8"));
                    if ((i & LBS_WANTKEYBOARDINPUT) != 0) DumpStyle("LBS_WANTKEYBOARDINPUT", LBS_WANTKEYBOARDINPUT.ToString("X8"));
                    if ((i & LBS_EXTENDEDSEL)       != 0) DumpStyle("LBS_EXTENDEDSEL",       LBS_EXTENDEDSEL.ToString("X8"));
                    if ((i & LBS_DISABLENOSCROLL)   != 0) DumpStyle("LBS_DISABLENOSCROLL",   LBS_DISABLENOSCROLL.ToString("X8"));
                    if ((i & LBS_NODATA)            != 0) DumpStyle("LBS_NODATA",            LBS_NODATA.ToString("X8"));
                    if ((i & LBS_NOSEL)             != 0) DumpStyle("LBS_NOSEL",             LBS_NOSEL.ToString("X8"));
                    if ((i & LBS_COMBOBOX)          != 0) DumpStyle("LBS_COMBOBOX",          LBS_COMBOBOX.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("SysListView32"))
                {
                    if ((i & LVS_TYPEMASK) == LVS_ICON)       DumpStyle("LVS_ICON",            LVS_ICON.ToString("X8"));
                    if ((i & LVS_TYPEMASK) == LVS_REPORT)     DumpStyle("LVS_REPORT",          LVS_REPORT.ToString("X8"));
                    if ((i & LVS_TYPEMASK) == LVS_SMALLICON)  DumpStyle("LVS_SMALLICON",       LVS_SMALLICON.ToString("X8"));
                    if ((i & LVS_TYPEMASK) == LVS_LIST)       DumpStyle("LVS_LIST",            LVS_LIST.ToString("X8"));

                    if ((i & LVS_SINGLESEL)       != 0)       DumpStyle("LVS_SINGLESEL",       LVS_SINGLESEL.ToString("X8"));
                    if ((i & LVS_SHOWSELALWAYS)   != 0)       DumpStyle("LVS_SHOWSELALWAYS",   LVS_SHOWSELALWAYS.ToString("X8"));
                    if ((i & LVS_SORTASCENDING)   != 0)       DumpStyle("LVS_SORTASCENDING",   LVS_SORTASCENDING.ToString("X8"));
                    if ((i & LVS_SORTDESCENDING)  != 0)       DumpStyle("LVS_SORTDESCENDING",  LVS_SORTDESCENDING.ToString("X8"));
                    if ((i & LVS_SHAREIMAGELISTS) != 0)       DumpStyle("LVS_SHAREIMAGELISTS", LVS_SHAREIMAGELISTS.ToString("X8"));
                    if ((i & LVS_NOLABELWRAP)     != 0)       DumpStyle("LVS_NOLABELWRAP",     LVS_NOLABELWRAP.ToString("X8"));
                    if ((i & LVS_AUTOARRANGE)     != 0)       DumpStyle("LVS_AUTOARRANGE",     LVS_AUTOARRANGE.ToString("X8"));
                    if ((i & LVS_EDITLABELS)      != 0)       DumpStyle("LVS_EDITLABELS",      LVS_EDITLABELS.ToString("X8"));
                    if ((i & LVS_OWNERDATA)       != 0)       DumpStyle("LVS_OWNERDATA",       LVS_OWNERDATA.ToString("X8"));
                    if ((i & LVS_NOSCROLL)        != 0)       DumpStyle("LVS_NOSCROLL",        LVS_NOSCROLL.ToString("X8"));

                    if ((i & LVS_ALIGNMASK) == LVS_ALIGNTOP)  DumpStyle("LVS_ALIGNTOP",        LVS_ALIGNTOP.ToString("X8"));
                    if ((i & LVS_ALIGNMASK) == LVS_ALIGNLEFT) DumpStyle("LVS_ALIGNLEFT",       LVS_ALIGNLEFT.ToString("X8"));

                    if ((i & LVS_OWNERDRAWFIXED)  != 0)       DumpStyle("LVS_OWNERDRAWFIXED",  LVS_OWNERDRAWFIXED.ToString("X8"));
                    if ((i & LVS_NOCOLUMNHEADER)  != 0)       DumpStyle("LVS_NOCOLUMNHEADER",  LVS_NOCOLUMNHEADER.ToString("X8"));
                    if ((i & LVS_NOSORTHEADER)    != 0)       DumpStyle("LVS_NOSORTHEADER",    LVS_NOSORTHEADER.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("MDIClient"))
                    if ((i & MDIS_ALLCHILDSTYLES) != 0) DumpStyle("MDIS_ALLCHILDSTYLES", MDIS_ALLCHILDSTYLES.ToString("X8"));

                if (TB_Class.Text.StartsWith("SysTreeView32"))
                {
                    if ((i & TVS_CHECKBOXES)      != 0) DumpStyle("TVS_CHECKBOXES",      TVS_CHECKBOXES.ToString("X8"));
                    if ((i & TVS_DISABLEDRAGDROP) != 0) DumpStyle("TVS_DISABLEDRAGDROP", TVS_DISABLEDRAGDROP.ToString("X8"));
                    if ((i & TVS_EDITLABELS)      != 0) DumpStyle("TVS_EDITLABELS",      TVS_EDITLABELS.ToString("X8"));
                    if ((i & TVS_FULLROWSELECT)   != 0) DumpStyle("TVS_FULLROWSELECT",   TVS_FULLROWSELECT.ToString("X8"));
                    if ((i & TVS_HASBUTTONS)      != 0) DumpStyle("TVS_HASBUTTONS",      TVS_HASBUTTONS.ToString("X8"));
                    if ((i & TVS_HASLINES)        != 0) DumpStyle("TVS_HASLINES",        TVS_HASLINES.ToString("X8"));
                    if ((i & TVS_INFOTIP)         != 0) DumpStyle("TVS_INFOTIP",         TVS_INFOTIP.ToString("X8"));
                    if ((i & TVS_LINESATROOT)     != 0) DumpStyle("TVS_LINESATROOT",     TVS_LINESATROOT.ToString("X8"));
                    if ((i & TVS_NOHSCROLL)       != 0) DumpStyle("TVS_NOHSCROLL",       TVS_NOHSCROLL.ToString("X8"));
                    if ((i & TVS_NONEVENHEIGHT)   != 0) DumpStyle("TVS_NONEVENHEIGHT",   TVS_NONEVENHEIGHT.ToString("X8"));
                    if ((i & TVS_NOSCROLL)        != 0) DumpStyle("TVS_NOSCROLL",        TVS_NOSCROLL.ToString("X8"));
                    if ((i & TVS_NOTOOLTIPS)      != 0) DumpStyle("TVS_NOTOOLTIPS",      TVS_NOTOOLTIPS.ToString("X8"));
                    if ((i & TVS_RTLREADING)      != 0) DumpStyle("TVS_RTLREADING",      TVS_RTLREADING.ToString("X8"));
                    if ((i & TVS_SHOWSELALWAYS)   != 0) DumpStyle("TVS_SHOWSELALWAYS",   TVS_SHOWSELALWAYS.ToString("X8"));
                    if ((i & TVS_SINGLEEXPAND)    != 0) DumpStyle("TVS_SINGLEEXPAND",    TVS_SINGLEEXPAND.ToString("X8"));
                    if ((i & TVS_TRACKSELECT)     != 0) DumpStyle("TVS_TRACKSELECT",     TVS_TRACKSELECT.ToString("X8"));
                }
            }

            var isEnabled = Win32.IsWindowEnabled(hWnd) ? "enabled" : "disabled";
            var isVisible = Win32.IsWindowVisible(hWnd) ? "visible" : "hidden";

            return $"{hPrefix + i.ToString(hFormat)} ({isEnabled}, {isVisible})";
        }

        private string GetWindowStylesEx(IntPtr hWnd)
        {
            var i = Win32.GetWindowLong(hWnd, Win32.WindowLongIndex.GWL_EXSTYLE);
            LV_ExtendedStyles.Items.Clear();

            if (i != 0)
            {
                if ((i & WS_EX_ACCEPTFILES)         != 0) DumpStyleEx("WS_EX_ACCEPTFILES",         WS_EX_ACCEPTFILES.ToString("X8"));
                if ((i & WS_EX_APPWINDOW)           != 0) DumpStyleEx("WS_EX_APPWINDOW",           WS_EX_APPWINDOW.ToString("X8"));
                if ((i & WS_EX_CLIENTEDGE)          != 0) DumpStyleEx("WS_EX_CLIENTEDGE",          WS_EX_CLIENTEDGE.ToString("X8"));
                if ((i & WS_EX_COMPOSITED)          != 0) DumpStyleEx("WS_EX_COMPOSITED",          WS_EX_COMPOSITED.ToString("X8"));
                if ((i & WS_EX_CONTEXTHELP)         != 0) DumpStyleEx("WS_EX_CONTEXTHELP",         WS_EX_CONTEXTHELP.ToString("X8"));
                if ((i & WS_EX_CONTROLPARENT)       != 0) DumpStyleEx("WS_EX_CONTROLPARENT",       WS_EX_CONTROLPARENT.ToString("X8"));
                if ((i & WS_EX_DLGMODALFRAME)       != 0) DumpStyleEx("WS_EX_DLGMODALFRAME",       WS_EX_DLGMODALFRAME.ToString("X8"));
                if ((i & WS_EX_LAYERED)             != 0) DumpStyleEx("WS_EX_LAYERED",             WS_EX_LAYERED.ToString("X8"));
                if ((i & WS_EX_LAYOUTRTL)           != 0) DumpStyleEx("WS_EX_LAYOUTRTL",           WS_EX_LAYOUTRTL.ToString("X8"));
                if ((i & WS_EX_LEFT)                != 0) DumpStyleEx("WS_EX_LEFT",                WS_EX_LEFT.ToString("X8"));
                if ((i & WS_EX_LEFTSCROLLBAR)       != 0) DumpStyleEx("WS_EX_LEFTSCROLLBAR",       WS_EX_LEFTSCROLLBAR.ToString("X8"));
                if ((i & WS_EX_LTRREADING)          != 0) DumpStyleEx("WS_EX_LTRREADING",          WS_EX_LTRREADING.ToString("X8"));
                if ((i & WS_EX_MDICHILD)            != 0) DumpStyleEx("WS_EX_MDICHILD",            WS_EX_MDICHILD.ToString("X8"));
                if ((i & WS_EX_NOACTIVATE)          != 0) DumpStyleEx("WS_EX_NOACTIVATE",          WS_EX_NOACTIVATE.ToString("X8"));
                if ((i & WS_EX_NOINHERITLAYOUT)     != 0) DumpStyleEx("WS_EX_NOINHERITLAYOUT",     WS_EX_NOINHERITLAYOUT.ToString("X8"));
                if ((i & WS_EX_NOPARENTNOTIFY)      != 0) DumpStyleEx("WS_EX_NOPARENTNOTIFY",      WS_EX_NOPARENTNOTIFY.ToString("X8"));
                if ((i & WS_EX_NOREDIRECTIONBITMAP) != 0) DumpStyleEx("WS_EX_NOREDIRECTIONBITMAP", WS_EX_NOREDIRECTIONBITMAP.ToString("X8"));
//              if ((n & WS_EX_OVERLAPPEDWINDOW)    != 0) DumpStyleEx("WS_EX_OVERLAPPEDWINDOW",    WS_EX_OVERLAPPEDWINDOW.ToString("X8"));
//              if ((n & WS_EX_PALETTEWINDOW)       != 0) DumpStyleEx("WS_EX_PALETTEWINDOW",       WS_EX_PALETTEWINDOW.ToString("X8"));
                if ((i & WS_EX_RIGHT)               != 0) DumpStyleEx("WS_EX_RIGHT",               WS_EX_RIGHT.ToString("X8"));
                if ((i & WS_EX_RIGHTSCROLLBAR)      != 0) DumpStyleEx("WS_EX_RIGHTSCROLLBAR",      WS_EX_RIGHTSCROLLBAR.ToString("X8"));
                if ((i & WS_EX_RTLREADING)          != 0) DumpStyleEx("WS_EX_RTLREADING",          WS_EX_RTLREADING.ToString("X8"));
                if ((i & WS_EX_STATICEDGE)          != 0) DumpStyleEx("WS_EX_STATICEDGE",          WS_EX_STATICEDGE.ToString("X8"));
                if ((i & WS_EX_TOOLWINDOW)          != 0) DumpStyleEx("WS_EX_TOOLWINDOW",          WS_EX_TOOLWINDOW.ToString("X8"));
                if ((i & WS_EX_TOPMOST)             != 0) DumpStyleEx("WS_EX_TOPMOST",             WS_EX_TOPMOST.ToString("X8"));
                if ((i & WS_EX_TRANSPARENT)         != 0) DumpStyleEx("WS_EX_TRANSPARENT",         WS_EX_TRANSPARENT.ToString("X8"));
                if ((i & WS_EX_WINDOWEDGE)          != 0) DumpStyleEx("WS_EX_WINDOWEDGE",          WS_EX_WINDOWEDGE.ToString("X8"));
            }

            return hPrefix + i.ToString(hFormat);
        }

        #endregion
        #region Class

        // There are rumers that <GetClassLong> can cause software to crash on x64
        // and that's why I am using <GetClassLongPtr> instead
        private static IntPtr GetClassLongPtr(IntPtr hWnd, Win32.ClassLongIndex nIndex)
        {
            return IntPtr.Size > 4 ? Win32.GetClassLongPtr64(hWnd, nIndex) : 
                                     new IntPtr(Win32.GetClassLongPtr32(hWnd, nIndex));
        }

        private string GetClassName(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            Win32.GetClassName(hWnd, sb, 256);

            var value = sb.ToString();
            // Output identifiers for the cute little classes below
            switch (value)
            {
                case "#32768": return value + " (Menu)";
                case "#32769": return value + " (Desktop window)";
                case "#32770": return value + " (Dialog box)";
                case "#32771": return value + " (Task-switch window)";
                case "#32772": return value + " (Icon title)";
            }

            return value;
        }

        private string GetClassStyles(IntPtr hWnd)
        {
            var value = (int)GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCL_STYLE);

            // Add the available class styles to the combo box
            CMB_ClassStyles.Items.Clear();
            CMB_ClassStyles.Enabled = value != 0;

            if (value != 0)
            {
                foreach (var style in Enum.GetValues(typeof(Win32.ClassStyles)))
                {
                    if ((value & (int)style) != 0)
                        CMB_ClassStyles.Items.Add(style.ToString());
                }

                // Select the first index, if any
                if (CMB_ClassStyles.Items.Count != 0)
                    CMB_ClassStyles.SelectedIndex = 0;
            }

            return hPrefix + value.ToString(hFormat);
        }

        private string GetClassBytes(IntPtr hWnd)
        {
            var value = (long)GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCL_CBCLSEXTRA);
            var i = 0;
            
            CMB_ClassBytes.Items.Clear();
            CMB_ClassBytes.Enabled = value != 0;

            while (value != 0)
            {
                if (value >= 4)
                {
                    var bytes = GetClassLongPtr(hWnd, (Win32.ClassLongIndex)i).ToString(hFormat);
                    CMB_ClassBytes.Items.Add("+" + i + "       " + hPrefix + bytes);
                }
                else
                    CMB_ClassBytes.Items.Add("+" + i + "       " + "(Unavailable)");

                i += 4;
                value = Math.Max(value - 4, 0);
            } 
            
            if (CMB_ClassBytes.Items.Count != 0)
                CMB_ClassBytes.SelectedIndex = 0;

            return value.ToString();
        }

        private string GetClassAtom(IntPtr hWnd)
        {
            return hPrefix + GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCW_ATOM).ToString("X4");
        }

        private string GetWindowBytes(IntPtr hWnd)
        {
            return GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCL_CBWNDEXTRA).ToString();
        }

        private string GetIconHandle(IntPtr hWnd)
        {
            var value = hPrefix + GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCL_HICON).ToString(hFormat);
            return value == "00000000" ? "(none)" : value;
        }

        private string GetIconHandleSM(IntPtr hWnd)
        {
            var value = hPrefix + Win32.GetClassLongPtr32(hWnd, Win32.ClassLongIndex.GCL_HICONSM).ToString(hFormat);
            return value == "00000000" ? "(none)" : value;
        }

        private string GetCursorHandle(IntPtr hWnd)
        {
            var value = GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCL_HCURSOR).ToString("X");

            if (Environment.OSVersion.Version.Major < 6)
            {
                // Hex handles for Windows XP and below
                switch (value)
                {
                    case "0"    : return "(none)";
                    case "10011": return hPrefix + value + " (IDC_ARROW)";
                    case "10013": return hPrefix + value + " (IDC_IBEAM)";
                    case "10015": return hPrefix + value + " (IDC_WAIT)";
                    case "10017": return hPrefix + value + " (IDC_CROSS)";
                    case "10019": return hPrefix + value + " (IDC_UPARROW)";
                    case "1001B": return hPrefix + value + " (IDC_SIZENWSE)";
                    case "1001D": return hPrefix + value + " (IDC_SIZENESW)";
                    case "1001F": return hPrefix + value + " (IDC_SIZEWE)";
                    case "10021": return hPrefix + value + " (IDC_SIZENS)";
                    case "10023": return hPrefix + value + " (IDC_SIZEALL)";
                    case "10025": return hPrefix + value + " (IDC_NO)";
                    case "10027": return hPrefix + value + " (IDC_APPSTARTING)";
                    case "10029": return hPrefix + value + " (IDC_HELP)";
                }
            }
            else if (Environment.OSVersion.Version.Major >= 6)
            {
                // Hex handles for Windows Vista and above
                switch (value)
                {
                    case "0"    : return "(none)";
                    case "10003": return hPrefix + value + " (IDC_ARROW)";
                    case "10005": return hPrefix + value + " (IDC_IBEAM)";
                    case "10007": return hPrefix + value + " (IDC_WAIT)";
                    case "10009": return hPrefix + value + " (IDC_CROSS)";
                    case "1000B": return hPrefix + value + " (IDC_UPARROW)";
                    case "1000D": return hPrefix + value + " (IDC_SIZENWSE)";
                    case "1000F": return hPrefix + value + " (IDC_SIZENESW)";
                    case "10011": return hPrefix + value + " (IDC_SIZEWE)";
                    case "10013": return hPrefix + value + " (IDC_SIZENS)";
                    case "10015": return hPrefix + value + " (IDC_SIZEALL)";
                    case "10017": return hPrefix + value + " (IDC_NO)";
                    case "10019": return hPrefix + value + " (IDC_APPSTARTING)";
                    case "1001B": return hPrefix + value + " (IDC_HELP)";
                }
            }

            return value;
        }

        private string GetBackgroundBrush(IntPtr hWnd)
        {
            var value = GetClassLongPtr(hWnd, Win32.ClassLongIndex.GCL_HBRBACKGROUND).ToString();
            int n;

            /* Apparently, the return value of <0> is shared between <hBrush.None>
            ** and <hBrush.COLOR_SCROLLBAR>.
            ** 
            ** The only way to distinguish between the two is by treating <0> as a
            ** value for <hBrush.None> and <1> as a value for <hBrush.COLOR_SCROLLBAR>.
            ** Then subtract 1 from each return value afterwards to show a correct
            ** output in the TextBox.
            */
            switch (value)
            {
                case "0" : n = 0;  return "(none)";
                case "1" : n = 1;  return n - 1 + " (COLOR_SCROLLBAR)";
                case "2" : n = 2;  return n - 1 + " (COLOR_BACKGROUND)";
                case "3" : n = 3;  return n - 1 + " (COLOR_ACTIVECAPTION)";
                case "4" : n = 4;  return n - 1 + " (COLOR_INACTIVECAPTION)";
                case "5" : n = 5;  return n - 1 + " (COLOR_MENU)";
                case "6" : n = 6;  return n - 1 + " (COLOR_WINDOW)";
                case "7" : n = 7;  return n - 1 + " (COLOR_WINDOWFRAME)";
                case "8" : n = 8;  return n - 1 + " (COLOR_MENUTEXT)";
                case "9" : n = 9;  return n - 1 + " (COLOR_WINDOWTEXT)";
                case "10": n = 10; return n - 1 + " (COLOR_CAPTIONTEXT)";
                case "11": n = 11; return n - 1 + " (COLOR_ACTIVEBORDER)";
                case "12": n = 12; return n - 1 + " (COLOR_INACTIVEBORDER)";
                case "13": n = 13; return n - 1 + " (COLOR_APPWORKSPACE)";
                case "14": n = 14; return n - 1 + " (COLOR_HIGHLIGHT)";
                case "15": n = 15; return n - 1 + " (COLOR_HIGHLIGHTTEXT)";
                case "16": n = 16; return n - 1 + " (COLOR_BTNFACE)";
                case "17": n = 17; return n - 1 + " (COLOR_BTNSHADOW)";
                case "18": n = 18; return n - 1 + " (COLOR_GRAYTEXT)";
                case "19": n = 19; return n - 1 + " (COLOR_BTNTEXT)";
                case "20": n = 20; return n - 1 + " (COLOR_INACTIVECAPTIONTEXT)";
                case "21": n = 21; return n - 1 + " (COLOR_BTNHIGHLIGHT)";
                case "22": n = 22; return n - 1 + " (COLOR_3DDKSHADOW)";
                case "23": n = 23; return n - 1 + " (COLOR_3DLIGHT)";
                case "24": n = 24; return n - 1 + " (COLOR_INFOTEXT)";
                case "25": n = 25; return n - 1 + " (COLOR_INFOBK)";
                case "26": n = 26; return n - 1 + " (COLOR_HOTLIGHT)";
                case "27": n = 27; return n - 1 + " (COLOR_GRADIENTACTIVECAPTION)";
                case "28": n = 28; return n - 1 + " (COLOR_GRADIENTINACTIVECAPTION)";
                case "29": n = 29; return n - 1 + " (COLOR_MENUHILIGHT)";
                case "30": n = 30; return n - 1 + " (COLOR_MENUBAR)";
                case "31": n = 31; return n - 1 + " (COLOR_FORM)";
                default:
                    // GetClassLongPtr() sometimes reutrns "FFFFFFFF" before the actual value
                    value = hPrefix + Win32.GetClassLongPtr32(hWnd, Win32.ClassLongIndex.GCL_HBRBACKGROUND).ToString(hFormat);
                    break;
            }

//          if (value.StartsWith("FFFFFFFF"))
//              value = value.Substring(8);
            
            return value;
        }

        #endregion
        #region Process

        private string GetModuleName(IntPtr hWnd)
        {
            var pid = 0;
            Win32.GetWindowThreadProcessId(hWnd, ref pid);
            var process = Process.GetProcessById(pid);

            return process.MainModule.ModuleName;
        }

        private string GetModulePath(IntPtr hWnd)
        {
            var pid = 0;
            Win32.GetWindowThreadProcessId(hWnd, ref pid);
            var process = Process.GetProcessById(pid);

            return process.MainModule.FileName;
        }

        private string GetProcessIdEx(IntPtr hWnd)
        {
            var pid = 0;
            Win32.GetWindowThreadProcessId(hWnd, ref pid);
            var process = Process.GetProcessById(pid);
            targetPID = pid;

            return hPrefix + process.Id.ToString(hFormat) + " (" + process.Id + ")";
        }

        private int GetProcessId(IntPtr hWnd)
        {
            var pid = 0;
            Win32.GetWindowThreadProcessId(hWnd, ref pid);
            var process = Process.GetProcessById(pid);

            return process.Id;
        }

        private string GetThreadId(IntPtr hWnd)
        {
            var pid = 0;
            return hPrefix + Win32.GetWindowThreadProcessId(hWnd, ref pid).ToString(hFormat) +
                      " (" + Win32.GetWindowThreadProcessId(hWnd, ref pid) + ")";
        }

        private string GetPriorityClass(IntPtr hWnd)
        {
            switch (Win32.GetPriorityClass(hWnd))
            {
                case (uint)Win32.ProcessPriorityClass.NORMAL_PRIORITY_CLASS:
                    return "NORMAL_PRIORITY_CLASS (8)";
                case (uint)Win32.ProcessPriorityClass.IDLE_PRIORITY_CLASS:
                    return "IDLE_PRIORITY_CLASS (4)";
                case (uint)Win32.ProcessPriorityClass.HIGH_PRIORITY_CLASS:
                    return "HIGH_PRIORITY_CLASS (13)";
                case (uint)Win32.ProcessPriorityClass.REALTIME_PRIORITY_CLASS:
                    return "REALTIME_PRIORITY_CLASS (23)";
                case (uint)Win32.ProcessPriorityClass.BELOW_NORMAL_PRIORITY_CLASS:
                    return "BELOW_NORMAL_PRIORITY_CLASS (6)";
                case (uint)Win32.ProcessPriorityClass.ABOVE_NORMAL_PRIORITY_CLASS:
                    return "ABOVE_NORMAL_PRIORITY_CLASS (10)";
            }

            return "(unknown)";
        }

        #endregion

        #endregion
        #region Methods

        #region Read/Write Settings

        /// <summary>
        /// Loads up the program settings from the settings file.
        /// </summary>
        private void ReadSettings()
        {
            if (!File.Exists(IniFile.SettingsPath()))
                return;

            CMNU_RememberWinPos.Checked = IniFile.ReadINI(IniFile.SettingsPath(), "WindowPos", "RememberPos", true);

            if (IniFile.ReadINI(IniFile.SettingsPath(), "WindowPos", "RememberPos", true))
            {
                if (IniFile.ReadINI(IniFile.SettingsPath(), "WindowPos", "FirstRun", true))
                    CenterToScreen();
                else
                {

                    var winPos = new Point(IniFile.ReadINI(IniFile.SettingsPath(), "WindowPos", "PosX", 0),
                                           IniFile.ReadINI(IniFile.SettingsPath(), "WindowPos", "PosY", 0));
                    Location = winPos;
                }
            }

            CMNU_StayOnTop.Checked         = IniFile.ReadINI(IniFile.SettingsPath(), "Main",    "TopMost" ,          false);
            CMNU_EasyMove.Checked          = IniFile.ReadINI(IniFile.SettingsPath(), "Main",    "EasyMove",          true);
            CMNU_Collapse.Checked          = IniFile.ReadINI(IniFile.SettingsPath(), "Main",    "Collapse",          true);
            CMNU_NativeHighlighter.Checked = IniFile.ReadINI(IniFile.SettingsPath(), "Main",    "NativeHighlighter", true);

            CMNU_Default.Checked           = IniFile.ReadINI(IniFile.SettingsPath(), "HexMode", "Default",           true);
            CMNU_VisualCPP.Checked         = IniFile.ReadINI(IniFile.SettingsPath(), "HexMode", "VisualCPP",         false);
            CMNU_VisualBasic.Checked       = IniFile.ReadINI(IniFile.SettingsPath(), "HexMode", "VisualBasic",       false);
        }

        /// <summary>
        /// Writes the program settings to the settings file.
        /// </summary>
        private void SaveSettings()
        {
            if (IniFile.ReadINI(IniFile.SettingsPath(), "WindowPos", "FirstRun", true))
                IniFile.WriteINI(IniFile.SettingsPath(), "WindowPos", "FirstRun", false);

            IniFile.WriteINI(IniFile.SettingsPath(), "WindowPos", "RememberPos", CMNU_RememberWinPos.Checked);

            // Prevent writing a negative value
            if (WindowState != FormWindowState.Minimized)
            {
                IniFile.WriteINI(IniFile.SettingsPath(), "WindowPos", "PosX", Location.X);
                IniFile.WriteINI(IniFile.SettingsPath(), "WindowPos", "PosY", Location.Y);
            }

            IniFile.WriteINI(IniFile.SettingsPath(), "Main",    "TopMost" ,          CMNU_StayOnTop.Checked);
            IniFile.WriteINI(IniFile.SettingsPath(), "Main",    "EasyMove",          CMNU_EasyMove.Checked);
            IniFile.WriteINI(IniFile.SettingsPath(), "Main",    "Collapse",          CMNU_Collapse.Checked);
            IniFile.WriteINI(IniFile.SettingsPath(), "Main",    "NativeHighlighter", CMNU_NativeHighlighter.Checked);

            IniFile.WriteINI(IniFile.SettingsPath(), "HexMode", "Default",           CMNU_Default.Checked);
            IniFile.WriteINI(IniFile.SettingsPath(), "HexMode", "VisualCPP",         CMNU_VisualCPP.Checked);
            IniFile.WriteINI(IniFile.SettingsPath(), "HexMode", "VisualBasic",       CMNU_VisualBasic.Checked);
        }

        #endregion
        #region DPI Related

        /// <summary>
        /// Sets the sizing and location of form and controls based on whether
        /// the monitor DPI is 96 (100% scaling) or 120 (125% scaling).
        /// </summary>
        private void SetSizeBasedOnDpi()
        {
            if (IsDpi96())
            {
                formHeightCollapsed = 99;
                formHeightExtended  = 442;

                PB_Tool.Location   = new Point(10, 18);
                LBL_HowTo.Location = new Point(51, 19);
                LBL_HowTo.Size     = new Size(267, 29);

                LV_WindowStyles.Columns[0].Width   = 215;
                LV_ExtendedStyles.Columns[0].Width = 215;
            }
            else
            {
                formHeightCollapsed = 123;
                formHeightExtended  = 510;

                PB_Tool.Location   = new Point(13, 24);
                LBL_HowTo.Location = new Point(54, 22);
                LBL_HowTo.Size     = new Size(373, 36);

                LV_WindowStyles.Columns[0].Width   = 300;
                LV_ExtendedStyles.Columns[0].Width = 300;
            }

            Height = formHeightCollapsed;
        }

        #endregion
        #region System Menu

        private static void ShowAboutDialog()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var version = Application.ProductVersion;
            var buildDate = new FileInfo(entryAssembly.Location).LastWriteTime;
            var author = Application.CompanyName;
            var info = "Version: " + version
                                   + "\nBuild Date: " + buildDate
                                   + "\n\nAuthor: "   + author
                                   + "\nPage: http://github.com/ei/SharpFind"
                                   + "\n\nThis open-source project is licensed under the MIT license.";

            MessageBox.Show(info, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void ShowChangelog()
        {
            var path = Application.StartupPath + "\\Changelog.txt";
            if (File.Exists(path))
                Process.Start(path);
            else
                MessageBox.Show("The following file was not found:\n" + path, "Not Found");
        }

        private static void ShowLicense()
        {
            var path = Application.StartupPath + "\\License.txt";
            if (File.Exists(path))
                Process.Start(path);
            else
                MessageBox.Show("The following file was not found:\n" + path, "Not Found");
        }

        /// <summary>
        /// Closes and restarts the program as administrator if it's not already
        /// running as one.
        /// </summary>
        private static void RunAsAdministrator()
        {
            if (IsRunningAsAdmin())
                return;

            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.CurrentDirectory,
                FileName = Application.ExecutablePath,
                Verb = "runas"
            };

            try   { Process.Start(psi); }
            catch { return; }
            Application.Exit();
        }

        #endregion
        #region Finder Tool

        private void CaptureMouse(bool captured)
        {
            if (captured)
            {
                Win32.SetCapture(Handle);

                Cursor.Current = _cursorFinder;
                PB_Tool.Image = Resources.finder_out;

                if (CMNU_Collapse.Checked)
                {
                    PNL_Bottom.Visible = false;
                    Height = formHeightCollapsed;
                }
            }
            else
            {
                Win32.ReleaseCapture();

                Cursor.Current = _cursorDefault;
                PB_Tool.Image = Resources.finder_in;

                if (hWnd != IntPtr.Zero && isHandleNull == false)
                {
                    PNL_Bottom.Visible = true;
                    RetrieveWindowInfo(hWnd);
                    Height = formHeightExtended;
                }

                if (hPreviousWindow != IntPtr.Zero)
                {
                    WindowHighlighter.Refresh(hPreviousWindow);
                    hPreviousWindow = IntPtr.Zero;
                }
            }

            isCapturing = captured;
        }

        private void HandleMouseMovement()
        {
            if (!isCapturing) return;
            try
            {
                hWnd = Win32.WindowFromPoint(Cursor.Position);

                // Prevent retrieving information about the program itself, just like Spy++
                var pid = GetProcessId(hWnd);
                if (Win32.GetCurrentProcessId() == pid)
                {
                    isHandleNull = true;
                    Text = appName;
                    return;
                }

                isHandleNull = false;

                if (hPreviousWindow != IntPtr.Zero && hPreviousWindow != hWnd)
                    WindowHighlighter.Refresh(hPreviousWindow);

                if (hWnd == IntPtr.Zero)
                    return;

                hPreviousWindow = hWnd;
                Text = appName + " - " + hPrefix + hWnd.ToString(hFormat);

                // The flickering shall not pass
                if (hWndOld == hWnd)
                    return;

                WindowHighlighter.Highlight(hWnd, CMNU_NativeHighlighter.Checked);
                hWndOld = hWnd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Cursor.Current = _cursorDefault;
                PB_Tool.Image = Resources.finder_in;
            }
        }

        private void RetrieveWindowInfo(IntPtr hWnd)
        {
            // General Information tab
            TB_WindowCaption.Text  = GetWindowText(hWnd);
            TB_WindowHandle.Text   = hPrefix + hWnd.ToString(hFormat) + " (" + hWnd.ToInt32() + ")";
            TB_Class.Text          = GetWindowClass(hWnd);
            TB_Style.Text          = GetWindowStyles(hWnd);
            TB_Rectangle.Text      = GetWindowRect(hWnd);
            TB_RestoredRect.Text   = GetRestoredRect(hWnd);
            TB_ClientRect.Text     = GetClientRect(hWnd);
            TB_InstanceHandle.Text = GetInstanceHandle(hWnd);
            TB_ControlID.Text      = GetControlId(hWnd);
            TB_UserData.Text       = GetUserData(hWnd);
            GetWindowBytesCombo(hWnd);

            //Styles tab
            TB_WindowStyles.Text   = TB_Style.Text.Split('(')[0].TrimEnd();
            TB_ExtendedStyles.Text = GetWindowStylesEx(hWnd);

            // Class tab
            TB_ClassName.Text      = GetClassName(hWnd);
            TB_ClassStyles.Text    = GetClassStyles(hWnd);
            TB_ClassBytes.Text     = GetClassBytes(hWnd);
            TB_ClassAtom.Text      = GetClassAtom(hWnd);
            TB_WindowBytes.Text    = GetWindowBytes(hWnd);
            TB_IconHandle.Text     = GetIconHandle(hWnd);
            TB_IconHandleSM.Text   = GetIconHandleSM(hWnd);
            TB_CursorHandle.Text   = GetCursorHandle(hWnd);
            TB_BkgndBrush.Text     = GetBackgroundBrush(hWnd);

            // Process tab
            TB_ModuleName.Text     = GetModuleName(hWnd);
            TB_ModulePath.Text     = GetModulePath(hWnd);
            TB_ProcessID.Text      = GetProcessIdEx(hWnd);
            TB_ThreadID.Text       = GetThreadId(hWnd);
            TB_PriorityClass.Text  = GetPriorityClass(Process.GetProcessById(targetPID).Handle);
        }

        #endregion

        #endregion
        #region Events

        #region Form

        private void Frm_Main_Load(object sender, EventArgs e)
        {
            PNL_Bottom.Visible = false;

            // Add Window Menu items
            var handle = Win32.GetSystemMenu(Handle, false);
            Win32.InsertMenu(handle, 05, Win32.InsertMenuFlags.MF_BYPOSITION | 
                                         Win32.InsertMenuFlags.MF_SEPARATOR, 0, null);

            Win32.InsertMenu(handle, 06, Win32.InsertMenuFlags.MF_BYPOSITION | 
                                         Win32.InsertMenuFlags.MF_POPUP, 
                                         (uint)CMENU_Configuration.Handle, "Configuration");

            Win32.InsertMenu(handle, 07, Win32.InsertMenuFlags.MF_BYPOSITION | 
                                         Win32.InsertMenuFlags.MF_SEPARATOR, 0, null);

            Win32.InsertMenu(handle, 08, Win32.InsertMenuFlags.MF_BYPOSITION,  MNU_ABOUT,     "About...\tF1");
            Win32.InsertMenu(handle, 09, Win32.InsertMenuFlags.MF_BYPOSITION,  MNU_CHANGELOG, "Changelog...");
            Win32.InsertMenu(handle, 10, Win32.InsertMenuFlags.MF_BYPOSITION,  MNU_LICENSE,   "License...");

            // Vista and up
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (!IsRunningAsAdmin())
                {
                    Win32.InsertMenu(handle, 11, Win32.InsertMenuFlags.MF_BYPOSITION | 
                                                 Win32.InsertMenuFlags.MF_SEPARATOR, 0, null);

                    Win32.InsertMenu(handle, 12, Win32.InsertMenuFlags.MF_BYPOSITION,  MNU_ADMIN, "Run as Administrator...\tF2");
                }
            }


            ReadSettings();

            if (CMNU_StayOnTop.Checked)
                TopMost = true;

            if (CMNU_Default.Checked)
            {
                hPrefix = string.Empty;
                hFormat = "X8";
            }
            else if (CMNU_VisualCPP.Checked)
            {
                hPrefix = "0x";
                hFormat = "X";
            }
            else if (CMNU_VisualBasic.Checked)
            {
                hPrefix = "&H";
                hFormat = "X";
            }
        }

        private void Frm_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void Frm_Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.T && e.Modifiers == Keys.Control)
                CMNU_StayOnTop.PerformClick();

            if (e.KeyCode == Keys.E && e.Modifiers == Keys.Control)
                CMNU_EasyMove.PerformClick();

            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                CMNU_Collapse.PerformClick();

            if (e.KeyCode == Keys.N && e.Modifiers == Keys.Control)
                CMNU_NativeHighlighter.PerformClick();

            if (e.KeyCode == Keys.F1)
                ShowAboutDialog();

            if (e.KeyCode == Keys.F2)
                RunAsAdministrator();
        }

        private void EasyMove(object sender, MouseEventArgs e)
        {
            if (!CMNU_EasyMove.Checked)
                return;

            if (e.Button == MouseButtons.Left)
            {
                Win32.ReleaseCapture();
                Win32.SendMessage(Handle, (int)Win32.WindowsMessages.WM_NCLBUTTONDOWN, 0x2 /* HTCAPTION */, 0);
            }
        }

        #endregion
        #region Controls

        private void PB_Tool_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                CaptureMouse(true);
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LNKLBL_ModuleInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Make sure the process is still running to prevent an exception
            var p = Path.GetFileNameWithoutExtension(TB_ModuleName.Text);
            if (Process.GetProcessesByName(p).Length > 0)
            {
                var instance = new Frm_ModuleInfo();
                instance.LBL_Process_R.Text = TB_ModuleName.Text;
                instance.ShowDialog();
            }
            else
            {
                MessageBox.Show($"The process '{p}.exe' is no longer running!", 
                                Application.ProductName, 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Warning);
            }
        }

        #endregion
        #region System Menu

        private void CMNU_RememberWinPos_Click(object sender, EventArgs e)
        {
            CMNU_RememberWinPos.Checked = !CMNU_RememberWinPos.Checked;
        }

        private void CMNU_StayOnTop_Click(object sender, EventArgs e)
        {
            if (!CMNU_StayOnTop.Checked)
            {
                CMNU_StayOnTop.Checked = true;
                TopMost = true;
            }
            else
            {
                CMNU_StayOnTop.Checked = false;
                TopMost = false;
            }
        }

        private void CMNU_EasyMove_Click(object sender, EventArgs e)
        {
            CMNU_EasyMove.Checked = !CMNU_EasyMove.Checked;
        }

        private void CMNU_Collapse_Click(object sender, EventArgs e)
        {
            CMNU_Collapse.Checked = !CMNU_Collapse.Checked;
        }

        private void CMNU_NativeHighlighter_Click(object sender, EventArgs e)
        {
            CMNU_NativeHighlighter.Checked = !CMNU_NativeHighlighter.Checked;
        }

        private void CMNU_Default_Click(object sender, EventArgs e)
        {
            if (!CMNU_Default.Checked)
            {
                CMNU_Default.Checked = true;
                CMNU_VisualCPP.Checked = false;
                CMNU_VisualBasic.Checked = false;

                hPrefix = string.Empty;
                hFormat = "X8";
            }
        }

        private void CMNU_VisualCPPHex_Click(object sender, EventArgs e)
        {
            if (!CMNU_VisualCPP.Checked)
            {
                CMNU_Default.Checked = false;
                CMNU_VisualCPP.Checked = true;
                CMNU_VisualBasic.Checked = false;

                hPrefix = "0x";
                hFormat = "X";
            }
        }

        private void CMNU_VisualBasicHex_Click(object sender, EventArgs e)
        {
            if (!CMNU_VisualBasic.Checked)
            {
                CMNU_Default.Checked = false;
                CMNU_VisualCPP.Checked = false;
                CMNU_VisualBasic.Checked = true;

                hPrefix = "&H";
                hFormat = "X";
            }
        }

        #endregion

        #endregion
    }
}