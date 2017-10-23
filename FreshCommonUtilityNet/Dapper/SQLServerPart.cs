using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using FreshCommonUtility.CommonModel;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dapper
{
    /// <summary>
    /// SQLServer part
    /// </summary>
    public class SqlServerPart : IDealMoreOtherPart
    {
        #region [1、Get disabled foreign key sql]

        /// <summary>
        /// Get disabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetDisabledForeignKeySql(IDbConnection connection)
        {
            var disableSql = @"DECLARE
        @nocheckSql NVARCHAR (MAX)
    SET @nocheckSql = (
        SELECT
            'alter table dbo.[' + b.name + '] nocheck constraint [' + a.name + '];'
        FROM
            sysobjects a,
            sysobjects b
        WHERE
            a.xtype = 'f'
        AND a.parent_obj = b.id
        AND b.xtype = 'u' FOR xml PATH('')
	) select @nocheckSql";
            var result = connection.ExecuteScalar(disableSql);
            return result.ToString();
        }

        #endregion

        #region [2、Get enabled foreign key sql]

        /// <summary>
        /// Get enabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetEnabledForeignKeySql(IDbConnection connection)
        {
            var disableSql = @"DECLARE
		@checkSql NVARCHAR (MAX)
	SET @checkSql = (
		SELECT
			'alter table dbo.[' + b.name + '] check constraint [' + a.name + '];'
		FROM
			sysobjects a,
			sysobjects b
		WHERE
			a.xtype = 'f'
		AND a.parent_obj = b.id
		AND b.xtype = 'u' FOR xml PATH ('')
	) select @checkSql";
            var result = connection.ExecuteScalar(disableSql);
            return result.ToString();
        }

        #endregion

        #region [3、Get delete foreign key sql]

        /// <summary>
        /// Get delete foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetDeleteForeignKeySql(IDbConnection connection)
        {
            var disableSql = @"DECLARE 
		@delSql nvarchar (MAX)
        SET @delSql = (
		SELECT
			'alter table [' + O.name + '] drop constraint [' + F.name + '];'
		FROM
			sysobjects O,
			sys.foreign_keys F
		WHERE
			F.parent_object_id = O.id FOR xml path ('')
	) select @delSql ";
            var result = connection.ExecuteScalar(disableSql);
            return result.ToString();
        }
        #endregion

        #region [4、Get recreat foreign key sql]

        /// <summary>
        /// Get recreat foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetReCreatForeignKeySql(IDbConnection connection)
        {
            var disableSql = @"DECLARE 
		@createSql nvarchar (MAX)
        SET @createSql = (
		SELECT
			'ALTER TABLE [' + OBJECT_NAME(k.parent_object_id) + '] ADD CONSTRAINT [' + k.name + '] FOREIGN KEY ([' + COL_NAME(
				k.parent_object_id,
				c.parent_column_id
			) + ']) REFERENCES [' + OBJECT_NAME(k.referenced_object_id) + ']([' + COL_NAME(
				k.referenced_object_id,
				key_index_id
			) + '])' + CASE k.delete_referential_action
		WHEN 0 THEN
			''
		WHEN 1 THEN
			' ON DELETE CASCADE '
		WHEN 2 THEN
			' ON DELETE SET NULL '
		WHEN 3 THEN
			' ON DELETE SET DEFAULT '
		END + CASE k.update_referential_action
		WHEN 0 THEN
			''
		WHEN 1 THEN
			' ON UPDATE CASCADE '
		WHEN 2 THEN
			' ON UPDATE SET NULL '
		WHEN 3 THEN
			' ON UPDATE SET DEFAULT'
		END + ';'
		FROM
			sys.foreign_keys k,
			sys.foreign_key_columns c
		WHERE
			c.constraint_object_id = k.object_id FOR xml path ('')
	) select @createSql ";
            var result = connection.ExecuteScalar(disableSql);
            return result.ToString();
        }
        #endregion

        #region [5、Delete Table or drop table SQL Code]
        /// <summary>
        /// Save table refenced model dictionary
        /// </summary>
        private readonly Dictionary<string, List<string>> _tableRefencedModelDictionary = new Dictionary<string, List<string>>();

        /// <summary>
        /// Delete or drop table by name of table or view.
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <param name="type">1:delete;0:drop</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>delete or drop table or view sql command</returns>
        private string GetDeleteOrDropDataTableSqlByName(IDbConnection connection, List<string> tableNameList, string dataBase, bool isView, int type)
        {
            if (tableNameList == null || tableNameList.Count < 1) return string.Empty;
            var hadDeleteTable = new Dictionary<string, int>();
            var historyDictionary = new Dictionary<string, int>();
            StringBuilder resulteBuilder = new StringBuilder();
            var typeSql = type == 1 ? " DELETE FROM " : (type == 0 ? " DROP TABLE " : string.Empty);
            foreach (var tableName in tableNameList.Distinct().Where(tableName => !string.IsNullOrEmpty(tableName)))
            {
                if (historyDictionary.ContainsKey(tableName)) continue;
                var referencedTableList = GetDeleteTableNameList(connection, tableName, historyDictionary);
                if (!string.IsNullOrEmpty(dataBase))
                {
                    dataBase = dataBase + ".dbo.";
                }
                string itemDeleteSql = ";IF EXISTS ( SELECT * FROM " + dataBase + "sysobjects WHERE name = '{0}' AND type = '" + (isView ? "V" : "U") + "') " + typeSql + dataBase + "[{0}] ;";
                if (referencedTableList == null || referencedTableList.Count < 1) continue;
                foreach (var tempString in referencedTableList)
                {
                    if (hadDeleteTable.ContainsKey(tempString))
                    {
                        hadDeleteTable[tempString]++;
                    }
                    else
                    {
                        hadDeleteTable.Add(tempString, 1);
                        resulteBuilder.AppendFormat(itemDeleteSql, tempString);
                    }
                }
            }
            return resulteBuilder.ToString();
        }

        /// <summary>
        /// Citation Graph
        /// </summary>
        /// <param name="connection">connection</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        private Dictionary<string, List<string>> GetReferencedMap(IDbConnection connection)
        {
            if (_tableRefencedModelDictionary.Any()) return _tableRefencedModelDictionary;
            string sqlCmd = $@"
SELECT
    object_name(constraint_object_id) ForeignKey,
	object_name(parent_object_id) TableName,
	col_name(
        parent_object_id,
        parent_column_id
    ) ForeignKeyCell,
	object_name(referenced_object_id) ReferencedTableName,
	col_name(
        referenced_object_id,
        referenced_column_id
    ) ReferencedCell
FROM
    sys.foreign_key_columns ";
            var resulteInfo = new List<ReferencedModel>();
            var dr = connection.ExecuteReader(sqlCmd);
            while (dr.Read())
            {
                var tempModel = new ReferencedModel
                {
                    ForeignKey = (string)dr["ForeignKey"],
                    ForeignKeyCell = (string)dr["ForeignKeyCell"],
                    ReferencedCell = (string)dr["ReferencedCell"],
                    ReferencedTableName = (string)dr["ReferencedTableName"],
                    TableName = (string)dr["TableName"]
                };
                resulteInfo.Add(tempModel);
                if (_tableRefencedModelDictionary.ContainsKey(tempModel.ReferencedTableName)) continue;
                _tableRefencedModelDictionary.Add(tempModel.ReferencedTableName, new List<string>());
            }
            dr.Close();

            //oriented graph
            foreach (var rowModel in resulteInfo.Where(rowModel => rowModel.ReferencedTableName != rowModel.TableName))
            {
                _tableRefencedModelDictionary[rowModel.ReferencedTableName].Add(rowModel.TableName);
            }
            return _tableRefencedModelDictionary;
        }

        /// <summary>
        /// get citation graph set.
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="tableName">table name</param>
        /// <param name="historyDictionary">access link route</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns></returns>
        private List<string> GetDeleteTableNameList(IDbConnection connection, string tableName, Dictionary<string, int> historyDictionary)
        {
            if (string.IsNullOrEmpty(tableName)) return null;
            var referencedMap = GetReferencedMap(connection);
            if (!referencedMap.ContainsKey(tableName))
            {
                historyDictionary.Add(tableName, 1);
                return new List<string> { tableName };
            }
            var resultList = TraversingGraph(referencedMap, tableName, historyDictionary);
            return resultList;
        }

        /// <summary>
        /// Recursive completion of depth traversal graphs
        /// </summary>
        /// <param name="sourceDictionary">source node data</param>
        /// <param name="nodeName">current node name</param>
        /// <param name="historyDictionary">had access</param>
        /// <returns></returns>
        private List<string> TraversingGraph(Dictionary<string, List<string>> sourceDictionary, string nodeName, Dictionary<string, int> historyDictionary)
        {
            var result = new List<string>();
            //是否已经访问过
            if (historyDictionary.ContainsKey(nodeName)) return result;
            //出度为0
            if (!sourceDictionary.ContainsKey(nodeName))
            {
                //标记已经访问
                historyDictionary.Add(nodeName, 1);
                result.Add(nodeName);
                return result;
            }
            //出度大于0
            for (int i = 0; i < sourceDictionary[nodeName].Count; i++)
            {
                var nextNodeName = sourceDictionary[nodeName][i];
                var recurrenceList = TraversingGraph(sourceDictionary, nextNodeName, historyDictionary);
                if (recurrenceList != null && recurrenceList.Any()) result.AddRange(recurrenceList);
            }
            result.Add(nodeName);
            //标记已经访问
            historyDictionary.Add(nodeName, 1);
            return result;
        }

        /// <summary>
        /// Drop table or view by name
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="tableName">table or view name</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>drop table or view sql command</returns>
        public string GetDropDataTableSqlByName(IDbConnection connection, string tableName, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(connection, new List<string> { tableName }, dataBase, isView, 0);
        }

        /// <summary>
        /// Drop table or view by name
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>drop table or view sql command</returns>
        public string GetDropDataTableSqlByName(IDbConnection connection, List<string> tableNameList, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(connection, tableNameList, dataBase, isView, 0);
        }

        /// <summary>
        /// Delete table or view by name
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="tableName">table or view name</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-5</creattime>
        /// <returns>delete table or view sql command</returns>
        public string GetDeleteDataTableSqlByName(IDbConnection connection, string tableName, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(connection, new List<string> { tableName }, dataBase, isView, 1);
        }

        /// <summary>
        /// Delete table or view by name
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>delete table or view sql command</returns>
        public string GetDeleteDataTableSqlByName(IDbConnection connection, List<string> tableNameList, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(connection, tableNameList, dataBase, isView, 1);
        }
        #endregion

        #region [6、BulkCopy DataTable data to DB table]

        /// <summary> 
        /// insert large data
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="tableName">tablename</param>
        /// <param name="dt">the same sturction of datatable</param>
        /// <param name="timeOut">time out</param>
        public static void BulkCopy(IDbConnection connection, string tableName, DataTable dt, int timeOut = 60)
        {
            if (string.IsNullOrEmpty(tableName) || dt == null || dt.Rows.Count < 0) return;
            var sqlConnection = connection as SqlConnection;
            if (sqlConnection == null) return;
            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.BulkCopyTimeout = timeOut;
                    bulkCopy.DestinationTableName = tableName;
                    try
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }
                        bulkCopy.WriteToServer(dt);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// insert large data
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="dt">the same sturction of datatable</param>
        /// <param name="timeOut">time out</param>
        public static void BulkCopy(IDbConnection connection, DataTable dt, int timeOut = 60)
        {
            BulkCopy(connection, dt.TableName, dt, timeOut);
        }

        /// <summary>
        /// insert large data
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="ds">Table's set,every one have the same as db table struction,Table's name is DB table name</param>
        /// <param name="timeOut">time out</param>
        public static void BulkCopy(IDbConnection connection, DataSet ds, int timeOut = 60)
        {
            if (ds == null || ds.Tables.Count < 1) return;
            var sqlConnection = connection as SqlConnection;
            if (sqlConnection == null) return;
            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        if (dt == null || dt.Rows.Count < 1 || string.IsNullOrEmpty(dt.TableName)) continue;
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.BulkCopyTimeout = timeOut;
                            bulkCopy.DestinationTableName = dt.TableName;
                            foreach (DataColumn col in dt.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dt);
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <summary>
        /// insert large data
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="ds">Table's set,every one have the same as db table struction,Table's name is DB table name</param>
        /// <param name="timeOut">time out</param>
        public static void BulkCopy(IDbConnection connection, List<DataTable> ds, int timeOut = 60)
        {
            if (ds == null || ds.Count < 1) return;
            var sqlConnection = connection as SqlConnection;
            if (sqlConnection == null) return;
            using (SqlTransaction transaction = sqlConnection.BeginTransaction())
            {
                try
                {
                    foreach (DataTable dt in ds)
                    {
                        if (dt == null || dt.Rows.Count < 1 || string.IsNullOrEmpty(dt.TableName)) continue;
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.BulkCopyTimeout = timeOut;
                            bulkCopy.DestinationTableName = dt.TableName;
                            foreach (DataColumn col in dt.Columns)
                            {
                                bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                            }
                            bulkCopy.WriteToServer(dt);
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        #endregion
    }
}
