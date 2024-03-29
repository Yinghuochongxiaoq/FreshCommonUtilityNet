﻿using System;
using System.Collections.Generic;
using System.Data;
using FreshCommonUtility.DataConvert;
using FreshCommonUtility.Npoi;
using FreshCommonUtilityNetTest.Model;

namespace FreshCommonUtilityNetTest.DataConvert
{
    public class DataTypeConvertHelperTests
    {
        /// <summary>
        /// list to datatable slowly
        /// </summary>
        public void ToDataTableSlowlyTest()
        {
            var filePath = "..\\..\\TestUseFile\\TestExportLessData.xlsx";
            var excelHelperTwo = new NpoiHelper(filePath);
            string message;
            var tableTwo = excelHelperTwo.ExcelToDataTable("sheet1", true, out message);
            List<PersonGattScheduleInfoDto> personGantScheduleInfoDtosList =
                DataTypeConvertHelper.ToList<PersonGattScheduleInfoDto>(tableTwo);
            var storgeTable = DataTypeConvertHelper.ToDataTable(personGantScheduleInfoDtosList);
            tableTwo.Rows.Count.IsEqualTo(storgeTable.Rows.Count);
            var i = 5;
            var j = 4;
            tableTwo.Rows[i][j].IsEqualTo(storgeTable.Rows[i][j]);
        }

        /// <summary>
        /// list to datatable fast
        /// </summary>
        public void ToDataTableTest()
        {
            var filePath = "..\\..\\TestUseFile\\TestExport.xlsx";
            var excelHelperTwo = new NpoiHelper(filePath);
            string message;
            var tableTwo = excelHelperTwo.ExcelToDataTable(null, true, out message);
            List<PersonGattScheduleInfoDto> personGantScheduleInfoDtosList =
                DataTypeConvertHelper.ToList<PersonGattScheduleInfoDto>(tableTwo);
            var storgeTable = DataTypeConvertHelper.ToDataTable(personGantScheduleInfoDtosList);
            tableTwo.Rows.Count.IsEqualTo(storgeTable.Rows.Count);
            var i = 5;
            var j = 4;
            tableTwo.Rows[i][j].IsEqualTo(storgeTable.Rows[i][j]);
        }

        /// <summary>
        /// DataTable to list
        /// </summary>
        public void ToListTest()
        {
            var filePath = "..\\..\\TestUseFile\\TestExport.xlsx";
            var excelHelperTwo = new NpoiHelper(filePath);
            string message;
            var tableTwo = excelHelperTwo.ExcelToDataTable(null, true, out message);
            List<PersonGattScheduleInfoDto> tableToList =
                DataTypeConvertHelper.ToList<PersonGattScheduleInfoDto>(tableTwo);
            tableTwo.Rows.Count.IsEqualTo(tableToList.Count);
        }

        /// <summary>
        /// To int 
        /// </summary>
        public void ToIntTest()
        {
            var intNumber = "90";
            var intNumberResult = DataTypeConvertHelper.ToInt(intNumber);
            intNumberResult.IsEqualTo(90);

            var strNoNumber = "90你好";
            var convertNumberIsZero = DataTypeConvertHelper.ToInt(strNoNumber);
            convertNumberIsZero.IsEqualTo(0);
        }

        /// <summary>
        /// To int
        /// </summary>
        public void ToIntTest1()
        {
            var intNumber = "90";
            var intNumberResult = DataTypeConvertHelper.ToInt(intNumber, 20);
            intNumberResult.IsEqualTo(90);

            var strNoNumber = "90你好";
            var convertNumberIsDefault = DataTypeConvertHelper.ToInt(strNoNumber, 20);
            convertNumberIsDefault.IsEqualTo(20);
        }

        /// <summary>
        /// Chinese to Pinyin
        /// </summary>
        public void GetFullPinyinTest()
        {
            var testChinese = "秦先生";
            var testPinyin = "QinXianSheng";
            var resulteInfo = DataTypeConvertHelper.GetFullPinyin(testChinese);
            resulteInfo.IsEqualTo(testPinyin);
            resulteInfo = DataTypeConvertHelper.GetEachFirstLetterPinyin(testChinese);
            resulteInfo.IsEqualTo("QXS");
        }

        /// <summary>
        /// number to Chinese
        /// </summary>
        public void ToChineseTest()
        {
            var number = 1234567;
            var numberChine = "一百二十三万四千五百六十七";
            var numberOld = "壹佰贰拾叁萬肆仟伍佰陆拾柒";
            var resulteInfo = DataTypeConvertHelper.ToChinese(number, false);
            resulteInfo.IsEqualTo(numberChine);
            resulteInfo = DataTypeConvertHelper.ToChinese(number, true);
            resulteInfo.IsEqualTo(numberOld);
        }

        /// <summary>
        /// Get entity difference 
        /// </summary>
        public void GetEntityDifference()
        {
            var one = new TestsTabelToListObject
            {
                Age = 10,
                Name = "k",
                Height = 20.907,
                Right = true,
                Sex = Enum.EnumSex.Boy,
                YouLong = new TimeSpan(1, 1, 1, 5)
            };

            var two = new TestsTabelToListObject
            {
                Age = 100,
                Name = "k",
                Height = 20.907,
                Right = true,
                Sex = Enum.EnumSex.Boy,
                YouLong = new TimeSpan(1, 1, 1, 5)
            };
            var differenceStr = DataTypeConvertHelper.GetEntityDifference(one, two);
            var resulte = "Age：“10”==>“100”|";
            differenceStr.IsEqualTo(resulte);
        }

        /// <summary>
        /// IDataReader to entity list.
        /// </summary>
        public void ToEntityTest()
        {
            var filePath = "..\\..\\TestUseFile\\TestExport.xlsx";
            var excelHelperTwo = new NpoiHelper(filePath);
            string message;
            var tableTwo = excelHelperTwo.ExcelToDataTable(null, true, out message);
            IDataReader dr = tableTwo.CreateDataReader();
            List<PersonGattScheduleInfoDto> tableToList =
                DataTypeConvertHelper.ToList<PersonGattScheduleInfoDto>(dr);
            tableTwo.Rows.Count.IsEqualTo(tableToList.Count);
        }
    }
}