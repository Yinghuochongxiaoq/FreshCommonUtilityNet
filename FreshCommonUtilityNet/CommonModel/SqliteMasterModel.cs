#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.CommonModel
//文件名称：SqliteMasterModel
//创 建 人：FreshMan
//创建日期：2017/10/14 17:17:32
//用    途：记录类的用途
//======================================================================
#endregion

namespace FreshCommonUtilityNet.CommonModel
{
    /// <summary>
    /// sqlite sqliteMaster model
    /// </summary>
    internal class SqliteMasterModel
    {
        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// tbl_name
        /// </summary>
        public string TblName { get; set; }

        /// <summary>
        /// sql
        /// </summary>
        public string Sql { get; set; }
    }
}
