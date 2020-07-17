using System;
using System.IO;
using Config;                  //使用配置模块
public class TxtFileHelper
{
    public static string ReadString(string filename)
    {
        if (!File.Exists(filename)) return "";
        try
        {
            StreamReader reader = new StreamReader(filename);
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            //LogHelper.Write(LogType.Error, ex.ToString());
            return "";
        }
    }
}