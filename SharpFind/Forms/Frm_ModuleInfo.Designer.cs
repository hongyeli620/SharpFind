namespace SharpFind.Forms
{
    partial class Frm_ModuleInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GB_Summary = new System.Windows.Forms.GroupBox();
            this.TC_Details = new System.Windows.Forms.TabControl();
            this.TP_Module = new System.Windows.Forms.TabPage();
            this.TP_Thread = new System.Windows.Forms.TabPage();
            this.BTN_Close = new System.Windows.Forms.Button();
            this.separator1 = new SharpFind.Controls.Separator();
            this.PNL_Top = new System.Windows.Forms.Panel();
            this.PNL_Bottom = new System.Windows.Forms.Panel();
            this.TC_Details.SuspendLayout();
            this.PNL_Top.SuspendLayout();
            this.PNL_Bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // GB_Summary
            // 
            this.GB_Summary.Location = new System.Drawing.Point(8, 8);
            this.GB_Summary.Name = "GB_Summary";
            this.GB_Summary.Size = new System.Drawing.Size(424, 83);
            this.GB_Summary.TabIndex = 0;
            this.GB_Summary.TabStop = false;
            // 
            // TC_Details
            // 
            this.TC_Details.Controls.Add(this.TP_Module);
            this.TC_Details.Controls.Add(this.TP_Thread);
            this.TC_Details.Location = new System.Drawing.Point(8, 103);
            this.TC_Details.Name = "TC_Details";
            this.TC_Details.SelectedIndex = 0;
            this.TC_Details.Size = new System.Drawing.Size(424, 233);
            this.TC_Details.TabIndex = 1;
            // 
            // TP_Module
            // 
            this.TP_Module.Location = new System.Drawing.Point(4, 22);
            this.TP_Module.Name = "TP_Module";
            this.TP_Module.Padding = new System.Windows.Forms.Padding(3);
            this.TP_Module.Size = new System.Drawing.Size(416, 207);
            this.TP_Module.TabIndex = 0;
            this.TP_Module.Text = "Module Details";
            this.TP_Module.UseVisualStyleBackColor = true;
            // 
            // TP_Thread
            // 
            this.TP_Thread.Location = new System.Drawing.Point(4, 22);
            this.TP_Thread.Name = "TP_Thread";
            this.TP_Thread.Padding = new System.Windows.Forms.Padding(3);
            this.TP_Thread.Size = new System.Drawing.Size(408, 192);
            this.TP_Thread.TabIndex = 1;
            this.TP_Thread.Text = "Thread Details";
            this.TP_Thread.UseVisualStyleBackColor = true;
            // 
            // BTN_Close
            // 
            this.BTN_Close.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BTN_Close.Location = new System.Drawing.Point(357, 245);
            this.BTN_Close.Name = "BTN_Close";
            this.BTN_Close.Size = new System.Drawing.Size(75, 23);
            this.BTN_Close.TabIndex = 3;
            this.BTN_Close.Text = "&Close";
            this.BTN_Close.UseVisualStyleBackColor = true;
            this.BTN_Close.Click += new System.EventHandler(this.BTN_Close_Click);
            // 
            // separator1
            // 
            this.separator1.Location = new System.Drawing.Point(8, 251);
            this.separator1.Name = "separator1";
            this.separator1.Orientation = SharpFind.Controls.Separator._Orientation.Horizontal;
            this.separator1.Size = new System.Drawing.Size(343, 10);
            this.separator1.TabIndex = 2;
            this.separator1.Text = "separator1";
            // 
            // PNL_Top
            // 
            this.PNL_Top.Controls.Add(this.GB_Summary);
            this.PNL_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.PNL_Top.Location = new System.Drawing.Point(0, 0);
            this.PNL_Top.Name = "PNL_Top";
            this.PNL_Top.Padding = new System.Windows.Forms.Padding(5);
            this.PNL_Top.Size = new System.Drawing.Size(440, 97);
            this.PNL_Top.TabIndex = 4;
            // 
            // PNL_Bottom
            // 
            this.PNL_Bottom.Controls.Add(this.separator1);
            this.PNL_Bottom.Controls.Add(this.BTN_Close);
            this.PNL_Bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PNL_Bottom.Location = new System.Drawing.Point(0, 97);
            this.PNL_Bottom.Name = "PNL_Bottom";
            this.PNL_Bottom.Size = new System.Drawing.Size(440, 276);
            this.PNL_Bottom.TabIndex = 5;
            // 
            // Frm_ModuleInfo
            // 
            this.AcceptButton = this.BTN_Close;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 373);
            this.Controls.Add(this.TC_Details);
            this.Controls.Add(this.PNL_Bottom);
            this.Controls.Add(this.PNL_Top);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_ModuleInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Module Info";
            this.TC_Details.ResumeLayout(false);
            this.PNL_Top.ResumeLayout(false);
            this.PNL_Bottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GB_Summary;
        private System.Windows.Forms.TabControl TC_Details;
        private System.Windows.Forms.TabPage TP_Module;
        private System.Windows.Forms.TabPage TP_Thread;
        private Controls.Separator separator1;
        private System.Windows.Forms.Button BTN_Close;
        private System.Windows.Forms.Panel PNL_Top;
        private System.Windows.Forms.Panel PNL_Bottom;
    }
}