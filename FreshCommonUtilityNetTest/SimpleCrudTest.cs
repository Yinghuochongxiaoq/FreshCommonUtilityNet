using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FreshCommonUtility.Dapper;
using FreshCommonUtility.SqlHelper;
using FreshCommonUtilityNetTest;

namespace FreshCommonUtilityTests
{
    /// <summary>
    /// SimpleCrud test class
    /// </summary>
    public class SimpleCrudTest
    {
        /// <summary>
        /// Db username
        /// </summary>
        private readonly string _userName;

        /// <summary>
        /// Db password
        /// </summary>
        private readonly string _password;

        /// <summary>
        /// Db host name
        /// </summary>
        private readonly string _hostName;

        /// <summary>
        /// Db port
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// Db name
        /// </summary>
        private readonly string _dbName;

        /// <summary>
        /// construction function
        /// </summary>
        /// <param name="dbtype"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="dbName"></param>
        public SimpleCrudTest(SimpleCRUD.Dialect dbtype, string userName = "FreshMan", string password = "qinxianbo", string hostname = "localhost", int port = 3306, string dbName = "testdb")
        {
            _userName = userName;
            _password = password;
            _hostName = hostname;
            _port = port;
            _dbName = dbName;
            SimpleCRUD.SetDialect(dbtype);
        }

        /// <summary>
        /// Get open connection object.
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetOpenConnection()
        {
            IDbConnection connection;
            var postgreConnectionString = $"Server={_hostName};Port={_port};User Id={_userName};Password={_password};Database={_dbName};";
            var mysqlConnectionString = $"Server={_hostName};Port={_port};User Id={_userName};Password={_password};Database={_dbName};SslMode=None";
            var sqlserverConnectionString = @"Data Source = (localdb)\MSSQLLocalDB;Initial Catalog=DapperSimpleCrudTestDb;Integrated Security=True;MultipleActiveResultSets=true;";
            var sqliteConnectionString = @"Data Source=MyDatabase.sqlite;Version=3;";
            var _dbtype = SimpleCRUD.GetDialectString();
            var dictionary = new Dictionary<string, string>()
            {
                { SimpleCRUD.Dialect.MySQL.ToString(), mysqlConnectionString },
                //{ SimpleCRUD.Dialect.PostgreSQL.ToString(),postgreConnectionString},
                { SimpleCRUD.Dialect.SQLServer.ToString(),sqlserverConnectionString},
                { SimpleCRUD.Dialect.SQLite.ToString(),sqliteConnectionString }
            };
            var str = dictionary[_dbtype];
            connection = SqlConnectionHelper.GetOpenConnection(str);
            return connection;
        }

