using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BBDEVSYS.ViewModels.Authentication;
using BBDEVSYS.Services.Authentication;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Models.Entities;

namespace BBDEVSYS.Controllers.Authentication
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication
        [AllowAnonymous]
        public ActionResult Index()
        {
            AuthenticationViewModel model = new AuthenticationViewModel();

            return View("~/Views/Authentication/Login.cshtml", model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AuthenticationViewModel authen)
        {
            AuthenticationService service = new AuthenticationService();
            ValidationResult result = service.Login(authen, Response);

            //if (result.ErrorFlag)
            //{
            //    return Json(
            //        new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
            //        JsonRequestBehavior.AllowGet
            //    );
            //}
            //else
            //{
            //    PYMFEEEntities context = new PYMFEEEntities();
            //    var val = context.ValidateUser(authen.Username, authen.Password).FirstOrDefault();
            //    switch (val)
            //    {
            //        case "1":
            //            return View(authen);

            //        case "2":
            //            return View(authen);

            //        default:
            //            return RedirectToAction("List", "Invoice");

            //    }
            //    //return RedirectToAction("List", "Invoice");
            //    //return RedirectToAction("Index", "Inbox", new { Area = "Inbox" });
            //}

            return Json(
                    new { success = !result.ErrorFlag, responseText = result.Message, errorList = result.ModelStateErrorList },
                    JsonRequestBehavior.AllowGet
                );

        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            AuthenticationViewModel model = new AuthenticationViewModel();

            AuthenticationService service = new AuthenticationService();
            ValidationResult result = service.Logout(Response);
            //return Redirect("http://together/web/staff/home");
            return View("~/Views/Authentication/Login.cshtml", model);
        }

        [AllowAnonymous]
        public ActionResult Default()
        {
            return RedirectToRoute("URLWithoutParam", new { area = "Authentication", controller = "Authentication", action = "Index" });
        }

    }
}