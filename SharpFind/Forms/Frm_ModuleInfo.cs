using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System;
using SharpFind.Classes;

namespace SharpFind.Forms
{
    public partial class Frm_ModuleInfo : Form
    {
        public Frm_ModuleInfo()
        {
            InitializeComponent();
        }

        private Process ParentProcess { get; set; }

        #region Events

        private void Frm_ModuleInfo_Load(object sender, EventArgs e)
        {
            GetModuleSummary();

            if (LBL_Path_R.Text != string.Empty)
            {
                GetModuleDetails();
                GetThreadDetails();
            }
        }

        private void LNKLBL_Explore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowFileInExplorer(LBL_Path_R.Text);
        }

        private void TC_Details_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TC_Details.SelectedIndex)
            {
                case 0:
                    if (!LV_Module.Visible)
                    {
                        LV_Thread.Visible = false;
                        LV_Module.Visible = true;
                        LV_Module.BringToFront();
                    }
                    break;
                case 1:
                    if (!LV_Thread.Visible)
                    {
                        LV_Module.Visible = false;
                        LV_Thread.Visible = true;
                        LV_Thread.BringToFront();
                    }
                    break;
            }
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
        #region Functions

        /// <summary>
        /// Formats the given numeric value into KB, MB, GB, etc.
        /// </summary>
        /// 
        /// <param name="byteCount">
        /// Number of bytes to be formatted.
        /// </param>
        private static string FormatByteSize(long byteCount)
        {
            var sb = new StringBuilder(10);
            NativeMethods.StrFormatByteSize(byteCount, sb, sb.Capacity);

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="tid">
        /// Id of the thread.
        /// </param>
        private static IntPtr GetThreadStartAddress(int tid)
        {
            var hThread = NativeMethods.OpenThread(NativeMethods.ThreadAccess.QUERY_INFORMATION, false, tid);
            if (hThread == IntPtr.Zero)
                throw new Win32Exception("Unable to open thread");

            var dwStartAddress = Marshal.AllocHGlobal(IntPtr.Size);
            try
            {
                const int STATUS_SUCCESS = 0x0;
                const NativeMethods.THREADINFOCLASS flag = NativeMethods.THREADINFOCLASS.ThreadQuerySetWin32StartAddress;
                var ntStatus = NativeMethods.NtQueryInformationThread(hThread, flag, dwStartAddress, IntPtr.Size, IntPtr.Zero);
                if (ntStatus != STATUS_SUCCESS)
                    throw new Win32Exception($"NtQueryInformationThread failure. NTSTATUS returns 0x{ntStatus:X4}");

                return Marshal.ReadIntPtr(dwStartAddress);
            }
            finally
            {
                NativeMethods.CloseHandle(hThread);
                Marshal.FreeHGlobal(dwStartAddress);
            }
        }

        /// <summary>
        /// Returns the actual start address of a thread.
        /// </summary>
        /// 
        /// <param name="p">
        /// The target process that is used to retrieve information about the
        /// modules.
        /// </param>
        /// 
        /// <param name="startAddress">
        /// Address of function's first byte in target process.
        /// </param>
        private static string GetThreadStartAddress(Process p, long startAddress)
        {
            foreach (ProcessModule pm in p.Modules)
            {
                if (startAddress >= (long)pm.BaseAddress && startAddress <= (long)pm.BaseAddress + pm.ModuleMemorySize)
                    return pm.ModuleName + "+0x" + (startAddress - (long)pm.BaseAddress).ToString("X2");
            }
            return string.Empty;
        }

        #endregion
        #region Methods

        /// <summary>
        /// Opens the directory of the designated file in Explorer and selects it.
        /// </summary>
        /// 
        /// <param name="path">
        /// Path to the file.
        /// </param>
        private static void ShowFileInExplorer(string path)
        {
            var winDir = Environment.GetEnvironmentVariable("windir");
            if (winDir == null)
                return;

            var explorer = Path.Combine(winDir, "explorer.exe");
            var args = $"/select, {"\"" + path + "\""}";
            Process.Start(explorer, args);
        }

        /// <summary>
        /// Retrieves very minimal information about the specified process.
        /// </summary>
        private void GetModuleSummary()
        {
            var pNoExt = Path.GetFileNameWithoutExtension(LBL_Process_R.Text);
            foreach (var p in Process.GetProcessesByName(pNoExt))
            {
                if (p.ProcessName == "Idle" || p.ProcessName == "System")
                    return;

                // Retrieve the process icon
                var moduleIcon = Icon.ExtractAssociatedIcon(p.MainModule.FileName);
                PB_Icon.Image  = moduleIcon?.ToBitmap();

                var moduleCount = p.Modules.Count;
                var threadCount = p.Threads.Count;

                LBL_PID_R.Text     = Convert.ToString(p.Id);
                LBL_Modules_R.Text = $"{moduleCount} modules attached; {threadCount} threads";
                LBL_Path_R.Text    = p.MainModule.FileName;

                ParentProcess = p;
            }
        }

        /// <summary>
        /// Retrieves the names of all loaded modules in the process, their base
        /// address and size on disk.
        /// </summary>
        private void GetModuleDetails()
        {
            var pmc = ParentProcess.Modules;
            for (var i = 0; i < pmc.Count; i++)
            {
                var pm = pmc[i];
                var lvi = new ListViewItem(pm.ModuleName)
                {
                    ToolTipText = pm.FileName + "\n" +
                                  pm.FileVersionInfo.LegalCopyright  + "\n" +
                                  pm.FileVersionInfo.FileDescription + "\n" +
                                  pm.FileVersionInfo.ProductVersion
                };

                var length = new FileInfo(pm.FileName).Length;
                lvi.SubItems.Add("0x" + pm.BaseAddress.ToString("X4"));
                lvi.SubItems.Add(FormatByteSize(length));
                LV_Module.Items.Add(lvi);
            }

            LV_Module.Items[0].BackColor = SystemColors.GradientActiveCaption;
            LV_Module.Sorting = SortOrder.Ascending;
        }

        /// <summary>
        /// Retrieves the thread Ids, starting address, and priority level of
        /// the specified process.
        /// </summary>
        private void GetThreadDetails()
        {
            foreach (ProcessThread pt in ParentProcess.Threads)
            {
                var thread  = pt;
                var lvi     = new ListViewItem(thread.Id.ToString());
                var address = (long)GetThreadStartAddress(thread.Id);

                lvi.SubItems.Add(GetThreadStartAddress(ParentProcess, address));
                lvi.SubItems.Add(thread.PriorityLevel.ToString());
                LV_Thread.Items.Add(lvi);
            }
        }

        #endregion
    }
}