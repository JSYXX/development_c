using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;                //使用反射
using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Data;                      //使用IDataReader
using System.Windows.Forms;             //使用messagebox
using System.Text.RegularExpressions;   //使用正则表达式
using Config;   //使用log

namespace PSLCalcu
{   
    
    /// <summary>
    /// 概化计算引擎关系库DAO
    /// 计算模块信息表DAO
    /// 1、概化计算引擎面向的关系库，默认的database是psldb，因此在config中的关系库配置中，默认的database应该是psldb。
    /// </summary>
    public class PSLModulesDAO
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(PSLModulesDAO));       //全局log
        
        #region 公有变量
        public static bool ErrorFlag = false;                                                   //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        #endregion
        
        #region 公有方法
        //清空计算模块信息表
        public static bool ClearData()
        {
            string sqlStr = "";
            try 
            { 
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use psldb;truncate table pslmodules");
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLModulesDAO.ClearData()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //抽取计算模块信息写入到计算模块信息表
        public static bool extractData()
        {
            string sqlStr = "";            
            try
            {
                DbHelper dbhelper = new DbHelper();
                Dictionary<string, string> outputdesclist = new Dictionary<string,string>();                           //存放所有计算模块输出描述，用来判断不同计算模块之间的输出描述是否有重复。如果有重复，则会造成标签自动命名的错误问题。
                int sum = 0;            //错误统计
                int outDescSum = 0;     //计算结果描述重复

                Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);       //获得"PSLCalcu.Module.dll"
                Type[] types = assembly.GetTypes();                                         //获得PSLCalcu.Module.dll中的所有class
                string messageStr="";
                foreach (Type type in types)                                                //对class进行遍历
                {
                    if (type.GetInterface("IModule", true) != null)                         //找到实现了IModule接口的计算模块class
                    {
                        object tmpModuleObj = Activator.CreateInstance(type);               //获取计算公式类实例

                        //PropertyInfo GetModuleName = type.GetProperty("moduleName");                    
                        //string modulename=(string)GetModuleName.GetValue(tmpModuleObj, null);
                        string modulename = type.Name;                                      //20170315修改，主程序抽取算法类信息时，算法名字将直接取算法类类名，而不再使用moduleName属性，以此保证moduleName不会重名。

                        PropertyInfo GetModuleDesc = type.GetProperty("moduleDesc");
                        string moduledesc = (string)GetModuleDesc.GetValue(tmpModuleObj, null);             //计算公式模块属性：算法公式描述

                        PropertyInfo GetAlgorithms = type.GetProperty("algorithms");
                        string algorithms = (string)GetAlgorithms.GetValue(tmpModuleObj, null);             //计算公式模块属性：包含算法

                        PropertyInfo GetAlgorithmsFlag = type.GetProperty("algorithmsflag");
                        string algorithmsflag = (string)GetAlgorithmsFlag.GetValue(tmpModuleObj, null);
                        

                        PropertyInfo GetOutputNumber = type.GetProperty("outputNumber");
                        int outputNumber = (int)GetOutputNumber.GetValue(tmpModuleObj, null);               //计算公式模块属性：输出数量

                        PropertyInfo GetOutputDescs = type.GetProperty("outputDescs");
                        string outputdescs = (string)GetOutputDescs.GetValue(tmpModuleObj, null);           //计算公式模块属性：输出描述

                        PropertyInfo GetOutputDescsCN = type.GetProperty("outputDescsCN");
                        string outputdescscn = (string)GetOutputDescsCN.GetValue(tmpModuleObj, null);       //计算公式模块属性：输出描述中文
                                               
                        PropertyInfo GetOutputTable = type.GetProperty("outputTable");
                        string outputTable = (string)GetOutputTable.GetValue(tmpModuleObj, null);           //计算公式模块属性：输出table

                        PropertyInfo GetOutputPermitNULL = type.GetProperty("outputPermitNULL");
                        int outputpermitnull = Convert.ToInt32(GetOutputPermitNULL.GetValue(tmpModuleObj, null));           //计算公式模块属性：输出table

                        string moduleparaexample = "";
                        string moduleparadesc = "";
                        Regex modulepararegex = new Regex(@"^$");
                        if (type.GetInterface("IModuleExPara", true) != null) {
                            PropertyInfo GetModuleParaExample = type.GetProperty("moduleParaExample");      //计算公式模块属性：参数示例
                            moduleparaexample = (string)GetModuleParaExample.GetValue(tmpModuleObj, null);
                            
                            PropertyInfo GetModuleParaDesc = type.GetProperty("moduleParaDesc");
                            moduleparadesc = (string)GetModuleParaDesc.GetValue(tmpModuleObj, null);        //计算公式模块属性：参数描述
                            //moduleparadesc = "";

                            PropertyInfo GetModuleParaRegex = type.GetProperty("moduleParaRegex");
                            modulepararegex = (Regex)GetModuleParaRegex.GetValue(tmpModuleObj, null);
                        }
                        
                        //对抽取的公式信息进行检查，如果没有错误，则将当前公式信息写入数据库。
                        //如果有错误，在检查程序中写入log，并返回出错的数量。

                        sum += checkAlgorithmsFlag(modulename, outputNumber, algorithmsflag);               //检查标志的数量和字母是否符合要求
                       
                        sum += checkOutputDescsNumber(modulename, outputNumber, outputdescs);               //检查输出描述的数量是否符合要求，计算引擎根据该项自动生成标签，如果数量不对，会造成计算引擎错误
                      
                        //sum += checkOutputDescsUnique(modulename, ref outputdesclist,outputdescs);        //检查输出描述是否和已有输出描述重复
                        sum += checkPara(modulename, moduleparaexample, modulepararegex);                   //检查条件参数是否需要，如果需要moduleParaExample是否符合moduleParaRegex
                    
                       
                        //计算结果描述，不做强制要求，错误不计入sum，不会导致不能抽取计算组件信息
                        outDescSum = checkOutputDescsUnique(modulename, ref outputdesclist, outputdescs);          //检查输出描述是否和已有输出描述重复
                        if (outDescSum != 0)
                        {
                            string MessageStr=String.Format("算法{0}的计算结果描述与其他算法有重复。" + Environment.NewLine +
                                            "——对计算结果描述有重复不做硬性要求，但原则上不同算法计算描述应该不重复。" + Environment.NewLine +
                                            "——请仔细检查log，以确认重复的描述不会造成计算标签重名。" + Environment.NewLine +
                                            "——目前已知一维偏差和二维偏差计算结果描述相同。"+ Environment.NewLine +
                                            "——目前已知实时数据过滤算法和分段过滤算法计算结果描述相同。",
                                            modulename
                                            );
                            MessageBox.Show(MessageStr);
                        }
                        if (sum == 0)
                        {
                            string fileds = "modulename,moduledesc,moduleclass,modulealgorithms,modulealgorithmsflag,moduleparaexample,moduleparadesc,moduleoutputnumber,moduleoutputtype,moduleoutputdescs,moduleoutputdescscn,moduleoutputtable,moduleoutputpermitnull";
                            //概化计算引擎关系库默认database是psldb时，可以不用use psldb
                            //概化计算引擎关系库默认database不是psldb时，必须使用use psldb
                            sqlStr = String.Format("use psldb;insert into pslmodules({0}) values ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", fileds, modulename, moduledesc, "", algorithms, algorithmsflag, moduleparaexample, moduleparadesc, outputNumber, "", outputdescs, outputdescscn, outputTable, outputpermitnull);
                            Console.WriteLine("模块名称"+modulename);
                            dbhelper.ExecuteNonQuery(sqlStr);
                        }
                        else
                        {                            
                          
                        }
                                        
                    }//end if
                }//end foreach(Type t in types)            

                //如果出错数量为0，则返回true。不为0则返回false
                if (sum == 0)
                {
                    return true;
                }
                else
                {
                    //string messageStr = String.Format("抽取计算模块信息时发生错误，请检查Log文件！");
                    //MessageBox.Show(messageStr);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLModulesDAO.extractData()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}",ex.ToString());
                logHelper.Error(messageStr);             
                
                return false;
            }
        }
        //从计算模块信息表读取计算模块信息
        public static List<PSLModule> ReadData()
        {
            string sqlStr = "";
            try
            {
                DbHelper dbhelper = new DbHelper();
                string fields = "pslmodules.id,"                        //计算模块的信息                    
                                 + "pslmodules.modulename,"
                                 + "pslmodules.moduledesc,"
                                 + "pslmodules.moduleclass,"
                                 + "pslmodules.modulealgorithms,"
                                 + "pslmodules.modulealgorithmsflag,"
                                 + "pslmodules.moduleparaexample,"
                                 + "pslmodules.moduleparadesc,"
                                 + "pslmodules.moduleoutputtable,"
                                 + "pslmodules.moduleoutputnumber,"
                                 + "pslmodules.moduleoutputtype,"
                                 + "pslmodules.moduleoutputdescs,"
                                 + "pslmodules.moduleoutputdescscn,"
                                 + "pslmodules.moduleoutputpermitnull"
                                 ;

                sqlStr = String.Format("use psldb;select {0} from pslmodules ", fields);
                IDataReader reader = dbhelper.ExecuteReader(sqlStr);
                List<PSLModule> pslmodules = IDataReader2PSLModules(reader);
                reader.Close();

                return pslmodules;
            }
            catch (Exception ex)
            {
                ErrorFlag = true;
                string messageStr;
                messageStr = String.Format("DAO层PSLModulesDAO.ReadData()错误：---------->") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("sql语句：{0}", sqlStr) + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("错误信息：{0}", ex.ToString());
                logHelper.Error(messageStr); 
                return null;
            }
        }
        #endregion

        #region 辅助函数
        //SQL数据库返回结果类型转换：由IDataReader转持久化类数据集
        private static List<PSLModule> IDataReader2PSLModules(IDataReader reader)
        {
            List<PSLModule> items = new List<PSLModule>();
            while (reader.Read())
            {
                PSLModule item = new PSLModule();
                DAOSupport.ReaderToObject(reader, item);
                items.Add(item);
            }
            return items;
        }

        //检查算法开启标记的数量与计算模块的计算结果数量是否相符
        private static int checkAlgorithmsFlag(string modulename, int outputNumber, string algorithmsflag)
        {
            int sum = 0;
            if (algorithmsflag.Length != outputNumber)
            {
                string messageStr;
                messageStr = String.Format("计算模块{0}的algorithmsflag参数与outputNumber参数不符！", modulename);
                logHelper.Error(messageStr);
                sum += 1;
                return sum;
            }
            Regex regex = new Regex(@"^(Y|y|N|n){1}$");
            for (int i = 0; i < outputNumber; i++)
            {
                string str = algorithmsflag.Substring(i, 1);
                if (!regex.IsMatch(algorithmsflag.Substring(i, 1)))
                {
                    string messageStr;
                    messageStr = String.Format("计算模块{0}的algorithmsflag参数格式不对，必须是YyNn四个字母之一！", modulename);
                    logHelper.Error(messageStr);
                    sum += 1;
                }
            }
            return sum;

        }
        //检查算法输出描述梳理与计算模块计算结果数量是否相符
        private static int checkOutputDescsNumber(string modulename, int outputNumber, string outputdescs)
        {
            int sum = 0;
            string[] outputdescsArray = Regex.Split(outputdescs, ";|；");
            if (outputNumber != 0 && outputdescsArray.Length != outputNumber)     //如果outputNumber=0，则意味着输出是任意数量，跳过检查
            {
                sum += 1;
                string messageStr = String.Format("计算模块{0}的outputdescs参数与outputNumber参数不符！", modulename);
                logHelper.Error(messageStr);             
            }
            return sum;

        }
        //检查算法输出描述定义是否有重复
        private static int checkOutputDescsUnique(string modulename, ref Dictionary<string,string> outputdesclist, string outputdescs)
        {
            int sum = 0;
            string[] outputdescsArray = Regex.Split(outputdescs, ";|；");

            for (int i = 0; i < outputdescsArray.Length; i++)
            {
                if (outputdesclist.ContainsKey(outputdescsArray[i]) == false)
                {
                    outputdesclist.Add(outputdescsArray[i], modulename);
                }
                else
                {
                    sum += 1;
                    string messageStr = String.Format("计算模件{0}的计算结果描述{1}，与计算模件{2}的计算结果描述重复，请检查！", modulename, outputdescsArray[i], outputdesclist[outputdescsArray[i]]);
                    logHelper.Error(messageStr);
                }                
            }
                                       
            return sum;

        }
        //检查条件参数和正则表达式
        private static int checkPara(string modulename, string moduleparaexample,Regex modulepararegex)
        {
            int sum = 0;
            if (moduleparaexample != "" && !modulepararegex.IsMatch(moduleparaexample))
            {
                sum += 1;
                string messageStr = String.Format("计算模块{0}的计算参数示例moduleParaDesc与参数的正则表达式modulepararegex参数不符，请检查！", modulename);
                logHelper.Error(messageStr);  
            }
            return sum;
        }
        #endregion
    }
}
