using System.ComponentModel;

namespace FreshCommonUtilityNetTest.Enum
{
    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("Other")]
        Other = 0,

        /// <summary>
        /// 
        /// </summary>
        [Description("SO")]
        SO = 1,
        /// <summary>
        /// 
        /// </summary>
        [Description("WO")]
        WO = 2,

        /// <summary>
        /// 
        /// </summary>
        [Description("PlanOrderHdrM")]
        PlanOrderHdrM = 4,

        /// <summary>
        /// 
        /// </summary>
        [Description("Detail")]
        WoDet = 8
    }
}
