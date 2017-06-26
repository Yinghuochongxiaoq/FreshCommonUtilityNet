using System;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Security
{
    /// <summary>
    /// 随机数生成
    /// </summary>
    public class RandomHelper
    {
        private static long _seed = DateTime.Now.Ticks;////随机种子
        /// <summary>
        /// 获取随机码
        /// </summary>
        /// <param name="randomLen"></param>
        /// <param name="randomChars"></param>
        /// <returns></returns>
        public static string GetRandomNo(int randomLen, string randomChars)
        {
            if (randomLen < 1 || string.IsNullOrEmpty(randomChars))
            {
                return string.Empty;
            }
            var randomNo = string.Empty;
            for (var i = 0; i < randomLen; i++)
            {
                var random = new Random((int)_seed);////随机种子，确保相对比较真实
                var randomChar = random.Next(randomChars.Length);
                randomNo += randomChars[randomChar];
                _seed = (DateTime.Now.Ticks + _seed);
            }
            return randomNo;
        }

        /// <summary>
        /// 混合随机数 ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789  其中字母O去除掉，容易和数字0 混淆客户不好辨认
        /// </summary>
        /// <param name="randomLen"></param>
        /// <returns></returns>
        public static string GetRandomNo(int randomLen)
        {
            return GetRandomNo(randomLen, "ABCDEFGHIJKLMNPQRSTUVWXYZ0123456789");
        }

        /// <summary>
        /// 字母随机数 ABCDEFGHIJKLMNOPQRSTUVWXYZ
        /// </summary>
        /// <param name="randomLen"></param>
        /// <returns></returns>
        public static string GetRandomLetterNo(int randomLen)
        {
            return GetRandomNo(randomLen, "AaBbCcDdEeFfGgHhIiJjKkLMmNnPpQqRrSsTtUuVvWwXxYyZz");
        }

        /// <summary>
        /// 数字随机数 0123456789
        /// </summary>
        /// <param name="randomLen"></param>
        /// <returns></returns>
        public static string GetRandomNumbersNo(int randomLen)
        {
            return GetRandomNo(randomLen, "0123456789");
        }
    }
}
