#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNetTest.Email
//文件名称：EmailHelperTest
//创 建 人：FreshMan
//创建日期：2017/7/8 14:04:15
//用    途：记录类的用途
//======================================================================
#endregion

using FreshCommonUtility.Email;

namespace FreshCommonUtilityNetTest.Email
{
    public class EmailHelperTest
    {
        /// <summary>
        /// 同步发送
        /// </summary>
        public void SendEmail()
        {
            var toEmail = "******";
            var message = "这是同步测试文件";
            var subject = "这里是主题";
            
            EmailHelper.SendEmail(toEmail, subject, message);
        }

        public void SendEmailAsync()
        {
            var toEmail = "*******";
            var message = "这是异步测试文件";
            var subject = "这里异步是主题";

            EmailHelper.SendEmailAsync(toEmail, subject, message);
        }
    }
}
