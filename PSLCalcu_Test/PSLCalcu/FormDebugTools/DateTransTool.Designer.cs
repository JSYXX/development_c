namespace PSLCalcu
{
    partial class DateTransTool
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
            this.tCalcuStartDate = new System.Windows.Forms.Label();
            this.tb_rawticks = new System.Windows.Forms.TextBox();
            this.btn_transdate = new System.Windows.Forms.Button();
            this.tb_resultdate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dt_startdate = new System.Windows.Forms.DateTimePicker();
            this.dt_starttime = new System.Windows.Forms.DateTimePicker();
            this.btn_transticks = new System.Windows.Forms.Button();
            this.tb_resultticks = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tCalcuStartDate
            // 
            this.tCalcuStartDate.AutoSize = true;
            this.tCalcuStartDate.Location = new System.Drawing.Point(12, 9);
            this.tCalcuStartDate.Name = "tCalcuStartDate";
            this.tCalcuStartDate.Size = new System.Drawing.Size(113, 12);
            this.tCalcuStartDate.TabIndex = 13;
            this.tCalcuStartDate.Text = "请输入时间Tick值：";
            // 
            // tb_rawticks
            // 
            this.tb_rawticks.Location = new System.Drawing.Point(120, 6);
            this.tb_rawticks.Name = "tb_rawticks";
            this.tb_rawticks.Size = new System.Drawing.Size(130, 21);
            this.tb_rawticks.TabIndex = 14;
            // 
            // btn_transdate
            // 
            this.btn_transdate.Location = new System.Drawing.Point(268, 6);
            this.btn_transdate.Name = "btn_transdate";
            this.btn_transdate.Size = new System.Drawing.Size(100, 23);
            this.btn_transdate.TabIndex = 24;
            this.btn_transdate.Text = "转换为日期->>";
            this.btn_transdate.UseVisualStyleBackColor = true;
            this.btn_transdate.Click += new System.EventHandler(this.btn_exportCSV_Click);
            // 
            // tb_resultdate
            // 
            this.tb_resultdate.Location = new System.Drawing.Point(390, 6);
            this.tb_resultdate.Name = "tb_resultdate";
            this.tb_resultdate.ReadOnly = true;
            this.tb_resultdate.Size = new System.Drawing.Size(148, 21);
            this.tb_resultdate.TabIndex = 25;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 26;
            this.label1.Text = "请输入日期值：";
            // 
            // dt_startdate
            // 
            this.dt_startdate.Location = new System.Drawing.Point(120, 37);
            this.dt_startdate.Name = "dt_startdate";
            this.dt_startdate.Size = new System.Drawing.Size(130, 21);
            this.dt_startdate.TabIndex = 53;
            this.dt_startdate.ValueChanged += new System.EventHandler(this.dt_startdate_ValueChanged);
            // 
            // dt_starttime
            // 
            this.dt_starttime.CustomFormat = "HH:mm:ss";
            this.dt_starttime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_starttime.Location = new System.Drawing.Point(120, 64);
            this.dt_starttime.Name = "dt_starttime";
            this.dt_starttime.ShowUpDown = true;
            this.dt_starttime.Size = new System.Drawing.Size(130, 21);
            this.dt_starttime.TabIndex = 54;
            this.dt_starttime.ValueChanged += new System.EventHandler(this.dt_starttime_ValueChanged);
            // 
            // btn_transticks
            // 
            this.btn_transticks.Location = new System.Drawing.Point(268, 38);
            this.btn_transticks.Name = "btn_transticks";
            this.btn_transticks.Size = new System.Drawing.Size(100, 23);
            this.btn_transticks.TabIndex = 55;
            this.btn_transticks.Text = "转换为tick值->>";
            this.btn_transticks.UseVisualStyleBackColor = true;
            this.btn_transticks.Click += new System.EventHandler(this.btn_transticks_Click);
            // 
            // tb_resultticks
            // 
            this.tb_resultticks.Location = new System.Drawing.Point(390, 40);
            this.tb_resultticks.Name = "tb_resultticks";
            this.tb_resultticks.ReadOnly = true;
            this.tb_resultticks.Size = new System.Drawing.Size(148, 21);
            this.tb_resultticks.TabIndex = 56;
            // 
            // DateTransTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 96);
            this.Controls.Add(this.tb_resultticks);
            this.Controls.Add(this.btn_transticks);
            this.Controls.Add(this.dt_startdate);
            this.Controls.Add(this.dt_starttime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_resultdate);
            this.Controls.Add(this.btn_transdate);
            this.Controls.Add(this.tb_rawticks);
            this.Controls.Add(this.tCalcuStartDate);
            this.Name = "DateTransTool";
            this.Text = "DateTransTool";
            this.Load += new System.EventHandler(this.DateTransTool_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label tCalcuStartDate;
        private System.Windows.Forms.TextBox tb_rawticks;
        private System.Windows.Forms.Button btn_transdate;
        private System.Windows.Forms.TextBox tb_resultdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dt_startdate;
        private System.Windows.Forms.DateTimePicker dt_starttime;
        private System.Windows.Forms.Button btn_transticks;
        private System.Windows.Forms.TextBox tb_resultticks;
    }
}