
2020-04-28 12:51:07,450 [16] ERROR PSLCalcu.PSLHistoryCalcuConfigDAO - DAO层PSLDataDAO.WriteOrUpdate()错误：---------->
sql语句：use psldb;select id from psldata202004 where tagid='19376' and tagstarttime='637212960000000000'
错误信息：System.Threading.ThreadAbortException: 正在中止线程。
   在 MySql.Data.Common.MyNetworkStream.HandleOrRethrowException(Exception e)
   在 MySql.Data.Common.MyNetworkStream.Read(Byte[] buffer, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.TimedStream.Read(Byte[] buffer, Int32 offset, Int32 count)
   在 System.IO.BufferedStream.Read(Byte[] array, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.MySqlStream.ReadFully(Stream stream, Byte[] buffer, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.MySqlStream.LoadPacket()
   在 MySql.Data.MySqlClient.MySqlStream.ReadPacket()
   在 MySql.Data.MySqlClient.NativeDriver.GetResult(Int32& affectedRow, Int64& insertedId)
   在 MySql.Data.MySqlClient.Driver.GetResult(Int32 statementId, Int32& affectedRows, Int64& insertedId)
   在 MySql.Data.MySqlClient.Driver.NextResult(Int32 statementId, Boolean force)
   在 MySql.Data.MySqlClient.MySqlDataReader.NextResult()
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteReader(CommandBehavior behavior)
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteReader()
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteScalar()
   在 DBInterface.RDBInterface.BaseDbHelper.ExecuteScalar(IDbTransaction transaction, String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 846
   在 DBInterface.RDBInterface.BaseDbHelper.ExecuteScalar(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 796
   在 DBInterface.RDBInterface.DbHelper.ExecuteScalar(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:行号 276
   在 PSLCalcu.PSLDataDAO.WriteOrUpdate(UInt32 tagid, List`1 pvalues, DateTime startdate, DateTime enddate) 位置 d:\c#\PSLCalcu_Test\PSLCalcu\DAO\PSLDataDAO.cs:行号 260

2020-04-28 13:19:45,628 [18] ERROR PSLCalcu.AppRunForm - 计算引擎错误1：读取实时数据为空，第0个标签对应时间段内没有实时数据!
——计算模块的名称是：31-MOPCFilter2One，计算起始时间是：2020/4/1 0:06:00，计算结束时间是：2020/4/1 0:07:00。

2020-04-28 13:29:56,243 [14] ERROR PSLCalcu.PSLHistoryCalcuConfigDAO - DAO层PSLDataDAO.WriteOrUpdate()错误：---------->
sql语句：use psldb;update psldata202004 set tagid='11151',tagstarttime='637213140000000000',tagendtime='637213176000000000',tagvalue='0',tagstatus='100' where id='33243'
错误信息：System.Threading.ThreadAbortException: 正在中止线程。
   在 System.Buffer.InternalBlockCopy(Array src, Int32 srcOffsetBytes, Array dst, Int32 dstOffsetBytes, Int32 byteCount)
   在 System.IO.MemoryStream.Write(Byte[] buffer, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.MySqlPacket.Write(Byte[] bytesToWrite, Int32 offset, Int32 countToWrite)
   在 MySql.Data.MySqlClient.MySqlPacket.WriteStringNoNull(String v)
   在 MySql.Data.MySqlClient.Statement.InternalBindParameters(String sql, MySqlParameterCollection parameters, MySqlPacket packet)
   在 MySql.Data.MySqlClient.Statement.BindParameters()
   在 MySql.Data.MySqlClient.Statement.Execute()
   在 MySql.Data.MySqlClient.PreparableStatement.Execute()
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteReader(CommandBehavior behavior)
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteReader()
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQuery()
   在 DBInterface.RDBInterface.BaseDbHelper.ExecuteNonQuery(IDbTransaction transaction, String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 736
   在 DBInterface.RDBInterface.BaseDbHelper.ExecuteNonQuery(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 686
   在 DBInterface.RDBInterface.DbHelper.ExecuteNonQuery(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:行号 258
   在 PSLCalcu.PSLDataDAO.WriteOrUpdate(UInt32 tagid, List`1 pvalues, DateTime startdate, DateTime enddate) 位置 d:\c#\PSLCalcu_Test\PSLCalcu\DAO\PSLDataDAO.cs:行号 287

2020-04-28 13:39:22,924 [16] ERROR PSLCalcu.PSLHistoryCalcuConfigDAO - DAO层PSLDataDAO.WriteOrUpdate()错误：---------->
sql语句：use psldb;select id from psldata202004 where tagid='15764' and tagstarttime='637213680000000000'
错误信息：System.Threading.ThreadAbortException: 正在中止线程。
   在 MySql.Data.MySqlClient.NativeDriver.Ping()
   在 MySql.Data.MySqlClient.Driver.Ping()
   在 MySql.Data.MySqlClient.MySqlPool.GetPooledConnection()
   在 MySql.Data.MySqlClient.MySqlPool.TryToGetDriver()
   在 MySql.Data.MySqlClient.MySqlPool.GetConnection()
   在 MySql.Data.MySqlClient.MySqlConnection.Open()
   在 DBInterface.RDBInterface.BaseDbHelper.Open(String connectionString) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 330
   在 DBInterface.RDBInterface.DbHelper.ExecuteScalar(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:行号 275
   在 PSLCalcu.PSLDataDAO.WriteOrUpdate(UInt32 tagid, List`1 pvalues, DateTime startdate, DateTime enddate) 位置 d:\c#\PSLCalcu_Test\PSLCalcu\DAO\PSLDataDAO.cs:行号 260

2020-04-28 13:40:39,872 [16] ERROR PSLCalcu.AppRunForm - 计算引擎错误1：读取实时数据为空，第0个标签对应时间段内没有实时数据!
——计算模块的名称是：44-MOPCFilter2One，计算起始时间是：2020/4/1 0:33:00，计算结束时间是：2020/4/1 0:34:00。

2020-04-28 13:40:42,938 [22] ERROR PSLCalcu.AppRunForm - 计算引擎错误1：读取实时数据为空，第0个标签对应时间段内没有实时数据!
——计算模块的名称是：12-MOPCFilter2One，计算起始时间是：2020/4/1 0:35:00，计算结束时间是：2020/4/1 0:36:00。

2020-04-28 13:41:27,224 [16] ERROR PSLCalcu.PSLHistoryCalcuConfigDAO - DAO层PSLDataDAO.WriteOrUpdate()错误：---------->
sql语句：use psldb;insert into psldata202004(tagid,tagstarttime,tagendtime,tagvalue,tagstatus) values ('18302','637214400000000000','637214436000000000','0','100')
错误信息：System.Threading.ThreadAbortException: 正在中止线程。
   在 MySql.Data.Common.MyNetworkStream.HandleOrRethrowException(Exception e)
   在 MySql.Data.Common.MyNetworkStream.Read(Byte[] buffer, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.TimedStream.Read(Byte[] buffer, Int32 offset, Int32 count)
   在 System.IO.BufferedStream.Read(Byte[] array, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.MySqlStream.ReadFully(Stream stream, Byte[] buffer, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.MySqlStream.LoadPacket()
   在 MySql.Data.MySqlClient.MySqlStream.ReadPacket()
   在 MySql.Data.MySqlClient.NativeDriver.GetResult(Int32& affectedRow, Int64& insertedId)
   在 MySql.Data.MySqlClient.Driver.GetResult(Int32 statementId, Int32& affectedRows, Int64& insertedId)
   在 MySql.Data.MySqlClient.Driver.NextResult(Int32 statementId, Boolean force)
   在 MySql.Data.MySqlClient.MySqlDataReader.NextResult()
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteReader(CommandBehavior behavior)
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteReader()
   在 MySql.Data.MySqlClient.MySqlCommand.ExecuteNonQuery()
   在 DBInterface.RDBInterface.BaseDbHelper.ExecuteNonQuery(IDbTransaction transaction, String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 736
   在 DBInterface.RDBInterface.BaseDbHelper.ExecuteNonQuery(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 686
   在 DBInterface.RDBInterface.DbHelper.ExecuteNonQuery(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:行号 258
   在 PSLCalcu.PSLDataDAO.WriteOrUpdate(UInt32 tagid, List`1 pvalues, DateTime startdate, DateTime enddate) 位置 d:\c#\PSLCalcu_Test\PSLCalcu\DAO\PSLDataDAO.cs:行号 274

2020-04-28 13:41:27,496 [14] ERROR PSLCalcu.PSLHistoryCalcuConfigDAO - DAO层PSLDataDAO.WriteOrUpdate()错误：---------->
sql语句：use psldb;select id from psldata202004 where tagid='15656' and tagstarttime='637214436000000000'
错误信息：MySql.Data.MySqlClient.MySqlException (0x80004005): Out of sync with server
   在 MySql.Data.MySqlClient.NativeDriver.ReadOk(Boolean read)
   在 MySql.Data.MySqlClient.NativeDriver.SetDatabase(String dbName)
   在 MySql.Data.MySqlClient.Driver.SetDatabase(String dbName)
   在 MySql.Data.MySqlClient.MySqlConnection.ChangeDatabase(String databaseName)
   在 MySql.Data.MySqlClient.MySqlConnection.Open()
   在 DBInterface.RDBInterface.BaseDbHelper.Open(String connectionString) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 330
   在 DBInterface.RDBInterface.DbHelper.ExecuteScalar(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:行号 275
   在 PSLCalcu.PSLDataDAO.WriteOrUpdate(UInt32 tagid, List`1 pvalues, DateTime startdate, DateTime enddate) 位置 d:\c#\PSLCalcu_Test\PSLCalcu\DAO\PSLDataDAO.cs:行号 260

2020-04-28 17:57:06,491 [10] ERROR PSLCalcu.PSLHistoryCalcuConfigDAO - DAO层PSLDataDAO.WriteOrUpdate()错误：---------->
sql语句：use psldb;select id from psldata202004 where tagid='126902' and tagstarttime='637236864000000000'
错误信息：System.Threading.ThreadAbortException: 正在中止线程。
   在 MySql.Data.Common.MyNetworkStream.HandleOrRethrowException(Exception e)
   在 MySql.Data.Common.MyNetworkStream.Read(Byte[] buffer, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.TimedStream.Read(Byte[] buffer, Int32 offset, Int32 count)
   在 System.IO.BufferedStream.Read(Byte[] array, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.MySqlStream.ReadFully(Stream stream, Byte[] buffer, Int32 offset, Int32 count)
   在 MySql.Data.MySqlClient.MySqlStream.LoadPacket()
   在 MySql.Data.MySqlClient.MySqlStream.ReadPacket()
   在 MySql.Data.MySqlClient.NativeDriver.ReadOk(Boolean read)
   在 MySql.Data.MySqlClient.NativeDriver.SetDatabase(String dbName)
   在 MySql.Data.MySqlClient.Driver.SetDatabase(String dbName)
   在 MySql.Data.MySqlClient.MySqlConnection.ChangeDatabase(String databaseName)
   在 MySql.Data.MySqlClient.MySqlConnection.Open()
   在 DBInterface.RDBInterface.BaseDbHelper.Open(String connectionString) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:行号 330
   在 DBInterface.RDBInterface.DbHelper.ExecuteScalar(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) 位置 d:\c#\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:行号 275
   在 PSLCalcu.PSLDataDAO.WriteOrUpdate(UInt32 tagid, List`1 pvalues, DateTime startdate, DateTime enddate) 位置 d:\c#\PSLCalcu_Test\PSLCalcu\DAO\PSLDataDAO.cs:行号 260
