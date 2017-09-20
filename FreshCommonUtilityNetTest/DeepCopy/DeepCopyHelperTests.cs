using System;
using System.Collections.Generic;
using System.Diagnostics;
using FreshCommonUtility.DeepCopy;

namespace FreshCommonUtilityNetTest.DeepCopy
{
    public class DeepCopyHelperTests
    {
        public void DeepCopyRecursionTest()
        {
            var tempObj = new Model.TestsTabelToListObject
            {
                Age = 10,
                Name = "k",
                Height = 20.907,
                Right = true,
                Sex = Enum.EnumSex.Boy,
                YouLong = new TimeSpan(1, 1, 1, 5),
                AddressList = new List<string> { "FreshMan.com", "China.Chongqing" }
            };
            var copyResult = DeepCopyHelper.DeepCopyRecursion(tempObj) as Model.TestsTabelToListObject;
            Debug.Assert(copyResult != null);
            new TimeSpan(1, 1, 1, 5).IsEqualTo(copyResult.YouLong);
            tempObj.AddressList[1] = "ChangeAddress";
            copyResult.AddressList[1].IsNotEqualTo(tempObj.AddressList[1]);
        }

        public void DeepCopyTest()
        {
            var tempObj = new Model.TestsTabelToListObject
            {
                Age = 10,
                Name = "k",
                Height = 20.907,
                Right = true,
                Sex = Enum.EnumSex.Boy,
                YouLong = new TimeSpan(1, 1, 1, 5),
                AddressList = new List<string> { "FreshMan.com", "China.Chongqing" }
            };

            var child = new Model.TestsTabelToListObject
            {
                Age = 1,
                Name = "T",
                Height = 30,
                Right = true,
                Sex = Enum.EnumSex.Grily,
                YouLong = new TimeSpan(1, 1, 1, 5),
                AddressList = new List<string> { "Hongjun.com", "China.Chongqing" },
                ParentObject = tempObj
            };

            tempObj.ChildObject = child;

            var copyResult = tempObj.DeepCopy();
            Debug.Assert(copyResult != null);
            new TimeSpan(1, 1, 1, 5).IsEqualTo(copyResult.YouLong);
            tempObj.AddressList[1] = "ChangeAddress";
            copyResult.AddressList[1].IsNotEqualTo(tempObj.AddressList[1]);
        }
    }
}