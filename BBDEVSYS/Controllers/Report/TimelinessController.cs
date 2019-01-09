using BBDEVSYS.Services.CenterSetting;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Posting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Report
{
    public class TimelinessController : Controller
    {
        // GET: rptTimeliness
        public ActionResult List()
        {

            return View("~/Views/Report/PaymentPostingTimeliness/TimelinessReport.cshtml");
        }

        public ActionResult SubmitForm(PaymentPostingViewModel formData)
        {

            PaymentPostingService service = new PaymentPostingService();
            //service.ExportReport(formData);

            service.ExportReport(formData);
            return null;

        }

        [HttpGet]
        public FileContentResult ExportToExcel()
        {
            PaymentPostingService service = new PaymentPostingService();
          

            DataTable  dt =  service.ExportReport();

            string g = "";
            string[] columns = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            byte[] filecontent = ExcelExportHelper.ExportExcel(dt, "Technology", false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "Technologies.xlsx");
        }

    }
}