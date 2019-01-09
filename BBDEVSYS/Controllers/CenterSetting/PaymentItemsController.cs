using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.CenterSetting.PaymentItems;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.CenterSetting.PaymentItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.CenterSetting
{
    public class PaymentItemsController : Controller
    {
        // GET: PaymentItems

        [AuthorizeService(AllowRoleList = new[] { PaymentItemsViewModel.RoleForDisplayData, PaymentItemsViewModel.RoleForManageData })]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            PaymentItemsService service = new PaymentItemsService();
            PaymentItemsViewModel model = service.InitialListSearch();

            return View("~/Views/CenterSetting/PaymentItems/PaymentItemsList.cshtml", model);
        }

        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { PaymentItemsViewModel.RoleForDisplayData, PaymentItemsViewModel.RoleForManageData })]
        public string GetList(string companyCode)
        {
            PaymentItemsService service = new PaymentItemsService();
            return service.GetList(companyCode);
        }

        [AuthorizeService(AllowRoleList = new[] { PaymentItemsViewModel.RoleForManageData })]
        [HttpPost]
        public ActionResult SubmitForm(PaymentItemsViewModel formData)
        {
            PaymentItemsService service = new PaymentItemsService();
            ValidationResult result = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { PaymentItemsViewModel.RoleForDisplayData, PaymentItemsViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey)
        {

            PaymentItemsService service = new PaymentItemsService();
            PaymentItemsViewModel pymItemsiewModel = service.InitialDetailView(recordKey, ConstantVariableService.FormStateDisplay);

            if (pymItemsiewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
                return View("~/Views/CenterSetting/PaymentItems/PaymentItemsDetail.cshtml", pymItemsiewModel);
            }
            else
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
                //return to List page
                return View("~/Views/CenterSetting/PaymentItems/PaymentItemsList.cshtml");
            }

        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { PaymentItemsViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, string formState)
        {

            PaymentItemsService service = new PaymentItemsService();
            PaymentItemsViewModel AccruedViewModel = service.InitialDetailView(recordKey, formState);

            if (AccruedViewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
                return View("~/Views/CenterSetting/PaymentItems/PaymentItemsDetail.cshtml", AccruedViewModel);
            }
            else
            {
                //return to List page
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
                return View("~/Views/CenterSetting/PaymentItems/PaymentItemsList.cshtml");
            }

        }
        public PartialViewResult AddItem(int sequence)//,int pymID, string companyCode)
        {
            PaymentItemsService service = new PaymentItemsService();
            PaymentItemsChargeViewModel invoiceItemModel = service.InitialItem(sequence);//, pymID, companyCode);

            return PartialView("~/Views/CenterSetting/PaymentItems/PaymentItemsDetailItem.cshtml", invoiceItemModel);

        }
    }
}