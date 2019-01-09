using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Invoice;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Invoice
{
    public class InvoiceController : Controller
    {
        // GET: Invoice

        [AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForDisplayData, InvoiceViewModel.RoleForManageData })]
        //[Authorize(Roles = "Admin")]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            InvoiceService service = new InvoiceService();
            InvoiceViewModel model = service.InitialListSearch();

            return View("~/Views/Invoice/InvoiceList.cshtml", model);
        }

        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForDisplayData, InvoiceViewModel.RoleForManageData })]
        public string GetList(string companyCode,int monthSValue,int monthEValue,int yearSValue,int yearEValue,string pymName ,string status)
        {
            InvoiceService service = new InvoiceService();
            return service.GetList(companyCode , monthSValue, monthEValue, yearSValue, yearEValue,pymName,status);
        }
        [HttpPost]
        [AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForManageData })]     
        public ActionResult SubmitForm(InvoiceViewModel formData)
        {
            InvoiceService service = new InvoiceService();
            ValidationResult result = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForDisplayData, InvoiceViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey, int monthValue, int yearValue, string companyCode, string paymentItemCode, string formState)
        {

            InvoiceService service = new InvoiceService();
          
            InvoiceViewModel invoiceLst = service.GetDetail(recordKey,monthValue, yearValue, companyCode, paymentItemCode, ConstantVariableService.FormStateDisplay);
            invoiceLst.FormAction = formState;
            invoiceLst.FormState = formState;
            if (invoiceLst != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
                return View("~/Views/Invoice/InvoiceDetail.cshtml", invoiceLst);
            }
            else
            {
                //return to List page
                return List();
            }

        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { InvoiceViewModel.RoleForDisplayData, InvoiceViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, int monthValue, int yearValue, string companyCode, string paymentItemCode, string formState)
        {
            InvoiceService service = new InvoiceService();
            InvoiceViewModel invoiceLst = service.GetDetail(recordKey, monthValue, yearValue, companyCode, paymentItemCode, formState);



            if (invoiceLst != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
                return View("~/Views/Invoice/InvoiceDetail.cshtml", invoiceLst);
            }
            else
            {
                //return to List page
                return List();
            }


        }

        // [HttpPost]
        public ActionResult ManageClose(int recordKey, int monthValue, int yearValue, string companyCode, string paymentItemCode)//, string paymentitemsCode, string formState)
        {//(int recordKey, string formState)//

            InvoiceService service = new InvoiceService();
            InvoiceViewModel invoiceLst = service.GetDetail(recordKey, monthValue, yearValue, companyCode, paymentItemCode, ConstantVariableService.FormActionClosed);
            
            if (invoiceLst != null)
            {
                ValidationResult result = service.SaveClose(invoiceLst, ModelState);

                return
                    Json(
                    new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
                    JsonRequestBehavior.AllowGet
               
           );
            }
            else
            {
                //return to List page
                return List();
            }

        }

        public PartialViewResult AddItem(int sequence, string companyCode, string pymItems)
        {
            InvoiceService service = new InvoiceService();
            InvoiceDetailViewModel invoiceItemModel = service.InitialItem(sequence,companyCode,pymItems);

            return PartialView("~/Views/Invoice/InvoiceItemDetail.cshtml", invoiceItemModel);

        }
        public ActionResult GetPaymentItem(string companyCode) {
            InvoiceService service = new InvoiceService();
            InvoiceViewModel model = service.GetPaymentItemsList(companyCode); ;
            return Json(model,JsonRequestBehavior.AllowGet);
        }

       
    }
}