namespace PSLCalcu
{
    partial class DateValue
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
            this.dt_startdate = new System.Windows.Forms.DateTimePicker();
            this.dt_starttime = new System.Windows.Forms.DateTimePicker();
            this.label9 = new System.Windows.Forms.Label();
            this.dt_enddate = new System.Windows.Forms.DateTimePicker();
            this.dt_endtime = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dt_insertdate = new System.Windows.Forms.DateTimePicker();
            this.dt_inserttime = new System.Windows.Forms.DateTimePicker();
            this.tb_startvalue = new System.Windows.Forms.TextBox();
            this.tb_endvalue = new System.Windows.Forms.TextBox();
            this.tb_insertvalue = new System.Windows.Forms.TextBox();
            this.bt_export4calcu = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dt_startdate
            // 
            this.dt_startdate.Location = new System.Drawing.Point(116, 19);
            this.dt_startdate.Name = "dt_startdate";
            this.dt_startdate.Size = new System.Drawing.Size(119, 21);
            this.dt_startdate.TabIndex = 54;
            this.dt_startdate.ValueChanged += new System.EventHandler(this.dt_startdate_ValueChanged);
            // 
            // dt_starttime
            // 
            this.dt_starttime.CustomFormat = "HH:mm:ss";
            this.dt_starttime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_starttime.Location = new System.Drawing.Point(241, 19);
            this.dt_starttime.Name = "dt_starttime";
            this.dt_starttime.ShowUpDown = true;
            this.dt_starttime.Size = new System.Drawing.Size(73, 21);
            this.dt_starttime.TabIndex = 55;
            this.dt_starttime.ValueChanged += new System.EventHandler(this.dt_starttime_ValueChanged);
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(29, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 18);
            this.label9.TabIndex = 53;
            this.label9.Text = "前有效点时刻：";
            // 
            // dt_enddate
            // 
            this.dt_enddate.Location = new System.Drawing.Point(116, 50);
            this.dt_enddate.Name = "dt_enddate";
            this.dt_enddate.Size = new System.Drawing.Size(119, 21);
            this.dt_enddate.TabIndex = 67;
            this.dt_enddate.ValueChanged += new System.EventHandler(this.dt_enddate_ValueChanged);
            // 
            // dt_endtime
            // 
            this.dt_endtime.CustomFormat = "HH:mm:ss";
            this.dt_endtime.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.dt_endtime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_endtime.Location = new System.Drawing.Point(241, 50);
            this.dt_endtime.Name = "dt_endtime";
            this.dt_endtime.ShowUpDown = true;
            this.dt_endtime.Size = new System.Drawing.Size(73, 21);
            this.dt_endtime.TabIndex = 68;
            this.dt_endtime.ValueChanged += new System.EventHandler(this.dt_endtime_ValueChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(28, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 18);
            this.label2.TabIndex = 69;
            this.label2.Text = "后有效点时刻：";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(29, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 70;
            this.label1.Text = "插值时刻：";
            // 
            // dt_insertdate
            // 
            this.dt_insertdate.Location = new System.Drawing.Point(116, 86);
            this.dt_insertdate.Name = "dt_insertdate";
            this.dt_insertdate.Size = new System.Drawing.Size(119, 21);
            this.dt_insertdate.TabIndex = 71;
            this.dt_insertdate.ValueChanged += new System.EventHandler(this.dt_insertdate_ValueChanged);
            // 
            // dt_inserttime
            // 
            this.dt_inserttime.CustomFormat = "HH:mm:ss";
            this.dt_inserttime.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.dt_inserttime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_inserttime.Location = new System.Drawing.Point(241, 86);
            this.dt_inserttime.Name = "dt_inserttime";
            this.dt_inserttime.ShowUpDown = true;
            this.dt_inserttime.Size = new System.Drawing.Size(73, 21);
            this.dt_inserttime.TabIndex = 72;
            this.dt_inserttime.ValueChanged += new System.EventHandler(this.dt_inserttime_ValueChanged);
            // 
            // tb_startvalue
            // 
            this.tb_startvalue.Location = new System.Drawing.Point(348, 19);
            this.tb_startvalue.Name = "tb_startvalue";
            this.tb_startvalue.Size = new System.Drawing.Size(184, 21);
            this.tb_startvalue.TabIndex = 73;
            // 
            // tb_endvalue
            // 
            this.tb_endvalue.Location = new System.Drawing.Point(348, 50);
            this.tb_endvalue.Name = "tb_endvalue";
            this.tb_endvalue.Size = new System.Drawing.Size(184, 21);
            this.tb_endvalue.TabIndex = 74;
            // 
            // tb_insertvalue
            // 
            this.tb_insertvalue.Location = new System.Drawing.Point(348, 86);
            this.tb_insertvalue.Name = "tb_insertvalue";
            this.tb_insertvalue.Size = new System.Drawing.Size(184, 21);
            this.tb_insertvalue.TabIndex = 75;
            // 
            // bt_export4calcu
            // 
            this.bt_export4calcu.Location = new System.Drawing.Point(383, 117);
            this.bt_export4calcu.Name = "bt_export4calcu";
            this.bt_export4calcu.Size = new System.Drawing.Size(59, 26);
            this.bt_export4calcu.TabIndex = 76;
            this.bt_export4calcu.Text = ">> 计算";
            this.bt_export4calcu.UseVisualStyleBackColor = true;
            this.bt_export4calcu.Click += new System.EventHandler(this.bt_export4calcu_Click);
            // 
            // DateValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 155);
            this.Controls.Add(this.bt_export4calcu);
            this.Controls.Add(this.tb_insertvalue);
            this.Controls.Add(this.tb_endvalue);
            this.Controls.Add(this.tb_startvalue);
            this.Controls.Add(this.dt_insertdate);
            this.Controls.Add(this.dt_inserttime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dt_enddate);
            this.Controls.Add(this.dt_endtime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dt_startdate);
            this.Controls.Add(this.dt_starttime);
            this.Controls.Add(this.label9);
            this.Name = "DateValue";
            this.Text = "DateValue";
            this.Load += new System.EventHandler(this.DateValue_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dt_startdate;
        private System.Windows.Forms.DateTimePicker dt_starttime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DateTimePicker dt_enddate;
        private System.Windows.Forms.DateTimePicker dt_endtime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dt_insertdate;
        private System.Windows.Forms.DateTimePicker dt_inserttime;
        private System.Windows.Forms.TextBox tb_startvalue;
        private System.Windows.Forms.TextBox tb_endvalue;
        private System.Windows.Forms.TextBox tb_insertvalue;
        private System.Windows.Forms.Button bt_export4calcu;
    }
}