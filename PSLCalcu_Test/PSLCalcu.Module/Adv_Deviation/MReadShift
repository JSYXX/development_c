using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;   //使用正则表达式
using PCCommon; //使用PValue
using System.IO;
using System.Linq;                      //使用list.orderby


namespace PSLCalcu.Module
{
    /// <summary>
    /// 读取值次表
    /// ——读取值次表。到固定文件夹\ShiftConfig下，读取值次配置文件shiftconfig.csv
    /// ——该算法放在所有算法前面，计算周期为1小时或者1天。
    /// ——由于PSLCalcu.Module不能引用PSLCalcu命名空间。因此在MReadShift()中不能直接调用PSLDataDAO。MReadShift()不能再内部对读取出的值次范围时间段，在数据库中先删除对应的记录。
    /// ——但是MReadShift()实际读取出的时间段范围，大于MReadShift()算法执行周期的起始和截止时间。因此需要在PSLDataDAO.WriteOrUpdate()和PSLDataDAO.WriteHistoryCalcuResults()中进行特殊处理。
    /// 
    /// 
    /// 版本：1.0
    ///    
    /// 修改纪录
    ///     
    ///		2018.01.13 版本：1.0 gaofeng 创建。    
    /// <author>
    ///		<name>gaofeng</name>
    ///		<date>2018.01.13</date>
    /// </author> 
    /// </summary>
    public class MReadShift : BaseModule, IModule, IModuleExPara
    {
        #region 计算模块信息：模块名称、包含的算法、输出项个数、输出项名称、输出项写入的数据表名称
        //这些信息，每个计算模块各不相同，用于区别不同的算法实现类。
        //这些信息，仅供计算引擎获取计算模块信息时使用，可以通过反射的方式，创建实例来调用，并获取其值。
        //在真正计算时，不需要使用这些方法，因此这些方法无需静态。
        private string _moduleName = "MReadShift";
        public string moduleName
        {
            get
            {
                return _moduleName;
            }
        }
        private string _moduleDesc = "读取值次表";
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
        private string _inputDescsCN = "读取值次信息，不需要任何源标签。这里只需要随便填写一个有值的源标签，以保证计算引擎可以运行该算法。";
        public string inputDescsCN
        {
            get
            {
                return _inputDescsCN;
            }
        }
        private string _algorithms = "MReadShift";
        public string algorithms
        {
            get
            {
                return _algorithms;
            }
        }
        private string _algorithmsflag = "Y";
        public string algorithmsflag
        {
            get
            {
                return _algorithmsflag;
            }
        }
        private string _moduleParaExample = "0";  // 注意：如果计算模块需要参数，则该属性必须有值，不能为空。检查程序依据此属性是否为空来决定是否对配置项进行正则检查
        public string moduleParaExample
        {
            get
            {
                return _moduleParaExample;
            }
        }
        private string _moduleParaDesc = "读取哪个值次的时间序列（0为读取全部值次时间序列）";
        public string moduleParaDesc
        {
            get
            {
                return _moduleParaDesc;
            }
        }
        //正则表达式，用来检测计算的参数，是否符合要求
        //——@表示不转义字符串。@后的“”内是正则表达式内容       

