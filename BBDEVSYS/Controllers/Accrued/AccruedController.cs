using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Accrued;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Accrued;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BBDEVSYS.Controllers.Accrued
{
    public class AccruedController : Controller
    {
        // GET: Accrued
        [AuthorizeService(AllowRoleList = new[] { AccruedViewModel.RoleForDisplayData, AccruedViewModel.RoleForManageData })]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            ViewBag.DisplayOnly = AuthorizeService.CheckDisplayOnly(new[] { AccruedViewModel.RoleForManageData });
            AccruedService service = new AccruedService();
            AccruedViewModel model = service.InitialListSearch();
            return View("~/Views/Accrued/AccruedList.cshtml", model);
        }

        ////For ajax load list data
        //[AuthorizeService(AllowRoleList = new[] { AccruedViewModel.RoleForDisplayData, AccruedViewModel.RoleForManageData })]
        //public string GetList()
        //{
        //    AccruedService service = new AccruedService();
        //    return service.GetList();
        //}
        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { AccruedViewModel.RoleForDisplayData, AccruedViewModel.RoleForManageData })]
        public string GetList(string companyCode, int monthValue, int yearValue)
        {
            AccruedService service = new AccruedService();
            return service.GetList(companyCode, monthValue, yearValue);
        }
        [HttpPost]
        [AuthorizeService(AllowRoleList = new[] { AccruedViewModel.RoleForManageData })]
        public ActionResult SubmitForm(AccruedViewModel formData)
        {
            foreach (var item in formData.AccruedItemList)
            {
                AccruedDetailViewModel accruedItem = new AccruedDetailViewModel();
                JavaScriptSerializer js = new JavaScriptSerializer();
                accruedItem = js.Deserialize<AccruedDetailViewModel>(item.AccruedJSON);
                var accruedItemSub = accruedItem.AccruedItemSubList;
                item.AccruedItemSubList.AddRange(accruedItemSub);
            }


            AccruedService service = new AccruedService();
            ValidationResult result = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
            );
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AccruedViewModel.RoleForDisplayData, AccruedViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey)
        {

            AccruedService service = new AccruedService();
            AccruedViewModel AccruedViewModel = service.InitialDetailView(recordKey, ConstantVariableService.FormStateDisplay);

            if (AccruedViewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
                return View("~/Views/Accrued/AccruedDetail.cshtml", AccruedViewModel);
            }
            else
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
                //return to List page
                return View("~/Views/Accrued/AccruedList.cshtml");
            }

        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AccruedViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, string formState)
        {

            AccruedService service = new AccruedService();
            AccruedViewModel AccruedViewModel = service.InitialDetailView(recordKey, formState);

            if (AccruedViewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
                return View("~/Views/Accrued/AccruedDetail.cshtml", AccruedViewModel);
            }
            else
            {
                //return to List page
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
                return View("~/Views/Accrued/AccruedList.cshtml");
            }

        }



        #region mark
        //[HttpGet]
        //public ActionResult List()
        //{
        //    ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
        //    AccruedService service = new AccruedService();
        //    AccruedViewModel model = service.InitialListSearch();

        //    return View("~/Views/Accrued/AccruedDetail.cshtml", model);
        //}

        ////For ajax load list data
        //[AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForDisplayData, InvoiceViewModel.RoleForManageData })]
        //public string GetList(string companyCode, int monthValue, int yearValue)
        //{
        //    AccruedService service = new AccruedService();
        //    return service.GetList(companyCode, monthValue, yearValue);
        //}
        //[HttpPost]
        //[AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForManageData })]
        //public ActionResult SubmitForm(AccruedViewModel formData)
        //{
        //    AccruedService service = new AccruedService();
        //    ValidationResult result = service.SubmitForm(formData, ModelState);

        //    return Json(
        //        new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
        //        JsonRequestBehavior.AllowGet
        //    );
        //}
        // [HttpGet]
        // [AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForDisplayData, InvoiceViewModel.RoleForManageData })]
        // public ActionResult Detail()
        // {

        //     AccruedService service = new AccruedService();
        //     AccruedViewModel accruedLst = service.GetDetail(monthID, yearID, companyCode, descriptionCode, ConstantVariableService.FormStateDisplay);

        //     if (accruedLst != null)
        //     {
        //         ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
        //         return View("~/Views/Accrued/AccruedDetail.cshtml", accruedLst);
        //     }
        //     else
        //     {
        //         //return to List page
        //         return List();
        //     }

        // }
        //// [HttpGet]
        // public ActionResult Manage(int month, int year, string compCode)
        // {

        //     AccruedService service = new AccruedService();
        //     AccruedViewModel accruedLst = service.GetDetail(month, year, compCode, ConstantVariableService.FormStateCreate);

        //     if (accruedLst != null)
        //     {
        //         ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateCreate);
        //         return Json(accruedLst, JsonRequestBehavior.AllowGet);
        //         // return View("~/Views/Accrued/AccruedDetail.cshtml", accruedLst);
        //     }
        //     else
        //     {
        //         //return to List page
        //         return List();
        //     }

        // }
        #endregion
        //[HttpGet]
        public ActionResult InitialAccruedItemsList(string compCode, int month, int year, string formState,int accrued_id)
        {
            AccruedService service = new AccruedService();

            List<AccruedDetailViewModel> accruedItemViewModel = new List<AccruedDetailViewModel>();
            var result = service.InitialAccruedItemsList(compCode, month, year,formState,accrued_id);

            if (result != null)
            {
                accruedItemViewModel = result;

                //return Json(
                //    new
                //    {
                //        success = true,
                //        responseText = "",
                //        html = UtilityService.RenderPartialView(this, "~/Views/Accrued/AccruedItemsDetail.cshtml", accruedItemViewModel)
                //    },
                //        JsonRequestBehavior.AllowGet);
                return new JsonResult()
                {
                    Data = new
                    {
                        success = true,
                        responseText = "",
                        html = UtilityService.RenderPartialView(this, "~/Views/Accrued/AccruedItemsDetail.cshtml", accruedItemViewModel)
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
                        success = true,
                        responseText = "",
                        html = ""
                    },
                    JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult SubmitReportForm (int recordKey)//(AccruedViewModel formData)
        {
            AccruedService service = new AccruedService();
            //foreach (var item in formData.AccruedItemList)

            //{
            //    AccruedDetailViewModel accruedItem = new AccruedDetailViewModel();
            //    JavaScriptSerializer js = new JavaScriptSerializer();
            //    accruedItem = js.Deserialize<AccruedDetailViewModel>(item.AccruedJSON);
            //    var accruedItemSub = accruedItem.AccruedItemSubList;
            //    item.AccruedItemSubList.AddRange(accruedItemSub);
            //}
            byte[] filecontent = service.SubmitFormFileContent(recordKey);//(formData);
            if (filecontent == null)
            {
                return List();
            }
            else
            {
                return File(filecontent, ExcelExportHelper.ExcelContentType, "Accrued Report.xlsx");
            }

        }

        [HttpGet]
        public FileContentResult SubmitFormReportExport()
        {
            AccruedService service = new AccruedService();


            //DataTable dt = service.ExportReport();
            //string g = "";

            var dt = new System.Data.DataTable("test");
            dt.Columns.Add("col1", typeof(int));
            dt.Columns.Add("col2", typeof(string));

            dt.Rows.Add(1, "product 1");
            dt.Rows.Add(2, "product 2");
            dt.Rows.Add(3, "product 3");
            dt.Rows.Add(4, "product 4");
            dt.Rows.Add(5, "product 5");
            dt.Rows.Add(6, "product 6");
            dt.Rows.Add(7, "product 7");
            string[] columns = dt.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            byte[] filecontent = ExcelExportHelper.ExportExcel(dt, "Technology", false, columns);
            return File(filecontent, ExcelExportHelper.ExcelContentType, "Technologies.xlsx");
        }


    }
}