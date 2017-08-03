using System;
using System.Text;
using FreshCommonUtility.RegExp;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.ExpandMath
{
    /// <summary>
    /// Big int 
    /// </summary>
    public static class BigInt
    {
        /// <summary>
        /// Integer multiply
        /// </summary>
        /// <param name="num1"></param>
        /// <param name="num2"></param>
        /// <returns></returns>
        public static string IntegerMultiply(string num1, string num2)
        {
            if (!RegExpHelper.IsNumeric(num1))
                throw new ArgumentException(nameof(num1) + $" ={num1} have non-integer numeric,please check.");
            if (!RegExpHelper.IsNumeric(num2))
                throw new ArgumentException(nameof(num2) + $" ={num2} have non-integer numeric,please check.");
            var oneFlag = RegExpHelper.IsLeftZeroNumeric(num1);
            var twoFlag = RegExpHelper.IsLeftZeroNumeric(num2);
            num1 = oneFlag ? num1.Substring(1) : num1;
            num2 = twoFlag ? num2.Substring(1) : num2;
            var leftZero = ((oneFlag ? 1 : 0) + (twoFlag ? 1 : 0)) % 2 == 1;
            int m = num1.Length, n = num2.Length;
            int[] pos = new int[m + n];

            for (int i = m - 1; i >= 0; i--)
            {
                for (int j = n - 1; j >= 0; j--)
                {
                    int mul = (num1[i] - '0') * (num2[j] - '0');
                    int p1 = i + j, p2 = i + j + 1;
                    int sum = mul + pos[p2];

                    pos[p1] += sum / 10;
                    pos[p2] = (sum) % 10;
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (int p in pos) if (!(sb.Length == 0 && p == 0)) sb.Append(p);
            return sb.Length == 0 ? "0" : (leftZero ? "-" : "") + sb;
        }
    }
}