        private Regex _moduleParaRegex = new Regex(@"^(0|\+?[1-9][0-9]*)$");
        public Regex moduleParaRegex
        {
            get
            {
                return _moduleParaRegex;
            }
        }
        private int _outputNumber = 1;
        public int outputNumber
        {
            get
            {
                return _outputNumber;
            }
        }
        private string _outputDescs = "Shift";
        public string outputDescs
        {
            get
            {
                return _outputDescs;
            }
        }
        private string _outputDescsCN = "值次信息";
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
        public static Results Calcu(List<PValue>[] inputs, CalcuInfo calcuinfo)
        {
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
               //0、参数。要读取哪一个值此的信息。shiftNumber=0时，表示读取所有值此信息
                int shiftNumber = int.Parse(calcuinfo.fparas); 
                
                //1、值次文件遍历
                string csvFilePath=System.Environment.CurrentDirectory+"\\ShiftConfig\\";
                //string csvFileName = "shiftconfig.csv";
                string[][] csvdata;
                DirectoryInfo tagfolder = new DirectoryInfo(csvFilePath);
                FileInfo[] tagfiles = tagfolder.GetFiles("shiftconfig*.csv", SearchOption.TopDirectoryOnly);

                //2、如果值次文件中，生效时间有在当前计算周期前一个小时的文件时，进行读取
                for (i = 0; i < tagfiles.Length; i++)
                {
                    string filename = tagfiles[i].ToString();
                    string[] filenames = filename.Split('_');
                    if (filenames.Length < 3) continue;
                    DateTime filevaliddate = new DateTime();
                    try
                    {
                        filevaliddate = DateTime.Parse(filenames[1] + " " + filenames[2].Substring(0, 2) + ":" + filenames[2].Substring(2, 2));
                    }
                    catch { continue; }

                    //值次表的管理与读取思路。
                    //——值次表某值次的时间信息，只有当一个值次完成时，才会使用。
                    //——比如，2点到8点的值次，一定是等到8点结束后才会使用这个信息进行计算，不管是什么计算均是如此。
                    //——因此值次表的更新，应当放在该值次表第一个有效值次的起始时间后，结束时间前。
                    //——当值次表以第一个有效值次的起始时间命名时，就可以让计算引擎
                    //——每小时读取一次值次表。如果发现有文件名所包含的时间在当前计算截止时间往前一小时以内的值次表。就读入该值次表所有信息。
                    
                    //——下面的表达式保证了MReadShift按照1小时周期读取时，仅会在前一个小时整处，读取一次。
                    //——在并发计算引擎中，由于是先整体删除数据，再批量写入，且写入的时候不再检查。因此必须保证每一个值此表，仅能在一个时刻被读取。否则就会出现多个重复值次信息。
                    if (filevaliddate < calcuinfo.fendtime && calcuinfo.fendtime.Subtract(filevaliddate).TotalHours <= 1 && calcuinfo.fendtime.Subtract(filevaliddate).TotalHours>0) 
                    {
                        string fullpath = csvFilePath + filename;
                        csvdata = CsvFileReaderForModule.Read(fullpath);
                        
                        //写结束点的末尾:结尾点状态改变不改变已经不重要               
                        List<PValue> result = Array2PValue(csvdata);
                        

                        //根据参数读取对应值次的时间序列
                        if (shiftNumber != 0)
                        {   //如果要取得值次shiftNumber不为0，则表示仅去某一个值次shiftNumber的值，其余值次信息均删除
                            for (int j = result.Count - 1; j >= 0; j--)
                            {
                                if (result[j].Value != shiftNumber)
                                {
                                    result.RemoveAt(j);
                                }
                            }
                        }
                        
                        if(result!=null && result.Count!=0) result=result.OrderBy(m => m.Timestamp).ToList();

                        //对于读取到的值次信息，在写入数据库时的防重复机制
                        //——在实时数据计算引擎下，读取值此信息属于PSLDataDAO.WriteOrUpdate()方法的pvalues.Count > 1情况。该情况下，程序会自动寻找计算结果中的timestamp最大值，最小值。先删除该时间段内的所有数据。在插入数据。
                        //——在历史数据计算引擎下，首先会删除重算时间段内tagid所有数据。然后在读取值次表时，每天的计算结果，再向计算结果添加数据时，使用的是字典，key由tagid和timestamp组成。保证唯一

                        
                        //组织计算结果
                        results = new List<PValue>[1];
                        results[0] = result;
                        return new Results(results, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);

                    }
                }//end for

                //如果循环体内没有找到适合时间内的文件，则不会在循环体内退出
                //这种找不到适合文件的情况属于正常情况
                //返回一个总体为空的结果，计算主引擎，对MReadShift和MReadConst的计算结果整体为null，不会报错。其他均会报错
                results = null;
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
                _fatalInfo = "读取值次表\\ShiftConfig\\shiftconfig.csv错误！" + ex.ToString();
                return new Results(null, _errorFlag, _errorInfo, _warningFlag, _warningInfo, _fatalFlag, _fatalInfo);
            }
        }

        public static List<PValue> Array2PValue(string[][] csvdata)
        {
            List<PValue> result = new List<PValue>();
            if (csvdata.Length > 0)
            {
                //默认值次配置表第一行是标题行
                //值次最后一行csv数据，也要读进来。因为每一行，都是完整的值此信息，都有起始时间和截止时间。
                for (int i = 1; i < csvdata.Length; i++)
                {
                    result.Add(new PValue(double.Parse(csvdata[i][5]), DateTime.Parse(csvdata[i][1]), DateTime.Parse(csvdata[i][2]), 0));
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
