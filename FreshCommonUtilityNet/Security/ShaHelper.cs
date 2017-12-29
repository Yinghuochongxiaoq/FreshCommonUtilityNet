#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.Security
//文件名称：ShaHelper
//创 建 人：FreshMan
//创建日期：2017/12/29 18:33:55
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Security.Cryptography;
using System.Text;

namespace FreshCommonUtilityNet.Security
{
    /// <summary>
    /// Sha加密方法
    /// </summary>
    public class ShaHelper
    {
        /// <summary>
        /// Sha1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <returns>返回40位UTF8 大写</returns>
        public static string Sha1(string content)
        {
            return Sha1(content, Encoding.UTF8);
        }

        /// <summary>
        /// Sha1 加密，返回大写字符串
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <param name="encode">指定加密编码</param>
        /// <returns>返回40位大写字符串</returns>
        public static string Sha1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytesIn = encode.GetBytes(content);
                byte[] bytesOut = sha1.ComputeHash(bytesIn);
                sha1.Dispose();
                string result = BitConverter.ToString(bytesOut);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }
    }
}
