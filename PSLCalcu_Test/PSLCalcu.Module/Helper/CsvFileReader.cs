using System;
using System.Collections.Generic;
using System.Text;          //使用StringBuilder
using System.Collections;   //使用ArrayList
using System.IO;            //使用File类

namespace PSLCalcu.Module
{
    /// <summary>
    /// PSLCalcu.Module专用，不存在弹出窗口
    /// </summary>
    public static class CsvFileReaderForModule
    {
        public static string[][] Read(string fullname)
        {
            try
            {
                if (!File.Exists(fullname)) return new string[][] { };
                System.Text.Encoding fileEncoding = GetFileEncodeType(fullname);

                //有title行
                //var lines = File.ReadAllLines(fullname).Skip(1);
                //没有标题行
                var lines = File.ReadAllLines(fullname, fileEncoding); //无论什么格式，均已UTF8格式读入

                var list = new List<string[]>();
                var builder = new StringBuilder();      //builder一行中分析出一个字段
                foreach (var line in lines)
                {
                    builder.Clear();
                    var comma = false;
                    var array = line.ToCharArray();     //array每一行内容打散成字符数组
                    var values = new List<string>();    //value用于存放每一行分析结果
                    var length = array.Length;
                    var index = 0;
                    while (index < length)
                    {
                        var item = array[index++];
                        switch (item)
                        {
                            case ',':
                                if (comma) //遇到“,”,如果在双引号内，认为未结束，向builder添加当前元素item
                                {
                                    builder.Append(item);
                                }
                                else    //遇到“,”,如果不在双引号内，就认为该元素结束，当前builder结束
                                {
                                    values.Add(builder.ToString());
                                    builder.Clear();
                                }
                                break;
                            case '"':   //遇到双引号，置引号标记位
                                comma = !comma;
                                break;
                            default:    //未遇到“,”认为未结束，向builder添加当前元素item
                                builder.Append(item);
                                break;
                        }
                        if (index == length)
                        {
                            values.Add(builder.ToString());
                            builder.Clear();
                        }
                    }
                    var count = values.Count;
                    if (count == 0) continue;
                    list.Add(values.ToArray());
                }
                return list.ToArray();

            }
            catch (Exception ex)
            {
                
                return null;
            }
        }
   

       

        /// <summary>
        /// 判断CSV文件的编码格式
        /// </summary>        
        /// <param name="fileName">CSV的文件路径</param>
        private static System.Text.Encoding  GetFileEncodeType(string filename)
        { 
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read); 
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs); 
            Byte[] buffer = br.ReadBytes(2); 
            if(buffer[0]>=0xEF) 
            { 
                if(buffer[0]==0xEF && buffer[1]==0xBB) 
                { 
                    return System.Text.Encoding.UTF8; 
                } 
                else if(buffer[0]==0xFE && buffer[1]==0xFF) 
                { 
                    return System.Text.Encoding.BigEndianUnicode; 
                } 
                else if(buffer[0]==0xFF && buffer[1]==0xFE) 
                { 
                    return System.Text.Encoding.Unicode; 
                } 
                else
                { 
                    return System.Text.Encoding.Default; 
                } 
            } 
            else
            { 
                return System.Text.Encoding.Default; 
            } 
       }
   }//end class
}
