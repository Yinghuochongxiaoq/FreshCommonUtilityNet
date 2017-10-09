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

        [DataMember]
        public string WorkNum { get; set; }

        [DataMember]
        public string WorkName { get; set; }

        [DataMember]
        public string Tel { get; set; }

        [DataMember]
        public List<string> Skills { get; set; }

        [DataMember]
        public string OrderId { get; set; }

        [DataMember]
        public OrderType OrderType { get; set; }

        [DataMember]
        public string OperationNum { get; set; }

        [DataMember]
        public string MachineName { get; set; }

        [DataMember]
        public string ItemNum { get; set; }

        public DateTime? StartTime { get; set; }

        [DataMember]
        public DateTime? EndTime { get; set; }

        [DataMember]
        public bool IsFixed { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public PersonTimeType Persontimetype { get; set; }

        [DataMember]
        public PersonTimeState PersontimeState { get; set; }

        [DataMember]
        public string TaskId { get; set; }

        [DataMember]
        public DateTime InitStartTime { get; set; }

        [DataMember]
        public DateTime InitEndTime { get; set; }

        [DataMember]
        public double Salary { get; set; }

        [DataMember]
        public double DayOverTimeFactor { get; set; }

        [DataMember]
        public double WeekOverTimeFactor { get; set; }

        [DataMember]
        public double HolidayOverTimeFactor { get; set; }
    }
}
