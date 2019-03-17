using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Adjustrefund
{
    public class AdjustRefundUploadController : Controller
    {
        // GET: AdjustRefundUpload
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdjustRefundUpload/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdjustRefundUpload/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdjustRefundUpload/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdjustRefundUpload/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdjustRefundUpload/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AdjustRefundUpload/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdjustRefundUpload/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
