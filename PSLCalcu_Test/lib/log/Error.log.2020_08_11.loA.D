
2020-08-11 14:34:15,741 [9] ERROR PSLCalcu.PSLTagNameIdMapDAO - DAO层PSLTagNameIdMapDAO.ReadMap()错误：读取计算标签名称id映射表时出错---------->

2020-08-11 14:34:15,742 [9] ERROR PSLCalcu.PSLTagNameIdMapDAO - sql语句：use psldb;select * from psltagnameidmap 

2020-08-11 14:34:15,798 [9] ERROR PSLCalcu.PSLTagNameIdMapDAO - 错误信息：MySql.Data.MySqlClient.MySqlException (0x80004005): Unknown database 'psldb'
   at MySql.Data.MySqlClient.MySqlStream.ReadPacket()
   at MySql.Data.MySqlClient.NativeDriver.GetResult(Int32& affectedRow, Int64& insertedId)
   at MySql.Data.MySqlClient.Driver.GetResult(Int32 statementId, Int32& affectedRows, Int64& insertedId)
   at MySql.Data.MySqlClient.Driver.NextResult(Int32 statementId, Boolean force)
   at MySql.Data.MySqlClient.MySqlDataReader.NextResult()
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReader(CommandBehavior behavior)
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteDbDataReader(CommandBehavior behavior)
   at System.Data.Common.DbCommand.ExecuteReader(CommandBehavior behavior)
   at DBInterface.RDBInterface.BaseDbHelper.ExecuteReader(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:line 582
   at DBInterface.RDBInterface.DbHelper.ExecuteReader(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:line 237
   at PSLCalcu.PSLTagNameIdMapDAO.ReadMap() in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu\DAO\PSLTagIdNameMapDAO.cs:line 657

2020-08-11 14:34:15,831 [9] ERROR PSLCalcu.PSLTagNameIdMapDAO - DAO层PSLTagNameIdMapDAO.ReadMap()错误：读取计算标签名称id映射表时出错---------->

2020-08-11 14:34:15,831 [9] ERROR PSLCalcu.PSLTagNameIdMapDAO - sql语句：use psldb;select * from psltagnameidmap 

2020-08-11 14:34:15,836 [9] ERROR PSLCalcu.PSLTagNameIdMapDAO - 错误信息：MySql.Data.MySqlClient.MySqlException (0x80004005): Unknown database 'psldb'
   at MySql.Data.MySqlClient.MySqlStream.ReadPacket()
   at MySql.Data.MySqlClient.NativeDriver.GetResult(Int32& affectedRow, Int64& insertedId)
   at MySql.Data.MySqlClient.Driver.GetResult(Int32 statementId, Int32& affectedRows, Int64& insertedId)
   at MySql.Data.MySqlClient.Driver.NextResult(Int32 statementId, Boolean force)
   at MySql.Data.MySqlClient.MySqlDataReader.NextResult()
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteReader(CommandBehavior behavior)
   at MySql.Data.MySqlClient.MySqlCommand.ExecuteDbDataReader(CommandBehavior behavior)
   at System.Data.Common.DbCommand.ExecuteReader(CommandBehavior behavior)
   at DBInterface.RDBInterface.BaseDbHelper.ExecuteReader(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:line 582
   at DBInterface.RDBInterface.DbHelper.ExecuteReader(String commandText, IDbDataParameter[] dbParameters, CommandType commandType) in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:line 237
   at PSLCalcu.PSLTagNameIdMapDAO.ReadFlagMap() in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu\DAO\PSLTagIdNameMapDAO.cs:line 691
