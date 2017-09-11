using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using FreshCommonUtility.CommonModel;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.SqlHelper
{
    /// <summary>
    /// Ms sql helper
    /// </summary>
    public class MsSqlHelper
    {
        #region [1、删除表数据SQL Code]
        /// <summary>
        /// 存储表间关系有向图
        /// </summary>
        private static readonly Dictionary<string, List<string>> TableRefencedModelDictionary = new Dictionary<string, List<string>>();

        /// <summary>
        /// 根据表或视图名删除一个表或视图
        /// </summary>
        /// <param name="tableName">表或视图名称</param>
        /// <param name="dataBase">数据库名称，默认为当前链接数据库</param>
        /// <param name="isView">是否是视图：true：是；false：否（默认）</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-5</creattime>
        /// <returns>删除字符串</returns>
        public static string GetDeleteDataTableSqlByName(string tableName, string dataBase = null, bool isView = false)
        {
            return GetDeleteDataTableSqlByName(new List<string> { tableName }, dataBase, isView);
        }

        /// <summary>
        /// 根据表或视图名删除一个表或视图
        /// </summary>
        /// <param name="tableNameList">表或视图名称集合</param>
        /// <param name="dataBase">数据库名称，默认为当前链接数据库</param>
        /// <param name="isView">是否是视图：true：是；false：否（默认）</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>删除字符串</returns>
        public static string GetDeleteDataTableSqlByName(List<string> tableNameList, string dataBase = null, bool isView = false)
        {
            if (tableNameList == null || tableNameList.Count < 1) return string.Empty;
            var hadDeleteTable = new Dictionary<string, int>();
            var historyDictionary = new Dictionary<string, int>();
            StringBuilder resulteBuilder = new StringBuilder();
            foreach (var tableName in tableNameList.Distinct().Where(tableName => !string.IsNullOrEmpty(tableName)))
            {
                if (historyDictionary.ContainsKey(tableName)) continue;
                var referencedTableList = GetDeleteTableNameList(tableName, historyDictionary);
                if (!string.IsNullOrEmpty(dataBase))
                {
                    dataBase = dataBase + ".dbo.";
                }
                string itemDeleteSql = ";IF EXISTS ( SELECT * FROM " + dataBase + " sysobjects WHERE name = '{0}' AND type = '" + (isView ? "V" : "U") + "') DELETE FROM " + dataBase + "[{0}] ;";
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
        /// 获取引用图
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// </summary>        private static Dictionary<string, List<string>> GetReferencedMap()
        {
            if (TableRefencedModelDictionary.Any()) return TableRefencedModelDictionary;
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
            var connection = SqlConnectionHelper.GetOpenConnection();
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

            //形成有向图
            foreach (var rowModel in resulteInfo.Where(rowModel => rowModel.ReferencedTableName != rowModel.TableName))
            {
                TableRefencedModelDictionary[rowModel.ReferencedTableName].Add(rowModel.TableName);
            }
            return TableRefencedModelDictionary;
        }

        /// <summary>
        /// 获得被引用集合
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="historyDictionary">访问链路</param>
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
        /// 递归完成深度遍历图
        /// </summary>
        /// <param name="sourceDictionary">原始结点数据值</param>
        /// <param name="nodeName">当前结点</param>
        /// <param name="historyDictionary">访问记录是否已经访问过</param>
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
        #endregion
    }
}
