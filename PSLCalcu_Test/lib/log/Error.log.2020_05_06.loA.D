
2020-05-06 09:02:51,007 [14] ERROR PSLCalcu.PSLHistoryCalcuConfigDAO - DAO层PSLDataDAO.WriteOrUpdate()错误：---------->
sql语句：use psldb;select id from psldata202004 where tagid='135682' and tagstarttime='637238400000000000'
错误信息：System.TimeoutException: Timeout in IO operation
   在 MySql.Data.MySqlClient.TimedStream.StopTimer()
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
