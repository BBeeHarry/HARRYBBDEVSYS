using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BBDEVSYS.ViewModels.Error;

namespace SPSTNew.Controllers.Error
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Message()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
        
    }
}