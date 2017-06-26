using System;
using System.Collections.Generic;
using System.Linq;

namespace FreshCommonUtilityNetTest
{
    /// <summary>
    /// Rewrite Assert class.
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Compare two object is equals
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void IsEqualTo<T>(this T obj, T other)
        {
            if (!obj.Equals(other))
            {
                throw new Exception($"{obj} should be qual to {other}");
            }
        }

        /// <summary>
        /// 不等判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void IsNotEqualTo<T>(this T obj, T other)
        {
            if (obj.Equals(other))
            {
                throw new Exception($"{obj} should not be qual to {other}");
            }
        }

        /// <summary>
        /// Compare two Enumberable object is equals.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="other"></param>
        public static void IsSequenceEqualTo<T>(this IEnumerable<T> obj, IEnumerable<T> other)
        {
            if (!obj.SequenceEqual(other))
            {
                throw new Exception($"{obj} should be qual to {other}");
            }
        }

        /// <summary>
        /// assert var is false.
        /// </summary>
        /// <param name="b"></param>
        public static void IsFalse(this bool b)
        {
            if (b)
            {
                throw new Exception("Expected false.");
            }
        }

        /// <summary>
        /// assert var is true
        /// </summary>
        /// <param name="b"></param>
        public static void IsTrue(this bool b)
        {
            if (!b)
            {
                throw new Exception("Expected false.");
            }
        }

        /// <summary>
        /// assert object is null
        /// </summary>
        /// <param name="obj"></param>
        public static void IsNull(this object obj)
        {
            if (obj != null)
            {
                throw new Exception("expected null.");
            }
        }
    }
}
