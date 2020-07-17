using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCCommon;         //使用PValue
using System.IO;        //读取csv

namespace PSLCalcu.Module
{
    /// <summary>
    /// 读取常数标签
    /// ——读取常量标签。到固定文件夹\ConstConfig下，读取常量标签配置文件constconfig.csv
    /// ——该算法放在所有算法前面，计算周期为1小时或者1天。
    /// ——输出数量为0，代表输出数量不固定。
    /// ——由于PSLCalcu.Module不能引用PSLCalcu命名空间。因此在MReadConst()中不能直接调用PSLDataDAO。MReadConst()不能在内部对读取出的值次范围时间段，在数据库中先删除对应的记录。
    /// ——MReadConst()理论上每次每个标签仅读取1个记录，在最后使用PSLDataDAO.WriteOrUpdate()写数据时，如果有相同时刻的记录则更替。没有则写入。
    /// ——常量标签在使用时，约定为，仅返回计算周期起始时刻前最后一个时刻有效值。
    /// 
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///		2018.05.14 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.05.14</date>
    /// </author> 
    /// </summary>
    public class MReadConst :BaseModule, IModule
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MReadConst";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "读取常数标签";
        public string moduleDesc
        {
            get
            {
                return _moduleDesc;
            }
        }
        private int _inputNumber = 0;
        public int inputNumber
        {
            get
            {
                return _inputNumber;
            }
        }
        private string _inputDescsCN = "读取常数标签，不需要任何源标签。这里只需要随便填写一个有值的源标签，以保证计算引擎可以运行该算法。";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MReadConst";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "";    //算法标志为空，表示算法标志任意，在算法标志检查时，会跳过。
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private int _outputNumber = 0;          //输出是0，表示输入数量任意，在输出检查时，会跳过。
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "ConstValue";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "常数标签值";
        public string outputDescsCN
        {
            get
            {
                return _outputDescsCN;
            }
        }
        private string _outputTable = "plsdata";
        public string outputTable
        {
            get
            {
                return _outputTable;
            }
        }
        private bool _outputPermitNULL = true;
        public bool outputPermitNULL
        {
            get
            {
                return _outputPermitNULL;
            }
        }
        #endregion

        #region 输入输出参数的读写接口
        //输入输出参数读写，之所以要单独做成类的静态变量，主要是考虑有外部计算条件的计算模块，外部条件完成对时间帅选后，可以直接用时间去处理inputData
        //这样能够保持带条件和不带条件的计算，Calcu方法在形式上可以统一。
        private static List<PValue>[] _inputData;
        public static List<PValue>[] inputData
        {
            get
            {
                return _inputData;
            }
            set
            {
                _inputData = value;
            }
        }

        private static CalcuInfo _calcuInfo;
        public static CalcuInfo calcuInfo
        {
            get
            {
                return _calcuInfo;
            }
            set
            {
                _calcuInfo = value;
            }
        }

        private static List<PValue>[] _outputData;
        public static List<PValue>[] outputData
        {
            get
            {
                return _outputData;
            }
            set
            {
                _outputData = value;
            }
        }
        
        #endregion

        #region 计算模块算法
        /// <summary>
        /// 计算模块算法实现:求同时有效的条件时间段数量
        /// </summary>
        /// <param name="input">输入数据</param>              
        /// <param name="calcuinfo">当次计算项相关信息</param>
        /// <param name="calcuinfo.algorithmsFlag">当次计算算法执行标志</param>
        /// <param name="calcuinfo.algorithmsPara">当次计算算法需要的参数，如超限统计的限值</param>       
        /// <returns>值次信息</returns>

        public static Results Calcu()
        {
            return Calcu(_inputData, _calcuInfo);
        }
        public static int OutputNumberStatic = 0;
       
