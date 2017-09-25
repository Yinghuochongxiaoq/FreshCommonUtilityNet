using System.Data;
using System.Linq;
using Dapper;
using FreshCommonUtility.CommonModel;

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
    }
}
