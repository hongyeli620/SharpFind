﻿using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System;

using SharpFind.Classes;
using SharpFind.Properties;

// <using static> is a C#6 feature. See:
// https://blogs.msdn.microsoft.com/csharpfaq/2014/11/20/new-features-in-c-6/
// C#6 IDE support starts at Visual Studio 2013 and up
using static SharpFind.Classes.NativeMethods;

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

            ControlBox = true;
            MinimizeBox = true;
            MaximizeBox = false;
            ShowIcon = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            if (IsDpi96())
            {
                formHeightCollapsed = 99;
                formHeightExtended = 413;

                PB_Tool.Location = new Point(10, 18);
                LBL_HowTo.Location = new Point(53, 19);
                LBL_HowTo.Size = new Size(267, 29);

                LV_WindowStyles.Columns[0].Width = 215;
                LV_ExtendedStyles.Columns[0].Width = 215;
            }
            else
            {
                formHeightCollapsed = 123;
                formHeightExtended = 510;

                PB_Tool.Location = new Point(13, 24);
                LBL_HowTo.Location = new Point(54, 22);
                LBL_HowTo.Size = new Size(373, 36);

                LV_WindowStyles.Columns[0].Width = 300;
                LV_ExtendedStyles.Columns[0].Width = 300;
            }

            Height = formHeightCollapsed;

            using (var ms = new MemoryStream(Resources.finder))
            {
                _cursorDefault = Cursor.Current;
                _cursorFinder = new Cursor(ms);
            }
        }

        #region Variables

        private string appName;

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
        
        // Single window menu items
        private const int MNU_ABOUT = 1000;
        private const int MNU_LICENSE = 1001;
        private const int MNU_CHANGELOG = 1002;

        #endregion
        #region DPI Check

        public static Point GetSystemDpi()
        {
            var result = new Point();
            var hDC = GetDC(IntPtr.Zero);

            result.X = GetDeviceCaps(hDC, 88);
            result.Y = GetDeviceCaps(hDC, 90);

            ReleaseDC(IntPtr.Zero, hDC);

            return result;
        }

        public static bool IsDpi96()
        {
            var result = GetSystemDpi();
            return result.X == 96 || result.Y == 96;
        }

        #endregion
        #region Functions

        private static bool IsRunningAsAdmin()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        #region General

        private static bool isWindowEnabled(IntPtr hWnd)
        {
            return IsWindowEnabled(hWnd);
        }

        private static bool isWindowUnicode(IntPtr hWnd)
        {
            return IsWindowUnicode(hWnd);
        }

        private static bool isWindowVisible(IntPtr hWnd)
        {
            return IsWindowVisible(hWnd);
        }

        private static string getWindowCaption(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, 256);
            return sb.ToString();
        }

        private static string getClass(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            GetClassName(hWnd, sb, 256);
        
            var value = sb.ToString();
            if (isWindowUnicode(hWnd))
                value = value + " (unicode)";

            return value;
        }

        private static string getWindowRect(IntPtr hWnd)
        {
            var wRect = new RECT();
            GetWindowRect(hWnd, out wRect);
            var winState = IsZoomed(hWnd) ? " (maximized)" : string.Empty;

            return string.Format("({2},{3}) - ({4},{5}), {0} x {1}{6}", wRect.right - wRect.left,
                                                                        wRect.bottom - wRect.top,
                                                                        wRect.left,
                                                                        wRect.top,
                                                                        wRect.right,
                                                                        wRect.bottom,
                                                                        winState);
        }

        private static string getClientRect(IntPtr hWnd)
        {
            var cRect = new RECT();
            GetClientRect(hWnd, out cRect);
            return string.Format("({2},{3}) - ({4},{5}), {0} x {1}", cRect.right - cRect.left,
                                                                     cRect.bottom - cRect.top,
                                                                     cRect.left,
                                                                     cRect.top,
                                                                     cRect.right,
                                                                     cRect.bottom);
        }

        private static string getInstanceHandle(IntPtr hWnd)
        {
            return GetWindowLong(hWnd, GWL_HINSTANCE).ToString("X8");
        }

        private static string getControlID(IntPtr hWnd)
        {
            return GetWindowLong(hWnd, GWL_ID).ToString("X8");
        }

        private static string getUserData(IntPtr hWnd)
        {
            return GetWindowLong(hWnd, GWL_USERDATA).ToString("X8");
        }

        private void getWindowBytesCombo(IntPtr hWnd)
        {
            var value = (long)GetClassLongPtr(hWnd, GCL_CBWNDEXTRA);
            var i = 0;

            CMB_WindowBytes.Items.Clear();
            CMB_WindowBytes.Enabled = value != 0;

            while (value != 0)
            {
                if (value >= 4)
                    // <GetWindowLongPtr> is used here, otherwise it won't work right
                    // Dealing with x68/x64 compatibility is really a pain in the ass
                    CMB_WindowBytes.Items.Add("+" + i + "       " + GetWindowLongPtr(hWnd, i).ToString("X8"));             
                else
                    CMB_WindowBytes.Items.Add("+" + i + "       " + "(Unavailable)");

                i += 4; // 0, 4, 8, 12, 16, etc
                value = Math.Max(value -4, 0);
            }
            // Select the first index, if any
            if (CMB_WindowBytes.Items.Count != 0) CMB_WindowBytes.SelectedIndex = 0;
        }

        #endregion
        #region Styles

        private void DumpStyle(string style, string styleValue)
        {
            ListViewItem item;
            item = LV_WindowStyles.Items.Add(style);
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(styleValue).ForeColor =  SystemColors.GrayText;
            item.SubItems[1].Font = new Font("Lucida Sans Typewriter", 8F, FontStyle.Regular);
        }

        private void DumpStyleEx(string style, string styleValue)
        {
            ListViewItem item;
            item = LV_ExtendedStyles.Items.Add(style);
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(styleValue).ForeColor = SystemColors.GrayText;
            item.SubItems[1].Font = new Font("Lucida Sans Typewriter", 8F, FontStyle.Regular);
        }

        private string getStyle(IntPtr hWnd)
        {
            var i = GetWindowLongPtr(hWnd, GWL_STYLE);
            LV_WindowStyles.Items.Clear();

            if (i != 0)
            {
                if ((i & WindowStyles.WS_BORDER) != 0)
                    DumpStyle("WS_BORDER", WindowStyles.WS_BORDER.ToString("X8"));
                if ((i & WindowStyles.WS_CAPTION) == WindowStyles.WS_CAPTION)
                {
                    DumpStyle("WS_CAPTION", WindowStyles.WS_CAPTION.ToString("X8"));
                    if ((i & WindowStyles.WS_SYSMENU) != 0)
                        DumpStyle("WS_SYSMENU", WindowStyles.WS_SYSMENU.ToString("X8"));
                }
                if ((i & WindowStyles.WS_CHILD) != 0)
                {
                    DumpStyle("WS_CHILD", WindowStyles.WS_CHILD.ToString("X8"));
                    if ((i & WindowStyles.WS_TABSTOP) != 0)
                        DumpStyle("WS_TABSTOP", WindowStyles.WS_TABSTOP.ToString("X8"));
                    if ((i & WindowStyles.WS_GROUP) != 0)
                        DumpStyle("WS_GROUP",   WindowStyles.WS_GROUP.ToString("X8"));
                }
                else
                {
                    if ((i & WindowStyles.WS_POPUP) != 0)
                        DumpStyle("WS_POPUP", WindowStyles.WS_POPUP.ToString("X8"));
                    if ((i & WindowStyles.WS_SYSMENU) != 0)
                    {
                        if ((i & WindowStyles.WS_MINIMIZEBOX) != 0)
                            DumpStyle("WS_MINIMIZEBOX", WindowStyles.WS_MINIMIZEBOX.ToString("X8"));
                        if ((i & WindowStyles.WS_MAXIMIZEBOX) != 0)
                            DumpStyle("WS_MAXIMIZEBOX", WindowStyles.WS_MAXIMIZEBOX.ToString("X8"));
                    }
                }
                if ((i & WindowStyles.WS_CLIPCHILDREN)     != 0) DumpStyle("WS_CLIPCHILDREN",     WindowStyles.WS_CLIPCHILDREN.ToString("X8"));
                if ((i & WindowStyles.WS_CLIPSIBLINGS)     != 0) DumpStyle("WS_CLIPSIBLINGS",     WindowStyles.WS_CLIPSIBLINGS.ToString("X8"));
                if ((i & WindowStyles.WS_DISABLED)         != 0) DumpStyle("WS_DISABLED",         WindowStyles.WS_DISABLED.ToString("X8"));
                if ((i & WindowStyles.WS_DLGFRAME)         != 0) DumpStyle("WS_DLGFRAME",         WindowStyles.WS_DLGFRAME.ToString("X8"));
                if ((i & WindowStyles.WS_HSCROLL)          != 0) DumpStyle("WS_HSCROLL",          WindowStyles.WS_HSCROLL.ToString("X8"));
                if ((i & WindowStyles.WS_MAXIMIZE)         != 0) DumpStyle("WS_MAXIMIZE",         WindowStyles.WS_MAXIMIZE.ToString("X8"));
                if ((i & WindowStyles.WS_MINIMIZE)         != 0) DumpStyle("WS_MINIMIZE",         WindowStyles.WS_MINIMIZE.ToString("X8"));
                if ((i & WindowStyles.WS_OVERLAPPED)       != 0) DumpStyle("WS_OVERLAPPED",       WindowStyles.WS_OVERLAPPED.ToString("X8"));
                if ((i & WindowStyles.WS_OVERLAPPEDWINDOW) != 0) DumpStyle("WS_OVERLAPPEDWINDOW", WindowStyles.WS_OVERLAPPEDWINDOW.ToString("X8"));
                if ((i & WindowStyles.WS_POPUPWINDOW)      != 0) DumpStyle("WS_POPUPWINDOW",      WindowStyles.WS_POPUPWINDOW.ToString("X8"));
                if ((i & WindowStyles.WS_THICKFRAME)       != 0) DumpStyle("WS_THICKFRAME",       WindowStyles.WS_THICKFRAME.ToString("X8"));
                if ((i & WindowStyles.WS_VISIBLE)          != 0) DumpStyle("WS_VISIBLE",          WindowStyles.WS_VISIBLE.ToString("X8"));
                if ((i & WindowStyles.WS_VSCROLL)          != 0) DumpStyle("WS_VSCROLL",          WindowStyles.WS_VSCROLL.ToString("X8"));

                if (TB_Class.Text.StartsWith("Button"))
                {
                    if ((i & ButtonControlStyles.BS_BITMAP)      != 0) DumpStyle("BS_BITMAP",          ButtonControlStyles.BS_BITMAP.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_FLAT)        != 0) DumpStyle("BS_FLAT",            ButtonControlStyles.BS_FLAT.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_ICON)        != 0) DumpStyle("BS_ICON",            ButtonControlStyles.BS_ICON.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_LEFTTEXT)    != 0) DumpStyle("BS_LEFTTEXT",        ButtonControlStyles.BS_LEFTTEXT.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_MULTILINE)   != 0) DumpStyle("BS_MULTILINE",       ButtonControlStyles.BS_MULTILINE.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_NOTIFY)      != 0) DumpStyle("BS_NOTIFY",          ButtonControlStyles.BS_NOTIFY.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_PUSHLIKE)    != 0) DumpStyle("BS_PUSHLIKE",        ButtonControlStyles.BS_PUSHLIKE.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_RIGHTBUTTON) != 0) DumpStyle("BS_RIGHTBUTTON",     ButtonControlStyles.BS_RIGHTBUTTON.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_TEXT)        != 0) DumpStyle("BS_TEXT",            ButtonControlStyles.BS_TEXT.ToString("X8"));

                    if ((i & 0xf) == ButtonControlStyles.BS_PUSHBUTTON)      DumpStyle("BS_PUSHBUTTON",      ButtonControlStyles.BS_PUSHBUTTON.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_DEFPUSHBUTTON)   DumpStyle("BS_DEFPUSHBUTTON",   ButtonControlStyles.BS_DEFPUSHBUTTON.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_CHECKBOX)        DumpStyle("BS_CHECKBOX",        ButtonControlStyles.BS_CHECKBOX.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_AUTOCHECKBOX)    DumpStyle("BS_AUTOCHECKBOX",    ButtonControlStyles.BS_AUTOCHECKBOX.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_RADIOBUTTON)     DumpStyle("BS_RADIOBUTTON",     ButtonControlStyles.BS_RADIOBUTTON.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_3STATE)          DumpStyle("BS_3STATE",          ButtonControlStyles.BS_3STATE.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_AUTO3STATE)      DumpStyle("BS_AUTO3STATE",      ButtonControlStyles.BS_AUTO3STATE.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_GROUPBOX)        DumpStyle("BS_GROUPBOX",        ButtonControlStyles.BS_GROUPBOX.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_USERBUTTON)      DumpStyle("BS_USERBUTTON",      ButtonControlStyles.BS_USERBUTTON.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_AUTORADIOBUTTON) DumpStyle("BS_AUTORADIOBUTTON", ButtonControlStyles.BS_AUTORADIOBUTTON.ToString("X8"));
                    if ((i & 0xf) == ButtonControlStyles.BS_OWNERDRAW)       DumpStyle("BS_OWNERDRAW",       ButtonControlStyles.BS_OWNERDRAW.ToString("X8"));

                    if ((i & ButtonControlStyles.BS_BOTTOM)  == ButtonControlStyles.BS_BOTTOM)  DumpStyle("BS_BOTTOM",  ButtonControlStyles.BS_RIGHT.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_CENTER)  == ButtonControlStyles.BS_CENTER)  DumpStyle("BS_CENTER",  ButtonControlStyles.BS_CENTER.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_LEFT)    == ButtonControlStyles.BS_LEFT)    DumpStyle("BS_LEFT",    ButtonControlStyles.BS_LEFT.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_RIGHT)   == ButtonControlStyles.BS_RIGHT)   DumpStyle("BS_RIGHT",   ButtonControlStyles.BS_RIGHT.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_TOP)     == ButtonControlStyles.BS_TOP)     DumpStyle("BS_TOP",     ButtonControlStyles.BS_LEFT.ToString("X8"));
                    if ((i & ButtonControlStyles.BS_VCENTER) == ButtonControlStyles.BS_VCENTER) DumpStyle("BS_VCENTER", ButtonControlStyles.BS_CENTER.ToString("X8"));
                }

                if (TB_Class.Text.StartsWith("ComboBox"))
                {
                    if ((i & ComboBoxStyles.CBS_AUTOHSCROLL)       != 0) DumpStyle("CBS_AUTOHSCROLL",       ComboBoxStyles.CBS_AUTOHSCROLL.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_DISABLENOSCROLL)   != 0) DumpStyle("CBS_DISABLENOSCROLL",   ComboBoxStyles.CBS_DISABLENOSCROLL.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_HASSTRINGS)        != 0) DumpStyle("CBS_HASSTRINGS",        ComboBoxStyles.CBS_HASSTRINGS.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_LOWERCASE)         != 0) DumpStyle("CBS_LOWERCASE",         ComboBoxStyles.CBS_LOWERCASE.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_NOINTEGRALHEIGHT)  != 0) DumpStyle("CBS_NOINTEGRALHEIGHT",  ComboBoxStyles.CBS_NOINTEGRALHEIGHT.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_OEMCONVERT)        != 0) DumpStyle("CBS_OEMCONVERT",        ComboBoxStyles.CBS_OEMCONVERT.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_OWNERDRAWFIXED)    != 0) DumpStyle("CBS_OWNERDRAWFIXED",    ComboBoxStyles.CBS_OWNERDRAWFIXED.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_OWNERDRAWVARIABLE) != 0) DumpStyle("CBS_OWNERDRAWVARIABLE", ComboBoxStyles.CBS_OWNERDRAWVARIABLE.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_SORT)              != 0) DumpStyle("CBS_SORT",              ComboBoxStyles.CBS_SORT.ToString("X8"));
                    if ((i & ComboBoxStyles.CBS_UPPERCASE)         != 0) DumpStyle("CBS_UPPERCASE",         ComboBoxStyles.CBS_UPPERCASE.ToString("X8"));

                    if ((i & 0x3) == ComboBoxStyles.CBS_SIMPLE)       DumpStyle("CBS_SIMPLE",       ComboBoxStyles.CBS_SIMPLE.ToString("X8"));
                    if ((i & 0x3) == ComboBoxStyles.CBS_DROPDOWN)     DumpStyle("CBS_DROPDOWN",     ComboBoxStyles.CBS_DROPDOWN.ToString("X8"));
                    if ((i & 0x3) == ComboBoxStyles.CBS_DROPDOWNLIST) DumpStyle("CBS_DROPDOWNLIST", ComboBoxStyles.CBS_DROPDOWNLIST.ToString("X8"));
                }
                
                if (TB_Class.Text.StartsWith("SysTreeView32"))
                {
                    if ((i & TreeViewControlStyles.TVS_CHECKBOXES)      != 0) DumpStyle("TVS_CHECKBOXES",      TreeViewControlStyles.TVS_CHECKBOXES.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_DISABLEDRAGDROP) != 0) DumpStyle("TVS_DISABLEDRAGDROP", TreeViewControlStyles.TVS_DISABLEDRAGDROP.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_EDITLABELS)      != 0) DumpStyle("TVS_EDITLABELS",      TreeViewControlStyles.TVS_EDITLABELS.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_FULLROWSELECT)   != 0) DumpStyle("TVS_FULLROWSELECT",   TreeViewControlStyles.TVS_FULLROWSELECT.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_HASBUTTONS)      != 0) DumpStyle("TVS_HASBUTTONS",      TreeViewControlStyles.TVS_HASBUTTONS.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_HASLINES)        != 0) DumpStyle("TVS_HASLINES",        TreeViewControlStyles.TVS_HASLINES.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_INFOTIP)         != 0) DumpStyle("TVS_INFOTIP",         TreeViewControlStyles.TVS_INFOTIP.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_LINESATROOT)     != 0) DumpStyle("TVS_LINESATROOT",     TreeViewControlStyles.TVS_LINESATROOT.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_NOHSCROLL)       != 0) DumpStyle("TVS_NOHSCROLL",       TreeViewControlStyles.TVS_NOHSCROLL.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_NONEVENHEIGHT)   != 0) DumpStyle("TVS_NONEVENHEIGHT",   TreeViewControlStyles.TVS_NONEVENHEIGHT.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_NOSCROLL)        != 0) DumpStyle("TVS_NOSCROLL",        TreeViewControlStyles.TVS_NOSCROLL.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_NOTOOLTIPS)      != 0) DumpStyle("TVS_NOTOOLTIPS",      TreeViewControlStyles.TVS_NOTOOLTIPS.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_RTLREADING)      != 0) DumpStyle("TVS_RTLREADING",      TreeViewControlStyles.TVS_RTLREADING.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_SHOWSELALWAYS)   != 0) DumpStyle("TVS_SHOWSELALWAYS",   TreeViewControlStyles.TVS_SHOWSELALWAYS.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_SINGLEEXPAND)    != 0) DumpStyle("TVS_SINGLEEXPAND",    TreeViewControlStyles.TVS_SINGLEEXPAND.ToString("X8"));
                    if ((i & TreeViewControlStyles.TVS_TRACKSELECT)     != 0) DumpStyle("TVS_TRACKSELECT",     TreeViewControlStyles.TVS_TRACKSELECT.ToString("X8"));
                }
            }

            var isEnabled = IsWindowEnabled(hWnd) ? "enabled" : "disabled";
            var isVisible = IsWindowVisible(hWnd) ? "visible" : "hidden";

            return string.Format("{0} ({1}, {2})", GetWindowLong(hWnd, GWL_STYLE).ToString("X8"),
                                                   isEnabled, isVisible);
        }

        private string getExtendedStyles(IntPtr hWnd)
        {
            var i = GetWindowLong(hWnd, GWL_EXSTYLE);
            LV_ExtendedStyles.Items.Clear();

            if (i != 0)
            {
                if ((i & WindowStylesEx.WS_EX_ACCEPTFILES)         != 0) DumpStyleEx("WS_EX_ACCEPTFILES",         WindowStylesEx.WS_EX_ACCEPTFILES.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_APPWINDOW)           != 0) DumpStyleEx("WS_EX_APPWINDOW",           WindowStylesEx.WS_EX_APPWINDOW.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_CLIENTEDGE)          != 0) DumpStyleEx("WS_EX_CLIENTEDGE",          WindowStylesEx.WS_EX_CLIENTEDGE.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_COMPOSITED)          != 0) DumpStyleEx("WS_EX_COMPOSITED",          WindowStylesEx.WS_EX_COMPOSITED.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_CONTEXTHELP)         != 0) DumpStyleEx("WS_EX_CONTEXTHELP",         WindowStylesEx.WS_EX_CONTEXTHELP.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_CONTROLPARENT)       != 0) DumpStyleEx("WS_EX_CONTROLPARENT",       WindowStylesEx.WS_EX_CONTROLPARENT.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_DLGMODALFRAME)       != 0) DumpStyleEx("WS_EX_DLGMODALFRAME",       WindowStylesEx.WS_EX_DLGMODALFRAME.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_LAYERED)             != 0) DumpStyleEx("WS_EX_LAYERED",             WindowStylesEx.WS_EX_LAYERED.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_LAYOUTRTL)           != 0) DumpStyleEx("WS_EX_LAYOUTRTL",           WindowStylesEx.WS_EX_LAYOUTRTL.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_LEFT)                != 0) DumpStyleEx("WS_EX_LEFT",                WindowStylesEx.WS_EX_LEFT.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_LEFTSCROLLBAR)       != 0) DumpStyleEx("WS_EX_LEFTSCROLLBAR",       WindowStylesEx.WS_EX_LEFTSCROLLBAR.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_LTRREADING)          != 0) DumpStyleEx("WS_EX_LTRREADING",          WindowStylesEx.WS_EX_LTRREADING.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_MDICHILD)            != 0) DumpStyleEx("WS_EX_MDICHILD",            WindowStylesEx.WS_EX_MDICHILD.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_NOACTIVATE)          != 0) DumpStyleEx("WS_EX_NOACTIVATE",          WindowStylesEx.WS_EX_NOACTIVATE.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_NOINHERITLAYOUT)     != 0) DumpStyleEx("WS_EX_NOINHERITLAYOUT",     WindowStylesEx.WS_EX_NOINHERITLAYOUT.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_NOPARENTNOTIFY)      != 0) DumpStyleEx("WS_EX_NOPARENTNOTIFY",      WindowStylesEx.WS_EX_NOPARENTNOTIFY.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_NOREDIRECTIONBITMAP) != 0) DumpStyleEx("WS_EX_NOREDIRECTIONBITMAP", WindowStylesEx.WS_EX_NOREDIRECTIONBITMAP.ToString("X8"));
