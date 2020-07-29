
2020-07-28 20:37:32,917 [8] ERROR PSLCalcu.IniTable - DAO≤„connectTest()¥ÌŒÛ£∫---------->
œÍœ∏¥ÌŒÛ–≈œ¢:MySql.Data.MySqlClient.MySqlException (0x80004005): Unable to connect to any of the specified MySQL hosts.
   at MySql.Data.MySqlClient.NativeDriver.Open()
   at MySql.Data.MySqlClient.Driver.Open()
   at MySql.Data.MySqlClient.Driver.Create(MySqlConnectionStringBuilder settings)
   at MySql.Data.MySqlClient.MySqlPool.CreateNewPooledConnection()
   at MySql.Data.MySqlClient.MySqlPool.GetPooledConnection()
   at MySql.Data.MySqlClient.MySqlPool.TryToGetDriver()
   at MySql.Data.MySqlClient.MySqlPool.GetConnection()
   at MySql.Data.MySqlClient.MySqlConnection.Open()
   at DBInterface.RDBInterface.BaseDbHelper.Open(String connectionString) in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:line 330
   at DBInterface.RDBInterface.BaseDbHelper.Open() in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:line 291
   at DBInterface.RDBInterface.DbHelper.ConnTest() in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:line 89
   at PSLCalcu.IniTable.connectTest() in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu\DAO\IniTable.cs:line 32

2020-07-28 20:40:40,793 [10] ERROR PSLCalcu.IniTable - DAO≤„connectTest()¥ÌŒÛ£∫---------->
œÍœ∏¥ÌŒÛ–≈œ¢:MySql.Data.MySqlClient.MySqlException (0x80004005): Unable to connect to any of the specified MySQL hosts.
   at MySql.Data.MySqlClient.NativeDriver.Open()
   at MySql.Data.MySqlClient.Driver.Open()
   at MySql.Data.MySqlClient.Driver.Create(MySqlConnectionStringBuilder settings)
   at MySql.Data.MySqlClient.MySqlPool.CreateNewPooledConnection()
   at MySql.Data.MySqlClient.MySqlPool.GetPooledConnection()
   at MySql.Data.MySqlClient.MySqlPool.TryToGetDriver()
   at MySql.Data.MySqlClient.MySqlPool.GetConnection()
   at MySql.Data.MySqlClient.MySqlConnection.Open()
   at DBInterface.RDBInterface.BaseDbHelper.Open(String connectionString) in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:line 330
   at DBInterface.RDBInterface.BaseDbHelper.Open() in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DBProvider\BaseDbHelper.cs:line 291
   at DBInterface.RDBInterface.DbHelper.ConnTest() in D:\C# Project\JSYXX\PSLCalcu_Test\DBInterface\DBHelper\DbHelper.cs:line 89
   at PSLCalcu.IniTable.connectTest() in D:\C# Project\JSYXX\PSLCalcu_Test\PSLCalcu\DAO\IniTable.cs:line 32

2020-07-28 20:52:42,155 [8] ERROR PSLCalcu.PSLTagNameIdMapDAO - DAO≤„PSLTagNameIdMapDAO.ReadMap()¥ÌŒÛ£∫∂¡»°º∆À„±Í«©√˚≥∆id”≥…‰±Ì ±≥ˆ¥Ì---------->

2020-07-28 20:52:42,155 [8] ERROR PSLCalcu.PSLTagNameIdMapDAO - sql”Ôæ‰£∫use psldb;select * from psltagnameidmap 

2020-07-28 20:52:42,167 [8] ERROR PSLCalcu.PSLTagNameIdMapDAO - ¥ÌŒÛ–≈œ¢£∫MySql.Data.MySqlClient.MySqlException (0x80004005): Unknown database 'psldb'
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

2020-07-28 20:52:42,186 [8] ERROR PSLCalcu.PSLTagNameIdMapDAO - DAO≤„PSLTagNameIdMapDAO.ReadMap()¥ÌŒÛ£∫∂¡»°º∆À„±Í«©√˚≥∆id”≥…‰±Ì ±≥ˆ¥Ì---------->

2020-07-28 20:52:42,186 [8] ERROR PSLCalcu.PSLTagNameIdMapDAO - sql”Ôæ‰£∫use psldb;select * from psltagnameidmap 

2020-07-28 20:52:42,186 [8] ERROR PSLCalcu.PSLTagNameIdMapDAO - ¥ÌŒÛ–≈œ¢£∫MySql.Data.MySqlClient.MySqlException (0x80004005): Unknown database 'psldb'
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

2020-07-28 21:55:58,820 [10] ERROR PSLCalcu.PSLTagNameIdMapDAO - DAO≤„PSLTagNameIdMapDAO.ReadMap()¥ÌŒÛ£∫∂¡»°º∆À„±Í«©√˚≥∆id”≥…‰±Ì ±≥ˆ¥Ì---------->

2020-07-28 21:55:58,820 [10] ERROR PSLCalcu.PSLTagNameIdMapDAO - sql”Ôæ‰£∫use psldb;select * from psltagnameidmap 

2020-07-28 21:55:58,860 [10] ERROR PSLCalcu.PSLTagNameIdMapDAO - ¥ÌŒÛ–≈œ¢£∫MySql.Data.MySqlClient.MySqlException (0x80004005): Unknown database 'psldb'
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

2020-07-28 21:55:58,880 [10] ERROR PSLCalcu.PSLTagNameIdMapDAO - DAO≤„PSLTagNameIdMapDAO.ReadMap()¥ÌŒÛ£∫∂¡»°º∆À„±Í«©√˚≥∆id”≥…‰±Ì ±≥ˆ¥Ì---------->

2020-07-28 21:55:58,880 [10] ERROR PSLCalcu.PSLTagNameIdMapDAO - sql”Ôæ‰£∫use psldb;select * from psltagnameidmap 

2020-07-28 21:55:58,880 [10] ERROR PSLCalcu.PSLTagNameIdMapDAO - ¥ÌŒÛ–≈œ¢£∫MySql.Data.MySqlClient.MySqlException (0x80004005): Unknown database 'psldb'
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
