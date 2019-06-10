using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Sqlite_UWP
{
    // List of commands to remember
    //private static String DB_NAME = "SQLiteSample.db";
    //private static String TABLE_NAME = "SampleTable";
    //private static String SQL_CREATE_TABLE = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " (Key TEXT,Value TEXT);";
    //private static String SQL_QUERY_VALUE = "SELECT Value FROM " + TABLE_NAME + " WHERE Key = (?);";
    //private static String SQL_INSERT = "INSERT INTO " + TABLE_NAME + " VALUES(?,?);";
    //private static String SQL_UPDATE = "UPDATE " + TABLE_NAME + " SET Value = ? WHERE Key = ?";
    //private static String SQL_DELETE = "DELETE FROM " + TABLE_NAME + " WHERE Key = ?"

    public class DataAccess
    {
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                string tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS MyTable (Id INTEGER PRIMARY KEY, " +
                    "Text_Entry NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public static void AddData(string inputText)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO MyTable VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public static void DeleteItem(object[] copyOfSelectedItems)
        {
            // Currently deletes an item
            // BUG: If there is more than 1 item with the same name, it deletes both -- Just need to compare to ID's
            using (SqliteConnection db =
                new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                // Delete all selectd items
                foreach (var item in copyOfSelectedItems)
                {
                    SqliteCommand deleteCommand = new SqliteCommand
                    ("DELETE FROM MyTable WHERE Text_Entry=@text", db);
                    deleteCommand.Parameters.AddWithValue("@text", item.ToString());
                    deleteCommand.ExecuteNonQuery();
                }

                db.Close();
            }
        }

        public static List<String> GetData()
        {
            List<String> entries = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT Text_Entry from MyTable", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                db.Close();
            }

            return entries;
        }


        public static void UpdateItem(int id, string text)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                SqliteCommand updateCommand = new SqliteCommand
                    ("UPDATE MyTable SET Text_Entry=@text WHERE Id=@id", db);
                //updateCommand.Parameters.AddWithValue("@text", "TESTING UPDATE ALL DATA");
                updateCommand.Parameters.AddWithValue("@text", text);
                updateCommand.Parameters.AddWithValue("@id", id);
                updateCommand.ExecuteNonQuery();

                db.Close();
            }
        }
    }
}
