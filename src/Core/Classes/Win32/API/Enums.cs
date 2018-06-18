using System;

namespace SharpFind
{
    public partial class Win32
    {
        /// <remarks>Winuser.h</remarks>
        /// <summary>
        /// Index value is used by <see cref="GetClassLong(IntPtr, ClassLongIndex)"/>.
        /// </summary>
        public enum ClassLongIndex : int
        {
            GCL_HBRBACKGROUND = -10,
            GCL_HCURSOR       = -12,
            GCL_HICON         = -14,
            GCL_CBWNDEXTRA    = -18,
            GCL_CBCLSEXTRA    = -20,
            GCL_WNDPROC       = -24,
            GCL_STYLE         = -26,
            GCW_ATOM          = -32,
            GCL_HICONSM       = -34,
        }

        /// <remarks>Winuser.h</remarks>
        /// <summary>
        /// The flags are used by
        /// <see cref="InsertMenu(IntPtr, uint, uint, uint, string)"/>.
        /// </summary>
        [Flags()]
        public enum InsertMenuFlags : uint
        {
            MF_POPUP      = 0x010,
            MF_BYPOSITION = 0x400,
            MF_SEPARATOR  = 0x800
        }
        
        /// <summary>
        /// The priority class of a process. The value is returned by
        /// <see cref="GetPriorityClass(IntPtr)"/>.
        /// </summary>
        public enum ProcessPriorityClass : uint
        {
            NORMAL_PRIORITY_CLASS       = 0x20,
            IDLE_PRIORITY_CLASS         = 0x40,
            HIGH_PRIORITY_CLASS         = 0x80,
            REALTIME_PRIORITY_CLASS     = 0x100,
            BELOW_NORMAL_PRIORITY_CLASS = 0x4000,
            ABOVE_NORMAL_PRIORITY_CLASS = 0x8000,
        }

        /// <remarks>Winuser.h</remarks>
        /// <summary>
        /// The flags used by
        /// <see cref="RedrawWindow(IntPtr, IntPtr, IntPtr, RedrawWindowFlags)"/>.
        /// </summary>
        [Flags()]
        public enum RedrawWindowFlags : uint
        {
            RDW_INVALIDATE      = 0x1,
            RDW_INTERNALPAINT   = 0x2,
            RDW_ERASE           = 0x4,
            RDW_VALIDATE        = 0x8,
            RDW_NOINTERNALPAINT = 0x10,
            RDW_NOERASE         = 0x20,
            RDW_NOCHILDREN      = 0x40,
            RDW_ALLCHILDREN     = 0x80,
            RDW_UPDATENOW       = 0x100,
            RDW_ERASENOW        = 0x200,
            RDW_FRAME           = 0x400,
            RDW_NOFRAME         = 0x800
        }

        /// <remarks>Winuser.h</remarks>
        /// <summary>
        /// The current show state of a specified window.  Part of the
        /// <see cref="WINDOWPLACEMENT"/> struct.
        /// </summary>
        public enum ShowWindowCommands
        {
            SW_HIDE            = 0,
            SW_SHOWNORMAL      = 1,
            SW_SHOWMINIMIZED   = 2,
            SW_MAXIMIZE        = 3,
            SW_MAXIMIZED       = 3,
            SW_SHOWNOACTIVATE  = 4,
            SW_SHOW            = 5,
            SW_MINIMIZE        = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA          = 8,
            SW_RESTORE         = 9
        }

        /// <remarks>TlHelp32.h</remarks>
        /// <summary>
        /// The snapshot flags used by 
        /// <see cref="CreateToolhelp32Snapshot(SnapshotFlags, int)"/>.
        /// </summary>
        [Flags]
        public enum SnapshotFlags : uint
        {
            TH32CS_INHERIT      = 0x80000000,
            TH32CS_SNAPALL      = TH32CS_SNAPHEAPLIST | TH32CS_SNAPMODULE  |
                                                        TH32CS_SNAPPROCESS |
                                                        TH32CS_SNAPTHREAD,
            TH32CS_SNAPHEAPLIST = 0x00000001,
            TH32CS_SNAPMODULE   = 0x00000008,
            TH32CS_SNAPMODULE32 = 0x00000010,
            TH32CS_SNAPPROCESS  = 0x00000002,
            TH32CS_SNAPTHREAD   = 0x00000004,
        }

        /// <remarks>Wingdi.h</remarks>
        /// <summary>
        /// Defines how the color data for the source rectangle is to be
        /// combined with the color data for the destination rectangle to
        /// achieve the final color.
        /// </summary>
        public enum RasterOperations : uint
        {
            BLACKNESS   = 0x00000042,
            DSTINVERT   = 0x00550009,
            PATCOPY     = 0x00F00021,
            PATINVERT   = 0x005A0049,
            WHITENESS   = 0x00FF0062
        }

        /// <summary>
        /// Thread-specific access rights.
        /// </summary>
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE            = 0x0001,
            SUSPEND_RESUME       = 0x0002,
            GET_CONTEXT          = 0x0008,
            SET_CONTEXT          = 0x0010,
            SET_INFORMATION      = 0x0020,
            QUERY_INFORMATION    = 0x0040,
            SET_THREAD_TOKEN     = 0x0080,
            IMPERSONATE          = 0x0100,
            DIRECT_IMPERSONATION = 0x0200
        }

        public enum THREADINFOCLASS : int
        {
            ThreadBasicInformation,          //  0
            ThreadTimes,                     //  1
            ThreadPriority,                  //  2
            ThreadBasePriority,              //  3
            ThreadAffinityMask,              //  4
            ThreadImpersonationToken,        //  5
            ThreadDescriptorTableEntry,      //  6
            ThreadEnableAlignmentFaultFixup, //  7
            ThreadEventPair,                 //  8
            ThreadQuerySetWin32StartAddress, //  9
            ThreadZeroTlsCell,               // 10
            ThreadPerformanceCount,          // 11
            ThreadAmILastThread,             // 12
            ThreadIdealProcessor,            // 13
            ThreadPriorityBoost,             // 14
            ThreadSetTlsArrayAddress,        // 15
            ThreadIsIoPending,               // 16
            ThreadHideFromDebugger           // 17
        }

        /// <remarks>Winuser.h</remarks>
        /// <summary>
        /// Index value used by <see cref="GetWindowLong(IntPtr, WindowLongIndex)"/>.
        /// </summary>
        public enum WindowLongIndex : int
        {
            GWL_HINSTANCE = -6,
            GWL_ID        = -12,
            GWL_STYLE     = -16,
            GWL_EXSTYLE   = -20,
            GWL_USERDATA  = -21
        }

        // The list is too big. No need to waste bytes. Only added the
        // ones used by the program.
        /// <remarks>Winuser.h</remarks>
        public enum WindowsMessages : uint
        {
            WM_LBUTTONUP     = 0x202,
            WM_MOUSEMOVE     = 0x200,
            WM_NCLBUTTONDOWN = 0xA1,
            WM_NULL          = 0x00,
            WM_PAINT         = 0xF,
            WM_SYSCOMMAND    = 0x112
        }
    }
}