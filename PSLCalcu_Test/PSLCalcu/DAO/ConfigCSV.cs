using System;
using System.Collections.Generic;       //使用字典
using System.Windows.Forms;             //使用massagebox
using System.Text.RegularExpressions;   //使用正则表达式
using System.Reflection;                //使用反射
using Config;                           //使用calcumodule的dll名称和命名空间
using System.IO;                        //读取目录
using System.Linq;                      //使用orderby

namespace PSLCalcu
{
    /// <summary>
    /// 检查csv数据
    /// <summary>
    /// 检查导入的csv组态数据
    /// —检查中文标题行
    /// —检查英文标题行
    /// —检查各字段是否存在，并获取列号。
    /// —检查“数据库类型”字段是否正确。
    /// —检查标签配置的算法是否重复。
    /// —检查“计算模块名称”字段：必须在计算模块信息表中存在
    /// —检查“计算结果数量”字段：与计算模块信息表一致。
    /// —检查“计算结果标志位”字段：数量一致，且为Y或N。
    /// —检查“计算结果名称”字段：如果不为空，则数量一致。
    /// —检查“计算间隔”字段：不能为空。
    /// —检查“计算延时“字段：不能为空。
    /// 上述任何检查发现问题，或检查程序运行中发生错误，将返回false。
    /// </summary>   
    public class ConfigCSV
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(ConfigCSV));       //全局log
        public int MAX_NUMBER_CONSTTAG;

        #region 公有变量
        //读取的CSV组态数据
        public string[][] importdata;
        //有效组态数据起始行
        public int firstDataRow = 2;
        //各字段的位置：注意这里的命名对应着pslcalcuconfig中字段的命名
        //这里用字段位置，可以将csv的实际配置方式与DAO隔离。如果csv的列的位置出现变化，或者新增删除了列。仅需在此修改。
        public int calcuitemindex;
        public int sourcetagnameIndex;
        public int sourcetagdbIndex;
        public int sourcetagdescIndex;
        public int sourcetagdimIndex;
        public int sourcetagmrbIndex;
        public int sourcetagmreIndex;

        public int fmodulenameIndex;
        public int fnodeIndex;
        public int fgroupIndex;
        public int forderIndex;

        public int falgorithmsflagIndex;
        public int fparasIndex;
        public int fcondpslnamesIndex;
        public int fcondexpressionIndex;

        public int foutputtableIndex;
        public int foutputnumberIndex;
        public int foutputpsltagprefIndex;      //计算结果标签名称前缀（核心名称）(计算引擎内部标签id映射表)
        public int foutputpsltagaliasIndex;     //计算结果标签别名(替代核心名称+计算结果名称)(web端标签id映射表)
        public int foutputpsltagdescIndex;

        public int fintervalIndex;  //计算周期间隔
        public int fintervaltypeIndex;  //计算周期
        public int fdelayIndex;
        public int fstartdateIndex;

        public List<PSLCalcuItem> readyPslCalcuItems { get; set; }  //已经导入好存在于数据库的计算项。追加新计算项时，如果是rdbset类型时，源标签填写时，会有可能引用已经存在的计算项。
        #endregion

        #region 公有方法
        //检查CSV数据：包括文件结构和具体数据
        public bool CheckCSVData()
        {
            try
            {
                //检查第0行第0个单元格
                if (this.importdata[0][0] != "序号")
                {
                    string messageStr = String.Format("概化计算组态文件格式不对，丢失中文标题行，请检查组态文件！CSV文件必须是ANSI或者UTF-8(包含BOM)编码。");
                    MessageBox.Show(messageStr);
                    return false;
                }
                //检查第1行第0个单元格
                if (this.importdata[1][0] != "index")
                {
                    string messageStr = String.Format("概化计算组态文件格式不对，丢失英文标题行，请检查组态文件！CSV文件必须是ANSI或者UTF-8(包含BOM)编码。");
                    MessageBox.Show(messageStr);
                    return false;
                }
                //获取必要的column位置：以下是用csv文件的英文title行的字段名称来寻找，如果找不到，弹出信息框，并退出。
                //下面变量，左侧对应pslcalcuconfig表，右侧变量对应概化计算csv配置文件title。
                this.calcuitemindex = this.SearchField(this.importdata[1], "index");
                this.sourcetagnameIndex = this.SearchField(this.importdata[1], "sourcetag");
                this.sourcetagdbIndex = this.SearchField(this.importdata[1], "dbtype");
                this.sourcetagdescIndex = this.SearchField(this.importdata[1], "desc");
                this.sourcetagdimIndex = this.SearchField(this.importdata[1], "dim");
                this.sourcetagmrbIndex = this.SearchField(this.importdata[1], "mrb");
                this.sourcetagmreIndex = this.SearchField(this.importdata[1], "mre");

                this.fmodulenameIndex = this.SearchField(this.importdata[1], "modulename");
                this.fnodeIndex = this.SearchField(this.importdata[1], "node");
                this.fgroupIndex = this.SearchField(this.importdata[1], "group");
                this.forderIndex = this.SearchField(this.importdata[1], "order");

                //falgorithmsIndex= this.SearchField(importdata[1], "algorithm");        //计算公式modulename包含的算法，由组态宏写在modulename单元格的注释中
                this.falgorithmsflagIndex = this.SearchField(this.importdata[1], "flag");
                this.fparasIndex = this.SearchField(this.importdata[1], "para");
                this.fcondpslnamesIndex = this.SearchField(this.importdata[1], "condpsl");
                this.fcondexpressionIndex = this.SearchField(this.importdata[1], "condlogic");

                this.foutputtableIndex = this.SearchField(this.importdata[1], "outputtable");
                this.foutputnumberIndex = this.SearchField(this.importdata[1], "outputnumber");    //计算结果数量，仅作为填表时的提示使用。不进行读入。
                this.foutputpsltagprefIndex = this.SearchField(this.importdata[1], "psltagpref");
                this.foutputpsltagaliasIndex = this.SearchField(this.importdata[1], "psltagalias");
                this.foutputpsltagdescIndex = this.SearchField(this.importdata[1], "psltagdesc");

                this.fintervalIndex = this.SearchField(this.importdata[1], "interval");
                this.fintervaltypeIndex = this.SearchField(this.importdata[1], "type");
                this.fdelayIndex = this.SearchField(this.importdata[1], "delay");
                this.fstartdateIndex = this.SearchField(this.importdata[1], "startdate");

                //只要有一列找不到，返回false
                if (this.sourcetagnameIndex == -1 || this.sourcetagdbIndex == -1 || this.sourcetagdescIndex == -1 || this.sourcetagdimIndex == -1 || this.sourcetagmrbIndex == -1 || this.sourcetagmreIndex == -1 ||
                    this.fmodulenameIndex == -1 || this.fnodeIndex == -1 || this.fgroupIndex == -1 || this.forderIndex == -1 ||
                    this.falgorithmsflagIndex == -1 || this.fparasIndex == -1 || this.fcondpslnamesIndex == -1 ||
                    this.foutputtableIndex == -1 || this.foutputpsltagprefIndex == -1 ||
                    this.fintervalIndex == -1 || this.fintervaltypeIndex == -1 || this.fdelayIndex == -1 || this.fstartdateIndex == -1
                    )
                {
                    return false;
                }
                //对各column的值进行检查，如果有问题，给出详细说明。该功能随后在Excel模板中实现。
                //出现的问题写入专门的log
                int sum = 0;

                sum = sum + this.checkSourceDb(this.sourcetagdbIndex);                                          //检查“数据库类型”是否正确：必须是rdb或者rtdb，大小写均可。
                //特别检查
                sum = sum + this.checkRdbSet();                                                                 //检查“数据库类型为rdbset”的源标签
                //sum = sum + this.checkConst();                                                                //检查数据类型为“const”的计算项
                //通用检查
                sum = sum + this.checkSourceMrb(this.sourcetagmrbIndex);                                        //检查"量程最小值"和“量程最大值”不能为空。
                sum = sum + this.checkSourceMre(this.sourcetagmreIndex);
                sum = sum + this.checkfnode(this.fnodeIndex);                                                   //检查fnode字段必须可转化为数字
                sum = sum + this.checkfgroup(this.fgroupIndex);                                                 //检查fgroup字段必须可转化为数字
                sum = sum + this.checkforder(this.forderIndex);                                                 //检查forder字段必须可转化为数字
                //sum = sum + this.checkCalcuUnique(this.sourcetagnameIndex, this.fmodulenameIndex);            //检查标签配置的算法是否重复：一个标签某一算法只能配置一次。2017.12.29目前的方式一个标签同一算法可以配置多次。但需要给计算结果命名。
                sum = sum + this.checkfModuleName(this.fmodulenameIndex);                                       //检查“计算模块名称”字段：必须是在pslmodules表内存在的算法模块。
                sum = sum + this.checkfModulePara(this.fmodulenameIndex, this.fparasIndex);                     //检查“计算模块参数”字段：计算模块参数必须符合模块参数的正则表达式。
                //sum = sum + this.checkfCond(this.fcondpslnamesIndex, this.fcondexpressionIndex);              //检查“计算条件参数和计算条件逻辑参数”：计算条件参数有1个条件，计算条件逻辑参数为空。计算条件有1个以上，计算条件逻辑参数不为空。
                sum = sum + this.checkfOutputNumber(this.fmodulenameIndex, this.foutputnumberIndex);            //检查“计算结果数量”字段是否正确：计算结果数量需要与计算模块信息匹配。
                sum = sum + this.checkfAlgorithmsFlag(this.fmodulenameIndex, this.falgorithmsflagIndex);        //检查“计算结果标志位”字段是否正确：必须是由Y或N构成，长度必须和计算模块包含的算法项数量相同。                
                //sum = sum + this.checkfOutputPSLNames(this.fmodulenameIndex, this.foutputpsltagprefIndex);    //检查“计算结果名称”字段数量：如果不为空，则数量必须和pslmodules表定义的数量一致。2017.12.26修改计算结果标签名命名规则。                
                sum = sum + this.checkOutputTagAlias(this.foutputpsltagaliasIndex, this.foutputnumberIndex);    //检查“计算结果别名”数量是否正确
                sum = sum + this.checkfInterval(this.fintervalIndex);                                           //检查“计算间隔”字段：不能为空。mysql整形不能为空
                sum = sum + this.checkfDelay(this.fdelayIndex);                                                 //检查“计算延时“字段：不能为空。mysql整形不能为空
                sum = sum + this.checkfStartdate(this.fstartdateIndex);                                         //检查“计算起始时间”格式是否正确
                if (sum != 0)
                {
                    string messageStr = String.Format("概化计算组态文件组态数据项有错误，请检查log文件！");
                    MessageBox.Show(messageStr);
                    return false;
                }
            }
            catch
            {
                string messageStr = String.Format("概化计算组态文件数据解析错误，请检查组态文件！\n——1、用文本编辑器打开csv文件，检查是否有非正常换行。Excel单元格内换行可造成该问题");
                MessageBox.Show(messageStr);
                return false;
            }
            return true;
        }
        //针对PGIM数据库tagname路径中的\进行特殊处理，全部转换为^
        public bool PGIMPathCharChange()
        {
            try
            {
                for (int i = this.firstDataRow; i < this.importdata.Length; i++)
                {
                    if (this.importdata[i][this.sourcetagnameIndex].Trim() != "")
                    {
                        this.importdata[i][this.sourcetagnameIndex] = this.importdata[i][this.sourcetagnameIndex].Replace(@"\", "^");
                    }

                }
            }
            catch
            {
                string messageStr = String.Format("概化计算组态文件PGIM标签路径转换错误，请检查组态文件！");
                MessageBox.Show(messageStr);
                return false;
            }
            return true;

        }
        //读取常数标签
        public bool ReadAndReplaceConst()
        {
            //找到MReadConst项
            //对于MReadConst计算，需要从常数配置文件中读取计算结果标签来填充
            //对于其他计算，由于后面在自动生成标签和别名时，const项会自动略过，这里需要转换标签名称和别名的大写。            
            string filename = "";
            string filefullpath = "";
            string[][] csvdata;

            List<string> sourcetagConst = new List<string>();
            try
            {
                for (int i = this.firstDataRow; i < this.importdata.Length; i++)
                {
                    //对类型为noinput，计算为MReadConst，进行特殊处理，将常数配置表中的标签读入
                    if (this.importdata[i][this.sourcetagdbIndex].Trim().ToLower() == "noinput" && this.importdata[i][this.fmodulenameIndex] == "MReadConst")
                    {
                        //1、常量配置文件遍历
                        string csvFilePath = System.Environment.CurrentDirectory + "\\ConstConfig\\";
                        DirectoryInfo tagfolder = new DirectoryInfo(csvFilePath);
                        FileInfo[] tagfiles = tagfolder.GetFiles("constconfig*.csv", SearchOption.TopDirectoryOnly);

                        //2、如果常数配置文件中，生效时间有在当前计算周期前一个小时的文件时，进行读取
                        Dictionary<DateTime, string> filevaliddateDic = new Dictionary<DateTime, string>();
                        for (int j = 0; j < tagfiles.Length; j++)
                        {
                            filename = tagfiles[j].ToString();
                            string[] filenames = filename.Split('_');
                            if (filenames.Length < 3) continue;
                            DateTime filevaliddate = new DateTime();
                            try
                            {
                                filevaliddate = DateTime.Parse(filenames[1] + " " + filenames[2].Substring(0, 2) + ":" + filenames[2].Substring(2, 2));
                                filevaliddateDic.Add(filevaliddate, filename);
                            }
                            catch { continue; }
                        }
                        //3、找到时间上最后的文件
                        if (filevaliddateDic.Count == 0)
                        {
                            string messageStr;
                            messageStr = String.Format("概化计算组态文件组态信息错误。常数读取计算项MReadConst检查错误！") + Environment.NewLine;
                            //logHelper.Error(messageStr);
                            messageStr += String.Format("***在ConstConfig下没有找到常数标签配置文件constcofig_xxxx-xx-xx_xxxx.csv。");
                            logHelper.Error(messageStr);
                            return false;
                        }
                        else
                        {
                            filevaliddateDic = filevaliddateDic.OrderByDescending(m => m.Key).ToDictionary(pair => pair.Key, pair => pair.Value);   //按时间顺序降序排列
                            filename = filevaliddateDic.First().Value;                                                                              //取时间上排在最后的文件
                            filefullpath = csvFilePath + filename;
                            csvdata = CsvFileReader.Read(filefullpath);
                        }

                        //4、读取常数标签
                        if (csvdata == null || csvdata.Length == 0)
                        {
                            string messageStr;
                            messageStr = String.Format("概化计算组态文件组态信息错误。常数读取计算项MReadConst检查错误！") + Environment.NewLine;
                            //logHelper.Error(messageStr);
                            messageStr += String.Format("***选定的常数标签配置文件为空。");
                            logHelper.Error(messageStr);
                            return false;
                        }
                        else if (csvdata.Length - 1 >= MAX_NUMBER_CONSTTAG)
                        {
                            string messageStr;
                            messageStr = String.Format("概化计算组态文件组态信息错误。常数读取计算项MReadConst检查错误！") + Environment.NewLine;
                            //logHelper.Error(messageStr);
                            messageStr += String.Format("***包含的常数标签数量大于{0}个。", MAX_NUMBER_CONSTTAG);
                            logHelper.Error(messageStr);
                            return false;
                        }
                        else
                        {
                            string outputtags = "";
                            string outputalias = "";
                            string outputdesc = "";
                            int outputnumber = 0;
                            for (int irow = 1; irow < csvdata.Length; irow++)
                            {
                                outputtags = outputtags + ";" + this.importdata[i][this.foutputpsltagprefIndex].Trim().ToUpper() + "" + csvdata[irow][1].Trim().ToUpper();
                                //outputtags = outputtags + ";" + this.importdata[i][this.foutputpsltagprefIndex].Trim().ToUpper() + "_" + csvdata[irow][1].Trim().ToUpper();
                                if (csvdata[irow][2] != "")
                                {
                                    outputalias = outputalias + ";" + this.importdata[i][this.foutputpsltagprefIndex].Trim().ToUpper() + "_" + csvdata[irow][2].Trim().ToUpper();
                                }
                                else
                                {
                                    outputalias = outputalias + ";" + this.importdata[i][this.foutputpsltagprefIndex].Trim().ToUpper() + "_" + csvdata[irow][1].Trim().ToUpper();
                                }
                                outputdesc = outputdesc + ";" + this.importdata[i][this.foutputpsltagdescIndex].Trim().ToUpper() + "_" + csvdata[irow][3];
                                outputnumber++;
                            }
                            string calcuFlag = "";
                            for (int j = 0; j < outputnumber; j++)
                            {
                                calcuFlag = calcuFlag + "Y";
                            }
                            //在这一步补齐所有关于输出的内容，使得ReadConst看起来是一条正常的计算
                            this.importdata[i][this.falgorithmsflagIndex] = calcuFlag;                                              //根据配置文件，补齐计算标志位
                            this.importdata[i][this.foutputnumberIndex] = outputnumber.ToString();                                  //根据配置文档，修改输出数量，为后面检查准备
                            this.importdata[i][this.foutputpsltagprefIndex] = outputtags.Substring(1, outputtags.Length - 1);       //根据配置文档，修改输出标签，
                            this.importdata[i][this.foutputpsltagaliasIndex] = outputalias.Substring(1, outputalias.Length - 1);    //根据配置文档，修改输入标签别名。由于别名是在用核心标签和计算结果组合的时候进行替换。而那个程序中const类型的计算项直接跳过了。因此需要在这里直接给出别名。
                            this.importdata[i][this.foutputpsltagdescIndex] = outputdesc.Substring(1, outputdesc.Length - 1);       //根据配置文当，修改输出描述，

                        }
                    }
                    else if (this.importdata[i][this.sourcetagdbIndex].Trim().ToLower() == "noinput" && this.importdata[i][this.fmodulenameIndex] != "MReadConst")
                    {
                        //对类型为noinput，计算不为MReadConst得计算项，别名为空则，则用计算结果名自动替换别名并转大写。别名不为空，则将别名转大写。
                        this.importdata[i][this.foutputpsltagprefIndex] = this.importdata[i][this.foutputpsltagprefIndex].Trim().ToUpper();
                        if (this.importdata[i][this.foutputpsltagaliasIndex] != "")
                        {
                            this.importdata[i][this.foutputpsltagaliasIndex] = this.importdata[i][this.foutputpsltagaliasIndex].Trim().ToUpper();
                        }
                        else
                        {
                            this.importdata[i][this.foutputpsltagaliasIndex] = this.importdata[i][this.foutputpsltagprefIndex].Trim().ToUpper();
                        }
                    }

                }//end for percalcu
                return true;
            }
            catch (Exception ex)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。常数读取计算项MReadConst检查错误！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("****检查过程发生异常，详细信息：{0}。", ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //检查“计算条件参数和计算条件逻辑参数”：计算条件参数有1个条件，计算条件逻辑参数为空。计算条件有1个以上，计算条件逻辑参数不为空。
        public bool checkfCond()
        {
            int columncondpsltagname = this.fcondpslnamesIndex;
            int columnmocondexpress = this.fcondexpressionIndex;

            int sum = 0;
            List<string> errorlines = new List<string>();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {

                if (this.importdata[i][columncondpsltagname].Trim() == "")
                {

                    //如果条件标签为空，表达式必须为空。如果不为空，则错误。
                    if (this.importdata[i][columnmocondexpress].Trim() != "")
                    {
                        string messageStr = String.Format("第{0}行的计算条件标签{1}，条件表达式必须为空，请检查！", i + 1, this.importdata[i][columncondpsltagname]);
                        logHelper.Error(messageStr);
                        sum = sum + 1;
                    }
                }
                else
                {
                    this.importdata[i][columncondpsltagname] = this.importdata[i][columncondpsltagname].Trim().ToUpper();     //将条件标签转为大写
                    string[] condpsltagsArray = Regex.Split(this.importdata[i][columncondpsltagname], ";|；");
                    //如果条件标签不为空，当只有一个条件标签时，表达式必须为空。如果不为空，则错误。
                    if (condpsltagsArray.Length == 1)
                    {
                        if (this.importdata[i][columnmocondexpress].Trim() != "")
                        {
                            string messageStr = String.Format("第{0}行的计算条件标签{1}，条件表达式必须为空，请检查！", i + 1, this.importdata[i][columncondpsltagname]);
                            logHelper.Error(messageStr);
                            sum = sum + 1;
                        }
                    }
                    //如果条件标签不为空，当只有多个条件标签时
                    //——1、表达式接受的参数数量与条件标签必须一致。不一致则错误。
                    //——2、检查表达式是否正确。
                    else
                    {
                        //——1、表达式接受的参数数量与条件标签必须一致。不一致则错误。
                        int paraNumberInExpress = NumberOfDigits(this.importdata[i][columnmocondexpress].Trim());
                        if (condpsltagsArray.Length != paraNumberInExpress)
                        {
                            string messageStr = String.Format("第{0}行的计算条件标签数量与条件表达式变量数量不符，请检查！", i + 1);
                            logHelper.Error(messageStr);
                            sum = sum + 1;
                            continue;
                        }
                        //——2、表达式的参数表示，必须是从1开始的1、2、3....
                        Boolean errorflag = false;
                        for (int j = 0; j < condpsltagsArray.Length; j++)
                        {
                            if (!this.importdata[i][columnmocondexpress].Contains("{" + (j + 1).ToString() + "}"))
                            {
                                string messageStr = String.Format("第{0}行的计算条件表达式变量索引有误，变量索引必须从1开始，并且数量和计算条件相同，请检查！", i + 1);
                                logHelper.Error(messageStr);
                                sum = sum + 1;
                                errorflag = true;
                                break;      //发现缺少任何一个变量索引，就退出
                            }
                        }
                        if (errorflag) continue;    //如果缺少变量索引，下面的公式检查肯定出错，直接跳过
                        //——3、检查表达式是否正确。
                        string[] condExpressionStr = Regex.Split(this.importdata[i][columnmocondexpress], ";|；");
                        ICondEvaluatable exp;
                        exp = new CondExpression(condExpressionStr[0]);
                        if (exp.IsValid == false)
                        {
                            string messageStr = String.Format("第{0}行的计算条件表达式有误，含有非法字符，或表达式结构有误，请检查！", i + 1);
                            logHelper.Error(messageStr);
                            sum = sum + 1;
                            continue;
                        }
                    }//结束计算条件标签单个与多个
                }//结束计算条件标签空与非空
            }//end for
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。计算条件标签和计算条件表达式有误。请检查log文件！");
                MessageBox.Show(messageStr);
                return false;
            }
            else
            {
                return true;
            }
        }
        //自动生成概化标签对内名称，对外别名，中文描述        
        public bool AutoGeneratePSLTags()
        {
            //根据modulename字段自动生成psltagnames字段
            //将importdata二维数组中的foutputpsltagprefIndex对应的列的内容，替换成由sourcetagnameIndex列和fmodulenameIndex列对应的公式输出描述组合成的新标签。
            //注意：以下规则说明，要与配置说明_标签命名规则和程序自动生成规则.txt文档保持一致。

            //2017.12.26,修改生成规则
            //计算结果标签的命名规则如下：
            //（前缀名+[参数说明]+[条件说明])+计算结果描述+时间间隔
            //命名规则分为三部分：
            //1、第一部分是前缀部分，该部分标识该标签对应原始测点的含义。
            //如果一个测点对于某一种算法，配置了多种参数情况进行计算，为了区别这些计算结果，需要在标签名中添加[参数说明]。此时，需要填写配置表的“计算结果标签名前缀”字段。
            //——例子：温度点AHPUMP001TEMP，每一个小时计算一次处于50到100之间的时间序列，和90到100之间的时间序列。
            //——则两个计算项均需要配置“计算结果标签名前缀”，AHPUMP001TEMP_P50_100，AHPUMP001TEMP_P90_100
            //如果一个测点对于某一种算法，配置了多种条件情况进行计算，为了区别这些计算结果，需要在标签名中添加[条件说明]。此时，需要填写配置表的“计算结果标签名前缀”字段。
            //——例子：压力点AHPUMP001PRES，分别在AHPUMP001TEMP_50_100和AHPUMP001TEMP_90_100情况下计算均值。
            //——则两个计算均需要配置“计算结果标签名前缀”，AHPUMP001PRES_C50_100,AHPUMP001PRES_C90_100.
            //除上述两种情况外，不需要填写“计算结果标签名前缀”。
            //2、计算结果描述部分
            //计算结果描述部分，来自于抽取的计算模件信息。这些信息存放在计算模件信息表。
            //——由检查程序保证配置的公式，一定存在于信息表中。
            //——有检查程序保证，计算结果描述的数量，与计算模件实际计算结果数量一致。
            //——由检查程序保证不同计算模件的计算结果描述，相互不重复，具有唯一性。
            //3、计算时间间隔
            //——同一个标签，同一个计算公式，可能配置了不同的计算间隔，为了避免这些计算结果名称混淆，必须在计算结果标签名称中添加计算间隔。
            //
            //程序自动生成标签名规则：
            //1、如果配置表“计算结果标签名前缀”字段不为空，则优先采用“计算结果标签名前缀”。如果该字段为空，则采用源标签名最后一段作为前缀。
            //这里建议，在填写配置表的时候，尽量要填写“计算结果标签名前缀”字段。
            //2、如果一个计算有多个计算结果，则程序自动生成多个计算结果标签，各个标签之间用；分割。
            //3、程序根据APPConfig.rdbtable_resulttagincludeinterval参数，决定是否在计算结果标签中添加时间间隔。可以在“设置”中修改。建议添加。
            //——例子，温度点AHPUMP001TEMP，每一个小时计算一次处于50到100之间的时间序列，和90到100之间的时间序列。
            //——计算结果标签名称为，AHPUMP001TEMP_P50_100_MCondSpan2_1h，AHPUMP001TEMP_P90_100_MCondSpan2_1h
            //——例子：压力点AHPUMP001PRES，分别在AHPUMP001TEMP_50_100和AHPUMP001TEMP_90_100情况下计算均值。
            //——则两个计算结果标签名为，AHPUMP001PRES_C50_100_Avg_1h,AHPUMP001PRES_C90_100_Avg_1h.
            try
            {
                //1、先从PSLModule表中读取计算模件信息。
                List<PSLModule> pslmodules = PSLModulesDAO.ReadData();            //从数据库中读取计算模件信息。
                if (PSLModulesDAO.ErrorFlag)
                {
                    string messageStr = String.Format("计算模件信息读取错误，请检查计算模件信息表PSLModules！");
                    MessageBox.Show(messageStr);
                    return false;
                }
                else if (pslmodules.Count == 0)
                {
                    string messageStr = String.Format("没有读取到任何计算模件信息，请先抽取计算模件信息！");
                    MessageBox.Show(messageStr);
                    return false;
                }
                //2、对每一行配置信息，根据规则自动生成计算结果标签名称，并替换在原来的foutputpsltagprefIndex位置
                for (int i = this.firstDataRow; i < this.importdata.Length; i++)
                {
                    //*****************对于读取常数计算项，直接跳过*******************************
                    //读取常数计算项的特点是，输入类型是noinput，计算组件名称为MReadConst
                    if (this.importdata[i][this.sourcetagdbIndex] == "noinput" && this.importdata[i][this.fmodulenameIndex] == "MReadConst") continue;     //这里常数项处理直接跳过了，因此：1、不会将标签名作为核心再去组合。2、不糊将标签描述作为核心再去组合。

                    //*****************处理计算引擎内部使用的标签名称***********************
                    //2.1、获取名称第一部分，前缀部分
                    //——如果foutputpsltagpref为空，则采用sourcetagname作为前缀，需要对sourcetagname做处理。不同的实时数据库，标签全路径形式不一样。
                    //——golden的标签名中含有"."，"."分割的最后一部分代表实际标签名。
                    //——pgim的标签名，在配置表中格式如“\\PGIMServer\OPCScanner\BLR\HA001PUMP001P。从csv读入后，用PGIMPathCharChange，将所有的"\"替换成了"^"。因此最后一个"^"后代表标签名。
                    string tagnamepref = "";        //名称前缀，即计算引擎内部标签名称核心部分。引擎内部标签名称采用“核心名称”+“计算结果名称”+“时间周期”。                    
                    if (this.importdata[i][this.foutputpsltagprefIndex].Trim() == "")
                    {
                        //如果输出标签前缀为空，则使用源标签核心做为前缀
                        int index4dot = this.importdata[i][this.sourcetagnameIndex].Trim().IndexOf('.');              //获取当前计算的源标签名。如果源标签名称如果有"."，则表示数据表名称，仅采用"."后的字符串作为源数据标签名
                        int index4arrow = this.importdata[i][this.sourcetagnameIndex].Trim().IndexOf('^');            //获取当前计算的源标签名。如果源标签名称如果有"^"，则表示数据表名称，仅采用"^"后的字符串作为源数据标签名
                        if (index4dot != -1 || index4arrow != -1)                                           //如果sourcename中含有"."或者"^"，则用"."或者"^"进行分割。
                        {
                            string[] tagsplit = this.importdata[i][this.sourcetagnameIndex].Trim().Split(new char[2] { '.', '^' });        //用\或者^来分割psltag，获得最后一部分
                            tagnamepref = tagsplit[tagsplit.Length - 1];
                        }
                        else
                        {                                                                                   //如果sourcename中即没有"."也没有"^"。则直接将sourcename作为tagnamepref
                            tagnamepref = this.importdata[i][this.sourcetagnameIndex].Trim();
                        }
                    }
                    else
                    {
                        //否则直接读取输出标签前缀
                        tagnamepref = this.importdata[i][this.foutputpsltagprefIndex].Trim();
                    }

                    //2.2、获得名称第二部分，当前行对应公式的计算结果描述
                    //对计算结果标签名为空的项，自动填写。源标签名称如果有"."，则表示数据表名称，仅采用"."后的字符串作为源数据标签名
                    //当前需要做些特殊处理

                    string modulename = this.importdata[i][this.fmodulenameIndex].Trim();                         //取出当前csv数据行的modulename
                    int moduleindex = pslmodules.FindIndex(x => x.modulename == modulename);            //用当前行modulename从 这行有可能出错，找不到算法模块    
                    string moduleoutputdescs = pslmodules[moduleindex].moduleoutputdescs;               //根据modulename从pslmodules中找对应的公式信息。pslmodules中找到计算模块输出描述moduleoutputdescs
                    string[] outputdescs = Regex.Split(moduleoutputdescs, ";|；");                 //根据计算模块输出描述moduleoutputdescs，分割得到计算结果描述字符串数组



                    //2.3根据参数APPConfig.rdbtable_resulttagincludeinterval，决定自动生成的概化标签名称中是否自动添加时间间隔
                    string strinterval = "";
                    if (APPConfig.rdbtable_resulttagincludeinterval == "1")    //自动生成标签时，带计算间隔类型
                        strinterval = this.importdata[i][this.fintervalIndex] + this.importdata[i][this.fintervaltypeIndex];
                    else                                                      //自动生成标签时，不带计算间隔类型
                        strinterval = "";

                    //2.4根据计算结果数量，生成计算结果标签字符串
                    string newpsltagnames = "";
                    for (int j = 0; j < outputdescs.Length; j++)
                    {
                        newpsltagnames = newpsltagnames + tagnamepref + "_" + outputdescs[j] + "_" + strinterval + ";";
                    }
                    //2.5将组合好的标签名，替换原tagnamepref位置的内容
                    this.importdata[i][this.foutputpsltagprefIndex] = newpsltagnames.Substring(0, newpsltagnames.Length - 1).Trim().ToUpper();

                    //*****************处理web端使用的标签名称***********************
                    //2.6web端标签的别名
                    string[] outputalias;
                    if (this.importdata[i][this.foutputpsltagaliasIndex].Trim() == "")
                    {
                        //如果别名为空，则用前面已经准备好的计算引擎内部的标签名，做别名。这样计算引擎内部标签名和web端标签名称一致。
                        this.importdata[i][this.foutputpsltagaliasIndex] = newpsltagnames.Substring(0, newpsltagnames.Length - 1).Trim().ToUpper();
                    }
                    else
                    {
                        //如果别名不为空，则用“别名”+“计算周期”作为web端标签名称。如果有多个计算结果，需要为每一个计算结果准备别名
                        outputalias = Regex.Split(this.importdata[i][this.foutputpsltagaliasIndex].Trim(), ";|；");
                        string newpsltagalias = "";
                        for (int j = 0; j < outputalias.Length; j++)
                        {
                            newpsltagalias = newpsltagalias + outputalias[j] + "_" + strinterval + ";";
                        }
                        this.importdata[i][this.foutputpsltagaliasIndex] = newpsltagalias.Substring(0, newpsltagalias.Length - 1).Trim().ToUpper();
                    }

                    //*****************处理标签中文描述***********************
                    //2.7将组合好描述，替换源描述位置
                    string tagdescpref = this.importdata[i][this.foutputpsltagdescIndex].Trim();            //当前计算项的计算结果中文描述
                    string[] tagdescprefArray = Regex.Split(tagdescpref, ";|；");
                    string moduleoutputdescsCN = pslmodules[moduleindex].moduleoutputdescscn;               //根据modulename从pslmodules中找对应的公式信息。pslmodules中找到计算模块输出中文描述moduleoutputdescscn
                    string[] outputdescsCN = Regex.Split(moduleoutputdescsCN, ";|；");                 //根据计算模块输出描述moduleoutputdescs，分割得到计算结果描述字符串数组
                    string newpsltagdescsCN = "";

                    //如果配置表中的的计算结果描述本身使用了分号，即tagdescprefArray得到的长度大于1，则意味计算结果描述的单元格为每一个结果单独定义好了名称。
                    //——此时直接使用该描述作为结算结果描述
                    //——如果配置表中的的计算结果描述本身没有使用分号，则意味着计算结果，要采用单元格给出的前缀+计算结果后缀的形式来组合
                    if (tagdescprefArray.Length == 1)
                    {
                        //如果原表的单元仅有一项内容，即没有使用分号，则用原表的内容和计算结果的中文描述构成结果的中文描述
                        for (int j = 0; j < outputdescsCN.Length; j++)
                        {
                            newpsltagdescsCN = newpsltagdescsCN + tagdescpref + "_" + outputdescsCN[j] + ";";
                        }
                        this.importdata[i][this.foutputpsltagdescIndex] = newpsltagdescsCN.Substring(0, newpsltagdescsCN.Length - 1).Trim().ToUpper();
                    }
                    else
                    {

                    }


                }//end for 每个点
                return true;
            }
            catch (Exception ex)
            {
                string messageStr = String.Format("公式配置信息出错，找不到配置的算法模块！");
                MessageBox.Show(messageStr);
                return false;
            }
        }
        //检查源数据类型为“rdbset”时，原标签位置填写是否正确，并替换原标签。注意有两种类型。
        public bool AutoReplaceSourceTags()
        {
            //当源数据类型为rdbset时，表示要一次读取一个计算所有的结果。在这里有两种情况。
            //第一种配置源标签的方式：批量读取某算法结果标签。
            //——该配置，通过配置内容，唯一的指向某一条计算。需要将该计算的所有结果标签替换为当前计算的源标签。检查标签格式是否正确，是否能找到对应的计算项。把找到的计算项序号替换源标签
            //第二种配置源标签的方式：从文件中批量读取。
            //——该配置，配置内容是一个csv文件名称，该文件存放在SourcTagConfig文件夹下。该csv文件中包含了当前算法所需的所有源标签及描述。

            //在第一种情况下，源标签名位置，应该已经被替换为要读结果的那条计算的序号
            //要读结果的那条计算项，计算结果描述已经在AutoGeneratePSLTags()中被替换为正则完全的计算结果标签
            //这里，只需根据计算项序号，找到对应的算法对象，将其计算结果替换到源标签位置
            int calcuitemsetIndex;
            try
            {
                for (int i = this.firstDataRow; i < this.importdata.Length; i++)
                {
                    string[] sourcetagArray = importdata[i][this.sourcetagnameIndex].Split(';');        //如果此时，源标签字段是诸如#calcuIndex;1;2;3的格式，sourcetagArray将该字段分割
                    string sourcetagStr = "";
                    if (importdata[i][this.sourcetagdbIndex] == "rdbset" && sourcetagArray[0] == "#calcuIndex") //如果当前计算项数据类型为rdbset，源标签以#calcuIndex开头，则进行替换
                    {
                        for (int j = 1; j < sourcetagArray.Length; j++)         //sourcetagArray保存着要读取结果的计算项
                        {
                            calcuitemsetIndex = int.Parse(sourcetagArray[j]);    //此时在sourcetagnameIndex填写的是要取计算结果的计算项的序号

                            //先从importdata中取对应计算项的计算结果
                            for (int k = this.firstDataRow; k < this.importdata.Length; k++)
                            {
                                if (int.Parse(importdata[k][0]) == calcuitemsetIndex)
                                {
                                    sourcetagStr += importdata[k][foutputpsltagprefIndex] + ";";
                                    break;
                                }

                            }
                            //再从已经准备好的计算项中（数据库中已经有的）
                            for (int k = 0; k < this.readyPslCalcuItems.Count; k++)
                            {
                                if (this.readyPslCalcuItems[k].fid == calcuitemsetIndex)
                                {
                                    sourcetagStr += this.readyPslCalcuItems[k].foutputpsltagnames + ";";
                                    break;
                                }
                            }
                        }
                        importdata[i][this.sourcetagnameIndex] = sourcetagStr.Substring(0, sourcetagStr.Length - 1);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                string messageStr = String.Format("公式配置信息替换rdbset类型的源标签出错，请检查！");
                MessageBox.Show(messageStr);
                return false;
            }
        }

        //检查计算结果标签唯一性
        public bool checkPSLNameUnique()
        {
            //由程序生成的计算结果标签AutoGeneratePSLTags都在foutputpsltagprefIndex字段内。
            int columnmodulename = this.fmodulenameIndex;
            int columnoutputplsnames = this.foutputpsltagprefIndex;
            //输出项名称用分号分隔，不计算的不取结果的，要用空字符站位。
            int sum = 0;
            Dictionary<string, int> outputtagnamelist = new Dictionary<string, int>();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string[] outputtagsArray = Regex.Split(this.importdata[i][columnoutputplsnames], ";|；");
                for (int j = 0; j < outputtagsArray.Length; j++)
                {
                    if (outputtagnamelist.ContainsKey(outputtagsArray[j]) == false)
                    {
                        outputtagnamelist.Add(outputtagsArray[j], i);
                    }
                    else
                    {
                        sum += 1;
                        string messageStr = String.Format("第{0}行的计算结果标签{1}，与第{2}行计算结果标签重复，请检查！", i + 1, outputtagsArray[j], outputtagnamelist[outputtagsArray[j]]);
                        logHelper.Error(messageStr);
                    }
                }
            }

            if (sum != 0)
            {
                string messageStr = String.Format("概化计算组态文件计算结果标签名有重复，请检查log文件！");
                MessageBox.Show(messageStr);
                return false;
            }
            else
            {
                return true;
            }

        }
        #endregion

        #region 辅助函数，
        //查找字段对应的列
        private int SearchField(string[] str, string key)
        {

            int ret = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == key)
                {
                    ret = i;
                    break;
                }
            }

            if (ret == -1)
            {
                string messageStr = String.Format("找不到数据列{0}，请检查组态文件！", key);
                MessageBox.Show(messageStr);
            }

            return ret;
        }
        //检查数据库类型字段：数据库类型必须是rtdb或者rdb，计算引擎要根据这个字段做读取数据的分支判断
        private int checkSourceDb(int column)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string sourcdb = this.importdata[i][column].Trim().ToLower();
                this.importdata[i][column] = sourcdb;
                if (sourcdb != "noinput" && sourcdb != "rtdb" && sourcdb != "opc" && sourcdb != "rdb" && sourcdb != "rdbset")
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。有数据库类型填写错误。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****错误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查数据类型为rdbset时，对源数据进行替换和整理。支持两种格式：1、批量读取某算法结果标签。2、从文件批量读取。
        private int checkRdbSet()
        {
            //第一种配置源标签的方式：批量读取某算法结果标签。比如：M;010230NO005055_000000;MTempBase;1h。其中第一个参数M是按计算项取结果。第二个参数是指向计算项的源标签描述sourcetagdescs。
            //——该配置，通过配置内容，唯一的指向某一条计算。需要将该计算的当前计算的源标签替换为指向计算的所有结果标签。检查标签格式是否正确，是否能找到对应的计算项。把找到的计算项序号替换源标签
            //第二种配置源标签的方式：从文件中批量读取。比如：F;Device01。
            //——该配置，配置内容是一个csv文件名称，该文件存放在SourcTagConfig文件夹下。该csv文件中包含了当前算法所需的所有源标签及描述。
            //第三种配置源标签的方式：根据设备号和计算序号。比如：D;33;1
            //注意，以上两种情况下的源标签替换工作，都在导入计算配置csv文件的过程中完成。实际导入计算引擎数据库的，是已经替换好源标签的计算项。

            int sum = 0;
            List<string> errorlines = new List<string>();
            string csvFilePath = System.Environment.CurrentDirectory + "\\SourcTagConfig\\";

            List<string> sourcetagRdbSet = new List<string>();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                if (this.importdata[i][this.sourcetagdbIndex] == "rdbset")
                {
                    string[] tagnames = Regex.Split(this.importdata[i][this.sourcetagnameIndex], ";|；");         //获取rdbset行的源标签字符串
                    if ((tagnames[0].Trim().ToUpper() == "M" && tagnames.Length == 4) ||         //module，读取算法所有输入结果。例如：M;010230NO005055_000000;MTempBase;1h
                        (tagnames[0].Trim().ToUpper() == "F" && tagnames.Length == 2) ||         //file，用csv文件配置源数据标签。例如：F;Device01。表示读取Device01.csv文件的数据
                        (tagnames[0].Trim().ToUpper() == "D" && tagnames.Length == 3)            //device，用group和order指定读取哪个设备的计算结果。例如：D;33;1。表示读取属于设备33（group=33）的所有计算序号为1（order=1）的计算结果。
                       )
                    {
                        //所有正确的配置在if中列举

                        //1、
                        //如果是配置为读取某算法结果标签，第一个配置项应为"M",即module。则后面字符串必须以三部分构成。
                        //如M;060000UnitLoad_000000;MDivBase;1d。
                        //——其中M指用计算项结果配置，module
                        //——060000UnitLoad_000000，计算项源标签描述(不用计算项源标签，是因为源标签也可能被替换)
                        //——MDivBase，计算项的计算名称
                        //——1d，计算项计算周期

                        //2、
                        //如果是配置为从文件读取，则第一个配置项应为"F",即file。则后面字符串必须以一部分构成。
                        //如F;DeviceTag_01
                        //——其中F指用文件配置。file
                        //——DeviceTag_01，指在SourcTagConfig文件下的DeviceTag_01.csv文件

                        //这里是正常情况，不做任何处理
                    }
                    else
                    {
                        //不满足以上正常情况的，即为非正常情况，记错，跳转下一个
                        //sum = sum + 1;
                        //errorlines.Add((i - this.firstDataRow + 1).ToString());

                        //20181229修改，不满足以上条件，说明标签是直接用；配置好的多个标签，不需要要替换，但仍需要多个标签同时读取
                        //这种情况，无需替换，直接跳过
                        continue;
                    }

                    if (tagnames[0].Trim().ToUpper() == "M")
                    {

                        //如果配置为M，如M;060000UnitLoad_000000;MDivBase;1d。就是读取某一个计算项所有结果。
                        //就是找到对应的算法项，并将算法项序号替换到sourcetagnameIndex字段中。如#calcuIndex;1
                        string sourcetagdescs = tagnames[1];
                        string modulename = tagnames[2];
                        string period = tagnames[3];


                        bool findflag = false;
                        for (int j = this.firstDataRow; j < this.importdata.Length; j++)
                        {
                            //测试
                            //string temp1 = this.importdata[j][this.fmodulenameIndex].ToUpper();
                            //string temp2 = this.importdata[j][this.foutputpsltagprefIndex].ToUpper();
                            //string temp3 = this.importdata[j][this.fintervalIndex].ToUpper() + this.importdata[j][this.fintervaltypeIndex].ToUpper();
                            //
                            //此处zuoweiqiang 修改过

                            if (this.importdata[j][this.fmodulenameIndex].ToUpper() == modulename.ToUpper() &&                  //前置计算项的算法模块名称与rdbset源标签中的算法模块名称相同
                                this.importdata[j][this.sourcetagnameIndex].ToUpper() == sourcetagdescs.ToUpper() &&            //前置计算项的输入标签描述与rdbset源标签中引用的输入标签描述相同
                                this.importdata[j][this.fintervalIndex].ToUpper() + this.importdata[j][this.fintervaltypeIndex].ToUpper() == period.ToUpper())  //前置计算项的计算周期与rdbset源标签中指定的核心周期相同
                            {
                                this.importdata[i][this.sourcetagnameIndex] = "#calcuIndex;" + this.importdata[j][0];                         //将找到的行号給到columnsourctag
                                                                                                                                              //因为此时找到的计算项，其结果标签还没有生成
                                                                                                                                              //因此这没有办法直接填写当前项的源标签。
                                                                                                                                              //只能在后面先生成结果标签后，再填写源标签。在AutoReplaceSourceTags()中执行
                                findflag = true;
                                break;
                            }
                        }
                        for (int j = 0; j < this.readyPslCalcuItems.Count; j++)
                        {
                            if (this.readyPslCalcuItems[j].fmodulename.ToUpper() == modulename.ToUpper() &&
                               this.readyPslCalcuItems[j].sourcetagname.ToUpper() == sourcetagdescs.ToUpper() &&
                               this.readyPslCalcuItems[j].finterval.ToString().ToUpper() + this.readyPslCalcuItems[j].fintervaltype.ToUpper() == period.ToUpper())
                                this.importdata[i][this.sourcetagnameIndex] = "#calcuIndex;" + this.readyPslCalcuItems[j].fid;

                        }
                        if (findflag == false)
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                            continue;
                        }

                    }//end if “M”

                    if (tagnames[0].Trim().ToUpper() == "F")
                    {
                        //如果配置为F，如F;DeviceTag_01，就是从文件中读取标签名称。
                        try
                        {
                            //寻找源标签配置文件，替换文件中的源标签
                            string filename = tagnames[1];
                            string fullpath = csvFilePath + filename + ".CSV";
                            string[][] csvdata = CsvFileReader.Read(fullpath);
                            string sourcetagsStr = "";
                            for (int j = 1; j < csvdata.Length; j++)
                            {
                                sourcetagsStr += csvdata[j][1].Trim().ToUpper() + ";";        //读取第一列源标签列
                            }

                            this.importdata[i][this.sourcetagnameIndex] = sourcetagsStr.Substring(0, sourcetagsStr.Length - 1); //将csv文件读取的源标签连接字符串，直接覆盖到源标签位置
                                                                                                                                //与上面相比，在rdbset下，就有两种不同类型的结果。在AutoReplaceSourceTags()中要加以区分
                        }
                        catch
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                            continue;
                        }
                    }

                    if (tagnames[0].Trim().ToUpper() == "D")
                    {
                        //如果配置成D，如D;30;1。就是读取同属某一个设备30的所有点，序号为1的所有计算项的计算结果
                        //如果配置成D;30;1,2。就是读取同属某一个设备30的所有点，序号为1和2的所有计算项的计算结果
                        //如果配置成D;30;1-4。就是读取同属某一个设备30所有点，序号为1到4的所有计算项的计算结果
                        //如果配置成D;30;1|4。就是读取同属某一个设备30所有点，序号为1和4的所有计算项的计算结果
                        //就要找到所有的算法项，并将算法项序号替换到sourcetagnameIndex字段中。如#calcuIndex;1;2;3;4;5

                        //找出有哪些计算号
                        try
                        {
                            string deviceNumber = tagnames[1];                          //设备号
                            string calcuNumberStr = tagnames[2];                        //计算号字符串  如1,2,3。或者1-3
                            string[] calcuNumbersStrArray = calcuNumberStr.Split('|');  //
                            List<string> calcuNumber = new List<string>();              //计算号数组
                            for (int k = 0; k < calcuNumbersStrArray.Length; k++)
                            {
                                if (calcuNumbersStrArray.Contains("-"))
                                {
                                    string[] limitStr = calcuNumbersStrArray[i].Split('-');
                                    int lower = int.Parse(limitStr[0]);
                                    int upper = int.Parse(limitStr[1]);
                                    for (int j = lower; j <= upper; j++)
                                        calcuNumber.Add(j.ToString());
                                }
                                else
                                {
                                    calcuNumber.Add(calcuNumbersStrArray[k]);
                                }
                            }
                            //根据计算号，找出源标签名称
                            bool findflag4m = false;
                            string calcuIndexStr = "#calcuIndex;";
                            for (int k = 0; k < calcuNumber.Count; k++)
                            {
                                //在当前要导入的csv数据的计算项中寻找对应的行
                                for (int j = this.firstDataRow; j < this.importdata.Length; j++)
                                {
                                    if (this.importdata[j][this.fgroupIndex].ToUpper() == deviceNumber.ToUpper() &&                     //前置计算项的算法模块group值与rdbset源标签中的算法模块所属设备号相同
                                        this.importdata[j][this.forderIndex].ToUpper() == calcuNumber[k].ToUpper()                         //前置计算项的算法模块order值与rdbset源标签中的算法模块所属设备计算序号相同
                                        )
                                    {
                                        calcuIndexStr += this.importdata[j][0].ToString() + ";";                                        //将找到的行号給到calcuIndexStr
                                        findflag4m = true;
                                    }
                                }
                                //在已经导入数据库的计算项中寻找对应的行
                                for (int j = 0; j < this.readyPslCalcuItems.Count; j++)
                                {
                                    if (this.readyPslCalcuItems[j].fgroup.ToUpper() == deviceNumber.ToUpper() &&
                                       int.Parse(this.readyPslCalcuItems[j].forder) == int.Parse(calcuNumber[k].ToUpper())
                                      )
                                    {
                                        calcuIndexStr += this.readyPslCalcuItems[j].fid.ToString() + ";";                               //将找到的行号給到calcuIndexStr
                                        findflag4m = true;
                                    }
                                }
                            }//endfor

                            //全部找完后，根据结果填写找到的行号到importdata到对应的单元格（sourcetagnameIndex列）
                            if (findflag4m == false)
                            {
                                sum = sum + 1;
                                errorlines.Add((i - this.firstDataRow + 1).ToString());
                                continue;
                            }
                            else
                            {
                                this.importdata[i][this.sourcetagnameIndex] = calcuIndexStr.Substring(0, calcuIndexStr.Length - 1);                         //将找到的行号給到columnsourctag
                                //因为此时找到的计算项，其结果标签还没有生成
                                //因此这没有办法直接填写当前项的源标签。
                                //只能在后面先生成结果标签后，再填写源标签。在AutoReplaceSourceTags()中执行
                            }
                        }
                        catch (Exception ex)
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                            continue;
                        }
                    }//end if D

                }//end if rebset
            }//end for

            //根据结果给出提示
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。rdbset类型的计算源标签格式不正确。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****错误的源标签和对应的算法是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查node字段必须能转化成数字
        private int checkfnode(int column)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string nodeStr = this.importdata[i][column].Trim();

                try
                {
                    Convert.ToInt32(nodeStr);
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。计算节点字段node存在非数字项。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****错误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查group字段必须能转化为数字
        private int checkfgroup(int column)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string groupStr = this.importdata[i][column].Trim();

                try
                {
                    Convert.ToInt32(groupStr);
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。计算组字段group存在非数字项。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****错误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查order字段必须能转化为数字
        private int checkforder(int column)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string orderStr = this.importdata[i][column].Trim();

                try
                {
                    Convert.ToInt32(orderStr);
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。计算顺序字段order存在非数字项。。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****错误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查标签配置的算法是否重复：一个标签某一算法只能配置一次。
        private int checkCalcuUnique(int columntag, int columnmodule)
        {

            int sum = 0;
            List<string> errorlines = new List<string>();

            string[] sourceTagAndmoduleName = new string[this.importdata.Length - this.firstDataRow];
            //将源标签字段和算法名称字段组合成一个字段放入数组
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                sourceTagAndmoduleName[i - this.firstDataRow] = (this.importdata[i][columntag] + "&&" + this.importdata[i][columnmodule]);
            }
            //对该数组进行排序
            Array.Sort(sourceTagAndmoduleName);
            //如果相邻两个元素相同，则认为有重复。否则认为没有。
            for (int i = 0; i < sourceTagAndmoduleName.Length - 1; i++)
            {
                if (sourceTagAndmoduleName[i] == sourceTagAndmoduleName[i + 1])
                {
                    sum = sum + 1;
                    errorlines.Add(sourceTagAndmoduleName[i]);
                }
            }

            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。有同一标签使用相同算法两次的重复。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****错误的源标签和对应的算法是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查“计算模块名称”字段：必须是在pslmodules表内存在的。
        private int checkfModuleName(int column)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();

            List<PSLModule> pslmodules = PSLModulesDAO.ReadData();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string modulename = this.importdata[i][column].Trim();   //特别注意这里必须用trim()函数，否则单元格的前后空格会影响判断的准确性
                try
                {   //在pslmodules中找modulename，找不到会报错
                    int moduleindex = pslmodules.FindIndex(x => x.modulename == modulename);    //这行有可能出错，找不到算法模块 
                    if (moduleindex == -1)
                    {
                        sum = sum + 1;
                        errorlines.Add((i - this.firstDataRow + 1).ToString());
                    }
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法模块在算法库中不存在。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****算法配置有问题的数据行是(总表Excel行数)：{0}。请仔细核对算法模块的名称，包括大小写！", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查“计算模块参数”字段：计算模块参数必须符合模块参数的正则表达式
        private int checkfModulePara(int columnmodulename, int columnmoduleparas)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();
            List<PSLModule> pslmodules = PSLModulesDAO.ReadData();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string modulename = this.importdata[i][columnmodulename].Trim();     //特别注意这里必须用trim()函数，否则单元格的前后空格会影响判断的准确性
                string moduleparas = this.importdata[i][columnmoduleparas].Trim();   //获取计算参数字符串                
                try
                {
                    if (!APPConfig.noCheckFunction.Contains(modulename))
                    {
                        Assembly assembly = Assembly.LoadFrom(APPConfig.DLLNAME_CALCUMODULE);               //获得PSLCalcu.exe
                        Type type = assembly.GetType(APPConfig.NAMESPACE_CALCUMODULE + "." + modulename);     //获得PSLCalcu.exe中指定计算模块
                        object tmpModuleObj = Activator.CreateInstance(type);                               //获取计算公式类实例
                                                                                                            //如果对应的类有moduleParaExample属性，则获取该属性。没有该属性，则返回值为null
                        PropertyInfo GetModuleParaExample = type.GetProperty("moduleParaExample");
                        //如果对应的类有moduleParaExample属性，则获得具体值。
                        string moduleParaExample = "";
                        if (GetModuleParaExample != null) moduleParaExample = (string)GetModuleParaExample.GetValue(tmpModuleObj, null);
                        //如果计算公式的paraexample不为空，则用正则表达式对配置信息中的para进行检查
                        if (moduleParaExample != "" | moduleParaExample.Trim() != "")
                        {
                            PropertyInfo GetModuleParaRegex = type.GetProperty("moduleParaRegex");
                            Regex moduleParaRegex = (Regex)GetModuleParaRegex.GetValue(tmpModuleObj, null);
                            if (!moduleParaRegex.IsMatch(moduleparas))
                            {
                                sum = sum + 1;
                                errorlines.Add((i - this.firstDataRow + 1).ToString());
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());

                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。算法参数格式有误。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****算法配置有问题的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }

            return sum;
        }
        //检查“计算结果数量”字段是否正确：必须是由Y或N构成，长度必须和计算模块包含的算法项数量相同。
        private int checkfOutputNumber(int columnmodulename, int columnoutputnumber)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();

            List<PSLModule> pslmodules = PSLModulesDAO.ReadData();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string modulename = this.importdata[i][columnmodulename].Trim();

                try
                {
                    if (!APPConfig.noCheckFunction.Contains(modulename))
                    {
                        int moduleindex = pslmodules.FindIndex(x => x.modulename == modulename);    //这行有可能出错，找不到算法模块 
                        int outputnumber = pslmodules[moduleindex].moduleoutputnumber;
                        if (outputnumber == 0)
                        {   //如果对应算法的输入数量为0，表示算法实际的输出数量不确定，直接跳过检查。比如MReadConst。
                            continue;
                        }
                        else if (int.Parse(this.importdata[i][columnoutputnumber].Trim()) != outputnumber)
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());

                }

            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法模块计算结果数量有误。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****计算结果数量有问题的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查量程最小值不能为空
        private int checkSourceMrb(int columnsourcetagmrbIndex)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();

            List<PSLModule> pslmodules = PSLModulesDAO.ReadData();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string mre = this.importdata[i][columnsourcetagmrbIndex].Trim();
                try
                {
                    if (mre == "")
                    {
                        sum = sum + 1;
                        errorlines.Add((i - this.firstDataRow + 1).ToString());
                    }
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }

            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法模块量程最小值不能为空。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****计算结果数量有问题的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查量程最大值不能为空
        private int checkSourceMre(int columnsourcetagmreIndex)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();

            List<PSLModule> pslmodules = PSLModulesDAO.ReadData();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string mre = this.importdata[i][columnsourcetagmreIndex].Trim();
                try
                {
                    if (mre == "")
                    {
                        sum = sum + 1;
                        errorlines.Add((i - this.firstDataRow + 1).ToString());
                    }
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }

            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法模块量程最大值不能为空。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****计算结果数量有问题的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查“计算结果标志位”字段是否正确：算法标志位数量，则数量必须和输出数量一致
        private int checkfAlgorithmsFlag(int columnmodulename, int columnalgorithmsflag)
        {
            int sum = 0;
            List<string> errorlines = new List<string>();

            List<PSLModule> pslmodules = PSLModulesDAO.ReadData();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                string modulename = this.importdata[i][columnmodulename].Trim();
                try
                {   //在pslmodules中找modulename，找不到会报错
                    if (!APPConfig.noCheckFunction.Contains(modulename))
                    {
                        int moduleindex = pslmodules.FindIndex(x => x.modulename == modulename);    //这行有可能出错，找不到算法模块 
                        int outputnumber = pslmodules[moduleindex].moduleoutputnumber;
                        if (outputnumber == 0)
                        {   //如果输入结果为0，表示输出数量不确定，不需要检查标志，直接跳过
                            continue;
                        }
                        string algorithmflag = this.importdata[i][columnalgorithmsflag].Trim();
                        Regex regex = new Regex(@"^([ynYN]){" + outputnumber.ToString() + "}$");
                        if (!regex.IsMatch(algorithmflag))
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }

            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法计算结果标志位有误。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****计算结果标志位有问题的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //不用了！计算结果标签名全部自动生成。检查“计算结果名称”字段数量:计算结果输出标签如果不为空，则数量必须和输出数量一致:2017.12.26修改。修改计算结果的标签名生成规则。这个函数不在使用       
        private int checkfOutputPSLNames(int columnmodulename, int columnoutputplsnames)
        {
            //输出项名称用分号分隔，不计算的不取结果的，要用空字符站位。
            int sum = 0;
            List<string> errorlines = new List<string>();

            List<PSLModule> pslmodules = PSLModulesDAO.ReadData();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                if (this.importdata[i][columnoutputplsnames].Trim() != "")
                {
                    string modulename = this.importdata[i][columnmodulename].Trim();
                    try
                    {   //在pslmodules中找modulename，找不到会报错
                        int moduleindex = pslmodules.FindIndex(x => x.modulename == modulename);    //这行有可能出错，找不到算法模块 
                        int outputnumber = pslmodules[moduleindex].moduleoutputnumber;
                        string[] outputnames = Regex.Split(this.importdata[i][columnoutputplsnames].Trim(), ";|；");
                        if (outputnumber != outputnames.Length)
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                        }
                    }
                    catch
                    {
                        sum = sum + 1;
                        errorlines.Add((i - this.firstDataRow + 1).ToString());
                    }
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法计算结果标签数量与算法输出数量不符。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****数量不符的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查“计算结果别名”数量是否正确
        private int checkOutputTagAlias(int columnoutputpsltagalias, int columnoutputnumberIndex)
        {
            //输出项名称用分号分隔，不计算的不取结果的，要用空字符站位。
            int sum = 0;
            List<string> errorlines = new List<string>();

            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                if (!APPConfig.noCheckFunction.Contains(this.importdata[i][this.fmodulenameIndex].Trim()))
                {
                    if (this.importdata[i][columnoutputpsltagalias].Trim() != "")
                    {
                        string[] outputalias = Regex.Split(this.importdata[i][columnoutputpsltagalias].Trim(), ";|；");
                        if (outputalias.Length.ToString() != this.importdata[i][columnoutputnumberIndex])
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                        }

                    }
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法计算结果标签别名数量与算法输出数量不符。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****数量不符的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }

        //检查“计算间隔”字段：不能为空。mysql整形不能为空
        private int checkfInterval(int column)
        {
            //输出项名称用分号分隔，不计算的不取结果的，要用空字符站位。
            int sum = 0;
            List<string> errorlines = new List<string>();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                try
                {
                    int x = Convert.ToInt32(this.importdata[i][column].Trim());
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法计算间隔填写有误。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****填写有误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查“计算延时“字段：不能为空。mysql整形不能为空
        private int checkfDelay(int column)
        {
            //输出项名称用分号分隔，不计算的不取结果的，要用空字符站位。
            int sum = 0;
            List<string> errorlines = new List<string>();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                try
                {
                    int x = Convert.ToInt32(this.importdata[i][column].Trim());
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法计算延时填写有误。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****填写有误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查startdate
        private int checkfStartdate(int column)
        {
            //
            int sum = 0;
            List<string> errorlines = new List<string>();
            DateTime x;
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                try
                {
                    if (this.importdata[i][column].Trim() != "")
                        x = DateTime.Parse(this.importdata[i][column].Trim());
                }
                catch
                {
                    sum = sum + 1;
                    errorlines.Add((i - this.firstDataRow + 1).ToString());
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的算法起始时间填写有误。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****填写有误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //不用了！！该函数已将被CheckCondPSLName取代。检查“计算条件标签名”字段：必须是已经存在的计算结果，必须在psltagnameidmap中存在
        private int checkCondPSLTagName(int column)
        {
            //
            int sum = 0;
            List<string> errorlines = new List<string>();
            Dictionary<string, System.UInt32> TagName2IdMap = PSLTagNameIdMapDAO.ReadMap();
            for (int i = this.firstDataRow; i < this.importdata.Length; i++)
            {
                if (this.importdata[i][column].Trim() != "") //如果条件标签不为空，才检查
                {
                    string[] condpsltags = Regex.Split(this.importdata[i][column].Trim(), ";|；");
                    for (int j = 0; j < condpsltags.Length; j++)
                    {
                        //如果条件标签在字段中不存在
                        if (condpsltags[j] != "" && !TagName2IdMap.ContainsKey(condpsltags[j]))
                        {
                            sum = sum + 1;
                            errorlines.Add((i - this.firstDataRow + 1).ToString());
                        }
                    }
                }
            }
            if (sum != 0)
            {
                string messageStr;
                messageStr = String.Format("概化计算组态文件组态信息错误。配置的条件标签不存在。请检查组态文件！") + Environment.NewLine;
                //logHelper.Error(messageStr);
                string errorlinesStr = string.Join(",", errorlines.ToArray());
                messageStr += String.Format("****填写有误的数据行是(总表Excel行数)：{0}。", errorlinesStr);
                logHelper.Error(messageStr);
            }
            return sum;
        }
        //检查是否为数字。更好的办法替换为正则表达式
        private static int NumberOfDigits(string theString)
        {
            int count = 0;
            for (int i = 0; i < theString.Length; i++)
            {
                if (Char.IsDigit(theString[i]))
                {
                    count++;
                }
            }
            return count;
        }
        #endregion
    }
}
