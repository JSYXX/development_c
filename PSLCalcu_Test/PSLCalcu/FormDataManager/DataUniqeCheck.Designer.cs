namespace PSLCalcu
{
    partial class DataUniqeCheck
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bt_check4tagnames = new System.Windows.Forms.Button();
            this.tb_tagnames = new System.Windows.Forms.TextBox();
            this.lb_tagnames = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dt_enddate = new System.Windows.Forms.DateTimePicker();
            this.dt_startdate = new System.Windows.Forms.DateTimePicker();
            this.dt_endtime = new System.Windows.Forms.DateTimePicker();
            this.dt_starttime = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bt_check4calcu = new System.Windows.Forms.Button();
            this.tb_calcuIndexs = new System.Windows.Forms.TextBox();
            this.lb_calcuIndexs = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.bt_check4tagids = new System.Windows.Forms.Button();
            this.tb_tagids = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_remark = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bt_check4tagnames);
            this.groupBox3.Controls.Add(this.tb_tagnames);
            this.groupBox3.Controls.Add(this.lb_tagnames);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox3.Location = new System.Drawing.Point(12, 122);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(552, 47);
            this.groupBox3.TabIndex = 40;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "按标签名称进行";
            // 
            // bt_check4tagnames
            // 
            this.bt_check4tagnames.Location = new System.Drawing.Point(470, 14);
            this.bt_check4tagnames.Name = "bt_check4tagnames";
            this.bt_check4tagnames.Size = new System.Drawing.Size(59, 26);
            this.bt_check4tagnames.TabIndex = 67;
            this.bt_check4tagnames.Text = ">> 检查";
            this.bt_check4tagnames.UseVisualStyleBackColor = true;
            this.bt_check4tagnames.Click += new System.EventHandler(this.bt_check4tagnames_Click);
            // 
            // tb_tagnames
            // 
            this.tb_tagnames.Location = new System.Drawing.Point(88, 18);
            this.tb_tagnames.Name = "tb_tagnames";
            this.tb_tagnames.Size = new System.Drawing.Size(368, 21);
            this.tb_tagnames.TabIndex = 36;
            // 
            // lb_tagnames
            // 
            this.lb_tagnames.Location = new System.Drawing.Point(6, 21);
            this.lb_tagnames.Name = "lb_tagnames";
            this.lb_tagnames.Size = new System.Drawing.Size(73, 18);
            this.lb_tagnames.TabIndex = 64;
            this.lb_tagnames.Text = "标签名称：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dt_enddate);
            this.groupBox1.Controls.Add(this.dt_startdate);
            this.groupBox1.Controls.Add(this.dt_endtime);
            this.groupBox1.Controls.Add(this.dt_starttime);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(552, 51);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "时间段选择";
            // 
            // dt_enddate
            // 
            this.dt_enddate.Location = new System.Drawing.Point(337, 19);
            this.dt_enddate.Name = "dt_enddate";
            this.dt_enddate.Size = new System.Drawing.Size(119, 21);
            this.dt_enddate.TabIndex = 53;
            this.dt_enddate.ValueChanged += new System.EventHandler(this.dt_enddate_ValueChanged);
            // 
            // dt_startdate
            // 
            this.dt_startdate.Location = new System.Drawing.Point(71, 19);
            this.dt_startdate.Name = "dt_startdate";
            this.dt_startdate.Size = new System.Drawing.Size(119, 21);
            this.dt_startdate.TabIndex = 51;
            this.dt_startdate.ValueChanged += new System.EventHandler(this.dt_startdate_ValueChanged);
            // 
            // dt_endtime
            // 
            this.dt_endtime.CustomFormat = "HH:mm:ss";
            this.dt_endtime.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.dt_endtime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_endtime.Location = new System.Drawing.Point(462, 19);
            this.dt_endtime.Name = "dt_endtime";
            this.dt_endtime.ShowUpDown = true;
            this.dt_endtime.Size = new System.Drawing.Size(73, 21);
            this.dt_endtime.TabIndex = 54;
            this.dt_endtime.ValueChanged += new System.EventHandler(this.dt_endtime_ValueChanged);
            // 
            // dt_starttime
            // 
            this.dt_starttime.CustomFormat = "HH:mm:ss";
            this.dt_starttime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_starttime.Location = new System.Drawing.Point(196, 19);
            this.dt_starttime.Name = "dt_starttime";
            this.dt_starttime.ShowUpDown = true;
            this.dt_starttime.Size = new System.Drawing.Size(73, 21);
            this.dt_starttime.TabIndex = 52;
            this.dt_starttime.ValueChanged += new System.EventHandler(this.dt_starttime_ValueChanged);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(6, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 18);
            this.label9.TabIndex = 50;
            this.label9.Text = "起始时间：";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(275, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 18);
            this.label2.TabIndex = 66;
            this.label2.Text = "截止时间：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bt_check4calcu);
            this.groupBox2.Controls.Add(this.tb_calcuIndexs);
            this.groupBox2.Controls.Add(this.lb_calcuIndexs);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(12, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(552, 47);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "按计算项进行";
            // 
            // bt_check4calcu
            // 
            this.bt_check4calcu.Location = new System.Drawing.Point(470, 14);
            this.bt_check4calcu.Name = "bt_check4calcu";
            this.bt_check4calcu.Size = new System.Drawing.Size(59, 26);
            this.bt_check4calcu.TabIndex = 66;
            this.bt_check4calcu.Text = ">> 检查";
            this.bt_check4calcu.UseVisualStyleBackColor = true;
            this.bt_check4calcu.Click += new System.EventHandler(this.bt_check4calcu_Click);
            // 
            // tb_calcuIndexs
            // 
            this.tb_calcuIndexs.Location = new System.Drawing.Point(88, 18);
            this.tb_calcuIndexs.Name = "tb_calcuIndexs";
            this.tb_calcuIndexs.Size = new System.Drawing.Size(368, 21);
            this.tb_calcuIndexs.TabIndex = 36;
            // 
            // lb_calcuIndexs
            // 
            this.lb_calcuIndexs.Location = new System.Drawing.Point(6, 21);
            this.lb_calcuIndexs.Name = "lb_calcuIndexs";
            this.lb_calcuIndexs.Size = new System.Drawing.Size(87, 18);
            this.lb_calcuIndexs.TabIndex = 64;
            this.lb_calcuIndexs.Text = "计算项序号：";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.bt_check4tagids);
            this.groupBox4.Controls.Add(this.tb_tagids);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox4.Location = new System.Drawing.Point(12, 175);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(552, 47);
            this.groupBox4.TabIndex = 68;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "按标签名称进行";
            // 
            // bt_check4tagids
            // 
            this.bt_check4tagids.Location = new System.Drawing.Point(470, 14);
            this.bt_check4tagids.Name = "bt_check4tagids";
            this.bt_check4tagids.Size = new System.Drawing.Size(59, 26);
            this.bt_check4tagids.TabIndex = 67;
            this.bt_check4tagids.Text = ">> 检查";
            this.bt_check4tagids.UseVisualStyleBackColor = true;
            this.bt_check4tagids.Click += new System.EventHandler(this.bt_check4tagids_Click);
            // 
            // tb_tagids
            // 
            this.tb_tagids.Location = new System.Drawing.Point(88, 18);
            this.tb_tagids.Name = "tb_tagids";
            this.tb_tagids.Size = new System.Drawing.Size(368, 21);
            this.tb_tagids.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 18);
            this.label1.TabIndex = 64;
            this.label1.Text = "标签id：";
            // 
            // lb_remark
            // 
            this.lb_remark.Location = new System.Drawing.Point(10, 236);
            this.lb_remark.Name = "lb_remark";
            this.lb_remark.Size = new System.Drawing.Size(585, 18);
            this.lb_remark.TabIndex = 70;
            this.lb_remark.Text = "详细检查结果请参考output文件夹uniquecheckresult.txt文件.";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 254);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(585, 18);
            this.label3.TabIndex = 71;
            this.label3.Text = "查询重复用到group，性能有待优化";
            // 
            // DataUniqeCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 276);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lb_remark);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "DataUniqeCheck";
            this.Text = "数据唯一性检查";
            this.Load += new System.EventHandler(this.DataUniqeCheck_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bt_check4tagnames;
        private System.Windows.Forms.TextBox tb_tagnames;
        private System.Windows.Forms.Label lb_tagnames;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dt_enddate;
        private System.Windows.Forms.DateTimePicker dt_startdate;
        private System.Windows.Forms.DateTimePicker dt_endtime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bt_check4calcu;
        private System.Windows.Forms.TextBox tb_calcuIndexs;
        private System.Windows.Forms.Label lb_calcuIndexs;
        private System.Windows.Forms.DateTimePicker dt_starttime;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button bt_check4tagids;
        private System.Windows.Forms.TextBox tb_tagids;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_remark;
        private System.Windows.Forms.Label label3;
    }
}