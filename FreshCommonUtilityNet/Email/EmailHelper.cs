#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtility.Email
//文件名称：EmailHelper
//创 建 人：FreshMan
//创建日期：2017/7/8 14:19:56
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using FreshCommonUtility.Configure;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Email
{
    /// <summary>
    /// Email helper.
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// Send email method.
        /// </summary>
        /// <param name="toEmailAddress">send to email address.</param>
        /// <param name="subject">email subject.</param>
        /// <param name="message">send email content,txt or html boy,if html body ,you should set isHtmlBody param is true.</param>
        /// <param name="toName">send to email name,could by null,if null will use default string.</param>
        /// <param name="isHtmlBody">flag the send email </param>
        public static void SendEmail(string toEmailAddress, string subject, string message, string toName = null, bool isHtmlBody = false)
        {
            if (string.IsNullOrEmpty(toEmailAddress)) throw new ArgumentNullException(nameof(toEmailAddress));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            var mailUserName = AppConfigurationHelper.GetString("FromName");
            var fromEmailAddress = AppConfigurationHelper.GetString("FromEmailAddress");
            var mailServer = AppConfigurationHelper.GetString("EmailSmtpServerAddress");
            var mailPassword = AppConfigurationHelper.GetString("FromEmailPassword");
            if (string.IsNullOrEmpty(fromEmailAddress)
                || string.IsNullOrEmpty(mailPassword)
                || string.IsNullOrEmpty(mailServer))
            {
                throw new Exception("Email config error,please check you config.FromEmailAddress,EmailSmtpServerAddress,FromEmailPassword filed is must have.");
            }
            MailMessage messageModel = new MailMessage();
            // 接收人邮箱地址
            messageModel.To.Add(new MailAddress(toEmailAddress));
            messageModel.From = new MailAddress(fromEmailAddress, mailUserName);
            messageModel.BodyEncoding = Encoding.GetEncoding("UTF-8");
            messageModel.Body = message;
            //GB2312
            messageModel.SubjectEncoding = Encoding.GetEncoding("UTF-8");
            messageModel.Subject = subject;
            messageModel.IsBodyHtml = isHtmlBody;

            SmtpClient smtpclient = new SmtpClient(mailServer, 25)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mailUserName, mailPassword)
            };
            //SSL连接
            try
            {
                smtpclient.Send(messageModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Send email method.
        /// </summary>
        /// <param name="toEmailAddress">send to email address.</param>
        /// <param name="subject">email subject.</param>
        /// <param name="message">send email content,txt or html boy,if html body ,you should set isHtmlBody param is true.</param>
        /// <param name="toName">send to email name,could by null,if null will use default string.</param>
        /// <param name="isHtmlBody">flag the send email </param>
        public static void SendEmailAsync(string toEmailAddress, string subject, string message,
            string toName = null, bool isHtmlBody = false)
        {
            if (string.IsNullOrEmpty(toEmailAddress)) throw new ArgumentNullException(nameof(toEmailAddress));
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
            var mailUserName = AppConfigurationHelper.GetString("FromName");
            var fromEmailAddress = AppConfigurationHelper.GetString("FromEmailAddress");
            var mailServer = AppConfigurationHelper.GetString("EmailSmtpServerAddress");
            var mailPassword = AppConfigurationHelper.GetString("FromEmailPassword");
            if (string.IsNullOrEmpty(fromEmailAddress)
                || string.IsNullOrEmpty(mailPassword)
                || string.IsNullOrEmpty(mailServer))
            {
                throw new Exception("Email config error,please check you config.FromEmailAddress,EmailSmtpServerAddress,FromEmailPassword filed is must have.");
            }
            new Thread(delegate ()
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = mailServer,
                    Port = 25,
                    Credentials = new NetworkCredential(fromEmailAddress, mailPassword)
                };
                //邮箱的smtp地址
                //端口号
                //构建发件人的身份凭据类
                //构建消息类
                MailMessage objMailMessage = new MailMessage
                {
                    //设置优先级
                    Priority = MailPriority.High,
                    //消息发送人
                    From = new MailAddress(fromEmailAddress, mailUserName, Encoding.UTF8)
                };
                //收件人
                objMailMessage.To.Add(toEmailAddress);
                //标题
                objMailMessage.Subject = subject.Trim();
                //标题字符编码
                objMailMessage.SubjectEncoding = Encoding.UTF8;
                //正文
                objMailMessage.Body = message;
                objMailMessage.IsBodyHtml = true;
                //内容字符编码
                objMailMessage.BodyEncoding = Encoding.UTF8;
                //发送
                smtp.Send(objMailMessage);
            }).Start();
        }

        /// <summary>
        /// Chech email address.
        /// </summary>
        /// <param name="emailAddress">email address.</param>
        /// <returns>this email address is valid.</returns>
        public static bool IsEmailAddress(string emailAddress)
        {
            var regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase);
            return regex.IsMatch(emailAddress);
        }
    }
}
