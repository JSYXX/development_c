using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon; //使用PValue

namespace PSLCalcu.Module
{
    /// <summary>
    /// IModule
    /// 单入多出概化计算引擎计算模块标准接口。
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
    public interface IModule
    {
        /// <summary>
        /// 计算模块的名称：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        string moduleName { get; }

        /// <summary>
        /// 计算模块的描述：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        string moduleDesc { get; }

        /// <summary>
        /// 计算模块的描述：计算模块输入数量，在具体计算模块中实现
        /// </summary>
        int inputNumber { get; }

        /// <summary>
        /// 计算模块的描述：计算模块输入描述，在具体计算模块中实现
        /// </summary>
        string inputDescsCN { get; }

        /// <summary>
        /// 计算模块包含算法：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        string algorithms { get; }

        /// <summary>
        /// 计算模块计算结果标志位：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        string algorithmsflag { get; }     

        /// <summary>
        /// 计算结果的结果数量：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        int outputNumber { get;  }     

        /// <summary>
        /// 计算结果的描述，以分号分隔：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        string outputDescs { get;  }

        /// <summary>
        /// 计算结果的描述，以分号分隔：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        string outputDescsCN { get; }

        /// <summary>
        /// 计算结果写入的数据表：计算模块信息参数，在具体计算模块中实现
        /// </summary>
        string outputTable { get;  }

        /// <summary>
        /// 计算结果是否可以为空：计算模块信息参数，该参数为true，则主程序不检测计算结果是否为空
        /// </summary>
        bool outputPermitNULL { get; }

        
    }
}
