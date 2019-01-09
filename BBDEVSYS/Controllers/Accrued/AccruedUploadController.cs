using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Accrued;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.AccruedUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Accrued
{
    public class AccruedUploadController : Controller
    {
        // GET: AccruedUpload
     
       [AuthorizeService(AllowRoleList = new[] { AccruedUploadViewModel.RoleForDisplayData, AccruedUploadViewModel.RoleForManageData })]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            ViewBag.DisplayOnly = AuthorizeService.CheckDisplayOnly(new[] { AccruedUploadViewModel.RoleForManageData });
            return View("~/Views/AccruedUpload/AccruedUploadList.cshtml");
        }
        //For ajax load list data
       [AuthorizeService(AllowRoleList = new[] { AccruedUploadViewModel.RoleForDisplayData, AccruedUploadViewModel.RoleForManageData })]

        public string GetList()
        {
            AccruedUploadService service = new AccruedUploadService();
            return service.GetList();
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AccruedUploadViewModel.RoleForDisplayData, AccruedUploadViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey)
        {
            AccruedUploadService service = new AccruedUploadService();
            AccruedUploadViewModel model = service.InitialDetailView(recordKey, ConstantVariableService.FormStateDisplay);
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
            return View("~/Views/Material/AccruedUpload/AccruedUploadDetail.cshtml", model);
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AccruedUploadViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, string formState)
        {
            AccruedUploadService service = new AccruedUploadService();
            AccruedUploadViewModel model = service.InitialDetailView(recordKey, formState);
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
            return View("~/Views/Material/AccruedUpload/AccruedUploadDetail.cshtml", model);
        }
        [HttpPost]
       [AuthorizeService(AllowRoleList = new[] { AccruedUploadViewModel.RoleForManageData })]
        public ActionResult SubmitForm(AccruedUploadViewModel formData)
        {
            AccruedUploadService service = new AccruedUploadService();
            ValidationResult resul = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !resul.ErrorFlag, responseText = resul.Message, errorList = resul.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
                );
        }



        public ActionResult ImportExcel(string fileName, string uploadType, string warehouseCode, string matStockType)
        {
            List<AccruedUploadItemViewModel> uploadaccruedItemViewModel = new List<AccruedUploadItemViewModel>();
            AccruedUploadService service = new AccruedUploadService();
            var result = service.ImportExcel(fileName, uploadType, warehouseCode, matStockType);

            if (!result.ErrorFlag)
            {
                uploadaccruedItemViewModel = result.ReturnResult;

                //return Json(
                //    new
                //    {
                //        success = !result.ErrorFlag,
                //        responseText = result.Message,
                //        html = UtilityService.RenderPartialView(this, "~/Views/Material/AccruedUpload/AccruedUploadItems.cshtml", uploadStockItemViewModel)
                //    },
                //        JsonRequestBehavior.AllowGet);

                return new JsonResult()
                {
                    Data = new
                    {
                        success = !result.ErrorFlag,
                        responseText = result.Message,
                        html = UtilityService.RenderPartialView(this, "~/Views/AccruedUpload/AccruedUploadItems.cshtml", uploadaccruedItemViewModel)
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

        //public ActionResult GetWarehouse(string matType)
        //{
        //    AccruedUploadService service = new AccruedUploadService();
        //    AccruedUploadViewModel model = new AccruedUploadViewModel();
        //    model = service.InitialWarehouse(matType);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}
    }
}
