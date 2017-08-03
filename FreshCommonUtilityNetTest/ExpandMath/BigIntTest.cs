using FreshCommonUtility.ExpandMath;

namespace FreshCommonUtilityNetTest.ExpandMath
{
    public class BigIntTest
    {
        /// <summary>
        /// big integer multiply test
        /// </summary>
        public void IntegerMultiplyTest()
        {
            var num1 = "14515154851051854110518451549840305456102024151202145102";
            var num2 = "265815484512154621215412024613202154513015746131545131151";
            var result =
                "3858352919501300144108364377010443235191472994613641090551514409584914746999915880334776614509231723755122272402";
            var calculate = BigInt.IntegerMultiply(num1, num2);
            calculate.IsEqualTo(result);
        }
    }
}
