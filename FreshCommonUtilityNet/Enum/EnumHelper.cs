using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Enum
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 获得枚举值的Description特性的值，一般是消息的搜索码
        /// </summary>
        /// <param name="value">要查找特性的枚举值</param>
        /// <returns>返回查找到的Description特性的值，如果没有，就返回.ToString()</returns>
        public static string GetEnumDescription(System.Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            if (fi == null)
            {
                return value.ToString();
            }
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// 获取枚举的数据源
        /// </summary>
        /// <returns>数据源</returns>
        public static List<EnumDataModel> GetEnumDataList<T>()
        {
            return EnumUtilData<T>.EnumDataList;
        }

        /// <summary>
        /// 通过枚举获取描述信息
        /// </summary>
        /// <param name="value">枚举字段 </param>
        /// <returns>描述信息</returns>
        public static string GetDescriptionByValue<T>(int value) where T : new()
        {
            return GetDescriptionByName<T>(value.ToString());
        }

        /// <summary>
        /// 通过枚举获取描述信息
        /// </summary>
        /// <param name="name">枚举字段 </param>
        /// <returns>描述信息</returns>
        public static string GetDescriptionByName<T>(string name) where T : new()
        {
            var t = GetEnumByName<T>(name);
            return GetDescriptionByEnum(t);
        }

        /// <summary>
        /// 通过枚举获取描述信息
        /// </summary>
        /// <param name="enumInstance">枚举</param>
        /// <returns>描述信息</returns>
        public static string GetDescriptionByEnum<T>(T enumInstance) where T : new()
        {
            var enumDataList = GetEnumDataList<T>();
            var enumData = enumDataList.Find(m => m.Value == enumInstance.GetHashCode());
            return enumData != null ? (enumData.Description ?? string.Empty) : string.Empty;
        }

        /// <summary>
        /// 通过枚举获取描述信息
        /// </summary>
        /// <param name="enumInstance"></param>
        /// <returns></returns>
        public static string GetDescriptionByEnum(System.Enum enumInstance)
        {
            var des = enumInstance.GetType().GetField(enumInstance.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            var attributes = des.ToList();
            if (attributes.Count < 1)
            {
                return string.Empty;
            }
            var descriptionAttribute = attributes[0] as DescriptionAttribute;
            return descriptionAttribute != null ? descriptionAttribute.Description : string.Empty;
        }

        /// <summary>
        /// 通过枚举值得到枚举
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>枚举</returns>
        public static T GetEnumByValue<T>(int value)
        {
            return GetEnumByName<T>(value.ToString());
        }

        /// <summary>
        /// 通过枚举值得到枚举
        /// </summary>
        /// <param name="name">枚举值</param>
        /// <returns>枚举</returns>
        public static T GetEnumByName<T>(string name)
        {
            var t = typeof(T);
            return (T)System.Enum.Parse(t, name);
        }

        /// <summary>
        /// 尝试转换枚举，失败则返回false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public static bool TryToEnum<T>(object value, out T parsed) where T : struct
        {
            var isParsed = false;
            if (System.Enum.IsDefined(typeof(T), value))
            {
                parsed = (T)System.Enum.Parse(typeof(T), value.ToString());
                isParsed = true;
            }
            else
            {
                parsed = (T)System.Enum.Parse(typeof(T), System.Enum.GetNames(typeof(T))[0]);
            }
            return isParsed;
        }

        /// <summary>
        /// 内部实现类，缓存
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        private static class EnumUtilData<TEnum>
        {
            /// <summary>
            /// 缓存数据
            /// </summary>
            // ReSharper disable once StaticMemberInGenericType
            internal static readonly List<EnumDataModel> EnumDataList;

            static EnumUtilData()
            {
                EnumDataList = InitData();
            }

            /// <summary>
            /// 初始化数据，生成枚举和描述的数据表
            /// </summary>
            private static List<EnumDataModel> InitData()
            {
                var dataList = new List<EnumDataModel>();

                var t = typeof(TEnum);
                var fieldInfoList = t.GetFields();
                foreach (var tField in fieldInfoList)
                {
                    if (tField.IsSpecialName)
                    {
                        continue;
                    }
                    var enumData = new EnumDataModel { Name = tField.Name };
                    enumData.Value = ((TEnum)System.Enum.Parse(t, enumData.Name)).GetHashCode();
                    var enumAttributelist = (DescriptionAttribute[])tField.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    enumData.Description = enumAttributelist.Length > 0 ? enumAttributelist[0].Description : tField.Name;
                    dataList.Add(enumData);
                }
                return dataList;
            }
        }

        /// <summary>
        /// 枚举数据实体
        /// </summary>
        public class EnumDataModel
        {
            /// <summary>
            /// get or set 枚举名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// get or set 枚举值
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// get or set 枚举描述
            /// </summary>
            public string Description { get; set; }
        }
    }
}
