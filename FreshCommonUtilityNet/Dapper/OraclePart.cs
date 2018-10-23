using Dapper;
using FreshCommonUtility.CommonModel;
using FreshCommonUtility.Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FreshCommonUtilityNet.Dapper
{
    /// <summary>
    /// Oracle part info.
    /// </summary>
    public class OraclePart : IDealMoreOtherPart
    {
        /*
        SELECT
	'alter table ' || table_name || ' enable constraint ' || constraint_name || ';' tt
FROM
	user_constraints
WHERE
	constraint_type = 'R';

SELECT
	'alter table ' || table_name || ' disable constraint ' || constraint_name || ';' tt
FROM
	user_constraints
WHERE
	constraint_type = 'R';

SELECT
	'alter table ' || table_name || ' drop constraint ' || constraint_name || ';' tt
FROM
	user_constraints
WHERE
	constraint_type = 'R';

SELECT
	'ALTER TABLE ' || CHILD.TABLE_NAME || ' ' || 'ADD CONSTRAINT ' || CHILD.CONSTRAINT_NAME || ' ' || 'FOREIGN KEY ' || '(' || cp.COLUMN_NAME || ')' || ' ' || 'REFERENCES ' || PARENT.TABLE_NAME || ' ' || '(' || pc.COLUMN_NAME || ') ' || CHILD.DELETE_RULE || ' ;'
FROM
	USER_CONSTRAINTS CHILD,
	USER_CONSTRAINTS PARENT,
	USER_CONS_COLUMNS cp,
	USER_CONS_COLUMNS pc
WHERE
	CHILD .CONSTRAINT_TYPE = 'R'
AND CHILD .R_OWNER = PARENT.OWNER
AND CHILD .R_CONSTRAINT_NAME = PARENT.CONSTRAINT_NAME
AND CHILD .CONSTRAINT_NAME = cp.CONSTRAINT_NAME
AND PARENT .CONSTRAINT_NAME = pc.CONSTRAINT_NAME
AND cp.POSITION = pc.POSITION
ORDER BY
	CHILD.OWNER,
	CHILD.TABLE_NAME,
	CHILD.CONSTRAINT_NAME,
	cp.POSITION;

CREATE TABLE zhao (
	ID NUMBER PRIMARY KEY,
	mingcheng NVARCHAR2 (50),
	neirong NVARCHAR2 (50),
	jiezhiriqi DATE,
	zhuangtai NVARCHAR2 (50)
);

CREATE TABLE tou (
	ID NUMBER PRIMARY KEY,
	zhao_id NUMBER,
	toubiaoqiye NVARCHAR2 (50),
	biaoshuneirong NVARCHAR2 (50),
	toubiaoriqi DATE,
	baojia NUMBER,
	zhuangtai NVARCHAR2 (50),
	FOREIGN KEY (zhao_id) REFERENCES zhao (ID)
);

BEGIN
declare
      num   number;
begin
    select count(1) INTO num from user_tables where table_name = upper('sys_FreshMan') ;
    if num > 0 then
        execute immediate 'drop table sys_FreshMan' ;
    end if;
end;
end;

ALTER TABLE TOU ADD CONSTRAINT SYS_C0021255 FOREIGN KEY (ZHAO_ID) REFERENCES ZHAO (ID) NO ACTION;

*/
        #region [1、Get disabled foreign key sql]

        /// <summary>
        /// Get disabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetDisabledForeignKeySql(IDbConnection connection)
        {
            string disabledSql = @"SELECT 'alter table ' || table_name || ' disable constraint ' || constraint_name || ';' tt FROM user_constraints WHERE constraint_type = 'R'";
            object resulte = connection.ExecuteScalar(disabledSql);
            return resulte.ToString();
        }

        #endregion

        #region [2、Get enabled foreign key sql]

        /// <summary>
        /// Get enabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetEnabledForeignKeySql(IDbConnection connection)
        {
            string disabledSql = @"SELECT 'alter table ' || table_name || ' enable constraint ' || constraint_name || ';' tt FROM user_constraints WHERE constraint_type = 'R'";
            object resulte = connection.ExecuteScalar(disabledSql);
            return resulte.ToString();
        }

        #endregion

        #region [3、Get delete foreign key sql]

        /// <summary>
        /// Get delete foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetDeleteForeignKeySql(IDbConnection connection)
        {
            string disabledSql = @"SELECT 'alter table ' || table_name || ' drop constraint ' || constraint_name || ';' tt FROM user_constraints WHERE constraint_type = 'R'";
            object resulte = connection.ExecuteScalar(disabledSql);
            return resulte.ToString();
        }
        #endregion

        #region [4、Get recreat foreign key sql]

        /// <summary>
        /// Get recreat foreign key sql
        /// </summary>
        /// <returns></returns>
        public string GetReCreatForeignKeySql(IDbConnection connection)
        {
            var disableSql = @"SELECT 
	'ALTER TABLE ' || CHILD.TABLE_NAME || ' ' || 'ADD CONSTRAINT ' || CHILD.CONSTRAINT_NAME || ' ' || 'FOREIGN KEY ' || '(' || cp.COLUMN_NAME || ')' || ' ' || 'REFERENCES ' || PARENT.TABLE_NAME || ' ' || '(' || pc.COLUMN_NAME || ') ' || CHILD.DELETE_RULE || ' ;' tt 
FROM 
	USER_CONSTRAINTS CHILD,
	USER_CONSTRAINTS PARENT,
	USER_CONS_COLUMNS cp,
	USER_CONS_COLUMNS pc
WHERE 
	CHILD .CONSTRAINT_TYPE = 'R' 
AND CHILD .R_OWNER = PARENT.OWNER 
AND CHILD .R_CONSTRAINT_NAME = PARENT.CONSTRAINT_NAME 
AND CHILD .CONSTRAINT_NAME = cp.CONSTRAINT_NAME 
AND PARENT .CONSTRAINT_NAME = pc.CONSTRAINT_NAME 
AND cp.POSITION = pc.POSITION 
ORDER BY 
	CHILD.OWNER, 
	CHILD.TABLE_NAME, 
	CHILD.CONSTRAINT_NAME, 
	cp.POSITION";
            object resulte = connection.ExecuteScalar(disableSql);
            return resulte.ToString();
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
        // ReSharper disable once UnusedParameter.Local
        private string GetDeleteOrDropDataTableSqlByName(IDbConnection connection, List<string> tableNameList, string dataBase, bool isView, int type)
        {
            if (tableNameList == null || tableNameList.Count < 1) return string.Empty;
            var hadDeleteTable = new Dictionary<string, int>();
            var historyDictionary = new Dictionary<string, int>();
            StringBuilder resulteBuilder = new StringBuilder(" begin declare num number;begin ");

            var typeSql = type == 1 ? " DELETE FROM " : (type == 0 ? " DROP TABLE " : string.Empty);

            foreach (var tableName in tableNameList.Distinct().Where(tableName => !string.IsNullOrEmpty(tableName)))
            {
                if (historyDictionary.ContainsKey(tableName.ToUpper())) continue;
                var referencedTableList = GetDeleteTableNameList(connection, tableName.ToUpper(), historyDictionary);
                if (referencedTableList == null || referencedTableList.Count < 1) continue;
                foreach (var tempString in referencedTableList)
                {
                    if (hadDeleteTable.ContainsKey(tempString))
                    {
                        hadDeleteTable[tempString]++;
                    }
                    else
                    {
                        string itemString = @"select count(1) INTO num from user_tables where table_name = upper('{0}') ;
    if num > 0 then
        execute immediate '{1}{0}' ;
    end if;";
                        hadDeleteTable.Add(tempString, 1);
                        resulteBuilder.AppendFormat(itemString, tempString, typeSql);
                    }
                }
            }

            resulteBuilder.Append(" end;end; ");
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
            string sqlCmd = @"SELECT
	CHILD.TABLE_NAME TableName,
	CHILD.CONSTRAINT_NAME ForeignKey,
	cp.COLUMN_NAME ForeignKeyCell,
	PARENT.TABLE_NAME ReferencedTableName,
	pc.COLUMN_NAME ReferencedCell 
FROM 
	USER_CONSTRAINTS CHILD,
	USER_CONSTRAINTS PARENT,
	USER_CONS_COLUMNS cp,
	USER_CONS_COLUMNS pc 
WHERE 
	CHILD.CONSTRAINT_TYPE = 'R' 
AND CHILD.R_OWNER = PARENT.OWNER 
AND CHILD.R_CONSTRAINT_NAME = PARENT.CONSTRAINT_NAME 
AND CHILD.CONSTRAINT_NAME = cp.CONSTRAINT_NAME 
AND PARENT.CONSTRAINT_NAME = pc.CONSTRAINT_NAME 
AND cp.POSITION = pc.POSITION 
ORDER BY 
	CHILD.OWNER, 
	CHILD.TABLE_NAME, 
	CHILD.CONSTRAINT_NAME, 
	cp.POSITION";
            var resulteInfo = new List<ReferencedModel>();
            var dr = connection.ExecuteReader(sqlCmd);
            while (dr.Read())
            {
                var tempModel = new ReferencedModel
                {
                    ForeignKey = ((string)dr["ForeignKey"]).ToUpper(),
                    ForeignKeyCell = ((string)dr["ForeignKeyCell"]).ToUpper(),
                    ReferencedCell = ((string)dr["ReferencedCell"]).ToUpper(),
                    ReferencedTableName = ((string)dr["ReferencedTableName"]).ToUpper(),
                    TableName = ((string)dr["TableName"]).ToUpper()
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
            if (!referencedMap.ContainsKey(tableName.ToUpper()))
            {
                historyDictionary.Add(tableName.ToUpper(), 1);
                return new List<string> { tableName.ToUpper() };
            }
            var resultList = TraversingGraph(referencedMap, tableName.ToUpper(), historyDictionary);
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
            return GetDeleteOrDropDataTableSqlByName(connection, new List<string> { tableName.ToUpper() }, dataBase, isView, 0);
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
            return GetDeleteOrDropDataTableSqlByName(connection, new List<string> { tableName.ToUpper() }, dataBase, isView, 1);
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
    }
}
