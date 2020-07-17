using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSLCalcu
{
    partial class LogTest : Form
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
            this.cb_Debug = new System.Windows.Forms.CheckBox();
            this.cb_Info = new System.Windows.Forms.CheckBox();
            this.cb_Warn = new System.Windows.Forms.CheckBox();
            this.cb_error = new System.Windows.Forms.CheckBox();
            this.cb_fatal = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_exportCSV = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cb_Debug
            // 
            this.cb_Debug.AutoSize = true;
            this.cb_Debug.Location = new System.Drawing.Point(27, 28);
            this.cb_Debug.Name = "cb_Debug";
            this.cb_Debug.Size = new System.Drawing.Size(54, 16);
            this.cb_Debug.TabIndex = 29;
            this.cb_Debug.Text = "Debug";
            this.cb_Debug.UseVisualStyleBackColor = true;
            // 
            // cb_Info
            // 
            this.cb_Info.AutoSize = true;
            this.cb_Info.Location = new System.Drawing.Point(27, 50);
            this.cb_Info.Name = "cb_Info";
            this.cb_Info.Size = new System.Drawing.Size(48, 16);
            this.cb_Info.TabIndex = 30;
            this.cb_Info.Text = "Info";
            this.cb_Info.UseVisualStyleBackColor = true;
            // 
            // cb_Warn
            // 
            this.cb_Warn.AutoSize = true;
            this.cb_Warn.Location = new System.Drawing.Point(27, 72);
            this.cb_Warn.Name = "cb_Warn";
            this.cb_Warn.Size = new System.Drawing.Size(48, 16);
            this.cb_Warn.TabIndex = 31;
            this.cb_Warn.Text = "Warn";
            this.cb_Warn.UseVisualStyleBackColor = true;
            // 
            // cb_error
            // 
            this.cb_error.AutoSize = true;
            this.cb_error.Location = new System.Drawing.Point(27, 94);
            this.cb_error.Name = "cb_error";
            this.cb_error.Size = new System.Drawing.Size(54, 16);
            this.cb_error.TabIndex = 32;
            this.cb_error.Text = "Error";
            this.cb_error.UseVisualStyleBackColor = true;
            // 
            // cb_fatal
            // 
            this.cb_fatal.AutoSize = true;
            this.cb_fatal.Location = new System.Drawing.Point(27, 116);
            this.cb_fatal.Name = "cb_fatal";
            this.cb_fatal.Size = new System.Drawing.Size(54, 16);
            this.cb_fatal.TabIndex = 33;
            this.cb_fatal.Text = "Fatal";
            this.cb_fatal.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 12);
            this.label1.TabIndex = 34;
            this.label1.Text = "选择要测试写入的报警级别：";
            // 
            // btn_exportCSV
            // 
            this.btn_exportCSV.Location = new System.Drawing.Point(27, 138);
            this.btn_exportCSV.Name = "btn_exportCSV";
            this.btn_exportCSV.Size = new System.Drawing.Size(75, 23);
            this.btn_exportCSV.TabIndex = 35;
            this.btn_exportCSV.Text = "开始写入";
            this.btn_exportCSV.UseVisualStyleBackColor = true;
            this.btn_exportCSV.Click += new System.EventHandler(this.btn_exportCSV_Click);
            // 
            // LogTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 190);
            this.Controls.Add(this.btn_exportCSV);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_fatal);
            this.Controls.Add(this.cb_error);
            this.Controls.Add(this.cb_Warn);
            this.Controls.Add(this.cb_Info);
            this.Controls.Add(this.cb_Debug);
            this.Name = "LogTest";
            this.Text = "LogTest";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_Debug;
        private System.Windows.Forms.CheckBox cb_Info;
        private System.Windows.Forms.CheckBox cb_Warn;
        private System.Windows.Forms.CheckBox cb_error;
        private System.Windows.Forms.CheckBox cb_fatal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_exportCSV;
    }
}