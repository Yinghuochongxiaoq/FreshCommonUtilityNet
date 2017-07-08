using System;
using System.ComponentModel;

namespace FreshCommonUtilityNetTest.Enum
{
    /// <summary>
    /// 人员工作时间类型
    /// </summary>
    [Flags]
    public enum PersonTimeType
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        [Description("未知类型")]
        Nan = 0,

        /// <summary>
        /// 工作日
        /// </summary>
        [Description("工作日")]
        AvailableWorking = 1 << 0,

        /// <summary>
        /// 周末休息日
        /// </summary>
        [Description("周末休息日")]
        WeekRest = 1 << 1,

        /// <summary>
        /// 休息时间
        /// </summary>
        [Description("休息时间")]
        Holiday = 1 << 2,

        /// <summary>
        /// 公休日
        /// </summary>
        [Description("公休日")]
        CountryHoliday = 1 << 3,

        /// <summary>
        /// 调休日
        /// </summary>
        [Description("调休日")]
        TransferredDay = 1 << 4,

        /// <summary>
        /// 请假时间
        /// </summary>
        [Description("请假时间")]
        VacationTime = 1 << 5,

        /// <summary>
        /// 普通加班时间
        /// </summary>
        [Description("普通加班时间")]
        DayOverTime = 1 << 6,

        /// <summary>
        /// 周末加班时间
        /// </summary>
        [Description("周末加班时间")]
        WeekOverTime = 1 << 7,

        /// <summary>
        /// 假期加班时间
        /// </summary>
        [Description("假期加班时间")]
        HolidayOverTime = 1 << 8
    }
}
