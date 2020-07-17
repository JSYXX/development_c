namespace PSLCalcu
{
    partial class Setup
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
            this.lb_period = new System.Windows.Forms.Label();
            this.tb_CalcuPeriod = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_resulttag = new System.Windows.Forms.Label();
            this.rb_TagAuto = new System.Windows.Forms.RadioButton();
            this.rb_TagManual = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bt_save = new System.Windows.Forms.Button();
            this.cbIniTableIncludePSLData = new System.Windows.Forms.CheckBox();
            this.cbTag2IdMapReset = new System.Windows.Forms.CheckBox();
            this.cbCalcuTime = new System.Windows.Forms.CheckBox();
            this.cbAutoRun = new System.Windows.Forms.CheckBox();
            this.tab_Calcu = new System.Windows.Forms.TabControl();
            this.数据库及计算配置 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbEndYear = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbStartYear = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rb_ExcludeIntervalType = new System.Windows.Forms.RadioButton();
            this.rb_IncludeIntervalType = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.实时计算引擎 = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.tb_saveNumber = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_WritePeriod = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btSaveCalcu = new System.Windows.Forms.Button();
            this.tbMaxReadRTDB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.历史计算引擎 = new System.Windows.Forms.TabPage();
            this.bt_savehistorycalcu = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tb_ParallelCalcuPeriod4PSL = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tb_ParallelCalcuPeriod4RTD = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tab_Calcu.SuspendLayout();
            this.数据库及计算配置.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.实时计算引擎.SuspendLayout();
            this.历史计算引擎.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_period
            // 
            this.lb_period.AutoSize = true;
            this.lb_period.Location = new System.Drawing.Point(25, 48);
            this.lb_period.Name = "lb_period";
            this.lb_period.Size = new System.Drawing.Size(113, 12);
            this.lb_period.TabIndex = 13;
            this.lb_period.Text = "计算引擎扫描周期：";
            // 
            // tb_CalcuPeriod
            // 
            this.tb_CalcuPeriod.Location = new System.Drawing.Point(144, 45);
            this.tb_CalcuPeriod.Name = "tb_CalcuPeriod";
            this.tb_CalcuPeriod.Size = new System.Drawing.Size(50, 21);
            this.tb_CalcuPeriod.TabIndex = 14;
            this.tb_CalcuPeriod.TextChanged += new System.EventHandler(this.tb_CalcuPeriod_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "毫秒";
            // 
            // lb_resulttag
            // 
            this.lb_resulttag.AutoSize = true;
            this.lb_resulttag.Location = new System.Drawing.Point(6, 15);
            this.lb_resulttag.Name = "lb_resulttag";
            this.lb_resulttag.Size = new System.Drawing.Size(101, 12);
            this.lb_resulttag.TabIndex = 16;
            this.lb_resulttag.Text = "计算结果标签名：";
            // 
            // rb_TagAuto
            // 
            this.rb_TagAuto.AutoSize = true;
            this.rb_TagAuto.Checked = true;
            this.rb_TagAuto.Location = new System.Drawing.Point(5, 25);
            this.rb_TagAuto.Name = "rb_TagAuto";
            this.rb_TagAuto.Size = new System.Drawing.Size(239, 16);
            this.rb_TagAuto.TabIndex = 27;
            this.rb_TagAuto.TabStop = true;
            this.rb_TagAuto.Text = "自动(根据算法模块自动生成)(建议选项)";
            this.rb_TagAuto.UseVisualStyleBackColor = true;
            // 
            // rb_TagManual
            // 
            this.rb_TagManual.AutoSize = true;
            this.rb_TagManual.Location = new System.Drawing.Point(5, 3);
            this.rb_TagManual.Name = "rb_TagManual";
            this.rb_TagManual.Size = new System.Drawing.Size(155, 16);
            this.rb_TagManual.TabIndex = 28;
            this.rb_TagManual.Text = "手动(使用配置文件给定)";
            this.rb_TagManual.UseVisualStyleBackColor = true;
            this.rb_TagManual.CheckedChanged += new System.EventHandler(this.rb_TagManual_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rb_TagManual);
            this.panel1.Controls.Add(this.rb_TagAuto);
            this.panel1.Location = new System.Drawing.Point(24, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(245, 47);
            this.panel1.TabIndex = 29;
            // 
            // bt_save
            // 
            this.bt_save.Location = new System.Drawing.Point(150, 292);
            this.bt_save.Name = "bt_save";
            this.bt_save.Size = new System.Drawing.Size(61, 20);
            this.bt_save.TabIndex = 41;
            this.bt_save.Text = "保存";
            this.bt_save.UseVisualStyleBackColor = true;
            this.bt_save.Click += new System.EventHandler(this.bt_save_Click);
            // 
            // cbIniTableIncludePSLData
            // 
            this.cbIniTableIncludePSLData.AutoSize = true;
            this.cbIniTableIncludePSLData.Location = new System.Drawing.Point(6, 46);
            this.cbIniTableIncludePSLData.Name = "cbIniTableIncludePSLData";
            this.cbIniTableIncludePSLData.Size = new System.Drawing.Size(258, 16);
            this.cbIniTableIncludePSLData.TabIndex = 42;
            this.cbIniTableIncludePSLData.Text = "初始化数据表时包括psldata表(不建议选择)";
            this.cbIniTableIncludePSLData.UseVisualStyleBackColor = true;
            this.cbIniTableIncludePSLData.CheckedChanged += new System.EventHandler(this.cbIniTableIncludePSLData_CheckedChanged);
            // 
            // cbTag2IdMapReset
            // 
            this.cbTag2IdMapReset.AutoSize = true;
            this.cbTag2IdMapReset.Location = new System.Drawing.Point(6, 24);
            this.cbTag2IdMapReset.Name = "cbTag2IdMapReset";
            this.cbTag2IdMapReset.Size = new System.Drawing.Size(342, 16);
            this.cbTag2IdMapReset.TabIndex = 43;
            this.cbTag2IdMapReset.Text = "生成标签名id映射表时删除已有记录,重新生成(不建议选择)";
            this.cbTag2IdMapReset.UseVisualStyleBackColor = true;
            this.cbTag2IdMapReset.CheckedChanged += new System.EventHandler(this.cbTag2IdMapReset_CheckedChanged);
            // 
            // cbCalcuTime
            // 
            this.cbCalcuTime.AutoSize = true;
            this.cbCalcuTime.Location = new System.Drawing.Point(27, 110);
            this.cbCalcuTime.Name = "cbCalcuTime";
            this.cbCalcuTime.Size = new System.Drawing.Size(252, 16);
            this.cbCalcuTime.TabIndex = 44;
            this.cbCalcuTime.Text = "统计计算信息（仅测试算法和负载时使用）";
            this.cbCalcuTime.UseVisualStyleBackColor = true;
            // 
            // cbAutoRun
            // 
            this.cbAutoRun.AutoSize = true;
            this.cbAutoRun.Location = new System.Drawing.Point(27, 23);
            this.cbAutoRun.Name = "cbAutoRun";
            this.cbAutoRun.Size = new System.Drawing.Size(156, 16);
            this.cbAutoRun.TabIndex = 45;
            this.cbAutoRun.Text = "运行程序时自动启动计算";
            this.cbAutoRun.UseVisualStyleBackColor = true;
            // 
            // tab_Calcu
            // 
            this.tab_Calcu.Controls.Add(this.数据库及计算配置);
            this.tab_Calcu.Controls.Add(this.实时计算引擎);
            this.tab_Calcu.Controls.Add(this.历史计算引擎);
            this.tab_Calcu.Location = new System.Drawing.Point(12, 12);
            this.tab_Calcu.Name = "tab_Calcu";
            this.tab_Calcu.SelectedIndex = 0;
            this.tab_Calcu.Size = new System.Drawing.Size(374, 358);
            this.tab_Calcu.TabIndex = 46;
            // 
            // 数据库及计算配置
            // 
            this.数据库及计算配置.Controls.Add(this.groupBox1);
            this.数据库及计算配置.Controls.Add(this.panel2);
            this.数据库及计算配置.Controls.Add(this.label8);
            this.数据库及计算配置.Controls.Add(this.lb_resulttag);
            this.数据库及计算配置.Controls.Add(this.bt_save);
            this.数据库及计算配置.Controls.Add(this.panel1);
            this.数据库及计算配置.Location = new System.Drawing.Point(4, 22);
            this.数据库及计算配置.Name = "数据库及计算配置";
            this.数据库及计算配置.Padding = new System.Windows.Forms.Padding(3);
            this.数据库及计算配置.Size = new System.Drawing.Size(366, 332);
            this.数据库及计算配置.TabIndex = 0;
            this.数据库及计算配置.Text = "数据库及计算配置";
            this.数据库及计算配置.UseVisualStyleBackColor = true;
            this.数据库及计算配置.Click += new System.EventHandler(this.数据库及计算配置_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbTag2IdMapReset);
            this.groupBox1.Controls.Add(this.cbIniTableIncludePSLData);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbEndYear);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbStartYear);
            this.groupBox1.Location = new System.Drawing.Point(11, 149);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 137);
            this.groupBox1.TabIndex = 50;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "初始化数据表";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 12);
            this.label3.TabIndex = 44;
            this.label3.Text = "psldata数据表自动建表：";
            // 
            // tbEndYear
            // 
            this.tbEndYear.Location = new System.Drawing.Point(214, 95);
            this.tbEndYear.Name = "tbEndYear";
            this.tbEndYear.Size = new System.Drawing.Size(53, 21);
            this.tbEndYear.TabIndex = 48;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 46;
            this.label4.Text = "起始年份：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(147, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 47;
            this.label5.Text = "截止年份：";
            // 
            // tbStartYear
            // 
            this.tbStartYear.Location = new System.Drawing.Point(75, 95);
            this.tbStartYear.Name = "tbStartYear";
            this.tbStartYear.Size = new System.Drawing.Size(53, 21);
            this.tbStartYear.TabIndex = 45;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rb_ExcludeIntervalType);
            this.panel2.Controls.Add(this.rb_IncludeIntervalType);
            this.panel2.Location = new System.Drawing.Point(24, 93);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(293, 47);
            this.panel2.TabIndex = 30;
            // 
            // rb_ExcludeIntervalType
            // 
            this.rb_ExcludeIntervalType.AutoSize = true;
            this.rb_ExcludeIntervalType.Location = new System.Drawing.Point(5, 3);
            this.rb_ExcludeIntervalType.Name = "rb_ExcludeIntervalType";
            this.rb_ExcludeIntervalType.Size = new System.Drawing.Size(149, 16);
            this.rb_ExcludeIntervalType.TabIndex = 28;
            this.rb_ExcludeIntervalType.Text = "源标签名+计算结果描述";
            this.rb_ExcludeIntervalType.UseVisualStyleBackColor = true;
            this.rb_ExcludeIntervalType.CheckedChanged += new System.EventHandler(this.rb_ExcludeIntervalType_CheckedChanged);
            // 
            // rb_IncludeIntervalType
            // 
            this.rb_IncludeIntervalType.AutoSize = true;
            this.rb_IncludeIntervalType.Checked = true;
            this.rb_IncludeIntervalType.Location = new System.Drawing.Point(5, 24);
            this.rb_IncludeIntervalType.Name = "rb_IncludeIntervalType";
            this.rb_IncludeIntervalType.Size = new System.Drawing.Size(287, 16);
            this.rb_IncludeIntervalType.TabIndex = 27;
            this.rb_IncludeIntervalType.TabStop = true;
            this.rb_IncludeIntervalType.Text = "源标签名+计算结果描述+计算间隔类型(建议选项)";
            this.rb_IncludeIntervalType.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(173, 12);
            this.label8.TabIndex = 49;
            this.label8.Text = "计算结果标签名自动生成规则：";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // 实时计算引擎
            // 
            this.实时计算引擎.Controls.Add(this.label16);
            this.实时计算引擎.Controls.Add(this.tb_saveNumber);
            this.实时计算引擎.Controls.Add(this.label15);
            this.实时计算引擎.Controls.Add(this.label9);
            this.实时计算引擎.Controls.Add(this.label7);
            this.实时计算引擎.Controls.Add(this.tb_WritePeriod);
            this.实时计算引擎.Controls.Add(this.label6);
            this.实时计算引擎.Controls.Add(this.btSaveCalcu);
            this.实时计算引擎.Controls.Add(this.tbMaxReadRTDB);
            this.实时计算引擎.Controls.Add(this.label2);
            this.实时计算引擎.Controls.Add(this.cbAutoRun);
            this.实时计算引擎.Controls.Add(this.cbCalcuTime);
            this.实时计算引擎.Controls.Add(this.lb_period);
            this.实时计算引擎.Controls.Add(this.tb_CalcuPeriod);
            this.实时计算引擎.Controls.Add(this.label1);
            this.实时计算引擎.Location = new System.Drawing.Point(4, 22);
            this.实时计算引擎.Name = "实时计算引擎";
            this.实时计算引擎.Padding = new System.Windows.Forms.Padding(3);
            this.实时计算引擎.Size = new System.Drawing.Size(366, 332);
            this.实时计算引擎.TabIndex = 1;
            this.实时计算引擎.Text = "实时计算引擎";
            this.实时计算引擎.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(176, 138);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 12);
            this.label16.TabIndex = 56;
            this.label16.Text = "条记录保存一次";
            // 
            // tb_saveNumber
            // 
            this.tb_saveNumber.Location = new System.Drawing.Point(120, 135);
            this.tb_saveNumber.Name = "tb_saveNumber";
            this.tb_saveNumber.Size = new System.Drawing.Size(50, 21);
            this.tb_saveNumber.TabIndex = 55;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(25, 138);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(89, 12);
            this.label15.TabIndex = 54;
            this.label15.Text = "记录计算信息每";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(250, 166);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 12);
            this.label9.TabIndex = 53;
            this.label9.Text = "仅可在XML中设定";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(241, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 52;
            this.label7.Text = "小时";
            // 
            // tb_WritePeriod
            // 
            this.tb_WritePeriod.Location = new System.Drawing.Point(183, 74);
            this.tb_WritePeriod.Name = "tb_WritePeriod";
            this.tb_WritePeriod.Size = new System.Drawing.Size(50, 21);
            this.tb_WritePeriod.TabIndex = 51;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 12);
            this.label6.TabIndex = 50;
            this.label6.Text = "保存计算配置对象信息周期：";
            // 
            // btSaveCalcu
            // 
            this.btSaveCalcu.Location = new System.Drawing.Point(144, 281);
            this.btSaveCalcu.Name = "btSaveCalcu";
            this.btSaveCalcu.Size = new System.Drawing.Size(61, 20);
            this.btSaveCalcu.TabIndex = 49;
            this.btSaveCalcu.Text = "保存";
            this.btSaveCalcu.UseVisualStyleBackColor = true;
            this.btSaveCalcu.Click += new System.EventHandler(this.btSaveCalcu_Click);
            // 
            // tbMaxReadRTDB
            // 
            this.tbMaxReadRTDB.Location = new System.Drawing.Point(194, 163);
            this.tbMaxReadRTDB.Name = "tbMaxReadRTDB";
            this.tbMaxReadRTDB.ReadOnly = true;
            this.tbMaxReadRTDB.Size = new System.Drawing.Size(50, 21);
            this.tbMaxReadRTDB.TabIndex = 48;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 12);
            this.label2.TabIndex = 47;
            this.label2.Text = "计算引擎单次读取实时数据量：";
            // 
            // 历史计算引擎
            // 
            this.历史计算引擎.Controls.Add(this.bt_savehistorycalcu);
            this.历史计算引擎.Controls.Add(this.panel3);
            this.历史计算引擎.Controls.Add(this.label11);
            this.历史计算引擎.Controls.Add(this.label13);
            this.历史计算引擎.Controls.Add(this.tb_ParallelCalcuPeriod4PSL);
            this.历史计算引擎.Controls.Add(this.label12);
            this.历史计算引擎.Controls.Add(this.tb_ParallelCalcuPeriod4RTD);
            this.历史计算引擎.Controls.Add(this.label10);
            this.历史计算引擎.Location = new System.Drawing.Point(4, 22);
            this.历史计算引擎.Name = "历史计算引擎";
            this.历史计算引擎.Size = new System.Drawing.Size(366, 332);
            this.历史计算引擎.TabIndex = 2;
            this.历史计算引擎.Text = "历史计算引擎";
            this.历史计算引擎.UseVisualStyleBackColor = true;
            // 
            // bt_savehistorycalcu
            // 
            this.bt_savehistorycalcu.Location = new System.Drawing.Point(135, 242);
            this.bt_savehistorycalcu.Name = "bt_savehistorycalcu";
            this.bt_savehistorycalcu.Size = new System.Drawing.Size(61, 20);
            this.bt_savehistorycalcu.TabIndex = 60;
            this.bt_savehistorycalcu.Text = "保存";
            this.bt_savehistorycalcu.UseVisualStyleBackColor = true;
            this.bt_savehistorycalcu.Click += new System.EventHandler(this.bt_savehistorycalcu_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label14);
            this.panel3.Location = new System.Drawing.Point(15, 46);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(319, 81);
            this.panel3.TabIndex = 59;
            // 
            // label14
            // 
            this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label14.Location = new System.Drawing.Point(0, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(319, 81);
            this.label14.TabIndex = 58;
            this.label14.Text = "实时数据计算并发周期一般以天为单位，通常设为1天。该参数主要为了减少实时数据的读取次数。如计算周期为1小时，则一次读取一天的数据，一天内24小时每小时的计算会并发" +
    "执行。仅当实时数据计算周期有48小时或更长时间的设定时，才需要改变此设定。一般不建议将实时数据计算周期设定为大于1天。";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(259, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 12);
            this.label11.TabIndex = 57;
            this.label11.Text = "天";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(259, 136);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(17, 12);
            this.label13.TabIndex = 56;
            this.label13.Text = "天";
            // 
            // tb_ParallelCalcuPeriod4PSL
            // 
            this.tb_ParallelCalcuPeriod4PSL.Location = new System.Drawing.Point(192, 133);
            this.tb_ParallelCalcuPeriod4PSL.Name = "tb_ParallelCalcuPeriod4PSL";
            this.tb_ParallelCalcuPeriod4PSL.Size = new System.Drawing.Size(50, 21);
            this.tb_ParallelCalcuPeriod4PSL.TabIndex = 55;
            this.tb_ParallelCalcuPeriod4PSL.TextChanged += new System.EventHandler(this.tb_ParallelCalcuPeriod4PSL_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(13, 136);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(173, 12);
            this.label12.TabIndex = 54;
            this.label12.Text = "概化数据再概化计算并发周期：";
            // 
            // tb_ParallelCalcuPeriod4RTD
            // 
            this.tb_ParallelCalcuPeriod4RTD.Location = new System.Drawing.Point(203, 15);
            this.tb_ParallelCalcuPeriod4RTD.Name = "tb_ParallelCalcuPeriod4RTD";
            this.tb_ParallelCalcuPeriod4RTD.Size = new System.Drawing.Size(50, 21);
            this.tb_ParallelCalcuPeriod4RTD.TabIndex = 15;
            this.tb_ParallelCalcuPeriod4RTD.TextChanged += new System.EventHandler(this.tb_ParallelCalcuPeriod4RTD_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(137, 12);
            this.label10.TabIndex = 14;
            this.label10.Text = "实时数据计算并发周期：";
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 374);
            this.Controls.Add(this.tab_Calcu);
            this.Name = "Setup";
            this.Text = "Setup";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tab_Calcu.ResumeLayout(false);
            this.数据库及计算配置.ResumeLayout(false);
            this.数据库及计算配置.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.实时计算引擎.ResumeLayout(false);
            this.实时计算引擎.PerformLayout();
            this.历史计算引擎.ResumeLayout(false);
            this.历史计算引擎.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_period;
        private System.Windows.Forms.TextBox tb_CalcuPeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_resulttag;

        #region 计算结果标签名
            private System.Windows.Forms.RadioButton rb_TagAuto;
            private System.Windows.Forms.RadioButton rb_TagManual;
            private System.Windows.Forms.Panel panel1;
        #endregion 
        private System.Windows.Forms.Button bt_save;
        private System.Windows.Forms.CheckBox cbIniTableIncludePSLData;
        private System.Windows.Forms.CheckBox cbTag2IdMapReset;
        private System.Windows.Forms.CheckBox cbCalcuTime;
        private System.Windows.Forms.CheckBox cbAutoRun;
        private System.Windows.Forms.TabControl tab_Calcu;
        private System.Windows.Forms.TabPage 数据库及计算配置;
        private System.Windows.Forms.TabPage 实时计算引擎;
        private System.Windows.Forms.TextBox tbMaxReadRTDB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbEndYear;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbStartYear;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btSaveCalcu;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_WritePeriod;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rb_ExcludeIntervalType;
        private System.Windows.Forms.RadioButton rb_IncludeIntervalType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage 历史计算引擎;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tb_ParallelCalcuPeriod4PSL;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tb_ParallelCalcuPeriod4RTD;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button bt_savehistorycalcu;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tb_saveNumber;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}