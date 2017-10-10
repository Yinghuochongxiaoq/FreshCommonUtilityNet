using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using FreshCommonUtility.CommonModel;
using FreshCommonUtility.SqlHelper;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dapper
{
    /// <summary>
    /// MySql part
    /// </summary>
    public static class MySqlPart
    {
        #region [1、Get disabled foreign key sql]

        /// <summary>
        /// Get disabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetDisabledForeignKeySql(this IDbConnection connection)
        {
            return @"SET FOREIGN_KEY_CHECKS=0;";
        }

        #endregion

        #region [2、Get enabled foreign key sql]

        /// <summary>
        /// Get enabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetEnabledForeignKeySql(this IDbConnection connection)
        {
            return @"SET FOREIGN_KEY_CHECKS=1;";
        }

        #endregion

        #region [3、Get delete foreign key sql]

        /// <summary>
        /// Get delete foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetDeleteForeignKeySql(this IDbConnection connection)
        {
            var disableSql = @"SELECT CONCAT('alter table ' , o.TABLE_NAME , ' drop foreign key ' , o.CONSTRAINT_NAME , ';') as ForeighKey
FROM
	INFORMATION_SCHEMA.KEY_COLUMN_USAGE o
WHERE REFERENCED_TABLE_NAME is not NULL ";
            var result = connection.Query<ReferencedForeighKeyModel>(disableSql);
            return result.Aggregate(string.Empty, (current, tempAlter) => current + tempAlter);
        }
        #endregion

        #region [4、Get recreat foreign key sql]

        /// <summary>
        /// Get recreat foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetReCreatForeignKeySql(this IDbConnection connection)
        {
            var disableSql = @"SELECT CONCAT('alter table ' , o.TABLE_NAME , ' ADD CONSTRAINT ' , o.CONSTRAINT_NAME , ' foreign key(',o.COLUMN_NAME,') REFERENCES ',o.REFERENCED_TABLE_NAME,'(',o.REFERENCED_COLUMN_NAME,');') as ForeighKey
FROM
	INFORMATION_SCHEMA.KEY_COLUMN_USAGE o
WHERE REFERENCED_TABLE_NAME is not NULL ";
            var result = connection.Query<ReferencedForeighKeyModel>(disableSql);
            return result.Aggregate(string.Empty, (current, tempAlter) => current + tempAlter);
        }
        #endregion

        #region [5、Delete Table or drop table SQL Code]
        /// <summary>
        /// Save table refenced model dictionary
        /// </summary>
        private static readonly Dictionary<string, List<string>> TableRefencedModelDictionary = new Dictionary<string, List<string>>();

        /// <summary>
        /// Delete or drop table by name of table or view.
        /// </summary>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <param name="type">1:delete;0:drop</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>delete or drop table or view sql command</returns>
        private static string GetDeleteOrDropDataTableSqlByName(List<string> tableNameList, string dataBase, bool isView, int type)
        {
            if (tableNameList == null || tableNameList.Count < 1) return string.Empty;
            var hadDeleteTable = new Dictionary<string, int>();
            var historyDictionary = new Dictionary<string, int>();
            StringBuilder resulteBuilder = new StringBuilder();
            var typeSql = type == 1 ? " DELETE FROM " : (type == 0 ? " DROP TABLE " : string.Empty);
            foreach (var tableName in tableNameList.Distinct().Where(tableName => !string.IsNullOrEmpty(tableName)))
            {
                if (historyDictionary.ContainsKey(tableName)) continue;
                var referencedTableList = GetDeleteTableNameList(tableName, historyDictionary);
                if (!string.IsNullOrEmpty(dataBase))
                {
                    dataBase = dataBase + ".dbo.";
                }
                string itemDeleteSql = ";IF EXISTS ( SELECT * FROM " + dataBase + " sysobjects WHERE name = '{0}' AND type = '" + (isView ? "V" : "U") + "') " + typeSql + dataBase + "[{0}] ;";
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
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// </summary>        private static Dictionary<string, List<string>> GetReferencedMap()
        {
            if (TableRefencedModelDictionary.Any()) return TableRefencedModelDictionary;
            string sqlCmd = $@"SELECT
	CONSTRAINT_NAME as ForeignKey,
	COLUMN_NAME as ForeignKeyCell,
	REFERENCED_COLUMN_NAME as ReferencedCell,
	REFERENCED_TABLE_NAME as ReferencedTableName,
	TABLE_NAME as TableName
FROM
	INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
	TABLE_SCHEMA = '{0}'
AND REFERENCED_COLUMN_NAME IS NOT NULL ";
            var resulteInfo = new List<ReferencedModel>();
            var connection = SqlConnectionHelper.GetOpenConnection();
            var dataBaseName = connection.Database;
            if (string.IsNullOrEmpty(dataBaseName)) return null;
            sqlCmd = string.Format(sqlCmd, dataBaseName);
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
                if (TableRefencedModelDictionary.ContainsKey(tempModel.ReferencedTableName)) continue;
                TableRefencedModelDictionary.Add(tempModel.ReferencedTableName, new List<string>());
            }
            dr.Close();

            //oriented graph
            foreach (var rowModel in resulteInfo.Where(rowModel => rowModel.ReferencedTableName != rowModel.TableName))
            {
                TableRefencedModelDictionary[rowModel.ReferencedTableName].Add(rowModel.TableName);
            }
            return TableRefencedModelDictionary;
        }

        /// <summary>
        /// get citation graph set.
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="historyDictionary">access link route</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns></returns>
        private static List<string> GetDeleteTableNameList(string tableName, Dictionary<string, int> historyDictionary)
        {
            if (string.IsNullOrEmpty(tableName)) return null;
            var referencedMap = GetReferencedMap();
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
        private static List<string> TraversingGraph(Dictionary<string, List<string>> sourceDictionary, string nodeName, Dictionary<string, int> historyDictionary)
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
        /// <param name="tableName">table or view name</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>drop table or view sql command</returns>
        public static string GetDropDataTableSqlByName(string tableName, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(new List<string> { tableName }, dataBase, isView, 1);
        }

        /// <summary>
        /// Drop table or view by name
        /// </summary>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>drop table or view sql command</returns>
        public static string GetDropDataTableSqlByName(List<string> tableNameList, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(tableNameList, dataBase, isView, 0);
        }

        /// <summary>
        /// Delete table or view by name
        /// </summary>
        /// <param name="tableName">table or view name</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-5</creattime>
        /// <returns>delete table or view sql command</returns>
        public static string GetDeleteDataTableSqlByName(string tableName, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(new List<string> { tableName }, dataBase, isView, 1);
        }

        /// <summary>
        /// Delete table or view by name
        /// </summary>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>delete table or view sql command</returns>
        public static string GetDeleteDataTableSqlByName(List<string> tableNameList, string dataBase = null, bool isView = false)
        {
            return GetDeleteOrDropDataTableSqlByName(tableNameList, dataBase, isView, 1);
        }
        #endregion
    }
}
