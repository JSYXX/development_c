namespace PSLCalcu
{
    partial class SearchHelp
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
            this.lb_HelpInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_HelpInfo
            // 
            this.lb_HelpInfo.AutoSize = true;
            this.lb_HelpInfo.Location = new System.Drawing.Point(12, 9);
            this.lb_HelpInfo.Name = "lb_HelpInfo";
            this.lb_HelpInfo.Size = new System.Drawing.Size(53, 12);
            this.lb_HelpInfo.TabIndex = 47;
            this.lb_HelpInfo.Text = "帮助说明";
            this.lb_HelpInfo.Click += new System.EventHandler(this.lb_HelpInfo_Click);
            // 
            // SearchHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 334);
            this.Controls.Add(this.lb_HelpInfo);
            this.Name = "SearchHelp";
            this.Text = "SearchHelp";
            this.Load += new System.EventHandler(this.SearchHelp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_HelpInfo;
    }
}