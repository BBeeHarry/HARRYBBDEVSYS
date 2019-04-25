using BBDEVSYS.Services.Accrued;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.AccruedReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Report
{
    public class AccruedSummaryReportController : Controller
    {
        // GET: AccruedSummaryReport
        [AuthorizeService(AllowRoleList = new[] { AccruedDetailReportViewModel.RoleForDisplayData, AccruedDetailReportViewModel.RoleForManageData })]
        public ActionResult List()
        {
            AccruedSummaryReportService service = new AccruedSummaryReportService();
            AccruedDetailReportViewModel model = service.InitialListSearch();
            return View("~/Views/AccruedSummaryReport/AccruedSummaryReportList.cshtml", model);
        }

        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { AccruedDetailReportViewModel.RoleForDisplayData, AccruedDetailReportViewModel.RoleForManageData })]
        public string GetList(string companyCode, int monthS, int yearS, int monthE, int yearE,string chnn,int fee,string bu)
        {
            AccruedSummaryReportService service = new AccruedSummaryReportService();
            return service.GetList(companyCode, monthS, yearS,monthE,yearE,chnn,fee,bu);
        }
        [AuthorizeService(AllowRoleList = new[] { AccruedDetailReportViewModel.RoleForDisplayData, AccruedDetailReportViewModel.RoleForManageData })]
        public ActionResult SubmitForm(AccruedDetailReportViewModel formData)
        {
            AccruedSummaryReportService service = new AccruedSummaryReportService();
            byte[] filecontent  = service.SubmitFormFileContent(formData);

            //return null;


            //DataTable dt = service.ExportReport();

            //string g = "";
            //string[] columns = dt.Columns.Cast<DataColumn>()
            //                     .Select(x => x.ColumnName)
            //                     .ToArray();
            //byte[] filecontent = ExcelExportHelper.ExportExcel(dt, "Actual TMN Expense 2018", false, columns);

            string reportTypeName ="All";
            if (formData.FEE_TYPE=="2")
            {
                reportTypeName = "Accrued";
            }
            else if (formData.FEE_TYPE == "3")
            {
                reportTypeName = "Actual";
            }
            if (filecontent == null)
            {
                return List();
            }
            else
            {
                return File(filecontent, ExcelExportHelper.ExcelContentType, reportTypeName + " TMN Expense "+formData.END_YEAR+".xlsx");
            }

        }
        public ActionResult GetCompany(string bu)
        {
            AccruedSummaryReportService service = new AccruedSummaryReportService();
            AccruedDetailReportViewModel model = new AccruedDetailReportViewModel();
            model = service.InitialCompany(bu);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}