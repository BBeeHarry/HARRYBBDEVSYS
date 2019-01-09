using BBDEVSYS.Services.Shared;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BBDEVSYS.Controllers.Report
{
    public class ExportExcelController : Controller
    {
        // GET: ExportExcel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExprotToExcel(int id)

        {

            var model = "";// GetExportModel(id);

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=XX_" + DateTime.Now.ToString("ddmmyyyy") + ".xls");

            this.Response.ContentType = "application/vnd.ms-excel";
            return View(model);

        }


        //public ActionResult ExportData()
        //{
        //    String constring = ConfigurationManager.ConnectionStrings[ConstantVariableService.ConnStringServer].ConnectionString;
        //    SqlConnection con = new SqlConnection(constring);
        //    string query = "select * From Employee";
        //    DataTable dt = new DataTable();
        //    dt.TableName = "Employee";
        //    con.Open();
        //    SqlDataAdapter da = new SqlDataAdapter(query, con);
        //    da.Fill(dt);
        //    con.Close();
        //    using (XLWorkbook wb = new XLWorkbook())
        //    {
        //        wb.Worksheets.Add(dt);
        //        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        wb.Style.Font.Bold = true;

        //        Response.Clear();
        //        Response.Buffer = true;
        //        Response.Charset = "";
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", "attachment;filename= EmployeeReport.xlsx");

        //        using (MemoryStream MyMemoryStream = new MemoryStream())
        //        {
        //            wb.SaveAs(MyMemoryStream);
        //            MyMemoryStream.WriteTo(Response.OutputStream);
        //            Response.Flush();
        //            Response.End();
        //        }
        //    }
        //    return RedirectToAction("Index", "ExportData");
        //}

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }



        public ActionResult ExportToExcel()
        {
            var products = new System.Data.DataTable("test");
            products.Columns.Add("col1", typeof(int));
            products.Columns.Add("col2", typeof(string));

            products.Rows.Add(1, "product 1");
            products.Rows.Add(2, "product 2");
            products.Rows.Add(3, "product 3");
            products.Rows.Add(4, "product 4");
            products.Rows.Add(5, "product 5");
            products.Rows.Add(6, "product 6");
            products.Rows.Add(7, "product 7");


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
        public FileResult ExportResults(int id)
        {
            var model = "";// GetExportModel(id);
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("TestSheet1");
            var headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("ID");
            headerRow.CreateCell(1).SetCellValue("TestDataCol1");
            headerRow.CreateCell(2).SetCellValue("TestDataCol2");
            sheet.CreateFreezePane(0, 1, 0, 1);

            int rowNumber = 1;
            //foreach (var datarow in model.data)
            //{
            //    var datestyle = SetCellStyle(workbook, "dd/MM/yyyy");
            //    var row = sheet.CreateRow(rowNumber++);
            //    row.CreateCell(0).SetCellValue(datarow.Id);
            //    row.CreateCell(1).SetCellValue(datarow.TestData1);
            //    row.CreateCell(2).SetCellValue(datarow.TestData2);

            //    row.Cells[1].CellStyle = datestyle;
            //    row.Cells[2].CellStyle = datestyle;
            //}

            MemoryStream output = new MemoryStream();
            workbook.Write(output);
            return File(output.ToArray(), "application/vnd.ms-excel", "ExcelData.xls");
        }

        public void WriteExcelWithNPOI(DataTable dt, String extension)
        {

            IWorkbook workbook;

            if (extension == "xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (extension == "xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                throw new Exception("This format is not supported");
            }

            ISheet sheet1 = workbook.CreateSheet("Sheet 1");

            //make a header row
            IRow row1 = sheet1.CreateRow(0);

            for (int j = 0; j < dt.Columns.Count; j++)
            {

                ICell cell = row1.CreateCell(j);
                String columnName = dt.Columns[j].ToString();
                cell.SetCellValue(columnName);
            }

            //loops through data
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IRow row = sheet1.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    ICell cell = row.CreateCell(j);
                    String columnName = dt.Columns[j].ToString();
                    cell.SetCellValue(dt.Rows[i][columnName].ToString());
                }
            }

            using (var exportData = new MemoryStream())
            {
                Response.Clear();
                workbook.Write(exportData);
                if (extension == "xlsx") //xlsx file format
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ContactNPOI.xlsx"));
                    Response.BinaryWrite(exportData.ToArray());
                }
                else if (extension == "xls")  //xls file format
                {
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ContactNPOI.xls"));
                    Response.BinaryWrite(exportData.GetBuffer());
                }
                Response.End();
            }
        }


    }
}

