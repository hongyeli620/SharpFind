using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System;
using SharpFind.Classes;
using SharpFind.Controls;
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
            Text = Application.ProductName; // Pronounced Sharp Find
            ControlBox = true;
            MinimizeBox = true;
            MaximizeBox = false;
            ShowIcon = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;

            using (var ms = new MemoryStream(Resources.finder))
            {
                _cursorDefault = Cursor.Current;
                _cursorFinder = new Cursor(ms);
            }
        }

        #region Variables

        // Cursors to be used
        private readonly Cursor _cursorDefault;
        private readonly Cursor _cursorFinder;

        private bool isCapturing;
        private IntPtr hPreviousWindow;

        private IntPtr hWnd;
        private IntPtr hWndOld;
        private bool handleIsNull;

        // Single window menu items
        private const int MNU_ABOUT = 1000;
        private const int MNU_LICENSE = 1001;
        private const int MNU_CHANGELOG = 1002;

        #endregion
        #region Functions

        #region General

        private static bool IsUnicode(IntPtr hWnd)
        {
            return IsWindowUnicode(hWnd);
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
            if (IsUnicode(hWnd))
                value = value + " (Unicode)";

            return value;
        }

        private string getStyle(IntPtr hWnd)
        {
            var n = GetWindowLong(hWnd, GWL_STYLE);

            LV_WindowStyles.Items.Clear();

            if (n != 0)
            {
                ListViewItem item;
                if ((n & WindowStylesFlags.WS_BORDER) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_BORDER");
                    item.SubItems.Add(WindowStylesFlags.WS_BORDER.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_CAPTION) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_CAPTION");
                    item.SubItems.Add(WindowStylesFlags.WS_CAPTION.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_CHILD) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_CHILD");
                    item.SubItems.Add(WindowStylesFlags.WS_CAPTION.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_CLIPCHILDREN) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_CLIPCHILDREN");
                    item.SubItems.Add(WindowStylesFlags.WS_CLIPCHILDREN.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_CLIPSIBLINGS) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_CLIPSIBLINGS");
                    item.SubItems.Add(WindowStylesFlags.WS_CLIPSIBLINGS.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_DISABLED) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_DISABLED");
                    item.SubItems.Add(WindowStylesFlags.WS_DISABLED.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_DLGFRAME) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_DLGFRAME");
                    item.SubItems.Add(WindowStylesFlags.WS_DLGFRAME.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_GROUP) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_GROUP");
                    item.SubItems.Add(WindowStylesFlags.WS_GROUP.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_HSCROLL) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_HSCROLL");
                    item.SubItems.Add(WindowStylesFlags.WS_HSCROLL.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_ICONIC) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_ICONIC");
                    item.SubItems.Add(WindowStylesFlags.WS_ICONIC.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_MAXIMIZE) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_MAXIMIZE");
                    item.SubItems.Add(WindowStylesFlags.WS_MAXIMIZE.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_MAXIMIZEBOX) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_MAXIMIZEBOX");
                    item.SubItems.Add(WindowStylesFlags.WS_MAXIMIZEBOX.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_MINIMIZE) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_MINIMIZE");
                    item.SubItems.Add(WindowStylesFlags.WS_MINIMIZE.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_MINIMIZEBOX) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_MINIMIZEBOX");
                    item.SubItems.Add(WindowStylesFlags.WS_MINIMIZEBOX.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_OVERLAPPED) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_OVERLAPPED");
                    item.SubItems.Add(WindowStylesFlags.WS_OVERLAPPED.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_POPUP) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_POPUP");
                    item.SubItems.Add(WindowStylesFlags.WS_POPUP.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_SIZEBOX) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_SIZEBOX");
                    item.SubItems.Add(WindowStylesFlags.WS_SIZEBOX.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_SYSMENU) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_SYSMENU");
                    item.SubItems.Add(WindowStylesFlags.WS_SYSMENU.ToString("X8"));

                }
                if ((n & WindowStylesFlags.WS_TABSTOP) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_TABSTOP");
                    item.SubItems.Add(WindowStylesFlags.WS_TABSTOP.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_TILED) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_TILED");
                    item.SubItems.Add(WindowStylesFlags.WS_TILED.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_VISIBLE) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_VISIBLE");
                    item.SubItems.Add(WindowStylesFlags.WS_VISIBLE.ToString("X8"));
                }
                if ((n & WindowStylesFlags.WS_VSCROLL) != 0)
                {
                    item = LV_WindowStyles.Items.Add("WS_VSCROLL");
                    item.SubItems.Add(WindowStylesFlags.WS_VSCROLL.ToString("X8"));
                }
            }

            return GetWindowLong(hWnd, GWL_STYLE).ToString("X8");
        }

        private static string getWindowRect(IntPtr hWnd)
        {
            var wRect = new RECT();
            GetWindowRect(hWnd, out wRect);
            return string.Format("({2},{3}) - ({4},{5}), {0} x {1}", wRect.right - wRect.left,
                                                                     wRect.bottom - wRect.top,
                                                                     wRect.left,
                                                                     wRect.top,
                                                                     wRect.right,
                                                                     wRect.bottom);
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

        private string getExtendedStyles(IntPtr hWnd)
        {
            var n = GetWindowLong(hWnd, GWL_EXSTYLE);

            LV_ExtendedStyles.Items.Clear();

            if (n != 0)
            {
                ListViewItem item;
                if ((n & ExtendedStylesFlags.WS_EX_ACCEPTFILES) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_ACCEPTFILES");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_ACCEPTFILES.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_APPWINDOW) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_APPWINDOW");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_APPWINDOW.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_CLIENTEDGE) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_CLIENTEDGE");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_CLIENTEDGE.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_COMPOSITED) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_COMPOSITED");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_COMPOSITED.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_CONTEXTHELP) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_CONTEXTHELP");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_CONTEXTHELP.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_CONTROLPARENT) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_CONTROLPARENT");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_CONTROLPARENT.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_DLGMODALFRAME) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_DLGMODALFRAME");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_DLGMODALFRAME.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_LAYERED) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_LAYERED");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_LAYERED.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_LAYOUTRTL) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_LAYOUTRTL");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_LAYOUTRTL.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_LEFT) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_LEFT");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_LEFT.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_LEFTSCROLLBAR) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_LEFTSCROLLBAR");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_LEFTSCROLLBAR.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_LTRREADING) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_LTRREADING");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_LTRREADING.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_MDICHILD) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_MDICHILD");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_MDICHILD.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_NOACTIVATE) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_NOACTIVATE");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_NOACTIVATE.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_NOINHERITLAYOUT) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_NOINHERITLAYOUT");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_NOINHERITLAYOUT.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_NOPARENTNOTIFY) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_NOPARENTNOTIFY");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_NOPARENTNOTIFY.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_RIGHT) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_RIGHT");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_RIGHT.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_RIGHTSCROLLBAR) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_RIGHTSCROLLBAR");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_RIGHTSCROLLBAR.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_RTLREADING) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_RTLREADING");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_RTLREADING.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_STATICEDGE) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_STATICEDGE");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_STATICEDGE.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_TOOLWINDOW) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_TOOLWINDOW");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_TOOLWINDOW.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_TOPMOST) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_TOPMOST");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_TOPMOST.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_TRANSPARENT) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_TRANSPARENT");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_TRANSPARENT.ToString("X8"));
                }
                if ((n & ExtendedStylesFlags.WS_EX_WINDOWEDGE) != 0)
                {
                    item = LV_ExtendedStyles.Items.Add("WS_EX_WINDOWEDGE");
                    item.SubItems.Add(ExtendedStylesFlags.WS_EX_WINDOWEDGE.ToString("X8"));
                }
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
                if ((n & ClassStyles.CS_BYTEALIGNCLIENT) != 0)
                    CMB_ClassStyles.Items.Add("CS_BYTEALIGNCLIENT");
                if ((n & ClassStyles.CS_BYTEALIGNWINDOW) != 0) 
                    CMB_ClassStyles.Items.Add("CS_BYTEALIGNWINDOW");
                if ((n & ClassStyles.CS_CLASSDC) != 0) 
                    CMB_ClassStyles.Items.Add("CS_CLASSDC");
                if ((n & ClassStyles.CS_DBLCLKS) != 0) 
                    CMB_ClassStyles.Items.Add("CS_DBLCLKS");
                if ((n & ClassStyles.CS_DROPSHADOW) != 0)
                    CMB_ClassStyles.Items.Add("CS_DROPSHADOW");
                if ((n & ClassStyles.CS_GLOBALCLASS) != 0)
                    CMB_ClassStyles.Items.Add("CS_GLOBALCLASS");
                if ((n & ClassStyles.CS_HREDRAW) != 0)
                    CMB_ClassStyles.Items.Add("CS_HREDRAW");
                if ((n & ClassStyles.CS_IME) != 0) 
                    CMB_ClassStyles.Items.Add("CS_IME");
                if ((n & ClassStyles.CS_NOCLOSE) != 0) 
                    CMB_ClassStyles.Items.Add("CS_NOCLOSE");
                if ((n & ClassStyles.CS_OWNDC) != 0) 
                    CMB_ClassStyles.Items.Add("CS_OWNDC");
                if ((n & ClassStyles.CS_PARENTDC) != 0) 
                    CMB_ClassStyles.Items.Add("CS_PARENTDC");
                if ((n & ClassStyles.CS_SAVEBITS) != 0) 
                    CMB_ClassStyles.Items.Add("CS_SAVEBITS");
                if ((n & ClassStyles.CS_VREDRAW) != 0) 
                    CMB_ClassStyles.Items.Add("CS_VREDRAW");
                
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
            Height = 107;
            PNL_Bottom.Visible = false;

            // Use the native Explorer ListView theme for Windows Vista and upper
            if (Environment.OSVersion.Version.Major >= 6) 
            {
                ListViewEx.SetWindowTheme(LV_WindowStyles.Handle, "explorer", null);
                ListViewEx.SetWindowTheme(LV_ExtendedStyles.Handle, "explorer", null);
            }

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
                        var info = "Version: " + version
                                   + "\nBuild Date: " + buildDate
                                   + "\n\nAuthor: " + author
                                   + "\nPage: http://github.com/ei" 
                                   + "\n\nThis open-source project is licensed under the MIT license. See the license file for details.";

                        MessageBox.Show(info, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case MNU_CHANGELOG:
                        var changelogPath = Application.StartupPath + "\\Changelog.txt";
                        if   (File.Exists(changelogPath)) { Process.Start(changelogPath); }
                        else { MessageBox.Show("The following file was not found:\n" + changelogPath, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                        break;
                    case MNU_LICENSE:
                        var licensePath = Application.StartupPath + "\\License.txt";
                        if   (File.Exists(licensePath)) { Process.Start(licensePath); }
                        else { MessageBox.Show("The following file was not found:\n" + licensePath, "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
                        break;
                }
            }

            // Handle the Finder Tool drag & release
            switch (m.Msg)
            {
                case (int)WindowsMessages.LBUTTONUP:
                    CaptureMouse(false);
                    break;
                case (int)WindowsMessages.MOUSEMOVE:
                    HandleMouseMovement();
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
                    Height = 107; 
                }
            }
            else
            {
                ReleaseCapture();

                Cursor.Current = _cursorDefault;
                PB_Tool.Image = Resources.finder_in;

                if (!handleIsNull)
                {
                    PNL_Bottom.Visible = true;
                    Height = 421;
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
                    handleIsNull = true;
                    return;
                }

                handleIsNull = false;

                if (hPreviousWindow != IntPtr.Zero && hPreviousWindow != hWnd)
                    WindowHighlighter.Refresh(hPreviousWindow);

                if (hWnd == IntPtr.Zero)
                    return;

                hPreviousWindow = hWnd;

                // General Information tab
                TB_WindowCaption.Text = getWindowCaption(hWnd);
                TB_WindowHandle.Text = hWnd.ToInt32().ToString("X8") + " (" + hWnd.ToInt32() + ")";

                TB_Class.Text = getClass(hWnd);
                TB_Style.Text = getStyle(hWnd);
                TB_Rectangle.Text = getWindowRect(hWnd);
                TB_ClientRect.Text = getClientRect(hWnd);
                TB_InstanceHandle.Text = getInstanceHandle(hWnd);
                TB_ControlID.Text = getControlID(hWnd);
                TB_UserData.Text = getUserData(hWnd);
                getWindowBytesCombo(hWnd);

                //Styles tab
                TB_WindowStyles.Text = TB_Style.Text;
                TB_ExtendedStyles.Text = getExtendedStyles(hWnd);

                // Class tab
                TB_ClassName.Text = getClassName(hWnd);
                TB_ClassStyles.Text = getClassStyles(hWnd);
                TB_ClassBytes.Text = getClassBytes(hWnd);
                TB_ClassAtom.Text = getClassAtom(hWnd);
                TB_WindowBytes.Text = getWindowBytes(hWnd);
                TB_IconHandle.Text = getIconHandle(hWnd);
                TB_IconHandleSM.Text = getIconHandleSM(hWnd);
                TB_CursorHandle.Text = getCursorHandle(hWnd);
                TB_BkgndBrush.Text = getBkgndBrush(hWnd);

                // Process tab
                TB_ModuleName.Text = getModuleName(hWnd);
                TB_ModulePath.Text = getModulePath(hWnd);
                TB_ProcessID.Text = getProcessId(hWnd);
                TB_ThreadID.Text = getThreadID(hWnd);
                TB_PriorityClass.Text = getPriorityClass(Process.GetProcessById(pid).Handle);

                Text = Application.ProductName + " - " + TB_WindowHandle.Text.Split('(')[0];

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

        private void frm_Main_MouseDown(object sender, MouseEventArgs e)
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