        public static Results Calcu(List<PValue>[] inputs, CalcuInfo calcuinfo)
        {
            //对于MReadConst算法：
            //——不需要源标签，因此源标签填写任何一个rdb标签即可，源标签类型填写const
            //——读取数据阶段，遇到const，直接置input[0]=null，计算引擎不记报警和错误，直接跳转

            //公用变量
            bool _errorFlag = false;
            string _errorInfo = "";
            bool _warningFlag = false;
            string _warningInfo = "";
            bool _fatalFlag = false;
            string _fatalInfo = "";

            int i;

            List<PValue>[] results;
            
            try
            {
                //0、参数。读取的常量配置文件名称
                //string configfilename = String.Parse(calcuinfo.fparas);
                string[][] csvdata=null;
                string filename = "";
                string filefullpath = "";
               
                //1、常量配置文件遍历
                string csvFilePath = System.Environment.CurrentDirectory + "\\ConstConfig\\";
                DirectoryInfo tagfolder = new DirectoryInfo(csvFilePath);
                FileInfo[] tagfiles = tagfolder.GetFiles("constconfig*.csv", SearchOption.TopDirectoryOnly);

                //2、在常数配置文件夹中找格式符合的文件，并获得文件时间参数，
                Dictionary<DateTime, string> filevaliddateDic = new Dictionary<DateTime, string>();
                for (i = 0; i < tagfiles.Length; i++)
                {
                    filename = tagfiles[i].ToString();          //对文件名称进行解析，文件名称格式：constconfig_2018-01-01_0800.csv
                    string[] filenames = filename.Split('_');   //用_分割
                    if (filenames.Length < 3) continue;         //如果少于三部分则不对，看下一个文件名
                    DateTime filevaliddate = new DateTime();
                    try
                    {
                        filevaliddate = DateTime.Parse(filenames[1] + " " + filenames[2].Substring(0, 2) + ":" + filenames[2].Substring(2, 2));     //用第二部分构成时间
                        filevaliddateDic.Add(filevaliddate, filename);
                    }
                    catch { continue; } 
                }
                //如果没有找到任何文件，就直接退出。并且错误报警。因为一个项目必须有正确的尝试配置文件
                if (filevaliddateDic.Count == 0)
                {
                    //找不到任何文件，要报错，并退出
                    _errorFlag = true;
                    _errorInfo = "在\\ConstConfig文件夹下找不到符合条件的常数配置文件！";
                    return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);     //这里退出，在计算引擎出会记录报错
                }
                //3、找到时间上最后的文件
                //生效时间有在当前计算周期前一个小时的文件时，进行读取
                //找到文件要看最后一个时间是不是符合要求
                filevaliddateDic = filevaliddateDic.OrderBy(m => m.Key).ToDictionary(pair => pair.Key, pair => pair.Value);   //按时间顺序升序排列
                foreach (var item in filevaliddateDic)
                {
                    //3.1 依次获取每一个时间
                    DateTime filevaliddate = item.Key;
                    
                    //3.2 找到符合时间范围的那个文件：1、文档有效日期在当前计算结束日期之前。2、文档日期在当前计算时间之前2个小时以内（避免每次都读，仅在这两个小时内读取）
                    if (filevaliddate < calcuinfo.fendtime && calcuinfo.fendtime.Subtract(filevaliddate).TotalHours <= 2 && calcuinfo.fendtime.Subtract(filevaliddate).TotalHours > 0)
                    { 
                        //3.2.1将该文件的常数值读取出来
                        filename = item.Value;                           //取时间对应的文件名称
                        filefullpath = csvFilePath + filename;
                        csvdata = CsvFileReaderForModule.Read(filefullpath);
                        if (csvdata == null || csvdata.Length == 0)
                        {
                            //说明相应文件内没有符合条件的数据，要给出报警
                            //如果不符合要求，则说明当前没有需要更新的常数文件，正常退出
                            _errorFlag = true;
                            _errorInfo = "在"+filefullpath+"文件中没有找到有效数据！";
                            return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);     //这里退出，在计算引擎出会记录报错
                        }

                        //3.2.2 如果静态参数输出数量OutputNumberStatic，还没有被初始化，则必须先进行一次初始化。
                        //这是一个静态变量，程序启动后，仅执行一次。
                        if (OutputNumberStatic == 0)    //只要OutputNumberStatic为0，就说明未被初始化，因为只要配置了常数读取算法，就一定有常数项
                        {                            
                            OutputNumberStatic = csvdata.Length - 1;
                        }

                        //3.2.3 读取常数标签的值，并组织计算结果
                        //特别注意，结果中，常数标签值得时间戳timestamp，不是用calcuinfo的起始时间，而是用文件名的时间。
                        results = new List<PValue>[OutputNumberStatic];
                        for (i = 1; i < csvdata.Length; i++)
                        {
                            results[i - 1] = new List<PValue>();
                            results[i - 1].Add(new PValue(double.Parse(csvdata[i][4]), filevaliddate, calcuinfo.fendtime, 0));    //要保证写入的点在当前时间段内，这些计算引擎写入结果时才不会出错。
                        }
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);   
                    
                    }//end if 符合时间范围
                }//end foreach

                //如果循环体内没有找到适合时间内的文件，则不会在循环体内退出
                //这种找不到适合文件的情况属于正常情况
                //返回一个总体为空的结果，计算主引擎，对MReadShift和MReadConst的计算结果整体为null，不会报错。
                results = null;     //在完成new List<PValue>[OutputNumberStatic]之后，  results恰好是一个长度为OutputNumberStatic，每个元素为null的数组。
                return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

                
                
            }
            catch (Exception ex)
            {
                //计算中出任何错误，则需要记录log
                //LogHelper.Write(LogType.Error, "计算模块错误!");
                //记录计算模块的名称、当前标签、起始时间、结束时间
                //string moduleInfo = string.Format("——计算模块的名称是：{0}，当前计算源标签是：{1}，计算起始时间是：{2}，计算结束时间是：{3}。", calcuInfo.fmodulename, calcuInfo.sourcetagname, calcuinfo.fstarttime.ToString(), calcuinfo.fendtime.ToString());
                //LogHelper.Write(LogType.Error, moduleInfo);
                //计算引擎报错具体信息
                //string errInfo = string.Format("——具体报错信息：{0}。", ex.ToString());
                //LogHelper.Write(LogType.Error, errInfo);
                //返回null供计算引擎处理
              
                _fatalFlag = true;
                _fatalInfo = "读取值常数配置表\\ConstConfig\\constconfig.csv错误！" + ex.ToString();
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }
       
        #endregion

    }
}
