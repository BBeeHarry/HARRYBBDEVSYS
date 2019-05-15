using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Adjustrefund;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Adjustrefund;
using Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Adjustrefund
{
    public class AdjustRefundUploadController : Controller
    {
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForDisplayData, AdjustrefundUploadViewModel.RoleForManageData })]
        // GET: AdjustRefundUpload
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);

            AdjustrefundUploadViewModel model = new AdjustrefundUploadViewModel();
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            model = service.InitialListSearch();
            model.NameFormView = "AdjustrefundUploadDetail";
            //return View("~/Views/Adjustrefund/Upload.cshtml");
            return View("~/Views/Adjustrefund/AdjustrefundUploadDetail.cshtml", model);
        }

        // GET: AdjustRefundUpload/Details/5
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForDisplayData, AdjustrefundUploadViewModel.RoleForManageData })]
        public ActionResult Details(int id)
        {
            return View();
        }
        [HttpPost]
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForManageData })]
        public ActionResult SubmitForm(AdjustrefundUploadViewModel formData)
        {


            AdjustRefundUploadService service = new AdjustRefundUploadService();
            ValidationResult result = service.SubmitForm(formData, ModelState);

            //return Json(
            //    new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
            //    JsonRequestBehavior.AllowGet
            //);
            return new JsonResult()
            {
                Data = new
                {
                    success = !result.ErrorFlag,
                    responseText = result.Message,
                    errorList = result.ModelStateErrorList,
                    dataCallback = result.AdditionalInfo1
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                //MaxJsonLength = Int32.MaxValue // Use this value to set your maximum size for all of your Requests
            };
        }

        //[HttpPost]
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForManageData })]
        public ActionResult SubmitMergeForm(AdjustrefundUploadViewModel formData)
        {

            AdjustRefundUploadService service = new AdjustRefundUploadService();


            byte[] filecontent = service.SubmitFormFileContent(formData);

            //DataSet getDataset = new DataSet();

            //getDataset = AdjustRefundUploadService.InitialDataFormViewModel(formData);



            //if (getDataset.Tables.Count > 0)
            //{

            //    filecontent = ExcelExportHelper.ExportExcelMutiSheetData(getDataset, "", false);//, columns);
            //}

            //HttpPostedFileBase file = Request.Files[0];
            //if (file.ContentLength > 0)
            //{
            //    var fileName = Path.GetFileName(file.FileName);
            //    assignment.FileLocation = Path.Combine(
            //        Server.MapPath("~/App_Data/uploads"), fileName);
            //    file.SaveAs(assignment.FileLocation);
            //}

            string shotNameTeam = string.Empty;

            shotNameTeam = SettingService.SetShortName(formData.UserRequest);

            if (filecontent == null)
            {
                return List();
            }
            else
            {
                //DownloadCSV(getDataset.Tables["Fund Transfer Memo"]);
                return File(filecontent, ExcelExportHelper.ExcelContentType, shotNameTeam+ "_Mapping Input_"+DateTime.Now.Date.ToString("yyyyMMdd")+ "_MIS format_Adjust" + ".xlsx");
             
            }
            
        }
       public ActionResult DownloadCSV(DataTable formData)
        {

            AdjustRefundUploadService service = new AdjustRefundUploadService();

            #region export CSV

            DataTable dt = new DataTable();
            dt = formData;// AdjustRefundUploadService.InitialDataFormViewModel(formData).Tables["Fund Transfer Memo"] ;
            //Build the CSV file data as a Comma separated string.
            string csvHeader = string.Empty;
            //Build the CSV file data as a Comma separated string.
            string csvDetails = string.Empty;
            //foreach (DataRow rows in dt.Rows)
            //{
            foreach (DataColumn column in dt.Columns)
            {
                //Add the Header row for CSV file.
                csvDetails += column.ColumnName + ' ';
            }


            //Add new line.
            csvDetails += "\r\n";

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    //Add the Data rows.
                    csvDetails += row[column.ColumnName].ToString().Replace(",", ";") + ' ';
                }

                //Add new line.
                csvDetails += "\r\n";
            }
            //}

            // Download the CSV file.
            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.Buffer = true;
            //HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=AR_FNTT_" + DateTime.Now.Date.ToString("yyyyMMdd") + ".csv");
            //HttpContext.Current.Response.Charset = "";
            //HttpContext.Current.Response.ContentType = "application/text";
            //HttpContext.Current.Response.Output.Write(csvDetails);
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();

            #endregion
            
            //string shotNameTeam = string.Empty;

            //shotNameTeam = SettingService.SetShortName(formData.UserRequest);
            if (csvDetails == "")
            {
                return List();
            }
            else
            {
                return File(new System.Text.UTF8Encoding().GetBytes(csvDetails), "text/csv", "AR_FNTT_" + DateTime.Now.Date.ToString("yyyyMMdd") + ".csv");
            }

        }
        // return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report123.csv");
        //    System.IO.FileInfo exportFile = //create your ExportFile
        //return File(exportFile.FullName, "text/csv", string.Format("Export-{0}.csv", DateTime.Now.ToString("yyyyMMdd-HHmmss")));
        public PartialViewResult PreviewData(AdjustrefundUploadViewModel formData)
        {
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            AdjustrefundUploadViewModel model = service.SubmitMergeFormData(formData);
            if (model != null)
            {
                return PartialView("~/Views/Adjustrefund/AdjustrefundUploadItems.cshtml", model);
            }
            else
            {
                return null;
            }

        }
        public ActionResult PreviewUploadMISData(string srData)
        {
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            AdjustrefundUploadViewModel model = service.SubmitPreviewUploadData(srData);

            //if (model != null)
            //{
            //    return PartialView("~/Views/Adjustrefund/AdjustrefundPreviewItems.cshtml", model);
            //}
            //else
            //{
            //    return null;
            //}
            return new JsonResult()
            {
                Data = new
                {
                    success = true,
                    responseText = "",
                    html = UtilityService.RenderPartialView(this, "~/Views/Adjustrefund/AdjustrefundPreviewItems.cshtml", model)
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue // Use this value to set your maximum size for all of your Requests
            };

        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase[] files)
        {

            //Ensure model state is valid  
            if (ModelState.IsValid)
            {   //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var InputFileName = Path.GetFileName(file.FileName);
                        var ServerSavePath = Path.Combine(Server.MapPath("~/UploadedFiles/") + InputFileName);
                        //Save file to server folder  
                        file.SaveAs(ServerSavePath);
                        //assigning file uploaded status to ViewBag for showing message to user.  
                        ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
                    }

                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");
                        return View();
                    }

                    reader.IsFirstRowAsColumnNames = true;

                    DataSet result = reader.AsDataSet();
                    reader.Close();

                    return View(result.Tables[0]);
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View();
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForDisplayData, AdjustrefundUploadViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey)
        {

            AdjustRefundUploadService service = new AdjustRefundUploadService();
            AdjustrefundUploadViewModel adjrefundViewModel = new AdjustrefundUploadViewModel();
            //service.InitialDetailView(recordKey, ConstantVariableService.FormStateDisplay);

            if (adjrefundViewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
                return View("~/Views/Adjustrefund/AdjustrefundUploadDetail.cshtml", adjrefundViewModel);
            }
            else
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
                //return to List page
                return View("~/Views/Adjustrefund/AdjustrefundUploadDetail.cshtml");
            }

        }

        [HttpPost]
        //[ActionName("Index")]
        public ActionResult Index_Post(string fileName, string sheetName)
        {
            //Fill dataset with records
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
            tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);
            //DataSet excelDS = ExcelService.ConvertExcelToDataSet(tempFilePath, fileName, 1);
            ValidationWithReturnResult<DataSet> ds = new ValidationWithReturnResult<DataSet>();
            ds = service.ConvertExcelToDataSet(tempFilePath, fileName, sheetName);
            DataSet dataSet = new DataSet();
            dataSet = ds.ReturnResult;
            //DataSet dataSet = GetRecordsFromDatabase();

            StringBuilder sb = new StringBuilder();

            sb.Append("<table>");

            //LINQ to get Column names
            var columnName = dataSet.Tables[0].Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            sb.Append("<tr>");
            //Looping through the column names
            foreach (var col in columnName)
                sb.Append("<td>" + col + "</td>");
            sb.Append("</tr>");

            //Looping through the records
            foreach (DataRow dr in dataSet.Tables[0].Rows)
            {
                sb.Append("<tr>");
                foreach (DataColumn dc in dataSet.Tables[0].Columns)
                {
                    sb.Append("<td>" + dr[dc] + "</td>");
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");

            //Writing StringBuilder content to an excel file.
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=UserReport.xls");
            Response.Write(sb.ToString());
            Response.Flush();
            Response.Close();

            AdjustrefundUploadViewModel model = new AdjustrefundUploadViewModel();

            return View("~/Views/Adjustrefund/AdjustrefundUploadDetail.cshtml", model);
        }


        [HttpPost]
        public ActionResult ImporttoDatabase(AdjustrefundUploadViewModel importExcel)
        {
            if (ModelState.IsValid)
            {
                string path = "";// Server.MapPath("~/Content/Upload/" + importExcel.files.FileName);
                //importExcel.files.SaveAs(path);

                string excelConnectionString = @"Provider='Microsoft.ACE.OLEDB.12.0';Data Source='" + path + "';Extended Properties='Excel 12.0 Xml;IMEX=1'";
                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                //Sheet Name
                excelConnection.Open();
                string tableName = excelConnection.GetSchema("Tables").Rows[0]["TABLE_NAME"].ToString();
                excelConnection.Close();
                //End

                OleDbCommand cmd = new OleDbCommand("Select * from [" + tableName + "]", excelConnection);

                excelConnection.Open();

                OleDbDataReader dReader;
                dReader = cmd.ExecuteReader();
                SqlBulkCopy sqlBulk = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["CS"].ConnectionString);

                //Give your Destination table name
                sqlBulk.DestinationTableName = "sale";

                //Mappings
                sqlBulk.ColumnMappings.Add("Date", "AddedOn");
                sqlBulk.ColumnMappings.Add("Region", "Region");
                sqlBulk.ColumnMappings.Add("Person", "Person");
                sqlBulk.ColumnMappings.Add("Item", "Item");
                sqlBulk.ColumnMappings.Add("Units", "Units");
                sqlBulk.ColumnMappings.Add("Unit Cost", "UnitCost");
                sqlBulk.ColumnMappings.Add("Total", "Total");

                sqlBulk.WriteToServer(dReader);
                excelConnection.Close();

                ViewBag.Result = "Successfully Imported";
            }
            return View();
        }


        public ActionResult ImportExcel(string fileName, string sheetName)
        {
            List<AdjustrefundUploadViewModel> uploadViewModel = new List<AdjustrefundUploadViewModel>();
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            var result = service.ImportExcel(fileName, sheetName);

            #region generate table

            //DataSet dataSet = new DataSet();
            //dataSet = DatatablesService.ConvertObjectToDatatables(result.ReturnResult[0].adjList);

            //StringBuilder sb = new StringBuilder();

            //sb.Append("<table>");

            ////LINQ to get Column names
            //var columnName = dataSet.Tables[0].Columns.Cast<DataColumn>()
            //                     .Select(x => x.ColumnName)
            //                     .ToArray();
            //sb.Append("<tr>");
            ////Looping through the column names
            //foreach (var col in columnName)
            //    sb.Append("<td>" + col + "</td>");
            //sb.Append("</tr>");

            ////Looping through the records
            //foreach (DataRow dr in dataSet.Tables[0].Rows)
            //{
            //    sb.Append("<tr>");
            //    foreach (DataColumn dc in dataSet.Tables[0].Columns)
            //    {
            //        sb.Append("<td>" + dr[dc] + "</td>");
            //    }
            //    sb.Append("</tr>");
            //}

            //sb.Append("</table>");

            ////Writing StringBuilder content to an excel file.
            //Response.Clear();
            //Response.ClearContent();
            //Response.ClearHeaders();
            //Response.Charset = "";
            //Response.Buffer = true;
            //Response.ContentType = "application/vnd.ms-excel";
            //Response.AddHeader("content-disposition", "attachment;filename=UserReport.xls");
            //Response.Write(sb.ToString());
            //Response.Flush();
            //Response.Close();
            #endregion

            if (!result.ErrorFlag)
            {
                uploadViewModel = result.ReturnResult;


                return new JsonResult()
                {
                    Data = new
                    {
                        success = !result.ErrorFlag,
                        responseText = result.Message,
                        html = ""//UtilityService.RenderPartialView(this, "~/Views/InvoiceUpload/InvoiceUploadItems.cshtml", uploadInvoiceItemViewModel)
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue // Use this value to set your maximum size for all of your Requests
                };
            }
            else
            {
                return Json(
                    new
                    {
                        success = !result.ErrorFlag,
                        responseText = result.Message,
                        html = "",
                        errorList = result.ModelStateErrorList
                    },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForDisplayData, AdjustrefundUploadViewModel.RoleForManageData })]
        // GET: AdjustRefundMapping
        public ActionResult LastAdjustList()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            //PaymentItemsService service = new PaymentItemsService();
            AdjustrefundUploadViewModel model = new AdjustrefundUploadViewModel();
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            model = service.InitialListSearch("ALL");
            model.NameFormView = "AdjustrefundCheckAdjDetail";
            //return View("~/Views/Adjustrefund/Upload.cshtml");
            return View("~/Views/Adjustrefund/AdjustrefundCheckAdjDetail.cshtml", model);
        }
        //[HttpPost]
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForManageData })]
        public ActionResult SubmitMappingForm(AdjustrefundUploadViewModel formData)
        {

            AdjustRefundUploadService service = new AdjustRefundUploadService();
            byte[] filecontent = service.SubmitFormFileMappingContent(formData);
            string shotNameTeam = string.Empty;

            shotNameTeam = SettingService.SetShortName(formData.UserRequest);
            if (filecontent == null)
            {
                return List();
            }
            else
            {
                return File(filecontent, ExcelExportHelper.ExcelContentType, shotNameTeam+ "_Mapping Input_"+DateTime.Now.Date.ToString("yyyyMMdd")+ "_MIS format_Convert" + ".xlsx");

            }
        }

        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForManageData })]
        public ActionResult SubmitReportForm(AdjustrefundUploadViewModel formData)
        {

            AdjustRefundUploadService service = new AdjustRefundUploadService();
            byte[] filecontent = service.SubmitFormFileCloseAndSendContent(formData);

            string shotNameTeam = string.Empty;

            shotNameTeam = SettingService.SetShortName(formData.UserRequest);
            string fileName = shotNameTeam + "_Summary Send out_" + DateTime.Now.Date.ToString("dd MMM yyyy") + "_Send Close SR & Done Activity";
            if (formData.UserRequest == "00002222" || formData.UserRequest =="00004444")
            {
                fileName = shotNameTeam + "_Summary Send out_" + DateTime.Now.Date.ToString("dd MMM yyyy") + "_result_Send out";
            }
            if (filecontent == null)
            {
                return List();
            }
            else
            {
                return File(filecontent, ExcelExportHelper.ExcelContentType, fileName + ".xlsx");

            }
        }

        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForDisplayData, AdjustrefundUploadViewModel.RoleForManageData })]
        // GET: AdjustRefundUpload to MIS
        public ActionResult UploadList()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            //PaymentItemsService service = new PaymentItemsService();
            AdjustrefundUploadViewModel model = new AdjustrefundUploadViewModel();
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            model = service.InitialListSearch("ALL");
            model.NameFormView = "AdjustrefundUploadMISDetail";
            model.UserRequest = "00003333";
            model.HeaderSummary = "Result Summary Upoload Data of "+DateTime.Now.Date.ToString("dd/MM/yyyy");
            //return View("~/Views/Adjustrefund/Upload.cshtml");
            return View("~/Views/Adjustrefund/AdjustrefundUploadMISDetail.cshtml", model);
        }
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForDisplayData, AdjustrefundUploadViewModel.RoleForManageData })]
        // GET: AdjustRefundUpload to MIS
        public ActionResult SummaryReportList()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            AdjustrefundUploadViewModel model = new AdjustrefundUploadViewModel();
            AdjustRefundUploadService service = new AdjustRefundUploadService();
            model = service.InitialListSearch("ALL");
            model.NameFormView = "AdjustrefundDoneAndClose";
            model.UserRequest = "00003333";
            return View("~/Views/Adjustrefund/AdjustrefundDoneAndClose.cshtml", model);
        }

        //[HttpPost]
        [AuthorizeService(AllowRoleList = new[] { AdjustrefundUploadViewModel.RoleForManageData })]
        public ActionResult SubmitUploadForm(AdjustrefundUploadViewModel formData)
        {

            AdjustRefundUploadService service = new AdjustRefundUploadService();

            //AdjustrefundUploadViewModel model = service.SubmitMergeFormData(formData);

            ValidationResult result = service.SubmitForm(formData, ModelState);
            return Json(
              new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
              JsonRequestBehavior.AllowGet
          );

        }

        public ActionResult ImportExcelGenerateTable(AdjustrefundUploadViewModel formData)
        {
            DataTable uploadViewModel = new DataTable();
            AdjustRefundUploadService service = new AdjustRefundUploadService();

            var result = service.InitialDataFormUploadViewModel(formData);


            AdjustrefundUploadViewModel model = formData;

            if (!result.ErrorFlag)
            {
                uploadViewModel = result.ReturnResult;
                model.adjDataTable = result.ReturnResult;

                return new JsonResult()
                {
                    Data = new
                    {
                        success = !result.ErrorFlag,
                        responseText = result.Message,
                        html = UtilityService.RenderPartialView(this, "~/Views/Adjustrefund/AdjustrefundUploadItems.cshtml", model)
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = Int32.MaxValue // Use this value to set your maximum size for all of your Requests
                };
            }
            else
            {
                return Json(
                    new
                    {
                        success = !result.ErrorFlag,
                        responseText = result.Message,
                        html = "",
                        errorList = result.ModelStateErrorList
                    },
                    JsonRequestBehavior.AllowGet);
            }
        }



    }
}