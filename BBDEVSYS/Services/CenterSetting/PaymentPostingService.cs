using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.ViewModels.Posting;
using System.Transactions;
using System.Data;
using BBDEVSYS.Services.Shared;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using BBDEVSYS.Models.Entities;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;

namespace BBDEVSYS.Services.CenterSetting
{
    public partial class PaymentPostingService
    {
        public void ExportReport(PaymentPostingViewModel formData)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //using (var context = new MIS_PAYMENTEntities())
                    //{
                    //    var postingDB = context.PAYMENT_POSTING_MONTH.ToList();
                    //}
                }


                #region GetReportData
                DataTable getDataReport = new DataTable();
                ReportDocument rd = new ReportDocument();

                rd.Load(Path.Combine(HttpContext.Current.Server.MapPath("~/Reports/CAEvaluation/CALevelComparison"), "CALevelComparisonReport.rpt"));



                getDataReport = null;//ReportService.ConvertListToDatatable(null);

                getDataReport.TableName = "CALevelComparison";
                rd.SetDataSource(getDataReport);
                rd.SetParameterValue("getFormPeriodName", "");
                #endregion
                #region Export EXCEL
                //Encoding FileName
                //periodName = Uri.EscapeDataString(Path.GetFileNameWithoutExtension("")).Replace("%20", " ") + Path.GetExtension(periodName);
                ReportService.ExportExcel(rd, "");
                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable ExportReport()
        {
            DataTable dt = new DataTable();
            try
            {
                //using (var context  = new MIS_PAYMENTEntities())
                //{
                //    var data = (from m in context.PAYMENT_SOURCE select m).ToList();

                    

                //    dt = ReportService.ConvertListToDatatable(data);



                //}

                #region getData
                //using (var context = new MIS_PAYMENTEntities())
                //{
                //    DateTime sDate = Convert.ToDateTime("2018-06-01");
                //    DateTime eDate = Convert.ToDateTime("2018-06-20");
                //    /*  
                //    data = data.Where(m => m.CUSTOMER_PYM_DATE.Value >= sDate && m.CUSTOMER_PYM_DATE.Value <= eDate).ToList();*/
                //    var data = (from m in context.PAYMENT_POSTING_MONTH
                //                where m.CUSTOMER_PYM_DATE.Value >= sDate && m.CUSTOMER_PYM_DATE.Value <= eDate
                //                select m).ToList();



                //    string[] comp = { "Mobile", "Online", "TV", "Convergence" };
                //    List<string> lsCom = comp.ToList();

                //    var dataCompany = (from m in context.COMPANY
                //                       select m).ToList();
                //    dataCompany = dataCompany.Where(l => lsCom.All(o => o == l.COMPANY_TYPE) && l.BAN_COMPANY != "TR").ToList();

                //    var dataSource = (from m in context.PAYMENT_SOURCE
                //                      where m.SOURCE_TYPE != "M"
                //                      && m.POSTING_SYSTEM == "O"
                //                      && m.EXCEPT_KPI_STATUS == true
                //                      select m
                //                      ).ToList();

                //    dataSource = dataSource.Where(s => dataCompany.Any(o => s.COMPANY == o.BAN_COMPANY)).ToList();

                //    data = data.Where(m => dataCompany.Any(o => m.BAN_COMPANY == o.BAN_COMPANY)
                //                            && dataSource.Any(p => m.SOURCE_ID == p.SOURCE_ID)).ToList();

                //    var dataOffset = data.GroupBy(g => new { g.SYS_RECEIVE_DATE, g.BAN, g.TAX_INV_NUMBER, g.SOURCE_ID, g.BAN_COMPANY, g.PAYMENT_CHANNEL, g.SUB_PAYMENT_CHANNEL, g.VOUCHER_INV })
                //        //g=>g.ACCOUNT_TYPE == "X" && g.ACCOUNT_SUB_TYPE == "ECA" && g.SOURCE_ID.StartsWith("T") && g.SUB_PAYMENT_CHANNEL.StartsWith("PAS"))
                //        .Select(m => new { m, AMT_OFFSET = m.Sum(s => s.ACTV_AMT) }).Where(m => m.AMT_OFFSET != 0).SelectMany(t => t.m.Select(b => b)
                //        ).ToList();

                //    data = dataOffset.Where(m => (m.Line3 == null ? "" : m.Line3) == "" && m.ACTV_CODE == "PYM").ToList();

                //    var dataECA = data.Where(g => g.ACCOUNT_TYPE == "X" && g.ACCOUNT_SUB_TYPE == "ECA" && g.SOURCE_ID.StartsWith("T") && g.SUB_PAYMENT_CHANNEL.StartsWith("PAS")).ToList();

                //    data = data.Where(m => !dataECA.All(o => m.TRANID == o.TRANID)).ToList();

                //    data = data.Where(n => !data.Any(p => p.SOURCE_ID == "OPO" || p.SOURCE_ID == "OPB")).ToList();

                //    DateTime dateEmp = Convert.ToDateTime("2018-06-25");
                //    var getData = (from D in data
                //                   join s in dataSource on D.SOURCE_ID equals s.SOURCE_ID// new { D.SOURCE_ID ,D.BAN_COMPANY ,D.AR_system} equals new { s.SOURCE_ID ,s.COMPANY,s.AR_System}

                //                   select new
                //                   {
                //                       D.TRANID,
                //                       D.SYS_RECEIVE_DATE,
                //                       D.SYS_CREATION_DATE,
                //                       D.CUSTOMER_PYM_DATE,
                //                       D.POSTING_DATE,
                //                       D.ACTV_AMT,
                //                       D.ORIGINAL_BAN,
                //                       D.BAN,

                //                       D.TAX_INV_NUMBER,
                //                       D.SOURCE_ID,
                //                       D.AR_system,
                //                       D.BAN_COMPANY,
                //                       D.PAYMENT_CHANNEL,
                //                       D.SUB_PAYMENT_CHANNEL,
                //                       D.File_Name_AR,
                //                       D.SOURCE_TYPE,
                //                       D.PRODUCT_ID,
                //                       D.Line3,
                //                       D.ACCOUNT_TYPE,
                //                       D.ACCOUNT_SUB_TYPE,
                //                       POST_PERIOD_sec = DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE),
                //                       POST_PERIOD_min = DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE),

                //                       CHANNEL_TYPE = (s.SOURCE_TYPE == "O") ?
                //                       ((DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) <= 15) ? "Online" :
                //                       ((DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 15) ? "Online-Batch" : "Online-Batch (Error:Invalid Data)")) :
                //                       "Batch",

                //                       KPI = (s.SOURCE_TYPE == "O") ?
                //                         (DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) <= 15) ? "ON-TIME" :
                //                         (DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 15) ? "OVER-TIME" :
                //                         (D.SUB_PAYMENT_CHANNEL == "Employee Program" && D.POSTING_DATE <= dateEmp) ? "ON-TIME" :
                //                         (D.SUB_PAYMENT_CHANNEL == "Employee Program" && D.POSTING_DATE > dateEmp) ? "OVER-TIME"
                //                         : "OVER-TIME" : "OVER-TIME",

                //                       BUCKET = (DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 16 && DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 5) ? "00:<=15 sec." :
                //                                   (DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 15 && DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 61) ? "01:16-60sec." :
                //                                   (DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 60 && DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 6) ? "02:61sec.-5min." :
                //                                   (DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 5 && DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 16) ? "03:05-15min." :
                //                                   (DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 15 && DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 31) ? "04:16-30min." :
                //                                   (DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 30 && DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 61) ? "05:31-60min." :
                //                                   (DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 60 && DbFunctions.DiffMinutes(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 1441) ? "06:60min.-1440(1day)."
                //                               : "07:Next day",


                //                       SLA_KPI = DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 301 ? "1. WITHIN 5min." :
                //                                   (DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 300 && DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 601) ? "2. RANGE 5-10min." :
                //                                   (DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 600 && DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 901) ? "3. RANGE 10-15min." :
                //                                   (DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 900 && DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 86401) ? "4. WITHIN 24hrs." :
                //                                   (DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) > 86400 && DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) < 86401) ? "5. WITHOUT 24hrs."
                //                               : "WITHOUT 24hrs.",

                //                       POST_PERIOD_Days = string.Concat(Convert.ToString(DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) / 60 / 60 / 24), "Day(s),",
                //                               Convert.ToString(DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) / 60 / 60 % 24), " Hour(s),",
                //                               Convert.ToString(DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) / 60 % 60), " Minute(s),",
                //                               Convert.ToString(DbFunctions.DiffSeconds(D.SYS_RECEIVE_DATE, D.SYS_CREATION_DATE) % 60), "Second(s)."
                //                                               )
                //                   }


                //   ).ToList();
                //    dt = ReportService.ConvertListToDatatable(getData);

                //}
                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }

        public void ExportReportTest()
        {
            try
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


                DateTime dat = DateTime.Now;

                //using (var context = new MIS_PAYMENTEntities())
                //{
                //    //context.Database.SqlQuery
                //    //IEnumerable<EmployeeDetails> empDetails = context.ExecuteStoreQuery<EmployeeDetails> ("exec GetEmployeeData").ToList();
                //}

                //1. Get the connection string
                string constr = "Data Source = HCW_T15-APATSAR; Initial Catalog = MIS_PAYMENT; user id = sa; password = True2017; MultipleActiveResultSets = true"; //ConfigurationManager.ConnectionStrings[ConstantVariableService.ConnStringServer].ToString();

                DataTable dt = new DataTable(); //used to store the store procedure result.
                                                //List<MyEmployee> list = new List<MyEmployee>();  //used to store the model.

                using (SqlConnection con = new SqlConnection(constr))
                {
                    // con.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_mis_payment_posting_timeliness", con))
                    {
                        con.Open();

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DATE_START", dat);
                        cmd.CommandTimeout = 7200;
                        SqlDataAdapter ada = new SqlDataAdapter();
                        ada.SelectCommand = cmd;
                        ada.Fill(dt);
                        con.Close();
                        //--Convert the query result to MVC model.
                        // var d = dt.ta;
                        //list = dt.AsEnumerable()
                        //    .Select(c => new MyEmployee
                        //    {
                        //        EmployeeID = c.Field<int>("EmployeeID"),
                        //        LastName = c.Field<string>("LastName"),
                        //        FirstName = c.Field<string>("FirstName")
                        //    }).ToList();
                    }
                }




                WriteExcelWithNPOI(products, "xlsx");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void WriteExcelWithNPOI(DataTable dt, String extension)
        {


            IWorkbook workbook;

            if (extension == "xlsx")
            {
                //application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml
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
            //wb.Worksheets.Add(dt);
            //        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //        wb.Style.Font.Bold = true;


            ISheet sheet1 = workbook.CreateSheet("Detail");


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
                HttpContext.Current.Response.Clear();
                workbook.Write(exportData);
                if (extension == "xlsx") //xlsx file format
                {
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ContactNPOI.xlsx"));
                    HttpContext.Current.Response.BinaryWrite(exportData.ToArray());
                }
                else if (extension == "xls")  //xls file format
                {
                    HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "ContactNPOI.xls"));
                    HttpContext.Current.Response.BinaryWrite(exportData.GetBuffer());
                }
                HttpContext.Current.Response.End();
            }
        }

    }
}