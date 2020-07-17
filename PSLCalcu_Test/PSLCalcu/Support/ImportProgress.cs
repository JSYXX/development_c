using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PSLCalcu
{
    public partial class ImportProgress : Form
    {
        #region 属性

        /// <summary>
        /// 设置进度条的当前值
        /// </summary>
        public int ProgressValue
        {
            get { return this.progressBar.Value; }
            set { progressBar.Value = value; }
        }
        /// <summary>
        /// 设置进度条的文字
        /// </summary>
        public string ProgressText
        {
            get { return this.progressLabel.Text; }
            set { progressLabel.Text = value; }
        }
        #endregion

        //特别注意，在window的form设计器中，form中控件的初始化在InitializeComponent中。
        //如果该form在其他地方要用构造的方式创建实例，则必须手动写form的构造函数，把InitializeComponent()加入到构造函数。此时form中的控件才能一并实例化
        public ImportProgress() 
        {
            InitializeComponent();
        }
        
        private void ImportProgress_Load(object sender, EventArgs e)
        {

        }

        private void ImportProgress_FormClosed(object sender, EventArgs e)
        { }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }
            
    }
}