        /// <summary>
        /// test insert with Specified tablename
        /// </summary>
        public void TestInsertWithSpecifiedTableName()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestInsertWithSpecifiedTableName", Age = 10 });
                var user = connection.Get<User>(id);
                user.Name.IsEqualTo("TestInsertWithSpecifiedTableName");
                connection.Delete<User>(id);
            }
        }

        /// <summary>
        /// Test inseret use big int primary key.
        /// </summary>
        public void TestInsertUsingBigIntPrimaryKey()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert<long, BigCar>(new BigCar { Make = "Big", Model = "Car" });
                connection.Delete<BigCar>(id);
            }
        }

        /// <summary>
        /// Test insert use generic limited fields
        /// </summary>
        public void TestInseretUsingGenericLimitedFields()
        {
            using (var connection = GetOpenConnection())
            {
                //arrange
                var user = new User { Name = "User1", Age = 10, ScheduledDayOff = System.DayOfWeek.Thursday };

                //act
                var id = connection.Insert<int?, UserEditableSettings>(user);

                //assert
                var insertedUser = connection.Get<User>(id);
                insertedUser.ScheduledDayOff.IsNull();

                connection.Delete<User>(id);
            }
        }

        /// <summary>
        /// async insert use generic limited fields.
        /// </summary>
        public void TestInsertUsingGenericLimitedFieldsAsync()
        {
            using (var connection = GetOpenConnection())
            {
                //arrange
                var user = new User { Name = "User1", Age = 10, ScheduledDayOff = System.DayOfWeek.Friday };

                //act
                var idTask = connection.InsertAsync<int?, UserEditableSettings>(user);
                idTask.Wait();
                var id = idTask.Result;

                //assert
                var insertedUser = connection.Get<User>(id);
                insertedUser.ScheduledDayOff.IsNull();

                connection.Delete<User>(id);
            }
        }

        /// <summary>
        /// Test get method.
        /// </summary>
        public void TestSimpleGet()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestSimpleGet", Age = 18, ScheduledDayOff = System.DayOfWeek.Saturday, CreatedDate = new System.DateTime(2016, 2, 2) });
                var user = connection.Get<User>(id);
                user.Name.IsEqualTo("TestSimpleGet");
                user.CreatedDate.IsNotEqualTo(new System.DateTime(2016, 2, 2));
                connection.Delete<User>(id);
            }
        }

        /// <summary>
        /// Test delete by id
        /// </summary>
        public void TestDeleteById()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "UserTestDeleteById", Age = 18 });
                connection.Delete<User>(id);
                connection.Get<User>(id).IsNull();
            }
        }

        /// <summary>
        /// Test delete by object
        /// </summary>
        public void TestDeleteByObject()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestDeleteByObject", Age = 18 });
                var user = connection.Get<User>(id);
                connection.Delete(user);
                connection.Get<User>(id).IsNull();
            }
        }

        /// <summary>
        /// Test get simple list.
        /// </summary>
        public void TestSimpleGetList()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestSimpleGetList", Age = 18 });
                connection.Insert(new User { Name = "TestSimpleGetList", Age = 18 });
                var user = connection.GetList<User>(new { });
                user.Count().IsEqualTo(2);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Tset filtered list
        /// </summary>
        public void TestFilteredGetList()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestFilteredGetList1", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetList2", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetList3", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetList4", Age = 11 });

                var user = connection.GetList<User>(new { Age = 10 });
                user.Count().IsEqualTo(3);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test filtered get list with multiple keys.
        /// </summary>
        public void TestFilteredGetListWithMultipleKeysKeyMaster()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new KeyMaster { Key1 = 1, Key2 = 1 });
                connection.Insert(new KeyMaster { Key1 = 1, Key2 = 2 });
                connection.Insert(new KeyMaster { Key1 = 1, Key2 = 3 });
                connection.Insert(new KeyMaster { Key1 = 2, Key2 = 4 });

                var keyMasters = connection.GetList<KeyMaster>(new { Key1 = 1 });
                keyMasters.Count().IsEqualTo(3);
                connection.Execute("Delete from KeyMaster");
            }
        }

        /// <summary>
        /// Test filtered with sql get list
        /// </summary>
        public void TestFilteredWithSqlGetList()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestFilteredWithSQLGetList1", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredWithSQLGetList2", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredWithSQLGetList3", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredWithSQLGetList4", Age = 11 });

                var user = connection.GetList<User>("where Name like 'TestFilteredWithSQLGetList%' and Age = 10");
                user.Count().IsEqualTo(3);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test get list with null where case.
        /// </summary>
        public void TestGetListWithNullWhere()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestGetListWithNullWhere", Age = 10 });
                var user = connection.GetList<User>(null);
                user.Count().IsEqualTo(1);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test get list with out where case
        /// </summary>
        public void TestGetListWithoutWhere()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestGetListWithoutWhere", Age = 10 });
                var user = connection.GetList<User>();
                user.Count().IsEqualTo(1);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test get list with parameters
        /// </summary>
        public void TestsGetListWithParameters()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestsGetListWithParameters1", Age = 10 });
                connection.Insert(new User { Name = "TestsGetListWithParameters2", Age = 10 });
                connection.Insert(new User { Name = "TestsGetListWithParameters3", Age = 10 });
                connection.Insert(new User { Name = "TestsGetListWithParameters4", Age = 11 });

                var user = connection.GetList<User>("where Age > @Age", new { Age = 10 });
                user.Count().IsEqualTo(1);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test get list with readonly property
        /// </summary>
        public void TestGetWithReadonlyProperty()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestGetWithReadonlyProperty", Age = 10 });
                var user = connection.Get<User>(id);
                user.CreatedDate.Year.IsEqualTo(DateTime.Now.Year);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test insert with readonly property
        /// </summary>
        public void TestInsertWithReadonlyProperty()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestInsertWithReadonlyProperty", Age = 10, CreatedDate = new DateTime(2001, 1, 1) });
                var user = connection.Get<User>(id);
                //the date can't be 2001 - it should be the autogenerated date from the database
                user.CreatedDate.Year.IsEqualTo(DateTime.Now.Year);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test update with readonly property.
        /// </summary>
        public void TestUpdateWithReadonlyProperty()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestUpdateWithReadonlyProperty", Age = 10 });
                var user = connection.Get<User>(id);
                user.Age = 11;
                user.CreatedDate = new DateTime(2001, 1, 1);
                connection.Update(user);
                user = connection.Get<User>(id);
                //don't allow changing created date since it has a readonly attribute
                user.CreatedDate.Year.IsEqualTo(DateTime.Now.Year);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test Get With NotMapped Property
        /// </summary>
        public void TestGetWithNotMappedProperty()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestGetWithNotMappedProperty", Age = 10, NotMappedInt = 1000 });
                var user = connection.Get<User>(id);
                user.NotMappedInt.IsEqualTo(0);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test Insert With NotMapped Property
        /// </summary>
        public void TestInsertWithNotMappedProperty()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestInsertWithNotMappedProperty", Age = 10, CreatedDate = new DateTime(2001, 1, 1), NotMappedInt = 1000 });
                var user = connection.Get<User>(id);
                user.NotMappedInt.IsEqualTo(0);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test Update With NotMapped Property
        /// </summary>
        public void TestUpdateWithNotMappedProperty()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestUpdateWithNotMappedProperty", Age = 10 });
                var user = connection.Get<User>(id);
                user.Age = 11;
                user.CreatedDate = new DateTime(2001, 1, 1);
                user.NotMappedInt = 1234;
                connection.Update(user);
                user = connection.Get<User>(id);

                user.NotMappedInt.IsEqualTo(0);

                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// Test Insert With SpecifiedKey
        /// </summary>
        public void TestInsertWithSpecifiedKey()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new Car { Make = "Honda", Model = "Civic" });
                id.IsEqualTo(1);
            }
        }

        /// <summary>
        /// <para>Test Insert With ExtraProperties Should Skip NonSimpleTypes And Properties </para>
        /// <para>Marked EditableFalse</para>
        /// </summary>
        public void TestInsertWithExtraPropertiesShouldSkipNonSimpleTypesAndPropertiesMarkedEditableFalse()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new Car { Make = "Honda", Model = "Civic", Users = new List<User> { new User { Age = 12, Name = "test" } } });
                id.IsEqualTo(2);
            }
        }

        /// <summary>
        /// TestUpdate
        /// </summary>
        public void TestUpdate()
        {
            using (var connection = GetOpenConnection())
            {
                var newid = (int)connection.Insert(new Car { Make = "Honda", Model = "Civic" });
                var newitem = connection.Get<Car>(newid);
                newitem.Make = "Toyota";
                connection.Update(newitem);
                var updateditem = connection.Get<Car>(newid);
                updateditem.Make.IsEqualTo("Toyota");
                connection.Delete<Car>(newid);
            }
        }

        /// <summary>
        /// <para>We expect scheduled day off to NOT be updated, since it's not a property of </para>
        /// <para>UserEditableSettings</para>
        /// </summary>
        public void TestUpdateUsingGenericLimitedFields()
        {
            using (var connection = GetOpenConnection())
            {
                //arrange
                var user = new User { Name = "User1", Age = 10, ScheduledDayOff = DayOfWeek.Friday };
                user.Id = connection.Insert(user) ?? 0;

                user.ScheduledDayOff = DayOfWeek.Thursday;
                var userAsEditableSettings = (UserEditableSettings)user;
                userAsEditableSettings.Name = "User++";

                connection.Update(userAsEditableSettings);

                //act
                var insertedUser = connection.Get<User>(user.Id);

                //assert
                insertedUser.Name.IsEqualTo("User++");
                insertedUser.ScheduledDayOff.IsEqualTo(DayOfWeek.Friday);

                connection.Delete<User>(user.Id);
            }
        }

        /// <summary>
        /// <para>We expect scheduled day off to NOT be updated, since it's not a property of </para>
        /// <para>UserEditableSettings</para>
        /// </summary>
        public void TestUpdateUsingGenericLimitedFieldsAsync()
        {
            using (var connection = GetOpenConnection())
            {
                //arrange
                var user = new User { Name = "User1", Age = 10, ScheduledDayOff = DayOfWeek.Friday };
                user.Id = connection.Insert(user) ?? 0;

                user.ScheduledDayOff = DayOfWeek.Thursday;
                var userAsEditableSettings = (UserEditableSettings)user;
                userAsEditableSettings.Name = "User++";

                connection.UpdateAsync(userAsEditableSettings).Wait();

                //act
                var insertedUser = connection.Get<User>(user.Id);

                //assert
                insertedUser.Name.IsEqualTo("User++");
                insertedUser.ScheduledDayOff.IsEqualTo(DayOfWeek.Friday);

                connection.Delete<User>(user.Id);
            }
        }

        /// <summary>
        /// Test Delete ByObject With Attributes
        /// </summary>
        public void TestDeleteByObjectWithAttributes()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new Car { Make = "Honda", Model = "Civic" });
                var car = connection.Get<Car>(id);
                connection.Delete(car);
                connection.Get<Car>(id).IsNull();
            }
        }

        /// <summary>
        /// Test Delete By MultipleKey Object With Attributes
        /// </summary>
        public void TestDeleteByMultipleKeyObjectWithAttributesKeyMaster()
        {
            using (var connection = GetOpenConnection())
            {
                var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
                connection.Insert(keyMaster);
                var car = connection.Get<KeyMaster>(new { Key1 = 1, Key2 = 2 });
                connection.Delete(car);
                connection.Get<KeyMaster>(keyMaster).IsNull();
            }
        }

        /// <summary>
        /// Test Complex Types Marked Editable AreSaved
        /// </summary>
        public void TestComplexTypesMarkedEditableAreSaved()
        {
            using (var connection = GetOpenConnection())
            {
                var id = (int)connection.Insert(new User { Name = "User", Age = 11, ScheduledDayOff = DayOfWeek.Friday });
                var user1 = connection.Get<User>(id);
                user1.ScheduledDayOff.IsEqualTo(DayOfWeek.Friday);
                connection.Delete(user1);
            }
        }

        /// <summary>
        /// Test Nullable SimpleTypes AreSaved
        /// </summary>
        public void TestNullableSimpleTypesAreSaved()
        {
            using (var connection = GetOpenConnection())
            {
                var id = (int)connection.Insert(new User1 { Name = "User", Age = 11, ScheduledDayOff = 2 });
                var user1 = connection.Get<User1>(id);
                user1.ScheduledDayOff.IsEqualTo(2);
                connection.Delete(user1);
            }
        }

        /// <summary>
        /// Test Insert Into DifferentSchema
        /// </summary>
        public void TestInsertIntoDifferentSchema()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new CarLog { LogNotes = "blah blah blah" });
                id.IsEqualTo(1);
                connection.Delete<CarLog>(id);

            }
        }

        /// <summary>
        /// Test Get From DifferentSchema
        /// </summary>
        public void TestGetFromDifferentSchema()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new CarLog { LogNotes = "TestGetFromDifferentSchema" });
                var carlog = connection.Get<CarLog>(id);
                carlog.LogNotes.IsEqualTo("TestGetFromDifferentSchema");
                connection.Delete<CarLog>(id);
            }
        }

        /// <summary>
        /// <para>Test Trying To Get From Table In Schema Without</para>
        /// <para>Data Annotation Should Fail</para>
        /// </summary>
        public void TestTryingToGetFromTableInSchemaWithoutDataAnnotationShouldFail()
        {
            using (var connection = GetOpenConnection())
            {
                try
                {
                    connection.Get<SchemalessCarLog>(1);
                }
                catch (Exception)
                {
                    //we expect to get an exception, so return
                    return;
                }

                //if we get here without throwing an exception, the test failed.
                throw new Exception("Expected exception");
            }
        }

        /// <summary>
        /// TestGetFromTableWithNonIntPrimaryKey
        /// </summary>
        public void TestGetFromTableWithNonIntPrimaryKey()
        {
            using (var connection = GetOpenConnection())
            {
                //note - there's not support for inserts without a non-int id, so drop down to a normal execute
                connection.Execute("INSERT INTO CITY (NAME, POPULATION) VALUES ('Morgantown', 31000)");
                var city = connection.Get<City>("Morgantown");
                city.Population.IsEqualTo(31000);
            }
        }

        /// <summary>
        /// TestDeleteFromTableWithNonIntPrimaryKey
        /// </summary>
        public void TestDeleteFromTableWithNonIntPrimaryKey()
        {
            using (var connection = GetOpenConnection())
            {
                //note - there's not support for inserts without a non-int id, so drop down to a normal execute
                connection.Execute("INSERT INTO CITY (NAME, POPULATION) VALUES ('Fairmont', 18737)");
                connection.Delete<City>("Fairmont").IsEqualTo(1);
            }
        }

        /// <summary>
        /// TestNullableEnumInsert
        /// </summary>
        public void TestNullableEnumInsert()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "Enum-y", Age = 10, ScheduledDayOff = DayOfWeek.Thursday });
                var user = connection.GetList<User>(new { Name = "Enum-y" }).FirstOrDefault() ?? new User();
                user.ScheduledDayOff.IsEqualTo(DayOfWeek.Thursday);
                connection.Delete<User>(user.Id);
            }
        }

        /// <summary>
        /// dialect test 
        /// </summary>
        public void TestChangeDialect()
        {
            var currentDbType = SimpleCRUD.GetDialect();
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.MySQL);
            SimpleCRUD.GetDialect().IsEqualTo(SimpleCRUD.Dialect.MySQL);
            SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
            SimpleCRUD.GetDialect().IsEqualTo(SimpleCRUD.Dialect.SQLServer);
            SimpleCRUD.SetDialect(currentDbType);
        }

        /// <summary>
        /// <para>A GUID is being created and returned on insert but never actually</para>
        /// <para>applied to the insert query.</para>

        ///This can be seen on a table where the key
        ///is a GUID and defaults to(newid()) and no GUID is provided on the
        ///insert.Dapper will generate a GUID but it is not applied so the GUID is
        ///generated by newid() but the Dapper GUID is returned instead which is
        ///        incorrect.
        ///GUID primary key tests
        /// </summary>
        public void TestInsertIntoTableWithUnspecifiedGuidKey()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "GuidUser" });
                id.GetType().Name.IsEqualTo("Guid");
                var record = connection.Get<GUIDTest>(id);
                record.Name.IsEqualTo("GuidUser");
                connection.Delete<GUIDTest>(id);
            }
        }

        /// <summary>
        /// TestInsertIntoTableWithGuidKey
        /// </summary>
        public void TestInsertIntoTableWithGuidKey()
        {
            using (var connection = GetOpenConnection())
            {
                var guid = new Guid("1a6fb33d-7141-47a0-b9fa-86a1a1945da9");
                var id = connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "InsertIntoTableWithGuidKey", Id = guid });
                id.IsEqualTo(guid);
                connection.Delete<GUIDTest>(id);
            }
        }

        /// <summary>
        /// TestGetRecordWithGuidKey
        /// </summary>
        public void TestGetRecordWithGuidKey()
        {
            using (var connection = GetOpenConnection())
            {
                var guid = new Guid("2a6fb33d-7141-47a0-b9fa-86a1a1945da9");
                connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "GetRecordWithGuidKey", Id = guid });
                var id = connection.GetList<GUIDTest>().First().Id;
                var record = connection.Get<GUIDTest>(id);
                record.Name.IsEqualTo("GetRecordWithGuidKey");
                connection.Delete<GUIDTest>(id);

            }
        }

        /// <summary>
        /// TestGetRecordWithGuidKey
        /// </summary>
        public void TestDeleteRecordWithGuidKey()
        {
            using (var connection = GetOpenConnection())
            {
                var guid = new Guid("3a6fb33d-7141-47a0-b9fa-86a1a1945da9");
                connection.Insert<Guid, GUIDTest>(new GUIDTest { Name = "DeleteRecordWithGuidKey", Id = guid });
                var id = connection.GetList<GUIDTest>().First().Id;
                connection.Delete<GUIDTest>(id);
                connection.Get<GUIDTest>(id).IsNull();
            }
        }

        /// <summary>
        /// async  tests
        /// </summary>
        public void TestMultiInsertASync()
        {
            using (var connection = GetOpenConnection())
            {
                connection.InsertAsync(new User { Name = "TestMultiInsertASync1", Age = 10 });
                connection.InsertAsync(new User { Name = "TestMultiInsertASync2", Age = 10 });
                connection.InsertAsync(new User { Name = "TestMultiInsertASync3", Age = 10 });
                connection.InsertAsync(new User { Name = "TestMultiInsertASync4", Age = 11 });
                System.Threading.Thread.Sleep(300);
                //tiny wait to let the inserts happen
                var list = connection.GetList<User>(new { Age = 10 });
                list.Count().IsEqualTo(3);
                connection.Execute("Delete from Users");

            }
        }

        /// <summary>
        /// MultiInsertWithGuidAsync
        /// </summary>
        public void MultiInsertWithGuidAsync()
        {
            using (var connection = GetOpenConnection())
            {
                connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" });
                connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" });
                connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" });
                connection.InsertAsync<Guid, GUIDTest>(new GUIDTest { Name = "MultiInsertWithGuidAsync" });
                //tiny wait to let the inserts happen
                System.Threading.Thread.Sleep(300);
                var list = connection.GetList<GUIDTest>(new { Name = "MultiInsertWithGuidAsync" });
                list.Count().IsEqualTo(4);
                connection.Execute("Delete from GUIDTest");
            }
        }

        /// <summary>
        /// TestSimpleGetAsync
        /// </summary>
        public void TestSimpleGetAsync()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestSimpleGetAsync", Age = 10 });
                var user = connection.GetAsync<User>(id);
                user.Result.Name.IsEqualTo("TestSimpleGetAsync");
                connection.Delete<User>(id);
            }
        }

        /// <summary>
        /// TestMultipleKeyGetAsync
        /// </summary>
        public void TestMultipleKeyGetAsyncKeyMaster()
        {
            using (var connection = GetOpenConnection())
            {
                var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
                connection.Insert(keyMaster);
                var result = connection.GetAsync<KeyMaster>(new { Key1 = 1, Key2 = 2 });
                result.Result.Key1.IsEqualTo(1);
                result.Result.Key2.IsEqualTo(2);
                connection.Delete(keyMaster);
            }
        }

        /// <summary>
        /// TestDeleteByIdAsync
        /// </summary>
        public void TestDeleteByIdAsync()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "UserAsyncDelete", Age = 10 });
                connection.DeleteAsync<User>(id);
                //tiny wait to let the delete happen
                System.Threading.Thread.Sleep(300);
                connection.Get<User>(id).IsNull();
            }
        }

        /// <summary>
        /// TestDeleteByObjectAsync
        /// </summary>
        public void TestDeleteByObjectAsync()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new User { Name = "TestDeleteByObjectAsync", Age = 10 });
                var user = connection.Get<User>(id);
                connection.DeleteAsync(user);
                connection.Get<User>(id).IsNull();
                connection.Delete<User>(id);
            }
        }

        /// <summary>
        /// TestDeleteByMultipleKeyObject
        /// </summary>
        public void TestDeleteByMultipleKeyObjectKeyMaster()
        {
            using (var connection = GetOpenConnection())
            {
                var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
                connection.Insert(keyMaster);
                connection.Get<KeyMaster>(keyMaster);
                connection.Delete<KeyMaster>(new { Key1 = 1, Key2 = 2 });
                connection.Get<KeyMaster>(keyMaster).IsNull();
                connection.Delete(keyMaster);
            }
        }

        /// <summary>
        /// TestSimpleGetListAsync
        /// </summary>
        public void TestSimpleGetListAsync()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestSimpleGetListAsync1", Age = 10 });
                connection.Insert(new User { Name = "TestSimpleGetListAsync2", Age = 10 });
                var user = connection.GetListAsync<User>(new { });
                user.Result.Count().IsEqualTo(2);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestFilteredGetListAsync
        /// </summary>
        public void TestFilteredGetListAsync()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestFilteredGetListAsync1", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetListAsync2", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetListAsync3", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetListAsync4", Age = 11 });

                var user = connection.GetListAsync<User>(new { Age = 10 });
                user.Result.Count().IsEqualTo(3);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestFilteredGetListParametersAsync
        /// </summary>
        public void TestFilteredGetListParametersAsync()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestFilteredGetListParametersAsync1", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetListParametersAsync2", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetListParametersAsync3", Age = 10 });
                connection.Insert(new User { Name = "TestFilteredGetListParametersAsync4", Age = 11 });

                var user = connection.GetListAsync<User>("where Age = @Age", new { Age = 10 });
                user.Result.Count().IsEqualTo(3);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestRecordCountAsync
        /// </summary>
        public void TestRecordCountAsync()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(30);
                connection.RecordCountAsync<User>().Result.IsEqualTo(30);

                connection.RecordCountAsync<User>("where age = 10 or age = 11").Result.IsEqualTo(2);


                connection.Execute("Delete from Users");
            }

        }

        /// <summary>
        /// TestRecordCountByObjectAsync
        /// </summary>
        public void TestRecordCountByObjectAsync()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(30);
                connection.RecordCountAsync<User>().Result.IsEqualTo(30);

                connection.RecordCountAsync<User>(new { age = 10 }).Result.IsEqualTo(1);


                connection.Execute("Delete from Users");
            }

        }

        /// <summary>
        /// column attribute tests
        /// </summary>
        public void TestInsertWithSpecifiedColumnName()
        {
            using (var connection = GetOpenConnection())
            {
                var itemId = connection.Insert(new StrangeColumnNames { Word = "InsertWithSpecifiedColumnName", StrangeWord = "Strange 1" });
                itemId.IsEqualTo(1);
                connection.Delete<StrangeColumnNames>(itemId);

            }
        }

        /// <summary>
        /// TestDeleteByObjectWithSpecifiedColumnName
        /// </summary>
        public void TestDeleteByObjectWithSpecifiedColumnName()
        {
            using (var connection = GetOpenConnection())
            {
                var itemId = connection.Insert(new StrangeColumnNames { Word = "TestDeleteByObjectWithSpecifiedColumnName", StrangeWord = "Strange 1" });
                var strange = connection.Get<StrangeColumnNames>(itemId);
                connection.Delete(strange);
                connection.Get<StrangeColumnNames>(itemId).IsNull();
            }
        }

        /// <summary>
        /// TestSimpleGetListWithSpecifiedColumnName
        /// </summary>
        public void TestSimpleGetListWithSpecifiedColumnName()
        {
            using (var connection = GetOpenConnection())
            {
                var id1 = connection.Insert(new StrangeColumnNames { Word = "TestSimpleGetListWithSpecifiedColumnName1", StrangeWord = "Strange 2", });
                var id2 = connection.Insert(new StrangeColumnNames { Word = "TestSimpleGetListWithSpecifiedColumnName2", StrangeWord = "Strange 3", });
                var strange = connection.GetList<StrangeColumnNames>(new { });
                strange.First().StrangeWord.IsEqualTo("Strange 2");
                strange.Count().IsEqualTo(2);
                connection.Delete<StrangeColumnNames>(id1);
                connection.Delete<StrangeColumnNames>(id2);
            }
        }

        /// <summary>
        /// TestUpdateWithSpecifiedColumnName
        /// </summary>
        public void TestUpdateWithSpecifiedColumnName()
        {
            using (var connection = GetOpenConnection())
            {
                var newid = (int)connection.Insert(new StrangeColumnNames { Word = "Word Insert", StrangeWord = "Strange Insert" });
                var newitem = connection.Get<StrangeColumnNames>(newid);
                newitem.Word = "Word Update";
                connection.Update(newitem);
                var updateditem = connection.Get<StrangeColumnNames>(newid);
                updateditem.Word.IsEqualTo("Word Update");
                connection.Delete<StrangeColumnNames>(newid);
            }
        }

        /// <summary>
        /// TestFilteredGetListWithSpecifiedColumnName
        /// </summary>
        public void TestFilteredGetListWithSpecifiedColumnName()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new StrangeColumnNames { Word = "Word 5", StrangeWord = "Strange 1", });
                connection.Insert(new StrangeColumnNames { Word = "Word 6", StrangeWord = "Strange 2", });
                connection.Insert(new StrangeColumnNames { Word = "Word 7", StrangeWord = "Strange 2", });
                connection.Insert(new StrangeColumnNames { Word = "Word 8", StrangeWord = "Strange 2", });

                var strange = connection.GetList<StrangeColumnNames>(new { StrangeWord = "Strange 2" });
                strange.Count().IsEqualTo(3);
                connection.Execute("Delete from StrangeColumnNames");
            }
        }

        /// <summary>
        /// TestGetListPaged
        /// </summary>
        public void TestGetListPaged()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetListPaged<User>(2, 10, null, null);
                resultlist.Count().IsEqualTo(10);
                resultlist.Skip(4).First().Name.IsEqualTo("Person 14");
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestGetListPagedWithParameters
        /// </summary>
        public void TestGetListPagedWithParameters()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetListPaged<User>(1, 30, "where Age > @Age", null, new { Age = 14 });
                resultlist.Count().IsEqualTo(15);
                resultlist.First().Name.IsEqualTo("Person 15");
                connection.Execute("Delete from Users");
            }
        }


        /// <summary>
        /// TestGetListPagedWithSpecifiedPrimaryKey
        /// </summary>
        public void TestGetListPagedWithSpecifiedPrimaryKey()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new StrangeColumnNames { Word = "Word " + x, StrangeWord = "Strange " + x });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetListPaged<StrangeColumnNames>(2, 10, null, null);
                resultlist.Count().IsEqualTo(10);
                resultlist.Skip(4).First().Word.IsEqualTo("Word 14");
                connection.Execute("Delete from StrangeColumnNames");
            }
        }

        /// <summary>
        /// TestGetListPagedWithWhereClause
        /// </summary>
        public void TestGetListPagedWithWhereClause()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist1 = connection.GetListPaged<User>(1, 3, "Where Name LIKE 'Person 2%'", "age desc");
                resultlist1.Count().IsEqualTo(3);

                var resultlist = connection.GetListPaged<User>(2, 3, "Where Name LIKE 'Person 2%'", "age desc");
                resultlist.Count().IsEqualTo(3);
                resultlist.Skip(1).First().Name.IsEqualTo("Person 25");

                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestDeleteListWithWhereClause
        /// </summary>
        public void TestDeleteListWithWhereClause()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                connection.DeleteList<User>("Where age > 9");
                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(10);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestDeleteListWithWhereObject
        /// </summary>
        public void TestDeleteListWithWhereObject()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 10);

                connection.DeleteList<User>(new { age = 9 });
                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(9);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestDeleteListWithParameters
        /// </summary>
        public void TestDeleteListWithParameters()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 1;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 10);

                connection.DeleteList<User>("where age >= @Age", new { Age = 9 });
                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(8);
                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// TestRecordCountWhereClause
        /// </summary>
        public void TestRecordCountWhereClause()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(30);
                connection.RecordCount<User>().IsEqualTo(30);

                connection.RecordCount<User>("where age = 10 or age = 11").IsEqualTo(2);


                connection.Execute("Delete from Users");
            }

        }

        /// <summary>
        /// TestRecordCountWhereObject
        /// </summary>
        public void TestRecordCountWhereObject()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(30);
                connection.RecordCount<User>().IsEqualTo(30);

                connection.RecordCount<User>(new { age = 10 }).IsEqualTo(1);


                connection.Execute("Delete from Users");
            }

        }

        /// <summary>
        /// TestRecordCountParameters
        /// </summary>
        public void TestRecordCountParameters()
        {
            using (var connection = GetOpenConnection())
            {
                int x = 0;
                do
                {
                    connection.Insert(new User { Name = "Person " + x, Age = x, CreatedDate = DateTime.Now, ScheduledDayOff = DayOfWeek.Thursday });
                    x++;
                } while (x < 30);

                var resultlist = connection.GetList<User>();
                resultlist.Count().IsEqualTo(30);
                connection.RecordCount<User>("where Age > 15").IsEqualTo(14);


                connection.Execute("Delete from Users");
            }

        }

        /// <summary>
        /// TestInsertWithSpecifiedPrimaryKey
        /// </summary>
        public void TestInsertWithSpecifiedPrimaryKey()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.Insert(new UserWithoutAutoIdentity() { Id = 999, Name = "User999", Age = 10 });
                id.IsEqualTo(999);
                var user = connection.Get<UserWithoutAutoIdentity>(999);
                user.Name.IsEqualTo("User999");
                connection.Execute("Delete from UserWithoutAutoIdentity");
            }
        }

        /// <summary>
        /// TestInsertWithSpecifiedPrimaryKeyAsync
        /// </summary>
        public void TestInsertWithSpecifiedPrimaryKeyAsync()
        {
            using (var connection = GetOpenConnection())
            {
                var id = connection.InsertAsync(new UserWithoutAutoIdentity() { Id = 999, Name = "User999Async", Age = 10 });
                id.Result.IsEqualTo(999);
                var user = connection.GetAsync<UserWithoutAutoIdentity>(999);
                user.Result.Name.IsEqualTo("User999Async");
                connection.Execute("Delete from UserWithoutAutoIdentity");
            }
        }

        /// <summary>
        /// TestInsertWithMultiplePrimaryKeysAsync
        /// </summary>
        public async void TestInsertWithMultiplePrimaryKeysAsyncKeyMaster()
        {
            using (var connection = GetOpenConnection())
            {
                var keyMaster = new KeyMaster { Key1 = 1, Key2 = 2 };
                await connection.InsertAsync(keyMaster);
                var result = connection.GetAsync<KeyMaster>(new { Key1 = 1, Key2 = 2 });
                result.Result.Key1.IsEqualTo(1);
                result.Result.Key2.IsEqualTo(2);
                connection.Execute("Delete from KeyMaster");
            }
        }

        /// <summary>
        /// TestGetListNullableWhere
        /// </summary>
        public void TestGetListNullableWhere()
        {
            using (var connection = GetOpenConnection())
            {
                connection.Insert(new User { Name = "TestGetListWithoutWhere", Age = 10, ScheduledDayOff = DayOfWeek.Friday });
                connection.Insert(new User { Name = "TestGetListWithoutWhere", Age = 10 });

                //test with null property
                var list = connection.GetList<User>(new { ScheduledDayOff = (DayOfWeek?)null });
                list.Count().IsEqualTo(1);


                // test with db.null value
                list = connection.GetList<User>(new { ScheduledDayOff = DBNull.Value });
                list.Count().IsEqualTo(1);

                connection.Execute("Delete from Users");
            }
        }

        /// <summary>
        /// <para>ignore attribute tests</para>
        /// <para>i cheated here and stuffed all of these in one test</para>
        /// <para>didn't implement in postgres or mysql tests yet</para>
        /// </summary>
        public void IgnoreProperties()
        {
            using (var connection = GetOpenConnection())
            {
                var itemId = connection.Insert(new IgnoreColumns() { IgnoreInsert = "OriginalInsert", IgnoreUpdate = "OriginalUpdate", IgnoreSelect = "OriginalSelect", IgnoreAll = "OriginalAll" });
                var item = connection.Get<IgnoreColumns>(itemId);
                //verify insert column was ignored
                item.IgnoreInsert.IsNull();

                //verify select value wasn't selected 
                item.IgnoreSelect.IsNull();

                //verify the column is really there via straight dapper
                var fromDapper = connection.Query<IgnoreColumns>("Select * from IgnoreColumns where Id = @Id", new { id = itemId }).First();
                fromDapper.IgnoreSelect.IsEqualTo("OriginalSelect");

                //change value and update
                item.IgnoreUpdate = "ChangedUpdate";
                connection.Update(item);

                //verify that update didn't take effect
                item = connection.Get<IgnoreColumns>(itemId);
                item.IgnoreUpdate.IsEqualTo("OriginalUpdate");

                var allColumnDapper = connection.Query<IgnoreColumns>("Select IgnoreAll from IgnoreColumns where Id = @Id", new { id = itemId }).First();
                allColumnDapper.IgnoreAll.IsNull();

                connection.Delete<IgnoreColumns>(itemId);
            }
        }
    }
}
