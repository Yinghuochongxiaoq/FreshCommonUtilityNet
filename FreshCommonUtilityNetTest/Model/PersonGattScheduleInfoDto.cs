using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FreshCommonUtilityNetTest.Enum;

namespace FreshCommonUtilityNetTest.Model
{
    [Serializable]
    [DataContract]
    public class PersonGattScheduleInfoDto
    {
        [DataMember]
        public string Id { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        [DataMember]
        public string WorkNum { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        [DataMember]
        public string WorkName { get; set; }

        /// <summary>
        /// 员工电话
        /// </summary>
        [DataMember]
        public string Tel { get; set; }

        /// <summary>
        /// 员工技能/等级
        /// </summary>
        [DataMember]
        public List<string> Skills { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        [DataMember]
        public OrderType OrderType { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [DataMember]
        public string OperationNum { get; set; }

        /// <summary>
        /// 设备名称--可能为空
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        [DataMember]
        public string ItemNum { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        [DataMember]
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 任务结束时间
        /// </summary>
        [DataMember]
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 是否Fix，这个是根据Schedule来的
        /// </summary>
        [DataMember]
        public bool IsFixed { get; set; }
        /// <summary>
        /// 生产数量，可能为
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        /// <summary>
        /// 时间类型{工作日，休息日，公休}
        /// </summary>
        [DataMember]
        public PersonTimeType Persontimetype { get; set; }
        /// <summary>
        /// 时间状态（工作，休息）
        /// </summary>
        [DataMember]
        public PersonTimeState PersontimeState { get; set; }

        /// <summary>
        /// ScheduleAdapter id
        /// </summary>
        [DataMember]
        public string TaskId { get; set; }

        /// <summary>
        /// 初始化时间片开始时间
        /// </summary>
        [DataMember]
        public DateTime InitStartTime { get; set; }

        /// <summary>
        /// 初始化时间片结束时间
        /// </summary>
        [DataMember]
        public DateTime InitEndTime { get; set; }

        /// <summary>
        /// 薪资水平（时薪）
        /// </summary>
        [DataMember]
        public double Salary { get; set; }

        /// <summary>
        /// 日加班系数
        /// </summary>
        [DataMember]
        public double DayOverTimeFactor { get; set; }

        /// <summary>
        /// 周末加班系数
        /// </summary>
        [DataMember]
        public double WeekOverTimeFactor { get; set; }

        /// <summary>
        /// 假期加班系数
        /// </summary>
        [DataMember]
        public double HolidayOverTimeFactor { get; set; }
    }
}
