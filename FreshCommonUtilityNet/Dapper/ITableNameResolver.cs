#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.Dapper
//文件名称：ITableNameResolver
//创 建 人：FreshMan
//创建日期：2017/10/10 22:54:30
//用    途：记录类的用途
//======================================================================
#endregion

using System;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dapper
{
    /// <summary>
    /// Interface table name resolver
    /// </summary>
    public interface ITableNameResolver
    {
        /// <summary>
        /// table name resolver
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string ResolveTableName(Type type);
    }
}
