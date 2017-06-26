#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.Map
//文件名称：FastMapHelper
//创 建 人：FreshMan
//创建日期：2017/6/19 21:53:25
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Map
{
    /// <summary>
    /// 快速匹配两个对象，浅复制
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public static class FastMapHelper<TIn, TOut>
    {
        /// <summary>
        /// 只生成一次函数
        /// </summary>
        private static readonly Func<TIn, TOut> Cache = GetFunc();

        /// <summary>
        /// 快速匹配
        /// </summary>
        /// <returns></returns>
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite) continue;
                MemberExpression property = Expression.Property(parameterExpression,
                    typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }
            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)),
                memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, parameterExpression);
            return lambda.Compile();
        }

        /// <summary>
        /// 快速匹配对象
        /// </summary>
        /// <param name="tIn"></param>
        /// <returns></returns>
        public static TOut FastMap(TIn tIn)
        {
            return Cache(tIn);
        }
    }
}
