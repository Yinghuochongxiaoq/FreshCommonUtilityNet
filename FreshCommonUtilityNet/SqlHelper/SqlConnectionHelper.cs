using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using FreshCommonUtility.Configure;
using FreshCommonUtility.Dapper;
using MySql.Data.MySqlClient;

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
        /// Return connection object.
        /// </summary>
        private static MySqlConnection _conn;

        /// <summary>
        /// Lock resource
        /// </summary>
        private static readonly object SyncRoot = new object();

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
        public static MySqlConnection GetMySqlConnectionConnection(string connectionString = null)
        {
            if (!string.IsNullOrEmpty(connectionString) && connectionString.GetHashCode() != ConnectionString.GetHashCode())
            {
                _conn = new MySqlConnection(connectionString);
            }
            if (_conn == null)
            {
                lock (SyncRoot)
                {
                    if (_conn == null)
                    {
                        _conn = new MySqlConnection(ConnectionString);
                    }
                }
            }
            return _conn;
        }

        /// <summary>
        /// Get connection string.
        /// </summary>
        public static string GetConnectionString() => ConnectionString;

        /// <summary>
        /// <para>Get open connection</para>
        /// <para>if you want to link mysql db,please add SslMode=None; in you link string.</para>
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
