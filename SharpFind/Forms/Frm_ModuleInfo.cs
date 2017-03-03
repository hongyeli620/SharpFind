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
        }

        private void BTN_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LNKLBL_Explore_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowFileInExplorer(LBL_Path_R.Text);
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
                PB_Icon.Image  = moduleIcon.ToBitmap();

                var moduleCount = p.Modules.Count;
                var threadCount = p.Threads.Count;

                LBL_PID_R.Text     = Convert.ToString(p.Id);
                LBL_Modules_R.Text = $"{moduleCount} modules attached; {threadCount} threads";
                LBL_Path_R.Text    = p.MainModule.FileName; 
            }
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