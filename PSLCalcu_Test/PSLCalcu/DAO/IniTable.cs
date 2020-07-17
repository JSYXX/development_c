using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DBInterface.RDBInterface;         //使用关系数据库接口
using System.Windows.Forms;             //使用messageBox
using System.Data;
using System.Data.SqlClient;
namespace PSLCalcu
{
    public class IniTable
    {
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(IniTable));       //全局log

        #region 公有变量
        public static bool ErrorFlag = false;                                                               //全局ErrorFlag，用于读写发生错误时返回发生错误消息
        public static string ErrorInfo = "";
        public static string rdbConnStr = "";
        public static string testStatus = "";
        #endregion
        
        //测试关系数据库连接
        public static bool connectTest()
        {

            try
            {
                rdbConnStr = (DbHelper.DbConnection).Replace(" ", "_ ");
                DbHelper dbhelper = new DbHelper();     //如果出现“类型初始值设定项引发异常”的错误，请检查RTDbType、RTDbConnection两个静态参数的设置是否正确。尤其是改为xml配置时，如果配置不正确，则或报出该错误。               
                testStatus = dbhelper.ConnTest();                
                return true;
            }
            catch (Exception ex)
            {
                ErrorInfo = ex.ToString();

                string messageStr;                
                messageStr = String.Format("DAO层connectTest()错误：---------->")+Environment.NewLine;
                //logHelper.Error(messageStr);
                messageStr += String.Format("详细错误信息:" + ex.ToString());
                logHelper.Error(messageStr);
                return false;
            }
        }
        //初始化数据库
        public static bool createDB_psldb()
        {
            string sqlStr;
            string databasename = "psldb";
            //先判断是否存在psldb数据库
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("drop database if exists {0}", databasename);
                dbhelper.ExecuteNonQuery(sqlStr);
            }
            catch(Exception ex)
            {
                MessageBox.Show("删除数据库psldb失败。请检查mysql服务！"+Environment.NewLine+ex.ToString());
                return false;
            }

            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("create database {0}", databasename);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建数据库psldb失败。请检查mysql服务！" + Environment.NewLine +
                                "——请先尝试将数据库连接配置中的默认连接数据库改为sys！" + Environment.NewLine +               
                                ex.ToString()
                                );
                return false;
            } 
        }
        //计算模块信息表
        public static bool createTable_pslmodules()
        {
            string databasename = "psldb";
            string tablename = "pslmodules";
            string[] filedname ={
                                "id Integer primary key auto_increment" ,   //计算模块id号。
                                "modulename VarChar(60) not null unique" ,  //计算模块名称。注意，modulename必须唯一，因为读取pslcalcuconfig时，有和pslmodules表的内联查询。
                                "moduledesc Text",                          //计算模块描述。
                                "moduleclass Text" ,                        //计算模块所属类库，预留。
                                "modulealgorithms Text",                    //计算模块包含的算法。
                                "modulealgorithmsflag varchar(250)",        //计算模块包含的算法运算标志。
                                "moduleparaexample Text",                   //计算模块参数默认值。    
                                "moduleparadesc Text",                      //计算模块参数说明。
                                "moduleoutputtable  VarChar(60)",           //计算模块结果存放数据表。
                                "moduleoutputnumber Integer",               //计算模块计算结果个数。
                                "moduleoutputtype Text",                    //计算模块计算结果数据类型，预留。目前均为list<PValue>
                                "moduleoutputdescs Text",                   //计算模块输出项描述。
                                "moduleoutputdescscn Text",                 //计算模块输出项描述。
                                "moduleoutputpermitnull TinyInt(1)"         //是否支持空计算结果。tinyint(1)用来保存bool型
                                };

            return createTable(databasename, tablename, filedname);
        }
        //计算公式配置表
        public static bool createTable_pslcalcuconfig()
        {
            string databasename = "psldb";
            string tablename = "pslcalcuconfig";
            string[] filedname ={
                                "fid integer primary key auto_increment",   //id，自增
                                //源标签信息
                                "sourcetagname mediumtext",                     //数据源标签名，可以是实时数据库点，也可以是概化库点。由于存在多输入的计算，可能会配置多个标签。甚至有直接读取一个设备所有点的输入配置。text类型64kb都不够。需要mediumtext类型，16MB。
                                "sourcetagdb varchar(60)",                      //数据源标签所在数据库，是实时数据库，还是关系库
                                "sourcetagdesc text",                           //数据源标签描述
                                "sourcetagdim text",                            //数据源标签工程单位
                                "sourcetagmrb double",                          //数据源标签量程上限
                                "sourcetagmre double",                          //数据源标签量程下限
                                
                                //计算公式信息，1、名称
                                "fmodulename varchar(60)",                      //计算公式名称（组态宏下拉选择）
                                "fnode text",                                   //计算公式所属计算节点名。不限定为数字，预留足够的灵活性。某些情况下可能为字符串描述。比如apireadnode
                                "fgroup text",                                  //计算公式所属计算组名称（预留）。不限定为数字。比如表示设备时，为设备号，必须是数字。以便方便批量读取的配置。                                
                                "forder text",                                  //计算公式参与的计算的顺序号（预留）。不限定为数字。比如表示设备的计算项序号，必须是数字，以便方便批量读取的配置。
                                "fclass text",                                  //计算公式参与的计算的类名（预留，目前计算公式名称，就来自类名）
                                
                                //计算公式信息，2、算法
                                "falgorithms Text",                              //计算公式包含的算法（组态宏自动填写），分号分隔
                                "falgorithmsflag Text",                          //计算公式包含算法的计算标志,YN字符串。对于常数标签读取项，该长度与常数标签的个数一致。因此不能限定长度
                                "fparas text",                                   //计算公式的计算参数
                                "fcondpslnames text",                            //计算公式的条件标签，分号分隔（取时间序列值）
                                "fcondexpression text",                          //计算公式的条件表达式

                                //计算结果信息，3、结果
                                "foutputtable varchar(60)",                     //计算结果输出表名称（用于超驰算法内部的定义）
                                "foutputnumber integer",                        //计算结果输出项数量（组态宏自动填写）
                                "foutputpsltagnames text",                      //计算结果输出项对应的psl标签名，以分号分隔。由于存在多个输出的计算，可能会配置多个标签。需要text类型

                                //计算循环信息  4、循环
                                "finterval integer",                            //计算时间间隔
                                "fintervaltype varchar(255)",                   //计算时间间隔的单位（秒、分、小时、天、月）
                                "fdelay integer",                               //计算延时时间（秒）

                                //当前计算信息，该信息不是来自CSV组态。而是用于保存计算引擎的计算进程
                                "fstarttime datetime",                          //计算起始时间（整体计算的起始时间）
                                "fendtime datetime",                            //计算结束时间（整体计算的结束时间，如果未赋值，表示从起始时间开始一直计算下去）
                                "nexttime datetime",                            //下一次计算时间                                                             
                                };

            return createTable(databasename, tablename, filedname);
        }
        //历史补算配置表:历史补算功能需要的信息与实时计算不同，但是为了管理方便，历史补算配置表采用和计算公式配置表完全相同的结构
        public static bool createTable_pslhistorycalcuconfig()
        {
            string databasename = "psldb";
            string tablename = "pslhistorycalcuconfig";
            string[] filedname ={
                                "fid integer primary key auto_increment",   //id，自增
                                //源标签信息
                                "sourcetagname varchar(255)",                   //数据源标签名，可以是实时数据库点，也可以是概化库点
                                "sourcetagdb varchar(60)",                      //数据源标签所在数据库，是实时数据库，还是关系库
                                "sourcetagdesc text",                           //数据源标签描述
                                "sourcetagdim text",                            //数据源标签工程单位
                                "sourcetagmrb double",                          //数据源标签量程上限
                                "sourcetagmre double",                          //数据源标签量程下限
                                
                                //计算公式信息，1、名称
                                "fmodulename varchar(60)",                      //计算公式名称（组态宏下拉选择）
                                "fgroup text",                                  //计算公式所属计算组名称（预留）                                
                                "forder integer",                               //计算公式参与的计算的顺序号（预留）
                                "fclass text",                                  //计算公式参与的计算的类名（预留，目前计算公式名称，就来自类名）
                                
                                //计算公式信息，2、算法
                                "falgorithms Text",                              //计算公式包含的算法（组态宏自动填写），分号分隔
                                "falgorithmsflag Text",                          //计算公式包含算法的计算标志,YN字符串。对于常数标签读取项，该长度与常数标签的个数一致。因此不能限定长度
                                "fparas text",                                   //计算公式的计算参数
                                "fcondpslnames text",                            //计算公式的条件标签，分号分隔（取时间序列值）
                                "fcondexpression text",                          //计算公式的条件表达式

                                //计算结果信息，3、结果
                                "foutputtable varchar(60)",                     //计算结果输出表名称（用于超驰算法内部的定义）
                                "foutputnumber integer",                        //计算结果输出项数量（组态宏自动填写）  
                                "foutputpsltagnames text",                      //计算结果输出项对应的psl标签名  ，分号分隔
                                
                                //计算循环信息  4、循环
                                "finterval integer",                            //计算时间间隔
                                "fintervaltype varchar(255)",                   //计算时间间隔的单位（秒、分、小时、天、月）
                                "fdelay integer",                               //计算延时时间（秒）

                                //当前计算信息，该信息不是来自CSV组态。而是用于保存计算引擎的计算进程
                                "fstarttime datetime",                          //计算起始时间（整体计算的起始时间）
                                "fendtime datetime",                            //计算结束时间（整体计算的结束时间，如果未赋值，表示从起始时间开始一直计算下去）
                                "nexttime datetime",                            //下一次计算时间                                                             
                                };
            return createTable(databasename, tablename, filedname);
        }
        //计算引擎内部概化标签名称id映射表
        public static bool createTable_psltagnameidmap()
        {

            string databasename = "psldb";
            string tablename = "psltagnameidmap";
            string[] filedname ={                                                          
                                "psltagid mediumint unsigned not null primary key auto_increment",  //tagid，无符号中整形，3个字节，范围0~16777215，主键，提高索引效率
                                "psltagsaveflag tinyint unsigned not null",                         //tagsaveflag，标签是否保存的标记
                                "psltagname text not null",                                         //tagname标签名
                                "psltagdesc text",                                                  //tagdesc标签名称
                                "pslcalcuconfigindex mediumint not null",                           //标签所在计算项序号                                
                                "psltaggroup text",                                                 //标签所在计算组的组号（或所在设备的设备号）
                                "psltagorder integer",                                              //标签所在计算组内的计算序号（或所在设备内的计算序号）
                                "outputtablename varchar(60)"                                       //标签保存的数据表的名称（暂时不用，默认psldataYYYYMM）                                                  
                                };

            return createTable(databasename, tablename, filedname);
        }
        //web端标签名称id映射表
        public static bool createTable_webtagnameidmap()
        {
            string databasename = "psldb";
            string tablename = "webtagnameidmap";
            string[] filedname ={                                                          
                                "psltagid mediumint unsigned not null primary key auto_increment",//tagid，无符号中整形，3个字节，范围0~16777215，主键，提高索引效率
                                "psltagname text not null",                                       //tagstarttime是TIMESTAMP类型，4个字节，普通索引，提高索引效率 
                                "pslcalcuconfigindex mediumint not null",
                                "psltagdesc text",
                                "psltaggroup text",
                                "psltagorder integer",
                                "outputtablename varchar(60)"                                                    
                                };

            return createTable(databasename, tablename, filedname);
        }
        //概化数据表
        public static bool createTable_psldata(int startYear, int endYear,int intervalMonth)
        {
            //对于概化计算，psldata是对占用空间和效率最敏感的一张表
            //1、标签点标识采用tagid，而不用tagname。同时tagid作为主索引。（字符串索引比整形索引慢的多 http://blog.csdn.net/mypqx/article/details/8469319）
            //——一个tagid就代表一个唯一统计点。统一个算法同一个源数据，不同统计周期，计算结果写在不同tagid内。

            //2、标签点tagstarttime是该统计量统计时间段的起始时间。 （timestamp索引比整形索引效率好 http://imysql.cn/2008_07_17_timestamp_vs_int_and_datetime）
            //——在psldata中，同一tagid和tagstarttime下，只应该有一个点。
            //psldata表，主要考虑当存储数据量特别大的时候：            
            //——在计算引擎端的写入问题。计算引擎每写入一个值时，要先判断这个统计值是否存在（select * where tagid=currentid and tagstarttime=currenttime ），如果已经存在，则更新。不存在则插入。
            //——在web应用端读取的效率问题。应用端最常用的语句是（select * where tagid=？？ and tagstarttime=)或者（select * where tagid=？？ and tagstarttime>? and tagstarttime<?)
            //——考虑使用聚合索引，http://blog.csdn.net/qq_33556185/article/details/52192551
            //——或者看看replace，http://www.cnblogs.com/wancy86/p/replace_into.html ，关键是这种方法能否实现对两个key值进行操作，或者直接在聚合索引上操作。
            string databasename = "psldb";
            string tablename = "psldata";
            string[] filedname ={
                                "id integer unsigned primary key auto_increment",       //id，无符号整形，4个字节，最大值是42亿。记录的id号，自增，主键。在玉环电厂800点半年的计算中，计算一半记录数据量已经达到10亿条。                              
                                "tagid mediumint unsigned not null",                    //tagid，无符号中整形，3个字节，范围0~16777215，聚合索引，提高索引效率。（经过服务器测试，200个点，每个点配置全部23个算法，就有4w多个标签，所以短整型是不够的）
                                "tagstarttime bigint unsigned not null",                //tagstarttime，8个字节整形，存放1970开始的毫秒值，即C#DataTime类型的ticks值。不采用4个字节来表示秒，是考虑对应的PValue类，还要用于OPC数据接收，数据计算，需要保持毫秒精度
                                "tagendtime bigint unsigned not null",                  //tagendtime，8个字节整形，存放1970开始的毫秒值，即C#DataTime类型的ticks值。不采用4个字节来表示秒，是考虑对应的PValue类，还要用于OPC数据接收，数据计算，需要保持毫秒精度
                                "tagvalue double not null",                             //tagvalue，双精度浮点型，8个字节。由于PGIM的实时数据均以8字节的双精度浮点数保存。为了保持精度，psldata也以双精度保存实时值。
                                "tagstatus smallint unsigned not null"                  //tagstatus，小整形，2个字节，范围0~65535。                                                        
                                };
            //对于一段时间内统计值，如平均值，starttime，就是这段平均值起始时间，endtime就是这段平均值结束时间
            //对于一段时间内统计值，如最大值，最小值，starttime就是发生最大值得时刻，endtime为空。


            //存储空间的计算：
            //一条记录33Byte。如果一分钟一条记录
            //一个分钟标签点，一天是24*60=1440条记录，24*60*32=47520Byts 约48KB空间
            //一个分钟标签点，一年是365*24*60=525600条记录，365*24*60*32=25228800Byts 约25M空间

            //1000个分钟标签点，一年是525600000条记录（5.2亿条），25*1000=25000M 约25G空间。

            //常规项目按5000个分钟标签点

            //分割数据表的月份间隔，只有1个月、3个月、6个月、12个月四种方式。具体选取那种方式，前提就是保证划分完成的表，内部数据最大不超过5亿。
            if (intervalMonth >= 12)
                intervalMonth = 12;
            else if (intervalMonth >= 6)
                intervalMonth = 6;
            else if (intervalMonth >= 3)
                intervalMonth = 3;
            else
                intervalMonth = 1;

            for (int i = startYear; i <= endYear; i++)
            {
                for (int j = 1; j <= 12 / intervalMonth; j++)
                {
                    createTable(databasename, tablename + i.ToString()+j.ToString("D2"), filedname);
                }
            }
            return true;

        }
        //值次信息表
        public static bool createTable_pslshift()
        { 
            string databasename = "psldb";
            string tablename = "pslshiftinfo";
            string[] filedname ={
                                "id integer primary key auto_increment",        //id，自增                                
                                "shiftstarttime datetime",                      //值次起始时间
                                "shiftendtime datetime",                        //值次结束时间
                                "shiftworktime TINYINT",                        //值次工作时长
                                "shiftindex TINYINT",                           //值次序号
                                "shiftname text",                               //值次名称
                                "shiftgroupname text",                          //团队名称
                                "shiftmonitorname text",                        //值长名称
                                "shiftmonitorcomm text",                        //值长联系方式
                                "shiftvicemonitorname text",                    //副值长名称
                                "shiftvicemonitorcomm text"                     //副值长联系方式
                                };
            return createTable(databasename, tablename, filedname);
        }
        //值班员工信息表
        public static bool createTable_pslemployeeforshift()
        {
            string databasename = "psldb";
            string tablename = "pslemployeeforshift";
            string[] filedname ={
                                "id integer primary key auto_increment",        //id，自增
                                "employeeid text",                              //员工id号，字符型
                                "employeename text",                            //员工姓名
                                "employeeunit TINYINT",                         //员工所在机组
                                "employeeshift TINYINT",                        //员工所在运行值次
                                "employeetitle text",                           //员工职位名称
                                "employeemobile text",                          //员工联系方式
                                "employeeremark1 text",                         //副值长备注1
                                "employeeremark2 text"                          //副值长备注2
                                };
            return createTable(databasename, tablename, filedname);
        }
        //员工排班表
        public static bool createTable_pslplanforshift()
        {
            string databasename = "psldb";
            string tablename = "pslplanforshift";
            string[] filedname ={
                                "id integer primary key auto_increment",        //id，自增
                                "planstartdate datetime",                       //值班计划起始时间：该表内，使用planstartdate、planenddate、employeestartdate、employeeenddate做联合索引，该四个字段的组合有唯一性
                                "planenddate datetime",                         //值班计划截止时间
                                "employeestartdate datetime",                   //员工值班起始时间
                                "employeeenddate datetime",                     //员工值班截止时间
                                "employeeid char(16)",                          //员工id号，字符型
                                "employeeunit TINYINT",                         //员工所在机组，与pslemployeeforshift表信息不重复。pslemployeeforshift信息有可能更改。
                                "employeeshifte TINYINT",                       //员工所在运行值次，与pslemployeeforshift表信息不重复。pslemployeeforshift信息有可能更改。
                                "employeeremark1 text",                         //副值长备注1
                                "employeeremark2 text"                          //副值长备注2
                                };
            return createTable(databasename, tablename, filedname);
        }
        //得分计算加权系数表
        public static bool createTable_pslscoreweight()
        {
            string databasename = "psldb";
            string tablename = "pslscoreweight";
            string[] filedname ={
                                "id integer primary key auto_increment",        //id。自增
                                "psltagname text not null",                     //tag名称。这里用偏差得分计算（分钟周期）后的得分值标签                                
                                "psltagdesc text",                              //tag描述。
                                "pslweighttype text not null",                  //得分权重类型。（用以区别同样的标签，不同的权重用途。即同样的得分可以有多套权重，比如说不同周期，或者不同用途）
                                "pslweightvalue double not null",               //tag得分对应的权重
                                "pslweightvaliddate bigint not null"           //tag得分权重的生效时间                               
                                };
            return createTable(databasename, tablename, filedname);
        }
        //概化计算时间记录表
        public static bool createTable_psltimerecord()
        {
            string databasename = "psldb";
            string tablename = "psltimerecord";
            string[] filedname ={
                                "id integer unsigned primary key auto_increment",       //id，无符号整形，4个字节，记录的id号，自增。最大值是42亿     
                                "modulename text not null",                             //计算模件名称
                                "beforereaddata timestamp not null",                    //读取数据前，起始时间
                                "beforereaddatams mediumint not null",                  //读取数据前，起始时间ms                                                                                                
                                "beforefilter timestamp not null",                      //过滤前
                                "beforefilterms mediumint not null",                    //过滤前ms 
                                "beforereflection timestamp not null",                  //反射前
                                "beforereflectionms mediumint not null",                //反射前ms   
                                "beforecalcu timestamp not null",                       //计算前
                                "beforecalcums mediumint not null",                     //计算前ms                                                                  
                                "beforewritedata timestamp not null",                   //保存数据前
                                "beforewritedatams mediumint not null",                 //保存数据前ms                                
                                "beforeupdatecalcuInfo timestamp not null",             //更新数据前
                                "beforeupdatecalcuInfoms mediumint not null",           //更新数据前ms                                
                                "endcurrent timestamp not null",                        //结束时间     
                                "endcurrentms  mediumint not null",                     //结束时间ms 
                                "readspan double unsigned not null",                    //读源标签数据时间                 
                                "filterspan double unsigned not null",                  //读条件标签数据时间
                                "reflectionspan double unsigned not null",              //反射算法时间
                                "calcuspan double unsigned not null",                   //计算时间
                                "writespan double unsigned not null",                   //写数据结果时间
                                "updatespan double unsigned not null",                  //更新界面时间
                                "totalspan double unsigned not null"                    //总时间
                                };


            return createTable(databasename, tablename, filedname);
        }

        #region 创建数据表
        private static bool createTable(string databasename, string tablename, string[] fieldsinfo)
        {
            string sqlStr;
            //先判断是否存在psldb数据库
            try
            {
                DbHelper dbhelper = new DbHelper();
                sqlStr = String.Format("use {0}", databasename);
                dbhelper.ExecuteNonQuery(sqlStr);


            }
            catch
            {
                MessageBox.Show("数据库psldb不存在，请手动在关系数据库中创建该数据表！");
                return false;
            }

            try
            {
                DbHelper dbhelper = new DbHelper();
                //创建数据表之前先删除：注意必须将use {0}加在前面，用分号分隔。只有这种方法，分号后面的sql才会在指定database中操作
                sqlStr = String.Format("use {0};drop table if exists {1}", databasename, tablename);
                dbhelper.ExecuteNonQuery(sqlStr);

                //创建数据表
                string fieldsStr = String.Join(", ", fieldsinfo);
                //全部采用myisam引擎
                sqlStr = String.Format("use {0};create table {1} ({2})  engine=MyISAM AUTO_INCREMENT=100", databasename, tablename, fieldsStr);
                //全部采用innodb引擎
                //sqlStr = String.Format("use {0};create table {1} ({2}) AUTO_INCREMENT=100", databasename, tablename, fieldsStr);
                dbhelper.ExecuteNonQuery(sqlStr);
                return true;
            }
            catch
            {
                MessageBox.Show("数据表创建和删除失败！");
                return false;
            }
        }
        #endregion
    }
}
