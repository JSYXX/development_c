namespace PSLCalcu
{
    partial class HistoryCalcuAuto
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.tCalcuStartDate = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lv_SelectedCalcuList = new PSLCalcu.DoubleBufferListView();
            this.sindex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ssourcetagname = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ssourcetagdb = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ssourcetagdesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ssourcetagdim = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ssourcetagmrb = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ssourcetagmre = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfmodulename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfgroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sforder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfclass = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfalgorithmsflag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfparas = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfcondpslnames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfoutputtable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfoutputnumber = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfoutputpsltagnames = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfinterval = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfdelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfprogress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sfwarnings = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.errors = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cb_CheckTags = new System.Windows.Forms.CheckBox();
            this.lb_CalcuWarnings = new System.Windows.Forms.Label();
            this.lb_CalcuErrors = new System.Windows.Forms.Label();
            this.rb_step = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rb_insert = new System.Windows.Forms.RadioButton();
            this.bt_historyCalcu = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_reviseAllCalcuItems = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label_selectedCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(445, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 42;
            this.label5.Text = "0:00:00";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(192, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 41;
            this.label4.Text = "0:00:00";
            // 
            // dtEndDate
            // 
            this.dtEndDate.Location = new System.Drawing.Point(324, 21);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Size = new System.Drawing.Size(119, 21);
            this.dtEndDate.TabIndex = 40;
            this.dtEndDate.ValueChanged += new System.EventHandler(this.dtEndDate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(262, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 39;
            this.label2.Text = "结束时间：";
            // 
            // dtStartDate
            // 
            this.dtStartDate.Location = new System.Drawing.Point(68, 22);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Size = new System.Drawing.Size(119, 21);
            this.dtStartDate.TabIndex = 38;
            this.dtStartDate.ValueChanged += new System.EventHandler(this.dtStartDate_ValueChanged);
            // 
            // tCalcuStartDate
            // 
            this.tCalcuStartDate.AutoSize = true;
            this.tCalcuStartDate.Location = new System.Drawing.Point(6, 26);
            this.tCalcuStartDate.Name = "tCalcuStartDate";
            this.tCalcuStartDate.Size = new System.Drawing.Size(65, 12);
            this.tCalcuStartDate.TabIndex = 37;
            this.tCalcuStartDate.Text = "起始时间：";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lv_SelectedCalcuList);
            this.panel2.Location = new System.Drawing.Point(12, 76);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1331, 586);
            this.panel2.TabIndex = 48;
            // 
            // lv_SelectedCalcuList
            // 
            this.lv_SelectedCalcuList.CheckBoxes = true;
            this.lv_SelectedCalcuList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sindex,
            this.ssourcetagname,
            this.ssourcetagdb,
            this.ssourcetagdesc,
            this.ssourcetagdim,
            this.ssourcetagmrb,
            this.ssourcetagmre,
            this.sfmodulename,
            this.sfgroup,
            this.sforder,
            this.sfclass,
            this.sfalgorithmsflag,
            this.sfparas,
            this.sfcondpslnames,
            this.sfoutputtable,
            this.sfoutputnumber,
            this.sfoutputpsltagnames,
            this.sfinterval,
            this.sfdelay,
            this.sfprogress,
            this.sfwarnings,
            this.errors});
            this.lv_SelectedCalcuList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lv_SelectedCalcuList.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lv_SelectedCalcuList.FullRowSelect = true;
            this.lv_SelectedCalcuList.GridLines = true;
            this.lv_SelectedCalcuList.Location = new System.Drawing.Point(0, 0);
            this.lv_SelectedCalcuList.Margin = new System.Windows.Forms.Padding(9, 10, 9, 10);
            this.lv_SelectedCalcuList.Name = "lv_SelectedCalcuList";
            this.lv_SelectedCalcuList.Size = new System.Drawing.Size(1331, 586);
            this.lv_SelectedCalcuList.TabIndex = 6;
            this.lv_SelectedCalcuList.UseCompatibleStateImageBehavior = false;
            this.lv_SelectedCalcuList.View = System.Windows.Forms.View.Details;
            // 
            // sindex
            // 
            this.sindex.Text = "Index";
            // 
            // ssourcetagname
            // 
            this.ssourcetagname.Text = "SourceTag";
            this.ssourcetagname.Width = 120;
            // 
            // ssourcetagdb
            // 
            this.ssourcetagdb.Text = "SourcDB";
            this.ssourcetagdb.Width = 40;
            // 
            // ssourcetagdesc
            // 
            this.ssourcetagdesc.Text = "Description";
            this.ssourcetagdesc.Width = 120;
            // 
            // ssourcetagdim
            // 
            this.ssourcetagdim.Text = "Dim";
            this.ssourcetagdim.Width = 5;
            // 
            // ssourcetagmrb
            // 
            this.ssourcetagmrb.Text = "MRB";
            this.ssourcetagmrb.Width = 5;
            // 
            // ssourcetagmre
            // 
            this.ssourcetagmre.Text = "MRE";
            this.ssourcetagmre.Width = 5;
            // 
            // sfmodulename
            // 
            this.sfmodulename.Text = "CalcuType";
            this.sfmodulename.Width = 80;
            // 
            // sfgroup
            // 
            this.sfgroup.Text = "group";
            this.sfgroup.Width = 5;
            // 
            // sforder
            // 
            this.sforder.Text = "Order";
            this.sforder.Width = 5;
            // 
            // sfclass
            // 
            this.sfclass.Text = "Class";
            this.sfclass.Width = 5;
            // 
            // sfalgorithmsflag
            // 
            this.sfalgorithmsflag.Text = "Flag";
            this.sfalgorithmsflag.Width = 40;
            // 
            // sfparas
            // 
            this.sfparas.Text = "Para";
            this.sfparas.Width = 5;
            // 
            // sfcondpslnames
            // 
            this.sfcondpslnames.Text = "Condition";
            this.sfcondpslnames.Width = 120;
            // 
            // sfoutputtable
            // 
            this.sfoutputtable.Text = "OutputTable";
            this.sfoutputtable.Width = 5;
            // 
            // sfoutputnumber
            // 
            this.sfoutputnumber.Text = "OutputNumber";
            this.sfoutputnumber.Width = 5;
            // 
            // sfoutputpsltagnames
            // 
            this.sfoutputpsltagnames.Text = "OutputTags";
            this.sfoutputpsltagnames.Width = 120;
            // 
            // sfinterval
            // 
            this.sfinterval.Text = "Interval";
            // 
            // sfdelay
            // 
            this.sfdelay.Text = "Delay";
            this.sfdelay.Width = 40;
            // 
            // sfprogress
            // 
            this.sfprogress.Text = "Progress";
            this.sfprogress.Width = 120;
            // 
            // sfwarnings
            // 
            this.sfwarnings.Text = "Warning";
            // 
            // errors
            // 
            this.errors.Text = "Error";
            // 
            // cb_CheckTags
            // 
            this.cb_CheckTags.AutoSize = true;
            this.cb_CheckTags.Enabled = false;
            this.cb_CheckTags.Location = new System.Drawing.Point(839, 43);
            this.cb_CheckTags.Name = "cb_CheckTags";
            this.cb_CheckTags.Size = new System.Drawing.Size(120, 16);
            this.cb_CheckTags.TabIndex = 52;
            this.cb_CheckTags.Text = "计算前检查标签点";
            this.cb_CheckTags.UseVisualStyleBackColor = true;
            // 
            // lb_CalcuWarnings
            // 
            this.lb_CalcuWarnings.AutoSize = true;
            this.lb_CalcuWarnings.Location = new System.Drawing.Point(919, 682);
            this.lb_CalcuWarnings.Name = "lb_CalcuWarnings";
            this.lb_CalcuWarnings.Size = new System.Drawing.Size(53, 12);
            this.lb_CalcuWarnings.TabIndex = 51;
            this.lb_CalcuWarnings.Text = "警告提示";
            // 
            // lb_CalcuErrors
            // 
            this.lb_CalcuErrors.AutoSize = true;
            this.lb_CalcuErrors.Location = new System.Drawing.Point(1151, 682);
            this.lb_CalcuErrors.Name = "lb_CalcuErrors";
            this.lb_CalcuErrors.Size = new System.Drawing.Size(53, 12);
            this.lb_CalcuErrors.TabIndex = 50;
            this.lb_CalcuErrors.Text = "错误提示";
            // 
            // rb_step
            // 
            this.rb_step.AutoSize = true;
            this.rb_step.Location = new System.Drawing.Point(6, 22);
            this.rb_step.Name = "rb_step";
            this.rb_step.Size = new System.Drawing.Size(107, 16);
            this.rb_step.TabIndex = 26;
            this.rb_step.TabStop = true;
            this.rb_step.Text = "取前值(阶梯线)";
            this.rb_step.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rb_step);
            this.groupBox2.Controls.Add(this.rb_insert);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(555, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 53);
            this.groupBox2.TabIndex = 53;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "起止时刻点插值方式";
            // 
            // rb_insert
            // 
            this.rb_insert.AutoSize = true;
            this.rb_insert.Enabled = false;
            this.rb_insert.Location = new System.Drawing.Point(128, 23);
            this.rb_insert.Name = "rb_insert";
            this.rb_insert.Size = new System.Drawing.Size(101, 16);
            this.rb_insert.TabIndex = 25;
            this.rb_insert.TabStop = true;
            this.rb_insert.Text = "取插值（折线)";
            this.rb_insert.UseVisualStyleBackColor = true;
            // 
            // bt_historyCalcu
            // 
            this.bt_historyCalcu.Location = new System.Drawing.Point(1199, 39);
            this.bt_historyCalcu.Name = "bt_historyCalcu";
            this.bt_historyCalcu.Size = new System.Drawing.Size(144, 22);
            this.bt_historyCalcu.TabIndex = 54;
            this.bt_historyCalcu.Text = "开始历史数据计算--->";
            this.bt_historyCalcu.UseVisualStyleBackColor = true;
            this.bt_historyCalcu.Click += new System.EventHandler(this.bt_historyCalcu_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtStartDate);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtEndDate);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tCalcuStartDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(15, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(519, 54);
            this.groupBox1.TabIndex = 54;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "历史数据重算的起止时刻";
            // 
            // cb_reviseAllCalcuItems
            // 
            this.cb_reviseAllCalcuItems.AutoSize = true;
            this.cb_reviseAllCalcuItems.Enabled = false;
            this.cb_reviseAllCalcuItems.Location = new System.Drawing.Point(839, 21);
            this.cb_reviseAllCalcuItems.Name = "cb_reviseAllCalcuItems";
            this.cb_reviseAllCalcuItems.Size = new System.Drawing.Size(240, 16);
            this.cb_reviseAllCalcuItems.TabIndex = 56;
            this.cb_reviseAllCalcuItems.Text = "把实时计算时间统一修改到重算截止时刻";
            this.cb_reviseAllCalcuItems.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(167, 682);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 59;
            this.label8.Text = "条";
            // 
            // label_selectedCount
            // 
            this.label_selectedCount.AutoSize = true;
            this.label_selectedCount.Location = new System.Drawing.Point(153, 682);
            this.label_selectedCount.Name = "label_selectedCount";
            this.label_selectedCount.Size = new System.Drawing.Size(11, 12);
            this.label_selectedCount.TabIndex = 58;
            this.label_selectedCount.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 682);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 57;
            this.label3.Text = "当前共选定配置项:";
            // 
            // HistoryCalcuAuto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 703);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label_selectedCount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cb_reviseAllCalcuItems);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bt_historyCalcu);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cb_CheckTags);
            this.Controls.Add(this.lb_CalcuWarnings);
            this.Controls.Add(this.lb_CalcuErrors);
            this.Controls.Add(this.panel2);
            this.Name = "HistoryCalcuAuto";
            this.Text = "自动设置历史数据重算";
            this.Load += new System.EventHandler(this.HistoryCalcuAuto_Load);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtStartDate;
        private System.Windows.Forms.Label tCalcuStartDate;
        private System.Windows.Forms.Panel panel2;
        private DoubleBufferListView lv_SelectedCalcuList;
        private System.Windows.Forms.ColumnHeader sindex;
        private System.Windows.Forms.ColumnHeader ssourcetagname;
        private System.Windows.Forms.ColumnHeader ssourcetagdb;
        private System.Windows.Forms.ColumnHeader ssourcetagdesc;
        private System.Windows.Forms.ColumnHeader ssourcetagdim;
        private System.Windows.Forms.ColumnHeader ssourcetagmrb;
        private System.Windows.Forms.ColumnHeader ssourcetagmre;
        private System.Windows.Forms.ColumnHeader sfmodulename;
        private System.Windows.Forms.ColumnHeader sfgroup;
        private System.Windows.Forms.ColumnHeader sforder;
        private System.Windows.Forms.ColumnHeader sfclass;
        private System.Windows.Forms.ColumnHeader sfalgorithmsflag;
        private System.Windows.Forms.ColumnHeader sfparas;
        private System.Windows.Forms.ColumnHeader sfcondpslnames;
        private System.Windows.Forms.ColumnHeader sfoutputtable;
        private System.Windows.Forms.ColumnHeader sfoutputnumber;
        private System.Windows.Forms.ColumnHeader sfoutputpsltagnames;
        private System.Windows.Forms.ColumnHeader sfinterval;
        private System.Windows.Forms.ColumnHeader sfdelay;
        private System.Windows.Forms.ColumnHeader sfprogress;
        private System.Windows.Forms.ColumnHeader sfwarnings;
        private System.Windows.Forms.ColumnHeader errors;
        private System.Windows.Forms.CheckBox cb_CheckTags;
        private System.Windows.Forms.Label lb_CalcuWarnings;
        private System.Windows.Forms.Label lb_CalcuErrors;
        private System.Windows.Forms.RadioButton rb_step;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rb_insert;
        private System.Windows.Forms.Button bt_historyCalcu;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cb_reviseAllCalcuItems;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_selectedCount;
        private System.Windows.Forms.Label label3;
    }
}