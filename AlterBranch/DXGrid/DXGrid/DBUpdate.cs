using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace DXGrid
{
    public static class DBUpdate
    {
        const int dbVersion = 3;

        public static void DbVersionUpdate()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"CREATE TABLE IF NOT EXISTS dbConfig (dbVersion INTEGER NOT NULL,
	                                                                      dbVersionDate TEXT)"
                };
                Command.ExecuteNonQuery();
                Connect.Close();
            }

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"INSERT INTO dbConfig (dbVersion, dbVersionDate)
                                     VALUES (1, '{DateTime.Now:g}')"
                };
                Command.ExecuteNonQuery();
                Connect.Close();
            }
        }

        public static void DbStructureUpdate()
        {
            //чек версии бд
            int currentDBVersion = 0;

            using (SQLiteConnection Connect1 = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect1.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = Connect1,
                    CommandText = $@"SELECT dbVersion FROM dbConfig WHERE ROWID = 1"
                };
                SQLiteDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    currentDBVersion = Convert.ToInt32(sqlReader["dbVersion"]);
                }
            }

            //обновление структуры если требуется
            if (currentDBVersion == 1)
            {
                using (SQLiteConnection Connect2 = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect2.Open();
                    SQLiteCommand Command = new SQLiteCommand
                    {
                        Connection = Connect2,
                        CommandText = $@"ALTER TABLE GridProjectTable ADD COLUMN Telegram TEXT"
                    };
                    Command.ExecuteNonQuery();
                    Connect2.Close();
                }

                using (SQLiteConnection Connect2 = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect2.Open();
                    SQLiteCommand Command = new SQLiteCommand
                    {
                        Connection = Connect2,
                        CommandText = $@"UPDATE dbConfig SET dbVersion = {dbVersion} WHERE ROWID = 1"
                    };
                    Command.ExecuteNonQuery();
                    Connect2.Close();
                }
                currentDBVersion = 2;
            }
            if (currentDBVersion == 2)
            {
                using (SQLiteConnection Connect2 = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect2.Open();
                    SQLiteCommand Command = new SQLiteCommand
                    {
                        Connection = Connect2,
                        CommandText = $@"ALTER TABLE GridProjectTable ADD COLUMN MaxInt INTEGER"
                    };
                    Command.ExecuteNonQuery();
                    Connect2.Close();
                }

                using (SQLiteConnection Connect2 = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect2.Open();
                    SQLiteCommand Command = new SQLiteCommand
                    {
                        Connection = Connect2,
                        CommandText = $@"UPDATE dbConfig SET dbVersion = {dbVersion} WHERE ROWID = 1"
                    };
                    Command.ExecuteNonQuery();
                    Connect2.Close();
                }
            }
        }
    }
}
