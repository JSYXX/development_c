namespace PSLCalcu
{
    partial class UpdateTagname
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
            this.label1 = new System.Windows.Forms.Label();
            this.tb_currentname = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_aftername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bt_search = new System.Windows.Forms.Button();
            this.cb_SelectAll = new System.Windows.Forms.CheckBox();
            this.bt_dorevise = new System.Windows.Forms.Button();
            this.lv_searchresults = new System.Windows.Forms.ListView();
            this.CalcuIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TagId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TagName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ReviseName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Field = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "原标签名称：";
            // 
            // tb_currentname
            // 
            this.tb_currentname.Location = new System.Drawing.Point(78, 93);
            this.tb_currentname.Name = "tb_currentname";
            this.tb_currentname.Size = new System.Drawing.Size(223, 21);
            this.tb_currentname.TabIndex = 1;
            this.tb_currentname.TextChanged += new System.EventHandler(this.tb_currentname_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(307, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "修改后的标签名称：";
            // 
            // tb_aftername
            // 
            this.tb_aftername.Location = new System.Drawing.Point(412, 93);
            this.tb_aftername.Name = "tb_aftername";
            this.tb_aftername.Size = new System.Drawing.Size(226, 21);
            this.tb_aftername.TabIndex = 3;
            this.tb_aftername.TextChanged += new System.EventHandler(this.tb_aftername_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "更新计算配置中的标签信息：";
            // 
            // bt_search
            // 
            this.bt_search.Location = new System.Drawing.Point(644, 91);
            this.bt_search.Name = "bt_search";
            this.bt_search.Size = new System.Drawing.Size(50, 23);
            this.bt_search.TabIndex = 5;
            this.bt_search.Text = "查询";
            this.bt_search.UseVisualStyleBackColor = true;
            this.bt_search.Click += new System.EventHandler(this.bt_search_Click);
            // 
            // cb_SelectAll
            // 
            this.cb_SelectAll.AutoSize = true;
            this.cb_SelectAll.Location = new System.Drawing.Point(15, 121);
            this.cb_SelectAll.Name = "cb_SelectAll";
            this.cb_SelectAll.Size = new System.Drawing.Size(48, 16);
            this.cb_SelectAll.TabIndex = 13;
            this.cb_SelectAll.Text = "全选";
            this.cb_SelectAll.UseVisualStyleBackColor = true;
            this.cb_SelectAll.CheckedChanged += new System.EventHandler(this.cb_SelectAll_CheckedChanged);
            // 
            // bt_dorevise
            // 
            this.bt_dorevise.Location = new System.Drawing.Point(220, 463);
            this.bt_dorevise.Name = "bt_dorevise";
            this.bt_dorevise.Size = new System.Drawing.Size(50, 23);
            this.bt_dorevise.TabIndex = 14;
            this.bt_dorevise.Text = "修改";
            this.bt_dorevise.UseVisualStyleBackColor = true;
            this.bt_dorevise.Click += new System.EventHandler(this.bt_dorevise_Click);
            // 
            // lv_searchresults
            // 
            this.lv_searchresults.CheckBoxes = true;
            this.lv_searchresults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CalcuIndex,
            this.Index,
            this.Field,
            this.TagName,
            this.TagId,
            this.ReviseName});
            this.lv_searchresults.Location = new System.Drawing.Point(12, 142);
            this.lv_searchresults.Name = "lv_searchresults";
            this.lv_searchresults.Size = new System.Drawing.Size(974, 298);
            this.lv_searchresults.TabIndex = 15;
            this.lv_searchresults.UseCompatibleStateImageBehavior = false;
            this.lv_searchresults.View = System.Windows.Forms.View.Details;
            this.lv_searchresults.SelectedIndexChanged += new System.EventHandler(this.lv_searchresults_SelectedIndexChanged);
            // 
            // CalcuIndex
            // 
            this.CalcuIndex.Text = "CalcuIndex";
            this.CalcuIndex.Width = 80;
            // 
            // Index
            // 
            this.Index.Text = "Index";
            this.Index.Width = 45;
            // 
            // TagId
            // 
            this.TagId.Text = "TagId";
            this.TagId.Width = 200;
            // 
            // TagName
            // 
            this.TagName.Text = "TagName";
            this.TagName.Width = 250;
            // 
            // ReviseName
            // 
            this.ReviseName.Text = "ReviseName";
            this.ReviseName.Width = 250;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 453);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "修改内容包括：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 468);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(161, 12);
            this.label5.TabIndex = 17;
            this.label5.Text = "—计算配置中对应的标签名称";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 483);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "—标签id映射表中对应的名称";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(461, 12);
            this.label7.TabIndex = 19;
            this.label7.Text = "—只有在变更标签名称时标签id保持不变的情况下，才能使用此功能对表亲进行替换。";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(533, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "—如果可能，标签变化后尽量采用初始化数据表，重新导入计算配置表的方式对标签名称进行更新。";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 66);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(377, 12);
            this.label9.TabIndex = 21;
            this.label9.Text = "—查询到含有原标签名称的对象后，要仔细辨别是否为要修改的对象。";
            // 
            // Field
            // 
            this.Field.Text = "Field";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(65, 123);
            this.label10.Name = "label10";
            this.label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label10.Size = new System.Drawing.Size(833, 12);
            this.label10.TabIndex = 22;
            this.label10.Text = "—特别注意：如果使用该界面修改标签名称，必须一次性正确选中所有要修改的标签。否则，再次修改时，会由于映射表已经改变，而找不到标签映射关系。";
            // 
            // UpdateTagname
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 505);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lv_searchresults);
            this.Controls.Add(this.bt_dorevise);
            this.Controls.Add(this.cb_SelectAll);
            this.Controls.Add(this.bt_search);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_aftername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_currentname);
            this.Controls.Add(this.label1);
            this.Name = "UpdateTagname";
            this.Text = "更新计算配置中的标签信息";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_currentname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_aftername;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bt_search;
        private System.Windows.Forms.CheckBox cb_SelectAll;
        private System.Windows.Forms.Button bt_dorevise;
        private System.Windows.Forms.ListView lv_searchresults;
        private System.Windows.Forms.ColumnHeader CalcuIndex;
        private System.Windows.Forms.ColumnHeader Index;
        private System.Windows.Forms.ColumnHeader TagId;
        private System.Windows.Forms.ColumnHeader TagName;
        private System.Windows.Forms.ColumnHeader ReviseName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ColumnHeader Field;
        private System.Windows.Forms.Label label10;
    }
}