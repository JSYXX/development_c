namespace PSLCalcu
{
    partial class DeleteConfig
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
            this.tb_calcuIndexs = new System.Windows.Forms.TextBox();
            this.lb_calcuIndexs = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bt_delelt4calcu);
            this.groupBox2.Controls.Add(this.tb_calcuIndexs);
            this.groupBox2.Controls.Add(this.lb_calcuIndexs);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(12, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(406, 47);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "输入要删除的计算项";
            // 
            // bt_delelt4calcu
            // 
            this.bt_delelt4calcu.Location = new System.Drawing.Point(341, 15);
            this.bt_delelt4calcu.Name = "bt_delelt4calcu";
            this.bt_delelt4calcu.Size = new System.Drawing.Size(59, 26);
            this.bt_delelt4calcu.TabIndex = 65;
            this.bt_delelt4calcu.Text = ">> 删除";
            this.bt_delelt4calcu.UseVisualStyleBackColor = true;
            this.bt_delelt4calcu.Click += new System.EventHandler(this.bt_delelt4calcu_Click);
            // 
            // tb_calcuIndexs
            // 
            this.tb_calcuIndexs.Location = new System.Drawing.Point(88, 18);
            this.tb_calcuIndexs.Name = "tb_calcuIndexs";
            this.tb_calcuIndexs.Size = new System.Drawing.Size(247, 21);
            this.tb_calcuIndexs.TabIndex = 36;
            // 
            // lb_calcuIndexs
            // 
            this.lb_calcuIndexs.Location = new System.Drawing.Point(6, 22);
            this.lb_calcuIndexs.Name = "lb_calcuIndexs";
            this.lb_calcuIndexs.Size = new System.Drawing.Size(87, 18);
            this.lb_calcuIndexs.TabIndex = 64;
            this.lb_calcuIndexs.Text = "计算项序号：";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(18, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 18);
            this.label1.TabIndex = 66;
            this.label1.Text = "计算项序号输入方式：";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(18, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(281, 18);
            this.label2.TabIndex = 67;
            this.label2.Text = "—单个计算项，直接输入计算项的序号。比如“2”。";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(18, 169);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(375, 18);
            this.label3.TabIndex = 68;
            this.label3.Text = "—多个计算项，直接输入计算项的序号，用逗号分隔。比如“2,3”。";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(18, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(419, 18);
            this.label4.TabIndex = 69;
            this.label4.Text = "—连续计算项，输入起始项和截止项序号，中间用-分隔。比如“100-150”。";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(18, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(164, 18);
            this.label5.TabIndex = 70;
            this.label5.Text = "—删除计算项对应标签的所有数据";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(18, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(164, 18);
            this.label6.TabIndex = 71;
            this.label6.Text = "—删除对应的标签";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(18, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(400, 18);
            this.label7.TabIndex = 72;
            this.label7.Text = "—删除对应的计算项（新版计算引擎不再要求计算下序号连续，可以删除中间某些计算项）";
            // 
            // DeleteConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 222);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Name = "DeleteConfig";
            this.Text = "删除已导入的计算配置";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button bt_delelt4calcu;
        private System.Windows.Forms.TextBox tb_calcuIndexs;
        private System.Windows.Forms.Label lb_calcuIndexs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}