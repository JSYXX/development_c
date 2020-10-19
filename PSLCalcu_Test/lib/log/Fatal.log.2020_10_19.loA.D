
2020-10-19 07:27:26,073 [19] FATAL PSLCalcu.AppRunForm - 计算引擎错误1：计算模块内部意外错误!
——计算错误详细信息：MySql.Data.MySqlClient.MySqlException (0x80004005): Timeout expired.  The timeout period elapsed prior to completion of the operation or the server is not responding. ---> System.TimeoutException: Timeout in IO operation
   at MySql.Data.MySqlClient.TimedStream.StopTimer()
   at MySql.Data.MySqlClient.TimedStream.Read(Byte[] buffer, Int32 offset, Int32 count)
   at System.IO.BufferedStream.Read(Byte[] array, Int32 offset, Int32 count)
   at MySql.Data.MySqlClient.MySqlStream.ReadFully(Stream stream, Byte[] buffer, Int32 offset, Int32 count)
   at MySql.Data.MySqlClient.MySqlStream.LoadPacket()
   at MySql.Data.MySqlClient.MySqlStream.ReadPacket()
   at MySql.Data.MySqlClient.NativeDriver.GetResult(Int32& affectedRow, Int64& insertedId)
   at MySql.Data.MySqlClient.Driver.GetResult(Int32 statementId, Int32& affectedRows, Int64& insertedId)
   at MySql.Data.MySqlClient.Driver.NextResult(Int32 statementId, Boolean force)
   at MySql.Data.MySqlClient.MySqlDataReader.NextResult()
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReader(CommandBehavior behavior)
   at PSLCalcu.Module.BLL.AlgorithmBLL.getShortData(String tableName, String tagid, String year, String month, String day) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 22
   at PSLCalcu.Module.MPVBaseLong.Calcu(DataSet inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBaseLong.cs:line 359。
——计算模块的名称是：108-MPVBaseLong，计算起始时间是：2020/2/26 15:00:00，计算结束时间是：2020/2/27 15:00:00。

2020-10-19 07:27:27,172 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误2：计算模块内部意外错误!
——计算错误详细信息：MySql.Data.MySqlClient.MySqlException (0x80004005): You have an error in your SQL syntax; check the manual that corresponds to your MySQL server version for the right syntax to use near '.psl_mpvbase where `tagId`=1102 and `yearvalue`="2020" and `monthvalue`="1" and ' at line 1
   at PSLCalcu.Module.BLL.AlgorithmBLL.isHaveData(String year, String month, String day, String hour, String type, String tableName) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 268
   at PSLCalcu.Module.BLL.AlgorithmBLL.insertMPVBase(MPVBaseMessageOutBadClass newClass, String type, String year, String month, String day, String hour) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 180
   at PSLCalcu.Module.MPVBaseShort.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBaseShort.cs:line 400。
——计算模块的名称是：100-MPVBaseShort，计算起始时间是：2020/1/1 0:00:00，计算结束时间是：2020/1/1 1:00:00。

2020-10-19 07:30:15,136 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误5：计算模块内部意外错误!
——计算错误详细信息：MySql.Data.MySqlClient.MySqlException (0x80004005): You have an error in your SQL syntax; check the manual that corresponds to your MySQL server version for the right syntax to use near '.psl_mpvbase where `tagId`=1102 and `yearvalue`="2020" and `monthvalue`="1" and ' at line 1
   at PSLCalcu.Module.BLL.AlgorithmBLL.isHaveData(String year, String month, String day, String hour, String type, String tableName) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 268
   at PSLCalcu.Module.BLL.AlgorithmBLL.insertMPVBase(MPVBaseMessageOutBadClass newClass, String type, String year, String month, String day, String hour) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 180
   at PSLCalcu.Module.MPVBaseShort.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBaseShort.cs:line 400。
——计算模块的名称是：100-MPVBaseShort，计算起始时间是：2020/1/1 1:00:00，计算结束时间是：2020/1/1 2:00:00。

2020-10-19 20:17:54,920 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误1：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: Input string was not in a correct format.
   at System.Number.StringToNumber(String str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)
   at System.Number.ParseInt64(String value, NumberStyles options, NumberFormatInfo numfmt)
   at System.Int64.Parse(String s)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 377。
——计算模块的名称是：110-MPVBasePlusSftOri，计算起始时间是：2020/1/1 0:02:00，计算结束时间是：2020/1/1 0:03:00。

2020-10-19 20:18:04,077 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误2：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: Input string was not in a correct format.
   at System.Number.StringToNumber(String str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)
   at System.Number.ParseInt64(String value, NumberStyles options, NumberFormatInfo numfmt)
   at System.Int64.Parse(String s)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 377。
——计算模块的名称是：110-MPVBasePlusSftOri，计算起始时间是：2020/1/1 0:03:00，计算结束时间是：2020/1/1 0:04:00。

2020-10-19 20:18:45,422 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误3：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: Input string was not in a correct format.
   at System.Number.StringToNumber(String str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)
   at System.Number.ParseInt64(String value, NumberStyles options, NumberFormatInfo numfmt)
   at System.Int64.Parse(String s)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 377。
——计算模块的名称是：110-MPVBasePlusSftOri，计算起始时间是：2020/1/1 0:04:00，计算结束时间是：2020/1/1 0:05:00。

2020-10-19 20:19:09,637 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误4：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: Input string was not in a correct format.
   at System.Number.StringToNumber(String str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)
   at System.Number.ParseInt64(String value, NumberStyles options, NumberFormatInfo numfmt)
   at System.Int64.Parse(String s)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 377。
——计算模块的名称是：110-MPVBasePlusSftOri，计算起始时间是：2020/1/1 0:05:00，计算结束时间是：2020/1/1 0:06:00。

2020-10-19 21:56:42,633 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误1：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:21:00，计算结束时间是：2020/1/1 6:22:00。

2020-10-19 21:56:45,866 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误2：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:24:00，计算结束时间是：2020/1/1 6:25:00。

2020-10-19 21:56:45,974 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误3：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:25:00，计算结束时间是：2020/1/1 6:26:00。

2020-10-19 21:56:46,084 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误4：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:26:00，计算结束时间是：2020/1/1 6:27:00。

2020-10-19 21:56:46,208 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误5：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:27:00，计算结束时间是：2020/1/1 6:28:00。

2020-10-19 21:56:46,297 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误6：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:28:00，计算结束时间是：2020/1/1 6:29:00。

2020-10-19 21:56:46,573 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误7：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:31:00，计算结束时间是：2020/1/1 6:32:00。

2020-10-19 21:56:46,660 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误8：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:32:00，计算结束时间是：2020/1/1 6:33:00。

2020-10-19 21:56:47,621 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误9：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:43:00，计算结束时间是：2020/1/1 6:44:00。

2020-10-19 21:56:48,728 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误10：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:54:00，计算结束时间是：2020/1/1 6:55:00。

2020-10-19 21:56:48,840 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误11：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 6:55:00，计算结束时间是：2020/1/1 6:56:00。

2020-10-19 21:56:49,275 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误12：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:00:00，计算结束时间是：2020/1/1 7:01:00。

2020-10-19 21:56:49,641 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误13：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:04:00，计算结束时间是：2020/1/1 7:05:00。

2020-10-19 21:56:51,746 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误14：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:24:00，计算结束时间是：2020/1/1 7:25:00。

2020-10-19 21:56:52,050 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误15：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:27:00，计算结束时间是：2020/1/1 7:28:00。

2020-10-19 21:56:52,519 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误16：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:32:00，计算结束时间是：2020/1/1 7:33:00。

2020-10-19 21:56:52,663 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误17：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:33:00，计算结束时间是：2020/1/1 7:34:00。

2020-10-19 21:56:52,979 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误18：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:36:00，计算结束时间是：2020/1/1 7:37:00。

2020-10-19 21:56:53,109 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误19：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:37:00，计算结束时间是：2020/1/1 7:38:00。

2020-10-19 21:56:53,395 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误20：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:40:00，计算结束时间是：2020/1/1 7:41:00。

2020-10-19 21:56:53,495 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误21：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:41:00，计算结束时间是：2020/1/1 7:42:00。

2020-10-19 21:56:53,574 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误22：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:42:00，计算结束时间是：2020/1/1 7:43:00。

2020-10-19 21:56:53,682 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误23：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:43:00，计算结束时间是：2020/1/1 7:44:00。

2020-10-19 21:56:54,021 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误24：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:46:00，计算结束时间是：2020/1/1 7:47:00。

2020-10-19 21:56:54,282 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误25：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:49:00，计算结束时间是：2020/1/1 7:50:00。

2020-10-19 21:56:54,399 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误26：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:50:00，计算结束时间是：2020/1/1 7:51:00。

2020-10-19 21:56:54,486 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误27：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:51:00，计算结束时间是：2020/1/1 7:52:00。

2020-10-19 21:56:54,610 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误28：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:52:00，计算结束时间是：2020/1/1 7:53:00。

2020-10-19 21:56:54,705 [17] FATAL PSLCalcu.AppRunForm - 计算引擎错误29：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at System.DateTimeParse.Parse(String s, DateTimeFormatInfo dtfi, DateTimeStyles styles)
   at System.Convert.ToDateTime(String value)
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 616。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:53:00，计算结束时间是：2020/1/1 7:54:00。

2020-10-19 21:59:05,324 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误1：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at PSLCalcu.Module.BLL.AlgorithmBLL.getMPVBasePlusSftOriOldData(String dutyTime, UInt32[] foutputpsltagids) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 76
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 281。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 7:59:00，计算结束时间是：2020/1/1 8:00:00。

2020-10-19 21:59:51,821 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误2：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at PSLCalcu.Module.BLL.AlgorithmBLL.getMPVBasePlusSftOriOldData(String dutyTime, UInt32[] foutputpsltagids) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 76
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 281。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 15:59:00，计算结束时间是：2020/1/1 16:00:00。

2020-10-19 22:00:41,114 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误3：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at PSLCalcu.Module.BLL.AlgorithmBLL.getMPVBasePlusSftOriOldData(String dutyTime, UInt32[] foutputpsltagids) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 76
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 281。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/1 23:59:00，计算结束时间是：2020/1/2 0:00:00。

2020-10-19 22:01:27,235 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误4：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at PSLCalcu.Module.BLL.AlgorithmBLL.getMPVBasePlusSftOriOldData(String dutyTime, UInt32[] foutputpsltagids) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 76
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 281。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/2 7:59:00，计算结束时间是：2020/1/2 8:00:00。

2020-10-19 22:02:41,713 [16] FATAL PSLCalcu.AppRunForm - 计算引擎错误5：计算模块内部意外错误!
——计算错误详细信息：System.FormatException: String was not recognized as a valid DateTime.
   at PSLCalcu.Module.BLL.AlgorithmBLL.getMPVBasePlusSftOriOldData(String dutyTime, UInt32[] foutputpsltagids) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\BLL\AlgorithmBLL.cs:line 76
   at PSLCalcu.Module.MPVBasePlusSftOri.Calcu(List`1[] inputs, CalcuInfo calcuinfo) in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu.Module\New_Base_Caculate\MPVBasePlusSftOri.cs:line 281。
——计算模块的名称是：100-MPVBasePlusSftOri，计算起始时间是：2020/1/2 15:59:00，计算结束时间是：2020/1/2 16:00:00。
