#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest.Dapper
//文件名称：DapperMySqlTest
//创 建 人：FreshMan
//创建日期：2017/7/8 14:49:17
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Dapper;
using FreshCommonUtility.Dapper;
using MySql.Data.MySqlClient;

namespace FreshCommonUtilityNetTest.Dapper
{
    public class DapperMySqlTest
    {
        /// <summary>
        /// MySQL DbInit
        /// </summary>
        private void SetupMySql()
        {
            using (var connection = new MySqlConnection(
                $"Server={"localhost"};Port={"3306"};User Id={"FreshMan"};Password={"qinxianbo"};Database={"sys"};SslMode=None"))
            {
                connection.Open();
                // drop  database 
                try
                {
                    connection.Execute("DROP DATABASE IF EXISTS testdb;");
                }
                catch (Exception)
                {
                    // ignored
                }
                connection.Execute("CREATE DATABASE testdb;");
            }
            Thread.Sleep(1000);

            using (var connection = new MySqlConnection(
                $"Server={"localhost"};Port={"3306"};User Id={"FreshMan"};Password={"qinxianbo"};Database={"testdb"};SslMode=None"))
            {
                connection.Open();
                connection.Execute(@" create table Users (Id INTEGER PRIMARY KEY AUTO_INCREMENT, Name nvarchar(100) not null, Age int not null, ScheduledDayOff int null, CreatedDate datetime default current_timestamp ) ");
                connection.Execute(@" create table Car (CarId INTEGER PRIMARY KEY AUTO_INCREMENT, Id INTEGER null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" create table BigCar (CarId BIGINT PRIMARY KEY AUTO_INCREMENT, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" insert into BigCar (CarId,Make,Model) Values (2147483649,'car','car') ");
                connection.Execute(@" create table City (Name nvarchar(100) not null, Population int not null) ");
                connection.Execute(@" CREATE TABLE GUIDTest(Id CHAR(38) NOT NULL,name varchar(50) NOT NULL, CONSTRAINT PK_GUIDTest PRIMARY KEY (Id ASC))");
                connection.Execute(@" create table StrangeColumnNames (ItemId INTEGER PRIMARY KEY AUTO_INCREMENT, word nvarchar(100) not null, colstringstrangeword nvarchar(100) not null, KeywordedProperty nvarchar(100) null) ");
                connection.Execute(@" create table UserWithoutAutoIdentity (Id INTEGER PRIMARY KEY, Name nvarchar(100) not null, Age int not null) ");
                connection.Execute(
                    @" CREATE TABLE Test(Id INTEGER PRIMARY KEY AUTO_INCREMENT ,Name nvarchar(100) NULL ) ");
                connection.Close();
            }

        }

        /// <summary>
        /// test mysql
        /// </summary>
        private void RunTestMySql()
        {
            var stopwatch = Stopwatch.StartNew();
            var mysqltester = new SimpleCrudTest(SimpleCRUD.Dialect.MySQL);
            mysqltester.GetOpenConnection();
            foreach (var method in typeof(SimpleCrudTest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                //skip schema tests
                if (method.Name.Contains("Schema")) continue;
                if (method.Name.Contains("Guid")) continue;
                if (method.Name.Contains("KeyMaster")) continue;
                if (method.Name.Contains("Ignore")) continue;
                var testwatch = Stopwatch.StartNew();
                Console.Write("Running\t" + method.Name + "\tin MySQL");
                method.Invoke(mysqltester, null);
                Console.WriteLine("\t- OK! \t{0}ms", testwatch.ElapsedMilliseconds);
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);

            Console.Write("MySQL testing complete.");
        }

        public void MySqlDapperTest()
        {
            SetupMySql();
            RunTestMySql();
        }
    }
}
