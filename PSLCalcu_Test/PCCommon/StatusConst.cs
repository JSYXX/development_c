using System;
using System.ComponentModel;

namespace PCCommon
{
    //概化数据状态位定义（注意，与实时数据状态位定义无关）
    public enum  StatusConst
    {
        //0           正常
        [Description("正常")] 
        Normal=0,
        [Description("PValue两点式插值错误")]
        InsertError = 5,                  //PValue两点式插值错误，起始时间等于截止时间
        
        //
        [Description("计算的输入数据为空")]
        InputIsNull=100,                 //计算的输入数据为空
        [Description("计算的输入数据超限")]
        InputOverLimit = 101,            //输入数据超限 

        //500-600     算法公用错误描述
        [Description("算法组件中的未知错误")]
        UnknownError =500,              //未知错误         
        
        //1000-1100   算法特有错误描述
        [Description("偏差算法参考指标超限")]
        OutOfValid4Bias = 1000,         //偏差计算参考指标超出有效范围
        [Description("壁温分析算法输入值存在状态不为0的点")]
        InvalidPoint =1010,             //壁温计算中某温度点输入值存在状态位不为0的无效点。
        [Description("壁温分析算法设备点中有空值")]
        NullInDevice = 1020,             //壁温计算中某温度点输入值存在状态位不为0的无效点。
        [Description("壁温分析算法设备点中有状态异常")]
        ErrorStatusInDevice = 1021,             //壁温计算中某温度点输入值存在状态位不为0的无效点。

    }
    //算法组件中，计算错误类型
    public enum CalcuError
    { 
        
    }
}
