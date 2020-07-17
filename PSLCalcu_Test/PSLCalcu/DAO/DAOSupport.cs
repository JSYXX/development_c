using System;
using System.Collections.Generic;
using System.Data;
using Config;   //使用log
using PCCommon;

namespace PSLCalcu
{
    public class DAOSupport
    {
       
       

        //SQL数据库返回结果单条记录的遍历
        //将返回的IDataReader类型的数据集中的一条数据记录转换为数据库持久化类对应类型的一个对象.
        //使用该转换函数的默认要求:
        //1、reader返回的数据集中字段名称，在持久化类中必须存在才能被读取。否则这个字段将被略过。但是持久化类targetObj中属性的顺序可以不与数据表一致。
        //2、reader读取的表的字段名称，与持久化类中对应的属性名称，必须完全一致，包括大小写。否则会被认为该表字段在持久化类targetObj中不存在。从而当成情况1来处理。
        //3、reader读取的表各字段的数据类型必须与targetObj对应属性的数据类型一致，如果数据类型不一致，则不能转换，会报错。ReaderToObject()仍能取出数据，只是类型不一致的那个字段对应属性为空。
        //总结：特别注意reader对应数据库字段，与targetObj对应Obj对象属性，字段名称、数据类型和属性名称、数据类型必须相同，对应的值才能读出。
        //设计特点：由于要转换的结果，即持久化类的具体数据类型并不清楚，因此这个转换结果要作为入参传给函数。
        /// <summary>
        /// 将IDataReader数据类型转换为Objec持久类对应的List<>数据类型
        /// </summary>
        /// <param name="reader">SQL语句返回的数据集</param>
        /// <param name="targetObj">数据表对应的持久化类数据集</param>        
        /// <returns>targetObj</returns>
        private static LogHelper logHelper = LogFactory.GetLogger(typeof(DAOSupport));       //全局log

        //将IDataReader结果集转换为List结构
        public static void ReaderToObject(IDataReader reader, object targetObj)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                //向民的转换程序解析reader，使用反射，这里效率是否有问题需要检测
                //参考文章http://blog.csdn.net/osmeteor/article/details/17248561
                //这里要求reader读出的数据其字段名称在持久化类属性中必须存在，一一对应。用reader.GetValue(i)给targetObj对应的属性赋值
                System.Reflection.PropertyInfo propertyInfo = targetObj.GetType().GetProperty(reader.GetName(i));   //propertyInfo是持久化类中属性名称和reader.GetName(i)名称完全一致的那个属性
                if (propertyInfo != null)   //如果在持久化类中找不到该属性，则忽略
                {
                    if (reader.GetValue(i) != DBNull.Value)     //如果该字段读对应的值不为空
                    {
                        if (propertyInfo.PropertyType.IsEnum)   //propertyInfo.PropertyType 持久化类该属性的数据类型是否为枚举
                        {
                            //如果是枚举型，用枚举类型转换，给持久化对象当前属性propertyInfo赋值，用枚举类型转换
                            propertyInfo.SetValue(targetObj, Enum.ToObject(propertyInfo.PropertyType, reader.GetValue(i)), null);
                        }                       
                        else
                        {
                            //如果不是枚举型，给持久化对象当前属性propertyInfo赋值
                            try
                            {
                                //这里用当前记录字段reader.GetName(i)对应的值reader.GetValue(i)给持久化类对应的属性赋值。                                
                                propertyInfo.SetValue(targetObj, reader.GetValue(i), null);
                            }
                            catch (Exception ex)
                            {
                                //当记录的值无法转换为持久化类属性所对应的类型时，会出错。赋空值null，并需要记录下这一错误。
                                propertyInfo.SetValue(targetObj, null, null);       //给当前持久化类对应的属性赋空值。
                                logHelper.Error(ex.ToString());    //写log文档，让配置者了解哪些字段数据类型不匹配。
                            }
                        }
                    }
                }
            }//end for 
        }//end ReaderToObject
       
        
    }
}
