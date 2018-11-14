#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest.Dapper
//文件名称：DapperOracleTest
//创 建 人：FreshMan
//创建日期：2018/10/22 09:55:17
//用    途：记录类的用途
//======================================================================
#endregion
using Dapper;
using FreshCommonUtility.Dapper;
using FreshCommonUtility.SqlHelper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace FreshCommonUtilityNetTest.Dapper
{
    public class DapperOracleTest
    {
        /**生成测试数据*/

        //select rownum as id,
        //       to_char(sysdate + rownum/24/3600, 'yyyy-mm-dd hh24:mi:ss') as inc_datetime,
        //       trunc(dbms_random.value(0, 100)) as random_id,
        //       dbms_random.string ('x', 20) random_string
        //        from dual
        //        connect by level <= 10;
        string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=183.230.101.143)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=tysh)));Persist Security Info=True;User ID=xns;Password=xns;";

        /// <summary>
        /// Oracle DbInit
        /// </summary>
        private void SetupOracle()
        {
            using (var connection = new OracleConnection(connectionString))
            {
                SimpleCRUD.SetDialect(SimpleCRUD.Dialect.Oracle);
                connection.Open();
                DropTable(connection);
                try
                {
                    connection.Execute(@"CREATE TABLE USERS (Id NUMBER NOT NULL,Name VARCHAR2 (100 CHAR) NULL,Age NUMBER NULL,ScheduledDayOff NUMBER NULL,CreatedDate DATE DEFAULT SYSDATE NULL)");
                    connection.Execute(@" ALTER TABLE USERS ADD CHECK (Id IS NOT NULL)");
                    connection.Execute(@" ALTER TABLE USERS ADD PRIMARY KEY (Id)");

                    connection.Execute(@" create table Car(CarId NUMBER NOT NULL, Id NUMBER null,Make VARCHAR2 (100 CHAR) not null, Model VARCHAR2 (100 CHAR) not null) ");
                    connection.Execute(@" ALTER TABLE Car ADD PRIMARY KEY (CarId)");

                    connection.Execute(@" create table BigCar (CarId NUMBER NOT NULL, Make  VARCHAR2 (100 CHAR) not null, Model  VARCHAR2 (100 CHAR) not null) ");
                    connection.Execute(@" ALTER TABLE BigCar ADD PRIMARY KEY (CarId)");
                    connection.Execute(@" insert into BigCar (CarId,Make,Model) Values (2147483649,'car','car') ");

                    connection.Execute(@" create table City (Name VARCHAR2 (100 CHAR) not null, Population NUMBER not null) ");

                    connection.Execute(@" CREATE TABLE GUIDTest(Id VARCHAR2 (38 CHAR) NOT NULL,name VARCHAR2 (50 CHAR) NOT NULL)");

                    connection.Execute(@" create table StrangeColumnNames (ItemId NUMBER NOT NULL, word VARCHAR2 (100 CHAR) not null, colstringstrangeword VARCHAR2 (100 CHAR) not null, KeywordedProperty VARCHAR2 (100 CHAR) null) ");
                    connection.Execute(@" ALTER TABLE StrangeColumnNames ADD PRIMARY KEY (ItemId)");

                    connection.Execute(@" create table UserWithoutAutoIdentity (Id NUMBER NOT NULL , Name VARCHAR2 (100 CHAR) not null, Age NUMBER not null) ");
                    connection.Execute(@" ALTER TABLE UserWithoutAutoIdentity ADD PRIMARY KEY (Id)");

                    connection.Execute(
                        @" CREATE TABLE Test(Id NUMBER NOT NULL ,Name VARCHAR2 (100 CHAR) NULL ) ");
                    connection.Execute(@" ALTER TABLE Test ADD PRIMARY KEY (Id)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // ignored
                }
                finally
                {
                    CloseConn(connection);
                }
            }
            Console.WriteLine("Created database");
        }

        private void RunTestOracle()
        {
            var stopwatch = Stopwatch.StartNew();
            var sqltester = new SimpleCrudForOracleTest(SimpleCRUD.Dialect.Oracle);
            foreach (var method in typeof(SimpleCrudForOracleTest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (method.Name.Contains("Schema")) continue;
                if (method.Name.Contains("Guid")) continue;
                if (method.Name.Contains("KeyMaster")) continue;
                if (method.Name.Contains("Ignore")) continue;
                var testwatch = Stopwatch.StartNew();
                Console.Write($"Running\t{method.Name} \tin Oracle");
                try
                {
                    method.Invoke(sqltester, null);
                    testwatch.Stop();
                    Console.WriteLine($"\t- OK! \t{testwatch.ElapsedMilliseconds}'ms ");
                }
                catch (Exception ex)
                {
                    testwatch.Stop();
                    Console.WriteLine($"\t- Fail! \t{testwatch.ElapsedMilliseconds}'ms {ex.Message}");
                }
            }
            stopwatch.Stop();
            //write result
            Console.WriteLine($"Time elapsed :{stopwatch.Elapsed}");

            using (var connection = SqlConnectionHelper.GetOpenConnection(connectionString))
            {
                DropTable(connection);
            }
            Console.Write("Oracle testing complete.");
        }

        public void OracleDapperTest()
        {
            SetupOracle();
            RunTestOracle();
        }

        static void CloseConn(OracleConnection conn)
        {
            if (conn == null) { return; }
            try
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Dispose();
            }
        }

        private void DropTable(IDbConnection connection)
        {
            try
            {
                connection.Execute("DROP TABLE Users");
            }
            catch (Exception)
            { }
            try
            {
                connection.Execute("DROP TABLE Car");
            }
            catch (Exception)
            { }
            try
            {
                connection.Execute("DROP TABLE BigCar");
            }
            catch (Exception)
            { }
            try
            {
                connection.Execute("DROP TABLE City");
            }
            catch (Exception)
            { }
            try
            {
                connection.Execute("DROP TABLE GUIDTest");
            }
            catch (Exception)
            { }
            try
            {
                connection.Execute("DROP TABLE StrangeColumnNames");
            }
            catch (Exception)
            { }
            try
            {
                connection.Execute("DROP TABLE UserWithoutAutoIdentity");
            }
            catch (Exception)
            { }
            try
            {
                connection.Execute("DROP TABLE Test");
            }
            catch (Exception)
            { }
        }
    }
}