/* NativeMethods.cs
** This file is part #Find.
**
** Documentaion pulled directly from MSDN.
** 
** Copyright 2017 by Jad Altahan <xviyy@aol.com>
** Licensed under MIT
** <https://github.com/xv/SharpFind/blob/master/LICENSE>
*/



namespace SharpFind.Classes
{
    internal static class NativeMethods
    {
        #region WinUser.h & CommCtrl.h Definitions

        internal static class Styles
        {
            /// <summary>
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// See: https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx
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
                WS_GROUP            = 0x00020000L,
                WS_TABSTOP          = 0x00010000L,
                WS_MINIMIZEBOX      = 0x00020000L,
                WS_MAXIMIZEBOX      = 0x00010000L,
//              WS_TILED            = WS_OVERLAPPED,
//              WS_ICONIC           = WS_MINIMIZE,
//              WS_SIZEBOX          = WS_THICKFRAME,
//              WS_TILEDWINDOW      = WS_OVERLAPPEDWINDOW,
                WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU |
                                      WS_THICKFRAME | WS_MINIMIZEBOX          |
                                      WS_MAXIMIZEBOX,
                WS_POPUPWINDOW      = WS_POPUP | WS_BORDER | WS_SYSMENU;
//              WS_CHILDWINDOW      = WS_CHILD;
            }

            /// <summary>
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class WindowStylesEx
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
//              WS_EX_OVERLAPPEDWINDOW    = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
//              WS_EX_PALETTEWINDOW       = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
                WS_EX_LAYERED             = 0x00080000,
                WS_EX_NOINHERITLAYOUT     = 0x00100000L,
                WS_EX_NOREDIRECTIONBITMAP = 0x00200000L,
                WS_EX_LAYOUTRTL           = 0x00400000L,
                WS_EX_COMPOSITED          = 0x02000000L,
                WS_EX_NOACTIVATE          = 0x08000000L;
            }

