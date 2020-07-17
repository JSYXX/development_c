namespace PSLCalcu
{
    partial class DataDelete
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bt_delelt4calcu = new System.Windows.Forms.Button();
            this.bt_export4calcu = new System.Windows.Forms.Button();
            this.tb_calcuIndexs = new System.Windows.Forms.TextBox();
            this.lb_calcuIndexs = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dt_enddate = new System.Windows.Forms.DateTimePicker();
            this.dt_startdate = new System.Windows.Forms.DateTimePicker();
            this.dt_endtime = new System.Windows.Forms.DateTimePicker();
            this.dt_starttime = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bt_export4tags = new System.Windows.Forms.Button();
            this.bt_delelt4tags = new System.Windows.Forms.Button();
            this.tb_tagnames = new System.Windows.Forms.TextBox();
            this.lb_tagnames = new System.Windows.Forms.Label();
            this.lb_result = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bt_delelt4calcu);
            this.groupBox2.Controls.Add(this.bt_export4calcu);
            this.groupBox2.Controls.Add(this.tb_calcuIndexs);
            this.groupBox2.Controls.Add(this.lb_calcuIndexs);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(12, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(591, 47);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "按计算项进行";
            // 
            // bt_delelt4calcu
            // 
            this.bt_delelt4calcu.Location = new System.Drawing.Point(462, 14);
            this.bt_delelt4calcu.Name = "bt_delelt4calcu";
            this.bt_delelt4calcu.Size = new System.Drawing.Size(59, 26);
            this.bt_delelt4calcu.TabIndex = 65;
            this.bt_delelt4calcu.Text = ">> 删除";
            this.bt_delelt4calcu.UseVisualStyleBackColor = true;
            this.bt_delelt4calcu.Click += new System.EventHandler(this.bt_delelt4calcu_Click);
            // 
            // bt_export4calcu
            // 
            this.bt_export4calcu.Location = new System.Drawing.Point(524, 14);
            this.bt_export4calcu.Name = "bt_export4calcu";
            this.bt_export4calcu.Size = new System.Drawing.Size(59, 26);
            this.bt_export4calcu.TabIndex = 66;
            this.bt_export4calcu.Text = ">> 导出";
            this.bt_export4calcu.UseVisualStyleBackColor = true;
            this.bt_export4calcu.Click += new System.EventHandler(this.bt_export4calcu_Click);
            // 
            // tb_calcuIndexs
            // 
            this.tb_calcuIndexs.Location = new System.Drawing.Point(88, 18);
            this.tb_calcuIndexs.Name = "tb_calcuIndexs";
            this.tb_calcuIndexs.Size = new System.Drawing.Size(368, 21);
            this.tb_calcuIndexs.TabIndex = 36;
            this.tb_calcuIndexs.TextChanged += new System.EventHandler(this.tb_calcuIndexs_TextChanged);
            // 
            // lb_calcuIndexs
            // 
            this.lb_calcuIndexs.Location = new System.Drawing.Point(6, 21);
            this.lb_calcuIndexs.Name = "lb_calcuIndexs";
            this.lb_calcuIndexs.Size = new System.Drawing.Size(87, 18);
            this.lb_calcuIndexs.TabIndex = 64;
            this.lb_calcuIndexs.Text = "计算项序号：";
            this.lb_calcuIndexs.MouseHover += new System.EventHandler(this.lb_calcuIndexs_MouseHover);
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
            this.groupBox1.TabIndex = 36;
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bt_export4tags);
            this.groupBox3.Controls.Add(this.bt_delelt4tags);
            this.groupBox3.Controls.Add(this.tb_tagnames);
            this.groupBox3.Controls.Add(this.lb_tagnames);
            this.groupBox3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox3.Location = new System.Drawing.Point(12, 122);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(591, 47);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "按标签名称进行";
            // 
            // bt_export4tags
            // 
            this.bt_export4tags.Location = new System.Drawing.Point(524, 14);
            this.bt_export4tags.Name = "bt_export4tags";
            this.bt_export4tags.Size = new System.Drawing.Size(59, 26);
            this.bt_export4tags.TabIndex = 67;
            this.bt_export4tags.Text = ">> 导出";
            this.bt_export4tags.UseVisualStyleBackColor = true;
            this.bt_export4tags.Click += new System.EventHandler(this.bt_export4tags_Click);
            // 
            // bt_delelt4tags
            // 
            this.bt_delelt4tags.Location = new System.Drawing.Point(462, 14);
            this.bt_delelt4tags.Name = "bt_delelt4tags";
            this.bt_delelt4tags.Size = new System.Drawing.Size(59, 26);
            this.bt_delelt4tags.TabIndex = 65;
            this.bt_delelt4tags.Text = ">> 删除";
            this.bt_delelt4tags.UseVisualStyleBackColor = true;
            this.bt_delelt4tags.Click += new System.EventHandler(this.bt_delelt4tags_Click);
            // 
            // tb_tagnames
            // 
            this.tb_tagnames.Location = new System.Drawing.Point(88, 18);
            this.tb_tagnames.Name = "tb_tagnames";
            this.tb_tagnames.Size = new System.Drawing.Size(368, 21);
            this.tb_tagnames.TabIndex = 36;
            this.tb_tagnames.TextChanged += new System.EventHandler(this.tb_tagnames_TextChanged);
            // 
            // lb_tagnames
            // 
            this.lb_tagnames.Location = new System.Drawing.Point(6, 21);
            this.lb_tagnames.Name = "lb_tagnames";
            this.lb_tagnames.Size = new System.Drawing.Size(73, 18);
            this.lb_tagnames.TabIndex = 64;
            this.lb_tagnames.Text = "标签名称：";
            this.lb_tagnames.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lb_tagnames_MouseMove);
            // 
            // lb_result
            // 
            this.lb_result.Location = new System.Drawing.Point(12, 182);
            this.lb_result.Name = "lb_result";
            this.lb_result.Size = new System.Drawing.Size(585, 18);
            this.lb_result.TabIndex = 65;
            this.lb_result.Text = "共删除0条记录";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(585, 18);
            this.label1.TabIndex = 66;
            this.label1.Text = "删除说明:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(10, 237);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(585, 18);
            this.label3.TabIndex = 67;
            this.label3.Text = "—删除的是相关标签时间戳>=startdate且<enddate的数据。这点与并发结算生成的结果相对应。";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(10, 255);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(585, 18);
            this.label4.TabIndex = 68;
            this.label4.Text = "—";
            // 
            // DataDelete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 302);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lb_result);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "DataDelete";
            this.Text = "数据删除和导出";
            this.Load += new System.EventHandler(this.DataDelete_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lb_calcuIndexs;
        private System.Windows.Forms.TextBox tb_calcuIndexs;
        private System.Windows.Forms.Button bt_delelt4calcu;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dt_enddate;
        private System.Windows.Forms.DateTimePicker dt_startdate;
        private System.Windows.Forms.DateTimePicker dt_endtime;
        private System.Windows.Forms.DateTimePicker dt_starttime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bt_delelt4tags;
        private System.Windows.Forms.TextBox tb_tagnames;
        private System.Windows.Forms.Label lb_tagnames;
        private System.Windows.Forms.Label lb_result;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bt_export4calcu;
        private System.Windows.Forms.Button bt_export4tags;
    }
}