using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

using BBDEVSYS.ViewModels.Shared;
using System.Configuration;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels;
using BBDEVSYS.Models.Entities;
using System.Transactions;

namespace BBDEVSYS.Services.Shared
{
    public class AttachmentService
    {
        public static string GetFileUniqueKey()
        {
            int maxSize = 20;
            char[] chars = new char[62];
            string a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            chars = a.ToCharArray();

            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);

            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }

        public static ValidationResult ValidateFileAttachment(AttachmentViewModel attachment, string validationType = "")
        {
            int maxFileSize = Int32.Parse(ConfigurationManager.AppSettings["MaxFileSize"]);
            string allowedFileExtensionConfig = ConfigurationManager.AppSettings["AllowedFileExtension"];

            if (string.Equals(validationType, "PHOTO", StringComparison.OrdinalIgnoreCase))
            {
                maxFileSize = Int32.Parse(ConfigurationManager.AppSettings["PhotoMaxFileSize"]);
                allowedFileExtensionConfig = ConfigurationManager.AppSettings["PhotoAllowedFileExtension"];
            }
            else if (string.Equals(validationType, "UPLOADDATA", StringComparison.OrdinalIgnoreCase))
            {
                allowedFileExtensionConfig = ConfigurationManager.AppSettings["UploadDataAllowedFileExtension"];
            }

            ValidationResult result = new ValidationResult();

            string[] allowedFileExtension = allowedFileExtensionConfig.Split('|');

            //Check file extension
            string allowed = "";
            for (int i = 0; i < allowedFileExtension.Length; i++)
            {
                if (string.Equals(attachment.FileExtension, allowedFileExtension[i], StringComparison.OrdinalIgnoreCase))
                {
                    allowed = "X";
                    break;
                }
            }

            if (!string.Equals(allowed, "X"))
            {
                result.ErrorFlag = true;
                result.MessageType = "E";
                result.Message = ValidatorMessage.attachment_invalid_type + "<br />";
            }

            //Check file size
            if (attachment.FileSize > maxFileSize)
            {
                result.ErrorFlag = true;
                result.MessageType = "E";
                result.Message = result.Message + ValidatorMessage.attachment_file_size_exceed;
            }

            return result;
        }

