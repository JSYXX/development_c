namespace PSLCalcu
{
    partial class CheckCurve
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
            this.label1 = new System.Windows.Forms.Label();
            this.tb_readstatus = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_select = new System.Windows.Forms.ComboBox();
            this.tc_Curve = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tb_scorestatus = new System.Windows.Forms.TextBox();
            this.tb_errorstatus = new System.Windows.Forms.TextBox();
            this.tb_spstatus = new System.Windows.Forms.TextBox();
            this.lb_vailddate = new System.Windows.Forms.Label();
            this.lb_modulename = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tb_output = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tb_score = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tb_error = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_sp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bt_calcu = new System.Windows.Forms.Button();
            this.dt_startdate = new System.Windows.Forms.DateTimePicker();
            this.dt_starttime = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_input2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_input1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_biascurveinfo = new System.Windows.Forms.TextBox();
            this.cb_biascurve = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tc_Curve.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(485, 12);
            this.label1.TabIndex = 60;
            this.label1.Text = "本界面一运行，曲线静态对象即完成初始化。静态对象仅在第一次使用时才会进行初始化。";
            // 
            // tb_readstatus
            // 
            this.tb_readstatus.Location = new System.Drawing.Point(8, 44);
            this.tb_readstatus.Multiline = true;
            this.tb_readstatus.Name = "tb_readstatus";
            this.tb_readstatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_readstatus.Size = new System.Drawing.Size(533, 248);
            this.tb_readstatus.TabIndex = 61;
            this.tb_readstatus.TextChanged += new System.EventHandler(this.tb_readstatus_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 62;
            this.label2.Text = "选择曲线：";
            // 
            // cb_select
            // 
            this.cb_select.FormattingEnabled = true;
            this.cb_select.Location = new System.Drawing.Point(66, 22);
            this.cb_select.Name = "cb_select";
            this.cb_select.Size = new System.Drawing.Size(104, 20);
            this.cb_select.TabIndex = 63;
            this.cb_select.SelectedIndexChanged += new System.EventHandler(this.cb_select_SelectedIndexChanged);
            // 
            // tc_Curve
            // 
            this.tc_Curve.Controls.Add(this.tabPage1);
            this.tc_Curve.Controls.Add(this.tabPage2);
            this.tc_Curve.Location = new System.Drawing.Point(0, 3);
            this.tc_Curve.Name = "tc_Curve";
            this.tc_Curve.SelectedIndex = 0;
            this.tc_Curve.Size = new System.Drawing.Size(580, 411);
            this.tc_Curve.TabIndex = 64;
            this.tc_Curve.Tag = "";
            this.tc_Curve.SelectedIndexChanged += new System.EventHandler(this.tc_Curve_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.tb_readstatus);
            this.tabPage1.Controls.Add(this.cb_select);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(572, 385);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "曲线读取情况";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.tb_scorestatus);
            this.tabPage2.Controls.Add(this.tb_errorstatus);
            this.tabPage2.Controls.Add(this.tb_spstatus);
            this.tabPage2.Controls.Add(this.lb_vailddate);
            this.tabPage2.Controls.Add(this.lb_modulename);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.tb_output);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.tb_score);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.tb_error);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.tb_sp);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.bt_calcu);
            this.tabPage2.Controls.Add(this.dt_startdate);
            this.tabPage2.Controls.Add(this.dt_starttime);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.tb_input2);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.tb_input1);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.tb_biascurveinfo);
            this.tabPage2.Controls.Add(this.cb_biascurve);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(572, 385);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "期望曲线计算测试";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(352, 358);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 12);
            this.label15.TabIndex = 92;
            this.label15.Text = "状态：";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(188, 358);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 12);
            this.label14.TabIndex = 91;
            this.label14.Text = "状态：";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(28, 358);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 12);
            this.label13.TabIndex = 90;
            this.label13.Text = "状态：";
            // 
            // tb_scorestatus
            // 
            this.tb_scorestatus.Location = new System.Drawing.Point(399, 355);
            this.tb_scorestatus.Name = "tb_scorestatus";
            this.tb_scorestatus.ReadOnly = true;
            this.tb_scorestatus.Size = new System.Drawing.Size(104, 21);
            this.tb_scorestatus.TabIndex = 89;
            // 
            // tb_errorstatus
            // 
            this.tb_errorstatus.Location = new System.Drawing.Point(235, 355);
            this.tb_errorstatus.Name = "tb_errorstatus";
            this.tb_errorstatus.ReadOnly = true;
            this.tb_errorstatus.Size = new System.Drawing.Size(98, 21);
            this.tb_errorstatus.TabIndex = 88;
            // 
            // tb_spstatus
            // 
            this.tb_spstatus.Location = new System.Drawing.Point(72, 355);
            this.tb_spstatus.Name = "tb_spstatus";
            this.tb_spstatus.ReadOnly = true;
            this.tb_spstatus.Size = new System.Drawing.Size(98, 21);
            this.tb_spstatus.TabIndex = 87;
            // 
            // lb_vailddate
            // 
            this.lb_vailddate.AutoSize = true;
            this.lb_vailddate.Location = new System.Drawing.Point(330, 292);
            this.lb_vailddate.Name = "lb_vailddate";
            this.lb_vailddate.Size = new System.Drawing.Size(77, 12);
            this.lb_vailddate.TabIndex = 86;
            this.lb_vailddate.Text = "计算组件名称";
            // 
            // lb_modulename
            // 
            this.lb_modulename.AutoSize = true;
            this.lb_modulename.Location = new System.Drawing.Point(141, 292);
            this.lb_modulename.Name = "lb_modulename";
            this.lb_modulename.Size = new System.Drawing.Size(77, 12);
            this.lb_modulename.TabIndex = 85;
            this.lb_modulename.Text = "计算组件名称";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(275, 231);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(287, 12);
            this.label12.TabIndex = 84;
            this.label12.Text = "二维期望曲线下，参考量是X、Y，被参考量期望值是Z";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(239, 216);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(305, 12);
            this.label11.TabIndex = 83;
            this.label11.Text = "说明：一维期望曲线下，参考量是X，被参考量期望值是Y";
            // 
            // tb_output
            // 
            this.tb_output.Location = new System.Drawing.Point(72, 221);
            this.tb_output.Name = "tb_output";
            this.tb_output.Size = new System.Drawing.Size(119, 21);
            this.tb_output.TabIndex = 82;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 224);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 81;
            this.label10.Text = "被考核量：";
            // 
            // tb_score
            // 
            this.tb_score.Location = new System.Drawing.Point(399, 328);
            this.tb_score.Name = "tb_score";
            this.tb_score.ReadOnly = true;
            this.tb_score.Size = new System.Drawing.Size(104, 21);
            this.tb_score.TabIndex = 80;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(340, 331);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 79;
            this.label9.Text = "得分值：";
            // 
            // tb_error
            // 
            this.tb_error.Location = new System.Drawing.Point(235, 328);
            this.tb_error.Name = "tb_error";
            this.tb_error.ReadOnly = true;
            this.tb_error.Size = new System.Drawing.Size(98, 21);
            this.tb_error.TabIndex = 78;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(176, 331);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 77;
            this.label8.Text = "偏差值：";
            // 
            // tb_sp
            // 
            this.tb_sp.Location = new System.Drawing.Point(72, 328);
            this.tb_sp.Name = "tb_sp";
            this.tb_sp.ReadOnly = true;
            this.tb_sp.Size = new System.Drawing.Size(98, 21);
            this.tb_sp.TabIndex = 76;
            this.tb_sp.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 331);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 75;
            this.label7.Text = "期望值：";
            // 
            // bt_calcu
            // 
            this.bt_calcu.Location = new System.Drawing.Point(18, 285);
            this.bt_calcu.Name = "bt_calcu";
            this.bt_calcu.Size = new System.Drawing.Size(101, 26);
            this.bt_calcu.TabIndex = 74;
            this.bt_calcu.Text = "开始计算--->";
            this.bt_calcu.UseVisualStyleBackColor = true;
            this.bt_calcu.Click += new System.EventHandler(this.bt_calcu_Click);
            // 
            // dt_startdate
            // 
            this.dt_startdate.Location = new System.Drawing.Point(72, 248);
            this.dt_startdate.Name = "dt_startdate";
            this.dt_startdate.Size = new System.Drawing.Size(119, 21);
            this.dt_startdate.TabIndex = 72;
            this.dt_startdate.ValueChanged += new System.EventHandler(this.dt_startdate_ValueChanged);
            // 
            // dt_starttime
            // 
            this.dt_starttime.CustomFormat = "HH:mm:ss";
            this.dt_starttime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_starttime.Location = new System.Drawing.Point(197, 248);
            this.dt_starttime.Name = "dt_starttime";
            this.dt_starttime.ShowUpDown = true;
            this.dt_starttime.Size = new System.Drawing.Size(84, 21);
            this.dt_starttime.TabIndex = 73;
            this.dt_starttime.ValueChanged += new System.EventHandler(this.dt_starttime_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 254);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 71;
            this.label6.Text = "计算时间：";
            // 
            // tb_input2
            // 
            this.tb_input2.Location = new System.Drawing.Point(252, 191);
            this.tb_input2.Name = "tb_input2";
            this.tb_input2.Size = new System.Drawing.Size(95, 21);
            this.tb_input2.TabIndex = 70;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(197, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 69;
            this.label5.Text = "参考量2：";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // tb_input1
            // 
            this.tb_input1.Location = new System.Drawing.Point(72, 191);
            this.tb_input1.Name = "tb_input1";
            this.tb_input1.Size = new System.Drawing.Size(119, 21);
            this.tb_input1.TabIndex = 68;
            this.tb_input1.TextChanged += new System.EventHandler(this.tb_calcuIndexs_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 194);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 67;
            this.label4.Text = "参考量1：";
            // 
            // tb_biascurveinfo
            // 
            this.tb_biascurveinfo.Location = new System.Drawing.Point(18, 42);
            this.tb_biascurveinfo.Multiline = true;
            this.tb_biascurveinfo.Name = "tb_biascurveinfo";
            this.tb_biascurveinfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_biascurveinfo.Size = new System.Drawing.Size(533, 143);
            this.tb_biascurveinfo.TabIndex = 66;
            this.tb_biascurveinfo.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // cb_biascurve
            // 
            this.cb_biascurve.FormattingEnabled = true;
            this.cb_biascurve.Location = new System.Drawing.Point(93, 12);
            this.cb_biascurve.Name = "cb_biascurve";
            this.cb_biascurve.Size = new System.Drawing.Size(288, 20);
            this.cb_biascurve.TabIndex = 65;
            this.cb_biascurve.SelectedIndexChanged += new System.EventHandler(this.cb_biascurve_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 64;
            this.label3.Text = "选择期望曲线：";
            // 
            // CheckCurve
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 417);
            this.Controls.Add(this.tc_Curve);
            this.Name = "CheckCurve";
            this.Text = "CheckCurve";
            this.Load += new System.EventHandler(this.CheckCurve_Load);
            this.tc_Curve.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_readstatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cb_select;
        private System.Windows.Forms.TabControl tc_Curve;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_biascurveinfo;
        private System.Windows.Forms.ComboBox cb_biascurve;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_input2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_input1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dt_startdate;
        private System.Windows.Forms.DateTimePicker dt_starttime;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bt_calcu;
        private System.Windows.Forms.TextBox tb_score;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tb_error;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_sp;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tb_output;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lb_modulename;
        private System.Windows.Forms.Label lb_vailddate;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tb_scorestatus;
        private System.Windows.Forms.TextBox tb_errorstatus;
        private System.Windows.Forms.TextBox tb_spstatus;
    }
}