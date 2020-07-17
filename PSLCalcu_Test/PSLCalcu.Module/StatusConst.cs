using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSLCalcu.Module
{
    //算法组件中，数据状态位枚举
    /*改到PCCommon中
    enum  StatusConst
    {
        //0           正常
        //

        //500-600     算法公用错误描述
        //1000-1100   算法特有错误描述
        Normal=0,
        InputOverLimit = 6,     //输入数据超限  



        InputIsNull=10,          //计算的输入数据为空
             
        
        
        UnknownError =500,       //未知错误         
        
        OutOfValid4Bias = 1000,       //偏差计算参考指标超出有效范围
        InvalidPoint =1010,           //壁温计算中某温度点输入值存在状态位不为0的无效点。

    }
    */ 
    //算法组件中，计算错误类型
    enum CalcuError
    { 
        
    }
}
