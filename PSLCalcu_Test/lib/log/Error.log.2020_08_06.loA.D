
2020-08-06 22:50:23,029 [11] ERROR PSLCalcu.IniTable - DAO≤„connectTest()¥ÌŒÛ£∫---------->
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
