#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.Dapper
//文件名称：IDealMoreOtherPart
//创 建 人：FreshMan
//创建日期：2017/10/10 22:47:13
//用    途：记录类的用途
//======================================================================
#endregion

using System.Collections.Generic;
using System.Data;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dapper
{
    /// <summary>
    /// deal more other self way part interface
    /// </summary>
    public interface IDealMoreOtherPart
    {
        #region [1、Get disabled foreign key sql]

        /// <summary>
        /// Get disabled foreign key sql
        /// </summary>
        /// <returns></returns>
        string GetDisabledForeignKeySql(IDbConnection connection);

        #endregion

        #region [2、Get enabled foreign key sql]

        /// <summary>
        /// Get enabled foreign key sql
        /// </summary>
        /// <returns></returns>
        string GetEnabledForeignKeySql(IDbConnection connection);

        #endregion

        #region [3、Get delete foreign key sql]

        /// <summary>
        /// Get delete foreign key sql
        /// </summary>
        /// <returns></returns>
        string GetDeleteForeignKeySql(IDbConnection connection);
        #endregion

        #region [4、Get recreat foreign key sql]

        /// <summary>
        /// Get recreat foreign key sql
        /// </summary>
        /// <returns></returns>
        string GetReCreatForeignKeySql(IDbConnection connection);
        #endregion

        #region [5、Delete Table or drop table SQL Code]

        /// <summary>
        /// Drop table or view by name
        /// </summary>
        /// <param name="tableName">table or view name</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>drop table or view sql command</returns>
        string GetDropDataTableSqlByName(string tableName, string dataBase = null, bool isView = false);

        /// <summary>
        /// Drop table or view by name
        /// </summary>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>drop table or view sql command</returns>
        string GetDropDataTableSqlByName(List<string> tableNameList, string dataBase = null, bool isView = false);

        /// <summary>
        /// Delete table or view by name
        /// </summary>
        /// <param name="tableName">table or view name</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-5</creattime>
        /// <returns>delete table or view sql command</returns>
        string GetDeleteDataTableSqlByName(string tableName, string dataBase = null, bool isView = false);

        /// <summary>
        /// Delete table or view by name
        /// </summary>
        /// <param name="tableNameList">table or view name set</param>
        /// <param name="dataBase">Database name,default value is current link database</param>
        /// <param name="isView">is view? true | false(default value)</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-09-06</creattime>
        /// <returns>delete table or view sql command</returns>
        string GetDeleteDataTableSqlByName(List<string> tableNameList, string dataBase = null, bool isView = false);

        #endregion
    }
}
