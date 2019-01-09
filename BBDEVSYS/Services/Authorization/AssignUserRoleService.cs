using BBDEVSYS.Services.Abstract;
using BBDEVSYS.ViewModels.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.ViewModels.Shared;
using BBDEVSYS.Models.Shared;
using System.Web.Mvc;
using System.Transactions;
using BBDEVSYS.Content.text;
using System.IO;
using System.Configuration;

namespace BBDEVSYS.Services.Authorization
{
    public class AssignUserRoleService : AbstractControllerService<AssignUserRoleViewModel>
    {
        public override string GetList()
        {
            string dataList = "";
            List<AssignUserRoleViewModel> assignUserRoleList = GetAssignUserRoleList();

            dataList = DatatablesService.ConvertObjectListToDatatables<AssignUserRoleViewModel>(assignUserRoleList);

            return dataList;
        }
        public string GetList(string _userRole)
        {
            string dataList = "";
            //List<AssignUserRoleViewModel> assignUserRoleList = GetAssignUserRoleList(_userRole);

            //dataList = DatatablesService.ConvertObjectListToDatatables<AssignUserRoleViewModel>(assignUserRoleList);

            return dataList;
        }

        public List<AssignUserRoleViewModel> GetAssignUserRoleList(string _userRoleCode="")//(string userRole, string userRoleCode = "")
        {
            List<AssignUserRoleViewModel> assignUserRoleList = new List<AssignUserRoleViewModel>();
            ValueHelpViewModel valueHelp = new ValueHelpViewModel();

            try
            {

                using (var context = new PYMFEEEntities())
                {

                    var userRol = string.IsNullOrEmpty(_userRoleCode)? 
                        (from user in context.UserRoles select user).ToList() : (from user in context.UserRoles where user.USERID == _userRoleCode select user).ToList(); ;

                    var assignUserRoleEmp = string.IsNullOrEmpty(_userRoleCode) ? 
                        (from emp in context.USERS select emp).ToList() : (from emp in context.USERS where emp.USERID == _userRoleCode select emp).ToList();
                    foreach (var item in assignUserRoleEmp)
                        {
                            string _getIcon = Convert.ToString(userRol.Where(m => m.USERID == item.USERID).FirstOrDefault());
                            var assign = new AssignUserRoleViewModel();
                            string icon = "";
                            if (!string.IsNullOrEmpty(item.USERID) && !string.IsNullOrEmpty(_getIcon))
                            {
                                icon = "<i class=\"fa fa-check-circle\" style=\"color: #4277f4; font-size: 20px\"></i>";
                            }
                            assign.AssignUserCode = item.USERID;
                            assign.AssignUserName = item.USERNAME;
                            assign.UserName = item.USERNAME;
                            assign.AssignUserRoleIcon = icon;

                            assign.AssignAvailableReason = ValidatorMessage.cannot_assignuserrole_action;
                            assignUserRoleList.Add(assign);
                        }

                    }
            }
            catch (Exception ex)
            {

            }
            return assignUserRoleList;
        }
        public AssignUserRoleViewModel InitialListSearch()
        {
            try
            {
                AssignUserRoleViewModel assignUserRoleViewModel = new AssignUserRoleViewModel();

                //assignUserRoleViewModel.AssignUserTypeList = ValueHelpService.GetValueHelp(ConstantVariableService.UserTypeValueHelp);

                return assignUserRoleViewModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public override AssignUserRoleViewModel GetDetail(int id)
        {
            throw new NotImplementedException();
        }

        public override AssignUserRoleViewModel NewFormData()
        {
            AssignUserRoleViewModel assignUserRole = new AssignUserRoleViewModel();
            try
            {

                assignUserRole.Photo.PreviewNoPhoto = VirtualPathUtility.ToAbsolute(ConfigurationManager.AppSettings["PreviewNoPhoto"]);
                assignUserRole.Photo.PreviewPhoto = assignUserRole.Photo.PreviewNoPhoto;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return assignUserRole;

        }

        public override ValidationResult ValidateFormData(AssignUserRoleViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            var _context = new PYMFEEEntities();
            try
            {
                var DeleteflagList = formData.AssignUserRoleItemList.Where(m => m.DeleteFlag == true).ToList();

                if (!modelState.IsValid)
                {
                    result.ModelStateErrorList = UtilityService.GetModelStateErrors(modelState);
                    result.ErrorFlag = true;
                }
                else
                {
                    var assignUserRoleText = ResourceText.AssignUserRole;

                    //Get item => deleteFlag != true
                    var itemList = formData.AssignUserRoleItemList.Where(m => m.DeleteFlag != true).ToList();

                    if (itemList == null || !itemList.Any()) //Check list is null or empty
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_listnull_error, assignUserRoleText)));

                        result.ErrorFlag = true;
                    }
                    else
                    {

                        //Check item
                        int line = 1;
                        foreach (var item in itemList)
                        {
                            //targetProduct/Group id not empty

                            if (item.CompositeRoleID == null)
                            {
                                string _itemEmpty = ResourceText.AssignUserRole;

                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, _itemEmpty, line.ToString())));

                                result.ErrorFlag = true;
                            }
                            if (item.CompositeRoleID != null)
                            {
                                var dupUserRoleList = itemList.GroupBy(m => m.CompositeRoleID
                                 ).Where(m => m.Count() > 1).ToList();
                                foreach (var itemDup in dupUserRoleList)
                                {
                                    if (itemDup.Key == item.CompositeRoleID)
                                    {
                                        var compositeName = _context.AppCompositeRoles.Where(m => m.ID == item.CompositeRoleID).FirstOrDefault().Name;

                                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.items_line_duplicate_error, ResourceText.ACRName, compositeName, line.ToString())));
                                        result.ErrorFlag = true;
                                    }
                                }
                            }

                            line++;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                result.MessageType = ex.ToString();
                result.Message = ex.ToString();
            }

            return result;
        }

        public AssignUserRoleViewModel GetDetail(int recordKey, string formState, string assignUserRoleCode, string assignUserType)
        {
            AssignUserRoleViewModel assignUserRole = NewFormData();
            assignUserRole.FormState = formState;
            assignUserRole.FormAction = formState;
            var userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var assignUserRoleList = GetAssignUserRoleList( assignUserRoleCode).FirstOrDefault();

                    assignUserRole.AssignUserCode = assignUserRoleList.AssignUserCode;
                    assignUserRole.AssignUserName = assignUserRoleList.AssignUserName;
                    assignUserRole.UserName = assignUserRoleList.UserName;
                    assignUserRole.AssignUserType = assignUserType;
                    assignUserRole.CREATE_BY = userInfo.UserCode;
                    assignUserRole.CREATE_DATE = DateTime.Now;


                    //Get line item
                    List<UserRole> assignUserRoleItemList = (from m in context.UserRoles
                                                             where m.USERID == assignUserRoleCode
                                                             select m).OrderBy(m => m.CompositeRoleID).ToList();
                    //var appComposite = context.AppCompositeRoles.Where(m => m.Status == ConstantVariableService.ConfigStatusActive).OrderBy(m => m.Name).ToList();
                    var appComposite = context.AppCompositeRoles.OrderBy(m => m.Name).ToList();
       
                    foreach (var item in assignUserRoleItemList)
                    {
                        AssignUserRoleItemViewModel assignUserRoleItem = new AssignUserRoleItemViewModel();
                        MVMMappingService.MoveData(item, assignUserRoleItem);
                        assignUserRoleItem.CompositeRoleID = item.CompositeRoleID;
                        assignUserRoleItem.CompositeRoleList = appComposite;
                        assignUserRoleItem.StatusText = ValueHelpService.GetValueHelpText("CONFIGSTATUS", appComposite.SingleOrDefault(m=>m.ID == item.CompositeRoleID).Status ).ValueText;
                        assignUserRole.AssignUserRoleItemList.Add(assignUserRoleItem);
                        //Get line item

                    }
                    ////Setting Signature
                    //var UserSignal = context.UserSignatures.Where(m => m.Username == assignUserRole.UserName).FirstOrDefault();
                    ////Photo
                    //if (UserSignal != null )
                    //{
                    //    string documentPath = AttachmentService.GetDocumentFilePath(AssignUserRoleViewModel.ProcessCode);

                    //    //Check file is existing
                    //    string filePath = System.Web.HttpContext.Current.Server.MapPath(documentPath);

                    //    string file = Path.Combine(filePath, UserSignal.SignatureFileName);

                    //    if (System.IO.File.Exists(file))
                    //    {
                    //        assignUserRole.Photo.PreviewPhoto = VirtualPathUtility.ToAbsolute(documentPath + "/" + UserSignal.SignatureFileName);
                    //    }
                    //}

                }
            }
            catch (Exception ex)
            {

            }

            return assignUserRole;
        }
        public string InitialGetRoleStatus(int roleID)
        {
            AssignUserRoleItemViewModel roleItem = new AssignUserRoleItemViewModel();
            var context = new PYMFEEEntities();
            var appComposite = context.AppCompositeRoles.Where(m => m.ID == roleID).OrderBy(m => m.Name).FirstOrDefault();
            var valueStatus = ValueHelpService.GetValueHelpText("CONFIGSTATUS", appComposite.Status);
            if (valueStatus != null)
            {
                roleItem.StatusText = valueStatus.ValueText;
               
            }
            return roleItem.StatusText;
        }
        public override ValidationResult SaveCreate(AssignUserRoleViewModel formData, ModelStateDictionary modelState)
        {
            throw new NotImplementedException();
        }

        public override ValidationResult SaveEdit(AssignUserRoleViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            try
            {
                result = ValidateFormData(formData, modelState);
                //Error
                if (result.ErrorFlag)
                {
                    return result;
                }
                var _context = new PYMFEEEntities();
                #region signal
                //var signal = _context.UserSignatures.Where(m => m.Username == formData.UserName).FirstOrDefault();
                //if (signal != null)
                //{
                //    formData.AssignUserRoleItem.PhotoFileName = signal.SignatureFileName;
                //}
                #endregion
                using (TransactionScope scope = new TransactionScope())
                {
                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {
                        //Save Item      
                        foreach (var item in formData.AssignUserRoleItemList)
                        {
                            var _entAssignUserRoleItem = new UserRole();
                            item.CREATE_BY = formData.CREATE_BY;
                            item.CREATE_DATE = formData.CREATE_DATE;
                            item.MODIFIED_BY = formData.MODIFIED_BY;
                            item.MODIFIED_DATE = formData.MODIFIED_DATE;
                            MVMMappingService.MoveData(item, _entAssignUserRoleItem);
                            _entAssignUserRoleItem.USERID = formData.AssignUserCode;

                            if (item.DeleteFlag)
                            {
                                if (_entAssignUserRoleItem.ID != 0)
                                {
                                    context.Entry(_entAssignUserRoleItem).State = System.Data.Entity.EntityState.Deleted;
                                    context.SaveChanges();
                                }
                            }
                            else
                            {

                                if (_entAssignUserRoleItem.ID != 0)
                                {
                                    context.Entry(_entAssignUserRoleItem).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    context.UserRoles.Add(_entAssignUserRoleItem);
                                }
                                context.SaveChanges();
                            }

                            //context.SaveChanges();
                        }
                        #region Manage UserSignature

                        ////UserSignature
                        //var _entUserSignature = new UserSignature();

                        //_entUserSignature.Username = formData.UserName;
                        //_entUserSignature.CreateBy = formData.CREATE_BY;
                        //_entUserSignature.CreateDate = formData.CREATE_DATE;
                        //_entUserSignature.LastModifyBy = formData.MODIFIED_BY;
                        //_entUserSignature.LastModifyDate = formData.MODIFIED_DATE;
                        //_entUserSignature.SignatureFileName = formData.Photo.PhotoSavedFilename;
                        //if (formData.Photo.DeletedPhotoFlag)
                        //{
                        //    if (signal != null)
                        //    {
                        //        context.Entry(signal).State = System.Data.Entity.EntityState.Deleted;
                        //        context.SaveChanges();
                        //    }
                        //}
                        //else
                        //{
                        //    if (!string.IsNullOrEmpty(formData.Photo.PhotoSavedFilename))
                        //    {
                        //        if (signal != null)
                        //        {
                        //            context.Entry(_entUserSignature).State = System.Data.Entity.EntityState.Modified;
                        //        }
                        //        else
                        //        {
                        //            context.UserSignatures.Add(_entUserSignature);
                        //        }
                        //        context.SaveChanges();

                        //    }
                        //}
                        #endregion
                    }


                    //Commit Header and Item
                    scope.Complete();

                    //Manage photo file
                    ManagePhotoFile(formData);

                    result.Message = ResourceText.SuccessfulEdit;
                    result.MessageType = "S";


                }
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
                result.ErrorFlag = true;
            }

            return result;
        }
        public void ManagePhotoFile(AssignUserRoleViewModel formData)
        {
            try
            {
                //Save employee photo
                if (formData.Photo.DeletedPhotoFlag)
                {
                    //Delete uploaded photo
                    AttachmentService.DeleteFile(formData.Photo.PhotoSavedFilename, ConfigurationManager.AppSettings["TempFilePath"]);
                    AttachmentService.DeleteFile(formData.Photo.FilenameToDelete, ConfigurationManager.AppSettings["TempFilePath"]);

                    //Delete old photo from agent document
                    AttachmentService.DeleteFile(formData.AssignUserRoleItem.PhotoFileName, AttachmentService.GetDocumentFilePath(AssignUserRoleViewModel.ProcessCode));
                }
                else
                {
                    if (!string.IsNullOrEmpty(formData.Photo.PhotoSavedFilename))
                    {
                        //Move file from temp to agent document storage
                        List<AttachmentViewModel> attachmentList = new List<AttachmentViewModel>();

                        AttachmentViewModel attachment = new AttachmentViewModel();

                        attachment.SavedFileName = formData.Photo.PhotoSavedFilename;

                        //Target path
                        attachment.DocumentFilePath = AttachmentService.GetDocumentFilePath(AssignUserRoleViewModel.ProcessCode);

                        attachmentList.Add(attachment);

                        AttachmentService.ManageFile(attachmentList);

                        //Delete old photo from agent document
                        AttachmentService.DeleteFile(formData.AssignUserRoleItem.PhotoFileName, AttachmentService.GetDocumentFilePath(AssignUserRoleViewModel.ProcessCode));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override ValidationResult SaveDelete(AssignUserRoleViewModel formData, ModelStateDictionary modelState)
        {
            throw new NotImplementedException();
        }

        public AssignUserRoleItemViewModel InitialItem()
        {
            AssignUserRoleItemViewModel assignUserRoleItemViewModel = new AssignUserRoleItemViewModel();
            List<AssignUserRoleItemViewModel> assignUserRoleItemList = new List<AssignUserRoleItemViewModel>();

            try
            {
                var context = new PYMFEEEntities();
                //var appComposite = context.AppCompositeRoles.Where(m => m.Status == ConstantVariableService.ConfigStatusActive).OrderBy(m => m.Name).ToList();
                var appComposite = context.AppCompositeRoles.OrderBy(m => m.Name).ToList();
                assignUserRoleItemViewModel.CompositeRoleList = appComposite;



            }
            catch (Exception ex)
            {

            }

            return assignUserRoleItemViewModel;
        }
    }
}