using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCCommon;

namespace DBInterface.RTDBInterface
{  

    /// <summary>
    /// IRTDbHelperExpand
    /// 实时数据库访问扩展接口
    /// 用于定义各种实时数据库中非共性的部分。
    /// 比如golden数据库断面数据写入和断面数据查询。
    /// 
    /// 修改纪录
    /// 
    ///		2016.7.13 版本：1.0 arrow 实时数据库访问扩展接口。
    /// 
    /// 版本：1.0
    /// 
    /// <author>
    ///		<name>arrow</name>
    ///		<date>2016.07.13</date>
    /// </author> 
    /// </summary>
    public interface IRTDbHelperExpand
    {
        /// <summary>
        /// 断面浮点数据读取
        /// </summary>
        /// <param name="dataTable">源内存数据表</param>
        //在mysql connect net的手册中没有关于bulkcopy的接口描述
        List<PValue> GetCrossSectionValue(string[] tagnames,DateTime datetime );
        List<PValue> GetCrossSectionValue(int[] tagids, DateTime datetime);       

    }
    
}
