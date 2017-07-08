using System;
using System.Collections.Generic;
using FreshCommonUtility.DataConvert;
using FreshCommonUtility.Npoi;

namespace FreshCommonUtilityNetTest.ExcelHelper
{
    public class ExcelHelperTests
    {
        public void DataTableToExcelTest()
        {
            List<Model.TestsTabelToListObject> testList = new List<Model.TestsTabelToListObject>
            {
                new Model.TestsTabelToListObject
                {
                    Age = 10,
                    Height = 20.907,
                    Name = "qinxianbo",
                    Right = true,
                    Sex = Enum.EnumSex.Boy,
                    YouLong = new TimeSpan(1, 1, 1, 1),
                    BrityDay = new DateTime(2017, 2, 3)
                },
                new Model.TestsTabelToListObject
                {
                    Age = 23,
                    Height = 234.907,
                    Name = "秦先波",
                    Right = true,
                    Sex = Enum.EnumSex.Boy,
                    YouLong = new TimeSpan(1, 1, 1, 2),
                    BrityDay = new DateTime(1994, 4, 5)
                },
                new Model.TestsTabelToListObject
                {
                    Age = 40,
                    Height = 20.907,
                    Name = "qinxianbo",
                    Right = true,
                    Sex = Enum.EnumSex.Boy,
                    YouLong = new TimeSpan(1, 1, 1, 3),
                    BrityDay = new DateTime(2017, 2, 23)
                },
                new Model.TestsTabelToListObject
                {
                    Height = 20.907,
                    Name = "杨宏俊",
                    Right = true,
                    Sex = Enum.EnumSex.Grily,
                    YouLong = new TimeSpan(1, 1, 1, 4),
                    BrityDay = new DateTime(1995, 6, 7)
                },
                new Model.TestsTabelToListObject
                {
                    Age = 10,
                    Name = "k",
                    Height = 20.907,
                    Right = true,
                    Sex = Enum.EnumSex.Boy,
                    YouLong = new TimeSpan(1, 1, 1, 5)
                }
            };
            var table = DataTypeConvertHelper.ToDataTable(testList);
            var filePath = "..\\..\\TestUseFile\\DataTableToExcel.xlsx";
            var excelHelper = new NpoiHelper(filePath);
            var result = excelHelper.DataTableToExcel(table, "sheet", true);
            (result - 1).IsEqualTo(testList.Count);
        }

        public void ExcelToDataTableTest()
        {
            var filePath = "..\\..\\TestUseFile\\ExcelToDataTable.xls";
            var excelHelper = new NpoiHelper(filePath);
            string message;
            var table = excelHelper.ExcelToDataTable(null, true, out message);
            table.Rows.Count.IsEqualTo(166);

            filePath = "..\\..\\TestUseFile\\TestExport.xlsx";
            var excelHelperTwo = new NpoiHelper(filePath);
            var tableTwo = excelHelperTwo.ExcelToDataTable(null, true, out message);
            tableTwo.Rows.Count.IsEqualTo(162);
        }
    }
}