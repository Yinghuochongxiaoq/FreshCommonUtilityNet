using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using FreshCommonUtility.Configure;
using FreshCommonUtility.Dapper;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.SqlHelper
{
    /// <summary>
    /// <para>Sql connection helper class.Use this helper class must first use InitConnectionServer function.</para>
    /// <para>config sql link string key:SqlConnectionString</para>
    /// </summary>
    public static class SqlConnectionHelper
    {
        /// <summary>
        /// Get connection string.
        /// </summary>
        private static string ConnectionString { get; set; }

        /// <summary>
        /// cut put point.
        /// </summary>
        static SqlConnectionHelper()
        {
            ConnectionString = AppConfigurationHelper.GetString("SqlConnectionString", null);
        }

        /// <summary>
        /// Get single MySQL connection object
        /// </summary>
        /// <param name="connectionString">you need new connection object.</param>
        /// <returns></returns>
        [Obsolete("This method is used by mysql,in next publish will delete.The new Mehtod is GetOpenConnection()")]
        public static MySqlConnection GetMySqlConnectionConnection(string connectionString = null)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                return new MySqlConnection(connectionString);
            }
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Get connection string.
        /// </summary>
        public static string GetConnectionString() => ConnectionString;

        /// <summary>
        /// <para>Get open connection</para>
        /// <para>if you want to link mysql db,please add SslMode=None; in you link string.</para>
        /// <para>MySQL link string eg:Server=127.0.0.1;Port=3306;User Id=test;Password=test;Database=testDb;SslMode=None</para>
        /// <para>Oracle link string eg:Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=test)));Persist Security Info=True;User ID=test;Password=test;</para>
        /// <para>SQLServer link string eg:Data Source = (localdb)\MSSQLLocalDB;Initial Catalog=DapperSimpleCrudTestDb;Integrated Security=True;MultipleActiveResultSets=true;</para>
        /// <para>SQLite link string eg:Data Source=MyDatabase.sqlite;Version=3;</para>
        /// </summary>
        /// <param name="connectionString">DIV you connection string</param>
        /// <returns>IDbConection object</returns>
        public static IDbConnection GetOpenConnection(string connectionString = null)
        {
            var dbtype = SimpleCRUD.GetDialect();
            if (!string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;
            }
            IDbConnection connection;
            //if (dbtype == SimpleCRUD.Dialect.PostgreSQL)
            //{
            //    connection = new NpgsqlConnection(ConnectionString);
            //    SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
            //}
            //else
            if (dbtype == SimpleCRUD.Dialect.MySQL)
            {
                connection = new MySqlConnection(ConnectionString);
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
            }
            else if (dbtype == SimpleCRUD.Dialect.SQLite)
            {
                connection = new SQLiteConnection(ConnectionString);
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
            }
            else if (dbtype == SimpleCRUD.Dialect.Oracle)
            {
                connection = new OracleConnection(ConnectionString);
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.Oracle);
            }
            else
            {
                connection = new SqlConnection(ConnectionString);
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
            }
            connection.Open();
            return connection;
        }
    }
}
