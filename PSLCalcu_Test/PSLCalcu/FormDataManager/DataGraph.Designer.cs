namespace PSLCalcu
{
    partial class DataGraph
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>        

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
            this.bt_Save = new System.Windows.Forms.Button();
            this.z1 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(744, 22);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(75, 26);
            this.bt_Save.TabIndex = 45;
            this.bt_Save.Text = "保存图像";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // z1
            // 
            this.z1.Location = new System.Drawing.Point(12, 12);
            this.z1.Name = "z1";
            this.z1.ScrollGrace = 0D;
            this.z1.ScrollMaxX = 0D;
            this.z1.ScrollMaxY = 0D;
            this.z1.ScrollMaxY2 = 0D;
            this.z1.ScrollMinX = 0D;
            this.z1.ScrollMinY = 0D;
            this.z1.ScrollMinY2 = 0D;
            this.z1.Size = new System.Drawing.Size(816, 446);
            this.z1.TabIndex = 1;
            // 
            // DataGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(841, 499);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.z1);
            this.Name = "DataGraph";
            this.Text = "数据曲线";
            this.Load += new System.EventHandler(this.DataGraph_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl z1;
        private System.Windows.Forms.Button bt_Save;
        private System.ComponentModel.IContainer components;
    }
}