//              if ((n & ExtendedStyles.WS_EX_OVERLAPPEDWINDOW)    != 0) DumpStyleEx("WS_EX_OVERLAPPEDWINDOW",    ExtendedWindowStyles.WS_EX_OVERLAPPEDWINDOW.ToString("X8"));
//              if ((n & ExtendedStyles.WS_EX_PALETTEWINDOW)       != 0) DumpStyleEx("WS_EX_PALETTEWINDOW",       ExtendedWindowStyles.WS_EX_PALETTEWINDOW.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_RIGHT)               != 0) DumpStyleEx("WS_EX_RIGHT",               WindowStylesEx.WS_EX_RIGHT.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_RIGHTSCROLLBAR)      != 0) DumpStyleEx("WS_EX_RIGHTSCROLLBAR",      WindowStylesEx.WS_EX_RIGHTSCROLLBAR.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_RTLREADING)          != 0) DumpStyleEx("WS_EX_RTLREADING",          WindowStylesEx.WS_EX_RTLREADING.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_STATICEDGE)          != 0) DumpStyleEx("WS_EX_STATICEDGE",          WindowStylesEx.WS_EX_STATICEDGE.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_TOOLWINDOW)          != 0) DumpStyleEx("WS_EX_TOOLWINDOW",          WindowStylesEx.WS_EX_TOOLWINDOW.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_TOPMOST)             != 0) DumpStyleEx("WS_EX_TOPMOST",             WindowStylesEx.WS_EX_TOPMOST.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_TRANSPARENT)         != 0) DumpStyleEx("WS_EX_TRANSPARENT",         WindowStylesEx.WS_EX_TRANSPARENT.ToString("X8"));
                if ((i & WindowStylesEx.WS_EX_WINDOWEDGE)          != 0) DumpStyleEx("WS_EX_WINDOWEDGE",          WindowStylesEx.WS_EX_WINDOWEDGE.ToString("X8"));
            }

            return GetWindowLong(hWnd, GWL_EXSTYLE).ToString("X8");
        }

        #endregion
        #region Class

        // There are rumers that <GetClassLong> can cause software to crash on x64
        // and that's why I am using <GetClassLongPtr> instead
        private static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            return IntPtr.Size > 4 ? GetClassLongPtr64(hWnd, nIndex) : new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        private static string getClassName(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            GetClassName(hWnd, sb, 256);

            var value = sb.ToString();
            // Output identifiers for the cute little classes below
            if (value == "#32768") return value + " (Menu)";
            if (value == "#32769") return value + " (Desktop window)";
            if (value == "#32770") return value + " (Dialog box)";
            if (value == "#32771") return value + " (Task-switch window)";
            if (value == "#32772") return value + " (Icon title)";

            return value;
        }

        private string getClassStyles(IntPtr hWnd)
        {
            var n = (int)GetClassLongPtr(hWnd, GCL_STYLE);

            // Add the available class styles to the combo box
            CMB_ClassStyles.Items.Clear();
            CMB_ClassStyles.Enabled = n != 0;

            if (n != 0)
            {
                if ((n & ClassStyles.CS_BYTEALIGNCLIENT) != 0) CMB_ClassStyles.Items.Add("CS_BYTEALIGNCLIENT");
                if ((n & ClassStyles.CS_BYTEALIGNWINDOW) != 0) CMB_ClassStyles.Items.Add("CS_BYTEALIGNWINDOW");
                if ((n & ClassStyles.CS_CLASSDC) != 0)         CMB_ClassStyles.Items.Add("CS_CLASSDC");
                if ((n & ClassStyles.CS_DBLCLKS) != 0)         CMB_ClassStyles.Items.Add("CS_DBLCLKS");
                if ((n & ClassStyles.CS_DROPSHADOW) != 0)      CMB_ClassStyles.Items.Add("CS_DROPSHADOW");
                if ((n & ClassStyles.CS_GLOBALCLASS) != 0)     CMB_ClassStyles.Items.Add("CS_GLOBALCLASS");
                if ((n & ClassStyles.CS_HREDRAW) != 0)         CMB_ClassStyles.Items.Add("CS_HREDRAW");
                if ((n & ClassStyles.CS_IME) != 0)             CMB_ClassStyles.Items.Add("CS_IME");
                if ((n & ClassStyles.CS_NOCLOSE) != 0)         CMB_ClassStyles.Items.Add("CS_NOCLOSE");
                if ((n & ClassStyles.CS_OWNDC) != 0)           CMB_ClassStyles.Items.Add("CS_OWNDC");
                if ((n & ClassStyles.CS_PARENTDC) != 0)        CMB_ClassStyles.Items.Add("CS_PARENTDC");
                if ((n & ClassStyles.CS_SAVEBITS) != 0)        CMB_ClassStyles.Items.Add("CS_SAVEBITS");
                if ((n & ClassStyles.CS_VREDRAW) != 0)         CMB_ClassStyles.Items.Add("CS_VREDRAW");
                
                // Select the first index, if any
                if (CMB_ClassStyles.Items.Count != 0)
                    CMB_ClassStyles.SelectedIndex = 0;
            }

            return GetClassLongPtr(hWnd, GCL_STYLE).ToString("X8");
        }

        private string getClassBytes(IntPtr hWnd)
        {
            var value = (long)GetClassLongPtr(hWnd, GCL_CBCLSEXTRA);
            var i = 0;
            
            CMB_ClassBytes.Items.Clear();
            CMB_ClassBytes.Enabled = value != 0;

            while (value != 0)
            {
                if (value >= 4)
                    CMB_ClassBytes.Items.Add("+" + i + "       " + GetClassLongPtr(hWnd, i).ToString("X8"));
                else
                    CMB_ClassBytes.Items.Add("+" + i + "       " + "(Unavailable)");

                i += 4;
                value = Math.Max(value - 4, 0);
            } 
            
            if (CMB_ClassBytes.Items.Count != 0)
                CMB_ClassBytes.SelectedIndex = 0;

            return GetClassLongPtr(hWnd, GCL_CBCLSEXTRA).ToString();
        }

        private static string getClassAtom(IntPtr hWnd)
        {
            return GetClassLongPtr(hWnd, GCW_ATOM).ToString("X4");
        }

        private static string getWindowBytes(IntPtr hWnd)
        {
            return GetClassLongPtr(hWnd, GCL_CBWNDEXTRA).ToString();
        }

        private static string getIconHandle(IntPtr hWnd)
        {
            var value = GetClassLongPtr(hWnd, GCL_HICON).ToString("X8");
            return value == "00000000" ? "(None)" : value;
        }

        private static string getIconHandleSM(IntPtr hWnd)
        {
            var value = GetClassLongPtr32(hWnd, GCL_HICONSM).ToString("X8");
            return value == "00000000" ? "(None)" : value;
        }

        private static string getCursorHandle(IntPtr hWnd)
        {
            var value = GetClassLongPtr(hWnd, GCL_HCURSOR).ToString("X");
            if (Environment.OSVersion.Version.Major <= 5.1)
            {
                // Hex handles for Windows XP and below
                if (value == "0")     return "(None)";
                if (value == "10011") return value + " (IDC_ARROW)";
                if (value == "10013") return value + " (IDC_IBEAM)";
                if (value == "10015") return value + " (IDC_WAIT)";
                if (value == "10017") return value + " (IDC_CROSS)";
                if (value == "10019") return value + " (IDC_UPARROW)";
                if (value == "1001B") return value + " (IDC_SIZENWSE)";
                if (value == "1001D") return value + " (IDC_SIZENESW)";
                if (value == "1001F") return value + " (IDC_SIZEWE)";
                if (value == "10021") return value + " (IDC_SIZENS)";
                if (value == "10023") return value + " (IDC_SIZEALL)";
                if (value == "10025") return value + " (IDC_NO)";
                if (value == "10027") return value + " (IDC_APPSTARTING)";
                if (value == "10029") return value + " (IDC_HELP)";
            }
            else if (Environment.OSVersion.Version.Major >= 6)
            {
                // Hex handles for Windows Vista and above
                if (value == "0")     return "(None)";
                if (value == "10003") return value + " (IDC_ARROW)";
                if (value == "10005") return value + " (IDC_IBEAM)";
                if (value == "10007") return value + " (IDC_WAIT)";
                if (value == "10009") return value + " (IDC_CROSS)";
                if (value == "1000B") return value + " (IDC_UPARROW)";
                if (value == "1000D") return value + " (IDC_SIZENWSE)";
                if (value == "1000F") return value + " (IDC_SIZENESW)";
                if (value == "10011") return value + " (IDC_SIZEWE)";
                if (value == "10013") return value + " (IDC_SIZENS)";
                if (value == "10015") return value + " (IDC_SIZEALL)";
                if (value == "10017") return value + " (IDC_NO)";
                if (value == "10019") return value + " (IDC_APPSTARTING)";
                if (value == "1001B") return value + " (IDC_HELP)";
            }

            return value;
        }

        private static string getBkgndBrush(IntPtr hWnd)
        {
            var value = GetClassLongPtr(hWnd, GCL_HBRBACKGROUND).ToString();
            int n;

            /* Apparently, the return value of <0> is shared between <hBrush.None>
            ** and <hBrush.COLOR_SCROLLBAR>.
            ** 
            ** The only way to distinguish between the two is by treating <0> as a
            ** value for <hBrush.None> and <1> as a value for <hBrush.COLOR_SCROLLBAR>.
            ** Then subtract 1 from each return value afterwards to show a correct
            ** output in the TextBox.
            */

            // This is going to be long way down. I could have minified it, but the
            // code is going to look really ugly and messy
            switch (value)
            {
                case "0":
                    n = 0;
                    return "(None)";
                case "1":
                    n = 1;
                    return n - 1 + " (COLOR_SCROLLBAR)";
                case "2":
                    n = 2;
                    return n - 1 + " (COLOR_BACKGROUND)";
                case "3":
                    n = 3;
                    return n - 1 + " (COLOR_ACTIVECAPTION)";
                case "4":
                    n = 4;
                    return n - 1 + " (COLOR_INACTIVECAPTION)";
                case "5":
                    n = 5;
                    return n - 1 + " (COLOR_MENU)";
                case "6":
                    n = 6;
                    return n - 1 + " (COLOR_WINDOW)";
                case "7":
                    n = 7;
                    return n - 1 + " (COLOR_WINDOWFRAME)";
                case "8":
                    n = 8;
                    return n - 1 + " (COLOR_MENUTEXT)";
                case "9":
                    n = 9;
                    return n - 1 + " (COLOR_WINDOWTEXT)";
                case "10":
                    n = 10;
                    return n - 1 + " (COLOR_CAPTIONTEXT)";
                case "11":
                    n = 11;
                    return n - 1 + " (COLOR_ACTIVEBORDER)";
                case "12":
                    n = 12;
                    return n - 1 + " (COLOR_INACTIVEBORDER)";
                case "13":
                    n = 13;
                    return n - 1 + " (COLOR_APPWORKSPACE)";
                case "14":
                    n = 14;
                    return n - 1 + " (COLOR_HIGHLIGHT)";
                case "15":
                    n = 15;
                    return n - 1 + " (COLOR_HIGHLIGHTTEXT)";
                case "16":
                    n = 16;
                    return n - 1 + " (COLOR_BTNFACE)";
                case "17":
                    n = 17;
                    return n - 1 + " (COLOR_BTNSHADOW)";
                case "18":
                    n = 18;
                    return n - 1 + " (COLOR_GRAYTEXT)";
                case "19":
                    n = 19;
                    return n - 1 + " (COLOR_BTNTEXT)";
                case "20":
                    n = 20;
                    return n - 1 + " (COLOR_INACTIVECAPTIONTEXT)";
                case "21":
                    n = 21;
                    return n - 1 + " (COLOR_BTNHIGHLIGHT)";
                case "22":
                    n = 22;
                    return n - 1 + " (COLOR_3DDKSHADOW)";
                case "23":
                    n = 23;
                    return n - 1 + " (COLOR_3DLIGHT)";
                case "24":
                    n = 24;
                    return n - 1 + " (COLOR_INFOTEXT)";
                case "25":
                    n = 25;
                    return n - 1 + " (COLOR_INFOBK)";
                case "26":
                    n = 26;
                    return n - 1 + " (COLOR_HOTLIGHT)";
                case "27":
                    n = 27;
                    return n - 1 + " (COLOR_GRADIENTACTIVECAPTION)";
                case "28":
                    n = 28;
                    return n - 1 + " (COLOR_GRADIENTINACTIVECAPTION)";
                case "29":
                    n = 29;
                    return n - 1 + " (COLOR_MENUHILIGHT)";
                case "30":
                    n = 30;
                    return n - 1 + " (COLOR_MENUBAR)";
                case "31":
                    n = 31;
                    return n - 1 + " (COLOR_FORM)";
                default:
                    value = GetClassLongPtr(hWnd, GCL_HBRBACKGROUND).ToString("X");
                    break;
            }

            if (value.StartsWith("FFFFFFFF"))
                value = value.Substring(8);
            
            return value;
        }

        #endregion
        #region Process

        private static string getModuleName(IntPtr hWnd)
        {
            var procId = 0;
            GetWindowThreadProcessId(hWnd, ref procId);
            var proc = Process.GetProcessById(procId);

            return proc.MainModule.ModuleName;
        }

        private static string getModulePath(IntPtr hWnd)
        {
            var procId = 0;
            GetWindowThreadProcessId(hWnd, ref procId);
            var proc = Process.GetProcessById(procId);

            return proc.MainModule.FileName;
        }

        private static string getProcessId(IntPtr hWnd)
        {
            var pid = 0;
            GetWindowThreadProcessId(hWnd, ref pid);
            var process = Process.GetProcessById(pid);

            return process.Id.ToString("X8") + " (" + process.Id + ")";
        }

        private static int getProcessIdDecimal(IntPtr hWnd)
        {
            var pid = 0;
            GetWindowThreadProcessId(hWnd, ref pid);
            var process = Process.GetProcessById(pid);

            return process.Id;
        }

        private static string getThreadID(IntPtr hWnd)
        {
            var pid = 0;
            return GetWindowThreadProcessId(hWnd, ref pid).ToString("X8") + " (" + GetWindowThreadProcessId(hWnd, ref pid) + ")";
        }

        private string getPriorityClass(IntPtr hWnd)
        {
            var n = GetPriorityClass(hWnd);
            if (n == PriorityClass.NORMAL_PRIORITY_CLASS)         return TB_PriorityClass.Text = "NORMAL_PRIORITY_CLASS (8)";
            if (n == PriorityClass.IDLE_PRIORITY_CLASS)           return TB_PriorityClass.Text = "IDLE_PRIORITY_CLASS (4)";
            if (n == PriorityClass.HIGH_PRIORITY_CLASS)           return TB_PriorityClass.Text = "HIGH_PRIORITY_CLASS (13)";
            if (n == PriorityClass.REALTIME_PRIORITY_CLASS)       return TB_PriorityClass.Text = "REALTIME_PRIORITY_CLASS (24)";
            if (n == PriorityClass.BELOW_NORMAL_PRIORITY_CLASS)   return TB_PriorityClass.Text = "BELOW_NORMAL_PRIORITY_CLASS";
            if (n == PriorityClass.ABOVE_NORMAL_PRIORITY_CLASS)   return TB_PriorityClass.Text = "ABOVE_NORMAL_PRIORITY_CLASS";
            if (n == PriorityClass.PROCESS_MODE_BACKGROUND_BEGIN) return TB_PriorityClass.Text = "PROCESS_MODE_BACKGROUND_BEGIN";
            if (n == PriorityClass.PROCESS_MODE_BACKGROUND_END)   return TB_PriorityClass.Text = "PROCESS_MODE_BACKGROUND_END";

            return GetPriorityClass(hWnd).ToString();
        }

        #endregion

        #endregion

        private void Frm_Main_Load(object sender, EventArgs e)
        {
            PNL_Bottom.Visible = false;

            // Add Window Menu items
            var handle = GetSystemMenu(Handle, false);
            InsertMenu(handle, 05, MF_BYPOSITION | MF_SEPARATOR, 0, null);
            InsertMenu(handle, 06, MF_BYPOSITION | MF_POPUP, (uint)CMENU_Configuration.Handle, "Configuration");
            InsertMenu(handle, 07, MF_BYPOSITION | MF_SEPARATOR, 0, null);
            InsertMenu(handle, 08, MF_BYPOSITION,  MNU_ABOUT, "About...");
            InsertMenu(handle, 09, MF_BYPOSITION,  MNU_CHANGELOG, "Changelog...");
            InsertMenu(handle, 10, MF_BYPOSITION,  MNU_LICENSE, "License...");
        }

        protected override void WndProc(ref Message m)
        {
            // Handle the Window Menu item click events
            if (m.Msg == WM_SYSCOMMAND)
            {
                switch (m.WParam.ToInt32())
                {
                    case MNU_ABOUT:
                        var entryAssembly = Assembly.GetEntryAssembly();
                        var version       = Application.ProductVersion;
                        var buildDate     = new FileInfo(entryAssembly.Location).LastWriteTime;
                        var author        = Application.CompanyName;
                        var info          = "Version: " + version
                                            + "\nBuild Date: " + buildDate
                                            + "\n\nAuthor: " + author
                                            + "\nPage: http://github.com/ei/SharpFind" 
                                            + "\n\nThis open-source project is licensed under the MIT license. See the license file for details.";

                        MessageBox.Show(info, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case MNU_CHANGELOG:
                        var changelogPath = Application.StartupPath + "\\Changelog.txt";
                        if   (File.Exists(changelogPath)) Process.Start(changelogPath);
                        else MessageBox.Show("The following file was not found:\n" + changelogPath, "Not Found", MessageBoxButtons.OK);
                        break;
                    case MNU_LICENSE:
                        var licensePath = Application.StartupPath + "\\License.txt";
                        if   (File.Exists(licensePath)) Process.Start(licensePath);
                        else MessageBox.Show("The following file was not found:\n" + licensePath, "Not Found", MessageBoxButtons.OK);
                        break;
                }
            }

            // Handle the Finder Tool drag & release
            switch (m.Msg)
            {
                case (int)WindowsMessages.WM_LBUTTONUP:
                    CaptureMouse(false);
                    break;
                case (int)WindowsMessages.WM_MOUSEMOVE:
                    HandleMouseMovement();
                    break;
                case (int)WindowsMessages.WM_PAINT:
                    if (LV_WindowStyles.View == View.Details && LV_WindowStyles.Columns.Count > 0)
                        LV_WindowStyles.Columns[LV_WindowStyles.Columns.Count - 1].Width = -2;

                    if (LV_ExtendedStyles.View == View.Details && LV_ExtendedStyles.Columns.Count > 0)
                        LV_ExtendedStyles.Columns[LV_ExtendedStyles.Columns.Count - 1].Width = -2;
                    break;
            }
            
            base.WndProc(ref m);
        }

        private void CaptureMouse(bool captured)
        {
            if (captured)
            {
                SetCapture(Handle);

                Cursor.Current = _cursorFinder;
                PB_Tool.Image = Resources.finder_out;
                
                if (CMNU_Minimize.Checked)
                {
                    PNL_Bottom.Visible = false;
                    Height = formHeightCollapsed; 
                }
            }
            else
            {
                ReleaseCapture();

                Cursor.Current = _cursorDefault;
                PB_Tool.Image = Resources.finder_in;

                if (!isHandleNull)
                {
                    PNL_Bottom.Visible = true;
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
                hWnd = WindowFromPoint(Cursor.Position);

                // Prevent retrieving information about the program itself, just like Spy++
                var pid = getProcessIdDecimal(hWnd);
                if (GetCurrentProcessId() == pid)
                {
                    isHandleNull = true;
                    return;
                }

                isHandleNull = false;

                if (hPreviousWindow != IntPtr.Zero && hPreviousWindow != hWnd)
                    WindowHighlighter.Refresh(hPreviousWindow);

                if (hWnd == IntPtr.Zero)
                    return;

                hPreviousWindow = hWnd;

                // General Information tab
                TB_WindowCaption.Text  = getWindowCaption(hWnd);
                TB_WindowHandle.Text   = hWnd.ToInt32().ToString("X8") + " (" + hWnd.ToInt32() + ")";

                TB_Class.Text          = getClass(hWnd);
                TB_Style.Text          = getStyle(hWnd);
                TB_Rectangle.Text      = getWindowRect(hWnd);
                TB_ClientRect.Text     = getClientRect(hWnd);
                TB_InstanceHandle.Text = getInstanceHandle(hWnd);
                TB_ControlID.Text      = getControlID(hWnd);
                TB_UserData.Text       = getUserData(hWnd);
                getWindowBytesCombo(hWnd);

                //Styles tab
                TB_WindowStyles.Text   = TB_Style.Text.Split('(')[0].TrimEnd();
                TB_ExtendedStyles.Text = getExtendedStyles(hWnd);

                // Class tab
                TB_ClassName.Text      = getClassName(hWnd);
                TB_ClassStyles.Text    = getClassStyles(hWnd);
                TB_ClassBytes.Text     = getClassBytes(hWnd);
                TB_ClassAtom.Text      = getClassAtom(hWnd);
                TB_WindowBytes.Text    = getWindowBytes(hWnd);
                TB_IconHandle.Text     = getIconHandle(hWnd);
                TB_IconHandleSM.Text   = getIconHandleSM(hWnd);
                TB_CursorHandle.Text   = getCursorHandle(hWnd);
                TB_BkgndBrush.Text     = getBkgndBrush(hWnd);

                // Process tab
                TB_ModuleName.Text     = getModuleName(hWnd);
                TB_ModulePath.Text     = getModulePath(hWnd);
                TB_ProcessID.Text      = getProcessId(hWnd);
                TB_ThreadID.Text       = getThreadID(hWnd);
                TB_PriorityClass.Text  = getPriorityClass(Process.GetProcessById(pid).Handle);

                Text = appName + " - " + TB_WindowHandle.Text.Split('(')[0].TrimEnd();

                // The flickering shall not pass
                if (hWndOld == hWnd)
                    return;

                WindowHighlighter.Highlight(hWnd);
                hWndOld = hWnd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Cursor.Current = _cursorDefault;
                PB_Tool.Image = Resources.finder_in;
            }
        }

        private void PB_Tool_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                CaptureMouse(true);
        }

        private void Frm_Main_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                CaptureMouse(false);
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        private void CMNU_Minimize_Click(object sender, EventArgs e)
        {
            CMNU_Minimize.Checked = !CMNU_Minimize.Checked;
        }
    }
}