        public static ValidationResult UploadFileToTemp(HttpRequestBase request)
        {
            string message = "test upload";
            ValidationResult result = new ValidationResult();

            try
            {
                foreach (string upload in request.Files)
                {
                    message = upload;

                    if (!(request.Files[upload] != null && request.Files[upload].ContentLength > 0))
                    {

                        message = "Empty";
                        continue;
                    }

                    //attachment.MimeType = request.Files[upload].ContentType;
                    Stream fileStream = request.Files[upload].InputStream;
                    byte[] fileData = new byte[request.Files[upload].ContentLength];
                    fileStream.Read(fileData, 0, request.Files[upload].ContentLength);

                    //Use physical path to store temp attachment
                    HttpPostedFileBase hpf = request.Files[upload] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;

                    string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
                    tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);
                    //string savedFileName = Path.Combine(TempFilePath, Path.GetFileName(request.Files[upload].FileName));

                    string filename = request.Params["savedFilename"].ToString(); // + "_" + request.Files[upload].FileName;
                    string savedFileName = Path.Combine(tempFilePath, filename);
                    hpf.SaveAs(savedFileName);

                    //Extract unique key
                    int index = filename.IndexOf("_");
                    if (index > 0)
                        result.AdditionalInfo1 = filename.Substring(0, index);

                    //File download URL
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    //result.AdditionalInfo2 = helper.Action("DownloadFile?fileName=" + filename, "Attachment", new { Area = "Shared" });
                    result.AdditionalInfo2 = helper.Action("DownloadFile", "Attachment", new { Area = "Shared" });
                    //result.AdditionalInfo2 = result.AdditionalInfo2 + "?fileName=" + filename;


                    //Download url
                    string downloadUrl = helper.Action("DownloadFile", "Attachment", new { Area = "Shared" });
                    string downloadParam = ConfigurationManager.AppSettings["TempFilePath"] + "/" + filename;

                    //Encoding parameter                    
                    var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(downloadParam);
                    var encodedParam = System.Convert.ToBase64String(plainTextBytes);

                    result.AdditionalInfo2 = result.AdditionalInfo2 + "?downloadParam=" + encodedParam;



                    //Delete old file from temp (incase Photo upload)
                    string filenameToDelete = request.Params["filenameToDelete"];
                    if (!string.IsNullOrEmpty(filenameToDelete))
                    {
                        AttachmentService.DeleteFile(filenameToDelete, ConfigurationManager.AppSettings["TempFilePath"]);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.ErrorFlag = true;
            }

            return result;
        }


        public static AttachmentViewModel DownloadFile(string downloadParam)
        {
            AttachmentViewModel result = new AttachmentViewModel();
            result.FileContent = new byte[0];

            try
            {
                //Decode parameter
                byte[] data = Convert.FromBase64String(downloadParam);
                string decodedParam = Encoding.Unicode.GetString(data);

                string fullPath = System.Web.HttpContext.Current.Server.MapPath(decodedParam);

                result.FileContent = System.IO.File.ReadAllBytes(@fullPath);

                result.SavedFileName = Path.GetFileName(fullPath);

                int index = result.SavedFileName.IndexOf('_');
                if (index > 0)
                {
                    result.FileName = result.SavedFileName.Substring(index + 1);
                }
                else
                {
                    result.FileName = result.SavedFileName;
                }

            }
            catch (Exception ex)
            {

            }

            return result;

        }

        //public static byte[] DownloadFile(string fileName)
        //{
        //    byte[] fileContent = new byte[0];
        //    try
        //    {
        //        string tempFilePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["TempFilePath"]);
        //        string uploadFilePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["DocumentFilePath"]);


        //        string fullPath = fullPath = Path.Combine(uploadFilePath, fileName);
        //        if (!System.IO.File.Exists(fullPath))
        //        {
        //            fullPath = Path.Combine(tempFilePath, fileName);
        //        }


        //        fileContent = System.IO.File.ReadAllBytes(@fullPath);
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return fileContent;

        //}

        public static void ManageFile(List<AttachmentViewModel> attachmentList)
        {

            string tempFilePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["TempFilePath"]);
            //string targetFilePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["DocumentFilePath"]);
            try
            {
                foreach (var attachment in attachmentList)
                {
                    string sourceFile = "";
                    string targetFile = "";
                    string documentFilePath = System.Web.HttpContext.Current.Server.MapPath(attachment.DocumentFilePath);

                    if (attachment.ID != 0) //File is in document file path
                    {
                        sourceFile = Path.Combine(documentFilePath, attachment.SavedFileName);
                    }
                    else
                    {
                        sourceFile = Path.Combine(tempFilePath, attachment.SavedFileName);
                        targetFile = Path.Combine(documentFilePath, attachment.SavedFileName);

                        //Move file
                        if (!attachment.DeleteFlag && !attachment.ErrorFlag)
                        {
                            //Check and Move file
                            try
                            {
                                //Create directory if not exist
                                System.IO.Directory.CreateDirectory(documentFilePath);

                                if (System.IO.File.Exists(sourceFile))
                                {
                                    System.IO.File.Move(sourceFile, targetFile);
                                }
                            }
                            catch (System.IO.IOException e)
                            {

                            }

                        }
                    }                    

                    if (attachment.DeleteFlag || attachment.ErrorFlag)
                    {
                        if (System.IO.File.Exists(sourceFile))
                        {
                            try
                            {
                                System.IO.File.Delete(sourceFile);
                            }
                            catch (System.IO.IOException e)
                            {

                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }


        public static void DeleteFile(string fileName, string filePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    //string tempFilePath = System.Web.HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["TempFilePath"]);

                    filePath = System.Web.HttpContext.Current.Server.MapPath(filePath);

                    string file = Path.Combine(filePath, fileName);

                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static string GetDocumentFilePath(string processCode, string folder1 = null, string folder2 = null, string folder3 = null)
        {
            string documentFilePath = "";

            try
            {
                documentFilePath = ConfigurationManager.AppSettings["DocumentFilePath"];

                //if (string.Equals(processCode, ViewModels.Agent.AgentInformationViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase) ||
                //    string.Equals(processCode, ViewModels.Agent.AgentWarehouseRNViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase) ||
                //    string.Equals(processCode, ViewModels.Agent.AgentVehicleRNViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase) ||
                //    string.Equals(processCode, ViewModels.Agent.AgentDocumentViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/Agent/" + folder1;
                //}
                //else if(string.Equals(processCode, ViewModels.Authorization.AssignUserRoleViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/Signature/";
                //}
                //else if (string.Equals(processCode, ViewModels.IOU.IOUViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/IOU/" + folder1;
                //}
                //else if (string.Equals(processCode, ViewModels.Job.JobViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/Job/" + folder1;
                //}
                //else if (string.Equals(processCode, ViewModels.Job.TradeViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/Trade/" + folder1;
                //}
                //else if (string.Equals(processCode, ViewModels.PRQ.PRQViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/PRQ/" + folder1;
                //}
                //else if (string.Equals(processCode, ViewModels.Job.JobUploadViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/JobUpload/";
                //}
                //else if (string.Equals(processCode, ViewModels.PR.PRViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/PR/" + folder1;
                //}
                //else if (string.Equals(processCode, ViewModels.Material.MaterialStockUploadViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/MaterialStockUpload/";
                //}
                //else if (string.Equals(processCode, ViewModels.Consumable.ConsumableUploadViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/ConsumableUpload/";
                //}
                //else if (string.Equals(processCode, ViewModels.BudgetPlan.BudgetPlanUploadViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/BudgetPlanUpload/";
                //}
                //else if (string.Equals(processCode, ViewModels.PRQ.PRQUploadViewModel.ProcessCode, StringComparison.OrdinalIgnoreCase))
                //{
                //    documentFilePath = documentFilePath + "/PRQUploadProcess/";
                //}

            }
            catch (Exception ex)
            {

            }

            return documentFilePath;
        }

        public static void ManageAttachment(List<AttachmentViewModel> attachmentList)
        {
            try
            {
                //User userInfo = UserService.GetSessionUserInfo();

                using (TransactionScope scope = new TransactionScope())
                {
                    using (var context = new PYMFEEEntities())
                    {
                        //Save attachment                        
                        foreach (var attachmentViewModel in attachmentList)
                        {
                            var attachment = new Attachment();

                            MVMMappingService.MoveData(attachmentViewModel, attachment);

                            if (attachmentViewModel.DeleteFlag)
                            {
                                if (attachment.ID != 0)
                                {
                                    context.Entry(attachment).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                            else
                            {
                                if (attachment.ID != 0)
                                {
                                    context.Entry(attachment).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    attachment.AttachDate = DateTime.Now;
                                    attachment.AttachBy = "";
                                    context.Attachments.Add(attachment);
                                }
                            }

                            context.SaveChanges();
                        }
                    }

                    scope.Complete();
                }

                //Move file
                AttachmentService.ManageFile(attachmentList);
            }
            catch (Exception ex)
            {

            }
        }

        public static List<AttachmentViewModel> GetAttachmentList(AttachmentViewModel option)
        {
            List<AttachmentViewModel> attachmentViewModelList = new List<AttachmentViewModel>();

            try
            {
                var documentTypeValueHelp = new List<ValueHelpViewModel>();

                if (!string.IsNullOrEmpty(option.DocumentTypeValueType))
                {
                    documentTypeValueHelp = ValueHelpService.GetValueHelp(option.DocumentTypeValueType);
                }

                using (var context = new PYMFEEEntities())
                {
                    List<Attachment> attachmentList = new List<Attachment>();

                    //if(option.DataID != 0)
                    //{
                    //    attachmentList = (from m in context.Attachments where m.ProcessCode == option.ProcessCode && m.DataID == option.DataID select m).OrderBy(m => m.ID).ToList();
                    //}

                    if (!string.IsNullOrEmpty(option.DataKey))
                    {
                        attachmentList = (from m in context.Attachments where m.ProcessCode == option.ProcessCode && m.DataKey == option.DataKey select m).OrderBy(m => m.ID).ToList();
                    }

                    foreach(var attachment in attachmentList)
                    {
                        if (!string.IsNullOrEmpty(option.AttachmentGroup))
                        {
                            if(!string.Equals(attachment.AttachmentGroup, option.AttachmentGroup, StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                        }

                        AttachmentViewModel attachmentViewModel = new AttachmentViewModel();

                        MVMMappingService.MoveData(attachment, attachmentViewModel);

                        //Download url
                        UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        string downloadUrl = helper.Action("DownloadFile", "Attachment", new { Area = "Shared" });
                        string downloadParam = option.DocumentFilePath + "/" + attachment.SavedFileName;

                        //Encoding parameter
                        var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(downloadParam);
                        var encodedParam = System.Convert.ToBase64String(plainTextBytes);

                        downloadUrl = downloadUrl + "?downloadParam=" + encodedParam;
                        
                        attachmentViewModel.DownloadURL = downloadUrl;
                        attachmentViewModel.DocumentTypeValueHelp = documentTypeValueHelp;

                        attachmentViewModelList.Add(attachmentViewModel);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return attachmentViewModelList;
        }

        //public static ValidationResult ValidateUploadPhoto(AttachmentViewModel attachment)
        //{
        //    int maxFileSize = Int32.Parse(ConfigurationManager.AppSettings["PhotoMaxFileSize"]);
        //    string allowedFileExtensionConfig = ConfigurationManager.AppSettings["PhotoAllowedFileExtension"];
        //    ValidationResult result = new ValidationResult();

        //    string[] allowedFileExtension = allowedFileExtensionConfig.Split('|');

        //    //Check file extension
        //    string allowed = "";
        //    for (int i = 0; i < allowedFileExtension.Length; i++)
        //    {
        //        if (string.Equals(attachment.FileExtension, allowedFileExtension[i], StringComparison.OrdinalIgnoreCase))
        //        {
        //            allowed = "X";
        //            break;
        //        }
        //    }

        //    if (!string.Equals(allowed, "X"))
        //    {
        //        result.ErrorFlag = true;
        //        result.MessageType = "E";
        //        result.Message = ValidatorMessage.attachment_invalid_type + "<br />";
        //    }

        //    //Check file size
        //    if (attachment.FileSize > maxFileSize)
        //    {
        //        result.ErrorFlag = true;
        //        result.MessageType = "E";
        //        result.Message = result.Message + ValidatorMessage.attachment_file_size_exceed;
        //    }

        //    return result;
        //}

    }
}