using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BBDEVSYS.Services.Authorization;
using BBDEVSYS.ViewModels.Authorization;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Shared;

namespace BBDEVSYS.Controllers.Authorization
{
    public class AppCompositeRoleController : Controller
    {
        [AuthorizeService(AllowRoleList = new[] { AppCompositeRoleViewModel.RoleForDisplayData, AppCompositeRoleViewModel.RoleForManageData })]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            ViewBag.DisplayOnly = AuthorizeService.CheckDisplayOnly(new[] { AppCompositeRoleViewModel.RoleForManageData });
            ViewBag.user = User.Identity.Name;
            return View("~/Views/Authorization/AppCompositeRole/AppCompositeRoleList.cshtml");
        }

        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { AppCompositeRoleViewModel.RoleForDisplayData, AppCompositeRoleViewModel.RoleForManageData })]
        public string GetList()
        {
            AppCompositeRoleService service = new AppCompositeRoleService();
            return service.GetList();
        }

        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AppCompositeRoleViewModel.RoleForDisplayData, AppCompositeRoleViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey)
        {

            AppCompositeRoleService service = new AppCompositeRoleService();
            AppCompositeRoleViewModel indicatorViewModel = service.InitialDetailView(recordKey, ConstantVariableService.FormStateDisplay);

            if (indicatorViewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
                return View("~/Views/Authorization/AppCompositeRole/AppCompositeRoleDetail.cshtml", indicatorViewModel);
            }
            else
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
                //return to List page
                return View("~/Views/Authorization/AppCompositeRole/AppCompositeRoleList.cshtml");
            }

        }

        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AppCompositeRoleViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, string formState)
        {

            AppCompositeRoleService service = new AppCompositeRoleService();
            AppCompositeRoleViewModel indicatorViewModel = service.InitialDetailView(recordKey, formState);

            if (indicatorViewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
                return View("~/Views/Authorization/AppCompositeRole/AppCompositeRoleDetail.cshtml", indicatorViewModel);
            }
            else
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
                //return to List page
                return View("~/Views/Authorization/AppCompositeRole/AppCompositeRoleList.cshtml");
            }

        }

        [HttpPost]
        [AuthorizeService(AllowRoleList = new[] { AppCompositeRoleViewModel.RoleForManageData })]
        public ActionResult SubmitForm(AppCompositeRoleViewModel formData)
        {
            AppCompositeRoleService service = new AppCompositeRoleService();
            ValidationResult result = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}