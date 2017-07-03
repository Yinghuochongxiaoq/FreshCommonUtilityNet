#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：FreshCommonUtilityNet.Npoi
//文件名称：NpoiHelper
//创 建 人：FreshMan
//创建日期：2017/6/28 20:42:32
//用    途：记录类的用途
//======================================================================
#endregion

using System;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Npoi
{
    /// <summary>
    /// NPOI文件解析类
    /// </summary>
    public sealed class NpoiHelper
    {
        #region [1、字段，构造函数]
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <author>FreshMan</author>
        /// <creattime>2015-11-19</creattime>
        /// <param name="fileName">指定文件路径:绝对路径</param>
        public NpoiHelper(string fileName)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// 指定文件路径:绝对路径
        /// </summary>
        private readonly string _fileName;

        /// <summary>
        /// 工作文件
        /// </summary>
        private IWorkbook _workbook;

        /// <summary>
        /// 文件流读取
        /// </summary>
        private FileStream _fs;
        #endregion

        #region [2、DataTable导入excel]
        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <author>FreshMan</author>
        /// <creattime>2015-11-19</creattime>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            if (string.IsNullOrEmpty(_fileName)) return 0;
            try
            {
                _fs = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //2007版本
                if (_fileName.EndsWith(".xlsx")) _workbook = new XSSFWorkbook();
                //2003版本
                else if (_fileName.EndsWith(".xls")) _workbook = new HSSFWorkbook();
                //非合法路径
                else return -1;
            }
            catch (Exception)
            {
                _fs.Dispose();
                throw;
            }
            //列数据量
            var icolIndex = 0;

            #region 创建数据表（sheet）
            ISheet sheet;
            if (_workbook != null)
            {
                sheet = string.IsNullOrEmpty(sheetName) ? _workbook.CreateSheet() : _workbook.CreateSheet(sheetName);
            }
            else
            {
                return -1;
            }
            #endregion

            #region 写入DataTable的列名

            //起始数据行号（0开始）
            int count;
            if (isColumnWritten)
            {
                #region 首行样式
                var headercellStyle = _workbook.CreateCellStyle();
                headercellStyle.BorderBottom = BorderStyle.Thin;
                headercellStyle.BorderLeft = BorderStyle.Thin;
                headercellStyle.BorderRight = BorderStyle.Thin;
                headercellStyle.BorderTop = BorderStyle.Thin;
                headercellStyle.Alignment = HorizontalAlignment.Center;
                //字体
                var headerfont = _workbook.CreateFont();
                headerfont.Boldweight = (short)FontBoldWeight.Bold;
                headercellStyle.SetFont(headerfont);
                #endregion

                #region 首行数据
                //用column name 作为列名
                var headerRow = sheet.CreateRow(0);
                foreach (DataColumn item in data.Columns)
                {
                    var cell = headerRow.CreateCell(icolIndex);
                    cell.SetCellValue(item.ColumnName);
                    cell.CellStyle = headercellStyle;
                    icolIndex++;
                }
                #endregion

                count = 1;
            }
            else
            {
                count = 0;
            }
            #endregion

            #region 内容行格式
            var cellStyle = _workbook.CreateCellStyle();

            //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text來看
            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;


            var cellfont = _workbook.CreateFont();
            cellfont.Boldweight = (short)FontBoldWeight.Normal;
            cellStyle.SetFont(cellfont);
            //自适应列宽度
            for (int iCol = 0; iCol < icolIndex; iCol++)
            {
                sheet.AutoSizeColumn(iCol);
            }
            #endregion

            #region 数据行内容填充
            for (var i = 0; i < data.Rows.Count; ++i)
            {
                var row = sheet.CreateRow(count);
                for (var j = 0; j < data.Columns.Count; ++j)
                {
                    ICell cell = row.CreateCell(j);
                    cell.SetCellValue(data.Rows[i][j].ToString());
                    cell.CellStyle = cellStyle;
                }
                ++count;
            }
            #endregion

            //写入到excel
            _workbook.Write(_fs);
            return count;
        }
        #endregion

        #region [3、excel导入DataTable中]
        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <param name="errorMessage">异常信息</param>
        /// <author>FreshMan</author>
        /// <creattime>2015-11-19</creattime>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn, out string errorMessage)
        {
            var data = new DataTable();
            if (string.IsNullOrEmpty(_fileName) || string.IsNullOrEmpty(sheetName))
            {
                errorMessage = "文件不存在或表不存在";
                return data;
            }
            _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            // 2007版本
            if (_fileName.EndsWith(".xlsx")) _workbook = new XSSFWorkbook(_fs);
            // 2003版本
            else if (_fileName.EndsWith(".xls")) _workbook = new HSSFWorkbook(_fs);
            else
            {
                errorMessage = "文件格式不正确";
                return data;
            }
            //获取sheet
            var sheetsNum = _workbook.NumberOfSheets;
            if (sheetsNum <= 0)
            {
                errorMessage = "文件为空";
                return data;
            }
            var sheetsIndex = _workbook.GetSheetIndex(sheetName);
            if (sheetsIndex < 0)
            {
                errorMessage = "不存在对应工作表";
                return data;
            }
            return ExcelToDataTable(sheetsIndex, isFirstRowColumn, out errorMessage);
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetIndex">excel工作薄sheet的索引位置（从0开始）</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <param name="errorMessage">异常信息</param>
        /// <author>FreshMan</author>
        /// <creattime>2015-11-19</creattime>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(int sheetIndex, bool isFirstRowColumn, out string errorMessage)
        {
            var data = new DataTable();
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(_fileName) || sheetIndex < 0)
            {
                errorMessage = "文件不存在或表不存在";
                return data;
            }

            try
            {
                #region 读取数据文件流，创建工作表

                _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
                // 2007版本
                if (_fileName.EndsWith(".xlsx")) _workbook = new XSSFWorkbook(_fs);
                // 2003版本
                else if (_fileName.EndsWith(".xls")) _workbook = new HSSFWorkbook(_fs);
                else
                {
                    errorMessage = "文件格式不正确";
                    return data;
                }
                //获取sheet
                var sheetsNum = _workbook.NumberOfSheets;
                if (sheetsNum <= sheetIndex)
                {
                    errorMessage = "超出索引范围";
                    return data;
                }
                var sheet = _workbook.GetSheetAt(sheetIndex);
                if (sheet == null)
                {
                    errorMessage = "文件为空";
                    return data;
                }
                IRow firstRow = sheet.GetRow(0);
                //一行最后一个cell的编号 即总的列数
                int cellCount = firstRow.LastCellNum;
                int startRow;

                #endregion

                #region 创建数据列

                if (isFirstRowColumn)
                {
                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        var column = new DataColumn(firstRow.GetCell(i).StringCellValue);
                        data.Columns.Add(column);
                    }
                    startRow = sheet.FirstRowNum + 1;
                }
                else
                {
                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        var column = new DataColumn();
                        data.Columns.Add(column);
                    }
                    startRow = sheet.FirstRowNum;
                }

                #endregion

                #region 填充数据

                //最后一列的标号
                var rowCount = sheet.LastRowNum;
                for (var i = startRow; i <= rowCount; ++i)
                {
                    var row = sheet.GetRow(i);
                    //没有数据的行默认是null
                    if (row == null) continue;

                    var dataRow = data.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        if (row.GetCell(j) == null) continue;
                        var str = row.GetCell(j).ToString();
                        dataRow[j] = str;
                    }
                    data.Rows.Add(dataRow);
                }

                #endregion

                return data;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return data;
            }
        }
        #endregion

        #region [4、获得数据表的工作表数量]
        /// <summary>
        /// 获得excel
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <author>FreshMan</author>
        /// <creattime>2015-11-19</creattime>
        /// <returns>excel中的工作表（sheet）数量</returns>
        public int GetExcelSheetsNumber(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath) && string.IsNullOrEmpty(_fileName)) return 0;
            filePath = string.IsNullOrEmpty(filePath) ? _fileName : filePath;
            try
            {
                _fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                // 2007版本
                if (_fileName.EndsWith(".xlsx")) _workbook = new XSSFWorkbook(_fs);
                // 2003版本
                else if (_fileName.EndsWith(".xls")) _workbook = new HSSFWorkbook(_fs);
                else return 0;
                return _workbook.NumberOfSheets;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #region [5、excel导入DataSet中]
        /// <summary>
        /// excel导入DataSet中
        /// </summary>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <author>FreshMan</author>
        /// <creattime>2015-11-19</creattime>
        /// <returns>excel中的每一个sheet作为一个DataTable添加到DataSet中并返回</returns>
        public DataSet ExcelToDataSet(bool isFirstRowColumn)
        {
            var dataSet = new DataSet();
            if (string.IsNullOrEmpty(_fileName)) return dataSet;

            try
            {
                #region 读取数据文件流

                _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
                // 2007版本
                if (_fileName.EndsWith(".xlsx")) _workbook = new XSSFWorkbook(_fs);
                // 2003版本
                else if (_fileName.EndsWith(".xls")) _workbook = new HSSFWorkbook(_fs);
                else return dataSet;
                #endregion

                #region 循环读取工作表中的数据
                //获得表总数
                var sheetsNum = _workbook.NumberOfSheets;
                if (sheetsNum <= 0) return dataSet;
                for (var sheetItem = 0; sheetItem < sheetsNum; sheetItem++)
                {
                    var data = new DataTable();
                    var sheet = _workbook.GetSheetAt(sheetItem);
                    if (sheet == null) continue;
                    IRow firstRow = sheet.GetRow(0);
                    //一行最后一个cell的编号 即总的列数
                    int cellCount = firstRow.LastCellNum;
                    int startRow;

                    #region 创建数据列

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            var column = new DataColumn(firstRow.GetCell(i).StringCellValue);
                            data.Columns.Add(column);
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            var column = new DataColumn();
                            data.Columns.Add(column);
                        }
                        startRow = sheet.FirstRowNum;
                    }

                    #endregion

                    #region 填充数据

                    //最后一列的标号
                    var rowCount = sheet.LastRowNum;
                    for (var i = startRow; i <= rowCount; ++i)
                    {
                        var row = sheet.GetRow(i);
                        //没有数据的行默认是null
                        if (row == null) continue;

                        var dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) == null) continue;
                            var str = row.GetCell(j).ToString();
                            dataRow[j] = str;
                        }
                        data.Rows.Add(dataRow);
                    }
                    #endregion

                    dataSet.Tables.Add(data);
                }
                #endregion

                return dataSet;
            }
            catch (Exception)
            {
                return dataSet;
            }
        }
        #endregion

        #region [6、DataSet与Execl互转]
        /// <summary>	
        /// 传入ds直接生成excel文件	
        /// </summary>	
        /// <param name="ds">DataSet</param>	
        /// <param name="strPath">文件路径</param>	
        /// <param name="reportHeader">execl表头</param>	
        /// <returns></returns>	
        public static bool ExportExcelByDataSet(DataSet ds, string strPath, string reportHeader = "")
        {
            //NPOI	 
            HSSFWorkbook hssfworkbook2 = new HSSFWorkbook();
            #region 循环开始
            for (int p = 0; p < ds.Tables.Count; p++)
            {
                int t = p + 1;
                HSSFSheet sheet = (HSSFSheet)hssfworkbook2.CreateSheet("page" + t);
                //定义字体 font   设置字体类型和大小	
                HSSFFont font = (HSSFFont)hssfworkbook2.CreateFont();
                font.FontName = "宋体";
                font.FontHeightInPoints = 11;

                //定义单元格格式；单元格格式style1 为font的格式	
                HSSFCellStyle style1 = (HSSFCellStyle)hssfworkbook2.CreateCellStyle();
                style1.SetFont(font);
                style1.Alignment = HorizontalAlignment.Left;

                HSSFCellStyle style2 = (HSSFCellStyle)hssfworkbook2.CreateCellStyle();
                style2.SetFont(font);
                style2.Alignment = HorizontalAlignment.Center;
                style2.BorderBottom = BorderStyle.Thin;
                style2.BorderLeft = BorderStyle.Thin;
                style2.BorderRight = BorderStyle.Thin;
                style2.BorderTop = BorderStyle.Thin;
                //style2.WrapText = true;	

                //设置大标题行	
                int rowCount = 0;
                int arrFlag = 0;
                string tileName1 = "";
                string tileName2 = "";

                string s = reportHeader;
                string[] sArray = s.Split('|');
                if (reportHeader != "")
                {
                    foreach (string i in sArray)
                    {
                        string str1 = i;
                        string[] subArray = str1.Split('@');
                        foreach (string k in subArray)
                        {
                            Console.WriteLine(k);
                            if (arrFlag == 0)
                            {
                                tileName1 = k;
                            }
                            else
                            {
                                tileName2 = k;
                            }
                            arrFlag = arrFlag + 1;
                        }
                        HSSFRow row0 = (HSSFRow)sheet.CreateRow(rowCount); //创建报表表头标题  8列	
                        row0.CreateCell(0).SetCellValue(tileName1);
                        row0.CreateCell(1).SetCellValue(tileName2);
                        rowCount = rowCount + 1;
                        arrFlag = 0;
                    }
                }
                //设置全局列宽和行高	
                sheet.DefaultColumnWidth = 14;//全局列宽	
                sheet.DefaultRowHeightInPoints = 15;//全局行高	
                //设置标题行数据	
                int a = 0;

                HSSFRow row1 = (HSSFRow)sheet.CreateRow(rowCount); //创建报表表头标题  8列	
                for (int k = 0; k < ds.Tables[p].Columns.Count; k++)
                {

                    var mColumnName = ds.Tables[p].Columns[k].ColumnName;
                    row1.CreateCell(a).SetCellValue(mColumnName);
                    row1.Cells[a].CellStyle = style2;
                    a++;

                }
                //填写ds数据进excel	
                for (int i = 0; i < ds.Tables[p].Rows.Count; i++)//写6行数据	
                {
                    HSSFRow row2 = (HSSFRow)sheet.CreateRow(i + rowCount + 1);
                    int b = 0;
                    for (int j = 0; j < ds.Tables[p].Columns.Count; j++)
                    {
                        var dgvValue = ds.Tables[p].Rows[i][j].ToString();
                        row2.CreateCell(b).SetCellValue(dgvValue);
                        b++;
                    }
                }
            }
            #endregion

            //获取用户选择路径	
            string reportPath = (strPath);

            //创建excel	
            FileStream file3 = new FileStream(reportPath, FileMode.Create);
            hssfworkbook2.Write(file3);
            file3.Close();
            return true;
        }

        /// <summary>
        /// 用NPOI直接读取excel返回DataSet
        /// </summary>
        /// <param name="excelFileStream">FileStream fs = File.Open(dlg.FileName, FileMode.Open);</param>
        /// <param name="sheetCount">sheet数量</param>
        /// <returns></returns>
        public static DataSet ReadExcelToDataSet(Stream excelFileStream, int sheetCount)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(excelFileStream);
            DataSet ds = new DataSet();

            for (int k = 0; k < sheetCount; k++)
            {
                HSSFSheet sheet = (HSSFSheet)workbook.GetSheetAt(k);
                DataTable table = new DataTable("table" + k);
                HSSFRow headerRow = (HSSFRow)sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    string columnName = headerRow.GetCell(i).StringCellValue;
                    DataColumn column = new DataColumn(columnName);
                    if (!table.Columns.Contains(columnName))
                    {
                        table.Columns.Add(column);
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (0 + 1); i <= rowCount; i++)
                {
                    HSSFRow row = (HSSFRow)sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = row.GetCell(j);
                    }
                    table.Rows.Add(dataRow);
                }
                ds.Tables.Add(table);
            }
            excelFileStream.Close();

            return ds;
        }
        #endregion
    }
}
