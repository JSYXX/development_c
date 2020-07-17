using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCCommon;

namespace DBInterface.RTDBInterface
{
    /// <summary>
    /// IRTDbHelper
    /// 实时数据库访问层标准接口。
    /// 
    /// 修改纪录
    ///     
    ///		2016.12.18 版本：1.0 arrow 创建标准接口。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.12.18</date>
    /// </author> 
    /// </summary>
    public interface IRTDbHelper : IDisposable
    {
        /// <summary>
        /// 当前实时库数据库类型。该参数必须在具体的实时数据库连接方法中重定义。
        /// </summary>
        CurrentRTDbType CurrentRTDbType { get; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 默认打开关闭数据库选项（默认为否）
        /// </summary>
        bool AutoOpenClose { get; set; }
        
        /// <summary>
        /// 异常信息
        /// </summary>
        string Exception { get; }

        /// <summary>
        /// 是否登录
        /// </summary>
        bool isLogOn { get; }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns>连接状态</returns>
        String Logon();       

        /// <summary>
        /// 断开数据库连接
        /// </summary>
        /// <returns></returns>
        void Logoff();

        /// <summary>
        /// 获取数据库服务器当前时间        
        /// </summary>
        /// <param name=""></param>       
        /// <returns>服务器当前时间</returns>
        DateTime GetHostDateTime();

        /// <summary>
        /// 批量检查数据库标签点是否存在
        /// 一次检查多个标签，返回数据库中找不到的标签点
        /// </summary>
        /// <param name="tagnames">标签名称</param>       
        /// <returns>数据库中不存在的标签点</returns>
        List<string> CheckTags(string[] tagnames);

        /// <summary>
        /// 获取标签列表
        /// 获得数据库中的所有标签名称
        /// </summary>
        /// <param name=""></param>       
        /// <returns>数据库中的所有标签名称</returns>
        List<SignalComm> GetTagList(string tagname, string tablename = "");                  //PGIM没有表的概念
                
        /// <summary>
        /// 批量读取数据库标签点的当前值（即时值、快照值）
        /// 一次读取多个标签，返回多个标签对应的PValue值
        /// </summary>
        /// <param name="tagnames">标签名称</param>       
        /// <returns>当前值PValue</returns>
        List<PValue> GetActValues(string[] tagnames);
        List<PValue> GetActValues(int[] tagids);

        /// <summary>
        /// 单个读取数据库标签点的当前值（即时值、快照值）
        /// 一次读取一个标签，返回一个标签对应的PValue值
        /// </summary>
        /// <param name="tagnames">标签名称</param>       
        /// <returns>当前值PValue</returns>
        PValue GetActValue(string tagname);
        PValue GetActValue(int tagid);

        /// <summary>
        /// 批量写入数据库标签点的当前值（即时值、快照值）
        /// 一次写入多个标签的PValue值
        /// </summary>
        /// <param name="tagnames">标签名称</param>       
        /// <returns>返回写入状态</returns>
        int SetActValues(string[] tagnames, List<PValue> pvalues);
        int SetActValues(int[] tagids, List<PValue> pvalues);

        /// <summary>
        /// 单个写入数据库标签点的当前值（即时值、快照值）
        /// 一次写入一个标签的PValue值
        /// </summary>
        /// <param name="tagnames">标签名称</param>       
        /// <returns>返回写入状态</returns>
        int SetActValue(string tagname, PValue pvalue);
        int SetActValue(int tagid, PValue pvalue);

        /// <summary>
        /// 读取数据库标签点的历史数据原始值
        /// 一次读取一个标签一段时间内的原始值，返回pvalues
        /// </summary>
        /// <param name="tagname">标签名称</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="DateTime">结束时间</param>
        /// <returns>历史值pvalues</returns>
        List<PValue> GetRawValues(string tagname, DateTime startdate, DateTime enddate);
        List<PValue> GetRawValues(int tagid, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 读取数据库标签点的历史数据插值:固定间隔
        /// 一次读取一个标签一段时间内的插值，返回pvalues
        /// </summary>
        /// <param name="tagname">标签名称</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="DateTime">结束时间</param>
        /// <param name="count">插值间隔(毫秒数)</param>
        /// <returns>历史值pvalues</returns>
        List<PValue> GetIntervalValuesFixInterval(string tagname, DateTime startdate, DateTime enddate, int interval);
        List<PValue> GetIntervalValuesFixInterval(int tagid, DateTime startdate, DateTime enddate, int interval);

        /// <summary>
        /// 读取数据库标签点的历史数据统计值
        /// </summary>
        /// <param name="tagname">标签名称</param>
        /// <param name="startdate">起始时间</param>
        /// <param name="endate">结束时间</param>
        /// <param name="type">统计值类型</param>
        /// <returns>历史值PValue</returns>
        PValue GetSummaryValues(string tagname, DateTime startdate, DateTime enddate, string type);
        PValue GetSummaryValues(int tagid, DateTime startdate, DateTime enddate, string type);

        /// <summary>
        /// 写入数据库标签点的历史数据
        /// 说明：计算引擎只读不写，该写入功能主要用来向数据库插入测试数据
        /// </summary>
        /// <param name="tagname">标签名称</param>
        /// <param name="data">要写入的数据</param>
        /// <returns>成功写入的记录数</returns>
        int PutArchivedValues(string tagname, string[][] data);
        int PutArchivedValues(int tagid, string[][] data);


    }
}
