using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Controllers.Shared
{
    public class AttachmentController : Controller
    {
        // GET: Attachment
        public ActionResult Index()
        {
            return View();

        }

        /********************************************
         * Upload file
         ***********************************************/
        [HttpGet]
        public ActionResult AddAttachmentLine(string fileName, int fileSize, string documentTypeValueType, string validationType)
        {
            string message = "";
            AttachmentViewModel attachment = new AttachmentViewModel();
            ValidationResult validateResult = new ValidationResult();
           
            try
            {
                attachment.FileName = fileName;
                attachment.FileSize = fileSize;
                attachment.FileSizeMB = (attachment.FileSize / 1024f) / 1024f;
                attachment.FileExtension = Path.GetExtension(fileName).Substring(1);
                attachment.FileUniqueKey = AttachmentService.GetFileUniqueKey();
                attachment.SavedFileName = attachment.FileUniqueKey + "_" + fileName;
                attachment.DocumentTypeValueType = documentTypeValueType;

                validateResult = AttachmentService.ValidateFileAttachment(attachment, validationType);
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }
          
            if (validateResult.ErrorFlag)
            {
                return Json(new
                {
                    success = false,
                    message = validateResult.Message,
                }, "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
               
                return Json(new
                {
                    success = true,
                    message = message,
                    savedFilename = attachment.SavedFileName,
                    fileUniqueKey = attachment.FileUniqueKey,
                    Url = Url.Action("DisplayAttachmentRow", attachment)
                }, "text/html", JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DisplayAttachmentRow(AttachmentViewModel attachment)
        {

            if (!string.IsNullOrEmpty(attachment.DocumentTypeValueType))
            {
                attachment.DocumentTypeValueHelp = ValueHelpService.GetValueHelp(attachment.DocumentTypeValueType);
            }

            return PartialView("~/Views/Shared/AttachmentItem.cshtml", attachment);
        }

        public ActionResult UploadFileToTemp()
        {
            string fileName = Request.Params["savedFilename"].ToString();
            ValidationResult result = AttachmentService.UploadFileToTemp(Request);

            return Json(new
            {
                success = !result.ErrorFlag,
                responseText = result.Message,
                fileUniqueKey = result.AdditionalInfo1,
                downloadURL = result.AdditionalInfo2,
                savedFileName = fileName
            }, "text/html");
        }

        public FileContentResult DownloadFile(string downloadParam)
        {
            var attachment = AttachmentService.DownloadFile(downloadParam);

            return File(attachment.FileContent, System.Net.Mime.MediaTypeNames.Application.Octet, attachment.FileName);
        }



        /********************************************
         * Upload photo
         ***********************************************/

        [HttpGet]
        public ActionResult ValidatePhotoFile(string fileName, int fileSize)
        {
            string message = "";
            AttachmentViewModel attachment = new AttachmentViewModel();
            ValidationResult validateResult = new ValidationResult();
            try
            {
                attachment.FileName = fileName;
                attachment.FileSize = fileSize;
                attachment.FileSizeMB = (attachment.FileSize / 1024f) / 1024f;
                attachment.FileExtension = Path.GetExtension(fileName).Substring(1);
                attachment.FileUniqueKey = AttachmentService.GetFileUniqueKey();
                attachment.SavedFileName = attachment.FileUniqueKey + "_" + fileName;

                validateResult = AttachmentService.ValidateFileAttachment(attachment, "PHOTO");

            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }

            if (validateResult.ErrorFlag)
            {
                return Json(new
                {
                    success = false,
                    message = validateResult.Message,
                }, "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = message,
                    savedFilename = attachment.SavedFileName,
                    fileUniqueKey = attachment.FileUniqueKey,
                    //Url = Url.Action("DisplayAttachmentRow", attachment)
                }, "text/html", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeletePhoto(string fileName)
        {
            AttachmentService.DeleteFile(fileName, ConfigurationManager.AppSettings["TempFilePath"]);

            return Json(new
            {
                success = true
            }, "text/html", JsonRequestBehavior.AllowGet);
        }



    }
}