            /// <summary>
            /// Class: Button
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
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
//              BS_PUSHBOX         = 0x0000000AL,
                BS_OWNERDRAW       = 0x0000000BL,
//              BS_TYPEMASK        = 0x0000000FL,
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
            /// Class: ComboBox
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class ComboBoxStyles
            {
                internal static readonly long
                CBS_SIMPLE            = 0x0001L,
                CBS_DROPDOWN          = 0x0002L,
                CBS_DROPDOWNLIST      = 0x0003L,
                CBS_OWNERDRAWFIXED    = 0x0010L,
                CBS_OWNERDRAWVARIABLE = 0x0020L,
                CBS_AUTOHSCROLL       = 0x0040L,
                CBS_OEMCONVERT        = 0x0080L,
                CBS_SORT              = 0x0100L,
                CBS_HASSTRINGS        = 0x0200L,
                CBS_NOINTEGRALHEIGHT  = 0x0400L,
                CBS_DISABLENOSCROLL   = 0x0800L,
                CBS_UPPERCASE         = 0x2000L,
                CBS_LOWERCASE         = 0x4000L;
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
            /// Class: SysDateTimePick32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class DateTimeControlStyles
            {
                internal static readonly int
                DTS_UPDOWN                 = 0x0001,
                DTS_SHOWNONE               = 0x0002,
                DTS_SHORTDATEFORMAT        = 0x0000,
                DTS_LONGDATEFORMAT         = 0x0004,
//              DTS_SHORTDATECENTURYFORMAT = 0x000C,
                DTS_TIMEFORMAT             = 0x0009,
                DTS_APPCANPARSE            = 0x0010,
                DTS_RIGHTALIGN             = 0x0020;
            }

            /// <summary>
            /// Class: #32770
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class DialogBoxStyles
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

            /// <summary>
            /// Class: Edit
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class EditControlStyles
            {
                internal static readonly long
                ES_LEFT        = 0x0000L,
                ES_CENTER      = 0x0001L,
                ES_RIGHT       = 0x0002L,
                ES_MULTILINE   = 0x0004L,
                ES_UPPERCASE   = 0x0008L,
                ES_LOWERCASE   = 0x0010L,
                ES_PASSWORD    = 0x0020L,
                ES_AUTOVSCROLL = 0x0040L,
                ES_AUTOHSCROLL = 0x0080L,
                ES_NOHIDESEL   = 0x0100L,
                ES_OEMCONVERT  = 0x0400L,
                ES_READONLY    = 0x0800L,
                ES_WANTRETURN  = 0x1000L,
                ES_NUMBER      = 0x2000L;
            }

            /// <summary>
            /// Class: SysHeader32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class HeaderControlStyles
            {
                internal static readonly int
                HDS_HORZ       = 0x0000,
                HDS_BUTTONS    = 0x0002,
                HDS_HOTTRACK   = 0x0004,
                HDS_HIDDEN     = 0x0008,
                HDS_DRAGDROP   = 0x0040,
                HDS_FULLDRAG   = 0x0080,
                HDS_FILTERBAR  = 0x0100,
                HDS_FLAT       = 0x0200,
                HDS_CHECKBOXES = 0x0400,
                HDS_NOSIZING   = 0x0800,
                HDS_OVERFLOW   = 0x1000;
            }

            /// <summary>
            /// Class: ListBox
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class ListBoxStyles
            {
                internal static readonly long
                LBS_NOTIFY            = 0x0001L,
                LBS_SORT              = 0x0002L,
                LBS_NOREDRAW          = 0x0004L,
                LBS_MULTIPLESEL       = 0x0008L,
                LBS_OWNERDRAWFIXED    = 0x0010L,
                LBS_OWNERDRAWVARIABLE = 0x0020L,
                LBS_HASSTRINGS        = 0x0040L,
                LBS_USETABSTOPS       = 0x0080L,
                LBS_NOINTEGRALHEIGHT  = 0x0100L,
                LBS_MULTICOLUMN       = 0x0200L,
                LBS_WANTKEYBOARDINPUT = 0x0400L,
                LBS_EXTENDEDSEL       = 0x0800L,
                LBS_DISABLENOSCROLL   = 0x1000L,
                LBS_NODATA            = 0x2000L,
                LBS_NOSEL             = 0x4000L,
                LBS_COMBOBOX          = 0x8000L;
            }

            /// <summary>
            /// Class: SysListView32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class ListViewStyles
            {
                internal static readonly int
                LVS_ICON            = 0x0000,
                LVS_REPORT          = 0x0001,
                LVS_SMALLICON       = 0x0002,
                LVS_LIST            = 0x0003,
                LVS_TYPEMASK        = 0x0003,
                LVS_SINGLESEL       = 0x0004,
                LVS_SHOWSELALWAYS   = 0x0008,
                LVS_SORTASCENDING   = 0x0010,
                LVS_SORTDESCENDING  = 0x0020,
                LVS_SHAREIMAGELISTS = 0x0040,
                LVS_NOLABELWRAP     = 0x0080,
                LVS_AUTOARRANGE     = 0x0100,
                LVS_EDITLABELS      = 0x0200,
                LVS_OWNERDATA       = 0x1000,
                LVS_NOSCROLL        = 0x2000,
//              LVS_TYPESTYLEMASK   = 0xFC00,
                LVS_ALIGNTOP        = 0x0000,
                LVS_ALIGNLEFT       = 0x0800,
                LVS_ALIGNMASK       = 0x0C00,
                LVS_OWNERDRAWFIXED  = 0x0400,
                LVS_NOCOLUMNHEADER  = 0x4000,
                LVS_NOSORTHEADER    = 0x8000;
            }

            /// <summary>
            /// Class: MDIClient
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class MDIClientStyles
            {
                internal static readonly int
                MDIS_ALLCHILDSTYLES = 0x0001;
            }

            /// <summary>
            /// Class: SysMonthCal32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class MonthCalendarControlStyles
            {
                internal static readonly int
                MCS_DAYSTATE         = 0x0001,
                MCS_MULTISELECT      = 0x0002,
                MCS_WEEKNUMBERS      = 0x0004,
                MCS_NOTODAYCIRCLE    = 0x0008,
                MCS_NOTODAY          = 0x0010,
                MCS_NOTRAILINGDATES  = 0x0040,
                MCS_SHORTDAYSOFWEEK  = 0x0080,
                MCS_NOSELCHANGEONNAV = 0x0100;
            }

            /// <summary>
            /// Class: SysPager
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class PagerControlStyles
            {
                internal static readonly int
                PGS_VERT       = 0x00000000,
                PGS_HORZ       = 0x00000001,
                PGS_AUTOSCROLL = 0x00000002,
                PGS_DRAGNDROP  = 0x00000004;
            }

            /// <summary>
            /// Class: msctls_progress32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class ProgressControlStyles
            {
                internal static readonly int
                PBS_SMOOTH   = 0x01,
                PBS_VERTICAL = 0x04;
            }

            /// <summary>
            /// Class: ScrollBar
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class ScrollbarStyles
            {
                internal static readonly long
                SBS_HORZ                    = 0x0000L,
                SBS_VERT                    = 0x0001L,
                SBS_TOPALIGN                = 0x0002L,
                SBS_LEFTALIGN               = 0x0002L,
                SBS_BOTTOMALIGN             = 0x0004L,
                SBS_RIGHTALIGN              = 0x0004L,
                SBS_SIZEBOXTOPLEFTALIGN     = 0x0002L,
                SBS_SIZEBOXBOTTOMRIGHTALIGN = 0x0004L,
                SBS_SIZEBOX                 = 0x0008L,
                SBS_SIZEGRIP                = 0x0010L;
            }

            /// <summary>
            /// Class: msctls_statusbar32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class StatusBarStyles
            {
                internal static readonly int
                SBARS_SIZEGRIP = 0x0100,
                SBARS_TOOLTIPS = 0x0800,
                SBT_TOOLTIPS   = 0x0800;
            }

            /// <summary>
            /// Class: Static
            /// Based on Windows Kits\8.1\Include\um\WinUser.h
            /// </summary>
            internal static class StaticControlStyles
            {
                internal static readonly long
                SS_LEFT            = 0x00000000L,
                SS_CENTER          = 0x00000001L,
                SS_RIGHT           = 0x00000002L,
                SS_ICON            = 0x00000003L,
                SS_BLACKRECT       = 0x00000004L,
                SS_GRAYRECT        = 0x00000005L,
                SS_WHITERECT       = 0x00000006L,
                SS_BLACKFRAME      = 0x00000007L,
                SS_GRAYFRAME       = 0x00000008L,
                SS_WHITEFRAME      = 0x00000009L,
                SS_USERITEM        = 0x0000000AL,
                SS_SIMPLE          = 0x0000000BL,
                SS_LEFTNOWORDWRAP  = 0x0000000CL,
                SS_OWNERDRAW       = 0x0000000DL,
                SS_BITMAP          = 0x0000000EL,
                SS_ENHMETAFILE     = 0x0000000FL,
                SS_ETCHEDHORZ      = 0x00000010L,
                SS_ETCHEDVERT      = 0x00000011L,
                SS_ETCHEDFRAME     = 0x00000012L,
                SS_TYPEMASK        = 0x0000001FL,
                SS_REALSIZECONTROL = 0x00000040L,
                SS_NOPREFIX        = 0x00000080L,
                SS_NOTIFY          = 0x00000100L,
                SS_CENTERIMAGE     = 0x00000200L,
                SS_RIGHTJUST       = 0x00000400L,
                SS_REALSIZEIMAGE   = 0x00000800L,
                SS_SUNKEN          = 0x00001000L,
                SS_EDITCONTROL     = 0x00002000L,
                SS_ENDELLIPSIS     = 0x00004000L,
                SS_PATHELLIPSIS    = 0x00008000L,
                SS_WORDELLIPSIS    = 0x0000C000L,
                SS_ELLIPSISMASK    = 0x0000C000L;
            }

            /// <summary>
            /// Class: ToolbarWindow32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class ToolbarControlStyles
            {
                internal static readonly int
                TBSTYLE_TOOLTIPS     = 0x0100,
                TBSTYLE_WRAPABLE     = 0x0200,
                TBSTYLE_ALTDRAG      = 0x0400,
                TBSTYLE_FLAT         = 0x0800,
                TBSTYLE_LIST         = 0x1000,
                TBSTYLE_CUSTOMERASE  = 0x2000,
                TBSTYLE_REGISTERDROP = 0x4000,
                TBSTYLE_TRANSPARENT  = 0x8000;
            }

            /// <summary>
            /// Class: ReBarWindow32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class RebarControlStyles
            {
                internal static readonly int
                RBS_TOOLTIPS        = 0x00000100,
                RBS_VARHEIGHT       = 0x00000200,
                RBS_BANDBORDERS     = 0x00000400,
                RBS_FIXEDORDER      = 0x00000800,
                RBS_REGISTERDROP    = 0x00001000,
                RBS_AUTOSIZE        = 0x00002000,
                RBS_VERTICALGRIPPER = 0x00004000,
                RBS_DBLCLKTOGGLE    = 0x00008000;
            }

            /// <summary>
            /// Class: SysAnimate32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class AnimationControlStyles
            {
                internal static readonly int
                ACS_CENTER      = 0x0001,
                ACS_TRANSPARENT = 0x0002,
                ACS_AUTOPLAY    = 0x0004,
                ACS_TIMER       = 0x0008;
            }

            /// <summary>
            /// Class: SysLink
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class SysLinkControlStyles
            {
                internal static readonly int
                LWS_TRANSPARENT     = 0x0001,
                LWS_IGNORERETURN    = 0x0002,
                LWS_NOPREFIX        = 0x0004,
                LWS_USEVISUALSTYLE  = 0x0008,
                LWS_USECUSTOMTEXT   = 0x0010,
                LWS_RIGHT           = 0x0020;
            }

            /// <summary>
            /// Class: msctls_trackbar32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class TrackbarControlStyles
            {
                internal static readonly int
                TBS_AUTOTICKS        = 0x0001,
                TBS_VERT             = 0x0002,
                TBS_HORZ             = 0x0000,
                TBS_TOP              = 0x0004,
                TBS_BOTTOM           = 0x0000,
                TBS_LEFT             = 0x0004,
                TBS_RIGHT            = 0x0000,
                TBS_BOTH             = 0x0008,
                TBS_NOTICKS          = 0x0010,
                TBS_ENABLESELRANGE   = 0x0020,
                TBS_FIXEDLENGTH      = 0x0040,
                TBS_NOTHUMB          = 0x0080,
                TBS_TOOLTIPS         = 0x0100,
                TBS_REVERSED         = 0x0200,
                TBS_DOWNISLEFT       = 0x0400,
                TBS_NOTIFYBEFOREMOVE = 0x0800,
                TBS_TRANSPARENTBKGND = 0x1000;
            }

            /// <summary>
            /// Class: SysTabControl32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class TabControlStyles
            {
                internal static readonly int
                TCS_SCROLLOPPOSITE    = 0x0001,
                TCS_BOTTOM            = 0x0002,
                TCS_RIGHT             = 0x0002,
                TCS_MULTISELECT       = 0x0004,
                TCS_FLATBUTTONS       = 0x0008,
                TCS_FORCEICONLEFT     = 0x0010,
                TCS_FORCELABELLEFT    = 0x0020,
                TCS_HOTTRACK          = 0x0040,
                TCS_VERTICAL          = 0x0080,
                TCS_TABS              = 0x0000,
                TCS_BUTTONS           = 0x0100,
                TCS_SINGLELINE        = 0x0000,
                TCS_MULTILINE         = 0x0200,
                TCS_RIGHTJUSTIFY      = 0x0000,
                TCS_FIXEDWIDTH        = 0x0400,
                TCS_RAGGEDRIGHT       = 0x0800,
                TCS_FOCUSONBUTTONDOWN = 0x1000,
                TCS_OWNERDRAWFIXED    = 0x2000,
                TCS_TOOLTIPS          = 0x4000,
                TCS_FOCUSNEVER        = 0x8000;
            }

            /// <summary>
            /// Class: tooltips_class32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class TooltipStyles
            {
                internal static readonly int
                TTS_ALWAYSTIP      = 0x01,
                TTS_NOPREFIX       = 0x02,
                TTS_NOANIMATE      = 0x10,
                TTS_NOFADE         = 0x20,
                TTS_BALLOON        = 0x40,
                TTS_CLOSE          = 0x80,
                TTS_USEVISUALSTYLE = 0x100;
            }

            /// <summary>
            /// Class: SysTreeView32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class TreeViewControlStyles
            {
                internal static readonly int
                TVS_HASBUTTONS      = 0x0001,
                TVS_HASLINES        = 0x0002,
                TVS_LINESATROOT     = 0x0004,
                TVS_EDITLABELS      = 0x0008,
                TVS_DISABLEDRAGDROP = 0x0010,
                TVS_SHOWSELALWAYS   = 0x0020,
                TVS_RTLREADING      = 0x0040,
                TVS_NOTOOLTIPS      = 0x0080,
                TVS_CHECKBOXES      = 0x0100,
                TVS_TRACKSELECT     = 0x0200,
                TVS_SINGLEEXPAND    = 0x0400,
                TVS_INFOTIP         = 0x0800,
                TVS_FULLROWSELECT   = 0x1000,
                TVS_NOSCROLL        = 0x2000,
                TVS_NONEVENHEIGHT   = 0x4000,
                TVS_NOHSCROLL       = 0x8000;
            }

            /// <summary>
            /// Class: msctls_updown32
            /// Based on Windows Kits\8.1\CommCtrl.h
            /// </summary>
            internal static class UpDownControlStyles
            {
                internal static readonly int
                UDS_WRAP        = 0x0001,
                UDS_SETBUDDYINT = 0x0002,
                UDS_ALIGNRIGHT  = 0x0004,
                UDS_ALIGNLEFT   = 0x0008,
                UDS_AUTOBUDDY   = 0x0010,
                UDS_ARROWKEYS   = 0x0020,
                UDS_HORZ        = 0x0040,
                UDS_NOTHOUSANDS = 0x0080,
                UDS_HOTTRACK    = 0x0100;
            }
        }

        #endregion
    }   
}