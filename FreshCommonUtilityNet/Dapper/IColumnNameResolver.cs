#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.Dapper
//文件名称：IColumnNameResolver
//创 建 人：FreshMan
//创建日期：2017/10/10 22:55:56
//用    途：记录类的用途
//======================================================================
#endregion

using System.Reflection;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Dapper
{
    /// <summary>
    /// Interface Column name resolver
    /// </summary>
    public interface IColumnNameResolver
    {
        /// <summary>
        /// resolve column name
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        string ResolveColumnName(PropertyInfo propertyInfo);
    }
}
