using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using FreshCommonUtilityNet.Dapper;
using Microsoft.CSharp.RuntimeBinder;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dapper
{
    /// <summary>
    /// Main class for Dapper.SimpleCRUD extensions
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static partial class SimpleCRUD
    {
        #region [1 Cons tract function]
        /// <summary>
        /// Stuck function,default sql type is LocalDB
        /// </summary>
        static SimpleCRUD()
        {
            SetDialect(_dialect);
        }
        #endregion

        #region [2 Partially variable]
        /// <summary>
        /// Default use SQLServer
        /// </summary>
        private static Dialect _dialect = Dialect.SQLServer;

        /// <summary>
        /// Datatable prefix
        /// </summary>
        private static string _encapsulation;

        /// <summary>
        /// Get insert last index
        /// </summary>
        private static string _getIdentitySql;

        /// <summary>
        /// Select page data sql
        /// </summary>
        private static string _getPagedListSql;

        /// <summary>
        /// Tablename cache
        /// </summary>
        private static readonly IDictionary<Type, Tuple<string, Dialect>> TableNames = new Dictionary<Type, Tuple<string, Dialect>>();

        /// <summary>
        /// columnname cache
        /// </summary>
        private static readonly IDictionary<string, Tuple<string, Dialect>> ColumnNames = new Dictionary<string, Tuple<string, Dialect>>();

        /// <summary>
        /// table name resolver
        /// </summary>
        private static ITableNameResolver _tableNameResolver = new TableNameResolver();

        /// <summary>
        /// column name resolver
        /// </summary>
        private static IColumnNameResolver _columnNameResolver = new ColumnNameResolver();

        /// <summary>
        /// deal more other part
        /// </summary>
        private static IDealMoreOtherPart _dealMoreOtherPart;
        #endregion

        #region [3 GetDialect]
        /// <summary>
        /// Returns the current dialect name
        /// </summary>
        /// <returns></returns>
        public static Dialect GetDialect()
        {
            return _dialect;
        }
        #endregion

        #region [4 GetDialectString]
        /// <summary>
        /// Returns the current dialect name
        /// </summary>
        /// <returns></returns>
        public static string GetDialectString()
        {
            return _dialect.ToString();
        }
        #endregion

        #region [5 Set Dialect,change database type]
        /// <summary>
        /// Sets the database dialect 
        /// </summary>
        /// <param name="dialect"></param>
        public static void SetDialect(Dialect dialect)
        {
            switch (dialect)
            {
                //case Dialect.PostgreSQL:
                //    _dialect = Dialect.PostgreSQL;
                //    _encapsulation = "\"{0}\"";
                //    _getIdentitySql = "SELECT LASTVAL() AS id";
                //    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                //_dealMoreOtherPart = null;
                //    break;
                case Dialect.SQLite:
                    _dialect = Dialect.SQLite;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = "SELECT LAST_INSERT_ROWID() AS id";
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({PageNumber}-1) * {RowsPerPage})";
                    _dealMoreOtherPart = new SqlitePart();
                    break;
                case Dialect.MySQL:
                    _dialect = Dialect.MySQL;
                    _encapsulation = "`{0}`";
                    _getIdentitySql = "SELECT LAST_INSERT_ID() AS id";
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}";
                    _dealMoreOtherPart = new MySqlPart();
                    break;
                default:
                    _dialect = Dialect.SQLServer;
                    _encapsulation = "[{0}]";
                    _getIdentitySql = "SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]";
                    _getPagedListSql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNUMBER BETWEEN (({PageNumber}-1) * {RowsPerPage} + 1) AND ({PageNumber} * {RowsPerPage})";
                    _dealMoreOtherPart = new SqlServerPart();
                    break;
            }
        }
        #endregion

        #region [6 Set youself Tablename resolver]
        /// <summary>
        /// Sets the table name resolver
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a table name</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }
        #endregion

        #region [7 Set youself Column resolver]
        /// <summary>
        /// Sets the column name resolver
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a column name</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            _columnNameResolver = resolver;
        }
        #endregion

        #region [8 Get Entity]
        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>By default filters on the Id column</para>
        /// <para>-Id column name can be overridden by adding an attribute on your primary key property [Key]</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a single entity by a single id from table T</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a single entity by a single id from table T.</returns>
        public static T Get<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Get<T> only supports an entity with a [Key] or Id property");

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0} where ", name);

            for (var i = 0; i < idProps.Count; i++)
            {
                if (i > 0)
                    sb.Append(" and ");
                sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
            }

            var dynParms = new DynamicParameters();
            if (idProps.Count == 1)
                dynParms.Add("@" + idProps.First().Name, id);
            else
            {
                foreach (var prop in idProps)
                    // ReSharper disable once PossibleNullReferenceException
                    dynParms.Add("@" + prop.Name, id.GetType().GetProperty(prop.Name).GetValue(id, null));
            }

            if (Debugger.IsAttached)
                Trace.WriteLine($"Get<{currenttype}>: {sb} with Id: {id}");
            return connection.Query<T>(sb.ToString(), dynParms, transaction, true, commandTimeout).FirstOrDefault();
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();
            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0}", name);

            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)), whereConditions);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(string.Format("GetList<{0}>: {1}", currenttype, sb));

            return connection.Query<T>(sb.ToString(), whereConditions, transaction, true, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>conditions is an SQL where clause and/or order by clause ex: "where name='bob'" or "where age>=@Age"</para>
        /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a list of entities with optional SQL where conditions</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();
            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0}", name);

            sb.Append(" " + conditions);

            if (Debugger.IsAttached)
                Trace.WriteLine(string.Format("GetList<{0}>: {1}", currenttype, sb));

            return connection.Query<T>(sb.ToString(), parameters, transaction, true, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Returns a list of all entities</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <returns>Gets a list of all entities</returns>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection)
        {
            return connection.GetList<T>(new { });
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age" - not required </para>
        /// <para>orderby is a column or list of columns to order by ex: "lastname, age desc" - not required - default is by primary key</para>
        /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="pageNumber"></param>
        /// <param name="rowsPerPage"></param>
        /// <param name="conditions"></param>
        /// <param name="orderby"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a paged list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetListPaged<T>(this IDbConnection connection, int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(_getPagedListSql))
                throw new Exception("GetListPage is not supported with the current SQL Dialect");

            if (pageNumber < 1)
                throw new Exception("Page must be greater than 0");

            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();
            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            var query = _getPagedListSql;
            if (string.IsNullOrEmpty(orderby))
            {
                orderby = GetColumnName(idProps.First());
            }

            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            query = query.Replace("{SelectColumns}", sb.ToString());
            query = query.Replace("{TableName}", name);
            query = query.Replace("{PageNumber}", pageNumber.ToString());
            query = query.Replace("{RowsPerPage}", rowsPerPage.ToString());
            query = query.Replace("{OrderBy}", orderby);
            query = query.Replace("{WhereClause}", conditions);
            query = query.Replace("{Offset}", ((pageNumber - 1) * rowsPerPage).ToString());

            if (Debugger.IsAttached)
                Trace.WriteLine(string.Format("GetListPaged<{0}>: {1}", currenttype, query));

            return connection.Query<T>(query, parameters, transaction, true, commandTimeout);
        }
        #endregion

        #region [9 Insert data to database]
        /// <summary>
        /// <para>Inserts a row into the database</para>
        /// <para>By default inserts into the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns the ID (primary key) of the newly inserted record if it is identity using the int? type, otherwise null</para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The ID (primary key) of the newly inserted record if it is identity using the int? type, otherwise null</returns>
        public static int? Insert<TEntity>(this IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Insert<int?, TEntity>(connection, entityToInsert, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Inserts a row into the database, using ONLY the properties defined by TEntity</para>
        /// <para>By default inserts into the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns the ID (primary key) of the newly inserted record if it is identity using the defined type, otherwise null</para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The ID (primary key) of the newly inserted record if it is identity using the defined type, otherwise null</returns>
        public static TKey Insert<TKey, TEntity>(this IDbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var idProps = GetIdProperties(entityToInsert).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Insert<T> only supports an entity with a [Key] or Id property");

            var keyHasPredefinedValue = false;
            var baseType = typeof(TKey);
            var underlyingType = Nullable.GetUnderlyingType(baseType);
            var keytype = underlyingType ?? baseType;
            if (keytype != typeof(int) && keytype != typeof(uint) && keytype != typeof(long) && keytype != typeof(ulong) && keytype != typeof(short) && keytype != typeof(ushort) && keytype != typeof(Guid) && keytype != typeof(string))
            {
                throw new Exception("Invalid return type");
            }

            var name = GetTableName(entityToInsert);
            var sb = new StringBuilder();
            sb.AppendFormat("insert into {0}", name);
            sb.Append(" (");
            BuildInsertParameters<TEntity>(sb);
            sb.Append(") ");
            sb.Append("values");
            sb.Append(" (");
            BuildInsertValues<TEntity>(sb);
            sb.Append(")");

            if (keytype == typeof(Guid))
            {
                var guidvalue = (Guid)idProps.First().GetValue(entityToInsert, null);
                if (guidvalue == Guid.Empty)
                {
                    var newguid = SequentialGuid();
                    idProps.First().SetValue(entityToInsert, newguid, null);
                }
                else
                {
                    keyHasPredefinedValue = true;
                }
                sb.Append(";select '" + idProps.First().GetValue(entityToInsert, null) + "' as id");
            }

            if ((keytype == typeof(int) || keytype == typeof(long)) && Convert.ToInt64(idProps.First().GetValue(entityToInsert, null)) == 0)
            {
                sb.Append(";" + _getIdentitySql);
            }
            else
            {
                keyHasPredefinedValue = true;
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(string.Format("Insert: {0}", sb));

            var r = connection.Query(sb.ToString(), entityToInsert, transaction, true, commandTimeout);

            if (keytype == typeof(Guid) || keyHasPredefinedValue)
            {
                return (TKey)idProps.First().GetValue(entityToInsert, null);
            }
            return (TKey)r.First().id;
        }
        #endregion

        #region [10 Update entity info]
        /// <summary>
        /// <para>Updates a record or records in the database with only the properties of TEntity</para>
        /// <para>By default updates records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Updates records where the Id property and properties with the [Key] attribute match those in the database.</para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns number of rows effected</para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of effected records</returns>
        public static int Update<TEntity>(this IDbConnection connection, TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var idProps = GetIdProperties(entityToUpdate).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(entityToUpdate);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0}", name);

            sb.AppendFormat(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            sb.Append(" where ");
            BuildWhere(sb, idProps, entityToUpdate);

            if (Debugger.IsAttached)
                Trace.WriteLine($"Update: {sb}");

            return connection.Execute(sb.ToString(), entityToUpdate, transaction, commandTimeout);
        }
        #endregion

        #region [11 Delete Data from database]
        /// <summary>
        /// <para>Deletes a record or records in the database that match the object passed in</para>
        /// <para>-By default deletes records in the table matching the class name</para>
        /// <para>Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns the number of records effected</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToDelete"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records effected</returns>
        public static int Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var idProps = GetIdProperties(entityToDelete).ToList();


            if (!idProps.Any())
                throw new ArgumentException("Entity must have at least one [Key] or Id property");

            var name = GetTableName(entityToDelete);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0}", name);

            sb.Append(" where ");
            BuildWhere(sb, idProps, entityToDelete);

            if (Debugger.IsAttached)
                Trace.WriteLine(string.Format("Delete: {0}", sb));

            return connection.Execute(sb.ToString(), entityToDelete, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Deletes a record or records in the database by ID</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Deletes records where the Id property and properties with the [Key] attribute match those in the database</para>
        /// <para>The number of records effected</para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records effected</returns>
        public static int Delete<T>(this IDbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();


            if (!idProps.Any())
                throw new ArgumentException("Delete<T> only supports an entity with a [Key] or Id property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0} where ", name);

            for (var i = 0; i < idProps.Count; i++)
            {
                if (i > 0)
                    sb.Append(" and ");
                sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
            }

            var dynParms = new DynamicParameters();
            if (idProps.Count == 1)
                dynParms.Add("@" + idProps.First().Name, id);
            else
            {
                foreach (var prop in idProps)
                    // ReSharper disable once PossibleNullReferenceException
                    dynParms.Add("@" + prop.Name, id.GetType().GetProperty(prop.Name).GetValue(id, null));
            }

            if (Debugger.IsAttached)
                Trace.WriteLine($"Delete<{currenttype}> {sb}");

            return connection.Execute(sb.ToString(), dynParms, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Deletes a list of records in the database</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Deletes records where that match the where clause</para>
        /// <para>whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}</para>
        /// <para>The number of records effected</para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records effected</returns>
        public static int DeleteList<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {

            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.AppendFormat("Delete from {0}", name);
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)));
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(string.Format("DeleteList<{0}> {1}", currenttype, sb));

            return connection.Execute(sb.ToString(), whereConditions, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Deletes a list of records in the database</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Deletes records where that match the where clause</para>
        /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age"</para>
        /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records effected</returns>
        public static int DeleteList<T>(this IDbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            if (string.IsNullOrEmpty(conditions))
                throw new ArgumentException("DeleteList<T> requires a where clause");
            if (!conditions.ToLower().Contains("where"))
                throw new ArgumentException("DeleteList<T> requires a where clause and must contain the WHERE keyword");

            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0}", name);
            sb.Append(" " + conditions);

            if (Debugger.IsAttached)
                Trace.WriteLine($"DeleteList<{currenttype}> {sb}");

            return connection.Execute(sb.ToString(), parameters, transaction, commandTimeout);
        }
        #endregion

        #region [12 Get command count]
        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Returns a number of records entity by a single id from table T</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age>=@Age" - not required </para>
        /// <para>parameters is an anonymous type to pass in named parameter values: new { Age = 15 }</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount<T>(this IDbConnection connection, string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select count(1)");
            sb.AppendFormat(" from {0}", name);
            sb.Append(" " + conditions);

            if (Debugger.IsAttached)
                Trace.WriteLine($"RecordCount<{currenttype}>: {sb}");

            return connection.ExecuteScalar<int>(sb.ToString(), parameters, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Returns a number of records entity by a single id from table T</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount<T>(this IDbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select count(1)");
            sb.AppendFormat(" from {0}", name);
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere(sb, whereprops, (T)Activator.CreateInstance(typeof(T)));
            }

            if (Debugger.IsAttached)
                Trace.WriteLine($"RecordCount<{currenttype}>: {sb}");

            return connection.ExecuteScalar<int>(sb.ToString(), whereConditions, transaction, commandTimeout);
        }
        #endregion

        #region [13 Get disabled foreign key sql]

        /// <summary>
        /// Get disabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetDisabledForeignKeySql(this IDbConnection connection)
        {
            return _dealMoreOtherPart.GetDisabledForeignKeySql(connection);
        }

        #endregion

        #region [14 Get enabled foreign key sql]

        /// <summary>
        /// Get enabled foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetEnabledForeignKeySql(this IDbConnection connection)
        {
            return _dealMoreOtherPart.GetEnabledForeignKeySql(connection);
        }

        #endregion

        #region [15 Get delete foreign key sql]

        /// <summary>
        /// Get delete foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetDeleteForeignKeySql(this IDbConnection connection)
        {
            return _dealMoreOtherPart.GetDeleteForeignKeySql(connection);
        }
        #endregion

        #region [16 Get recreat foreign key sql]

        /// <summary>
        /// Get recreat foreign key sql
        /// </summary>
        /// <returns></returns>
        public static string GetReCreatForeignKeySql(this IDbConnection connection)
        {
            return _dealMoreOtherPart.GetReCreatForeignKeySql(connection);
        }
        #endregion

        #region [17 Delete Table or drop table SQL Code]

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
        public static string GetDropDataTableSqlByName(this IDbConnection connection, string tableName, string dataBase = null, bool isView = false)
        {
            return _dealMoreOtherPart.GetDropDataTableSqlByName(connection, tableName, dataBase, isView);
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
        public static string GetDropDataTableSqlByName(this IDbConnection connection, List<string> tableNameList, string dataBase = null,
            bool isView = false)
        {
            return _dealMoreOtherPart.GetDropDataTableSqlByName(connection, tableNameList, dataBase, isView);
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
        public static string GetDeleteDataTableSqlByName(this IDbConnection connection, string tableName, string dataBase = null, bool isView = false)
        {
            return _dealMoreOtherPart.GetDeleteDataTableSqlByName(connection, tableName, dataBase, isView);
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
        public static string GetDeleteDataTableSqlByName(this IDbConnection connection, List<string> tableNameList, string dataBase = null,
            bool isView = false)
        {
            return _dealMoreOtherPart.GetDeleteDataTableSqlByName(connection, tableNameList, dataBase, isView);
        }

        #endregion

        #region [18 BuildUpdateSet]
        /// <summary>
        /// build update statement based on list on an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToUpdate"></param>
        /// <param name="sb"></param>
        private static void BuildUpdateSet<T>(T entityToUpdate, StringBuilder sb)
        {
            var nonIdProps = GetUpdateableProperties(entityToUpdate).ToArray();

            for (var i = 0; i < nonIdProps.Length; i++)
            {
                var property = nonIdProps[i];

                sb.AppendFormat("{0} = @{1}", GetColumnName(property), property.Name);
                if (i < nonIdProps.Length - 1)
                    sb.AppendFormat(", ");
            }
        }
        #endregion

        #region [19 BuildSelect]
        /// <summary>
        /// build select clause based on list of properties skipping ones with the IgnoreSelect and NotMapped attribute
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="props"></param>
        private static void BuildSelect(StringBuilder sb, IEnumerable<PropertyInfo> props)
        {
            var propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
            var addedAny = false;
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                if (propertyInfos.ElementAt(i).GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreSelectAttribute).Name || attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                if (addedAny)
                    sb.Append(",");
                sb.Append(GetColumnName(propertyInfos.ElementAt(i)));
                //if there is a custom column name add an "as customcolumnname" to the item so it maps properly
                if (propertyInfos.ElementAt(i).GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) != null)
                    sb.Append(" as " + Encapsulate(propertyInfos.ElementAt(i).Name));
                addedAny = true;

            }
        }
        #endregion

        #region [20 BuildWhere]
        /// <summary>
        /// match up generic properties to source entity properties to allow fetching of the column attribute
        /// the anonymous object used for search doesn't have the custom attributes attached to them so this allows us to build the correct where clause
        /// by converting the model type to the database column name via the column attribute
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sb"></param>
        /// <param name="idProps"></param>
        /// <param name="sourceEntity"></param>
        /// <param name="whereConditions"></param>
        // ReSharper disable once UnusedParameter.Local
        private static void BuildWhere<TEntity>(StringBuilder sb, IEnumerable<PropertyInfo> idProps, TEntity sourceEntity, object whereConditions = null)
        {
            var propertyInfos = idProps.ToArray();
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var useIsNull = false;

                var propertyToUse = propertyInfos.ElementAt(i);
                var sourceProperties = GetScaffoldableProperties<TEntity>().ToArray();
                for (var x = 0; x < sourceProperties.Count(); x++)
                {
                    if (sourceProperties.ElementAt(x).Name == propertyInfos.ElementAt(i).Name)
                    {
                        propertyToUse = sourceProperties.ElementAt(x);

                        if (whereConditions != null && propertyInfos.ElementAt(i).CanRead && (propertyInfos.ElementAt(i).GetValue(whereConditions, null) == null || propertyInfos.ElementAt(i).GetValue(whereConditions, null) == DBNull.Value))
                        {
                            useIsNull = true;
                        }
                        break;
                    }
                }
                sb.AppendFormat(
                    useIsNull ? "{0} is null" : "{0} = @{1}",
                    GetColumnName(propertyToUse),
                    propertyInfos.ElementAt(i).Name);

                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }
        #endregion

        #region [21 Build Insert Values and Parameters]
        /// <summary>
        /// build insert values which include all properties in the class that are:
        /// Not named Id
        /// Not marked with the Editable(false) attribute
        /// Not marked with the[Key] attribute(without required attribute)
        /// Not marked with[IgnoreInsert]
        /// Not marked with[NotMapped]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        private static void BuildInsertValues<T>(StringBuilder sb)
        {
            var props = GetScaffoldableProperties<T>().ToArray();
            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.PropertyType != typeof(Guid)
                      && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                      && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                    continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreInsertAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;

                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                sb.AppendFormat("@{0}", property.Name);
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);

        }

        /// <summary>
        /// build insert parameters which include all properties in the class that are not:
        /// marked with the Editable(false) attribute
        /// marked with the[Key] attribute
        /// marked with[IgnoreInsert]
        /// named Id
        /// marked with[NotMapped]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sb"></param>
        private static void BuildInsertParameters<T>(StringBuilder sb)
        {
            var props = GetScaffoldableProperties<T>().ToArray();

            for (var i = 0; i < props.Count(); i++)
            {
                var property = props.ElementAt(i);
                if (property.PropertyType != typeof(Guid)
                      && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                      && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                    continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreInsertAttribute).Name)) continue;
                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;
                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                sb.Append(GetColumnName(property));
                if (i < props.Count() - 1)
                    sb.Append(", ");
            }
            if (sb.ToString().EndsWith(", "))
                sb.Remove(sb.Length - 2, 2);
        }
        #endregion

        #region [22 Deal properties]
        /// <summary>
        /// Get all properties in an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetAllProperties<T>(T entity) where T : class
        {
            if (entity == null) return new PropertyInfo[0];
            return entity.GetType().GetProperties();
        }

        /// <summary>
        /// Get all properties that are not decorated with the Editable(false) attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static IEnumerable<PropertyInfo> GetScaffoldableProperties<T>()
        {
            IEnumerable<PropertyInfo> props = typeof(T).GetProperties();

            props = props.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(EditableAttribute).Name && !IsEditable(p)) == false);
            return props.Where(p => p.PropertyType.IsSimpleType() || IsEditable(p));
        }

        /// <summary>
        /// <para>Determine if the Attribute has an AllowEdit key and return its boolean state</para>
        /// <para>fake the funk and try to mimick EditableAttribute in System.ComponentModel.DataAnnotations</para>
        /// <para>This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference</para>
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static bool IsEditable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(EditableAttribute).Name);
                if (write != null)
                {
                    return write.AllowEdit;
                }
            }
            return false;
        }

        /// <summary>
        /// <para>        Determine if the Attribute has an IsReadOnly key and return its boolean state fake the funk and try to mimick ReadOnlyAttribute in System.ComponentModel This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference</para>
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static bool IsReadOnly(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(ReadOnlyAttribute).Name);
                if (write != null)
                {
                    return write.IsReadOnly;
                }
            }
            return false;
        }

        /// <summary>
        /// Get all properties that are:
        /// Not named Id
        /// Not marked with the Key attribute
        /// Not marked ReadOnly
        /// Not marked IgnoreInsert
        /// Not marked NotMapped
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Local
        private static IEnumerable<PropertyInfo> GetUpdateableProperties<T>(T entity)
        {
            var updateableProperties = GetScaffoldableProperties<T>();
            //remove ones with ID
            updateableProperties = updateableProperties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            //remove ones with key attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name) == false);
            //remove ones that are readonly
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => (attr.GetType().Name == typeof(ReadOnlyAttribute).Name) && IsReadOnly(p)) == false);
            //remove ones with IgnoreUpdate attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreUpdateAttribute).Name) == false);
            //remove ones that are not mapped
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name) == false);

            return updateableProperties;
        }

        //Get all properties that are named Id or have the Key attribute
        //For Inserts and updates we have a whole entity so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties(object entity)
        {
            var type = entity.GetType();
            return GetIdProperties(type);
        }

        //Get all properties that are named Id or have the Key attribute
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties(Type type)
        {
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        }

        //Gets the table name for this entity
        //For Inserts and updates we have a whole entity so this method is used
        //Uses class name by default and overrides if the class has a Table attribute
        private static string GetTableName(object entity)
        {
            var type = entity.GetType();
            return GetTableName(type);
        }

        //Gets the table name for this type
        //For Get(id) and Delete(id) we don't have an entity, just the type so this method is used
        //Use dynamic type to be able to handle both our Table-attribute and the DataAnnotation
        //Uses class name by default and overrides if the class has a Table attribute
        private static string GetTableName(Type type)
        {
            Tuple<string, Dialect> tempTableName;
            if (TableNames.TryGetValue(type, out tempTableName))
            {
                if (tempTableName.Item2 == _dialect) return tempTableName.Item1;
            }
            var tableName = _tableNameResolver.ResolveTableName(type);
            TableNames[type] = new Tuple<string, Dialect>(tableName, _dialect);

            return tableName;
        }

        private static string GetColumnName(PropertyInfo propertyInfo)
        {
            string key = string.Format("{0}.{1}", propertyInfo.DeclaringType, propertyInfo.Name);
            Tuple<string, Dialect> tempColumn;
            if (ColumnNames.TryGetValue(key, out tempColumn))
            {
                if (tempColumn.Item2 == _dialect) return tempColumn.Item1;
            }
            var columnName = _columnNameResolver.ResolveColumnName(propertyInfo);
            ColumnNames[key] = new Tuple<string, Dialect>(columnName, _dialect);

            return columnName;
        }

        private static string Encapsulate(string databaseword)
        {
            return string.Format(_encapsulation, databaseword);
        }

        /// <summary>
        /// Generates a guid based on the current date/time
        /// http://stackoverflow.com/questions/1752004/sequential-guid-generator-c-sharp
        /// </summary>
        /// <returns></returns>
        public static Guid SequentialGuid()
        {
            var tempGuid = Guid.NewGuid();
            var bytes = tempGuid.ToByteArray();
            var time = DateTime.Now;
            bytes[3] = (byte)time.Year;
            bytes[2] = (byte)time.Month;
            bytes[1] = (byte)time.Day;
            bytes[0] = (byte)time.Hour;
            bytes[5] = (byte)time.Minute;
            bytes[4] = (byte)time.Second;
            return new Guid(bytes);
        }
        #endregion

        #region [23 Database enum]
        /// <summary>
        /// Database server dialects
        /// </summary>
        public enum Dialect
        {
            /// <summary>
            /// SQL Server
            /// </summary>
            // ReSharper disable once InconsistentNaming
            SQLServer = 0,

            ///// <summary>
            ///// PostgreSQL
            ///// </summary>
            //////PostgreSQL = 1,

            /// <summary>
            /// MySQL
            /// </summary>
            // ReSharper disable once InconsistentNaming
            MySQL = 2,

            /// <summary>
            /// SQLite
            /// </summary>
            // ReSharper disable once InconsistentNaming
            SQLite = 3,
        }
        #endregion

        #region [24 Default table name resolver]
        /// <summary>
        /// table name resolver
        /// </summary>
        public class TableNameResolver : ITableNameResolver
        {
            /// <summary>
            /// table name resolver
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public virtual string ResolveTableName(Type type)
            {
                var tableName = Encapsulate(type.Name);

                var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as dynamic;
                if (tableattr != null)
                {
                    tableName = Encapsulate(tableattr.Name);
                    try
                    {
                        if (!String.IsNullOrEmpty(tableattr.Schema))
                        {
                            string schemaName = Encapsulate(tableattr.Schema);
                            tableName = String.Format("{0}.{1}", schemaName, tableName);
                        }
                    }
                    catch (RuntimeBinderException)
                    {
                        //Schema doesn't exist on this attribute.
                    }
                }

                return tableName;
            }
        }
        #endregion

        #region [25 Default column name resolver]
        /// <summary>
        /// column name resolver
        /// </summary>
        public class ColumnNameResolver : IColumnNameResolver
        {
            /// <summary>
            /// resolve column name
            /// </summary>
            /// <param name="propertyInfo"></param>
            /// <returns></returns>
            public virtual string ResolveColumnName(PropertyInfo propertyInfo)
            {
                var columnName = Encapsulate(propertyInfo.Name);

                var columnattr = propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) as dynamic;
                if (columnattr != null)
                {
                    columnName = Encapsulate(columnattr.Name);
                    if (Debugger.IsAttached)
                        Trace.WriteLine(String.Format("Column name for type overridden from {0} to {1}", propertyInfo.Name, columnName));
                }
                return columnName;
            }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class DapperExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="firstKey"></param>
        /// <param name="secondKey"></param>
        /// <param name="addChildren"></param>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public static IEnumerable<TFirst> Map<TFirst, TSecond, TKey>(this SqlMapper.GridReader reader,
            Func<TFirst, TKey> firstKey,
            Func<TSecond, TKey> secondKey,
            Action<TFirst, IEnumerable<TSecond>> addChildren)
        {
            var first = reader.Read<TFirst>().ToList();
            var childMap = reader.Read<TSecond>()
                .GroupBy(s => secondKey(s))
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
            foreach (var item in first)
            {
                IEnumerable<TSecond> children;
                if (childMap.TryGetValue(firstKey(item), out children))
                {
                    addChildren(item, children);
                }
            }
            return first;
        }
    }
}
