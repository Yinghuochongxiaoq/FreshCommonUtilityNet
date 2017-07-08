#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest.Security
//文件名称：IdCardValidatorHelperTest
//创 建 人：FreshMan
//创建日期：2017/7/8 14:19:56
//用    途：记录类的用途
//======================================================================
#endregion
using System;
using FreshCommonUtility.Security;

namespace FreshCommonUtilityNetTest.Security
{
    public class SecurityTest
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
    }
}
