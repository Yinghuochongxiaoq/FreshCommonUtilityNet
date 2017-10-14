#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest.Dapper
//文件名称：DapperSqlite
//创 建 人：FreshMan
//创建日期：2017/7/8 14:54:30
//用    途：记录类的用途
//======================================================================
#endregion
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using FreshCommonUtility.Dapper;

namespace FreshCommonUtilityNetTest.Dapper
{
    public class DapperSqliteTest
    {
        /// <summary>
        /// test sqlite
        /// </summary>
        private void SetupSqLite()
        {
            var filepath = Directory.GetCurrentDirectory() + "\\MyDatabase.sqlite";
            if (File.Exists(filepath)) File.Delete(filepath);
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
            var connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            using (connection)
            {
                connection.Open();

                connection.Execute(@" create table Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name nvarchar(100) not null, Age int not null, ScheduledDayOff int null, CreatedDate datetime default current_timestamp ) ");
                connection.Execute(@" create table Car (CarId INTEGER PRIMARY KEY AUTOINCREMENT, Id INTEGER null, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" create table BigCar (CarId INTEGER PRIMARY KEY AUTOINCREMENT, Make nvarchar(100) not null, Model nvarchar(100) not null) ");
                connection.Execute(@" insert into BigCar (CarId,Make,Model) Values (2147483649,'car','car') ");
                connection.Execute(@" create table City (Name nvarchar(100) not null, Population int not null) ");
                connection.Execute(@" CREATE TABLE GUIDTest([Id] [uniqueidentifier] NOT NULL,[name] [varchar](50) NOT NULL, CONSTRAINT [PK_GUIDTest] PRIMARY KEY  ([Id] ASC))");
                connection.Execute(@" create table StrangeColumnNames (ItemId INTEGER PRIMARY KEY AUTOINCREMENT, word nvarchar(100) not null, colstringstrangeword nvarchar(100) not null, KeywordedProperty nvarchar(100) null) ");
                connection.Execute(@" create table UserWithoutAutoIdentity (Id INTEGER PRIMARY KEY, Name nvarchar(100) not null, Age int not null) ");
                connection.Execute(@" create table IgnoreColumns (Id INTEGER PRIMARY KEY AUTOINCREMENT, IgnoreInsert nvarchar(100) null, IgnoreUpdate nvarchar(100) null, IgnoreSelect nvarchar(100)  null, IgnoreAll nvarchar(100) null) ");
                connection.Execute(@" CREATE TABLE KeyMaster (Key1 INTEGER NOT NULL, Key2 INTEGER NOT NULL, PRIMARY KEY ([Key1], [Key2]))");
                connection.Execute(
                    @" CREATE TABLE Test (Id INTEGER PRIMARY KEY AUTOINCREMENT,Name nvarchar(100) NULL ) ");

            }
        }

        /// <summary>
        /// test sqlite
        /// </summary>
        private void RunTestsSqLite()
        {
            var stopwatch = Stopwatch.StartNew();
            var pgtester = new SimpleCrudTest(SimpleCRUD.Dialect.SQLite);
            foreach (var method in typeof(SimpleCrudTest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                //skip schema tests
                if (method.Name.Contains("Schema")) continue;
                var testwatch = Stopwatch.StartNew();
                Console.Write("Running\t" + method.Name + "\tin SQLite");
                method.Invoke(pgtester, null);
                Console.WriteLine("\t- OK! \t{0}ms", testwatch.ElapsedMilliseconds);
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
            Console.Write("SQLite testing complete.");
            Console.ReadKey();
        }

        public void SqliteDapperTest()
        {
            SetupSqLite();
            RunTestsSqLite();
        }
    }
}
