using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using FreshCommonUtility.Web;
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
                typeof(DapperOracleTest),
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

            //EmitLearn.LearnInfo();
            //ExpressionLear.LearnInfo();

            //string result = WebHttpHelper.HttpsPost("https://www.ly.com", "key=123"); // key=4da4193e-384b-44d8-8a7f-2dd8b076d784
            //result.IsNotEqualTo(string.Empty);

            Console.ReadKey();
        }

        private static int VerifySignature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, string sSigture)
        {
            string hash = "";
            int ret = 0;
            ret = GenarateSinature(sToken, sTimeStamp, sNonce, sMsgEncrypt, ref hash);
            if (ret != 0)
                return ret;
            System.Console.WriteLine(hash);
            if (hash == sSigture)
                return 0;
            return -1;
        }

        public class DictionarySort : IComparer
        {
            public int Compare(object oLeft, object oRight)
            {
                string sLeft = oLeft as string;
                string sRight = oRight as string;
                int iLeftLength = sLeft.Length;
                int iRightLength = sRight.Length;
                int index = 0;
                while (index < iLeftLength && index < iRightLength)
                {
                    if (sLeft[index] < sRight[index])
                        return -1;
                    if (sLeft[index] > sRight[index])
                        return 1;
                    index++;
                }
                return iLeftLength - iRightLength;

            }
        }

        public static int GenarateSinature(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, ref string sMsgSignature)
        {
            ArrayList AL = new ArrayList {sToken, sTimeStamp, sNonce, sMsgEncrypt};
            AL.Sort(new DictionarySort());
            string raw = "";
            for (int i = 0; i < AL.Count; ++i)
            {
                raw += AL[i];
            }

            string hash;
            try
            {
                SHA1 sha = new SHA1CryptoServiceProvider();
                var enc = new ASCIIEncoding();
                byte[] dataToHash = enc.GetBytes(raw);
                byte[] dataHashed = sha.ComputeHash(dataToHash);
                hash = BitConverter.ToString(dataHashed).Replace("-", "");
                hash = hash.ToLower();
            }
            catch (Exception)
            {
                return -1;
            }
            sMsgSignature = hash;
            return 0;
        }


        //        public class Contact
        //        {
        //            [Key]
        //            [Column("ContactID")]
        //            public int ContactId { get; set; }
        //            public string ContactName { get; set; }
        //            public IEnumerable<Phone> Phones { get; set; }
        //        }

        //        public class Phone
        //        {
        //            [Key]
        //            public int PhoneId { get; set; }

        //            [Column("ContactID")]
        //            public int ContactId { get; set; }

        //            public string Number { get; set; }

        //            public Byte IsActive { get; set; }
        //        }

        //        public static void SaveInfo()
        //        {
        //            var phone = new Phone { Number = "1234567788", IsActive = 1 };

        //            var contact = new Contact { ContactName = "MMP" };
        //            using (
        //                var conn =
        //                    new SqlConnection(
        //                        "data source=192.168.8.210;user id=sa;password=Evget123456789;initial catalog=ASPDataZZ19;Persist Security Info=true;")
        //                )
        //            {
        //                conn.Insert(contact);
        //                phone.ContactId = contact.ContactId;
        //                conn.Insert(phone);
        //            }
        //        }

        //        public static IEnumerable<Contact> GetContacts()
        //        {
        //            var sql = @"set nocount on
        //DECLARE @t TABLE(ContactID int,  ContactName nvarchar(100))
        //INSERT @t
        //SELECT *
        //FROM Contact
        //set nocount off 
        //SELECT * FROM @t 
        //SELECT * FROM Phone where ContactId in (select t.ContactId from @t t)";
        //            using (var conn = new SqlConnection("data source=192.168.8.210;user id=sa;password=Evget123456789;initial catalog=ASPDataZZ19;Persist Security Info=true;"))
        //            {
        //                conn.Open();
        //                var mapped = conn.QueryMultiple(sql).Map<Contact, Phone, int>
        //        (
        //           contact => contact.ContactId,
        //           phone => phone.ContactId,
        //           (contact, phones) => { contact.Phones = phones; }
        //        );

        //                return mapped;
        //            }
        //        }
    }
}
