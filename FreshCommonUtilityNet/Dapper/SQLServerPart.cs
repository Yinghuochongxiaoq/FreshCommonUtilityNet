using System.Data;
using Dapper;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dapper
{
    /// <summary>
    /// SQLServer part
    /// </summary>
    public static class SqlServerPart
    {
        #region [1、Get disabled foreign key sql]

        /// <summary>
        /// Get disabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetDisabledForeignKeySql(this IDbConnection connection)
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
        public static string GetEnabledForeignKeySql(this IDbConnection connection)
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
        public static string GetDeleteForeignKeySql(this IDbConnection connection)
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
        public static string GetReCreatForeignKeySql(this IDbConnection connection)
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
    }
}
