namespace PSLCalcu
{
    partial class ReadConstInfo2PSLData
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
            this.bt_read = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dtEndDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtStartDate = new System.Windows.Forms.DateTimePicker();
            this.tCalcuStartDate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bt_read
            // 
            this.bt_read.Location = new System.Drawing.Point(277, 78);
            this.bt_read.Name = "bt_read";
            this.bt_read.Size = new System.Drawing.Size(144, 22);
            this.bt_read.TabIndex = 35;
            this.bt_read.Text = "开始读取--->";
            this.bt_read.UseVisualStyleBackColor = true;
            this.bt_read.Click += new System.EventHandler(this.bt_read_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(401, 12);
            this.label1.TabIndex = 34;
            this.label1.Text = "指定读取常数配置表的时间段：只有在指定时间段之内的配置表才会被读取";
            // 
            // dtEndDate
            // 
            this.dtEndDate.Location = new System.Drawing.Point(297, 39);
            this.dtEndDate.Name = "dtEndDate";
            this.dtEndDate.Size = new System.Drawing.Size(119, 21);
            this.dtEndDate.TabIndex = 33;
            this.dtEndDate.ValueChanged += new System.EventHandler(this.dtEndDate_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(226, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 32;
            this.label2.Text = "结束时间：";
            // 
            // dtStartDate
            // 
            this.dtStartDate.Location = new System.Drawing.Point(91, 39);
            this.dtStartDate.Name = "dtStartDate";
            this.dtStartDate.Size = new System.Drawing.Size(119, 21);
            this.dtStartDate.TabIndex = 31;
            this.dtStartDate.ValueChanged += new System.EventHandler(this.dtStartDate_ValueChanged);
            // 
            // tCalcuStartDate
            // 
            this.tCalcuStartDate.AutoSize = true;
            this.tCalcuStartDate.Location = new System.Drawing.Point(20, 45);
            this.tCalcuStartDate.Name = "tCalcuStartDate";
            this.tCalcuStartDate.Size = new System.Drawing.Size(65, 12);
            this.tCalcuStartDate.TabIndex = 30;
            this.tCalcuStartDate.Text = "起始时间：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(20, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(209, 12);
            this.label3.TabIndex = 36;
            this.label3.Text = "注意，常数配置向必须在第一张表中。";
            // 
            // ReadConstInfo2PSLData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 112);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bt_read);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtEndDate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtStartDate);
            this.Controls.Add(this.tCalcuStartDate);
            this.Name = "ReadConstInfo2PSLData";
            this.Text = "读取常数标签值到概化数据库";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_read;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtEndDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtStartDate;
        private System.Windows.Forms.Label tCalcuStartDate;
        private System.Windows.Forms.Label label3;
    }
}