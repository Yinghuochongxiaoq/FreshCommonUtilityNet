using System.ComponentModel;

namespace FreshCommonUtilityNetTest.Enum
{
    /// <summary>
    /// 人员状态
    /// </summary>
    public enum PersonTimeState
    {
        /// <summary>
        /// 工作
        /// </summary>
        [Description("工作")]
        Working = 0,

        /// <summary>
        /// 休息
        /// </summary>
        [Description("休息")]
        Off = 1,
    }
}
