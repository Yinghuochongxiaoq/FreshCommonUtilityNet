#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.CommonModel
//文件名称：ReferencedModel
//创 建 人：FreshMan
//创建日期：2017/7/8 14:49:17
//用    途：记录类的用途
//======================================================================
#endregion
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.CommonModel
{
    /// <summary>
    /// Referenced model
    /// </summary>
    public class ReferencedModel
    {
        /// <summary>
        /// ForeignKey
        /// </summary>
        public string ForeignKey { get; set; }

        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// table name key cell
        /// </summary>
        public string ForeignKeyCell { get; set; }

        /// <summary>
        /// ReferencedTableName
        /// </summary>
        public string ReferencedTableName { get; set; }

        /// <summary>
        /// ReferencedCell
        /// </summary>
        public string ReferencedCell { get; set; }

        /// <summary>
        /// referenced table list
        /// </summary>
        public List<ReferencedModel> ReferencedModelList { get; set; }
    }
}
