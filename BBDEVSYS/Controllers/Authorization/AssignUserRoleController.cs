using BBDEVSYS.Services.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BBDEVSYS.ViewModels.Authorization;
using BBDEVSYS.Services.Authorization;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.ViewModels.Shared;

namespace BBDEVSYS.Controllers.Authorization
{
    public class AssignUserRoleController : Controller
    {
        // GET: AssignRole
        [AuthorizeService(AllowRoleList = new[] { AssignUserRoleViewModel.RoleForDisplayData, AssignUserRoleViewModel.RoleForManageData })]
        public ActionResult List()
        {
            ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateList);
            AssignUserRoleService service = new AssignUserRoleService();
            AssignUserRoleViewModel model = service.InitialListSearch();

            ViewBag.DisplayOnly = AuthorizeService.CheckDisplayOnly(new[] { AssignUserRoleViewModel.RoleForManageData });

            return View("~/Views/Authorization/AssignUserRole/AssignUserRoleList.cshtml", model);
        }
        //For ajax load list data
        [AuthorizeService(AllowRoleList = new[] { AssignUserRoleViewModel.RoleForDisplayData, AssignUserRoleViewModel.RoleForManageData })]
        public string GetList()//string assignUserType)
        {
            AssignUserRoleService service = new AssignUserRoleService();
            return service.GetList();//assignUserType);
        }
        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AssignUserRoleViewModel.RoleForDisplayData, AssignUserRoleViewModel.RoleForManageData })]
        public ActionResult Detail(int recordKey, string assignUserRoleCode, string assignUserType)
        {

            AssignUserRoleService service = new AssignUserRoleService();
            AssignUserRoleViewModel assignUserViewModel = service.GetDetail(recordKey, ConstantVariableService.FormStateDisplay, assignUserRoleCode, assignUserType);
            if (assignUserViewModel != null)
            {
                //ViewBag.Title = UtilityService.GetPagetTitlePrefix(ConstantVariableService.FormStateDisplay);
                return View("~/Views/Authorization/AssignUserRole/AssignUserRoleDetail.cshtml", assignUserViewModel);
            }
            else
            { 
                //return to List page
                return List();
            }

        }

        [HttpGet]
        [AuthorizeService(AllowRoleList = new[] { AssignUserRoleViewModel.RoleForManageData })]
        public ActionResult Manage(int recordKey, string formState, string assignUserRoleCode, string assignUserType)
        {
            
                AssignUserRoleService service = new AssignUserRoleService();
                AssignUserRoleViewModel assignUserViewModel = service.GetDetail(recordKey, formState, assignUserRoleCode, assignUserType);
            if (assignUserViewModel != null)
            {
                ViewBag.Title = UtilityService.GetPagetTitlePrefix(formState);
                return View("~/Views/Authorization/AssignUserRole/AssignUserRoleDetail.cshtml", assignUserViewModel);
            }
            else
            {
                //return to List page
                return List();
            }

        }

        [HttpPost]
        [AuthorizeService(AllowRoleList = new[] { AssignUserRoleViewModel.RoleForManageData })]
        public ActionResult SubmitForm(AssignUserRoleViewModel formData)
        {
            AssignUserRoleService service = new AssignUserRoleService();
            ValidationResult result = service.SubmitForm(formData, ModelState);

            return Json(
                new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
                JsonRequestBehavior.AllowGet
            );
        }
        public PartialViewResult AddItem()
        {
            AssignUserRoleService service = new AssignUserRoleService();
            AssignUserRoleItemViewModel assignUserViewModel = service.InitialItem();

            return PartialView("~/Views/Authorization/AssignUserRole/AssignUserRoleItem.cshtml", assignUserViewModel);

        }
        public ActionResult GetValueHelpStatus(int roleID)
        {
            AssignUserRoleService service = new AssignUserRoleService();
            AssignUserRoleItemViewModel model = new AssignUserRoleItemViewModel();
            model.StatusText = service.InitialGetRoleStatus(roleID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}