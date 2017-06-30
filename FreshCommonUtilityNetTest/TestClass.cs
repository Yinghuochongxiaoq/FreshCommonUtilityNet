#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest
//文件名称：TestClass
//创 建 人：FreshMan
//创建日期：2017/6/17 11:37:55
//用    途：记录类的用途
//======================================================================
#endregion
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FreshCommonUtility.DataConvert;
using FreshCommonUtility.Security;
using FreshCommonUtilityNet.Npoi;

namespace FreshCommonUtilityNetTest
{
    /// <summary>
    /// TestClass
    /// </summary>
    public class TestClass
    {
        #region [1 AesHelperTest]

        /// <summary>
        /// 测试aes加密
        /// </summary>
        public void TestAesHelper()
        {
            var testStr = "FreshMan";
            var enCodeStr = AesHelper.AesEncrypt(testStr);
            var deCodeStr = AesHelper.AesDecrypt(enCodeStr);
            deCodeStr.IsEqualTo(testStr);
        }
        #endregion

        #region [2 DesHelperTest]

        /// <summary>
        /// 测试des加密
        /// </summary>
        public void TestDesHelper()
        {
            var testStr = "FreshMan";
            var enCodeStr = DesHelper.DesEnCode(testStr);
            var deCodeStr = DesHelper.DesDeCode(enCodeStr);
            deCodeStr.IsEqualTo(testStr);
        }
        #endregion

        #region [3 IdCardValidatorHelperTest]

        /// <summary>
        /// 获取新身份证最后一位校验位
        /// </summary>
        public void GetIdCardCheckCodeTest()
        {
            var idCardRightNumber = "632802197803272788";
            var checkCode = IdCardValidatorHelper.GetIdCardCheckCode(idCardRightNumber);
            checkCode.IsEqualTo("1");
        }

        /// <summary>
        /// 检查身份证长度是否合法
        /// </summary>
        public void CheckIdCardLenTest()
        {
            var idCardRightNumber = "632802197803272788";
            var resulte = IdCardValidatorHelper.CheckIdCardLen(idCardRightNumber);
            resulte.IsTrue();

            var idCardErrorNumber = "6328021978032727";
            var errorResulte = IdCardValidatorHelper.CheckIdCardLen(idCardErrorNumber);
            errorResulte.IsFalse();
        }

        /// <summary>
        /// 验证是否是新身份证
        /// </summary>
        public void IsNewIdCardTest()
        {
            var idCardRightNumber = "632802197803272788";
            var resulte = IdCardValidatorHelper.IsNewIdCard(idCardRightNumber);
            resulte.IsTrue();

            var idCardErrorNumber = "6328021978032727";
            var errorResulte = IdCardValidatorHelper.IsNewIdCard(idCardErrorNumber);
            errorResulte.IsFalse();
        }

        /// <summary>
        /// 获取身份证生日时间
        /// </summary>
        public void GetBirthdayTest()
        {
            var idCardRightNumber = "632802197803272788";
            var resulte = IdCardValidatorHelper.GetBirthday(idCardRightNumber);
            resulte.IsEqualTo("19780327");
        }

        /// <summary>
        /// 根据身份证号获得性别
        /// </summary>
        public void GetSexByIdCardTest()
        {
            var idCardRightNumber = "632802197803272788";
            DateTime brityday;
            var resulte = IdCardValidatorHelper.GetSexByIdCard(idCardRightNumber, out brityday);
            resulte.IsEqualTo(0);
        }

        /// <summary>
        /// 验证身份证合理性
        /// </summary>
        public void CheckIdCardTest()
        {
            var idCardRightNumber = "632802197803272788"; var resulte = IdCardValidatorHelper.CheckIdCard(idCardRightNumber);
            resulte.IsTrue();

            var idCardErrorNumber = "6328021978032727";
            var errorResulte = IdCardValidatorHelper.CheckIdCard(idCardErrorNumber);
            errorResulte.IsFalse();
        }
        #endregion

        #region [4 DataTypeConvertHelperTest]

        /// <summary>
        /// 汉子转拼音
        /// </summary>
        public void GetFullPinyinTest()
        {
            var testChinese = "秦先生";
            var testPinyin = "QinXianSheng";
            var resulteInfo = DataTypeConvertHelper.GetFullPinyin(testChinese);
            resulteInfo.IsEqualTo(testPinyin);
            resulteInfo = DataTypeConvertHelper.GetEachFirstLetterPinyin(testChinese);
            resulteInfo.IsEqualTo("QXS");
        }

        /// <summary>
        /// 数值换为汉字
        /// </summary>
        public void ToChineseTest()
        {
            var number = 1234567;
            var numberChine = "一百二十三万四千五百六十七";
            var numberOld = "壹佰贰拾叁萬肆仟伍佰陆拾柒";
            var resulteInfo = DataTypeConvertHelper.ToChinese(number, false);
            resulteInfo.IsEqualTo(numberChine);
            resulteInfo = DataTypeConvertHelper.ToChinese(number, true);
            resulteInfo.IsEqualTo(numberOld);
        }
        #endregion

        #region [5 Test DataTable to list]

        public void DataTableToList()
        {
            var testList = new List<TestsTabelToListObject>
            {
                new TestsTabelToListObject
                {
                    Age = 10,
                    Height = 20.907,
                    Name = "qinxianbo",
                    Right = true,
                    Sex = EnumSex.boy,
                    YouLong = new TimeSpan(1, 1, 1, 1)
                },
                new TestsTabelToListObject
                {
                    Age =23,
                    Height = 234.907,
                    Name = "秦先波",
                    Right = true,
                    Sex = EnumSex.boy,
                    YouLong = new TimeSpan(1, 1, 1, 2)
                },
                new TestsTabelToListObject
                {
                    Age = 40,
                    Height = 20.907,
                    Name = "qinxianbo",
                    Right = true,
                    Sex = EnumSex.boy,
                    YouLong = new TimeSpan(1, 1, 1, 3)
                },
                new TestsTabelToListObject
                {
                    Height = 20.907,
                    Name = "杨宏俊",
                    Right = true,
                    Sex = EnumSex.grily,
                    YouLong = new TimeSpan(1, 1, 1, 4)
                },
                new TestsTabelToListObject
                {
                    Age = 10,
                    Name = "k",
                    Height = 20.907,
                    Right = true,
                    Sex = EnumSex.boy,
                    YouLong = new TimeSpan(1, 1, 1, 5)
                }
            };
            var table = DataTypeConvertHelper.ToDataTable(testList);
            var npoi = new NpoiHelper("TestTable.xlsx");
            npoi.DataTableToExcel(table, null, true);
            string errorMessage;
            var tableFile = npoi.ExcelToDataTable(0, true, out errorMessage);
            foreach (DataRow dataRow in tableFile.Rows)
            {
                dataRow.ItemArray.ToList().ForEach(f =>
                {
                    Console.Write(f);
                    Console.Write("  ");
                });
                Console.Write("\r\n");
            }
            var backListChange = DataTypeConvertHelper.ToList<TestsTabelToListObject>(tableFile);
        }
        #endregion
    }

    public class TestsTabelToListObject
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        public int Age { get; set; }

        public double Height { get; set; }

        public EnumSex Sex { get; set; }

        public TimeSpan YouLong { get; set; }

        public bool Right { get; set; }
    }

    public enum EnumSex
    {
        boy,
        grily
    }
}
