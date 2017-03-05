using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;

namespace SharpFind.Forms
{
    public partial class Frm_ModuleInfo : Form
    {
        public Frm_ModuleInfo()
        {
            InitializeComponent();
        }

        private void Frm_ModuleInfo_Load(object sender, EventArgs e)
        {
            GetModuleSummary();
            GetModuleDetails();
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LNKLBL_Explore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowFileInExplorer(LBL_Path_R.Text);
        }

        private Process parentValue;
        private Process ParentProcess
        {
            get { return parentValue;  }
            set { parentValue = value; }
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

        private static string GetFormattedBytes(long byteCount)
        {
            string[] format = { "B", "KB", "MB", "GB" };
            decimal length = byteCount;
            int i = 0;

            if (byteCount == 0)
                return "0" + format[0];

            while (length > 1024)
            {
                length = decimal.Round(length / 1024, 2);
                i += 1;
                if (i >= format.Length - 1)
                    break;
            }

            return length.ToString() + " " + format[i];
        }

        private void GetModuleDetails()
        {
            ProcessModule pm;
            ProcessModuleCollection pmc = parentValue.Modules;

            for (int i = 0; i < pmc.Count; i++)
            {
                pm = pmc[i];

                var lvi = new ListViewItem(pm.ModuleName);
                lvi.ToolTipText = pm.FileName + "\n" + 
                                  pm.FileVersionInfo.LegalCopyright  + "\n" + 
                                  pm.FileVersionInfo.FileDescription + "\n" + 
                                  pm.FileVersionInfo.ProductVersion;

                lvi.SubItems.Add("0x" + pm.BaseAddress.ToString("x8"));

                var length = new FileInfo(pm.FileName).Length;
                lvi.SubItems.Add(GetFormattedBytes(length));
                LV_Module.Items.Add(lvi);
            }

            LV_Module.Items[0].ForeColor = Color.Brown;
            LV_Module.Sorting = SortOrder.Ascending;
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
    }
}