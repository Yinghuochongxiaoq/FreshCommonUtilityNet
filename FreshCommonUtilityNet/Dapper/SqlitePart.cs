#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.Dapper
//文件名称：SqlitePart
//创 建 人：FreshMan
//创建日期：2017/10/14 16:34:50
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using FreshCommonUtility.Dapper;
using FreshCommonUtilityNet.CommonModel;

namespace FreshCommonUtilityNet.Dapper
{
    /// <summary>
    /// SQLite part
    /// </summary>
    public class SqlitePart : IDealMoreOtherPart
    {
        #region [1、Get disabled foreign key sql]
        /// <summary>
        /// get disabled foreignkey sql
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetDisabledForeignKeySql(IDbConnection connection)
        {
            return "PRAGMA foreign_keys = OFF";
        }
        #endregion

        #region  [2、Get enabled foreign key sql]
        /// <summary>
        /// get enable foreign key sql
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetEnabledForeignKeySql(IDbConnection connection)
        {
            return "PRAGMA foreign_keys = ON";
        }
        #endregion

        #region  [3、Get delete foreign key sql]
        /// <summary>
        /// Get delte foreign key sql
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetDeleteForeignKeySql(IDbConnection connection)
        {
            return string.Empty;
        }
        #endregion

        #region  [4、Get recreat foreign key sql]
        /// <summary>
        /// get recreat foreign key sql
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string GetReCreatForeignKeySql(IDbConnection connection)
        {
            return string.Empty;
        }
        #endregion

        #region [5、Delete Table or drop table SQL Code]

        /// <summary>
        /// Get drop datatable sql by name
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataBase"></param>
        /// <param name="isView"></param>
        /// <returns></returns>
        public string GetDropDataTableSqlByName(IDbConnection connection, string tableName, string dataBase = null, bool isView = false)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                var exception = new ArgumentException("Drop table name is null.", nameof(tableName));
                throw exception;
            }
            if (!string.IsNullOrEmpty(dataBase))
            {
                dataBase = dataBase + ".";
            }
            var tableOrView = isView ? "view" : "table";
            return $" drop {tableOrView} if  exists {dataBase}{tableName} ;";
        }

        /// <summary>
        /// Get drop datatable sql by name
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableNameList"></param>
        /// <param name="dataBase"></param>
        /// <param name="isView"></param>
        /// <returns></returns>
        public string GetDropDataTableSqlByName(IDbConnection connection, List<string> tableNameList, string dataBase = null,
            bool isView = false)
        {
            if (tableNameList == null || tableNameList.Count < 1)
            {
                var exception = new ArgumentException("Drop table name is null.", nameof(tableNameList));
                throw exception;
            }
            var dropSql = new StringBuilder();
            foreach (string t in tableNameList)
            {
                dropSql.Append(GetDropDataTableSqlByName(connection, t, dataBase, isView));
            }
            return dropSql.ToString();
        }

        /// <summary>
        /// Get drop datatable sql by name
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// <param name="dataBase"></param>
        /// <param name="isView"></param>
        /// <returns></returns>
        public string GetDeleteDataTableSqlByName(IDbConnection connection, string tableName, string dataBase = null,
            bool isView = false)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                var exception = new ArgumentException("Delete table name is null.", nameof(tableName));
                throw exception;
            }
            var searchSql = $"select type as Type,name as Name,tbl_name as TblName,sql as Sql from sqlite_master where type='table' and name='{tableName}' ";
            var result = connection.Query<SqliteMasterModel>(searchSql);
            var sqliteMasterModels = result as IList<SqliteMasterModel> ?? result.ToList();
            if (result != null && !sqliteMasterModels.Any()) return string.Empty;
            var deletSql = new StringBuilder();
            foreach (var itemTable in sqliteMasterModels)
            {
                deletSql.Append($"delete from {itemTable.Name};");
            }
            return deletSql.ToString();
        }

        /// <summary>
        /// Get drop datatable sql by name
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableNameList"></param>
        /// <param name="dataBase"></param>
        /// <param name="isView"></param>
        /// <returns></returns>
        public string GetDeleteDataTableSqlByName(IDbConnection connection, List<string> tableNameList, string dataBase = null,
            bool isView = false)
        {
            if (tableNameList == null || tableNameList.Count < 1)
            {
                var exception = new ArgumentException("Delete table name is null.", nameof(tableNameList));
                throw exception;
            }
            var inParam = tableNameList.Aggregate(string.Empty, (current, t) => current + ("'" + t + "',"));
            inParam = inParam.TrimEnd(',');
            var searchSql = $"select type as Type,name as Name,tbl_name as TblName,sql as Sql from sqlite_master where type='table' and name in({inParam} )";
            var result = connection.Query<SqliteMasterModel>(searchSql);
            var sqliteMasterModels = result as IList<SqliteMasterModel> ?? result.ToList();
            if (result != null && !sqliteMasterModels.Any()) return string.Empty;
            var deletSql = new StringBuilder();
            foreach (var itemTable in sqliteMasterModels)
            {
                deletSql.Append($"delete from {itemTable.Name};");
            }
            return deletSql.ToString();
        }
        #endregion
    }
}
