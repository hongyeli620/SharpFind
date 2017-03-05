using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        private void Frm_ModuleInfo_Load(object sender, EventArgs e)
        {
            GetModuleSummary();

            if (LBL_Path_R.Text != string.Empty)
                GetModuleDetails();
        }

        private void LNKLBL_Explore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowFileInExplorer(LBL_Path_R.Text);
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

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
        /// Formats the given numeric value into KB, GB, etc.
        /// </summary>
        /// 
        /// <param name="byteCount">
        /// Number of bytes to be formatted.
        /// </param>
        private static string FormatByteSize(long byteCount)
        {
            var sb = new StringBuilder(12);
            NativeMethods.StrFormatByteSize(byteCount, sb, sb.Capacity);

            return sb.ToString();
        }

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

                lvi.SubItems.Add("0x" + pm.BaseAddress.ToString("X8"));

                var length = new FileInfo(pm.FileName).Length;
                lvi.SubItems.Add(FormatByteSize(length));
                LV_Module.Items.Add(lvi);
            }

            LV_Module.Items[0].ForeColor = Color.Brown;
            LV_Module.Sorting = SortOrder.Ascending;
        }
    }
}