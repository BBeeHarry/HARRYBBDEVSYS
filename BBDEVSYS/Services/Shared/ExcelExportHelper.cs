using BBDEVSYS.ViewModels.Accrued;
using BBDEVSYS.ViewModels.AccruedReport;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Services.Shared
{
    public class ExcelExportHelper
    {
        public static string ExcelContentType
        {
            get
            { return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; }
        }

        public static DataTable ListToDataTable<T>(List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();

            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public static byte[] AccruedExportExcel(DataSet dataSet, string heading = "", bool showSrNo = false, AccruedViewModel formData = null, params string[] columnsToTake)
        {
            try
            {

                byte[] result = null;
                #region columnName
                //var culture = CultureInfo.GetCultureInfo("en-US");
                //var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                //var month = (formData.PERIOD_YEAR * 12 + formData.PERIOD_MONTH) - (formData.PERIOD_YEAR * 12 + 1);
                //List<string> colNames = new List<string>();
                //List<string> addcolNames = new List<string>();
                //colNames.Add("CHANNELS");
                //colNames.Add("FEE");
                //colNames.Add("CHARGE");
                //addcolNames.AddRange(colNames);
                ////1 == Start Month
                //for (int i = 1 - 1; i <= month; i++)
                //{
                //    if (i > 13)
                //    {
                //        i = 0;
                //    }
                //    int rowmonth = i + 1;
                //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];
                //    addcolNames.Add(monthIndex);

                //}
                //string[] columns = dataSet.Tables[0].Columns.Cast<DataColumn>()
                //  .Where(x => addcolNames.Any(m => x.ColumnName == m))
                //                   .Select(x =>
                //                   (colNames.All(u => x.ColumnName != u) ? x.ColumnName + Convert.ToString(1).Substring(2, 2) : x.ColumnName)
                //                   )
                //                   .ToArray();

                //columnsToTake = columns;
                #endregion

                var culture = CultureInfo.GetCultureInfo("en-US");
                var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                int yearStart = formData.AccruedItemList.Min(m => m.INV_YEAR);
                int yearEnd = formData.AccruedItemList.Max(m => m.INV_YEAR);
                int monthStart = formData.AccruedItemList.Where(m => m.INV_YEAR == yearStart).Min(m => m.INV_MONTH);
                int monthEnd = formData.AccruedItemList.Where(m => m.INV_YEAR == yearEnd).Max(m => m.INV_MONTH);

                using (ExcelPackage package = new ExcelPackage())
                {
                    DataTable dataTable = new DataTable();
                    for (int data = 0; data < dataSet.Tables.Count; data++)
                    {
                        #region Set Column Name
                        if (data == 0)
                        {
                            string[] columns = dataSet.Tables[0].Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();

                            columnsToTake = columns;
                        }
                        else
                        {


                            var month = (yearEnd * 12 + monthEnd) - (yearStart * 12 + monthStart);
                            List<string> colNames = new List<string>();
                            List<string> addcolNames = new List<string>();
                            colNames.Add("CHANNELS");
                            colNames.Add("FEE");
                            colNames.Add("CHARGE");
                            addcolNames.AddRange(colNames);
                            //1 == Start Month
                            int i = 0;
                            int getmonth = 0;
                            int getyear = 0;
                            getmonth = monthStart - 1;
                            getyear = yearStart;
                            while (i <= month)
                            {

                                if (getmonth == 12)
                                {
                                    ++getyear;
                                    getmonth = 0;
                                }
                                int rowmonth = getmonth + 1;
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[getmonth];// + Convert.ToString(getyear).Substring(2, 2);
                                addcolNames.Add(monthIndex);
                                i++;
                                getmonth++;
                            }
                            //for (int i = monthStart-1; i <= month; i++)
                            //{
                            //    if (i > 13)
                            //    {
                            //        i = 0;
                            //    }
                            //    int rowmonth = i + 1;
                            //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];
                            //    addcolNames.Add(monthIndex);

                            //}
                            string[] columns = dataSet.Tables[data].Columns.Cast<DataColumn>()
                              .Where(x => addcolNames.Any(m => x.ColumnName == m))
                                               .Select(x => // x.ColumnName
                                               (colNames.All(u => x.ColumnName != u) ? x.ColumnName + Convert.ToString(yearStart).Substring(2, 2) : x.ColumnName)
                                               )
                                               .ToArray();
                            columnsToTake = columns;

                        }
                        #endregion

                        //DataTable 
                        dataTable = new DataTable();
                        dataTable = dataSet.Tables[data];

                        if (data == 1)
                        {
                            int indx = 3;
                            int cMonth = monthStart;

                            int x = 0;
                            while (x < 12)
                            {
                                if (cMonth>12)
                                {
                                    cMonth = 1;

                                }
                                dataTable.Columns[dateTimeInfo.AbbreviatedMonthNames[cMonth - 1]].SetOrdinal(indx);
                                x++;
                                cMonth++;
                                indx++;
                            }
                        }

                        int colIndex = 0;
                        //rename column names 
                        foreach (DataColumn item in dataTable.Columns)
                        {
                            var valcolumnsToTake = columnsToTake.Where(m => m.Contains(item.ToString())).FirstOrDefault();

                            if (valcolumnsToTake != null)
                            {
                                dataTable.Columns[item.ToString()].ColumnName = valcolumnsToTake.ToString();
                                dataTable.AcceptChanges();
                            }
                            colIndex++;
                            if (columnsToTake.Length == colIndex && colIndex != 0) break;
                        }

                        ExcelWorksheet workSheet = null;

                        //workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading + " " + dataSet.Tables[data].TableName));
                        workSheet = package.Workbook.Worksheets.Add(String.Format(dataSet.Tables[data].TableName));
                        int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 7;

                        if (showSrNo)
                        {
                            DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                            dataColumn.SetOrdinal(0);
                            int index = 1;
                            foreach (DataRow item in dataTable.Rows)
                            {
                                item[0] = index;
                                index++;
                            }
                        }
                        // add the content into the Excel file  
                        workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                        #region set datatable summary set datatype
                        if (data == 0)
                        {
                            workSheet.Cells[startRowFrom + 1, 6, 7 + dataTable.Rows.Count, 6].Style.Numberformat.Format = "#,##0;_-#,##0;0;_-@ ";
                            workSheet.Cells[startRowFrom + 1, 6, 7 + dataTable.Rows.Count, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                            workSheet.Cells[startRowFrom + 1, 7, 7 + dataTable.Rows.Count, 7].Style.Numberformat.Format = "#,##0.00;_-#,##0.00;0;_-@";
                            workSheet.Cells[startRowFrom + 1, 7, 7 + dataTable.Rows.Count, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            workSheet.Cells[startRowFrom + 1, 8, 7 + dataTable.Rows.Count, 8].Style.Numberformat.Format = "#,##0.00;_-#,##0.00;0;_-@";
                            workSheet.Cells[startRowFrom + 1, 8, 7 + dataTable.Rows.Count, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            int rowIgnore = 0;
                            foreach (var cell in workSheet.Cells[startRowFrom + 1, 3, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                            {
                                if (cell.Value == "Inv No." || cell.Value == "PO No.")
                                {
                                    rowIgnore = Convert.ToInt32(cell.Address.Substring(1));
                                }
                                if (!cell.Address.Contains("C"))
                                {
                                    if (rowIgnore != Convert.ToInt32(cell.Address.Substring(1)))
                                    {
                                        cell.Value = Convert.ToDecimal(cell.Value);
                                    }
                                }
                            }
                            using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 4, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                            {
                                string chkcolName = workSheet.Cells[startRowFrom + 1, 3, startRowFrom + dataTable.Rows.Count, 3].Value.ToString();

                                r.Style.Numberformat.Format = "#,##0.00;_-#,##0.00;0;_-@";
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            }


                        }



                        #endregion
                        // autofit width of cells with small content  
                        int columnIndex = 1;
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];

                            int maxLength = columnCells.Max(cell => cell.Value == null ? 0 : cell.Value.ToString().Count());
                            if (maxLength < 150)
                            {
                                workSheet.Column(columnIndex).AutoFit();
                            }
                            columnIndex++;
                        }
                        if (data == 0)
                        {

                            // format header - bold, red on white  
                            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                            {
                                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                r.Style.Font.Bold = true;
                                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ed6383"));

                                //border
                                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.White);

                            }
                            workSheet.Row(startRowFrom).Height = 40;
                        }
                        else //Detail
                        {
                            // format header - bold, red on white  
                            using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                            {
                                r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                r.Style.Font.Bold = true;
                                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#5ae8d2"));

                                //border
                                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.White);

                            }
                            workSheet.Row(startRowFrom).Height = 40;
                            //format highligh -bold, red on white
                            int rowT = 0;
                            int rowMergeS = 0;
                            int rowMergeE = 0;
                            foreach (DataRow item in dataTable.Rows)
                            {

                                rowT++;
                                //count row firs merge
                                if (!string.IsNullOrEmpty(item["CHANNELS"].ToString()))
                                {
                                    rowMergeS = startRowFrom + rowT;
                                }
                                if (item["CHARGE"] == "Total Trxn")
                                {

                                    using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                    {

                                        r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                        r.Style.Font.Bold = true;
                                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#fced6a"));
                                    }

                                }
                                if (item["CHARGE"] == "Total")
                                {
                                    using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                    {

                                        r.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                                        r.Style.Font.Bold = true;
                                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#fced6a"));
                                    }

                                }
                                if (item["CHARGE"] == "PO No.")
                                {
                                    using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                    {

                                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                        r.Style.Font.Bold = false;
                                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#7aa3ef"));
                                    }

                                }
                                if (item["CHARGE"] == "Inv No.")
                                {
                                    rowMergeE = startRowFrom + rowT;
                                    using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                    {
                                        r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                        r.Style.Font.Bold = false;
                                        r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                        r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#7aa3ef"));
                                    }

                                }
                                if (rowMergeS != 0 && rowMergeE != 0 && rowMergeS <= rowMergeE)
                                {

                                    //merge channels column
                                    using (ExcelRange r = workSheet.Cells[rowMergeS, 1, rowMergeE, 1])
                                    {
                                        r.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        r.Merge = true;
                                    }
                                    //merge fee column
                                    using (ExcelRange r = workSheet.Cells[rowMergeS, 2, rowMergeE, 2])
                                    {
                                        r.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                        r.Merge = true;
                                    }

                                    rowMergeS = 0;
                                    rowMergeE = 0;
                                }
                            }
                        }
                        using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                        {

                            r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                        }

                        // removed ignored columns  
                        for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                        {
                            if (i == 0 && showSrNo)
                            {
                                continue;
                            }
                            if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                            {
                                workSheet.DeleteColumn(i + 1);
                            }
                        }

                        if (data == 0)
                        {
                            workSheet.Cells["A1"].Value = "Company Name : ";
                            workSheet.Cells["A1"].Style.Font.Size = 12;
                            workSheet.Cells["A3"].Value = "Period of : ";
                            workSheet.Cells["A3"].Style.Font.Size = 12;
                            workSheet.Cells["A5"].Value = "Department : ";
                            workSheet.Cells["A5"].Style.Font.Size = 12;

                            workSheet.Cells["C1"].Value = formData.COMPANY_CODE_NAME;
                            workSheet.Cells["C1"].Style.Font.Size = 12;
                            workSheet.Cells["C3"].Value = formData.PERIOD_MONTH + "/" + formData.PERIOD_YEAR;
                            workSheet.Cells["C3"].Style.Font.Size = 12;
                            workSheet.Cells["C5"].Value = "Revenue Receiving Management";
                            workSheet.Cells["C5"].Style.Font.Size = 12;

                            workSheet.InsertColumn(1, 1);
                            workSheet.InsertRow(1, 1);
                            workSheet.Column(1).Width = 5;

                            #region Set  Footer  Total
                            //Set  Footer  Total
                            using (ExcelRange r = workSheet.Cells[(startRowFrom + 2 + dataTable.Rows.Count), 2, (startRowFrom + 2 + dataTable.Rows.Count), 6])
                            {
                                r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                r.Merge = true;
                                r.Value = "Total :";
                            }
                            //workSheet.Cells["F" + (startRowFrom + 2 + dataTable.Rows.Count).ToString()].Value = "Total :" ;
                            workSheet.Cells["I" + (startRowFrom + 2 + dataTable.Rows.Count).ToString()].Value = formData.TOTAL_AMT;
                            workSheet.Cells["I" + (startRowFrom + 2 + dataTable.Rows.Count).ToString()].Style.Numberformat.Format = "#,##0.00;_-#,##0.00;0;_-@";

                            using (ExcelRange r = workSheet.Cells[(startRowFrom + 2 + dataTable.Rows.Count), 2, (startRowFrom + 2 + dataTable.Rows.Count), dataTable.Columns.Count + 1])
                            {
                                r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                r.Style.Font.Bold = true;
                                r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#dd6a8b"));

                                //border
                                r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                                r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                                r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                                r.Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                                r.Style.Border.Right.Color.SetColor(System.Drawing.Color.White);

                            }
                            #endregion

                            workSheet.Cells["B" + (startRowFrom * 2 + dataTable.Rows.Count).ToString()].Value = "Remarks :" + formData.REMARK;

                            workSheet.Cells["L" + (startRowFrom * 2 + dataTable.Rows.Count).ToString()].Value = "อนุมัติโดย :";
                            workSheet.Cells["L" + (startRowFrom * 2 + dataTable.Rows.Count + 1).ToString()].Value = "ตำแหน่ง :";
                            workSheet.Cells["M" + (startRowFrom * 2 + dataTable.Rows.Count).ToString()].Value = formData.APPROVED_BY_NAME;
                            workSheet.Cells["M" + (startRowFrom * 2 + dataTable.Rows.Count + 1).ToString()].Value = "Assistant Director";
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(heading))
                            {
                                workSheet.Cells["A1"].Value = heading;
                                workSheet.Cells["A1"].Style.Font.Size = 20;

                                workSheet.InsertColumn(1, 1);
                                workSheet.InsertRow(1, 1);
                                workSheet.Column(1).Width = 5;
                            }
                        }
                        //----------}
                    }
                    //end for
                    result = package.GetAsByteArray();


                    return result;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static byte[] ExportExcel(DataSet dataSet, string heading = "", bool showSrNo = false, AccruedDetailReportViewModel formData = null, params string[] columnsToTake)
        {
            try
            {

                byte[] result = null;
                #region columnName
                var culture = CultureInfo.GetCultureInfo("en-US");
                var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                var month = (formData.END_YEAR * 12 + formData.END_MONTH) - (formData.START_YEAR * 12 + formData.START_MONTH);
                List<string> colNames = new List<string>();
                List<string> addcolNames = new List<string>();
                colNames.Add("CHANNELS");
                colNames.Add("FEE");
                colNames.Add("CHARGE");
                addcolNames.AddRange(colNames);
                for (int i = formData.START_MONTH - 1; i <= month; i++)
                {
                    if (i > 13)
                    {
                        i = 0;
                    }
                    int rowmonth = i + 1;
                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];
                    addcolNames.Add(monthIndex);

                }
                string[] columns = dataSet.Tables[0].Columns.Cast<DataColumn>()
                  .Where(x => addcolNames.Any(m => x.ColumnName == m))
                                   .Select(x =>
                                   (colNames.All(u => x.ColumnName != u) ? x.ColumnName + Convert.ToString(formData.START_YEAR).Substring(2, 2) : x.ColumnName)
                                   )
                                   .ToArray();

                columnsToTake = columns;
                #endregion

                using (ExcelPackage package = new ExcelPackage())
                {
                    DataTable dataTable = new DataTable();
                    for (int data = 0; data < dataSet.Tables.Count; data++)
                    {
                        //DataTable 
                        dataTable = new DataTable();
                        dataTable = dataSet.Tables[data];
                        int colIndex = 0;
                        //rename column names 
                        foreach (var item in dataTable.Columns)
                        {
                            var valcolumnsToTake = columnsToTake.Where(m => m.Contains(item.ToString())).FirstOrDefault();

                            if (valcolumnsToTake != null)
                            {
                                dataTable.Columns[item.ToString()].ColumnName = valcolumnsToTake.ToString();
                                dataTable.AcceptChanges();
                            }
                            colIndex++;
                            if (columnsToTake.Length == colIndex && colIndex != 0) break;
                        }

                        ExcelWorksheet workSheet = null;

                        workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading + " " + dataSet.Tables[data].TableName));

                        int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;

                        if (showSrNo)
                        {
                            DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                            dataColumn.SetOrdinal(0);
                            int index = 1;
                            foreach (DataRow item in dataTable.Rows)
                            {
                                item[0] = index;
                                index++;
                            }
                        }


                        // add the content into the Excel file  
                        workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);
                        #region set DataType Number
                        int rowIgnore = 0;
                        foreach (var cell in workSheet.Cells[startRowFrom + 1, 3, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                        {
                            if (cell.Value == "Inv No." || cell.Value == "PO No.")
                            {
                                rowIgnore = Convert.ToInt32(cell.Address.Substring(1));
                            }
                            if (!cell.Address.Contains("C"))
                            {
                                if (rowIgnore != Convert.ToInt32(cell.Address.Substring(1)))
                                {
                                    cell.Value = Convert.ToDecimal(cell.Value);
                                }
                            }
                        }
                        using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 4, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                        {
                            string chkcolName = workSheet.Cells[startRowFrom + 1, 3, startRowFrom + dataTable.Rows.Count, 3].Value.ToString();

                            r.Style.Numberformat.Format = "#,##0.00;_-#,##0.00;0;_-@";
                            r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        #endregion
                        // autofit width of cells with small content  
                        int columnIndex = 1;
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];

                            int maxLength = columnCells.Max(cell => cell.Value == null ? 0 : cell.Value.ToString().Count());
                            if (maxLength < 150)
                            {
                                workSheet.Column(columnIndex).AutoFit();
                            }


                            columnIndex++;
                        }

                        // format header - bold, red on white  
                        using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                        {
                            r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                            r.Style.Font.Bold = true;
                            r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#e62e00"));

                            //border
                            r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Left.Color.SetColor(System.Drawing.Color.White);
                            r.Style.Border.Right.Color.SetColor(System.Drawing.Color.White);

                        }
                        workSheet.Row(startRowFrom).Height = 40;

                        //format highligh -bold, red on white
                        int rowT = 0;
                        int rowMergeS = 0;
                        int rowMergeE = 0;
                        foreach (DataRow item in dataTable.Rows)
                        {

                            rowT++;
                            //count row firs merge
                            if (!string.IsNullOrEmpty(item["CHANNELS"].ToString()))
                            {
                                rowMergeS = startRowFrom + rowT;
                            }
                            if (item["CHARGE"] == "Total Trxn")
                            {

                                using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                {

                                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    r.Style.Font.Bold = true;
                                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#ed5c68"));
                                }

                            }
                            if (item["CHARGE"] == "Total")
                            {
                                using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                {

                                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    r.Style.Font.Bold = true;
                                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c11f3c"));
                                }

                            }
                            if (item["CHARGE"] == "PO No.")
                            {
                                using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                {

                                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    r.Style.Font.Bold = false;
                                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#305496"));
                                }

                            }
                            if (item["CHARGE"] == "Inv No.")
                            {
                                rowMergeE = startRowFrom + rowT;
                                using (ExcelRange r = workSheet.Cells[startRowFrom + rowT, 3, startRowFrom + rowT, dataTable.Columns.Count])
                                {
                                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                                    r.Style.Font.Bold = false;
                                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#305496"));
                                }

                            }
                            if (rowMergeS != 0 && rowMergeE != 0 && rowMergeS <= rowMergeE)
                            {

                                //merge channels column
                                using (ExcelRange r = workSheet.Cells[rowMergeS, 1, rowMergeE, 1])
                                {
                                    r.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    r.Merge = true;
                                }
                                //merge fee column
                                using (ExcelRange r = workSheet.Cells[rowMergeS, 2, rowMergeE, 2])
                                {
                                    r.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                                    r.Merge = true;
                                }

                                rowMergeS = 0;
                                rowMergeE = 0;
                            }
                        }

                        using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                        {

                            r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                            r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                            r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);

                        }

                        // removed ignored columns  
                        for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                        {
                            if (i == 0 && showSrNo)
                            {
                                continue;
                            }
                            if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                            {
                                workSheet.DeleteColumn(i + 1);
                            }
                        }


                        if (!String.IsNullOrEmpty(heading))
                        {
                            workSheet.Cells["A1"].Value = heading;
                            workSheet.Cells["A1"].Style.Font.Size = 20;

                            workSheet.InsertColumn(1, 1);
                            workSheet.InsertRow(1, 1);
                            workSheet.Column(1).Width = 5;
                        }
                        //----------}
                    }
                    //end for
                    result = package.GetAsByteArray();


                    return result;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static byte[] ExportExcel(DataTable dataTable, string heading = "", bool showSrNo = false, params string[] columnsToTake)
        {

            byte[] result = null;

            DataSet ds = new DataSet();
            using (ExcelPackage package = new ExcelPackage())
            {
                //----------decimal maxRows = dataTable.Rows.Count;
                //----------int maxInit = 200;
                //----------decimal countSheets = maxRows < maxInit ? 1 : maxRows / maxInit;

                //----------int cSheet = Convert.ToInt32(countSheets);



                ExcelWorksheet workSheet = null;
                //---------- for (int sheetNo = 1; sheetNo <= cSheet; sheetNo++)
                //----------{

                workSheet = package.Workbook.Worksheets.Add(String.Format("{0} Data", heading));

                int startRowFrom = String.IsNullOrEmpty(heading) ? 1 : 3;

                if (showSrNo)
                {
                    DataColumn dataColumn = dataTable.Columns.Add("#", typeof(int));
                    dataColumn.SetOrdinal(0);
                    int index = 1;
                    foreach (DataRow item in dataTable.Rows)
                    {
                        item[0] = index;
                        index++;
                    }
                }


                // add the content into the Excel file  
                workSheet.Cells["A" + startRowFrom].LoadFromDataTable(dataTable, true);

                // autofit width of cells with small content  
                int columnIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    ExcelRange columnCells = workSheet.Cells[workSheet.Dimension.Start.Row, columnIndex, workSheet.Dimension.End.Row, columnIndex];

                    int maxLength = columnCells.Max(cell => cell.Value == null ? 0 : cell.Value.ToString().Count());
                    if (maxLength < 150)
                    {
                        workSheet.Column(columnIndex).AutoFit();
                    }


                    columnIndex++;
                }

                // format header - bold, yellow on black  
                using (ExcelRange r = workSheet.Cells[startRowFrom, 1, startRowFrom, dataTable.Columns.Count])
                {
                    r.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    r.Style.Font.Bold = true;
                    r.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    r.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#e62e00"));
                }

                // format cells - add borders  
                using (ExcelRange r = workSheet.Cells[startRowFrom + 1, 1, startRowFrom + dataTable.Rows.Count, dataTable.Columns.Count])
                {
                    r.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    r.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    r.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
                    r.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
                }

                // removed ignored columns  
                for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
                {
                    if (i == 0 && showSrNo)
                    {
                        continue;
                    }
                    if (!columnsToTake.Contains(dataTable.Columns[i].ColumnName))
                    {
                        workSheet.DeleteColumn(i + 1);
                    }
                }

                if (!String.IsNullOrEmpty(heading))
                {
                    workSheet.Cells["A1"].Value = heading;
                    workSheet.Cells["A1"].Style.Font.Size = 20;

                    workSheet.InsertColumn(1, 1);
                    workSheet.InsertRow(1, 1);
                    workSheet.Column(1).Width = 5;
                }
                //----------}


                result = package.GetAsByteArray();
            }

            return result;
        }

        public static byte[] ExportExcel<T>(List<T> data, string Heading = "", bool showSlno = false, params string[] ColumnsToTake)
        {
            return ExportExcel(ListToDataTable<T>(data), Heading, showSlno, ColumnsToTake);
        }
    }
}