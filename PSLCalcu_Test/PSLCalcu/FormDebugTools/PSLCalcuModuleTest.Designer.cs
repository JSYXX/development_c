namespace PSLCalcu
{
    partial class PSLCalcuModuleTest
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
            this.label9 = new System.Windows.Forms.Label();
            this.cb_calcumodulename = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_csvdata = new System.Windows.Forms.Button();
            this.lb_csvdatafile = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_calcupara = new System.Windows.Forms.TextBox();
            this.lv_calcuresults = new System.Windows.Forms.ListView();
            this.Id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.desc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TimeStamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EndTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lb_calcuspan = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_sourctagnames = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(12, 45);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 18);
            this.label9.TabIndex = 51;
            this.label9.Text = "选择计算组件：";
            // 
            // cb_calcumodulename
            // 
            this.cb_calcumodulename.FormattingEnabled = true;
            this.cb_calcumodulename.Location = new System.Drawing.Point(99, 40);
            this.cb_calcumodulename.Name = "cb_calcumodulename";
            this.cb_calcumodulename.Size = new System.Drawing.Size(179, 20);
            this.cb_calcumodulename.TabIndex = 52;
            this.cb_calcumodulename.SelectedIndexChanged += new System.EventHandler(this.cb_calcumodulename_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 18);
            this.label1.TabIndex = 53;
            this.label1.Text = "选择数据文件：";
            // 
            // bt_csvdata
            // 
            this.bt_csvdata.Location = new System.Drawing.Point(101, 93);
            this.bt_csvdata.Name = "bt_csvdata";
            this.bt_csvdata.Size = new System.Drawing.Size(67, 20);
            this.bt_csvdata.TabIndex = 54;
            this.bt_csvdata.Text = "选择文件";
            this.bt_csvdata.UseVisualStyleBackColor = true;
            this.bt_csvdata.Click += new System.EventHandler(this.bt_csvdata_Click);
            // 
            // lb_csvdatafile
            // 
            this.lb_csvdatafile.Location = new System.Drawing.Point(174, 97);
            this.lb_csvdatafile.Name = "lb_csvdatafile";
            this.lb_csvdatafile.Size = new System.Drawing.Size(476, 18);
            this.lb_csvdatafile.TabIndex = 55;
            this.lb_csvdatafile.Text = "数据文件路径";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(29, 126);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 20);
            this.button1.TabIndex = 56;
            this.button1.Text = ">> 开始计算";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 18);
            this.label2.TabIndex = 57;
            this.label2.Text = "计算参数：";
            // 
            // tb_calcupara
            // 
            this.tb_calcupara.Location = new System.Drawing.Point(99, 66);
            this.tb_calcupara.Name = "tb_calcupara";
            this.tb_calcupara.Size = new System.Drawing.Size(471, 21);
            this.tb_calcupara.TabIndex = 58;
            this.tb_calcupara.TextChanged += new System.EventHandler(this.lb_calcupara_TextChanged);
            // 
            // lv_calcuresults
            // 
            this.lv_calcuresults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Index,
            this.name,
            this.desc,
            this.Value,
            this.TimeStamp,
            this.EndTime});
            this.lv_calcuresults.Location = new System.Drawing.Point(12, 152);
            this.lv_calcuresults.Name = "lv_calcuresults";
            this.lv_calcuresults.Size = new System.Drawing.Size(645, 442);
            this.lv_calcuresults.TabIndex = 59;
            this.lv_calcuresults.UseCompatibleStateImageBehavior = false;
            this.lv_calcuresults.View = System.Windows.Forms.View.Details;
            // 
            // Id
            // 
            this.Id.Text = "Id";
            this.Id.Width = 30;
            // 
            // Index
            // 
            this.Index.Text = "Index";
            this.Index.Width = 30;
            // 
            // name
            // 
            this.name.Text = "name";
            // 
            // desc
            // 
            this.desc.Text = "Desc";
            this.desc.Width = 240;
            // 
            // Value
            // 
            this.Value.Text = "Value";
            // 
            // TimeStamp
            // 
            this.TimeStamp.Text = "TimeStamp";
            this.TimeStamp.Width = 140;
            // 
            // EndTime
            // 
            this.EndTime.Text = "EndTime";
            this.EndTime.Width = 140;
            // 
            // lb_calcuspan
            // 
            this.lb_calcuspan.Location = new System.Drawing.Point(503, 130);
            this.lb_calcuspan.Name = "lb_calcuspan";
            this.lb_calcuspan.Size = new System.Drawing.Size(135, 18);
            this.lb_calcuspan.TabIndex = 64;
            this.lb_calcuspan.Text = "0";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(137, 130);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(373, 18);
            this.label16.TabIndex = 63;
            this.label16.Text = "计算用时(仅指公式计算需要的时间，不包含读数据和写结果时间)：";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 18);
            this.label3.TabIndex = 65;
            this.label3.Text = "源标签名称：";
            // 
            // tb_sourctagnames
            // 
            this.tb_sourctagnames.Location = new System.Drawing.Point(99, 12);
            this.tb_sourctagnames.Name = "tb_sourctagnames";
            this.tb_sourctagnames.Size = new System.Drawing.Size(471, 21);
            this.tb_sourctagnames.TabIndex = 66;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(576, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 18);
            this.label4.TabIndex = 67;
            this.label4.Text = "仅某些算法需要";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(576, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 18);
            this.label5.TabIndex = 68;
            this.label5.Text = "仅某些算法需要";
            // 
            // PSLCalcuModuleTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 606);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_sourctagnames);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lb_calcuspan);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.lv_calcuresults);
            this.Controls.Add(this.tb_calcupara);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lb_csvdatafile);
            this.Controls.Add(this.bt_csvdata);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cb_calcumodulename);
            this.Controls.Add(this.label9);
            this.Name = "PSLCalcuModuleTest";
            this.Text = "PSLCalcuModuleTest";
            this.Load += new System.EventHandler(this.PSLCalcuModuleTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cb_calcumodulename;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bt_csvdata;
        private System.Windows.Forms.Label lb_csvdatafile;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_calcupara;
        private System.Windows.Forms.ListView lv_calcuresults;
        private System.Windows.Forms.ColumnHeader Id;
        private System.Windows.Forms.ColumnHeader Index;
        private System.Windows.Forms.ColumnHeader TimeStamp;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader EndTime;
        private System.Windows.Forms.ColumnHeader desc;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.Label lb_calcuspan;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_sourctagnames;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}