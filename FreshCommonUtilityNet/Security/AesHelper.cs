using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Security
{
    /// <summary>
    /// AES加密解密
    /// </summary>
    public class AesHelper
    {
        /// <summary>
        /// 加密密钥
        /// </summary>
        public static readonly string SECRET = "Z3P54EKJGM6WS90O";

        /// <summary>
        /// 加密偏移量
        /// </summary>
        public static readonly string Offset = "R8QIDACU1Y7LXTFV";

        #region AES加密&解密

        /// <summary>AES加密方法 128位加密</summary>
        /// <param name="text">明文</param>
        /// <param name="key">密钥,长度为16的字符串</param>
        /// <param name="iv">偏移量,长度为16的字符串</param>
        /// <returns>密文</returns>
        public static string AesEncrypt(string text, string key = null, string iv = null)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    key = SECRET;
                }
                if (string.IsNullOrEmpty(iv))
                {
                    iv = Offset;
                }
                RijndaelManaged rijndaelCipher = new RijndaelManaged();

                rijndaelCipher.Mode = CipherMode.CBC;       //密码模式
                rijndaelCipher.Padding = PaddingMode.Zeros;  //填充模式
                rijndaelCipher.KeySize = 128;               //密钥为128位
                rijndaelCipher.BlockSize = 128;
                byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
                byte[] keyBytes = new byte[16];
                int len = pwdBytes.Length;
                if (len > 16)
                    len = keyBytes.Length;
                Array.Copy(pwdBytes, keyBytes, len);  //复制key16位作为密钥

                rijndaelCipher.Key = keyBytes;               //加密密钥

                byte[] ivBytes = Encoding.UTF8.GetBytes(iv); //偏移向量
                byte[] ivBytesNew = new byte[16];
                len = ivBytes.Length;
                if (len > 16)
                    len = ivBytesNew.Length;
                Array.Copy(ivBytes, ivBytesNew, len);  //复制IV16位作为密钥
                rijndaelCipher.IV = ivBytesNew;

                ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(text);
                byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
                // return Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);

                return ByteToHexStr(cipherBytes);

            }
            catch (IOException) { throw; }
            catch (CryptographicException) { throw; }
            catch (ArgumentException) { throw; }
            // ReSharper disable once PossibleIntendedRethrow
            catch (Exception ex) { throw ex; }
        }

        /// <summary>AES解密</summary>
        /// <param name="text">密文</param>
        /// <param name="key">密钥,长度为16的字符串</param>
        /// <param name="iv">偏移量,长度为16的字符串</param>
        /// <returns>明文</returns>
        public static string AesDecrypt(string text, string key = null, string iv = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = SECRET;
            }
            if (string.IsNullOrEmpty(iv))
            {
                iv = Offset;
            }
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.Zeros;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = HexStrToByte(text);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = Encoding.UTF8.GetBytes(iv); //偏移向量
            byte[] ivBytesNew = new byte[16];
            len = ivBytes.Length;
            if (len > 16)
                len = ivBytesNew.Length;
            Array.Copy(ivBytes, ivBytesNew, len);  //复制IV16位作为密钥
            rijndaelCipher.IV = ivBytesNew;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText).Replace("\0", "");
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] HexStrToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("x2");
                }
            }
            return returnStr;
        }
        #endregion
    }
}
