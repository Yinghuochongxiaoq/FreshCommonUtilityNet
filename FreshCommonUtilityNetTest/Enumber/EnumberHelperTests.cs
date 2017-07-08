using FreshCommonUtility.Enum;
using FreshCommonUtilityNetTest.Enum;

namespace FreshCommonUtilityNetTest.Enumber
{
    public class EnumberHelperTests
    {
        /// <summary>
        /// 枚举转换成list集合测试
        /// </summary>
        public void EnumToListTest()
        {
            var enumList = EnumHelper.GetEnumDataList<AuthEnum>();
            enumList.Count.IsEqualTo(3);
        }

        /// <summary>
        /// 获取枚举对象的描述
        /// </summary>
        public void GetEnumDescriptionTest()
        {
            var des = EnumHelper.GetEnumDescription(AuthEnum.FreshMan);
            des.IsEqualTo(AuthEnum.FreshMan.ToString());
        }

        /// <summary>
        /// 根据枚举对象的值获取枚举对象
        /// </summary>
        public void GetEnumByValueTest()
        {
            var enumTemp = EnumHelper.GetEnumByValue<AuthEnum>(AuthEnum.ZhuBao.GetHashCode());
            enumTemp.IsEqualTo(AuthEnum.ZhuBao);
        }

        /// <summary>
        /// 根据枚举对象的名称获取枚举对象
        /// </summary>
        public void GetEnumByNameTest()
        {
            var enumName = EnumHelper.GetEnumByName<AuthEnum>(AuthEnum.FreshMan.ToString());
            enumName.IsEqualTo(AuthEnum.FreshMan);
        }
    }
}