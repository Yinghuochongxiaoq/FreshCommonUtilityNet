using System;
using System.Collections.Generic;

namespace FreshCommonUtilityNetTest.Model
{
    public class TestsTabelToListObject
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        public int Age { get; set; }

        public double Height { get; set; }

        public Enum.EnumSex Sex { get; set; }

        public TimeSpan YouLong { get; set; }

        public bool Right { get; set; }

        public List<string> AddressList { get; set; }

        public DateTime BrityDay { get; set; }

        public TestsTabelToListObject ParentObject { get; set; }

        public TestsTabelToListObject ChildObject { get; set; }
    }
}
