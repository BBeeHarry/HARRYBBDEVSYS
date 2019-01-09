using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Invoice;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.InvoiceUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Invoice
{
    public class InvoiceUploadController : Controller
    {
        // GET: InvoiceUpload
        [AuthorizeService(AllowRoleList = new[] { InvoiceUploadViewModel.RoleForDisplayData, InvoiceUploadViewModel.RoleForManageData })]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            ViewBag.DisplayOnly = AuthorizeService.CheckDisplayOnly(new[] { InvoiceUploadViewModel.RoleForManageData });
            return View("~/Views/InvoiceUpload/InvoiceUploadList.cshtml");
        }
        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { InvoiceUploadViewModel.RoleForDisplayData, InvoiceUploadViewModel.RoleForManageData })]
        public string GetList()
        {
            InvoiceUploadService service = new InvoiceUploadService();
            return service.GetList();
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { InvoiceUploadViewModel.RoleForDisplayData, InvoiceUploadViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey)
        {
            InvoiceUploadService service = new InvoiceUploadService();
            InvoiceUploadViewModel model = service.InitialDetailView(recordKey, ConstantVariableService.FormStateDisplay);
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
            return View("~/Views/InvoiceUpload/InvoiceUploadDetail.cshtml", model);
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { InvoiceUploadViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, string formState)
      {
            InvoiceUploadService service = new InvoiceUploadService();
            InvoiceUploadViewModel model = service.InitialDetailView(recordKey, formState);
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
            return View("~/Views/InvoiceUpload/InvoiceUploadDetail.cshtml", model);
        }

        // [AuthorizeService(AllowRoleList = new[] { InvoiceUploadViewModel.RoleForManageData })]
        [HttpPost]
        [AuthorizeService(AllowRoleList = new[] { InvoiceUploadViewModel.RoleForManageData })]
        public ActionResult SubmitForm(InvoiceUploadViewModel formData)
        {
            InvoiceUploadService service = new InvoiceUploadService();
            ValidationResult resul = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !resul.ErrorFlag, responseText = resul.Message, errorList = resul.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
                );
        }



        public ActionResult ImportExcel(string fileName)
        {
            List<InvoiceUploadItemViewModel> uploadInvoiceItemViewModel = new List<InvoiceUploadItemViewModel>();
            InvoiceUploadService service = new InvoiceUploadService();
            var result = service.ImportExcel(fileName);

            if (!result.ErrorFlag)
            {
                uploadInvoiceItemViewModel = result.ReturnResult;
                

                return new JsonResult()
                {
                    Data = new
                    {
                        success = !result.ErrorFlag,
                        responseText = result.Message,
                        html = UtilityService.RenderPartialView(this, "~/Views/InvoiceUpload/InvoiceUploadItems.cshtml", uploadInvoiceItemViewModel)
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
        //public PartialViewResult GetUploadItemHeader(int month = 0,int year =0)
        //{
        //    InvoiceUploadService service = new InvoiceUploadService();
        //    var columnList = service.GetUploadItemHeader(month, year);
        //    return PartialView("~/Views/InvoiceUpload/InvoiceUploadItemHeader.cshtml", columnList);

        //}
        //public ActionResult GetWarehouse(string matType)
        //{
        //    InvoiceUploadService service = new InvoiceUploadService();
        //    InvoiceUploadViewModel model = new InvoiceUploadViewModel();
        //    model = service.InitialWarehouse(matType);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}
    }
}