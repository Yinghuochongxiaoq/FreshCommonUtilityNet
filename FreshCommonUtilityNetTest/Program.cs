using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using Dapper;
using FreshCommonUtility.Dapper;
using FreshCommonUtilityNetTest.Dapper;
using FreshCommonUtilityNetTest.DataConvert;
using FreshCommonUtilityNetTest.DeepCopy;
using FreshCommonUtilityNetTest.Enumber;
using FreshCommonUtilityNetTest.ExcelHelper;
using FreshCommonUtilityNetTest.ExpandMath;
using FreshCommonUtilityNetTest.Security;
using FreshCommonUtilityNetTest.Zip;

namespace FreshCommonUtilityNetTest
{
    /// <summary>
    /// 测试主程序入口
    /// </summary>
    static class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            List<Type> classTypeList = new List<Type>
            {
                typeof(DataTypeConvertHelperTests),
                typeof(DeepCopyHelperTests),
                typeof(EnumberHelperTests),
                typeof(ExcelHelperTests),
                typeof(SecurityTest),
                typeof(ZipHelperTests),
                typeof(DapperSqlServerTest),
                typeof(DapperMySqlTest),
                typeof(DapperSqliteTest),
                typeof(BigIntTest)
            };

            var stopwatch = Stopwatch.StartNew();
            foreach (var type in classTypeList)
            {
                var pgtester = Activator.CreateInstance(type);
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var testwatch = Stopwatch.StartNew();
                    Console.Write("Running\t" + method.Name + "\tin FreshCommonUtilityNetTest:");
                    method.Invoke(pgtester, null);
                    Console.WriteLine("\t- OK! \t{0}ms", testwatch.ElapsedMilliseconds);
                }
            }
            stopwatch.Stop();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);

            EmitLearn.LearnInfo();
            ExpressionLear.LearnInfo();
            Console.ReadKey();
        }

        public class Contact
        {
            [Key]
            [Column("ContactID")]
            public int ContactId { get; set; }
            public string ContactName { get; set; }
            public IEnumerable<Phone> Phones { get; set; }
        }

        public class Phone
        {
            [Key]
            public int PhoneId { get; set; }

            [Column("ContactID")]
            public int ContactId { get; set; }

            public string Number { get; set; }

            public Byte IsActive { get; set; }
        }

        public static void SaveInfo()
        {
            var phone = new Phone { Number = "1234567788", IsActive = 1 };

            var contact = new Contact { ContactName = "MMP" };
            using (
                var conn =
                    new SqlConnection(
                        "data source=192.168.8.210;user id=sa;password=Evget123456789;initial catalog=ASPDataZZ19;Persist Security Info=true;")
                )
            {
                conn.Insert(contact);
                phone.ContactId = contact.ContactId;
                conn.Insert(phone);
            }
        }

        public static IEnumerable<Contact> GetContacts()
        {
            var sql = @"set nocount on
DECLARE @t TABLE(ContactID int,  ContactName nvarchar(100))
INSERT @t
SELECT *
FROM Contact
set nocount off 
SELECT * FROM @t 
SELECT * FROM Phone where ContactId in (select t.ContactId from @t t)";
            using (var conn = new SqlConnection("data source=192.168.8.210;user id=sa;password=Evget123456789;initial catalog=ASPDataZZ19;Persist Security Info=true;"))
            {
                conn.Open();
                var mapped = conn.QueryMultiple(sql).Map<Contact, Phone, int>
        (
           contact => contact.ContactId,
           phone => phone.ContactId,
           (contact, phones) => { contact.Phones = phones; }
        );

                return mapped;
            }
        }
    }
}
