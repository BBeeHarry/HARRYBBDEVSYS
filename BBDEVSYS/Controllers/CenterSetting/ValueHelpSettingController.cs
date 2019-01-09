using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.CenterSetting;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.CenterSetting;
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.CenterSetting
{
    public class ValueHelpSettingController : Controller
    {
        // GET: ValueHelpSetting
        [AuthorizeService(AllowRoleList = new[] { ValueHelpSettingViewModel.RoleForDisplayData, ValueHelpSettingViewModel.RoleForManageData })]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            ViewBag.DisplayOnly = AuthorizeService.CheckDisplayOnly(new[] { ValueHelpSettingViewModel.RoleForManageData });
            return View("~/Views/CenterSetting/ValueHelpSetting/ValueHelpSettingList.cshtml");
        }
        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { ValueHelpSettingViewModel.RoleForDisplayData, ValueHelpSettingViewModel.RoleForManageData })]
        public string GetList()
        {
            ValueHelpSettingService service = new ValueHelpSettingService();
            return service.GetList();
        }

        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { ValueHelpSettingViewModel.RoleForDisplayData, ValueHelpSettingViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey)
        {
            ValueHelpSettingService service = new ValueHelpSettingService();
            ValueHelpSettingViewModel valueHelpModel = service.InitialDetailView(recordKey, ConstantVariableService.FormStateDisplay);
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
            return View("~/Views/CenterSetting/ValueHelpSetting/ValueHelpSettingDetail.cshtml", valueHelpModel);
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { ValueHelpSettingViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, string formState)
        {
            ValueHelpSettingService service = new ValueHelpSettingService();
            ValueHelpSettingViewModel valueHelpModel = service.InitialDetailView(recordKey, formState);
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
            return View("~/Views/CenterSetting/ValueHelpSetting/ValueHelpSettingDetail.cshtml", valueHelpModel);
        }
        [HttpPost]
        [AuthorizeService(AllowRoleList = new[] { ValueHelpSettingViewModel.RoleForManageData })]
        public ActionResult SubmitForm(ValueHelpSettingViewModel formData)
        {
            ValueHelpSettingService service = new ValueHelpSettingService();
            ValidationResult resul = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !resul.ErrorFlag, responseText = resul.Message, errorList = resul.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
                );
        }
        public PartialViewResult AddItem()
        {
            ValueHelpSettingService service = new ValueHelpSettingService();

            ValueHelpSettingItemViewModel valueHelpItemViewModel = service.InitialItem();

            return PartialView("~/Views/CenterSetting/ValueHelpSetting/ValueHelpSettingItem.cshtml", valueHelpItemViewModel);

        }
    }
}