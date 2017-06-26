using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Security
{
    /// <summary>
    /// Des secret helper.
    /// </summary>
    public class DesHelper
    {
        /// <summary>
        /// 默认密钥向量 
        /// </summary>
        private static readonly byte[] Keys = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

        /// <summary>
        /// 默认私钥
        /// </summary>
        private static string _privateKey = "#ImF3mh$27*7n7!H";

        /// <summary> 
        /// DES加密字符串 
        /// </summary> 
        /// <param name="encryptString">待加密的字符串</param> 
        /// <param name="encryptKey">加密密钥,要求为16位</param> 
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string DesEnCode(string encryptString, string encryptKey = null)
        {
            try
            {
                _privateKey = string.IsNullOrEmpty(encryptKey) ? _privateKey : encryptKey;
                byte[] rgbKey = Encoding.UTF8.GetBytes(_privateKey.Substring(0, 16));
                byte[] rgbIv = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                var dcsp = Aes.Create();
                MemoryStream mStream = new MemoryStream();
                if (dcsp != null)
                {
                    CryptoStream cStream = new CryptoStream(mStream, dcsp.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();
                }
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message + encryptString;
            }

        }

        /// <summary> 
        /// DES解密字符串 
        /// </summary> 
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为16位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DesDeCode(string decryptString, string decryptKey = null)
        {
            try
            {
                _privateKey = string.IsNullOrEmpty(decryptKey) ? _privateKey : decryptKey;
                byte[] rgbKey = Encoding.UTF8.GetBytes(_privateKey.Substring(0, 16));
                byte[] rgbIv = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                var dcsp = Aes.Create();
                MemoryStream mStream = new MemoryStream();
                if (dcsp != null)
                {
                    CryptoStream cStream = new CryptoStream(mStream, dcsp.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                    cStream.Write(inputByteArray, 0, inputByteArray.Length);
                    cStream.FlushFinalBlock();
                }
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message + decryptString;
            }
        }
    }
}
