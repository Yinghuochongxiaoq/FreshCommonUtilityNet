using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using System.Text;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.DataConvert
{
    /// <summary>
    /// Data convert
    /// </summary>
    public static class DataTypeConvertHelper
    {
        #region [1、数据类型转换]
        /// <summary>
        /// convert to bool
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool ToBool(object val)
        {
            if ((val == null) || (val == DBNull.Value))
            {
                return false;
            }
            if (val is bool)
            {
                return (bool)val;
            }
            return ((val.ToString().ToLower() == "true") || (val.ToString().ToLower() == "1"));
        }

        /// <summary>
        /// 转换成byte
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte ToByte(object val)
        {
            return ToByte(val, 0);
        }

        /// <summary>
        /// 转换成byte
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte ToByte(object val, byte defaultValue)
        {
            byte num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is byte)
            {
                return (byte)val;
            }
            return !byte.TryParse(val.ToString(), out num) ? defaultValue : num;
        }

        /// <summary>
        /// 转换成byte?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte? ToByteNullable(object val)
        {
            var num = ToByte(val);
            if (num.Equals(0))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成DateTime，转换失败返回1900-1-1
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(object val)
        {
            DateTime time;
            if ((val == null) || (val == DBNull.Value))
            {
                return new DateTime(0x76c, 1, 1);
            }
            if (val is DateTime)
            {
                return (DateTime)val;
            }
            return !DateTime.TryParse(val.ToString(), out time) ? new DateTime(0x76c, 1, 1) : time;
        }

        /// <summary>
        /// 转换成DateTime?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static DateTime? ToDateTimeNullable(object val)
        {
            var time = ToDateTime(val);
            if (time.Equals(new DateTime(0x76c, 1, 1)))
            {
                return null;
            }
            return time;
        }

        /// <summary>
        /// 转换为DateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue">返回的默认值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(object obj, DateTime defaultValue)
        {
            var result = defaultValue;
            if (obj == null)
            {
                return result;
            }
            if (!DateTime.TryParse(obj.ToString().Trim(), out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        ///  根据数据日期类型 转化日期
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <param name="dateFormat">输入日期格式 比如  yyyyMMdd</param>
        /// <returns></returns>
        public static DateTime ToDateTime(object obj, DateTime defaultValue, string dateFormat)
        {
            var result = defaultValue;

            if (obj == null)
            {
                return result;
            }
            //日期验证
            IFormatProvider ifp = new CultureInfo("zh-TW");
            DateTime.TryParseExact(obj.ToString(), dateFormat, ifp, DateTimeStyles.None, out result);
            return result;
        }

        /// <summary>
        /// 转换成decimal 默认保留2位小数点
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object val)
        {
            return ToDecimal(val, 0M, 2);
        }

        /// <summary>
        /// 转换成decimal
        /// </summary>
        /// <param name="val"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object val, int decimals)
        {
            return ToDecimal(val, 0M, decimals);
        }

        /// <summary>
        /// 转换成decimal
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal ToDecimal(object val, decimal defaultValue, int decimals)
        {
            var result = defaultValue;
            if (val == null || val == DBNull.Value)
            {
                return result;
            }
            if (!decimal.TryParse(val.ToString().Trim(), out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换成decimal?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal? ToDecimalNullable(object val)
        {
            decimal num = ToDecimal(val);
            if (num.Equals(0.0M))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成double 默认保留2位小数点
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double ToDouble(object val)
        {
            return ToDouble(val, 0.0, 2);
        }

        /// <summary>
        /// 转换成double
        /// </summary>
        /// <param name="val"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double ToDouble(object val, int digits)
        {
            return ToDouble(val, 0.0, digits);
        }

        /// <summary>
        /// 转换成double
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static double ToDouble(object val, double defaultValue, int digits)
        {
            double num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is double)
            {
                return Math.Round((double)val, digits);
            }
            if (!double.TryParse(val.ToString(), out num))
            {
                return defaultValue;
            }
            return Math.Round(num, digits);
        }

        /// <summary>
        /// 转换成double?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double? ToDoubleNullable(object val)
        {
            double num = ToDouble(val);
            if (num.Equals(0.0))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成float 默认保留2位小数点
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ToFloat(object val)
        {
            return ToFloat(val, 0f);
        }

        /// <summary>
        /// 转换成float
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ToFloat(object val, float defaultValue)
        {
            float num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is float)
            {
                return (float)val;
            }
            if (!float.TryParse(val.ToString(), out num))
            {
                return defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 转换成float?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float? ToFloatNullable(object val)
        {
            float num = ToFloat(val);
            if (num.Equals(0f))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成int
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ToInt(object val)
        {
            return ToInt(val, 0);
        }

        /// <summary>
        /// 转换成int
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(object val, int defaultValue)
        {
            int num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is int)
            {
                return (int)val;
            }
            if (!int.TryParse(val.ToString().Trim(), NumberStyles.Number, null, out num))
            {
                return defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 转换成int?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int? ToIntNullable(object val)
        {
            int num = ToInt(val);
            if (num.Equals(0))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成long
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long ToLong(object val)
        {
            return ToLong(val, 0L);
        }

        /// <summary>
        /// 转换成long
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(object val, long defaultValue)
        {
            long num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is long)
            {
                return (long)val;
            }
            if (!long.TryParse(val.ToString(), out num))
            {
                return defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 转换成long?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static long? ToLongNullable(object val)
        {
            long num = ToLong(val);
            if (num.Equals(0L))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成sbyte
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static sbyte ToSbyte(object val)
        {
            return ToSbyte(val, 0);
        }

        /// <summary>
        /// 转换成sbyte
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static sbyte ToSbyte(object val, sbyte defaultValue)
        {
            sbyte num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is sbyte)
            {
                return (sbyte)val;
            }
            if (!sbyte.TryParse(val.ToString(), out num))
            {
                return defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 转换成sbyte?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static sbyte? ToSbyteNullable(object val)
        {
            sbyte num = ToSbyte(val);
            if (num.Equals(0))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成short
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static short ToShort(object val)
        {
            return ToShort(val, 0);
        }

        /// <summary>
        /// 转换成short
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ToShort(object val, short defaultValue)
        {
            short num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is short)
            {
                return (short)val;
            }
            if (!short.TryParse(val.ToString(), out num))
            {
                return defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 转换成short?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static short? ToShortNullable(object val)
        {
            short num = ToShort(val);
            if (num.Equals(0))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成string
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToString(object val)
        {
            if ((val == null) || (val == DBNull.Value))
            {
                return string.Empty;
            }
            if (val.GetType() == typeof(byte[]))
            {
                return Encoding.ASCII.GetString((byte[])val, 0, ((byte[])val).Length);
            }
            return val.ToString();
        }

        /// <summary>
        /// 转换成string
        /// </summary>
        /// <param name="val"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string ToString(object val, string replace)
        {
            string str = ToString(val);
            return (string.IsNullOrEmpty(str) ? replace : str);
        }

        /// <summary>
        /// 转换成string
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToStringNullable(object val)
        {
            string str = ToString(val);
            return (string.IsNullOrEmpty(str) ? null : str);
        }

        /// <summary>
        /// 转换成uint
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static uint ToUint(object val)
        {
            return ToUint(val, 0);
        }

        /// <summary>
        /// 转换成uint
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static uint ToUint(object val, uint defaultValue)
        {
            uint num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is uint)
            {
                return (uint)val;
            }
            if (!uint.TryParse(val.ToString(), out num))
            {
                return defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 转换成uint?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static uint? ToUintNullable(object val)
        {
            uint num = ToUint(val);
            if (num.Equals(0))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 转换成ushort
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ushort ToUshort(object val)
        {
            return ToUshort(val, 0);
        }

        /// <summary>
        /// 转换成ushort
        /// </summary>
        /// <param name="val"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static ushort ToUshort(object val, ushort defaultValue)
        {
            ushort num;
            if ((val == null) || (val == DBNull.Value))
            {
                return defaultValue;
            }
            if (val is ushort)
            {
                return (ushort)val;
            }
            if (!ushort.TryParse(val.ToString(), out num))
            {
                return defaultValue;
            }
            return num;
        }

        /// <summary>
        /// 转换成ushort?
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static ushort? ToUshortNullable(object val)
        {
            ushort num = ToUshort(val);
            if (num.Equals(0))
            {
                return null;
            }
            return num;
        }

        /// <summary>
        /// 根据日期获取星期几
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToWeekByDate(DateTime date)
        {
            string weekstr = date.DayOfWeek.ToString();
            switch (weekstr)
            {
                case "Monday":
                    weekstr = "星期一";
                    break;
                case "Tuesday":
                    weekstr = "星期二";
                    break;
                case "Wednesday":
                    weekstr = "星期三";
                    break;
                case "Thursday":
                    weekstr = "星期四";
                    break;
                case "Friday":
                    weekstr = "星期五";
                    break;
                case "Saturday":
                    weekstr = "星期六";
                    break;
                case "Sunday":
                    weekstr = "星期日";
                    break;
            }
            return weekstr;
        }

        /// <summary>
        /// 根据日期获取周几
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToWeekDescByDate(DateTime date)
        {
            string weekstr = date.DayOfWeek.ToString();
            switch (weekstr)
            {
                case "Monday":
                    weekstr = "周一";
                    break;
                case "Tuesday":
                    weekstr = "周二";
                    break;
                case "Wednesday":
                    weekstr = "周三";
                    break;
                case "Thursday":
                    weekstr = "周四";
                    break;
                case "Friday":
                    weekstr = "周五";
                    break;
                case "Saturday":
                    weekstr = "周六";
                    break;
                case "Sunday":
                    weekstr = "周日";
                    break;
            }
            return weekstr;
        }

        /// <summary>
        /// http协议转化成Https协议  实例：http:// to  https://
        /// </summary>
        /// <returns></returns>
        public static string ConvertHttpToHttps(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return string.Empty;
            }
            if (val.ToLower().Contains("http://"))
            {
                val = "//" + val.Substring(7, val.Length - 7);
            }
            return val;
        }

        /// <summary>
        /// string类型数组转化为int类型数组
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static int[] StringsToInts(string[] strs)
        {
            //处理字符串数据为空情况
            int[] ints = new int[0];
            if (strs != null && strs.Any())
            {
                ints = new int[strs.Length];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = ToInt(strs[i], 0);
                }
                return ints;
            }
            return ints;
        }

        /// <summary>
        ///     把string转化成listint
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<int> StringToListInt(string value, char separator)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            var list = new List<int>();
            foreach (var item in value.Split(separator))
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                var id = ToInt(item);
                if (list.Contains(id))
                {
                    continue;
                }
                list.Add(id);
            }

            return list;
        }

        /// <summary>
        /// 把List转化成string类型
        /// </summary>
        /// <param name="valList">字符串</param>
        /// <param name="separator">分隔符</param>
        /// <param name="nDefault">默认值</param>
        /// <returns></returns>
        public static string ListIntToString(List<int> valList, char separator = ',', string nDefault = "")
        {
            if (valList == null || !valList.Any())
            {
                return nDefault;
            }

            var str = valList.Aggregate(string.Empty, (current, item) => current + (item + separator.ToString()));
            return str.TrimEnd(separator);
        }

        /// <summary>
        /// 字符串加*
        /// </summary>
        /// <param name="str">数据源</param>
        /// <param name="leftLength">左边取值</param>
        /// <param name="rightLength">右边取值</param>
        /// <param name="encryptType">加密字符</param>
        /// <returns></returns>
        public static string ConvertStrToEncrypt(string str, int leftLength, int rightLength, string encryptType = "*")
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (leftLength < 1)
            {
                leftLength = 1;
            }
            if (rightLength < 1)
            {
                rightLength = 1;
            }
            var length = leftLength + rightLength;
            var strLength = str.Length;
            if (strLength <= length)
            {
                return str;
            }
            if (string.IsNullOrEmpty(encryptType))
            {
                encryptType = "*";
            }
            var strLeft = str.Substring(0, leftLength);
            var strRight = str.Substring(strLength - rightLength);
            var strMid = string.Empty;
            var midLength = strLength - length;

            if (midLength > 0)
            {
                for (var i = 0; i < midLength; i++)
                {
                    strMid += encryptType;
                }
            }
            return strLeft + strMid + strRight;
        }

        /// <summary>
        /// 实体转化成键值对信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="listPara"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<string, string>> EntityToKeyValuePairs<TEntity>(IList<KeyValuePair<string, string>> listPara, TEntity entity)
        {
            var list = new List<KeyValuePair<string, string>>();
            if (entity == null)
            {
                return list;
            }
            var proList = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (!proList.Any())
            {
                return list;
            }
            if (listPara == null)
            {
                listPara = new List<KeyValuePair<string, string>>();
            }
            foreach (var item in proList)
            {
                if (!item.CanRead)
                {
                    continue;
                }
                var format = string.Empty;
                var formatAttribute = (FormatAttribute)item.GetCustomAttribute(typeof(FormatAttribute));
                if (formatAttribute != null)
                {
                    format = formatAttribute.Format;
                }
                var key = item.Name;
                var value = (item.GetValue(entity, null) ?? string.Empty);
                var valueFormat = string.IsNullOrEmpty(format) ? value.ToString() : string.Format(format, value);
                list.Add(new KeyValuePair<string, string>(key, valueFormat));
                listPara.Add(new KeyValuePair<string, string>(key, valueFormat));
            }
            return list;
        }
        #endregion

        #region [2、汉字装换]
        /// <summary>
        /// 数字转化成汉字的方法
        /// </summary>
        /// <param name="n">数字</param>
        /// <param name="fang">是否返回繁体字</param>
        /// <returns></returns>
        public static string ToChinese(int n, bool fang)
        {
            string strn = n.ToString();
            string str = "";
            string nn = "零壹贰叁肆伍陆柒捌玖";
            string ln = "零一二三四五六七八九";

            string mm = "  拾佰仟萬拾佰仟亿拾佰仟萬兆拾佰仟萬亿";
            string lm = "  十百千万十百千亿十百千万兆十百千万亿";

            int i = 0;
            while (i < strn.Length) //>>>>>>>>>>>>>>>>出现空格
            {

                int m = int.Parse(strn.Substring(i, 1));
                if (fang) //返回繁体字
                {
                    str += nn.Substring(m, 1);
                    if (lm.Substring(strn.Length - i, 1) != " ")
                    {
                        str += mm.Substring(strn.Length - i, 1);
                    }
                }
                else //返回简体字
                {
                    str += ln.Substring(m, 1);
                    if (lm.Substring(strn.Length - i, 1) != " ")
                    {
                        str += lm.Substring(strn.Length - i, 1);
                    }
                }
                i++;
            }
            if (str.Substring(str.Length - 1) == "零")
            {
                str = str.Substring(0, str.Length - 1);
            }
            if (str.Length > 1 && str.Substring(0, 2) == "一十")
            {
                str = str.Substring(1);
            }
            if (str.Length > 1 && str.Substring(0, 2) == "壹拾")
            {
                str = str.Substring(1);
            }

            return str;
        }

        #region 数组信息

        /// <summary>
        /// 一级汉字对应编码
        /// </summary>
        private static readonly int[] PyValue = {
            -20319, -20317, -20304, -20295, -20292, -20283, -20265, -20257, -20242,

            -20230, -20051, -20036, -20032, -20026, -20002, -19990, -19986, -19982,

            -19976, -19805, -19784, -19775, -19774, -19763, -19756, -19751, -19746,

            -19741, -19739, -19728, -19725, -19715, -19540, -19531, -19525, -19515,

            -19500, -19484, -19479, -19467, -19289, -19288, -19281, -19275, -19270,

            -19263, -19261, -19249, -19243, -19242, -19238, -19235, -19227, -19224,

            -19218, -19212, -19038, -19023, -19018, -19006, -19003, -18996, -18977,

            -18961, -18952, -18783, -18774, -18773, -18763, -18756, -18741, -18735,

            -18731, -18722, -18710, -18697, -18696, -18526, -18518, -18501, -18490,

            -18478, -18463, -18448, -18447, -18446, -18239, -18237, -18231, -18220,

            -18211, -18201, -18184, -18183, -18181, -18012, -17997, -17988, -17970,

            -17964, -17961, -17950, -17947, -17931, -17928, -17922, -17759, -17752,

            -17733, -17730, -17721, -17703, -17701, -17697, -17692, -17683, -17676,

            -17496, -17487, -17482, -17468, -17454, -17433, -17427, -17417, -17202,

            -17185, -16983, -16970, -16942, -16915, -16733, -16708, -16706, -16689,

            -16664, -16657, -16647, -16474, -16470, -16465, -16459, -16452, -16448,

            -16433, -16429, -16427, -16423, -16419, -16412, -16407, -16403, -16401,

            -16393, -16220, -16216, -16212, -16205, -16202, -16187, -16180, -16171,

            -16169, -16158, -16155, -15959, -15958, -15944, -15933, -15920, -15915,

            -15903, -15889, -15878, -15707, -15701, -15681, -15667, -15661, -15659,

            -15652, -15640, -15631, -15625, -15454, -15448, -15436, -15435, -15419,

            -15416, -15408, -15394, -15385, -15377, -15375, -15369, -15363, -15362,

            -15183, -15180, -15165, -15158, -15153, -15150, -15149, -15144, -15143,

            -15141, -15140, -15139, -15128, -15121, -15119, -15117, -15110, -15109,

            -14941, -14937, -14933, -14930, -14929, -14928, -14926, -14922, -14921,

            -14914, -14908, -14902, -14894, -14889, -14882, -14873, -14871, -14857,

            -14678, -14674, -14670, -14668, -14663, -14654, -14645, -14630, -14594,

            -14429, -14407, -14399, -14384, -14379, -14368, -14355, -14353, -14345,

            -14170, -14159, -14151, -14149, -14145, -14140, -14137, -14135, -14125,

            -14123, -14122, -14112, -14109, -14099, -14097, -14094, -14092, -14090,

            -14087, -14083, -13917, -13914, -13910, -13907, -13906, -13905, -13896,

            -13894, -13878, -13870, -13859, -13847, -13831, -13658, -13611, -13601,

            -13406, -13404, -13400, -13398, -13395, -13391, -13387, -13383, -13367,

            -13359, -13356, -13343, -13340, -13329, -13326, -13318, -13147, -13138,

            -13120, -13107, -13096, -13095, -13091, -13076, -13068, -13063, -13060,

            -12888, -12875, -12871, -12860, -12858, -12852, -12849, -12838, -12831,

            -12829, -12812, -12802, -12607, -12597, -12594, -12585, -12556, -12359,

            -12346, -12320, -12300, -12120, -12099, -12089, -12074, -12067, -12058,

            -12039, -11867, -11861, -11847, -11831, -11798, -11781, -11604, -11589,

            -11536, -11358, -11340, -11339, -11324, -11303, -11097, -11077, -11067,

            -11055, -11052, -11045, -11041, -11038, -11024, -11020, -11019, -11018,

            -11014, -10838, -10832, -10815, -10800, -10790, -10780, -10764, -10587,

            -10544, -10533, -10519, -10331, -10329, -10328, -10322, -10315, -10309,

            -10307, -10296, -10281, -10274, -10270, -10262, -10260, -10256, -10254

        };

        /// <summary>
        /// 对应拼音
        /// </summary>
        private static readonly string[] PyName = {
            "A", "Ai", "An", "Ang", "Ao", "Ba", "Bai", "Ban", "Bang", "Bao", "Bei",

            "Ben", "Beng", "Bi", "Bian", "Biao", "Bie", "Bin", "Bing", "Bo", "Bu",

            "Ba", "Cai", "Can", "Cang", "Cao", "Ce", "Ceng", "Cha", "Chai", "Chan",

            "Chang", "Chao", "Che", "Chen", "Cheng", "Chi", "Chong", "Chou", "Chu",

            "Chuai", "Chuan", "Chuang", "Chui", "Chun", "Chuo", "Ci", "Cong", "Cou",

            "Cu", "Cuan", "Cui", "Cun", "Cuo", "Da", "Dai", "Dan", "Dang", "Dao", "De",

            "Deng", "Di", "Dian", "Diao", "Die", "Ding", "Diu", "Dong", "Dou", "Du",

            "Duan", "Dui", "Dun", "Duo", "E", "En", "Er", "Fa", "Fan", "Fang", "Fei",

            "Fen", "Feng", "Fo", "Fou", "Fu", "Ga", "Gai", "Gan", "Gang", "Gao", "Ge",

            "Gei", "Gen", "Geng", "Gong", "Gou", "Gu", "Gua", "Guai", "Guan", "Guang",

            "Gui", "Gun", "Guo", "Ha", "Hai", "Han", "Hang", "Hao", "He", "Hei", "Hen",

            "Heng", "Hong", "Hou", "Hu", "Hua", "Huai", "Huan", "Huang", "Hui", "Hun",

            "Huo", "Ji", "Jia", "Jian", "Jiang", "Jiao", "Jie", "Jin", "Jing", "Jiong",

            "Jiu", "Ju", "Juan", "Jue", "Jun", "Ka", "Kai", "Kan", "Kang", "Kao", "Ke",

            "Ken", "Keng", "Kong", "Kou", "Ku", "Kua", "Kuai", "Kuan", "Kuang", "Kui",

            "Kun", "Kuo", "La", "Lai", "Lan", "Lang", "Lao", "Le", "Lei", "Leng", "Li",

            "Lia", "Lian", "Liang", "Liao", "Lie", "Lin", "Ling", "Liu", "Long", "Lou",

            "Lu", "Lv", "Luan", "Lue", "Lun", "Luo", "Ma", "Mai", "Man", "Mang", "Mao",

            "Me", "Mei", "Men", "Meng", "Mi", "Mian", "Miao", "Mie", "Min", "Ming", "Miu",

            "Mo", "Mou", "Mu", "Na", "Nai", "Nan", "Nang", "Nao", "Ne", "Nei", "Nen",

            "Neng", "Ni", "Nian", "Niang", "Niao", "Nie", "Nin", "Ning", "Niu", "Nong",

            "Nu", "Nv", "Nuan", "Nue", "Nuo", "O", "Ou", "Pa", "Pai", "Pan", "Pang",

            "Pao", "Pei", "Pen", "Peng", "Pi", "Pian", "Piao", "Pie", "Pin", "Ping",

            "Po", "Pu", "Qi", "Qia", "Qian", "Qiang", "Qiao", "Qie", "Qin", "Qing",

            "Qiong", "Qiu", "Qu", "Quan", "Que", "Qun", "Ran", "Rang", "Rao", "Re",

            "Ren", "Reng", "Ri", "Rong", "Rou", "Ru", "Ruan", "Rui", "Run", "Ruo",

            "Sa", "Sai", "San", "Sang", "Sao", "Se", "Sen", "Seng", "Sha", "Shai",

            "Shan", "Shang", "Shao", "She", "Shen", "Sheng", "Shi", "Shou", "Shu",

            "Shua", "Shuai", "Shuan", "Shuang", "Shui", "Shun", "Shuo", "Si", "Song",

            "Sou", "Su", "Suan", "Sui", "Sun", "Suo", "Ta", "Tai", "Tan", "Tang",

            "Tao", "Te", "Teng", "Ti", "Tian", "Tiao", "Tie", "Ting", "Tong", "Tou",

            "Tu", "Tuan", "Tui", "Tun", "Tuo", "Wa", "Wai", "Wan", "Wang", "Wei",

            "Wen", "Weng", "Wo", "Wu", "Xi", "Xia", "Xian", "Xiang", "Xiao", "Xie",

            "Xin", "Xing", "Xiong", "Xiu", "Xu", "Xuan", "Xue", "Xun", "Ya", "Yan",

            "Yang", "Yao", "Ye", "Yi", "Yin", "Ying", "Yo", "Yong", "You", "Yu",

            "Yuan", "Yue", "Yun", "Za", "Zai", "Zan", "Zang", "Zao", "Ze", "Zei",

            "Zen", "Zeng", "Zha", "Zhai", "Zhan", "Zhang", "Zhao", "Zhe", "Zhen",

            "Zheng", "Zhi", "Zhong", "Zhou", "Zhu", "Zhua", "Zhuai", "Zhuan",

            "Zhuang", "Zhui", "Zhun", "Zhuo", "Zi", "Zong", "Zou", "Zu", "Zuan",

            "Zui", "Zun", "Zuo"
        };

        #region 二级汉字

        /// <summary>
        /// 二级汉字数组
        /// </summary>
        private static readonly string[] OtherChinese = {
            "亍", "丌", "兀", "丐", "廿", "卅", "丕", "亘", "丞", "鬲", "孬", "噩", "丨", "禺", "丿"
            , "匕", "乇", "夭", "爻", "卮", "氐", "囟", "胤", "馗", "毓", "睾", "鼗", "丶", "亟", "鼐", "乜"
            , "乩", "亓", "芈", "孛", "啬", "嘏", "仄", "厍", "厝", "厣", "厥", "厮", "靥", "赝", "匚", "叵"
            , "匦", "匮", "匾", "赜", "卦", "卣", "刂", "刈", "刎", "刭", "刳", "刿", "剀", "剌", "剞", "剡"
            , "剜", "蒯", "剽", "劂", "劁", "劐", "劓", "冂", "罔", "亻", "仃", "仉", "仂", "仨", "仡", "仫"
            , "仞", "伛", "仳", "伢", "佤", "仵", "伥", "伧", "伉", "伫", "佞", "佧", "攸", "佚", "佝"
            , "佟", "佗", "伲", "伽", "佶", "佴", "侑", "侉", "侃", "侏", "佾", "佻", "侪", "佼", "侬"
            , "侔", "俦", "俨", "俪", "俅", "俚", "俣", "俜", "俑", "俟", "俸", "倩", "偌", "俳", "倬", "倏"
            , "倮", "倭", "俾", "倜", "倌", "倥", "倨", "偾", "偃", "偕", "偈", "偎", "偬", "偻", "傥", "傧"
            , "傩", "傺", "僖", "儆", "僭", "僬", "僦", "僮", "儇", "儋", "仝", "氽", "佘", "佥", "俎", "龠"
            , "汆", "籴", "兮", "巽", "黉", "馘", "冁", "夔", "勹", "匍", "訇", "匐", "凫", "夙", "兕", "亠"
            , "兖", "亳", "衮", "袤", "亵", "脔", "裒", "禀", "嬴", "蠃", "羸", "冫", "冱", "冽", "冼"
            , "凇", "冖", "冢", "冥", "讠", "讦", "讧", "讪", "讴", "讵", "讷", "诂", "诃", "诋", "诏"
            , "诎", "诒", "诓", "诔", "诖", "诘", "诙", "诜", "诟", "诠", "诤", "诨", "诩", "诮", "诰", "诳"
            , "诶", "诹", "诼", "诿", "谀", "谂", "谄", "谇", "谌", "谏", "谑", "谒", "谔", "谕", "谖", "谙"
            , "谛", "谘", "谝", "谟", "谠", "谡", "谥", "谧", "谪", "谫", "谮", "谯", "谲", "谳", "谵", "谶"
            , "卩", "卺", "阝", "阢", "阡", "阱", "阪", "阽", "阼", "陂", "陉", "陔", "陟", "陧", "陬", "陲"
            , "陴", "隈", "隍", "隗", "隰", "邗", "邛", "邝", "邙", "邬", "邡", "邴", "邳", "邶", "邺"
            , "邸", "邰", "郏", "郅", "邾", "郐", "郄", "郇", "郓", "郦", "郢", "郜", "郗", "郛", "郫"
            , "郯", "郾", "鄄", "鄢", "鄞", "鄣", "鄱", "鄯", "鄹", "酃", "酆", "刍", "奂", "劢", "劬", "劭"
            , "劾", "哿", "勐", "勖", "勰", "叟", "燮", "矍", "廴", "凵", "凼", "鬯", "厶", "弁", "畚", "巯"
            , "坌", "垩", "垡", "塾", "墼", "壅", "壑", "圩", "圬", "圪", "圳", "圹", "圮", "圯", "坜", "圻"
            , "坂", "坩", "垅", "坫", "垆", "坼", "坻", "坨", "坭", "坶", "坳", "垭", "垤", "垌", "垲", "埏"
            , "垧", "垴", "垓", "垠", "埕", "埘", "埚", "埙", "埒", "垸", "埴", "埯", "埸", "埤", "埝"
            , "堋", "堍", "埽", "埭", "堀", "堞", "堙", "塄", "堠", "塥", "塬", "墁", "墉", "墚", "墀"
            , "馨", "鼙", "懿", "艹", "艽", "艿", "芏", "芊", "芨", "芄", "芎", "芑", "芗", "芙", "芫", "芸"
            , "芾", "芰", "苈", "苊", "苣", "芘", "芷", "芮", "苋", "苌", "苁", "芩", "芴", "芡", "芪", "芟"
            , "苄", "苎", "芤", "苡", "茉", "苷", "苤", "茏", "茇", "苜", "苴", "苒", "苘", "茌", "苻", "苓"
            , "茑", "茚", "茆", "茔", "茕", "苠", "苕", "茜", "荑", "荛", "荜", "茈", "莒", "茼", "茴", "茱"
            , "莛", "荞", "茯", "荏", "荇", "荃", "荟", "荀", "茗", "荠", "茭", "茺", "茳", "荦", "荥"
            , "荨", "茛", "荩", "荬", "荪", "荭", "荮", "莰", "荸", "莳", "莴", "莠", "莪", "莓", "莜"
            , "莅", "荼", "莶", "莩", "荽", "莸", "荻", "莘", "莞", "莨", "莺", "莼", "菁", "萁", "菥", "菘"
            , "堇", "萘", "萋", "菝", "菽", "菖", "萜", "萸", "萑", "萆", "菔", "菟", "萏", "萃", "菸", "菹"
            , "菪", "菅", "菀", "萦", "菰", "菡", "葜", "葑", "葚", "葙", "葳", "蒇", "蒈", "葺", "蒉", "葸"
            , "萼", "葆", "葩", "葶", "蒌", "蒎", "萱", "葭", "蓁", "蓍", "蓐", "蓦", "蒽", "蓓", "蓊", "蒿"
            , "蒺", "蓠", "蒡", "蒹", "蒴", "蒗", "蓥", "蓣", "蔌", "甍", "蔸", "蓰", "蔹", "蔟", "蔺"
            , "蕖", "蔻", "蓿", "蓼", "蕙", "蕈", "蕨", "蕤", "蕞", "蕺", "瞢", "蕃", "蕲", "蕻", "薤"
            , "薨", "薇", "薏", "蕹", "薮", "薜", "薅", "薹", "薷", "薰", "藓", "藁", "藜", "藿", "蘧", "蘅"
            , "蘩", "蘖", "蘼", "廾", "弈", "夼", "奁", "耷", "奕", "奚", "奘", "匏", "尢", "尥", "尬", "尴"
            , "扌", "扪", "抟", "抻", "拊", "拚", "拗", "拮", "挢", "拶", "挹", "捋", "捃", "掭", "揶", "捱"
            , "捺", "掎", "掴", "捭", "掬", "掊", "捩", "掮", "掼", "揲", "揸", "揠", "揿", "揄", "揞", "揎"
            , "摒", "揆", "掾", "摅", "摁", "搋", "搛", "搠", "搌", "搦", "搡", "摞", "撄", "摭", "撖"
            , "摺", "撷", "撸", "撙", "撺", "擀", "擐", "擗", "擤", "擢", "攉", "攥", "攮", "弋", "忒"
            , "甙", "弑", "卟", "叱", "叽", "叩", "叨", "叻", "吒", "吖", "吆", "呋", "呒", "呓", "呔", "呖"
            , "呃", "吡", "呗", "呙", "吣", "吲", "咂", "咔", "呷", "呱", "呤", "咚", "咛", "咄", "呶", "呦"
            , "咝", "哐", "咭", "哂", "咴", "哒", "咧", "咦", "哓", "哔", "呲", "咣", "哕", "咻", "咿", "哌"
            , "哙", "哚", "哜", "咩", "咪", "咤", "哝", "哏", "哞", "唛", "哧", "唠", "哽", "唔", "哳", "唢"
            , "唣", "唏", "唑", "唧", "唪", "啧", "喏", "喵", "啉", "啭", "啁", "啕", "唿", "啐", "唼"
            , "唷", "啖", "啵", "啶", "啷", "唳", "唰", "啜", "喋", "嗒", "喃", "喱", "喹", "喈", "喁"
            , "喟", "啾", "嗖", "喑", "啻", "嗟", "喽", "喾", "喔", "喙", "嗪", "嗷", "嗉", "嘟", "嗑", "嗫"
            , "嗬", "嗔", "嗦", "嗝", "嗄", "嗯", "嗥", "嗲", "嗳", "嗌", "嗍", "嗨", "嗵", "嗤", "辔", "嘞"
            , "嘈", "嘌", "嘁", "嘤", "嘣", "嗾", "嘀", "嘧", "嘭", "噘", "嘹", "噗", "嘬", "噍", "噢", "噙"
            , "噜", "噌", "噔", "嚆", "噤", "噱", "噫", "噻", "噼", "嚅", "嚓", "嚯", "囔", "囗", "囝", "囡"
            , "囵", "囫", "囹", "囿", "圄", "圊", "圉", "圜", "帏", "帙", "帔", "帑", "帱", "帻", "帼"
            , "帷", "幄", "幔", "幛", "幞", "幡", "岌", "屺", "岍", "岐", "岖", "岈", "岘", "岙", "岑"
            , "岚", "岜", "岵", "岢", "岽", "岬", "岫", "岱", "岣", "峁", "岷", "峄", "峒", "峤", "峋", "峥"
            , "崂", "崃", "崧", "崦", "崮", "崤", "崞", "崆", "崛", "嵘", "崾", "崴", "崽", "嵬", "嵛", "嵯"
            , "嵝", "嵫", "嵋", "嵊", "嵩", "嵴", "嶂", "嶙", "嶝", "豳", "嶷", "巅", "彳", "彷", "徂", "徇"
            , "徉", "後", "徕", "徙", "徜", "徨", "徭", "徵", "徼", "衢", "彡", "犭", "犰", "犴", "犷", "犸"
            , "狃", "狁", "狎", "狍", "狒", "狨", "狯", "狩", "狲", "狴", "狷", "猁", "狳", "猃", "狺"
            , "狻", "猗", "猓", "猡", "猊", "猞", "猝", "猕", "猢", "猹", "猥", "猬", "猸", "猱", "獐"
            , "獍", "獗", "獠", "獬", "獯", "獾", "舛", "夥", "飧", "夤", "夂", "饣", "饧", "饨", "饩", "饪"
            , "饫", "饬", "饴", "饷", "饽", "馀", "馄", "馇", "馊", "馍", "馐", "馑", "馓", "馔", "馕", "庀"
            , "庑", "庋", "庖", "庥", "庠", "庹", "庵", "庾", "庳", "赓", "廒", "廑", "廛", "廨", "廪", "膺"
            , "忄", "忉", "忖", "忏", "怃", "忮", "怄", "忡", "忤", "忾", "怅", "怆", "忪", "忭", "忸", "怙"
            , "怵", "怦", "怛", "怏", "怍", "怩", "怫", "怊", "怿", "怡", "恸", "恹", "恻", "恺", "恂"
            , "恪", "恽", "悖", "悚", "悭", "悝", "悃", "悒", "悌", "悛", "惬", "悻", "悱", "惝", "惘"
            , "惆", "惚", "悴", "愠", "愦", "愕", "愣", "惴", "愀", "愎", "愫", "慊", "慵", "憬", "憔", "憧"
            , "憷", "懔", "懵", "忝", "隳", "闩", "闫", "闱", "闳", "闵", "闶", "闼", "闾", "阃", "阄", "阆"
            , "阈", "阊", "阋", "阌", "阍", "阏", "阒", "阕", "阖", "阗", "阙", "阚", "丬", "爿", "戕", "氵"
            , "汔", "汜", "汊", "沣", "沅", "沐", "沔", "沌", "汨", "汩", "汴", "汶", "沆", "沩", "泐", "泔"
            , "沭", "泷", "泸", "泱", "泗", "沲", "泠", "泖", "泺", "泫", "泮", "沱", "泓", "泯", "泾"
            , "洹", "洧", "洌", "浃", "浈", "洇", "洄", "洙", "洎", "洫", "浍", "洮", "洵", "洚", "浏"
            , "浒", "浔", "洳", "涑", "浯", "涞", "涠", "浞", "涓", "涔", "浜", "浠", "浼", "浣", "渚", "淇"
            , "淅", "淞", "渎", "涿", "淠", "渑", "淦", "淝", "淙", "渖", "涫", "渌", "涮", "渫", "湮", "湎"
            , "湫", "溲", "湟", "溆", "湓", "湔", "渲", "渥", "湄", "滟", "溱", "溘", "滠", "漭", "滢", "溥"
            , "溧", "溽", "溻", "溷", "滗", "溴", "滏", "溏", "滂", "溟", "潢", "潆", "潇", "漤", "漕", "滹"
            , "漯", "漶", "潋", "潴", "漪", "漉", "漩", "澉", "澍", "澌", "潸", "潲", "潼", "潺", "濑"
            , "濉", "澧", "澹", "澶", "濂", "濡", "濮", "濞", "濠", "濯", "瀚", "瀣", "瀛", "瀹", "瀵"
            , "灏", "灞", "宀", "宄", "宕", "宓", "宥", "宸", "甯", "骞", "搴", "寤", "寮", "褰", "寰", "蹇"
            , "謇", "辶", "迓", "迕", "迥", "迮", "迤", "迩", "迦", "迳", "迨", "逅", "逄", "逋", "逦", "逑"
            , "逍", "逖", "逡", "逵", "逶", "逭", "逯", "遄", "遑", "遒", "遐", "遨", "遘", "遢", "遛", "暹"
            , "遴", "遽", "邂", "邈", "邃", "邋", "彐", "彗", "彖", "彘", "尻", "咫", "屐", "屙", "孱", "屣"
            , "屦", "羼", "弪", "弩", "弭", "艴", "弼", "鬻", "屮", "妁", "妃", "妍", "妩", "妪", "妣"
            , "妗", "姊", "妫", "妞", "妤", "姒", "妲", "妯", "姗", "妾", "娅", "娆", "姝", "娈", "姣"
            , "姘", "姹", "娌", "娉", "娲", "娴", "娑", "娣", "娓", "婀", "婧", "婊", "婕", "娼", "婢", "婵"
            , "胬", "媪", "媛", "婷", "婺", "媾", "嫫", "媲", "嫒", "嫔", "媸", "嫠", "嫣", "嫱", "嫖", "嫦"
            , "嫘", "嫜", "嬉", "嬗", "嬖", "嬲", "嬷", "孀", "尕", "尜", "孚", "孥", "孳", "孑", "孓", "孢"
            , "驵", "驷", "驸", "驺", "驿", "驽", "骀", "骁", "骅", "骈", "骊", "骐", "骒", "骓", "骖", "骘"
            , "骛", "骜", "骝", "骟", "骠", "骢", "骣", "骥", "骧", "纟", "纡", "纣", "纥", "纨", "纩"
            , "纭", "纰", "纾", "绀", "绁", "绂", "绉", "绋", "绌", "绐", "绔", "绗", "绛", "绠", "绡"
            , "绨", "绫", "绮", "绯", "绱", "绲", "缍", "绶", "绺", "绻", "绾", "缁", "缂", "缃", "缇", "缈"
            , "缋", "缌", "缏", "缑", "缒", "缗", "缙", "缜", "缛", "缟", "缡", "缢", "缣", "缤", "缥", "缦"
            , "缧", "缪", "缫", "缬", "缭", "缯", "缰", "缱", "缲", "缳", "缵", "幺", "畿", "巛", "甾", "邕"
            , "玎", "玑", "玮", "玢", "玟", "珏", "珂", "珑", "玷", "玳", "珀", "珉", "珈", "珥", "珙", "顼"
            , "琊", "珩", "珧", "珞", "玺", "珲", "琏", "琪", "瑛", "琦", "琥", "琨", "琰", "琮", "琬"
            , "琛", "琚", "瑁", "瑜", "瑗", "瑕", "瑙", "瑷", "瑭", "瑾", "璜", "璎", "璀", "璁", "璇"
            , "璋", "璞", "璨", "璩", "璐", "璧", "瓒", "璺", "韪", "韫", "韬", "杌", "杓", "杞", "杈", "杩"
            , "枥", "枇", "杪", "杳", "枘", "枧", "杵", "枨", "枞", "枭", "枋", "杷", "杼", "柰", "栉", "柘"
            , "栊", "柩", "枰", "栌", "柙", "枵", "柚", "枳", "柝", "栀", "柃", "枸", "柢", "栎", "柁", "柽"
            , "栲", "栳", "桠", "桡", "桎", "桢", "桄", "桤", "梃", "栝", "桕", "桦", "桁", "桧", "桀", "栾"
            , "桊", "桉", "栩", "梵", "梏", "桴", "桷", "梓", "桫", "棂", "楮", "棼", "椟", "椠", "棹"
            , "椤", "棰", "椋", "椁", "楗", "棣", "椐", "楱", "椹", "楠", "楂", "楝", "榄", "楫", "榀"
            , "榘", "楸", "椴", "槌", "榇", "榈", "槎", "榉", "楦", "楣", "楹", "榛", "榧", "榻", "榫", "榭"
            , "槔", "榱", "槁", "槊", "槟", "榕", "槠", "榍", "槿", "樯", "槭", "樗", "樘", "橥", "槲", "橄"
            , "樾", "檠", "橐", "橛", "樵", "檎", "橹", "樽", "樨", "橘", "橼", "檑", "檐", "檩", "檗", "檫"
            , "猷", "獒", "殁", "殂", "殇", "殄", "殒", "殓", "殍", "殚", "殛", "殡", "殪", "轫", "轭", "轱"
            , "轲", "轳", "轵", "轶", "轸", "轷", "轹", "轺", "轼", "轾", "辁", "辂", "辄", "辇", "辋"
            , "辍", "辎", "辏", "辘", "辚", "軎", "戋", "戗", "戛", "戟", "戢", "戡", "戥", "戤", "戬"
            , "臧", "瓯", "瓴", "瓿", "甏", "甑", "甓", "攴", "旮", "旯", "旰", "昊", "昙", "杲", "昃", "昕"
            , "昀", "炅", "曷", "昝", "昴", "昱", "昶", "昵", "耆", "晟", "晔", "晁", "晏", "晖", "晡", "晗"
            , "晷", "暄", "暌", "暧", "暝", "暾", "曛", "曜", "曦", "曩", "贲", "贳", "贶", "贻", "贽", "赀"
            , "赅", "赆", "赈", "赉", "赇", "赍", "赕", "赙", "觇", "觊", "觋", "觌", "觎", "觏", "觐", "觑"
            , "牮", "犟", "牝", "牦", "牯", "牾", "牿", "犄", "犋", "犍", "犏", "犒", "挈", "挲", "掰"
            , "搿", "擘", "耄", "毪", "毳", "毽", "毵", "毹", "氅", "氇", "氆", "氍", "氕", "氘", "氙"
            , "氚", "氡", "氩", "氤", "氪", "氲", "攵", "敕", "敫", "牍", "牒", "牖", "爰", "虢", "刖", "肟"
            , "肜", "肓", "肼", "朊", "肽", "肱", "肫", "肭", "肴", "肷", "胧", "胨", "胩", "胪", "胛", "胂"
            , "胄", "胙", "胍", "胗", "朐", "胝", "胫", "胱", "胴", "胭", "脍", "脎", "胲", "胼", "朕", "脒"
            , "豚", "脶", "脞", "脬", "脘", "脲", "腈", "腌", "腓", "腴", "腙", "腚", "腱", "腠", "腩", "腼"
            , "腽", "腭", "腧", "塍", "媵", "膈", "膂", "膑", "滕", "膣", "膪", "臌", "朦", "臊", "膻"
            , "臁", "膦", "欤", "欷", "欹", "歃", "歆", "歙", "飑", "飒", "飓", "飕", "飙", "飚", "殳"
            , "彀", "毂", "觳", "斐", "齑", "斓", "於", "旆", "旄", "旃", "旌", "旎", "旒", "旖", "炀", "炜"
            , "炖", "炝", "炻", "烀", "炷", "炫", "炱", "烨", "烊", "焐", "焓", "焖", "焯", "焱", "煳", "煜"
            , "煨", "煅", "煲", "煊", "煸", "煺", "熘", "熳", "熵", "熨", "熠", "燠", "燔", "燧", "燹", "爝"
            , "爨", "灬", "焘", "煦", "熹", "戾", "戽", "扃", "扈", "扉", "礻", "祀", "祆", "祉", "祛", "祜"
            , "祓", "祚", "祢", "祗", "祠", "祯", "祧", "祺", "禅", "禊", "禚", "禧", "禳", "忑", "忐"
            , "怼", "恝", "恚", "恧", "恁", "恙", "恣", "悫", "愆", "愍", "慝", "憩", "憝", "懋", "懑"
            , "戆", "肀", "聿", "沓", "泶", "淼", "矶", "矸", "砀", "砉", "砗", "砘", "砑", "斫", "砭", "砜"
            , "砝", "砹", "砺", "砻", "砟", "砼", "砥", "砬", "砣", "砩", "硎", "硭", "硖", "硗", "砦", "硐"
            , "硇", "硌", "硪", "碛", "碓", "碚", "碇", "碜", "碡", "碣", "碲", "碹", "碥", "磔", "磙", "磉"
            , "磬", "磲", "礅", "磴", "礓", "礤", "礞", "礴", "龛", "黹", "黻", "黼", "盱", "眄", "眍", "盹"
            , "眇", "眈", "眚", "眢", "眙", "眭", "眦", "眵", "眸", "睐", "睑", "睇", "睃", "睚", "睨"
            , "睢", "睥", "睿", "瞍", "睽", "瞀", "瞌", "瞑", "瞟", "瞠", "瞰", "瞵", "瞽", "町", "畀"
            , "畎", "畋", "畈", "畛", "畲", "畹", "疃", "罘", "罡", "罟", "詈", "罨", "罴", "罱", "罹", "羁"
            , "罾", "盍", "盥", "蠲", "钅", "钆", "钇", "钋", "钊", "钌", "钍", "钏", "钐", "钔", "钗", "钕"
            , "钚", "钛", "钜", "钣", "钤", "钫", "钪", "钭", "钬", "钯", "钰", "钲", "钴", "钶", "钷", "钸"
            , "钹", "钺", "钼", "钽", "钿", "铄", "铈", "铉", "铊", "铋", "铌", "铍", "铎", "铐", "铑", "铒"
            , "铕", "铖", "铗", "铙", "铘", "铛", "铞", "铟", "铠", "铢", "铤", "铥", "铧", "铨", "铪"
            , "铩", "铫", "铮", "铯", "铳", "铴", "铵", "铷", "铹", "铼", "铽", "铿", "锃", "锂", "锆"
            , "锇", "锉", "锊", "锍", "锎", "锏", "锒", "锓", "锔", "锕", "锖", "锘", "锛", "锝", "锞", "锟"
            , "锢", "锪", "锫", "锩", "锬", "锱", "锲", "锴", "锶", "锷", "锸", "锼", "锾", "锿", "镂", "锵"
            , "镄", "镅", "镆", "镉", "镌", "镎", "镏", "镒", "镓", "镔", "镖", "镗", "镘", "镙", "镛", "镞"
            , "镟", "镝", "镡", "镢", "镤", "镥", "镦", "镧", "镨", "镩", "镪", "镫", "镬", "镯", "镱", "镲"
            , "镳", "锺", "矧", "矬", "雉", "秕", "秭", "秣", "秫", "稆", "嵇", "稃", "稂", "稞", "稔"
            , "稹", "稷", "穑", "黏", "馥", "穰", "皈", "皎", "皓", "皙", "皤", "瓞", "瓠", "甬", "鸠"
            , "鸢", "鸨", "鸩", "鸪", "鸫", "鸬", "鸲", "鸱", "鸶", "鸸", "鸷", "鸹", "鸺", "鸾", "鹁", "鹂"
            , "鹄", "鹆", "鹇", "鹈", "鹉", "鹋", "鹌", "鹎", "鹑", "鹕", "鹗", "鹚", "鹛", "鹜", "鹞", "鹣"
            , "鹦", "鹧", "鹨", "鹩", "鹪", "鹫", "鹬", "鹱", "鹭", "鹳", "疒", "疔", "疖", "疠", "疝", "疬"
            , "疣", "疳", "疴", "疸", "痄", "疱", "疰", "痃", "痂", "痖", "痍", "痣", "痨", "痦", "痤", "痫"
            , "痧", "瘃", "痱", "痼", "痿", "瘐", "瘀", "瘅", "瘌", "瘗", "瘊", "瘥", "瘘", "瘕", "瘙"
            , "瘛", "瘼", "瘢", "瘠", "癀", "瘭", "瘰", "瘿", "瘵", "癃", "瘾", "瘳", "癍", "癞", "癔"
            , "癜", "癖", "癫", "癯", "翊", "竦", "穸", "穹", "窀", "窆", "窈", "窕", "窦", "窠", "窬", "窨"
            , "窭", "窳", "衤", "衩", "衲", "衽", "衿", "袂", "袢", "裆", "袷", "袼", "裉", "裢", "裎", "裣"
            , "裥", "裱", "褚", "裼", "裨", "裾", "裰", "褡", "褙", "褓", "褛", "褊", "褴", "褫", "褶", "襁"
            , "襦", "襻", "疋", "胥", "皲", "皴", "矜", "耒", "耔", "耖", "耜", "耠", "耢", "耥", "耦", "耧"
            , "耩", "耨", "耱", "耋", "耵", "聃", "聆", "聍", "聒", "聩", "聱", "覃", "顸", "颀", "颃"
            , "颉", "颌", "颍", "颏", "颔", "颚", "颛", "颞", "颟", "颡", "颢", "颥", "颦", "虍", "虔"
            , "虬", "虮", "虿", "虺", "虼", "虻", "蚨", "蚍", "蚋", "蚬", "蚝", "蚧", "蚣", "蚪", "蚓", "蚩"
            , "蚶", "蛄", "蚵", "蛎", "蚰", "蚺", "蚱", "蚯", "蛉", "蛏", "蚴", "蛩", "蛱", "蛲", "蛭", "蛳"
            , "蛐", "蜓", "蛞", "蛴", "蛟", "蛘", "蛑", "蜃", "蜇", "蛸", "蜈", "蜊", "蜍", "蜉", "蜣", "蜻"
            , "蜞", "蜥", "蜮", "蜚", "蜾", "蝈", "蜴", "蜱", "蜩", "蜷", "蜿", "螂", "蜢", "蝽", "蝾", "蝻"
            , "蝠", "蝰", "蝌", "蝮", "螋", "蝓", "蝣", "蝼", "蝤", "蝙", "蝥", "螓", "螯", "螨", "蟒"
            , "蟆", "螈", "螅", "螭", "螗", "螃", "螫", "蟥", "螬", "螵", "螳", "蟋", "蟓", "螽", "蟑"
            , "蟀", "蟊", "蟛", "蟪", "蟠", "蟮", "蠖", "蠓", "蟾", "蠊", "蠛", "蠡", "蠹", "蠼", "缶", "罂"
            , "罄", "罅", "舐", "竺", "竽", "笈", "笃", "笄", "笕", "笊", "笫", "笏", "筇", "笸", "笪", "笙"
            , "笮", "笱", "笠", "笥", "笤", "笳", "笾", "笞", "筘", "筚", "筅", "筵", "筌", "筝", "筠", "筮"
            , "筻", "筢", "筲", "筱", "箐", "箦", "箧", "箸", "箬", "箝", "箨", "箅", "箪", "箜", "箢", "箫"
            , "箴", "篑", "篁", "篌", "篝", "篚", "篥", "篦", "篪", "簌", "篾", "篼", "簏", "簖", "簋"
            , "簟", "簪", "簦", "簸", "籁", "籀", "臾", "舁", "舂", "舄", "臬", "衄", "舡", "舢", "舣"
            , "舭", "舯", "舨", "舫", "舸", "舻", "舳", "舴", "舾", "艄", "艉", "艋", "艏", "艚", "艟", "艨"
            , "衾", "袅", "袈", "裘", "裟", "襞", "羝", "羟", "羧", "羯", "羰", "羲", "籼", "敉", "粑", "粝"
            , "粜", "粞", "粢", "粲", "粼", "粽", "糁", "糇", "糌", "糍", "糈", "糅", "糗", "糨", "艮", "暨"
            , "羿", "翎", "翕", "翥", "翡", "翦", "翩", "翮", "翳", "糸", "絷", "綦", "綮", "繇", "纛", "麸"
            , "麴", "赳", "趄", "趔", "趑", "趱", "赧", "赭", "豇", "豉", "酊", "酐", "酎", "酏", "酤"
            , "酢", "酡", "酰", "酩", "酯", "酽", "酾", "酲", "酴", "酹", "醌", "醅", "醐", "醍", "醑"
            , "醢", "醣", "醪", "醭", "醮", "醯", "醵", "醴", "醺", "豕", "鹾", "趸", "跫", "踅", "蹙", "蹩"
            , "趵", "趿", "趼", "趺", "跄", "跖", "跗", "跚", "跞", "跎", "跏", "跛", "跆", "跬", "跷", "跸"
            , "跣", "跹", "跻", "跤", "踉", "跽", "踔", "踝", "踟", "踬", "踮", "踣", "踯", "踺", "蹀", "踹"
            , "踵", "踽", "踱", "蹉", "蹁", "蹂", "蹑", "蹒", "蹊", "蹰", "蹶", "蹼", "蹯", "蹴", "躅", "躏"
            , "躔", "躐", "躜", "躞", "豸", "貂", "貊", "貅", "貘", "貔", "斛", "觖", "觞", "觚", "觜"
            , "觥", "觫", "觯", "訾", "謦", "靓", "雩", "雳", "雯", "霆", "霁", "霈", "霏", "霎", "霪"
            , "霭", "霰", "霾", "龀", "龃", "龅", "龆", "龇", "龈", "龉", "龊", "龌", "黾", "鼋", "鼍", "隹"
            , "隼", "隽", "雎", "雒", "瞿", "雠", "銎", "銮", "鋈", "錾", "鍪", "鏊", "鎏", "鐾", "鑫", "鱿"
            , "鲂", "鲅", "鲆", "鲇", "鲈", "稣", "鲋", "鲎", "鲐", "鲑", "鲒", "鲔", "鲕", "鲚", "鲛", "鲞"
            , "鲟", "鲠", "鲡", "鲢", "鲣", "鲥", "鲦", "鲧", "鲨", "鲩", "鲫", "鲭", "鲮", "鲰", "鲱", "鲲"
            , "鲳", "鲴", "鲵", "鲶", "鲷", "鲺", "鲻", "鲼", "鲽", "鳄", "鳅", "鳆", "鳇", "鳊", "鳋"
            , "鳌", "鳍", "鳎", "鳏", "鳐", "鳓", "鳔", "鳕", "鳗", "鳘", "鳙", "鳜", "鳝", "鳟", "鳢"
            , "靼", "鞅", "鞑", "鞒", "鞔", "鞯", "鞫", "鞣", "鞲", "鞴", "骱", "骰", "骷", "鹘", "骶", "骺"
            , "骼", "髁", "髀", "髅", "髂", "髋", "髌", "髑", "魅", "魃", "魇", "魉", "魈", "魍", "魑", "飨"
            , "餍", "餮", "饕", "饔", "髟", "髡", "髦", "髯", "髫", "髻", "髭", "髹", "鬈", "鬏", "鬓", "鬟"
            , "鬣", "麽", "麾", "縻", "麂", "麇", "麈", "麋", "麒", "鏖", "麝", "麟", "黛", "黜", "黝", "黠"
            , "黟", "黢", "黩", "黧", "黥", "黪", "黯", "鼢", "鼬", "鼯", "鼹", "鼷", "鼽", "鼾", "齄"
        };

        /// <summary>
        /// 二级汉字对应拼音数组
        /// </summary>
        private static readonly string[] OtherPinYin = {
            "Chu", "Ji", "Wu", "Gai", "Nian", "Sa", "Pi", "Gen", "Cheng", "Ge", "Nao", "E", "Shu", "Yu", "Pie", "Bi",
            "Tuo", "Yao", "Yao", "Zhi", "Di", "Xin", "Yin", "Kui", "Yu", "Gao", "Tao", "Dian", "Ji", "Nai", "Nie", "Ji",
            "Qi", "Mi", "Bei", "Se", "Gu", "Ze", "She", "Cuo", "Yan", "Jue", "Si", "Ye", "Yan", "Fang", "Po", "Gui",
            "Kui", "Bian", "Ze", "Gua", "You", "Ce", "Yi", "Wen", "Jing", "Ku", "Gui", "Kai", "La", "Ji", "Yan", "Wan",
            "Kuai", "Piao", "Jue", "Qiao", "Huo", "Yi", "Tong", "Wang", "Dan", "Ding", "Zhang", "Le", "Sa", "Yi", "Mu",
            "Ren",
            "Yu", "Pi", "Ya", "Wa", "Wu", "Chang", "Cang", "Kang", "Zhu", "Ning", "Ka", "You", "Yi", "Gou", "Tong",
            "Tuo",
            "Ni", "Ga", "Ji", "Er", "You", "Kua", "Kan", "Zhu", "Yi", "Tiao", "Chai", "Jiao", "Nong", "Mou", "Chou",
            "Yan",
            "Li", "Qiu", "Li", "Yu", "Ping", "Yong", "Si", "Feng", "Qian", "Ruo", "Pai", "Zhuo", "Shu", "Luo", "Wo",
            "Bi",
            "Ti", "Guan", "Kong", "Ju", "Fen", "Yan", "Xie", "Ji", "Wei", "Zong", "Lou", "Tang", "Bin", "Nuo", "Chi",
            "Xi",
            "Jing", "Jian", "Jiao", "Jiu", "Tong", "Xuan", "Dan", "Tong", "Tun", "She", "Qian", "Zu", "Yue", "Cuan",
            "Di", "Xi",
            "Xun", "Hong", "Guo", "Chan", "Kui", "Bao", "Pu", "Hong", "Fu", "Fu", "Su", "Si", "Wen", "Yan", "Bo", "Gun",
            "Mao", "Xie", "Luan", "Pou", "Bing", "Ying", "Luo", "Lei", "Liang", "Hu", "Lie", "Xian", "Song", "Ping",
            "Zhong", "Ming",
            "Yan", "Jie", "Hong", "Shan", "Ou", "Ju", "Ne", "Gu", "He", "Di", "Zhao", "Qu", "Dai", "Kuang", "Lei", "Gua",
            "Jie", "Hui", "Shen", "Gou", "Quan", "Zheng", "Hun", "Xu", "Qiao", "Gao", "Kuang", "Ei", "Zou", "Zhuo",
            "Wei", "Yu",
            "Shen", "Chan", "Sui", "Chen", "Jian", "Xue", "Ye", "E", "Yu", "Xuan", "An", "Di", "Zi", "Pian", "Mo",
            "Dang",
            "Su", "Shi", "Mi", "Zhe", "Jian", "Zen", "Qiao", "Jue", "Yan", "Zhan", "Chen", "Dan", "Jin", "Zuo", "Wu",
            "Qian",
            "Jing", "Ban", "Yan", "Zuo", "Bei", "Jing", "Gai", "Zhi", "Nie", "Zou", "Chui", "Pi", "Wei", "Huang", "Wei",
            "Xi",
            "Han", "Qiong", "Kuang", "Mang", "Wu", "Fang", "Bing", "Pi", "Bei", "Ye", "Di", "Tai", "Jia", "Zhi", "Zhu",
            "Kuai",
            "Qie", "Xun", "Yun", "Li", "Ying", "Gao", "Xi", "Fu", "Pi", "Tan", "Yan", "Juan", "Yan", "Yin", "Zhang",
            "Po",
            "Shan", "Zou", "Ling", "Feng", "Chu", "Huan", "Mai", "Qu", "Shao", "He", "Ge", "Meng", "Xu", "Xie", "Sou",
            "Xie",
            "Jue", "Jian", "Qian", "Dang", "Chang", "Si", "Bian", "Ben", "Qiu", "Ben", "E", "Fa", "Shu", "Ji", "Yong",
            "He",
            "Wei", "Wu", "Ge", "Zhen", "Kuang", "Pi", "Yi", "Li", "Qi", "Ban", "Gan", "Long", "Dian", "Lu", "Che", "Di",
            "Tuo", "Ni", "Mu", "Ao", "Ya", "Die", "Dong", "Kai", "Shan", "Shang", "Nao", "Gai", "Yin", "Cheng", "Shi",
            "Guo",
            "Xun", "Lie", "Yuan", "Zhi", "An", "Yi", "Pi", "Nian", "Peng", "Tu", "Sao", "Dai", "Ku", "Die", "Yin",
            "Leng",
            "Hou", "Ge", "Yuan", "Man", "Yong", "Liang", "Chi", "Xin", "Pi", "Yi", "Cao", "Jiao", "Nai", "Du", "Qian",
            "Ji",
            "Wan", "Xiong", "Qi", "Xiang", "Fu", "Yuan", "Yun", "Fei", "Ji", "Li", "E", "Ju", "Pi", "Zhi", "Rui", "Xian",
            "Chang", "Cong", "Qin", "Wu", "Qian", "Qi", "Shan", "Bian", "Zhu", "Kou", "Yi", "Mo", "Gan", "Pie", "Long",
            "Ba",
            "Mu", "Ju", "Ran", "Qing", "Chi", "Fu", "Ling", "Niao", "Yin", "Mao", "Ying", "Qiong", "Min", "Tiao", "Qian",
            "Yi",
            "Rao", "Bi", "Zi", "Ju", "Tong", "Hui", "Zhu", "Ting", "Qiao", "Fu", "Ren", "Xing", "Quan", "Hui", "Xun",
            "Ming",
            "Qi", "Jiao", "Chong", "Jiang", "Luo", "Ying", "Qian", "Gen", "Jin", "Mai", "Sun", "Hong", "Zhou", "Kan",
            "Bi", "Shi",
            "Wo", "You", "E", "Mei", "You", "Li", "Tu", "Xian", "Fu", "Sui", "You", "Di", "Shen", "Guan", "Lang", "Ying",
            "Chun", "Jing", "Qi", "Xi", "Song", "Jin", "Nai", "Qi", "Ba", "Shu", "Chang", "Tie", "Yu", "Huan", "Bi",
            "Fu",
            "Tu", "Dan", "Cui", "Yan", "Zu", "Dang", "Jian", "Wan", "Ying", "Gu", "Han", "Qia", "Feng", "Shen", "Xiang",
            "Wei",
            "Chan", "Kai", "Qi", "Kui", "Xi", "E", "Bao", "Pa", "Ting", "Lou", "Pai", "Xuan", "Jia", "Zhen", "Shi", "Ru",
            "Mo", "En", "Bei", "Weng", "Hao", "Ji", "Li", "Bang", "Jian", "Shuo", "Lang", "Ying", "Yu", "Su", "Meng",
            "Dou",
            "Xi", "Lian", "Cu", "Lin", "Qu", "Kou", "Xu", "Liao", "Hui", "Xun", "Jue", "Rui", "Zui", "Ji", "Meng", "Fan",
            "Qi", "Hong", "Xie", "Hong", "Wei", "Yi", "Weng", "Sou", "Bi", "Hao", "Tai", "Ru", "Xun", "Xian", "Gao",
            "Li",
            "Huo", "Qu", "Heng", "Fan", "Nie", "Mi", "Gong", "Yi", "Kuang", "Lian", "Da", "Yi", "Xi", "Zang", "Pao",
            "You",
            "Liao", "Ga", "Gan", "Ti", "Men", "Tuan", "Chen", "Fu", "Pin", "Niu", "Jie", "Jiao", "Za", "Yi", "Lv", "Jun",
            "Tian", "Ye", "Ai", "Na", "Ji", "Guo", "Bai", "Ju", "Pou", "Lie", "Qian", "Guan", "Die", "Zha", "Ya", "Qin",
            "Yu", "An", "Xuan", "Bing", "Kui", "Yuan", "Shu", "En", "Chuai", "Jian", "Shuo", "Zhan", "Nuo", "Sang",
            "Luo", "Ying",
            "Zhi", "Han", "Zhe", "Xie", "Lu", "Zun", "Cuan", "Gan", "Huan", "Pi", "Xing", "Zhuo", "Huo", "Zuan", "Nang",
            "Yi",
            "Te", "Dai", "Shi", "Bu", "Chi", "Ji", "Kou", "Dao", "Le", "Zha", "A", "Yao", "Fu", "Mu", "Yi", "Tai",
            "Li", "E", "Bi", "Bei", "Guo", "Qin", "Yin", "Za", "Ka", "Ga", "Gua", "Ling", "Dong", "Ning", "Duo", "Nao",
            "You", "Si", "Kuang", "Ji", "Shen", "Hui", "Da", "Lie", "Yi", "Xiao", "Bi", "Ci", "Guang", "Yue", "Xiu",
            "Yi",
            "Pai", "Kuai", "Duo", "Ji", "Mie", "Mi", "Zha", "Nong", "Gen", "Mou", "Mai", "Chi", "Lao", "Geng", "En",
            "Zha",
            "Suo", "Zao", "Xi", "Zuo", "Ji", "Feng", "Ze", "Nuo", "Miao", "Lin", "Zhuan", "Zhou", "Tao", "Hu", "Cui",
            "Sha",
            "Yo", "Dan", "Bo", "Ding", "Lang", "Li", "Shua", "Chuo", "Die", "Da", "Nan", "Li", "Kui", "Jie", "Yong",
            "Kui",
            "Jiu", "Sou", "Yin", "Chi", "Jie", "Lou", "Ku", "Wo", "Hui", "Qin", "Ao", "Su", "Du", "Ke", "Nie", "He",
            "Chen", "Suo", "Ge", "A", "En", "Hao", "Dia", "Ai", "Ai", "Suo", "Hei", "Tong", "Chi", "Pei", "Lei", "Cao",
            "Piao", "Qi", "Ying", "Beng", "Sou", "Di", "Mi", "Peng", "Jue", "Liao", "Pu", "Chuai", "Jiao", "O", "Qin",
            "Lu",
            "Ceng", "Deng", "Hao", "Jin", "Jue", "Yi", "Sai", "Pi", "Ru", "Cha", "Huo", "Nang", "Wei", "Jian", "Nan",
            "Lun",
            "Hu", "Ling", "You", "Yu", "Qing", "Yu", "Huan", "Wei", "Zhi", "Pei", "Tang", "Dao", "Ze", "Guo", "Wei",
            "Wo",
            "Man", "Zhang", "Fu", "Fan", "Ji", "Qi", "Qian", "Qi", "Qu", "Ya", "Xian", "Ao", "Cen", "Lan", "Ba", "Hu",
            "Ke", "Dong", "Jia", "Xiu", "Dai", "Gou", "Mao", "Min", "Yi", "Dong", "Qiao", "Xun", "Zheng", "Lao", "Lai",
            "Song",
            "Yan", "Gu", "Xiao", "Guo", "Kong", "Jue", "Rong", "Yao", "Wai", "Zai", "Wei", "Yu", "Cuo", "Lou", "Zi",
            "Mei",
            "Sheng", "Song", "Ji", "Zhang", "Lin", "Deng", "Bin", "Yi", "Dian", "Chi", "Pang", "Cu", "Xun", "Yang",
            "Hou", "Lai",
            "Xi", "Chang", "Huang", "Yao", "Zheng", "Jiao", "Qu", "San", "Fan", "Qiu", "An", "Guang", "Ma", "Niu", "Yun",
            "Xia",
            "Pao", "Fei", "Rong", "Kuai", "Shou", "Sun", "Bi", "Juan", "Li", "Yu", "Xian", "Yin", "Suan", "Yi", "Guo",
            "Luo",
            "Ni", "She", "Cu", "Mi", "Hu", "Cha", "Wei", "Wei", "Mei", "Nao", "Zhang", "Jing", "Jue", "Liao", "Xie",
            "Xun",
            "Huan", "Chuan", "Huo", "Sun", "Yin", "Dong", "Shi", "Tang", "Tun", "Xi", "Ren", "Yu", "Chi", "Yi", "Xiang",
            "Bo",
            "Yu", "Hun", "Zha", "Sou", "Mo", "Xiu", "Jin", "San", "Zhuan", "Nang", "Pi", "Wu", "Gui", "Pao", "Xiu",
            "Xiang",
            "Tuo", "An", "Yu", "Bi", "Geng", "Ao", "Jin", "Chan", "Xie", "Lin", "Ying", "Shu", "Dao", "Cun", "Chan",
            "Wu",
            "Zhi", "Ou", "Chong", "Wu", "Kai", "Chang", "Chuang", "Song", "Bian", "Niu", "Hu", "Chu", "Peng", "Da",
            "Yang", "Zuo",
            "Ni", "Fu", "Chao", "Yi", "Yi", "Tong", "Yan", "Ce", "Kai", "Xun", "Ke", "Yun", "Bei", "Song", "Qian", "Kui",
            "Kun", "Yi", "Ti", "Quan", "Qie", "Xing", "Fei", "Chang", "Wang", "Chou", "Hu", "Cui", "Yun", "Kui", "E",
            "Leng",
            "Zhui", "Qiao", "Bi", "Su", "Qie", "Yong", "Jing", "Qiao", "Chong", "Chu", "Lin", "Meng", "Tian", "Hui",
            "Shuan", "Yan",
            "Wei", "Hong", "Min", "Kang", "Ta", "Lv", "Kun", "Jiu", "Lang", "Yu", "Chang", "Xi", "Wen", "Hun", "E", "Qu",
            "Que", "He", "Tian", "Que", "Kan", "Jiang", "Pan", "Qiang", "San", "Qi", "Si", "Cha", "Feng", "Yuan", "Mu",
            "Mian",
            "Dun", "Mi", "Gu", "Bian", "Wen", "Hang", "Wei", "Le", "Gan", "Shu", "Long", "Lu", "Yang", "Si", "Duo",
            "Ling",
            "Mao", "Luo", "Xuan", "Pan", "Duo", "Hong", "Min", "Jing", "Huan", "Wei", "Lie", "Jia", "Zhen", "Yin", "Hui",
            "Zhu",
            "Ji", "Xu", "Hui", "Tao", "Xun", "Jiang", "Liu", "Hu", "Xun", "Ru", "Su", "Wu", "Lai", "Wei", "Zhuo", "Juan",
            "Cen", "Bang", "Xi", "Mei", "Huan", "Zhu", "Qi", "Xi", "Song", "Du", "Zhuo", "Pei", "Mian", "Gan", "Fei",
            "Cong",
            "Shen", "Guan", "Lu", "Shuan", "Xie", "Yan", "Mian", "Qiu", "Sou", "Huang", "Xu", "Pen", "Jian", "Xuan",
            "Wo", "Mei",
            "Yan", "Qin", "Ke", "She", "Mang", "Ying", "Pu", "Li", "Ru", "Ta", "Hun", "Bi", "Xiu", "Fu", "Tang", "Pang",
            "Ming", "Huang", "Ying", "Xiao", "Lan", "Cao", "Hu", "Luo", "Huan", "Lian", "Zhu", "Yi", "Lu", "Xuan", "Gan",
            "Shu",
            "Si", "Shan", "Shao", "Tong", "Chan", "Lai", "Sui", "Li", "Dan", "Chan", "Lian", "Ru", "Pu", "Bi", "Hao",
            "Zhuo",
            "Han", "Xie", "Ying", "Yue", "Fen", "Hao", "Ba", "Bao", "Gui", "Dang", "Mi", "You", "Chen", "Ning", "Jian",
            "Qian",
            "Wu", "Liao", "Qian", "Huan", "Jian", "Jian", "Zou", "Ya", "Wu", "Jiong", "Ze", "Yi", "Er", "Jia", "Jing",
            "Dai",
            "Hou", "Pang", "Bu", "Li", "Qiu", "Xiao", "Ti", "Qun", "Kui", "Wei", "Huan", "Lu", "Chuan", "Huang", "Qiu",
            "Xia",
            "Ao", "Gou", "Ta", "Liu", "Xian", "Lin", "Ju", "Xie", "Miao", "Sui", "La", "Ji", "Hui", "Tuan", "Zhi", "Kao",
            "Zhi", "Ji", "E", "Chan", "Xi", "Ju", "Chan", "Jing", "Nu", "Mi", "Fu", "Bi", "Yu", "Che", "Shuo", "Fei",
            "Yan", "Wu", "Yu", "Bi", "Jin", "Zi", "Gui", "Niu", "Yu", "Si", "Da", "Zhou", "Shan", "Qie", "Ya", "Rao",
            "Shu", "Luan", "Jiao", "Pin", "Cha", "Li", "Ping", "Wa", "Xian", "Suo", "Di", "Wei", "E", "Jing", "Biao",
            "Jie",
            "Chang", "Bi", "Chan", "Nu", "Ao", "Yuan", "Ting", "Wu", "Gou", "Mo", "Pi", "Ai", "Pin", "Chi", "Li", "Yan",
            "Qiang", "Piao", "Chang", "Lei", "Zhang", "Xi", "Shan", "Bi", "Niao", "Mo", "Shuang", "Ga", "Ga", "Fu", "Nu",
            "Zi",
            "Jie", "Jue", "Bao", "Zang", "Si", "Fu", "Zou", "Yi", "Nu", "Dai", "Xiao", "Hua", "Pian", "Li", "Qi", "Ke",
            "Zhui", "Can", "Zhi", "Wu", "Ao", "Liu", "Shan", "Biao", "Cong", "Chan", "Ji", "Xiang", "Jiao", "Yu", "Zhou",
            "Ge",
            "Wan", "Kuang", "Yun", "Pi", "Shu", "Gan", "Xie", "Fu", "Zhou", "Fu", "Chu", "Dai", "Ku", "Hang", "Jiang",
            "Geng",
            "Xiao", "Ti", "Ling", "Qi", "Fei", "Shang", "Gun", "Duo", "Shou", "Liu", "Quan", "Wan", "Zi", "Ke", "Xiang",
            "Ti",
            "Miao", "Hui", "Si", "Bian", "Gou", "Zhui", "Min", "Jin", "Zhen", "Ru", "Gao", "Li", "Yi", "Jian", "Bin",
            "Piao",
            "Man", "Lei", "Miao", "Sao", "Xie", "Liao", "Zeng", "Jiang", "Qian", "Qiao", "Huan", "Zuan", "Yao", "Ji",
            "Chuan", "Zai",
            "Yong", "Ding", "Ji", "Wei", "Bin", "Min", "Jue", "Ke", "Long", "Dian", "Dai", "Po", "Min", "Jia", "Er",
            "Gong",
            "Xu", "Ya", "Heng", "Yao", "Luo", "Xi", "Hui", "Lian", "Qi", "Ying", "Qi", "Hu", "Kun", "Yan", "Cong", "Wan",
            "Chen", "Ju", "Mao", "Yu", "Yuan", "Xia", "Nao", "Ai", "Tang", "Jin", "Huang", "Ying", "Cui", "Cong", "Xuan",
            "Zhang",
            "Pu", "Can", "Qu", "Lu", "Bi", "Zan", "Wen", "Wei", "Yun", "Tao", "Wu", "Shao", "Qi", "Cha", "Ma", "Li",
            "Pi", "Miao", "Yao", "Rui", "Jian", "Chu", "Cheng", "Cong", "Xiao", "Fang", "Pa", "Zhu", "Nai", "Zhi", "Zhe",
            "Long",
            "Jiu", "Ping", "Lu", "Xia", "Xiao", "You", "Zhi", "Tuo", "Zhi", "Ling", "Gou", "Di", "Li", "Tuo", "Cheng",
            "Kao",
            "Lao", "Ya", "Rao", "Zhi", "Zhen", "Guang", "Qi", "Ting", "Gua", "Jiu", "Hua", "Heng", "Gui", "Jie", "Luan",
            "Juan",
            "An", "Xu", "Fan", "Gu", "Fu", "Jue", "Zi", "Suo", "Ling", "Chu", "Fen", "Du", "Qian", "Zhao", "Luo", "Chui",
            "Liang", "Guo", "Jian", "Di", "Ju", "Cou", "Zhen", "Nan", "Zha", "Lian", "Lan", "Ji", "Pin", "Ju", "Qiu",
            "Duan",
            "Chui", "Chen", "Lv", "Cha", "Ju", "Xuan", "Mei", "Ying", "Zhen", "Fei", "Ta", "Sun", "Xie", "Gao", "Cui",
            "Gao",
            "Shuo", "Bin", "Rong", "Zhu", "Xie", "Jin", "Qiang", "Qi", "Chu", "Tang", "Zhu", "Hu", "Gan", "Yue", "Qing",
            "Tuo",
            "Jue", "Qiao", "Qin", "Lu", "Zun", "Xi", "Ju", "Yuan", "Lei", "Yan", "Lin", "Bo", "Cha", "You", "Ao", "Mo",
            "Cu", "Shang", "Tian", "Yun", "Lian", "Piao", "Dan", "Ji", "Bin", "Yi", "Ren", "E", "Gu", "Ke", "Lu", "Zhi",
            "Yi", "Zhen", "Hu", "Li", "Yao", "Shi", "Zhi", "Quan", "Lu", "Zhe", "Nian", "Wang", "Chuo", "Zi", "Cou",
            "Lu",
            "Lin", "Wei", "Jian", "Qiang", "Jia", "Ji", "Ji", "Kan", "Deng", "Gai", "Jian", "Zang", "Ou", "Ling", "Bu",
            "Beng",
            "Zeng", "Pi", "Po", "Ga", "La", "Gan", "Hao", "Tan", "Gao", "Ze", "Xin", "Yun", "Gui", "He", "Zan", "Mao",
            "Yu", "Chang", "Ni", "Qi", "Sheng", "Ye", "Chao", "Yan", "Hui", "Bu", "Han", "Gui", "Xuan", "Kui", "Ai",
            "Ming",
            "Tun", "Xun", "Yao", "Xi", "Nang", "Ben", "Shi", "Kuang", "Yi", "Zhi", "Zi", "Gai", "Jin", "Zhen", "Lai",
            "Qiu",
            "Ji", "Dan", "Fu", "Chan", "Ji", "Xi", "Di", "Yu", "Gou", "Jin", "Qu", "Jian", "Jiang", "Pin", "Mao", "Gu",
            "Wu", "Gu", "Ji", "Ju", "Jian", "Pian", "Kao", "Qie", "Suo", "Bai", "Ge", "Bo", "Mao", "Mu", "Cui", "Jian",
            "San", "Shu", "Chang", "Lu", "Pu", "Qu", "Pie", "Dao", "Xian", "Chuan", "Dong", "Ya", "Yin", "Ke", "Yun",
            "Fan",
            "Chi", "Jiao", "Du", "Die", "You", "Yuan", "Guo", "Yue", "Wo", "Rong", "Huang", "Jing", "Ruan", "Tai",
            "Gong", "Zhun",
            "Na", "Yao", "Qian", "Long", "Dong", "Ka", "Lu", "Jia", "Shen", "Zhou", "Zuo", "Gua", "Zhen", "Qu", "Zhi",
            "Jing",
            "Guang", "Dong", "Yan", "Kuai", "Sa", "Hai", "Pian", "Zhen", "Mi", "Tun", "Luo", "Cuo", "Pao", "Wan", "Niao",
            "Jing",
            "Yan", "Fei", "Yu", "Zong", "Ding", "Jian", "Cou", "Nan", "Mian", "Wa", "E", "Shu", "Cheng", "Ying", "Ge",
            "Lv",
            "Bin", "Teng", "Zhi", "Chuai", "Gu", "Meng", "Sao", "Shan", "Lian", "Lin", "Yu", "Xi", "Qi", "Sha", "Xin",
            "Xi",
            "Biao", "Sa", "Ju", "Sou", "Biao", "Biao", "Shu", "Gou", "Gu", "Hu", "Fei", "Ji", "Lan", "Yu", "Pei", "Mao",
            "Zhan", "Jing", "Ni", "Liu", "Yi", "Yang", "Wei", "Dun", "Qiang", "Shi", "Hu", "Zhu", "Xuan", "Tai", "Ye",
            "Yang",
            "Wu", "Han", "Men", "Chao", "Yan", "Hu", "Yu", "Wei", "Duan", "Bao", "Xuan", "Bian", "Tui", "Liu", "Man",
            "Shang",
            "Yun", "Yi", "Yu", "Fan", "Sui", "Xian", "Jue", "Cuan", "Huo", "Tao", "Xu", "Xi", "Li", "Hu", "Jiong", "Hu",
            "Fei", "Shi", "Si", "Xian", "Zhi", "Qu", "Hu", "Fu", "Zuo", "Mi", "Zhi", "Ci", "Zhen", "Tiao", "Qi", "Chan",
            "Xi", "Zhuo", "Xi", "Rang", "Te", "Tan", "Dui", "Jia", "Hui", "Nv", "Nin", "Yang", "Zi", "Que", "Qian",
            "Min",
            "Te", "Qi", "Dui", "Mao", "Men", "Gang", "Yu", "Yu", "Ta", "Xue", "Miao", "Ji", "Gan", "Dang", "Hua", "Che",
            "Dun", "Ya", "Zhuo", "Bian", "Feng", "Fa", "Ai", "Li", "Long", "Zha", "Tong", "Di", "La", "Tuo", "Fu",
            "Xing",
            "Mang", "Xia", "Qiao", "Zhai", "Dong", "Nao", "Ge", "Wo", "Qi", "Dui", "Bei", "Ding", "Chen", "Zhou", "Jie",
            "Di",
            "Xuan", "Bian", "Zhe", "Gun", "Sang", "Qing", "Qu", "Dun", "Deng", "Jiang", "Ca", "Meng", "Bo", "Kan", "Zhi",
            "Fu",
            "Fu", "Xu", "Mian", "Kou", "Dun", "Miao", "Dan", "Sheng", "Yuan", "Yi", "Sui", "Zi", "Chi", "Mou", "Lai",
            "Jian",
            "Di", "Suo", "Ya", "Ni", "Sui", "Pi", "Rui", "Sou", "Kui", "Mao", "Ke", "Ming", "Piao", "Cheng", "Kan",
            "Lin",
            "Gu", "Ding", "Bi", "Quan", "Tian", "Fan", "Zhen", "She", "Wan", "Tuan", "Fu", "Gang", "Gu", "Li", "Yan",
            "Pi",
            "Lan", "Li", "Ji", "Zeng", "He", "Guan", "Juan", "Jin", "Ga", "Yi", "Po", "Zhao", "Liao", "Tu", "Chuan",
            "Shan",
            "Men", "Chai", "Nv", "Bu", "Tai", "Ju", "Ban", "Qian", "Fang", "Kang", "Dou", "Huo", "Ba", "Yu", "Zheng",
            "Gu",
            "Ke", "Po", "Bu", "Bo", "Yue", "Mu", "Tan", "Dian", "Shuo", "Shi", "Xuan", "Ta", "Bi", "Ni", "Pi", "Duo",
            "Kao", "Lao", "Er", "You", "Cheng", "Jia", "Nao", "Ye", "Cheng", "Diao", "Yin", "Kai", "Zhu", "Ding", "Diu",
            "Hua",
            "Quan", "Ha", "Sha", "Diao", "Zheng", "Se", "Chong", "Tang", "An", "Ru", "Lao", "Lai", "Te", "Keng", "Zeng",
            "Li",
            "Gao", "E", "Cuo", "Lve", "Liu", "Kai", "Jian", "Lang", "Qin", "Ju", "A", "Qiang", "Nuo", "Ben", "De", "Ke",
            "Kun", "Gu", "Huo", "Pei", "Juan", "Tan", "Zi", "Qie", "Kai", "Si", "E", "Cha", "Sou", "Huan", "Ai", "Lou",
            "Qiang", "Fei", "Mei", "Mo", "Ge", "Juan", "Na", "Liu", "Yi", "Jia", "Bin", "Biao", "Tang", "Man", "Luo",
            "Yong",
            "Chuo", "Xuan", "Di", "Tan", "Jue", "Pu", "Lu", "Dui", "Lan", "Pu", "Cuan", "Qiang", "Deng", "Huo", "Zhuo",
            "Yi",
            "Cha", "Biao", "Zhong", "Shen", "Cuo", "Zhi", "Bi", "Zi", "Mo", "Shu", "Lv", "Ji", "Fu", "Lang", "Ke", "Ren",
            "Zhen", "Ji", "Se", "Nian", "Fu", "Rang", "Gui", "Jiao", "Hao", "Xi", "Po", "Die", "Hu", "Yong", "Jiu",
            "Yuan",
            "Bao", "Zhen", "Gu", "Dong", "Lu", "Qu", "Chi", "Si", "Er", "Zhi", "Gua", "Xiu", "Luan", "Bo", "Li", "Hu",
            "Yu", "Xian", "Ti", "Wu", "Miao", "An", "Bei", "Chun", "Hu", "E", "Ci", "Mei", "Wu", "Yao", "Jian", "Ying",
            "Zhe", "Liu", "Liao", "Jiao", "Jiu", "Yu", "Hu", "Lu", "Guan", "Bing", "Ding", "Jie", "Li", "Shan", "Li",
            "You",
            "Gan", "Ke", "Da", "Zha", "Pao", "Zhu", "Xuan", "Jia", "Ya", "Yi", "Zhi", "Lao", "Wu", "Cuo", "Xian", "Sha",
            "Zhu", "Fei", "Gu", "Wei", "Yu", "Yu", "Dan", "La", "Yi", "Hou", "Chai", "Lou", "Jia", "Sao", "Chi", "Mo",
            "Ban", "Ji", "Huang", "Biao", "Luo", "Ying", "Zhai", "Long", "Yin", "Chou", "Ban", "Lai", "Yi", "Dian", "Pi",
            "Dian",
            "Qu", "Yi", "Song", "Xi", "Qiong", "Zhun", "Bian", "Yao", "Tiao", "Dou", "Ke", "Yu", "Xun", "Ju", "Yu", "Yi",
            "Cha", "Na", "Ren", "Jin", "Mei", "Pan", "Dang", "Jia", "Ge", "Ken", "Lian", "Cheng", "Lian", "Jian", "Biao",
            "Chu",
            "Ti", "Bi", "Ju", "Duo", "Da", "Bei", "Bao", "Lv", "Bian", "Lan", "Chi", "Zhe", "Qiang", "Ru", "Pan", "Ya",
            "Xu", "Jun", "Cun", "Jin", "Lei", "Zi", "Chao", "Si", "Huo", "Lao", "Tang", "Ou", "Lou", "Jiang", "Nou",
            "Mo",
            "Die", "Ding", "Dan", "Ling", "Ning", "Guo", "Kui", "Ao", "Qin", "Han", "Qi", "Hang", "Jie", "He", "Ying",
            "Ke",
            "Han", "E", "Zhuan", "Nie", "Man", "Sang", "Hao", "Ru", "Pin", "Hu", "Qian", "Qiu", "Ji", "Chai", "Hui",
            "Ge",
            "Meng", "Fu", "Pi", "Rui", "Xian", "Hao", "Jie", "Gong", "Dou", "Yin", "Chi", "Han", "Gu", "Ke", "Li", "You",
            "Ran", "Zha", "Qiu", "Ling", "Cheng", "You", "Qiong", "Jia", "Nao", "Zhi", "Si", "Qu", "Ting", "Kuo", "Qi",
            "Jiao",
            "Yang", "Mou", "Shen", "Zhe", "Shao", "Wu", "Li", "Chu", "Fu", "Qiang", "Qing", "Qi", "Xi", "Yu", "Fei",
            "Guo",
            "Guo", "Yi", "Pi", "Tiao", "Quan", "Wan", "Lang", "Meng", "Chun", "Rong", "Nan", "Fu", "Kui", "Ke", "Fu",
            "Sou",
            "Yu", "You", "Lou", "You", "Bian", "Mou", "Qin", "Ao", "Man", "Mang", "Ma", "Yuan", "Xi", "Chi", "Tang",
            "Pang",
            "Shi", "Huang", "Cao", "Piao", "Tang", "Xi", "Xiang", "Zhong", "Zhang", "Shuai", "Mao", "Peng", "Hui", "Pan",
            "Shan", "Huo",
            "Meng", "Chan", "Lian", "Mie", "Li", "Du", "Qu", "Fou", "Ying", "Qing", "Xia", "Shi", "Zhu", "Yu", "Ji",
            "Du",
            "Ji", "Jian", "Zhao", "Zi", "Hu", "Qiong", "Po", "Da", "Sheng", "Ze", "Gou", "Li", "Si", "Tiao", "Jia",
            "Bian",
            "Chi", "Kou", "Bi", "Xian", "Yan", "Quan", "Zheng", "Jun", "Shi", "Gang", "Pa", "Shao", "Xiao", "Qing", "Ze",
            "Qie",
            "Zhu", "Ruo", "Qian", "Tuo", "Bi", "Dan", "Kong", "Wan", "Xiao", "Zhen", "Kui", "Huang", "Hou", "Gou", "Fei",
            "Li",
            "Bi", "Chi", "Su", "Mie", "Dou", "Lu", "Duan", "Gui", "Dian", "Zan", "Deng", "Bo", "Lai", "Zhou", "Yu", "Yu",
            "Chong", "Xi", "Nie", "Nv", "Chuan", "Shan", "Yi", "Bi", "Zhong", "Ban", "Fang", "Ge", "Lu", "Zhu", "Ze",
            "Xi",
            "Shao", "Wei", "Meng", "Shou", "Cao", "Chong", "Meng", "Qin", "Niao", "Jia", "Qiu", "Sha", "Bi", "Di",
            "Qiang", "Suo",
            "Jie", "Tang", "Xi", "Xian", "Mi", "Ba", "Li", "Tiao", "Xi", "Zi", "Can", "Lin", "Zong", "San", "Hou", "Zan",
            "Ci", "Xu", "Rou", "Qiu", "Jiang", "Gen", "Ji", "Yi", "Ling", "Xi", "Zhu", "Fei", "Jian", "Pian", "He", "Yi",
            "Jiao", "Zhi", "Qi", "Qi", "Yao", "Dao", "Fu", "Qu", "Jiu", "Ju", "Lie", "Zi", "Zan", "Nan", "Zhe", "Jiang",
            "Chi", "Ding", "Gan", "Zhou", "Yi", "Gu", "Zuo", "Tuo", "Xian", "Ming", "Zhi", "Yan", "Shai", "Cheng", "Tu",
            "Lei",
            "Kun", "Pei", "Hu", "Ti", "Xu", "Hai", "Tang", "Lao", "Bu", "Jiao", "Xi", "Ju", "Li", "Xun", "Shi", "Cuo",
            "Dun", "Qiong", "Xue", "Cu", "Bie", "Bo", "Ta", "Jian", "Fu", "Qiang", "Zhi", "Fu", "Shan", "Li", "Tuo",
            "Jia",
            "Bo", "Tai", "Kui", "Qiao", "Bi", "Xian", "Xian", "Ji", "Jiao", "Liang", "Ji", "Chuo", "Huai", "Chi", "Zhi",
            "Dian",
            "Bo", "Zhi", "Jian", "Die", "Chuai", "Zhong", "Ju", "Duo", "Cuo", "Pian", "Rou", "Nie", "Pan", "Qi", "Chu",
            "Jue",
            "Pu", "Fan", "Cu", "Zhu", "Lin", "Chan", "Lie", "Zuan", "Xie", "Zhi", "Diao", "Mo", "Xiu", "Mo", "Pi", "Hu",
            "Jue", "Shang", "Gu", "Zi", "Gong", "Su", "Zhi", "Zi", "Qing", "Liang", "Yu", "Li", "Wen", "Ting", "Ji",
            "Pei",
            "Fei", "Sha", "Yin", "Ai", "Xian", "Mai", "Chen", "Ju", "Bao", "Tiao", "Zi", "Yin", "Yu", "Chuo", "Wo",
            "Mian",
            "Yuan", "Tuo", "Zhui", "Sun", "Jun", "Ju", "Luo", "Qu", "Chou", "Qiong", "Luan", "Wu", "Zan", "Mou", "Ao",
            "Liu",
            "Bei", "Xin", "You", "Fang", "Ba", "Ping", "Nian", "Lu", "Su", "Fu", "Hou", "Tai", "Gui", "Jie", "Wei", "Er",
            "Ji", "Jiao", "Xiang", "Xun", "Geng", "Li", "Lian", "Jian", "Shi", "Tiao", "Gun", "Sha", "Huan", "Ji",
            "Qing", "Ling",
            "Zou", "Fei", "Kun", "Chang", "Gu", "Ni", "Nian", "Diao", "Shi", "Zi", "Fen", "Die", "E", "Qiu", "Fu",
            "Huang",
            "Bian", "Sao", "Ao", "Qi", "Ta", "Guan", "Yao", "Le", "Biao", "Xue", "Man", "Min", "Yong", "Gui", "Shan",
            "Zun",
            "Li", "Da", "Yang", "Da", "Qiao", "Man", "Jian", "Ju", "Rou", "Gou", "Bei", "Jie", "Tou", "Ku", "Gu", "Di",
            "Hou", "Ge", "Ke", "Bi", "Lou", "Qia", "Kuan", "Bin", "Du", "Mei", "Ba", "Yan", "Liang", "Xiao", "Wang",
            "Chi",
            "Xiang", "Yan", "Tie", "Tao", "Yong", "Biao", "Kun", "Mao", "Ran", "Tiao", "Ji", "Zi", "Xiu", "Quan", "Jiu",
            "Bin",
            "Huan", "Lie", "Me", "Hui", "Mi", "Ji", "Jun", "Zhu", "Mi", "Qi", "Ao", "She", "Lin", "Dai", "Chu", "You",
            "Xia", "Yi", "Qu", "Du", "Li", "Qing", "Can", "An", "Fen", "You", "Wu", "Yan", "Xi", "Qiu", "Han", "Zha"
        };

        #endregion 二级汉字

        #region 变量定义

        /// <summary>
        ///  GB2312-80 标准规范中第一个汉字的机内码.即"啊"的机内码
        /// </summary>
        private const int FirstChCode = -20319;
        /// <summary>
        /// GB2312-80 标准规范中最后一个汉字的机内码.即"齄"的机内码
        /// </summary>
        private const int LastChCode = -2050;
        /// <summary>
        /// GB2312-80 标准规范中最后一个一级汉字的机内码.即"座"的机内码
        /// </summary>
        private const int LastOfOneLevelChCode = -10247;
        // 配置中文字符
        //static Regex regex = new Regex("[\u4e00-\u9fa5]$");

        #endregion

        #endregion

        /// <summary>
        /// 取第一个拼音字母
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static string GetFirstLetterPinyin(char ch)
        {
            var rs = GetSingWordPinyin(ch);
            if (!String.IsNullOrEmpty(rs)) rs = rs.Substring(0, 1);

            return rs;
        }

        /// <summary>
        /// 取每个字的第一个拼音字母
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetEachFirstLetterPinyin(string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;

            var sb = new StringBuilder(str.Length + 1);
            var chs = str.ToCharArray();

            for (var i = 0; i < chs.Length; i++)
            {
                sb.Append(GetFirstLetterPinyin(chs[i]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取单字拼音
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static string GetSingWordPinyin(char ch)
        {
            // 拉丁字符
            if (ch <= '\x00FF') return ch.ToString();

            // 标点符号、分隔符
            if (Char.IsPunctuation(ch) || Char.IsSeparator(ch)) return ch.ToString();

            // 非中文字符
            if (ch < '\x4E00' || ch > '\x9FA5') return ch.ToString();
            //注册一个提供字符程序
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //获取中文的GB2312的字符集在.net core中需要手动的注册
            var arr = Encoding.GetEncoding("gb2312").GetBytes(ch.ToString());
            //Encoding.Default默认在中文环境里虽是GB2312，但在多变的环境可能是其它
            //var arr = Encoding.Default.GetBytes(ch.ToString()); 
            var chr = arr[0] * 256 + arr[1] - 65536;

            //***// 单字符--英文或半角字符
            if (chr > 0 && chr < 160) return ch.ToString();

            #region 中文字符处理

            // 判断是否超过GB2312-80标准中的汉字范围
            if (chr > LastChCode || chr < FirstChCode)
            {
                return ch.ToString();
            }
            // 如果是在一级汉字中
            else if (chr <= LastOfOneLevelChCode)
            {
                // 将一级汉字分为12块,每块33个汉字.
                for (int aPos = 11; aPos >= 0; aPos--)
                {
                    int aboutPos = aPos * 33;
                    // 从最后的块开始扫描,如果机内码大于块的第一个机内码,说明在此块中
                    if (chr >= PyValue[aboutPos])
                    {
                        // Console.WriteLine("存在于第 " + aPos.ToString() + " 块,此块的第一个机内码是: " + pyValue[aPos * 33].ToString());
                        // 遍历块中的每个音节机内码,从最后的音节机内码开始扫描,
                        // 如果音节内码小于机内码,则取此音节
                        for (int i = aboutPos + 32; i >= aboutPos; i--)
                        {
                            if (PyValue[i] <= chr)
                            {
                                // Console.WriteLine("找到第一个小于要查找机内码的机内码: " + pyValue[i].ToString());
                                return PyName[i];
                            }
                        }
                        break;
                    }
                }
            }
            // 如果是在二级汉字中
            else
            {
                int pos = Array.IndexOf(OtherChinese, ch.ToString());
                if (pos != decimal.MinusOne)
                {
                    return OtherPinYin[pos];
                }
            }

            #endregion 中文字符处理
            return String.Empty;
        }

        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="str">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string GetFullPinyin(string str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;

            var sb = new StringBuilder(str.Length * 10);
            var chs = str.ToCharArray();

            for (var j = 0; j < chs.Length; j++)
            {
                sb.Append(GetSingWordPinyin(chs[j]));
            }

            return sb.ToString();
        }
        #endregion

        #region [3、数据类型转换]

        #region [3.1 dr,dt, to model, to list model]
        /// <summary>
        /// DataRow转实体
        /// </summary>
        /// <typeparam name="T">数据型类</typeparam>
        /// <param name="dr">DataRow</param>
        /// <returns>模式</returns>
        public static T ToModel<T>(DataRow dr) where T : new()
        {
            T t = new T();
            if (dr == null) return default(T);
            // 获得此模型的公共属性
            PropertyInfo[] propertys = t.GetType().GetProperties();
            DataColumnCollection columns = dr.Table.Columns;
            foreach (PropertyInfo p in propertys)
            {
                string columnName = p.Name;

                if (columns.Contains(columnName))
                {
                    // 判断此属性是否有Setter或columnName值是否为空
                    object value = dr[columnName];
                    if (!p.CanWrite || value is DBNull || value == DBNull.Value) continue;
                    try
                    {
                        switch (p.PropertyType.ToString())
                        {
                            case "System.String":
                                p.SetValue(t, Convert.ToString(value), null);
                                break;
                            case "System.Int32":
                                p.SetValue(t, Convert.ToInt32(value), null);
                                break;
                            case "System.Int64":
                                p.SetValue(t, Convert.ToInt64(value), null);
                                break;
                            case "System.DateTime":
                            case "System.Nullable`1[System.DateTime]":
                                p.SetValue(t, Convert.ToDateTime(value), null);
                                break;
                            case "System.Boolean":
                                p.SetValue(t, Convert.ToBoolean(value), null);
                                break;
                            case "System.Double":
                                p.SetValue(t, Convert.ToDouble(value), null);
                                break;
                            case "System.Decimal":
                                p.SetValue(t, Convert.ToDecimal(value), null);
                                break;
                            case "System.TimeSpan":
                                p.SetValue(t, TimeSpan.Parse(value.ToString()), null);
                                break;
                            default:
                                p.SetValue(t, value, null);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            return t;
        }

        /// <summary>
        /// DataTable 转化为对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static TEntity ToEntity<TEntity>(DataTable dt) where TEntity : new()
        {
            TEntity t = default(TEntity);
            if (dt != null && dt.Rows.Count > 0)
            {
                int fieldCount = dt.Columns.Count;
                DataRow item = dt.Rows[0];
                t = (TEntity)Activator.CreateInstance(typeof(TEntity));

                for (int i = 0; i < fieldCount; i++)
                {
                    PropertyInfo field = t.GetType().GetProperty(dt.Columns[i].ColumnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (field != null)
                    {
                        if (item[i] == null || Convert.IsDBNull(item[i]))
                        {
                            field.SetValue(t, null, null);
                        }
                        else
                        {
                            field.SetValue(t, item[i], null);
                        }
                    }
                }
            }

            return t;
        }

        /// <summary>
        /// IDataReader转化为对象
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static TEntity ToEntity<TEntity>(IDataReader dr) where TEntity : new()
        {
            TEntity t = default(TEntity);
            EntityUtilCache<TEntity>.EmitInvoker(t, dr);
            return t;
        }

        /// <summary>
        /// IDataReader 转化为对象集合
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static List<TEntity> ToList<TEntity>(IDataReader dr) where TEntity : new()
        {
            var listEntity = new List<TEntity>();
            if (dr != null)
            {
                while (dr.Read())
                {
                    TEntity t = default(TEntity);
                    t = EntityUtilCache<TEntity>.EmitInvoker(t, dr);
                    listEntity.Add(t);
                }
            }
            return listEntity;
        }

        #region [3.1.2 比较慢的方法]
        /// <summary>
        /// <para>表格转集合</para>
        /// <para>DataTable中的列名称自动匹配TResult"/>中的属性</para>
        /// <para>当数据量大于100时，请用<see cref="ToListFast{TResult}"/></para>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dt"></param>
        /// <author>FreshMan</author>
        /// <creattime>2017-06-26</creattime>
        /// <returns></returns>
        private static List<TResult> ToListSlowly<TResult>(DataTable dt) where TResult : class, new()
        {
            //初始化转换对象
            List<TResult> resulteList = new List<TResult>();
            if (dt == null) return default(List<TResult>);
            //获取此模型的公共属性
            PropertyInfo[] propertys = typeof(TResult).GetProperties();
            DataColumnCollection columns = dt.Columns;
            foreach (DataRow dataRow in dt.Rows)
            {
                TResult t = new TResult();
                foreach (PropertyInfo p in propertys)
                {
                    string columnName = p.Name;
                    if (!columns.Contains(columnName)) continue;
                    //判断此属性是否有Setting或columnName值是否为空
                    object value = dataRow[columnName];
                    if (!p.CanWrite || value is DBNull || value == DBNull.Value || (!p.PropertyType.IsValueType && p.PropertyType != typeof(string))) continue;
                    try
                    {
                        switch (p.PropertyType.ToString())
                        {
                            case "System.String":
                                p.SetValue(t, Convert.ToString(value), null);
                                break;
                            case "System.Int32":
                                p.SetValue(t, Convert.ToInt32(value), null);
                                break;
                            case "System.Int64":
                                p.SetValue(t, Convert.ToInt64(value), null);
                                break;
                            case "System.DateTime":
                            case "System.Nullable`1[System.DateTime]":
                                p.SetValue(t, Convert.ToDateTime(value), null);
                                break;
                            case "System.Boolean":
                                p.SetValue(t, Convert.ToBoolean(value), null);
                                break;
                            case "System.Double":
                                p.SetValue(t, Convert.ToDouble(value), null);
                                break;
                            case "System.Decimal":
                                p.SetValue(t, Convert.ToDecimal(value), null);
                                break;
                            case "System.TimeSpan":
                                p.SetValue(t, TimeSpan.Parse(value.ToString()), null);
                                break;
                            default:
                                p.SetValue(t, value, null);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                resulteList.Add(t);
            }
            return resulteList;
        }
        #endregion

        #region [3.1.3 比较快的方法]
        /// <summary>
        /// <para>表格转集合</para>
        /// <para>DataTable中的列名称自动匹配<see cref="TResult"/>中的属性</para>
        /// <para>当数据量小于100是，请用<see cref="ToListSlowly{TResult}"/></para>
        /// <para>如果数据类型错误异常<see cref="ArgumentException"/></para>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="table"></param>
        /// <author>FreshMan</author>
        /// <creattime>2017-06-26</creattime>
        /// <returns></returns>
        private static List<TResult> ToListFast<TResult>(DataTable table) where TResult : class, new()
        {
            List<TResult> list = new List<TResult>();
            if (table == null || table.Rows.Count < 1) return list;
            DataTable dt = DataTableCreator<TResult>.GetDataTable();
            var oldColums = table.Columns;
            var newColums = dt.Columns;
            DataTableEntityBuilder<TResult> eblist = null;
            //行计数器
            long rowNum = 0;
            foreach (DataRow dataRow in table.Rows)
            {
                var dtRow = dt.NewRow();
                var flag = false;
                foreach (DataColumn dataColumn in oldColums.Cast<DataColumn>().Where(dataColumn => newColums.Contains(dataColumn.ColumnName)))
                {
                    flag = true;
                    try
                    {
                        dtRow[dataColumn.ColumnName] = dataRow[dataColumn.ColumnName];
                    }
                    catch (ArgumentException argumentException)
                    {
                        // ReSharper disable once PossibleIntendedRethrow
                        var exception =
                            new ArgumentException(
                                argumentException.Message + "context data is " +
                                dataRow.ItemArray.Aggregate(
                                    (current, temp) => current.ToString() + "," + temp.ToString()), argumentException);
                        throw exception;
                    }
                    catch (Exception exception)
                    {
                        // ReSharper disable once PossibleIntendedRethrow
                        throw exception;
                    }
                }
                if (flag && rowNum == 0)
                {
                    eblist = DataTableEntityBuilder<TResult>.CreateBuilder(dtRow);
                }
                if (!flag) continue;
                rowNum++;
                // ReSharper disable once PossibleNullReferenceException
                TResult tempInfo = eblist.Build(dtRow);
                list.Add(tempInfo);
            }
            dt.Dispose();
            return list;
        }

        /// <summary>
        /// <para>表格转集合</para>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="table"></param>
        /// <author>FreshMan</author>
        /// <creattime>2017-06-29</creattime>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(DataTable table) where TResult : class, new()
        {
            //初始化转换对象
            List<TResult> list = new List<TResult>();
            if (table == null || table.Rows.Count < 1) return list;
            return table.Rows.Count > 100 ? ToListFast<TResult>(table) : ToListSlowly<TResult>(table);
        }
        #endregion
        #endregion

        #region [3.2 dt to dynamic]
        /// <summary>
        /// 获取动态类型数据集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<dynamic> ToDynamicList(DataTable dt)
        {
            if (dt == null)
            {
                return null;
            }

            var list = new List<dynamic>();
            foreach (DataRow dataRow in dt.Rows)
            {
                dynamic dynamicEntity = null;
                foreach (DataColumn column in dt.Columns)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    dynamicEntity[column.ColumnName] = dataRow[column];
                }
                list.Add(dynamicEntity);
            }
            return list;
        }

        /// <summary>
        /// 获取动态类型数据集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static dynamic ToDynamic(DataTable dt)
        {
            if (dt == null || dt.Rows.Count > 0)
            {
                return null;
            }

            dynamic dynamicEntity = null;
            foreach (DataColumn column in dt.Columns)
            {
                // ReSharper disable once PossibleNullReferenceException
                dynamicEntity[column.ColumnName] = dt.Rows[0][column];
            }
            return dynamicEntity;
        }
        #endregion

        #region [3.3 fast map]
        /// <summary>
        /// 把source实体对象中的数据按（同名同类型的属性）规则复制到TTarget类型的新实体对象
        /// </summary>
        /// <typeparam name="TTarget">目标实体类型</typeparam>
        /// <typeparam name="TSource">源实体类型</typeparam>
        /// <param name="source">源实体对象</param>
        /// <returns>目标实体对象</returns>
        public static TTarget FastMap<TTarget, TSource>(TSource source)
        {
            if (source != null)
            {
                return FastMapper<TTarget, TSource>.MapReturnMethod(source);
            }

            return default(TTarget);
        }

        /// <summary>
        /// 把source实体对象中的数据按（同名同类型的属性）规则复制到target实体对象
        /// </summary>
        /// <typeparam name="TTarget">目标实体类型</typeparam>
        /// <typeparam name="TSource">源实体类型</typeparam>
        /// <param name="target">目标实体对象</param>
        /// <param name="source">源实体对象</param>
        public static void FastMap<TTarget, TSource>(TTarget target, TSource source)
        {
            if (source != null && target != null)
            {
                FastMapper<TTarget, TSource>.MapMethod(target, source);
            }
        }
        #endregion

        #region [3.4 Get entity difference]
        /// <summary>
        /// 获取两个实体的属性间的不同值，返回不同内容描述
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">原实体</param>
        /// <param name="target">目的实体</param>
        /// <returns>不同内容描述</returns>
        internal static string GetEntityDifference<T>(T source, T target)
        {
            if (source != null && target != null)
            {
                return EntityUtilCache<T>.GetEntityDifferenceInvoker(source, target);
            }
            return "";
        }
        #endregion

        #region [3.5 IEnumerable to dataTable]
        #region [3.5.1 比较慢的方法]
        /// <summary>
        /// <para>集合转化为表格</para>
        /// <para>T中应该只包含值类型，对应的DataTable自动匹配列名相同的属性</para>
        /// <para>当数据量大于100时，请用<see cref="ToDataTable{TSource}"/></para>
        /// </summary>
        /// <typeparam name="T">类型中不应该包含有引用类型</typeparam>
        /// <param name="entityList">转换的集合</param> 
        /// <author>FreshMan</author>
        /// <creattime>2017-06-26</creattime>
        /// <returns></returns>
        public static DataTable ToDataTableSlowly<T>(IList<T> entityList)
        {
            if (entityList == null) return null;
            var dt = CreateTable<T>();
            Type entityType = typeof(T);
            var properties = entityType.GetProperties();
            foreach (T item in entityList)
            {
                DataRow row = dt.NewRow();
                foreach (var property in properties)
                {
                    if (!property.CanRead || (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))) continue;
                    row[property.Name] = property.GetValue(item, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// <para>创建表格</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <author>FreshMan</author>
        /// <creattime>2017-06-26</creattime>
        /// <returns></returns>
        private static DataTable CreateTable<T>(string tableName = null)
        {
            Type entityType = typeof(T);
            PropertyDescriptorCollection propertyies = TypeDescriptor.GetProperties(entityType);
            DataTable dt = new DataTable(tableName);
            foreach (PropertyDescriptor prop in propertyies)
            {
                dt.Columns.Add(prop.Name);
            }
            return dt;
        }
        #endregion

        #region [3.5.2 比较快的方法]
        /// <summary>
        /// <para>Creates a DataTable from an IEnumerable</para>
        /// <para>当数据量小于100时，请用<see cref="ToDataTableSlowly{TSource}"/></para>
        /// </summary>
        /// <typeparam name="TSource">The Generic type of the Collection</typeparam>
        /// <param name="collection"></param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<TSource>(IEnumerable<TSource> collection)
        {
            DataTable dt = DataTableCreator<TSource>.GetDataTable();
            Func<TSource, object[]> map = DataRowMapperCache<TSource>.GetDataRowMapper(dt);

            foreach (TSource item in collection)
            {
                dt.Rows.Add(map(item));
            }
            return dt;
        }

        /// <summary>
        /// 使用泛型类型创建一个同样字段名的DataTable结构
        /// </summary>
        /// <typeparam name="TSource">泛型类型</typeparam>
        /// <returns>DataTable</returns>
        static internal DataTable CreateDataTable<TSource>()
        {
            DataTable dt = new DataTable();
            foreach (FieldInfo sourceMember in typeof(TSource).GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                dt.AddTableColumn(sourceMember, sourceMember.FieldType);
            }

            foreach (PropertyInfo sourceMember in typeof(TSource).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (sourceMember.CanRead)
                {
                    dt.AddTableColumn(sourceMember, sourceMember.PropertyType);
                }
            }
            return dt;
        }

        /// <summary>
        /// 只将值类型和string类型添加一列到DataTable中
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="sourceMember">列对象</param>
        /// <param name="memberType">列类型</param>
        private static void AddTableColumn(this DataTable dt, MemberInfo sourceMember, Type memberType)
        {
            if ((memberType.IsValueType || memberType == typeof(string)))
            {
                DataColumn dc;
                string fieldName = GetFieldNameAttribute(sourceMember);
                if (string.IsNullOrWhiteSpace(fieldName))
                {
                    fieldName = sourceMember.Name;
                }
                if (Nullable.GetUnderlyingType(memberType) == null)
                {
                    dc = new DataColumn(fieldName, memberType) { AllowDBNull = !memberType.IsValueType };
                }
                else
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    dc = new DataColumn(fieldName, Nullable.GetUnderlyingType(memberType)) { AllowDBNull = true };
                }
                dt.Columns.Add(dc);
            }
        }

        /// <summary>
        /// 获取Field特性，如果存在
        /// </summary>
        /// <param name="member">MemberInfo</param>
        /// <returns>String</returns>
        private static string GetFieldNameAttribute(MemberInfo member)
        {
            if (member.GetCustomAttributes(typeof(FieldNameAttribute), true).Any())
            {
                return ((FieldNameAttribute)member.GetCustomAttributes(typeof(FieldNameAttribute), true)[0]).FieldName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 检测字段名称或者设置的field特性名称是否匹配
        /// </summary>
        /// <param name="member">The Member of the Instance to check</param>
        /// <param name="name">The Name to compare with</param>
        /// <returns>True if Fields match</returns>
        /// <remarks>FieldNameAttribute takes precedence over TargetMembers name.</remarks>
        private static bool MemberMatchesName(MemberInfo member, string name)
        {
            string fieldnameAttribute = GetFieldNameAttribute(member);
            return fieldnameAttribute.ToLower() == name.ToLower() || member.Name.ToLower() == name.ToLower();
        }

        /// <summary>
        /// 创建表达式
        /// </summary>
        /// <param name="sourceInstanceExpression"></param>
        /// <param name="sourceMember"></param>
        /// <returns></returns>
        private static Expression GetSourceValueExpression(ParameterExpression sourceInstanceExpression, MemberInfo sourceMember)
        {
            MemberExpression memberExpression = Expression.PropertyOrField(sourceInstanceExpression, sourceMember.Name);
            Expression sourceValueExpression;

            // ReSharper disable once AssignNullToNotNullAttribute
            if (Nullable.GetUnderlyingType(sourceMember.ReflectedType) == null)
            {
                sourceValueExpression = Expression.Convert(memberExpression, typeof(object));
            }
            else
            {
                sourceValueExpression = Expression.Condition(
                    Expression.Property(Expression.Constant(sourceInstanceExpression), "HasValue"),
                    memberExpression,
                    Expression.Constant(DBNull.Value),
                    typeof(object));
            }
            return sourceValueExpression;
        }

        /// <summary>
        /// 创建一个委托，该TSource的实例映射到一个提供数据表的ItemArray
        /// </summary>
        /// <typeparam name="TSource">The Generic Type to map from</typeparam>
        /// <param name="dt">The DataTable to map to</param>
        /// <returns>Func(Of TSource, Object())</returns>
        static internal Func<TSource, object[]> CreateDataRowMapper<TSource>(DataTable dt)
        {
            Type sourceType = typeof(TSource);
            ParameterExpression sourceInstanceExpression = Expression.Parameter(sourceType, "SourceInstance");
            List<Expression> values = new List<Expression>();

            foreach (DataColumn col in dt.Columns)
            {
                foreach (FieldInfo sourceMember in sourceType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (MemberMatchesName(sourceMember, col.ColumnName))
                    {
                        values.Add(GetSourceValueExpression(sourceInstanceExpression, sourceMember));
                        break;
                    }
                }
                foreach (PropertyInfo sourceMember in sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (sourceMember.CanRead && MemberMatchesName(sourceMember, col.ColumnName))
                    {
                        values.Add(GetSourceValueExpression(sourceInstanceExpression, sourceMember));
                        break;
                    }
                }
            }
            // ReSharper disable once AssignNullToNotNullAttribute
            NewArrayExpression body = Expression.NewArrayInit(Type.GetType("System.Object"), values);
            return Expression.Lambda<Func<TSource, object[]>>(body, sourceInstanceExpression).Compile();
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        private sealed class DataRowMapperCache<TSource>
        {
            private DataRowMapperCache() { }

            // ReSharper disable once StaticMemberInGenericType
            private static readonly object LockObject = new object();
            private static Func<TSource, object[]> _mapper;

            static internal Func<TSource, object[]> GetDataRowMapper(DataTable dt)
            {
                if (_mapper == null)
                {
                    lock (LockObject)
                    {
                        if (_mapper == null)
                        {
                            _mapper = CreateDataRowMapper<TSource>(dt);
                        }
                    }
                }
                return _mapper;
            }
        }

        /// <summary>
        /// 创建实体对应的DataTable结构
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        private sealed class DataTableCreator<TSource>
        {
            private DataTableCreator() { }

            // ReSharper disable once StaticMemberInGenericType
            private static readonly object LockObject = new object();
            // ReSharper disable once StaticMemberInGenericType
            private static DataTable _emptyDataTable;
            static internal DataTable GetDataTable()
            {
                if (_emptyDataTable == null)
                {
                    lock (LockObject)
                    {
                        if (_emptyDataTable == null)
                        {
                            _emptyDataTable = CreateDataTable<TSource>();
                        }
                    }
                }
                return _emptyDataTable.Clone();
            }
        }
        #endregion
        #endregion

        #region [3.6 private]
        private static readonly MethodInfo GetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public);

        private static readonly FieldInfo DbNullValueField = typeof(DBNull).GetField("Value", BindingFlags.Public | BindingFlags.Static);

        private static readonly ConstructorInfo ExceptionConstructor = typeof(ApplicationException).GetConstructor(new[] { typeof(string), typeof(Exception) });

        private static readonly MethodInfo GetMessageMethod = typeof(Exception).GetMethod("get_Message");

        private static readonly MethodInfo ConcatMethod = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });

        private static readonly MethodInfo ChangeTypeMethod = typeof(Convert).GetMethod("ChangeType", new[] { typeof(object), typeof(Type) });

        private static readonly MethodInfo AppendFormatMethod = typeof(StringBuilder).GetMethod("AppendFormat", new[] { typeof(string), typeof(object), typeof(object), typeof(object) });

        private static readonly ConstructorInfo StringBuilderConstructor = typeof(StringBuilder).GetConstructor(new Type[] { });

        private static readonly MethodInfo ToStringMethod = typeof(object).GetMethod("ToString", new Type[] { });

        private static readonly MethodInfo SecureReaderGetValueMethod = typeof(DataTypeConvertHelper).GetMethod("SecureReaderGetValue", BindingFlags.Static | BindingFlags.NonPublic);

        private delegate T SetPropertyValueInvoker<T>(T obj, IDataReader obj1);

        private delegate TTarget MapReturnMethod<TTarget, TSource>(TSource source);

        private delegate void MapMethod<TSource, TTarget>(TSource source, TTarget target);

        private delegate string GetEntityDifferenceMethod<T>(T t1, T t2);

        private class Property
        {
            internal PropertyInfo PropInfo;
            internal string Description;

            internal Property(PropertyInfo propInfo)
            {
                PropInfo = propInfo;
                DescriptionAttribute[] descAttrList = (DescriptionAttribute[])propInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                Description = descAttrList.Length > 0 ? descAttrList[0].Description : propInfo.Name;
            }
        }

        private class PropertyCollection : List<Property>
        {
            internal PropertyCollection(PropertyInfo[] arr)
                : base(arr.Length)
            {
                foreach (PropertyInfo p in arr)
                {
                    NonCompareDifferenceAttribute[] nonCompareAttrList = (NonCompareDifferenceAttribute[])p.
                        GetCustomAttributes(typeof(NonCompareDifferenceAttribute), false);
                    if (nonCompareAttrList.Length > 0)
                        continue;
                    Property property = new Property(p);
                    Add(property);
                }
            }
        }

        private static class EntityUtilCache<T>
        {
            // ReSharper disable once StaticMemberInGenericType
            private static readonly Type TypeInfo;
            // ReSharper disable once StaticMemberInGenericType
            private static readonly PropertyInfo[] PropInfoArr;
            // ReSharper disable once StaticMemberInGenericType
            private static readonly PropertyCollection PropList;
            internal static readonly SetPropertyValueInvoker<T> EmitInvoker;
            internal static readonly GetEntityDifferenceMethod<T> GetEntityDifferenceInvoker;

            static EntityUtilCache()
            {
                TypeInfo = typeof(T);
                PropInfoArr = TypeInfo.GetProperties();
                PropList = new PropertyCollection(PropInfoArr);
                EmitInvoker = InternalGetEmitInvoker();
                //TestGetEmitInvoker();
                GetEntityDifferenceInvoker = InternalGetGetEntityDifferenceInvoker();
                //TestGetGetEntityDefferenceInvoker();
                TypeInfo = null;
                PropInfoArr = null;
                PropList = null;
            }

            private static SetPropertyValueInvoker<T> InternalGetEmitInvoker()
            {
                Type type = TypeInfo;

                Type ownerType = type.IsInterface ? typeof(object) : type;
                bool canSkipChecks = SecurityManager.IsGranted(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
                DynamicMethod method = new DynamicMethod("SetPropertyValueInvoker", type,
                    new[] { type, typeof(IDataReader) }, ownerType, canSkipChecks);

                ILGenerator il = method.GetILGenerator();

                InternalGeneratorEmitInvoker(il);

                return (SetPropertyValueInvoker<T>)method.CreateDelegate(typeof(SetPropertyValueInvoker<T>));
            }

            private static void InternalGeneratorEmitInvoker(ILGenerator il)
            {
                Type type = TypeInfo;
                PropertyInfo[] propInfoArr = PropInfoArr;

                il.Emit(OpCodes.Ldarg_0);
                LocalBuilder o = il.DeclareLocal(type);
                il.Emit(OpCodes.Stloc, o);
                Label lbl = il.DefineLabel();
                il.Emit(OpCodes.Ldloc, o);
                il.Emit(OpCodes.Brtrue_S, lbl);
                // ReSharper disable once AssignNullToNotNullAttribute
                il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { }));
                il.Emit(OpCodes.Stloc, o);
                il.MarkLabel(lbl);
                LocalBuilder val = il.DeclareLocal(typeof(object));

                int n = propInfoArr.Length;
                for (int i = 0; i < n; i++)
                {
                    PropertyInfo propInfo = propInfoArr[i];
                    MethodInfo mi = propInfo.GetSetMethod();
                    if (mi == null) continue;

                    Label lblElse = il.DefineLabel();

                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Ldstr, propInfo.Name);
                    il.Emit(OpCodes.Call, SecureReaderGetValueMethod);
                    il.Emit(OpCodes.Stloc, val);
                    il.Emit(OpCodes.Ldloc, val);
                    il.Emit(OpCodes.Brfalse_S, lblElse);
                    il.Emit(OpCodes.Ldloc, val);
                    il.Emit(OpCodes.Ldsfld, DbNullValueField);
                    il.Emit(OpCodes.Beq_S, lblElse);
                    il.BeginExceptionBlock();
                    il.Emit(OpCodes.Ldloc, o);
                    il.Emit(OpCodes.Ldloc, val);
                    if (propInfo.PropertyType.IsGenericType)
                    {
                        il.Emit(OpCodes.Ldtoken, propInfo.PropertyType.GetGenericArguments()[0]);
                        il.Emit(OpCodes.Call, GetTypeFromHandleMethod);
                        il.Emit(OpCodes.Call, ChangeTypeMethod);
                    }
                    else if (propInfo.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldtoken, propInfo.PropertyType);
                        il.Emit(OpCodes.Call, GetTypeFromHandleMethod);
                        il.Emit(OpCodes.Call, ChangeTypeMethod);
                    }
                    if (propInfo.PropertyType.IsValueType)
                        il.Emit(OpCodes.Unbox_Any, propInfo.PropertyType);
                    else
                        il.Emit(OpCodes.Castclass, propInfo.PropertyType);
                    il.EmitCall(OpCodes.Callvirt, mi, null);
                    il.BeginCatchBlock(typeof(Exception));
                    LocalBuilder e = il.DeclareLocal(typeof(Exception));
                    il.Emit(OpCodes.Stloc, e);
                    il.Emit(OpCodes.Ldstr, "[" + propInfo.Name + "]属性赋值出现错误。");
                    il.Emit(OpCodes.Ldloc, e);
                    il.Emit(OpCodes.Callvirt, GetMessageMethod);
                    il.Emit(OpCodes.Call, ConcatMethod);
                    il.Emit(OpCodes.Ldloc, e);
                    il.Emit(OpCodes.Newobj, ExceptionConstructor);
                    il.Emit(OpCodes.Throw);
                    il.EndExceptionBlock();
                    il.MarkLabel(lblElse);
                }

                il.Emit(OpCodes.Ldloc, o);
                il.Emit(OpCodes.Ret);
            }

            private static GetEntityDifferenceMethod<T> InternalGetGetEntityDifferenceInvoker()
            {
                Type type = TypeInfo;

                Type ownerType = type.IsInterface ? typeof(object) : type;
                bool canSkipChecks =
                    SecurityManager.IsGranted(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
                DynamicMethod method = new DynamicMethod("GetEntityDifference", typeof(string),
                                                         new[] { type, type }, ownerType, canSkipChecks);

                ILGenerator il = method.GetILGenerator();

                InternalGeneratorGetEntityDifferenceInvoker(il);

                return (GetEntityDifferenceMethod<T>)method.CreateDelegate(typeof(GetEntityDifferenceMethod<T>));
            }

            private static void InternalGeneratorGetEntityDifferenceInvoker(ILGenerator il)
            {
                string format = "{0}：“{1}”==>“{2}”|";
                LocalBuilder str = il.DeclareLocal(typeof(StringBuilder));
                il.Emit(OpCodes.Newobj, StringBuilderConstructor);
                il.Emit(OpCodes.Stloc, str);

                Type nullableType = typeof(Nullable<>);

                foreach (Property prop in PropList)
                {
                    PropertyInfo pi = prop.PropInfo;

                    MethodInfo getMethod = pi.GetGetMethod(false);
                    if (getMethod == null) continue;

                    LocalBuilder value1 = il.DeclareLocal(pi.PropertyType);
                    LocalBuilder value2 = il.DeclareLocal(pi.PropertyType);

                    if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == nullableType)
                    {
                        Label label1 = il.DefineLabel();
                        Label label2 = il.DefineLabel();
                        Label label3 = il.DefineLabel();
                        Label label4 = il.DefineLabel();
                        Label label5 = il.DefineLabel();

                        MethodInfo getMethodHasValue = pi.PropertyType.GetProperty("HasValue").GetGetMethod();
                        MethodInfo methodGetValueOrDefault = pi.PropertyType.GetMethod("GetValueOrDefault", new Type[] { });

                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Callvirt, getMethod);
                        il.Emit(OpCodes.Stloc, value1);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Callvirt, getMethod);
                        il.Emit(OpCodes.Stloc, value2);
                        il.Emit(OpCodes.Ldloca_S, value1);
                        il.Emit(OpCodes.Call, getMethodHasValue);
                        il.Emit(OpCodes.Ldloca_S, value2);
                        il.Emit(OpCodes.Call, getMethodHasValue);
                        il.Emit(OpCodes.Bne_Un_S, label3);
                        il.Emit(OpCodes.Ldloca_S, value1);
                        il.Emit(OpCodes.Call, getMethodHasValue);
                        il.Emit(OpCodes.Brfalse_S, label1);
                        il.Emit(OpCodes.Ldloca_S, value1);
                        il.Emit(OpCodes.Call, methodGetValueOrDefault);
                        il.Emit(OpCodes.Ldloca_S, value2);
                        il.Emit(OpCodes.Call, methodGetValueOrDefault);
                        Type valueType = pi.PropertyType.GetGenericArguments()[0];
                        MethodInfo method = valueType.GetMethod("op_Inequality");
                        if (method != null)
                        {
                            il.Emit(OpCodes.Call, method);
                            il.Emit(OpCodes.Br_S, label2);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ceq);
                            il.Emit(OpCodes.Brtrue_S, label1);
                        }
                        il.MarkLabel(label1);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.MarkLabel(label2);
                        il.Emit(OpCodes.Br_S, label4);
                        il.MarkLabel(label3);
                        il.Emit(OpCodes.Ldc_I4_1);
                        il.MarkLabel(label4);
                        il.Emit(OpCodes.Ldc_I4_0);
                        il.Emit(OpCodes.Ceq);
                        il.Emit(OpCodes.Brtrue_S, label5);
                        il.Emit(OpCodes.Ldloc, str);
                        il.Emit(OpCodes.Ldstr, format);
                        il.Emit(OpCodes.Ldstr, prop.Description);
                        il.Emit(OpCodes.Ldloc_S, value1);
                        il.Emit(OpCodes.Box, pi.PropertyType);
                        il.Emit(OpCodes.Ldloc_S, value2);
                        il.Emit(OpCodes.Box, pi.PropertyType);
                        il.Emit(OpCodes.Callvirt, AppendFormatMethod);
                        il.Emit(OpCodes.Pop);

                        il.MarkLabel(label5);
                    }
                    else
                    {
                        Label label1 = il.DefineLabel();
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Callvirt, getMethod);
                        il.Emit(OpCodes.Stloc, value1);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Callvirt, getMethod);
                        il.Emit(OpCodes.Stloc, value2);
                        il.Emit(OpCodes.Ldloc_S, value1);
                        il.Emit(OpCodes.Ldloc_S, value2);
                        MethodInfo method = pi.PropertyType.GetMethod("op_Inequality");
                        if (method != null)
                        {
                            il.Emit(OpCodes.Call, method);
                            il.Emit(OpCodes.Brfalse_S, label1);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ceq);
                            il.Emit(OpCodes.Brtrue_S, label1);
                        }
                        il.Emit(OpCodes.Ldloc, str);
                        il.Emit(OpCodes.Ldstr, format);
                        il.Emit(OpCodes.Ldstr, prop.Description);
                        il.Emit(OpCodes.Ldloc, value1);
                        if (pi.PropertyType.IsValueType)
                            il.Emit(OpCodes.Box, pi.PropertyType);
                        il.Emit(OpCodes.Ldloc, value2);
                        if (pi.PropertyType.IsValueType)
                            il.Emit(OpCodes.Box, pi.PropertyType);
                        il.Emit(OpCodes.Callvirt, AppendFormatMethod);
                        il.Emit(OpCodes.Pop);

                        il.MarkLabel(label1);
                    }
                }

                il.Emit(OpCodes.Ldloc, str);
                il.Emit(OpCodes.Callvirt, ToStringMethod);
                il.Emit(OpCodes.Ret);
            }
        }

        private static class FastMapper<TTarget, TSource>
        {
            internal static readonly MapReturnMethod<TTarget, TSource> MapReturnMethod;

            internal static readonly MapMethod<TTarget, TSource> MapMethod;

            static FastMapper()
            {
                MapReturnMethod = CreateMapReturnMethod(typeof(TTarget), typeof(TSource));
                MapMethod = CreateMapMethod(typeof(TTarget), typeof(TSource));
            }

            private static MapReturnMethod<TTarget, TSource> CreateMapReturnMethod(Type targetType, Type sourceType)
            {
                Type ownerType = targetType.IsInterface ? typeof(object) : targetType;
                bool canSkipChecks = SecurityManager.IsGranted(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
                DynamicMethod map = new DynamicMethod("MapReturn", targetType, new[] { sourceType }, ownerType, canSkipChecks);

                ILGenerator il = map.GetILGenerator();
                ConstructorInfo ci = targetType.GetConstructor(new Type[0]);
                il.DeclareLocal(targetType);
                // ReSharper disable once AssignNullToNotNullAttribute
                il.Emit(OpCodes.Newobj, ci);
                il.Emit(OpCodes.Stloc_0);
                PropertyInfo[] sourceProps = sourceType.GetProperties();
                PropertyInfo[] targetProps = targetType.GetProperties();
                foreach (PropertyInfo sourcePropertyInfo in sourceProps)
                {
                    MethodInfo getMethodInfo = sourcePropertyInfo.GetGetMethod();
                    if (getMethodInfo == null) continue;

                    PropertyInfo targetPropertyInfo = Array.Find(targetProps,
                            delegate (PropertyInfo p) { return p.Name == sourcePropertyInfo.Name && p.PropertyType == sourcePropertyInfo.PropertyType; }
                        );
                    if (targetPropertyInfo == null) continue;

                    MethodInfo setMethodInfo = targetPropertyInfo.GetSetMethod();
                    if (setMethodInfo == null) continue;

                    il.Emit(OpCodes.Ldloc_0);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Callvirt, getMethodInfo);
                    il.Emit(OpCodes.Callvirt, setMethodInfo);
                }
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ret);

                return (MapReturnMethod<TTarget, TSource>)map.CreateDelegate(typeof(MapReturnMethod<TTarget, TSource>));
            }

            private static MapMethod<TTarget, TSource> CreateMapMethod(Type targetType, Type sourceType)
            {
                Type ownerType = targetType.IsInterface ? typeof(object) : targetType;

                bool canSkipChecks = SecurityManager.IsGranted(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
                DynamicMethod map = new DynamicMethod("Map", null, new[] { targetType, sourceType }, ownerType, canSkipChecks);

                ILGenerator il = map.GetILGenerator();
                PropertyInfo[] sourceProps = sourceType.GetProperties();
                PropertyInfo[] targetProps = targetType.GetProperties();
                foreach (PropertyInfo sourcePropertyInfo in sourceProps)
                {
                    MethodInfo getMethodInfo = sourcePropertyInfo.GetGetMethod();
                    if (getMethodInfo == null) continue;

                    PropertyInfo targetPropertyInfo = Array.Find(targetProps,
                        p => p.Name == sourcePropertyInfo.Name && p.PropertyType == sourcePropertyInfo.PropertyType
                        );

                    MethodInfo setMethodInfo = targetPropertyInfo?.GetSetMethod();
                    if (setMethodInfo == null) continue;

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Callvirt, getMethodInfo);
                    il.Emit(OpCodes.Callvirt, setMethodInfo);
                }
                il.Emit(OpCodes.Ret);

                return (MapMethod<TTarget, TSource>)map.CreateDelegate(typeof(MapMethod<TTarget, TSource>));
            }
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// 字符格式化特性
    /// </summary>
    public class FormatAttribute : Attribute
    {
        /// <summary>
        /// string.format("格式化",obj)  比如：日期格式化：{0:yyyy-MM-dd}
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="format"></param>
        public FormatAttribute(string format)
        {
            Format = format;
        }
    }

    /// <summary>
    /// 不参与差异比较的数据实体属性    
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NonCompareDifferenceAttribute : Attribute
    {
    }

    /// <summary>
    /// 字段别名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FieldNameAttribute : Attribute
    {
        /// <summary>
        /// 别名
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldName"></param>
        public FieldNameAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }

    /// <summary>
    /// 创建转换的委托
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DataTableEntityBuilder<TEntity>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly MethodInfo GetValueMethod = typeof(DataRow).GetMethod("get_Item", new[] { typeof(int) });
        // ReSharper disable once StaticMemberInGenericType
        private static readonly MethodInfo IsDbNullMethod = typeof(DataRow).GetMethod("IsNull", new[] { typeof(int) });
        private delegate TEntity Load(DataRow dataRecord);

        private Load _handler;
        private DataTableEntityBuilder() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public TEntity Build(DataRow dataRecord)
        {
            return _handler(dataRecord);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRecord"></param>
        /// <returns></returns>
        public static DataTableEntityBuilder<TEntity> CreateBuilder(DataRow dataRecord)
        {
            DataTableEntityBuilder<TEntity> dynamicBuilder = new DataTableEntityBuilder<TEntity>();
            DynamicMethod method = new DynamicMethod("DynamicCreateEntity", typeof(TEntity), new[] { typeof(DataRow) }, typeof(TEntity), true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(TEntity));
            // ReSharper disable once AssignNullToNotNullAttribute
            generator.Emit(OpCodes.Newobj, typeof(TEntity).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            for (int i = 0; i < dataRecord.ItemArray.Length; i++)
            {
                PropertyInfo propertyInfo = typeof(TEntity).GetProperty(dataRecord.Table.Columns[i].ColumnName);
                Label endIfLabel = generator.DefineLabel();
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, IsDbNullMethod);
                    generator.Emit(OpCodes.Brtrue, endIfLabel);
                    generator.Emit(OpCodes.Ldloc, result);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, i);
                    generator.Emit(OpCodes.Callvirt, GetValueMethod);
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, propertyInfo.GetSetMethod());
                    generator.MarkLabel(endIfLabel);
                }
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            dynamicBuilder._handler = (Load)method.CreateDelegate(typeof(Load));
            return dynamicBuilder;
        }
    }
}