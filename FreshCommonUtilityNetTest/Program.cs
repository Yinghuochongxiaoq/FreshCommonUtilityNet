using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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

            //EmitLearn.LearnInfo();

            Console.ReadKey();
        }
    }
}
