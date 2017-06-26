using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.SqlHelper
{
    /// <summary>
    /// MySql helper.
    /// </summary>
    public static class SqlHelper
    {
        #region [1、ExcuteNonQuery get result of execute sql.]

        /// <summary>
        /// 增、删、改同步操作
        ///  </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">链接字符串</param>
        /// <returns>int</returns>
        public static int ExcuteNonQuery(string cmd, DynamicParameters param, bool flag = false, string connection = null)
        {
            int result;
            IDbConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                result = con.Execute(cmd, param, null, null, flag ? CommandType.StoredProcedure : CommandType.Text);
                con.Close();
            }
            return result;
        }

        /// <summary>
        /// 增、删、改异步操作
        /// </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">链接字符串</param>
        /// <returns>int</returns>
        public static async Task<int> ExcuteNonQueryAsync(string cmd, DynamicParameters param, bool flag = false, string connection = null)
        {
            int result;
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            if (flag)
            {
                result = await con.ExecuteAsync(cmd, param, null, null, CommandType.StoredProcedure);
            }
            else
            {
                result = await con.ExecuteAsync(cmd, param, null, null, CommandType.Text);
            }
            return result;
        }
        #endregion

        #region [2、ExecuteScalar get single value sql result.]

        /// <summary>
        /// 同步查询操作
        /// </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">连接字符串</param>
        /// <returns>The first cell selected</returns>
        public static object ExecuteScalar(string cmd, DynamicParameters param, bool flag = false, string connection = null)
        {
            object result;
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                result = con.ExecuteScalar(cmd, param, null, null, flag ? CommandType.StoredProcedure : CommandType.Text);
                con.Close();
            }
            return result;
        }

        /// <summary>
        /// 异步查询操作
        /// </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">连接字符串</param>
        /// <returns>The first cell selected</returns>
        public static async Task<object> ExecuteScalarAsync(string cmd, DynamicParameters param, bool flag = false, string connection = null)
        {
            object result;
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            if (flag)
            {
                result = await con.ExecuteScalarAsync(cmd, param, null, null, CommandType.StoredProcedure);
            }
            else
            {
                result = await con.ExecuteScalarAsync(cmd, param, null, null, CommandType.Text);
            }
            return result;
        }
        #endregion

        #region [3、FindOne get first one data.]
        /// <summary>
        /// 同步查询一条数据
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">连接字符串</param>
        /// <returns>t</returns>
        public static T FindOne<T>(string cmd, DynamicParameters param, bool flag = false, string connection = null) where T : class, new()
        {
            T dataReader;
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                dataReader = con.Query<T>(cmd, param, commandType: flag ? CommandType.StoredProcedure : CommandType.Text).FirstOrDefault();
                con.Close();
            }
            return dataReader;
        }

        /// <summary>
        /// 异步查询一条数据
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">连接字符串</param>
        /// <returns>t</returns>
        public static async Task<T> FindOneAsync<T>(string cmd, DynamicParameters param, bool flag = false, string connection = null) where T : class, new()
        {
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            var dataReader = await con.QueryAsync<T>(cmd, param, commandType: flag ? CommandType.StoredProcedure : CommandType.Text);
            return dataReader.FirstOrDefault();
        }
        #endregion

        #region [4、FindToList  find data to list]
        /// <summary>
        /// 同步查询数据集合
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">连接字符串</param>
        /// <returns>t</returns>
        public static IList<T> FindToList<T>(string cmd, DynamicParameters param, bool flag = false, string connection = null) where T : class, new()
        {
            IEnumerable<T> dataReader;
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                dataReader = con.Query<T>(cmd, param, commandType: flag ? CommandType.StoredProcedure : CommandType.Text);
                con.Close();
            }
            return dataReader.ToList();
        }

        /// <summary>
        /// 异步查询数据集合
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="cmd">sql语句</param>
        /// <param name="param">参数</param>
        /// <param name="flag">true存储过程，false sql语句(default)</param>
        /// <param name="connection">连接字符串</param>
        /// <returns>t</returns>
        public static async Task<IList<T>> FindToListAsync<T>(string cmd, DynamicParameters param, bool flag = false, string connection = null) where T : class, new()
        {
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connection);
            var dataReader = await con.QueryAsync<T>(cmd, param, commandType: flag ? CommandType.StoredProcedure : CommandType.Text);
            return dataReader.ToList();
        }
        #endregion

        #region [5、Slow searchPage data]
        /// <summary>
        /// search page data,slowly.e.g:long sqlint;
        /// var param = new DynamicParameters();
        /// param.Add("id",1);
        /// var pagedata = fhelper.SearchPageList<PiFUsersModel />("pifusers", "and id=@id", null, "*", 0, 1, param, out sqlint);
        /// </summary>
        /// <param name="tbName">table name</param>
        /// <param name="strWhere">where case</param>
        /// <param name="orderBy">order field.</param>
        /// <param name="fieldList">search field</param>
        /// <param name="pageIndex">current page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="param">params.</param>
        /// <param name="allCount">all count number.</param>
        /// <param name="groupby">group by</param>
        /// <param name="connectionstring">connection string.</param>
        /// <returns>page data</returns>
        public static IList<T> SearchPageList<T>(string tbName, string strWhere, string orderBy, string fieldList, int pageIndex, int pageSize, DynamicParameters param, out long allCount, string groupby = null, string connectionstring = null) where T : class, new()
        {
            var searchCount = $"select count(*) from {tbName} where 1=1 {strWhere} ";
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connectionstring);
            // ReSharper disable once RedundantAssignment
            allCount = 0;
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                allCount = (long)con.ExecuteScalar(searchCount, param);
                con.Close();
            }
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            long startPageNum = (pageIndex - 1) * pageSize;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append($" {fieldList} FROM {tbName} WHERE 1=1 {strWhere} ");
            if (!string.IsNullOrEmpty(groupby))
            {
                sql.AppendFormat(" Group by {0} ", groupby);
            }
            sql.Append($" {orderBy} limit {startPageNum} ");
            sql.Append($",{pageSize} ");
            IEnumerable<T> tList;
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                tList = con.Query<T>(sql.ToString(), param, commandType: CommandType.Text);
                con.Close();
            }
            return tList.ToList();
        }

        /// <summary>
        /// search page data,slowly.e.g:long sqlint;
        /// <mark>
        /// 
        /// var param = new DynamicParameters();
        /// param.Add("id",1);
        /// var pagedata = fhelper.SearchPageList< PiFUsersModel />("pifusers", "and id=@id", null, "*", 0, 1, param, out sqlint);
        /// </mark>
        /// </summary>
        /// <param name="tbName">table name</param>
        /// <param name="strWhere">where case</param>
        /// <param name="orderBy">order field.</param>
        /// <param name="fieldList">search field</param>
        /// <param name="pageIndex">current page number</param>
        /// <param name="pageSize">page size</param>
        /// <param name="param">params.</param>
        /// <param name="connectionstring">connection string.</param>
        /// <returns>page data</returns>
        public static async Task<IList<T>> SearchPageListAsync<T>(string tbName, string strWhere, string orderBy, string fieldList, int pageIndex, int pageSize, DynamicParameters param, string connectionstring = null) where T : class, new()
        {
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connectionstring);
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            long startPageNum = (pageIndex - 1) * pageSize;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append($" {fieldList} FROM {tbName} WHERE 1=1 {strWhere} ");
            sql.Append($" {orderBy} limit {startPageNum} ");
            sql.Append($",{pageSize} ");
            var tList = await con.QueryAsync<T>(sql.ToString(), param, commandType: CommandType.Text);
            return tList.ToList();
        }

        /// <summary>
        /// search page data,high.e.g:long sqlint;
        /// </summary>
        /// var param = new DynamicParameters();
        /// param.Add("id",1);
        /// var pagedata = fhelper.SearchPageList<PiFUsersModel/>("pifusers", "and id=@id", null, "*", 0, 1, param, out sqlint);
        /// <typeparam name="T"></typeparam>
        /// <param name="tbName">table name</param>
        /// <param name="strWhere">where case(begin and)</param>
        /// <param name="orderBy">order filed</param>
        /// <param name="fieldList">search field</param>
        /// <param name="primaryKey">primary key for imporove speed</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="allCount">all count data row</param>
        /// <param name="param">params</param>
        /// <param name="connectionstring">connection database string.</param>
        /// <returns></returns>
        public static IList<T> SearchPageListHigh<T>(string tbName, string strWhere, string orderBy, string fieldList, string primaryKey, int pageIndex, int pageSize, out long allCount, DynamicParameters param, string connectionstring = null)
        {
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connectionstring);
            //Search count.
            var searchCountStr = $"SELECT COUNT({primaryKey}) as dataCount from {tbName}  where 1=1 {strWhere}";

            object dataCount;
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                dataCount = con.ExecuteScalar(searchCountStr, param);
                con.Close();
            }
            allCount = (long)dataCount;
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            long startPageNum = (pageIndex - 1) * pageSize;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append($" {fieldList} FROM {tbName} WHERE {primaryKey} < (SELECT {primaryKey} FROM {tbName} WHERE {strWhere} ORDER BY {primaryKey} desc LIMIT {startPageNum},1)");
            sql.Append($" {strWhere} ");
            sql.Append($" {orderBy} limit {pageSize} ");

            IEnumerable<T> tList;
            lock (con)
            {
                if (con.State == ConnectionState.Closed) con.Open();
                tList = con.Query<T>(sql.ToString(), param, commandType: CommandType.Text);
                con.Close();
            }
            return tList.ToList();
        }

        /// <summary>
        /// Async search page data,high.e.g:long sqlint;
        /// var param = new DynamicParameters();
        /// param.Add("id",1);
        /// var pagedata = fhelper.SearchPageList<PiFUsersModel/>("pifusers", "and id=@id", null, "*", 0, 1, param, out sqlint);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbName">table name</param>
        /// <param name="strWhere">where case(begin and)</param>
        /// <param name="orderBy">order filed</param>
        /// <param name="fieldList">search field</param>
        /// <param name="primaryKey">primary key for imporove speed</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <param name="param">params</param>
        /// <param name="connectionstring">connection database string.</param>
        /// <returns></returns>
        public static async Task<IList<T>> SearchPageListHighAsync<T>(string tbName, string strWhere, string orderBy, string fieldList, string primaryKey, int pageIndex, int pageSize, DynamicParameters param, string connectionstring = null)
        {
            MySqlConnection con = SqlConnectionHelper.GetMySqlConnectionConnection(connectionstring);
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            long startPageNum = (pageIndex - 1) * pageSize;
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append($" {fieldList} FROM {tbName} WHERE {primaryKey} < (SELECT {primaryKey} FROM {tbName} WHERE {strWhere} ORDER BY {primaryKey} desc LIMIT {startPageNum},1)");
            sql.Append($" {strWhere} ");
            sql.Append($" {orderBy} limit {pageSize} ");
            var tList = await con.QueryAsync<T>(sql.ToString(), param, commandType: CommandType.Text);
            return tList.ToList();
        }
        #endregion
    }
}
