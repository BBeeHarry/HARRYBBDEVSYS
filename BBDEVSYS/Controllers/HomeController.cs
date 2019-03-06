using BBDEVSYS.Models.Entities;
using BBDEVSYS.Services.Home;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Invoice;
using BBDEVSYS.ViewModels.Shared;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers
{
    public class HomeController : Controller
    {
        [AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForDisplayData, InvoiceViewModel.RoleForManageData })]
        public ActionResult Index()
        {
            //Below code can be used to include dynamic data in Chart. Check view page and uncomment the line "dataPoints: @Html.Raw(ViewBag.DataPoints)"
            // ViewBag.DataPoints = JsonConvert.SerializeObject(DataService.GetRandomDataForCategoryAxis(10), _jsonSetting);

            //return View();
            return View("~/Views/Home/Home.cshtml");
        }

        public ActionResult BarChart()
        {
            DashboardService service = new DashboardService();

            var model = service.GetDataBarchart();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BarChartChannels()
        {
            DashboardService service = new DashboardService();

            var model = service.GetDataBarchartChannels();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BarChartCompany()
        {
            DashboardService service = new DashboardService();

            var model = service.GetDataBarchartCompany();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BarChartColumn()
        {
            DashboardService service = new DashboardService();

            var model = service.GetDataBarchart();
            ArrayList xValues = new ArrayList();
            ArrayList yValues1 = new ArrayList();
            ArrayList yValues2 = new ArrayList();
            model.ToList().ForEach(rs => xValues.Add(rs.MonthNameFee));
            model.ToList().ForEach(rs => yValues1.Add(rs.Actual));
            model.ToList().ForEach(rs => yValues2.Add(rs.Accrued));


            return File(
            new Chart(width: 800, height: 400).AddTitle("Payment Fee Actual")
        //.DataBindTable(data, "Name")
        .AddSeries(//"Default", 
        chartType: "Column",
        axisLabel: "MonthNameFee",
        xValue: xValues,
        yValues: yValues1,
        name: "Actual")
        .AddSeries(//"Default", 
        chartType: "Column",
        axisLabel: "MonthNameFee",
        xValue: xValues,
        yValues: yValues2,
         name: "Accrued")
         .AddLegend().GetBytes("png")
        //xValue: xValues, xField: "MonthNameFee",
        //yValues: yValues, yFields: "Actual")
        //.Write() ;
        , "image/bytes");
            //return null;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public string GetList()
        {
            HomeService service = new HomeService();
            return service.GetList();

        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private PYMFEEEntities _db = new PYMFEEEntities();
        public ActionResult ChartBarInvoice()
        {
            try
            {
                ViewBag.DataPoints = JsonConvert.SerializeObject(_db.FEE_INVOICE.ToList(), _jsonSetting);

                return View();
            }
            catch (System.Data.Entity.Core.EntityException)
            {
                return View("Error");
            }
            catch (System.Data.SqlClient.SqlException)
            {
                return View("Error");
            }
        }
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public ContentResult CSV()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            dataPoints.Add(new DataPoint(new DateTime(2015, 01, 01), 379540700));
            dataPoints.Add(new DataPoint(new DateTime(2015, 02, 01), 253789800));
            dataPoints.Add(new DataPoint(new DateTime(2015, 03, 01), 388395300));
            dataPoints.Add(new DataPoint(new DateTime(2015, 04, 01), 336969900));
            dataPoints.Add(new DataPoint(new DateTime(2015, 05, 01), 457084600));
            dataPoints.Add(new DataPoint(new DateTime(2015, 06, 01), 248089200));
            dataPoints.Add(new DataPoint(new DateTime(2015, 07, 01), 255413600));
            dataPoints.Add(new DataPoint(new DateTime(2015, 08, 01), 424761000));
            dataPoints.Add(new DataPoint(new DateTime(2015, 09, 01), 454809300));
            dataPoints.Add(new DataPoint(new DateTime(2015, 10, 01), 387429400));
            dataPoints.Add(new DataPoint(new DateTime(2015, 11, 01), 465985100));
            dataPoints.Add(new DataPoint(new DateTime(2015, 12, 01), 313730300));
            dataPoints.Add(new DataPoint(new DateTime(2016, 01, 01), 350835400));
            dataPoints.Add(new DataPoint(new DateTime(2016, 02, 01), 314258500));
            dataPoints.Add(new DataPoint(new DateTime(2016, 03, 01), 231297200));
            dataPoints.Add(new DataPoint(new DateTime(2016, 04, 01), 215798500));
            dataPoints.Add(new DataPoint(new DateTime(2016, 05, 01), 376682300));
            dataPoints.Add(new DataPoint(new DateTime(2016, 06, 01), 373822200));
            dataPoints.Add(new DataPoint(new DateTime(2016, 07, 01), 205076000));
            dataPoints.Add(new DataPoint(new DateTime(2016, 08, 01), 450441900));
            dataPoints.Add(new DataPoint(new DateTime(2016, 09, 01), 420526800));
            dataPoints.Add(new DataPoint(new DateTime(2016, 10, 01), 240008000));
            dataPoints.Add(new DataPoint(new DateTime(2016, 11, 01), 329213700));
            dataPoints.Add(new DataPoint(new DateTime(2016, 12, 01), 218013800));

            string csv = "";

            foreach (DataPoint DataPoint in dataPoints)
            {
                csv += DataPoint.XX.ToString("yyyy-MM-dd") + "," + DataPoint.Y.ToString() + "\n";
            }

            return Content(csv);
        }
    }
}