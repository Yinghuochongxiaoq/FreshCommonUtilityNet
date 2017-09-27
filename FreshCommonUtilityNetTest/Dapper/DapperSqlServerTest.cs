#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest.Dapper
//文件名称：DapperTest
//创 建 人：FreshMan
//创建日期：2017/7/8 14:45:52
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Dapper;
using FreshCommonUtility.Dapper;
using FreshCommonUtility.SqlHelper;

namespace FreshCommonUtilityNetTest.Dapper
{
    public class DapperSqlServerTest
    {
        /// <summary>
        /// SQL DbInit
        /// </summary>
        private void SetUp()
        {
            using (var connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=true"))
            {
                connection.Open();
                try
                {
                    connection.Execute(@" DROP DATABASE DapperSimpleCrudTestDb; ");
                }
                catch (Exception)
                {
                    // ignored
                }

                connection.Execute(@" CREATE DATABASE DapperSimpleCrudTestDb; ");
            }

            using (var connection = new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB;Initial Catalog=DapperSimpleCrudTestDb;Integrated Security=True;MultipleActiveResultSets=true;"))
            {
                connection.Open();
                connection.Execute(@" create table Users (Id int IDENTITY(1,1) not null, Name nvarchar(100) not null, Age int not null, ScheduledDayOff int null, CreatedDate datetime DEFAULT(getdate())) ");
                connection.Execute(@" create table Car (CarId int IDENTITY(1,1) not null, Id int null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" create table BigCar (CarId bigint IDENTITY(2147483650,1) not null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" create table City (Name nvarchar(100) not null, Population int not null) ");
                connection.Execute(@" CREATE SCHEMA Log; ");
                connection.Execute(@" create table Log.CarLog (Id int IDENTITY(1,1) not null, LogNotes nvarchar(100) NOT NULL) ");
                connection.Execute(@" CREATE TABLE [dbo].[GUIDTest]([Id] [uniqueidentifier] NOT NULL,[name] [varchar](50) NOT NULL, CONSTRAINT [PK_GUIDTest] PRIMARY KEY CLUSTERED ([Id] ASC))");
                connection.Execute(@" create table StrangeColumnNames (ItemId int IDENTITY(1,1) not null Primary Key, word nvarchar(100) not null, colstringstrangeword nvarchar(100) not null, KeywordedProperty nvarchar(100) null)");
                connection.Execute(@" create table UserWithoutAutoIdentity (Id int not null Primary Key, Name nvarchar(100) not null, Age int not null) ");
                connection.Execute(@" create table IgnoreColumns (Id int IDENTITY(1,1) not null Primary Key, IgnoreInsert nvarchar(100) null, IgnoreUpdate nvarchar(100) null, IgnoreSelect nvarchar(100)  null, IgnoreAll nvarchar(100) null) ");
                connection.Execute(@" CREATE TABLE GradingScale ([ScaleID] [int] IDENTITY(1,1) NOT NULL, [AppID] [int] NULL, [ScaleName] [nvarchar](50) NOT NULL, [IsDefault] [bit] NOT NULL)");
                connection.Execute(@" CREATE TABLE KeyMaster ([Key1] [int] NOT NULL, [Key2] [int] NOT NULL, CONSTRAINT [PK_KeyMaster] PRIMARY KEY CLUSTERED ([Key1] ASC, [Key2] ASC))");
                connection.Execute(@" CREATE TABLE [Test] ([Id] int NOT NULL IDENTITY(1,1) Primary Key,[Name] nvarchar(MAX) NULL ) ");
            }
            Console.WriteLine("Created database");
        }

        /// <summary>
        /// test sql server
        /// </summary>
        private void RunTestSqlServer()
        {
            var stopwatch = Stopwatch.StartNew();
            var sqltester = new SimpleCrudTest(SimpleCRUD.Dialect.SQLServer);
            foreach (var method in typeof(SimpleCrudTest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var testwatch = Stopwatch.StartNew();
                Console.Write($"Running\t{method.Name} \tin sql server");
                method.Invoke(sqltester, null);
                testwatch.Stop();
                Console.WriteLine($"\t- OK! \t{testwatch.ElapsedMilliseconds}'ms ");
            }
            stopwatch.Stop();
            //write result
            Console.WriteLine($"Time elapsed :{stopwatch.Elapsed}");

            using (var connection = SqlConnectionHelper.GetOpenConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Master;Integrated Security=True"))
            {
                connection.Execute(@" alter database DapperSimpleCrudTestDb set single_user with rollback immediate; DROP DATABASE DapperSimpleCrudTestDb; ");
            }
            Console.Write("SQL Server testing complete.");
        }

        public void SqlServerDapperTest()
        {
            SetUp();
            RunTestSqlServer();
        }
    }
}
