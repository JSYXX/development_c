namespace PSLCalcu
{
    partial class AppRunForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppRunForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tslb_CalcuConfig = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslb_CalcuWarnings = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslb_CalcuErrors = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslb_CurrentIndex = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbt_Start = new System.Windows.Forms.ToolStripButton();
            this.tsbt_Quit = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.bt_searchHelp = new System.Windows.Forms.Button();
            this.cb_moduletype = new System.Windows.Forms.ComboBox();
            this.checkBox_SelectModule = new System.Windows.Forms.CheckBox();
            this.cb_intervaltype = new System.Windows.Forms.ComboBox();
            this.checkBox_SelectIntervalType = new System.Windows.Forms.CheckBox();
            this.btn_exportCSV = new System.Windows.Forms.Button();
            this.btn_SetStartDate = new System.Windows.Forms.Button();
            this.dtStartTime = new System.Windows.Forms.DateTimePicker();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.tCalcuStartDate = new System.Windows.Forms.Label();
            this.checkBox_SelectAll = new System.Windows.Forms.CheckBox();
            this.lb_index = new System.Windows.Forms.Label();
            this.bt_Fixed = new System.Windows.Forms.Button();
            this.tb_Fastfixed = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lV_DataList = new System.Windows.Forms.ListView();
            this.index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sourcetagname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sourcetagdb = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sourcetagdesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sourcetagdim = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sourcetagmrb = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sourcetagmre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fmodulename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fgroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.forder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fclass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.falgorithmsflag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fparas = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fcondpslnames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.foutputtable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.foutputnumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.foutputpsltagnames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.finterval = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fdelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fstarttime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fendtime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fnexttime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rTDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.iniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iniToolTable = new System.Windows.Forms.ToolStripMenuItem();
            this.extractModulesInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导入得分权重信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.检查并抽取值次信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReadConstInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.概化数据表维护ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calcuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.appendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rectifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTagnameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCalcuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CheckTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkCurveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyCalcuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoReCalcuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemSearchData = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemDelData = new System.Windows.Forms.ToolStripMenuItem();
            this.uniqeCheckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCondSpan2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFCondSpanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFDistribute22ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFDistribute12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDigitalSumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mDigitalSetStatsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mFindMinInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mIndex9ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mIndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mFindMinToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mNormalizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mCondToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMultiAnalogSubToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMulitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mFOPC2MinuteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.并行时间分割ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spanPValues4rtdbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.并行时间分割SpanPValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMultiAnalogAvgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.统计相同元素个数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generaltestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.综合算法测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oPCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.时间转换工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.log测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsCalcuValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pSLDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pSLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webTagnameIDMap接口测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslb_CalcuConfig,
            this.tslb_CalcuWarnings,
            this.tslb_CalcuErrors,
            this.tslb_CurrentIndex});
            this.statusStrip1.Location = new System.Drawing.Point(3, 515);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 15, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1358, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tslb_CalcuConfig
            // 
            this.tslb_CalcuConfig.AutoSize = false;
            this.tslb_CalcuConfig.Name = "tslb_CalcuConfig";
            this.tslb_CalcuConfig.Size = new System.Drawing.Size(240, 17);
            this.tslb_CalcuConfig.Text = "tslb_CalcuConfig";
            this.tslb_CalcuConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tslb_CalcuWarnings
            // 
            this.tslb_CalcuWarnings.AutoSize = false;
            this.tslb_CalcuWarnings.Name = "tslb_CalcuWarnings";
            this.tslb_CalcuWarnings.Size = new System.Drawing.Size(250, 17);
            this.tslb_CalcuWarnings.Text = "警告提示";
            // 
            // tslb_CalcuErrors
            // 
            this.tslb_CalcuErrors.AutoSize = false;
            this.tslb_CalcuErrors.Name = "tslb_CalcuErrors";
            this.tslb_CalcuErrors.Size = new System.Drawing.Size(250, 17);
            this.tslb_CalcuErrors.Text = "错误提示";
            // 
            // tslb_CurrentIndex
            // 
            this.tslb_CurrentIndex.Name = "tslb_CurrentIndex";
            this.tslb_CurrentIndex.Size = new System.Drawing.Size(65, 17);
            this.tslb_CurrentIndex.Text = "当前计算项";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbt_Start,
            this.tsbt_Quit});
            this.toolStrip1.Location = new System.Drawing.Point(3, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1358, 26);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "Start";
            // 
            // tsbt_Start
            // 
            this.tsbt_Start.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tsbt_Start.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbt_Start.Image = global::PSLCalcu.Properties.Resources.start;
            this.tsbt_Start.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbt_Start.Name = "tsbt_Start";
            this.tsbt_Start.Size = new System.Drawing.Size(23, 23);
            this.tsbt_Start.Text = "Start";
            this.tsbt_Start.Click += new System.EventHandler(this.tsbt_Start_Click);
            // 
            // tsbt_Quit
            // 
            this.tsbt_Quit.AutoSize = false;
            this.tsbt_Quit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbt_Quit.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsbt_Quit.Image = global::PSLCalcu.Properties.Resources.exit;
            this.tsbt_Quit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbt_Quit.Name = "tsbt_Quit";
            this.tsbt_Quit.Size = new System.Drawing.Size(23, 23);
            this.tsbt_Quit.Text = "Quit";
            this.tsbt_Quit.Click += new System.EventHandler(this.tsbt_Quit_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(9, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.bt_searchHelp);
            this.splitContainer1.Panel1.Controls.Add(this.cb_moduletype);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox_SelectModule);
            this.splitContainer1.Panel1.Controls.Add(this.cb_intervaltype);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox_SelectIntervalType);
            this.splitContainer1.Panel1.Controls.Add(this.btn_exportCSV);
            this.splitContainer1.Panel1.Controls.Add(this.btn_SetStartDate);
            this.splitContainer1.Panel1.Controls.Add(this.dtStartTime);
            this.splitContainer1.Panel1.Controls.Add(this.dtStartDate);
            this.splitContainer1.Panel1.Controls.Add(this.tCalcuStartDate);
            this.splitContainer1.Panel1.Controls.Add(this.checkBox_SelectAll);
            this.splitContainer1.Panel1.Controls.Add(this.lb_index);
            this.splitContainer1.Panel1.Controls.Add(this.bt_Fixed);
            this.splitContainer1.Panel1.Controls.Add(this.tb_Fastfixed);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lV_DataList);
            this.splitContainer1.Size = new System.Drawing.Size(1336, 452);
            this.splitContainer1.SplitterDistance = 36;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1236, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 31;
            this.label2.Text = "行";
            this.label2.Visible = false;
            // 
            // bt_searchHelp
            // 
            this.bt_searchHelp.Location = new System.Drawing.Point(851, 6);
            this.bt_searchHelp.Name = "bt_searchHelp";
            this.bt_searchHelp.Size = new System.Drawing.Size(29, 23);
            this.bt_searchHelp.TabIndex = 30;
            this.bt_searchHelp.Text = "？";
            this.bt_searchHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bt_searchHelp.UseVisualStyleBackColor = true;
            this.bt_searchHelp.Click += new System.EventHandler(this.bt_searchHelp_Click);
            // 
            // cb_moduletype
            // 
            this.cb_moduletype.AccessibleName = "ModuleName";
            this.cb_moduletype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_moduletype.FormattingEnabled = true;
            this.cb_moduletype.Location = new System.Drawing.Point(299, 8);
            this.cb_moduletype.Name = "cb_moduletype";
            this.cb_moduletype.Size = new System.Drawing.Size(126, 20);
            this.cb_moduletype.TabIndex = 29;
            this.cb_moduletype.SelectedIndexChanged += new System.EventHandler(this.cb_moduletype_SelectedIndexChanged);
            // 
            // checkBox_SelectModule
            // 
            this.checkBox_SelectModule.AutoSize = true;
            this.checkBox_SelectModule.Location = new System.Drawing.Point(242, 10);
            this.checkBox_SelectModule.Name = "checkBox_SelectModule";
            this.checkBox_SelectModule.Size = new System.Drawing.Size(60, 16);
            this.checkBox_SelectModule.TabIndex = 28;
            this.checkBox_SelectModule.Text = "按算法";
            this.checkBox_SelectModule.UseVisualStyleBackColor = true;
            this.checkBox_SelectModule.CheckedChanged += new System.EventHandler(this.checkBox_SelectModule_CheckedChanged);
            this.checkBox_SelectModule.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkBox_SelectModule_MouseDown);
            // 
            // cb_intervaltype
            // 
            this.cb_intervaltype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_intervaltype.FormattingEnabled = true;
            this.cb_intervaltype.Location = new System.Drawing.Point(152, 8);
            this.cb_intervaltype.Name = "cb_intervaltype";
            this.cb_intervaltype.Size = new System.Drawing.Size(66, 20);
            this.cb_intervaltype.TabIndex = 27;
            this.cb_intervaltype.SelectedIndexChanged += new System.EventHandler(this.cb_intervaltype_SelectedIndexChanged);
            // 
            // checkBox_SelectIntervalType
            // 
            this.checkBox_SelectIntervalType.AutoSize = true;
            this.checkBox_SelectIntervalType.Location = new System.Drawing.Point(71, 10);
            this.checkBox_SelectIntervalType.Name = "checkBox_SelectIntervalType";
            this.checkBox_SelectIntervalType.Size = new System.Drawing.Size(84, 16);
            this.checkBox_SelectIntervalType.TabIndex = 26;
            this.checkBox_SelectIntervalType.Text = "按间隔类型";
            this.checkBox_SelectIntervalType.UseVisualStyleBackColor = true;
            this.checkBox_SelectIntervalType.CheckedChanged += new System.EventHandler(this.checkBox_SelectIntervalType_CheckedChanged);
            this.checkBox_SelectIntervalType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkBox_SelectIntervalType_MouseDown);
            // 
            // btn_exportCSV
            // 
            this.btn_exportCSV.Location = new System.Drawing.Point(912, 6);
            this.btn_exportCSV.Name = "btn_exportCSV";
            this.btn_exportCSV.Size = new System.Drawing.Size(75, 23);
            this.btn_exportCSV.TabIndex = 23;
            this.btn_exportCSV.Text = "导出";
            this.btn_exportCSV.UseVisualStyleBackColor = true;
            this.btn_exportCSV.Click += new System.EventHandler(this.btn_exportCSV_Click);
            // 
            // btn_SetStartDate
            // 
            this.btn_SetStartDate.Location = new System.Drawing.Point(770, 6);
            this.btn_SetStartDate.Name = "btn_SetStartDate";
            this.btn_SetStartDate.Size = new System.Drawing.Size(75, 23);
            this.btn_SetStartDate.TabIndex = 22;
            this.btn_SetStartDate.Text = "修改";
            this.btn_SetStartDate.UseVisualStyleBackColor = true;
            this.btn_SetStartDate.Click += new System.EventHandler(this.btn_SetStartDate_Click);
            // 
            // dtStartTime
            // 
            this.dtStartTime.CustomFormat = "HH:mm:ss";
            this.dtStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStartTime.Location = new System.Drawing.Point(675, 6);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.ShowUpDown = true;
            this.dtStartTime.Size = new System.Drawing.Size(73, 21);
            this.dtStartTime.TabIndex = 21;
            // 
            // dtStartDate
            // 
            this.dtStartDate.Location = new System.Drawing.Point(550, 6);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Size = new System.Drawing.Size(119, 21);
            this.dtStartDate.TabIndex = 17;
            // 
            // tCalcuStartDate
            // 
            this.tCalcuStartDate.AutoSize = true;
            this.tCalcuStartDate.Location = new System.Drawing.Point(468, 11);
            this.tCalcuStartDate.Name = "tCalcuStartDate";
            this.tCalcuStartDate.Size = new System.Drawing.Size(89, 12);
            this.tCalcuStartDate.TabIndex = 12;
            this.tCalcuStartDate.Text = "计算起始时间：";
            // 
            // checkBox_SelectAll
            // 
            this.checkBox_SelectAll.AutoSize = true;
            this.checkBox_SelectAll.Location = new System.Drawing.Point(4, 10);
            this.checkBox_SelectAll.Name = "checkBox_SelectAll";
            this.checkBox_SelectAll.Size = new System.Drawing.Size(48, 16);
            this.checkBox_SelectAll.TabIndex = 11;
            this.checkBox_SelectAll.Text = "全选";
            this.checkBox_SelectAll.UseVisualStyleBackColor = true;
            this.checkBox_SelectAll.CheckedChanged += new System.EventHandler(this.checkBox_SelectAll_CheckedChanged);
            this.checkBox_SelectAll.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkBox_SelectAll_MouseDown);
            // 
            // lb_index
            // 
            this.lb_index.AutoSize = true;
            this.lb_index.Location = new System.Drawing.Point(501, 12);
            this.lb_index.Name = "lb_index";
            this.lb_index.Size = new System.Drawing.Size(0, 12);
            this.lb_index.TabIndex = 8;
            // 
            // bt_Fixed
            // 
            this.bt_Fixed.Location = new System.Drawing.Point(1266, 6);
            this.bt_Fixed.Name = "bt_Fixed";
            this.bt_Fixed.Size = new System.Drawing.Size(46, 22);
            this.bt_Fixed.TabIndex = 7;
            this.bt_Fixed.Text = "定位";
            this.bt_Fixed.UseVisualStyleBackColor = true;
            this.bt_Fixed.Click += new System.EventHandler(this.bt_Fixed_Click);
            // 
            // tb_Fastfixed
            // 
            this.tb_Fastfixed.Location = new System.Drawing.Point(1161, 8);
            this.tb_Fastfixed.Name = "tb_Fastfixed";
            this.tb_Fastfixed.Size = new System.Drawing.Size(69, 21);
            this.tb_Fastfixed.TabIndex = 6;
            this.tb_Fastfixed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_Fastfixed_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1087, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "快速定位到：";
            // 
            // lV_DataList
            // 
            this.lV_DataList.CheckBoxes = true;
            this.lV_DataList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.index,
            this.sourcetagname,
            this.sourcetagdb,
            this.sourcetagdesc,
            this.sourcetagdim,
            this.sourcetagmrb,
            this.sourcetagmre,
            this.fmodulename,
            this.fgroup,
            this.forder,
            this.fclass,
            this.falgorithmsflag,
            this.fparas,
            this.fcondpslnames,
            this.foutputtable,
            this.foutputnumber,
            this.foutputpsltagnames,
            this.finterval,
            this.fdelay,
            this.fstarttime,
            this.fendtime,
            this.fnexttime});
            this.lV_DataList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lV_DataList.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lV_DataList.FullRowSelect = true;
            this.lV_DataList.GridLines = true;
            this.lV_DataList.Location = new System.Drawing.Point(0, 0);
            this.lV_DataList.Margin = new System.Windows.Forms.Padding(9, 10, 9, 10);
            this.lV_DataList.Name = "lV_DataList";
            this.lV_DataList.Size = new System.Drawing.Size(1336, 413);
            this.lV_DataList.TabIndex = 4;
            this.lV_DataList.UseCompatibleStateImageBehavior = false;
            this.lV_DataList.View = System.Windows.Forms.View.Details;
            this.lV_DataList.SelectedIndexChanged += new System.EventHandler(this.lV_DataList_SelectedIndexChanged);
            this.lV_DataList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseDoubleClick);
            // 
            // index
            // 
            this.index.Text = "Index";
            this.index.Width = 46;
            // 
            // sourcetagname
            // 
            this.sourcetagname.Text = "SourceTag";
            this.sourcetagname.Width = 20;
            // 
            // sourcetagdb
            // 
            this.sourcetagdb.Text = "SourcDB";
            this.sourcetagdb.Width = 25;
            // 
            // sourcetagdesc
            // 
            this.sourcetagdesc.Text = "Description";
            this.sourcetagdesc.Width = 20;
            // 
            // sourcetagdim
            // 
            this.sourcetagdim.Text = "Dim";
            this.sourcetagdim.Width = 20;
            // 
            // sourcetagmrb
            // 
            this.sourcetagmrb.Text = "MRB";
            this.sourcetagmrb.Width = 15;
            // 
            // sourcetagmre
            // 
            this.sourcetagmre.Text = "MRE";
            this.sourcetagmre.Width = 43;
            // 
            // fmodulename
            // 
            this.fmodulename.Text = "CalcuType";
            this.fmodulename.Width = 26;
            // 
            // fgroup
            // 
            this.fgroup.Text = "group";
            this.fgroup.Width = 38;
            // 
            // forder
            // 
            this.forder.Text = "Order";
            this.forder.Width = 47;
            // 
            // fclass
            // 
            this.fclass.Text = "Class";
            this.fclass.Width = 91;
            // 
            // falgorithmsflag
            // 
            this.falgorithmsflag.Text = "Flag";
            this.falgorithmsflag.Width = 33;
            // 
            // fparas
            // 
            this.fparas.Text = "Para";
            this.fparas.Width = 33;
            // 
            // fcondpslnames
            // 
            this.fcondpslnames.Text = "Condition";
            this.fcondpslnames.Width = 39;
            // 
            // foutputtable
            // 
            this.foutputtable.Text = "OutputTable";
            this.foutputtable.Width = 55;
            // 
            // foutputnumber
            // 
            this.foutputnumber.Text = "OutputNumber";
            this.foutputnumber.Width = 29;
            // 
            // foutputpsltagnames
            // 
            this.foutputpsltagnames.Text = "OutputTags";
            this.foutputpsltagnames.Width = 34;
            // 
            // finterval
            // 
            this.finterval.Text = "interval";
            this.finterval.Width = 42;
            // 
            // fdelay
            // 
            this.fdelay.Text = "Delay(Sec)";
            this.fdelay.Width = 55;
            // 
            // fstarttime
            // 
            this.fstarttime.Text = "StartTime";
            this.fstarttime.Width = 150;
            // 
            // fendtime
            // 
            this.fendtime.Text = "EndTime";
            this.fendtime.Width = 150;
            // 
            // fnexttime
            // 
            this.fnexttime.Text = "NextTime";
            this.fnexttime.Width = 150;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 50);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(9, 0, 9, 9);
            this.panel1.Size = new System.Drawing.Size(1358, 465);
            this.panel1.TabIndex = 5;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "PGIM PSLDistribution";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.calcuToolStripMenuItem,
            this.historyCalcuToolStripMenuItem,
            this.ToolStripMenuItemSearch,
            this.DebugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(3, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1358, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // systemToolStripMenuItem
            // 
            this.systemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            this.systemToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.systemToolStripMenuItem.Text = "系统";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.setupToolStripMenuItem.Text = "设置";
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.stopToolStripMenuItem.Text = "停止实时计算";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.exitToolStripMenuItem.Text = "退出";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionToolStripMenuItem,
            this.iniToolStripMenuItem,
            this.概化数据表维护ToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.databaseToolStripMenuItem.Text = "数据库";
            // 
            // connectionToolStripMenuItem
            // 
            this.connectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rTDBToolStripMenuItem,
            this.toolStripMenuItem2});
            this.connectionToolStripMenuItem.Name = "connectionToolStripMenuItem";
            this.connectionToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.connectionToolStripMenuItem.Text = "连接测试";
            // 
            // rTDBToolStripMenuItem
            // 
            this.rTDBToolStripMenuItem.Name = "rTDBToolStripMenuItem";
            this.rTDBToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.rTDBToolStripMenuItem.Text = "实时数据库连接测试";
            this.rTDBToolStripMenuItem.Click += new System.EventHandler(this.rTDBToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuItem2.Text = "关系数据库连接测试";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // iniToolStripMenuItem
            // 
            this.iniToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iniToolTable,
            this.extractModulesInfoToolStripMenuItem,
            this.导入得分权重信息ToolStripMenuItem,
            this.检查并抽取值次信息ToolStripMenuItem,
            this.ReadConstInfoToolStripMenuItem,
            this.ExcelToolStripMenuItem});
            this.iniToolStripMenuItem.Name = "iniToolStripMenuItem";
            this.iniToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.iniToolStripMenuItem.Text = "数据仓库维护";
            // 
            // iniToolTable
            // 
            this.iniToolTable.Name = "iniToolTable";
            this.iniToolTable.Size = new System.Drawing.Size(262, 22);
            this.iniToolTable.Text = "数据表初始化";
            this.iniToolTable.Click += new System.EventHandler(this.iniToolTable_Click);
            // 
            // extractModulesInfoToolStripMenuItem
            // 
            this.extractModulesInfoToolStripMenuItem.Name = "extractModulesInfoToolStripMenuItem";
            this.extractModulesInfoToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.extractModulesInfoToolStripMenuItem.Text = "抽取算法信息(导入计算配置前)";
            this.extractModulesInfoToolStripMenuItem.Click += new System.EventHandler(this.extractModulesInfoToolStripMenuItem_Click);
            // 
            // 导入得分权重信息ToolStripMenuItem
            // 
            this.导入得分权重信息ToolStripMenuItem.Name = "导入得分权重信息ToolStripMenuItem";
            this.导入得分权重信息ToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.导入得分权重信息ToolStripMenuItem.Text = "导入得分权重信息(导入计算配置后)";
            this.导入得分权重信息ToolStripMenuItem.Click += new System.EventHandler(this.importweightToolStripMenuItem_Click);
            // 
            // 检查并抽取值次信息ToolStripMenuItem
            // 
            this.检查并抽取值次信息ToolStripMenuItem.Name = "检查并抽取值次信息ToolStripMenuItem";
            this.检查并抽取值次信息ToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.检查并抽取值次信息ToolStripMenuItem.Text = "读取值次信息(导入计算配置后)";
            this.检查并抽取值次信息ToolStripMenuItem.Click += new System.EventHandler(this.检查并抽取值次信息ToolStripMenuItem_Click);
            // 
            // ReadConstInfoToolStripMenuItem
            // 
            this.ReadConstInfoToolStripMenuItem.Name = "ReadConstInfoToolStripMenuItem";
            this.ReadConstInfoToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.ReadConstInfoToolStripMenuItem.Text = "读取常数标签值(导入计算配置后)";
            this.ReadConstInfoToolStripMenuItem.Click += new System.EventHandler(this.ReadConstInfoToolStripMenuItem_Click);
            // 
            // ExcelToolStripMenuItem
            // 
            this.ExcelToolStripMenuItem.Name = "ExcelToolStripMenuItem";
            this.ExcelToolStripMenuItem.Size = new System.Drawing.Size(262, 22);
            this.ExcelToolStripMenuItem.Text = "算法信息导出到CSV文件";
            this.ExcelToolStripMenuItem.Click += new System.EventHandler(this.ExcelToolStripMenuItem_Click);
            // 
            // 概化数据表维护ToolStripMenuItem
            // 
            this.概化数据表维护ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CloseIndexToolStripMenuItem,
            this.openIndexToolStripMenuItem});
            this.概化数据表维护ToolStripMenuItem.Name = "概化数据表维护ToolStripMenuItem";
            this.概化数据表维护ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.概化数据表维护ToolStripMenuItem.Text = "概化数据表维护";
            // 
            // CloseIndexToolStripMenuItem
            // 
            this.CloseIndexToolStripMenuItem.Name = "CloseIndexToolStripMenuItem";
            this.CloseIndexToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.CloseIndexToolStripMenuItem.Text = "关闭索引";
            this.CloseIndexToolStripMenuItem.Click += new System.EventHandler(this.CloseIndexToolStripMenuItem_Click);
            // 
            // openIndexToolStripMenuItem
            // 
            this.openIndexToolStripMenuItem.Name = "openIndexToolStripMenuItem";
            this.openIndexToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.openIndexToolStripMenuItem.Text = "打开索引";
            this.openIndexToolStripMenuItem.Click += new System.EventHandler(this.OPenIndexToolStripMenuItem_Click);
            // 
            // calcuToolStripMenuItem
            // 
            this.calcuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1,
            this.appendToolStripMenuItem,
            this.rectifyToolStripMenuItem,
            this.updateTagnameToolStripMenuItem,
            this.deleteCalcuToolStripMenuItem,
            this.CheckTagToolStripMenuItem,
            this.checkCurveToolStripMenuItem});
            this.calcuToolStripMenuItem.Name = "calcuToolStripMenuItem";
            this.calcuToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.calcuToolStripMenuItem.Text = "计算组态";
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(310, 22);
            this.importToolStripMenuItem1.Text = "导入计算配置组态信息(标签id映射重置)";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
            // 
            // appendToolStripMenuItem
            // 
            this.appendToolStripMenuItem.Name = "appendToolStripMenuItem";
            this.appendToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.appendToolStripMenuItem.Text = "追加计算配置组态信息（标签id映射不重置）";
            this.appendToolStripMenuItem.Click += new System.EventHandler(this.appendToolStripMenuItem_Click);
            // 
            // rectifyToolStripMenuItem
            // 
            this.rectifyToolStripMenuItem.Name = "rectifyToolStripMenuItem";
            this.rectifyToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.rectifyToolStripMenuItem.Text = "修改计算配置组态信息(非标签信息)";
            this.rectifyToolStripMenuItem.Click += new System.EventHandler(this.rectifyToolStripMenuItem_Click);
            // 
            // updateTagnameToolStripMenuItem
            // 
            this.updateTagnameToolStripMenuItem.Name = "updateTagnameToolStripMenuItem";
            this.updateTagnameToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.updateTagnameToolStripMenuItem.Text = "修改计算配置组态信息(标签信息)";
            this.updateTagnameToolStripMenuItem.Click += new System.EventHandler(this.updatetagnameToolStripMenuItem_Click);
            // 
            // deleteCalcuToolStripMenuItem
            // 
            this.deleteCalcuToolStripMenuItem.Name = "deleteCalcuToolStripMenuItem";
            this.deleteCalcuToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.deleteCalcuToolStripMenuItem.Text = "删除计算配置组态信息";
            this.deleteCalcuToolStripMenuItem.Click += new System.EventHandler(this.deleteCalcuToolStripMenuItem_Click);
            // 
            // CheckTagToolStripMenuItem
            // 
            this.CheckTagToolStripMenuItem.Name = "CheckTagToolStripMenuItem";
            this.CheckTagToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.CheckTagToolStripMenuItem.Text = "检查实时标签有效性";
            this.CheckTagToolStripMenuItem.Click += new System.EventHandler(this.checkTagToolStripMenuItem_Click);
            // 
            // checkCurveToolStripMenuItem
            // 
            this.checkCurveToolStripMenuItem.Name = "checkCurveToolStripMenuItem";
            this.checkCurveToolStripMenuItem.Size = new System.Drawing.Size(310, 22);
            this.checkCurveToolStripMenuItem.Text = "检查期望曲线和得分曲线";
            this.checkCurveToolStripMenuItem.Click += new System.EventHandler(this.checkCurveToolStripMenuItem_Click_1);
            // 
            // historyCalcuToolStripMenuItem
            // 
            this.historyCalcuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historyToolStripMenuItem,
            this.AutoReCalcuToolStripMenuItem});
            this.historyCalcuToolStripMenuItem.Name = "historyCalcuToolStripMenuItem";
            this.historyCalcuToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.historyCalcuToolStripMenuItem.Text = "数据重算";
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            this.historyToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.historyToolStripMenuItem.Text = "手动设置历史数据重算";
            this.historyToolStripMenuItem.Click += new System.EventHandler(this.historyToolStripMenuItem_Click);
            // 
            // AutoReCalcuToolStripMenuItem
            // 
            this.AutoReCalcuToolStripMenuItem.Name = "AutoReCalcuToolStripMenuItem";
            this.AutoReCalcuToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.AutoReCalcuToolStripMenuItem.Text = "自动设置历史数据重算";
            this.AutoReCalcuToolStripMenuItem.Click += new System.EventHandler(this.AutoReCalcuToolStripMenuItem_Click);
            // 
            // ToolStripMenuItemSearch
            // 
            this.ToolStripMenuItemSearch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemSearchData,
            this.ToolStripMenuItemDelData,
            this.uniqeCheckToolStripMenuItem});
            this.ToolStripMenuItemSearch.Name = "ToolStripMenuItemSearch";
            this.ToolStripMenuItemSearch.Size = new System.Drawing.Size(65, 20);
            this.ToolStripMenuItemSearch.Text = "数据管理";
            // 
            // ToolStripMenuItemSearchData
            // 
            this.ToolStripMenuItemSearchData.Name = "ToolStripMenuItemSearchData";
            this.ToolStripMenuItemSearchData.Size = new System.Drawing.Size(178, 22);
            this.ToolStripMenuItemSearchData.Text = "数据查询";
            this.ToolStripMenuItemSearchData.Click += new System.EventHandler(this.ToolStripMenuItemSearchData_Click);
            // 
            // ToolStripMenuItemDelData
            // 
            this.ToolStripMenuItemDelData.Name = "ToolStripMenuItemDelData";
            this.ToolStripMenuItemDelData.Size = new System.Drawing.Size(178, 22);
            this.ToolStripMenuItemDelData.Text = "数据批量导出和删除";
            this.ToolStripMenuItemDelData.Click += new System.EventHandler(this.ToolStripMenuItemDelData_Click);
            // 
            // uniqeCheckToolStripMenuItem
            // 
            this.uniqeCheckToolStripMenuItem.Name = "uniqeCheckToolStripMenuItem";
            this.uniqeCheckToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.uniqeCheckToolStripMenuItem.Text = "数据唯一性检查";
            this.uniqeCheckToolStripMenuItem.Click += new System.EventHandler(this.uniqeCheckToolStripMenuItem_Click);
            // 
            // DebugToolStripMenuItem
            // 
            this.DebugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.testToolStripMenuItem1,
            this.testToolStripMenuItem2,
            this.综合算法测试ToolStripMenuItem,
            this.oPCToolStripMenuItem,
            this.时间转换工具ToolStripMenuItem,
            this.log测试ToolStripMenuItem,
            this.ToolsCalcuValueToolStripMenuItem,
            this.pSLDataToolStripMenuItem,
            this.pSLToolStripMenuItem,
            this.webTagnameIDMap接口测试ToolStripMenuItem});
            this.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem";
            this.DebugToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.DebugToolStripMenuItem.Text = "调试模式";
            this.DebugToolStripMenuItem.Click += new System.EventHandler(this.DebugToolStripMenuItem_Click);
            this.DebugToolStripMenuItem.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DebugToolStripMenuItem_MouseDown);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCondSpan2ToolStripMenuItem,
            this.mFCondSpanToolStripMenuItem,
            this.mToolStripMenuItem,
            this.mFDistribute22ToolStripMenuItem,
            this.mFDistribute12ToolStripMenuItem,
            this.mLToolStripMenuItem,
            this.mDigitalSumToolStripMenuItem,
            this.mToolStripMenuItem1,
            this.mDigitalSetStatsToolStripMenuItem,
            this.mToolStripMenuItem2,
            this.mFindMinInfoToolStripMenuItem,
            this.mIndex9ToolStripMenuItem,
            this.mIndexToolStripMenuItem,
            this.toolStripMenuItem3,
            this.mFindMinToolStripMenuItem,
            this.mNormalizeToolStripMenuItem,
            this.mToolStripMenuItem3,
            this.mCondToolStripMenuItem,
            this.mMultiAnalogSubToolStripMenuItem,
            this.mMulitToolStripMenuItem,
            this.mNToolStripMenuItem,
            this.mFOPC2MinuteToolStripMenuItem,
            this.并行时间分割ToolStripMenuItem,
            this.spanPValues4rtdbToolStripMenuItem,
            this.并行时间分割SpanPValuesToolStripMenuItem,
            this.mMultiAnalogAvgToolStripMenuItem,
            this.统计相同元素个数ToolStripMenuItem,
            this.generaltestToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.testToolStripMenuItem.Text = "算法测试";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // mCondSpan2ToolStripMenuItem
            // 
            this.mCondSpan2ToolStripMenuItem.Name = "mCondSpan2ToolStripMenuItem";
            this.mCondSpan2ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mCondSpan2ToolStripMenuItem.Text = "MLCondSpan2";
            this.mCondSpan2ToolStripMenuItem.Click += new System.EventHandler(this.mCondSpan2ToolStripMenuItem_Click);
            // 
            // mFCondSpanToolStripMenuItem
            // 
            this.mFCondSpanToolStripMenuItem.Name = "mFCondSpanToolStripMenuItem";
            this.mFCondSpanToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mFCondSpanToolStripMenuItem.Text = "MFCondSpan2";
            this.mFCondSpanToolStripMenuItem.Click += new System.EventHandler(this.mFCondSpanToolStripMenuItem_Click);
            // 
            // mToolStripMenuItem
            // 
            this.mToolStripMenuItem.Name = "mToolStripMenuItem";
            this.mToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mToolStripMenuItem.Text = "MLimitStatistics";
            this.mToolStripMenuItem.Click += new System.EventHandler(this.mToolStripMenuItem_Click);
            // 
            // mFDistribute22ToolStripMenuItem
            // 
            this.mFDistribute22ToolStripMenuItem.Name = "mFDistribute22ToolStripMenuItem";
            this.mFDistribute22ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mFDistribute22ToolStripMenuItem.Text = "MFDistribute22";
            this.mFDistribute22ToolStripMenuItem.Click += new System.EventHandler(this.mFDistribute22ToolStripMenuItem_Click);
            // 
            // mFDistribute12ToolStripMenuItem
            // 
            this.mFDistribute12ToolStripMenuItem.Name = "mFDistribute12ToolStripMenuItem";
            this.mFDistribute12ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mFDistribute12ToolStripMenuItem.Text = "MFDistribute12";
            this.mFDistribute12ToolStripMenuItem.Click += new System.EventHandler(this.mFDistribute12ToolStripMenuItem_Click);
            // 
            // mLToolStripMenuItem
            // 
            this.mLToolStripMenuItem.Name = "mLToolStripMenuItem";
            this.mLToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mLToolStripMenuItem.Text = "MLDistribute22";
            this.mLToolStripMenuItem.Click += new System.EventHandler(this.mLToolStripMenuItem_Click);
            // 
            // mDigitalSumToolStripMenuItem
            // 
            this.mDigitalSumToolStripMenuItem.Name = "mDigitalSumToolStripMenuItem";
            this.mDigitalSumToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mDigitalSumToolStripMenuItem.Text = "MMultiDigitalSum";
            this.mDigitalSumToolStripMenuItem.Click += new System.EventHandler(this.mDigitalSumToolStripMenuItem_Click);
            // 
            // mToolStripMenuItem1
            // 
            this.mToolStripMenuItem1.Name = "mToolStripMenuItem1";
            this.mToolStripMenuItem1.Size = new System.Drawing.Size(274, 22);
            this.mToolStripMenuItem1.Text = "MMultiDigitalSelect";
            this.mToolStripMenuItem1.Click += new System.EventHandler(this.mToolStripMenuItem1_Click);
            // 
            // mDigitalSetStatsToolStripMenuItem
            // 
            this.mDigitalSetStatsToolStripMenuItem.Name = "mDigitalSetStatsToolStripMenuItem";
            this.mDigitalSetStatsToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mDigitalSetStatsToolStripMenuItem.Text = "MDigitalSetStats";
            this.mDigitalSetStatsToolStripMenuItem.Click += new System.EventHandler(this.mDigitalSetStatsToolStripMenuItem_Click);
            // 
            // mToolStripMenuItem2
            // 
            this.mToolStripMenuItem2.Name = "mToolStripMenuItem2";
            this.mToolStripMenuItem2.Size = new System.Drawing.Size(274, 22);
            this.mToolStripMenuItem2.Text = "MFindMaxInfo";
            this.mToolStripMenuItem2.Click += new System.EventHandler(this.mToolStripMenuItem2_Click);
            // 
            // mFindMinInfoToolStripMenuItem
            // 
            this.mFindMinInfoToolStripMenuItem.Name = "mFindMinInfoToolStripMenuItem";
            this.mFindMinInfoToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mFindMinInfoToolStripMenuItem.Text = "MFindMinInfo";
            this.mFindMinInfoToolStripMenuItem.Click += new System.EventHandler(this.mFindMinInfoToolStripMenuItem_Click);
            // 
            // mIndex9ToolStripMenuItem
            // 
            this.mIndex9ToolStripMenuItem.Name = "mIndex9ToolStripMenuItem";
            this.mIndex9ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mIndex9ToolStripMenuItem.Text = "MIndex9";
            this.mIndex9ToolStripMenuItem.Click += new System.EventHandler(this.mIndex9ToolStripMenuItem_Click);
            // 
            // mIndexToolStripMenuItem
            // 
            this.mIndexToolStripMenuItem.Name = "mIndexToolStripMenuItem";
            this.mIndexToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mIndexToolStripMenuItem.Text = "MIndex20";
            this.mIndexToolStripMenuItem.Click += new System.EventHandler(this.mIndexToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(274, 22);
            this.toolStripMenuItem3.Text = "MFindMax";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // mFindMinToolStripMenuItem
            // 
            this.mFindMinToolStripMenuItem.Name = "mFindMinToolStripMenuItem";
            this.mFindMinToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mFindMinToolStripMenuItem.Text = "MFindMin";
            this.mFindMinToolStripMenuItem.Click += new System.EventHandler(this.mFindMinToolStripMenuItem_Click);
            // 
            // mNormalizeToolStripMenuItem
            // 
            this.mNormalizeToolStripMenuItem.Name = "mNormalizeToolStripMenuItem";
            this.mNormalizeToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mNormalizeToolStripMenuItem.Text = "MNormalize";
            this.mNormalizeToolStripMenuItem.Click += new System.EventHandler(this.mNormalizeToolStripMenuItem_Click);
            // 
            // mToolStripMenuItem3
            // 
            this.mToolStripMenuItem3.Name = "mToolStripMenuItem3";
            this.mToolStripMenuItem3.Size = new System.Drawing.Size(274, 22);
            this.mToolStripMenuItem3.Text = "MCondSpanSum";
            this.mToolStripMenuItem3.Click += new System.EventHandler(this.mToolStripMenuItem3_Click);
            // 
            // mCondToolStripMenuItem
            // 
            this.mCondToolStripMenuItem.Name = "mCondToolStripMenuItem";
            this.mCondToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mCondToolStripMenuItem.Text = "MCondSpanLong";
            this.mCondToolStripMenuItem.Click += new System.EventHandler(this.mCondToolStripMenuItem_Click);
            // 
            // mMultiAnalogSubToolStripMenuItem
            // 
            this.mMultiAnalogSubToolStripMenuItem.Name = "mMultiAnalogSubToolStripMenuItem";
            this.mMultiAnalogSubToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mMultiAnalogSubToolStripMenuItem.Text = "MMultiAnalogSub";
            this.mMultiAnalogSubToolStripMenuItem.Click += new System.EventHandler(this.mMultiAnalogSubToolStripMenuItem_Click);
            // 
            // mMulitToolStripMenuItem
            // 
            this.mMulitToolStripMenuItem.Name = "mMulitToolStripMenuItem";
            this.mMulitToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mMulitToolStripMenuItem.Text = "MMulitAnalogSum";
            this.mMulitToolStripMenuItem.Click += new System.EventHandler(this.mMulitToolStripMenuItem_Click);
            // 
            // mNToolStripMenuItem
            // 
            this.mNToolStripMenuItem.Name = "mNToolStripMenuItem";
            this.mNToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mNToolStripMenuItem.Text = "MAnalogReadCurrent";
            this.mNToolStripMenuItem.Click += new System.EventHandler(this.mNToolStripMenuItem_Click);
            // 
            // mFOPC2MinuteToolStripMenuItem
            // 
            this.mFOPC2MinuteToolStripMenuItem.Name = "mFOPC2MinuteToolStripMenuItem";
            this.mFOPC2MinuteToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mFOPC2MinuteToolStripMenuItem.Text = "MFOPC2Minute";
            this.mFOPC2MinuteToolStripMenuItem.Click += new System.EventHandler(this.mFOPC2MinuteToolStripMenuItem_Click);
            // 
            // 并行时间分割ToolStripMenuItem
            // 
            this.并行时间分割ToolStripMenuItem.Name = "并行时间分割ToolStripMenuItem";
            this.并行时间分割ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.并行时间分割ToolStripMenuItem.Text = "并行时间分割SpanPValues4SpanFilter";
            this.并行时间分割ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // spanPValues4rtdbToolStripMenuItem
            // 
            this.spanPValues4rtdbToolStripMenuItem.Name = "spanPValues4rtdbToolStripMenuItem";
            this.spanPValues4rtdbToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.spanPValues4rtdbToolStripMenuItem.Text = "并行时间分割SpanPValues4rtdb";
            this.spanPValues4rtdbToolStripMenuItem.Click += new System.EventHandler(this.spanPValues4rtdbToolStripMenuItem_Click);
            // 
            // 并行时间分割SpanPValuesToolStripMenuItem
            // 
            this.并行时间分割SpanPValuesToolStripMenuItem.Name = "并行时间分割SpanPValuesToolStripMenuItem";
            this.并行时间分割SpanPValuesToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.并行时间分割SpanPValuesToolStripMenuItem.Text = "并行时间分割SpanPValues4rdb";
            this.并行时间分割SpanPValuesToolStripMenuItem.Click += new System.EventHandler(this.并行时间分割SpanPValuesToolStripMenuItem_Click);
            // 
            // mMultiAnalogAvgToolStripMenuItem
            // 
            this.mMultiAnalogAvgToolStripMenuItem.Name = "mMultiAnalogAvgToolStripMenuItem";
            this.mMultiAnalogAvgToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.mMultiAnalogAvgToolStripMenuItem.Text = "MMultiAnalogAvg";
            this.mMultiAnalogAvgToolStripMenuItem.Click += new System.EventHandler(this.mMultiAnalogAvgToolStripMenuItem_Click);
            // 
            // 统计相同元素个数ToolStripMenuItem
            // 
            this.统计相同元素个数ToolStripMenuItem.Name = "统计相同元素个数ToolStripMenuItem";
            this.统计相同元素个数ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.统计相同元素个数ToolStripMenuItem.Text = "统计相同元素个数";
            this.统计相同元素个数ToolStripMenuItem.Click += new System.EventHandler(this.统计相同元素个数ToolStripMenuItem_Click);
            // 
            // generaltestToolStripMenuItem
            // 
            this.generaltestToolStripMenuItem.Name = "generaltestToolStripMenuItem";
            this.generaltestToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.generaltestToolStripMenuItem.Text = "generaltest";
            this.generaltestToolStripMenuItem.Click += new System.EventHandler(this.generaltestToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.testToolStripMenuItem1.Text = "计算条件表达式测试";
            this.testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click_1);
            // 
            // testToolStripMenuItem2
            // 
            this.testToolStripMenuItem2.Name = "testToolStripMenuItem2";
            this.testToolStripMenuItem2.Size = new System.Drawing.Size(208, 22);
            this.testToolStripMenuItem2.Text = "计算时间段过滤测试";
            this.testToolStripMenuItem2.Click += new System.EventHandler(this.testToolStripMenuItem2_Click);
            // 
            // 综合算法测试ToolStripMenuItem
            // 
            this.综合算法测试ToolStripMenuItem.Name = "综合算法测试ToolStripMenuItem";
            this.综合算法测试ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.综合算法测试ToolStripMenuItem.Text = "综合算法测试";
            this.综合算法测试ToolStripMenuItem.Click += new System.EventHandler(this.综合算法测试ToolStripMenuItem_Click);
            // 
            // oPCToolStripMenuItem
            // 
            this.oPCToolStripMenuItem.Name = "oPCToolStripMenuItem";
            this.oPCToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.oPCToolStripMenuItem.Text = "OPC接口测试";
            this.oPCToolStripMenuItem.Click += new System.EventHandler(this.oPCToolStripMenuItem_Click);
            // 
            // 时间转换工具ToolStripMenuItem
            // 
            this.时间转换工具ToolStripMenuItem.Name = "时间转换工具ToolStripMenuItem";
            this.时间转换工具ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.时间转换工具ToolStripMenuItem.Text = "时间转换工具";
            this.时间转换工具ToolStripMenuItem.Click += new System.EventHandler(this.时间转换工具ToolStripMenuItem_Click);
            // 
            // log测试ToolStripMenuItem
            // 
            this.log测试ToolStripMenuItem.Name = "log测试ToolStripMenuItem";
            this.log测试ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.log测试ToolStripMenuItem.Text = "Log测试";
            this.log测试ToolStripMenuItem.Click += new System.EventHandler(this.log测试ToolStripMenuItem_Click);
            // 
            // ToolsCalcuValueToolStripMenuItem
            // 
            this.ToolsCalcuValueToolStripMenuItem.Name = "ToolsCalcuValueToolStripMenuItem";
            this.ToolsCalcuValueToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.ToolsCalcuValueToolStripMenuItem.Text = "插值计算工具";
            this.ToolsCalcuValueToolStripMenuItem.Click += new System.EventHandler(this.ToolsCalcuValueToolStripMenuItem_Click);
            // 
            // pSLDataToolStripMenuItem
            // 
            this.pSLDataToolStripMenuItem.Name = "pSLDataToolStripMenuItem";
            this.pSLDataToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.pSLDataToolStripMenuItem.Text = "PSLData接口测试";
            this.pSLDataToolStripMenuItem.Click += new System.EventHandler(this.pSLDataToolStripMenuItem_Click);
            // 
            // pSLToolStripMenuItem
            // 
            this.pSLToolStripMenuItem.Name = "pSLToolStripMenuItem";
            this.pSLToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.pSLToolStripMenuItem.Text = "PSLTagnameIDMap接口测试";
            this.pSLToolStripMenuItem.Click += new System.EventHandler(this.pSLToolStripMenuItem_Click);
            // 
            // webTagnameIDMap接口测试ToolStripMenuItem
            // 
            this.webTagnameIDMap接口测试ToolStripMenuItem.Name = "webTagnameIDMap接口测试ToolStripMenuItem";
            this.webTagnameIDMap接口测试ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.webTagnameIDMap接口测试ToolStripMenuItem.Text = "WebTagnameIDMap接口测试";
            this.webTagnameIDMap接口测试ToolStripMenuItem.Click += new System.EventHandler(this.webTagnameIDMap接口测试ToolStripMenuItem_Click);
            // 
            // AppRunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1364, 537);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "AppRunForm";
            this.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "精石源信息计算引擎";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppRunForm_FormClosing);
            this.Load += new System.EventHandler(this.AppRunForm_Load);
            this.Resize += new System.EventHandler(this.AppRunForm_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton tsbt_Start;
        private System.Windows.Forms.ToolStripButton tsbt_Quit;
        private System.Windows.Forms.Button bt_Fixed;
        private System.Windows.Forms.TextBox tb_Fastfixed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lV_DataList;
        private System.Windows.Forms.ColumnHeader sourcetagname;
        private System.Windows.Forms.ColumnHeader index;
        private System.Windows.Forms.ColumnHeader sourcetagdesc;
        private System.Windows.Forms.ColumnHeader sourcetagdim;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label lb_index;
        private System.Windows.Forms.CheckBox checkBox_SelectAll;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iniToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rTDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem iniToolTable;
        private System.Windows.Forms.ToolStripMenuItem extractModulesInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calcuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.Button btn_SetStartDate;
        private System.Windows.Forms.DateTimePicker dtStartTime;  //计算起始时间
        private System.Windows.Forms.DateTimePicker dtStartDate;  //计算起始日期
        private System.Windows.Forms.Label tCalcuStartDate;
        private System.Windows.Forms.ColumnHeader sourcetagmrb;
        private System.Windows.Forms.ColumnHeader sourcetagmre;
        private System.Windows.Forms.ColumnHeader fmodulename;
        private System.Windows.Forms.ColumnHeader fgroup;
        private System.Windows.Forms.ColumnHeader fclass;
        private System.Windows.Forms.ColumnHeader falgorithmsflag;
        private System.Windows.Forms.ColumnHeader fparas;
        private System.Windows.Forms.ColumnHeader fcondpslnames;
        private System.Windows.Forms.ColumnHeader forder;
        private System.Windows.Forms.ColumnHeader foutputtable;
        private System.Windows.Forms.ColumnHeader foutputnumber;
        private System.Windows.Forms.ColumnHeader foutputpsltagnames;
        private System.Windows.Forms.ColumnHeader finterval;
        private System.Windows.Forms.ColumnHeader fdelay;
        private System.Windows.Forms.ColumnHeader fstarttime;
        private System.Windows.Forms.ColumnHeader fendtime;
        private System.Windows.Forms.ColumnHeader fnexttime;
        private System.Windows.Forms.ColumnHeader sourcetagdb;
        private System.Windows.Forms.ToolStripStatusLabel tslb_CalcuConfig;
        private System.Windows.Forms.ToolStripStatusLabel tslb_CalcuErrors;
        private System.Windows.Forms.ToolStripMenuItem DebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tslb_CalcuWarnings;
        private System.Windows.Forms.Button btn_exportCSV;
        private System.Windows.Forms.ToolStripMenuItem historyCalcuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem historyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CheckTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExcelToolStripMenuItem;
        private System.Windows.Forms.ComboBox cb_intervaltype;
        private System.Windows.Forms.CheckBox checkBox_SelectIntervalType;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCondSpan2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mFCondSpanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mFDistribute22ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mFDistribute12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDigitalSumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mDigitalSetStatsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mFindMinInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mIndex9ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mFindMinToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mNormalizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mCondToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMultiAnalogSubToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMulitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 综合算法测试ToolStripMenuItem;
        private System.Windows.Forms.ComboBox cb_moduletype;
        private System.Windows.Forms.CheckBox checkBox_SelectModule;
        private System.Windows.Forms.ToolStripMenuItem oPCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mFOPC2MinuteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 检查并抽取值次信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemSearch;
        private System.Windows.Forms.ToolStripMenuItem rectifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 并行时间分割ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spanPValues4rtdbToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 并行时间分割SpanPValuesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMultiAnalogAvgToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 统计相同元素个数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generaltestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateTagnameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导入得分权重信息ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AutoReCalcuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReadConstInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 时间转换工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemSearchData;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemDelData;
        private System.Windows.Forms.ToolStripMenuItem uniqeCheckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem log测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsCalcuValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 概化数据表维护ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openIndexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkCurveToolStripMenuItem;
        private System.Windows.Forms.Button bt_searchHelp;
        private System.Windows.Forms.ToolStripMenuItem pSLDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tslb_CurrentIndex;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem deleteCalcuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pSLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webTagnameIDMap接口测试ToolStripMenuItem;
    }
}