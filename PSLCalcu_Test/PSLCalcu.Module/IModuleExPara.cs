using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;   //使用正则表达式

namespace PSLCalcu.Module
{
    /// <summary>
    /// IModuleExPara
    /// 单入多出概化计算引擎计算模块扩展接口：计算模块计算条件。
    /// 计算模块条件是指，在统计算法满足一定的外部条件时，才进行循环统计。条件本身不参与计算内部的循环。
    /// 
    /// 这些外部条件往往是指其他标签（可以是数字量或者模拟量）满足一定的逻辑条件。
    /// 比如：当负荷>300Mw且主蒸汽流量>200t时
    /// 其返回值，是符合这些外部条件的时间段序列。
    /// 
    /// ——一个标签的统计值，可能对应多个计算条件，那么在条件表里就对应多条计算条件配置
    /// 
    /// 修改纪录
    ///    
    ///		2017.02.27 版本：1.0 gaofeng。
    /// 
    /// 版本：2.0
    /// 
    /// <author>
    ///		<name>GaoFeng</name>
    ///		<date>2017.02.27</date>
    /// </author> 
    /// </summary>
    
        
    public interface IModuleExPara
    {
        /// <summary>
        /// 计算模块需要的参数示例：计算模块参数示例，在具体计算模块中实现
        /// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        /// </summary>
        string moduleParaExample { get; }

        /// <summary>
        /// 计算模块需要的参数解释：计算模块参数解释，在具体计算模块中实现
        /// 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        /// </summary>
        string moduleParaDesc { get; }

        /// <summary>
        /// 计算模块需要的参数正则表达式：计算模块正则表达式，在具体计算模块中实现
        /// </summary>
        Regex moduleParaRegex { get; }
        
    }
}
