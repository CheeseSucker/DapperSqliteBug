using System;
using Dapper;
using Microsoft.Data.Sqlite;

namespace DapperSqliteBug
{
    internal class MyModel
    {
        public long MyValue;
    }

    /// <summary>
    /// Demonstration of a bug with Dapper 1.50.x and SqLite
    /// If the first value in a column is NULL, then Dapper crashes on the next non-NULL value. 
    /// </summary>
    public class Program
    {

        public static void Main(string[] args)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                connection.Open();
                connection.Execute("CREATE TABLE MyTable (MyValue INTEGER)");
                connection.Execute("INSERT INTO MyTable (MyValue) VALUES (4)");
                connection.Execute("INSERT INTO MyTable (MyValue) VALUES (NULL)");
                connection.Execute("INSERT INTO MyTable (MyValue) VALUES (4)");

                // This is fine
                var result1 = connection.Query<MyModel>("SELECT * FROM MyTable");

                // This is also fine
                var result2 = connection.Query<MyModel>("SELECT * FROM MyTable ORDER BY MyValue IS NULL ASC");

                // This will fail because NULL is the first value in the column
                var result3 = connection.Query<MyModel>("SELECT * FROM MyTable ORDER BY MyValue IS NULL DESC");

                // InvalidCastException has been encountered before this line
                connection.Close();
            }
        }
    }
}
