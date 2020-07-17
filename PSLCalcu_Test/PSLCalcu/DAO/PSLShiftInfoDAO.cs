using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBInterface.RDBInterface;         //使用关系数据库接口
using Config;   //使用log
using System.Data;                      //使用IDataReader
using System.Windows.Forms;

namespace PSLCalcu.DAO
{
    /// <summary>
    /// 值次信息数据表DAO
    /// </summary>
    public class PSLShiftInfoDAO
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLHistoryCalcuConfigDAO));       //全局log
        private static string psldataTableName = "psldata";

        #region 公有变量
        public static bool ErrorFlag = false;                                                               //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        #endregion

        #region 公用函数
        public static List<ShiftItem> Read()
        {
            string sqlStr;
            string databasename = "psldb";
            string tablename = "pslshiftinfo";
            try
            {
                List<ShiftItem> shiftinfo=new List<ShiftItem>();
                return shiftinfo;
            }
            catch (Exception ex)
            {
                string msgStr = String.Format("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                MessageBox.Show(msgStr);
                return null;
            }
        }
        public static  bool Write(string[][] csvdata)
        { 
            
            string sqlStr;
            string databasename = "psldb";
            string tablename = "pslshiftinfo";
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                string msgStr = String.Format("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                MessageBox.Show(msgStr);
                return false;
            }
        }
        #endregion
    }
}
