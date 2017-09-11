/**============================================================
* 命名空间: FreshCommonUtilityNet.CommonModel
*
* 功 能： N/A
* 类 名： ReferencedModel
*
* Ver 变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2017/9/6 14:48:47 FreshMan 初版
*
* Copyright (c) 2017 Lir Corporation. All rights reserved.
*==============================================================
*==此技术信息为本公司机密信息,未经本公司书面同意禁止向第三方披露==
*==版权所有：重庆慧都科技有限公司                             ==
*==============================================================
*/
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
