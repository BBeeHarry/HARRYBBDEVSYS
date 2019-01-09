using BBDEVSYS.Services.Abstract;
using BBDEVSYS.ViewModels.AccruedUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.Models.Shared;
using System.Web.Mvc;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Services.Shared;
using System.Transactions;
using BBDEVSYS.Content.text;
using BBDEVSYS.ViewModels.Shared;
using System.Configuration;
using System.Data;
using System.Globalization;
using BBDEVSYS.Services.Accrued;

namespace BBDEVSYS.Services.Accrued
{
    public class AccruedUploadService : AbstractControllerService<AccruedUploadViewModel>
    {
        public override AccruedUploadViewModel GetDetail(int id)
        {
            AccruedUploadViewModel accruedUpload = NewFormData();

            try
            {
                //User user = UserService.GetSessionUserInfo();
                using (var context = new PYMFEEEntities())
                {
                    //AccruedUpload entaccruedUpload = new AccruedUpload();
                    //entaccruedUpload = (from data in context.AccruedUploads
                    //                     where data.ID == id
                    //                     orderby data.ID
                    //                     select data).FirstOrDefault();
                    //MVMMappingService.MoveData(entaccruedUpload, accruedUpload);
                    //var whList = (from w in context.Warehouses where w.AccruedCategory == accruedUpload.AccruedType select w).ToList();
                    //accruedUpload.WarehouseCodeValueHelp = whList;
                    ////Check employee is Super admin?
                    //var roleAdminList = (from m in context.UserRoles
                    //                     join n in context.AppCompositeRoles on m.CompositeRoleID equals n.ID
                    //                     where m.Username == user.ADUser &&
                    //                     n.StockSuperAdmin == true
                    //                     select n).ToList();

                    //if (roleAdminList.Any())
                    //{
                    //    accruedUpload.UserSuperAdmin = true;
                    //}
                    //else
                    //{
                    //    accruedUpload.UserSuperAdmin = false;
                    //}


                    ////Get line item
                    //var accruedUploadItemList = (from m in context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG
                    //                              where m.AccruedUploadID == id
                    //                              select m).OrderBy(m => m.ID).ToList();

                    //var org = HRService.GetAllOrgList();

                    //var mat = (from m in context.AccruedGens orderby m.AccruedCode select m).ToList();


                    //foreach (var item in accruedUploadItemList)
                    //{
                    //    AccruedUploadItemViewModel accruedUploadItem = new AccruedUploadItemViewModel();
                    //    MVMMappingService.MoveData(item, accruedUploadItem);
                    //    item.AccruedCode = ProductDetailService.AccruedCodeSplitEmpty(item.AccruedCode);
                    //    var getmat = mat.Where(m => ProductDetailService.AccruedCodeSplitEmpty(m.AccruedCode) == item.AccruedCode).FirstOrDefault();
                    //    var getorg = org.Where(m => m.OrgID == item.OrgID).FirstOrDefault();

                    //    accruedUploadItem.AccruedName = getmat == null ? "" : getmat.DescriptionTH;
                    //    accruedUploadItem.OrgName = org == null ? "" : getorg.OrgName;

                    //    accruedUpload.AccruedUploadItemList.Add(accruedUploadItem);
                    //    //Get line item

                    //}

                }
                //Get attachment
                AttachmentViewModel getAttachmentOption = new AttachmentViewModel();
                getAttachmentOption.ProcessCode = AccruedUploadViewModel.ProcessCode;
                //getAttachmentOption.DataID = id;
                getAttachmentOption.DataKey = id.ToString();
                getAttachmentOption.DocumentFilePath = AttachmentService.GetDocumentFilePath(AccruedUploadViewModel.ProcessCode);

                accruedUpload.AttachmentList = AttachmentService.GetAttachmentList(getAttachmentOption);
            }
            catch (Exception ex)
            {

            }
            return accruedUpload;
        }

        public AccruedUploadViewModel InitialSearch()
        {
            AccruedUploadViewModel accrued = new AccruedUploadViewModel();
            var context = new PYMFEEEntities();
            try
            {
                //accrued.AccruedTypeValueHelp = ValueHelpService.GetValueHelp("AccruedTYPE").OrderBy(m => m.ValueKey).ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return accrued;
        }

        public override string GetList()
        {
            string dataList = "";
            List<AccruedUploadViewModel> accruedList = GetAccruedUploadList();

            dataList = DatatablesService.ConvertObjectListToDatatables<AccruedUploadViewModel>(accruedList);

            return dataList;
        }


        public List<AccruedUploadViewModel> GetAccruedUploadList()
        {
            List<AccruedUploadViewModel> accruedUploadList = new List<AccruedUploadViewModel>();
            AccruedUploadViewModel entaccruedUpload = NewFormData();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    //var accruedUpload = (from m in context.FEE_ACCRUED_PLAN_UPLOAD
                    //                      orderby new { m.AccruedType, m.WarehouseCode, m.UploadType, m.UploadDate }
                    //                      select m).ToList();

                   // var wh = (from w in context.Warehouses select w).ToList();
                    //foreach (var item in accruedUpload)
                    //{
                      //var accruedUploadModel = new AccruedUploadViewModel();
                        //MVMMappingService.MoveData(item, accruedUploadModel);
                        //var whName = wh.Where(m => m.Code == item.WarehouseCode).FirstOrDefault();
                        //var mattypeName = entaccruedUpload.AccruedTypeValueHelp.Where(m => m.ValueKey == item.AccruedType).FirstOrDefault();
                        //accruedUploadModel.WarehouseName = whName == null ? "" : whName.Name;
                        //accruedUploadModel.AccruedTypeName = mattypeName == null ? "" : mattypeName.ValueText;

                        ////Get attachment
                        //AttachmentViewModel getAttachmentOption = new AttachmentViewModel();
                        //getAttachmentOption.ProcessCode = AccruedUploadViewModel.ProcessCode;
                        ////getAttachmentOption.DataID = id;
                        //getAttachmentOption.DataKey = item.ID.ToString();
                        //getAttachmentOption.DocumentFilePath = AttachmentService.GetDocumentFilePath(AccruedUploadViewModel.ProcessCode);

                        //accruedUploadModel.AttachmentList = AttachmentService.GetAttachmentList(getAttachmentOption);

                        //accruedUploadModel.FileName = accruedUploadModel.AttachmentList.FirstOrDefault().FileName;
                        //accruedUploadList.Add(accruedUploadModel);



                   // }
                }

            }
            catch (Exception ex)
            {

            }
            return accruedUploadList;
        }


        public override AccruedUploadViewModel NewFormData()
        {
            AccruedUploadViewModel accruedUpload = new AccruedUploadViewModel();
            var context = new PYMFEEEntities();
            //var empList = new List<HREmployee>();
            //empList = (from m in context.HREmployees orderby m.FirstnameTH select m).ToList();
            //try
            //{
            //    User userInfo = UserService.GetSessionUserInfo();
            //    accruedUpload.AccruedTypeValueHelp = ValueHelpService.GetValueHelp("AccruedTYPE").OrderBy(m => m.ValueKey).ToList();
            //    accruedUpload.UploadDate = DateTime.Now.Date;
            //    //Check employee is Super admin?
            //    var roleAdminList = (from m in context.UserRoles
            //                         join n in context.AppCompositeRoles on m.CompositeRoleID equals n.ID
            //                         where m.Username == userInfo.ADUser &&
            //                         n.StockSuperAdmin == true
            //                         select n).ToList();

            //    if (roleAdminList.Any())
            //    {
            //        accruedUpload.UploadType = ResourceText.Replace;
            //        accruedUpload.UserSuperAdmin = true;
            //    }
            //    else
            //    {
            //        accruedUpload.UploadType = ResourceText.Add;
            //        accruedUpload.UserSuperAdmin = false;
            //    }

            //    accruedUpload.UploadStatus = ConstantVariableService.TransStatusCompleted;
            //    accruedUpload.UploadBy = userInfo.UserCode;

            //    var uploadByText = empList.Where(m => m.EmpNo == accruedUpload.UploadBy).FirstOrDefault();
            //    accruedUpload.UploadByName = (uploadByText == null ? "" : string.Concat(uploadByText.EmpNo, " ", uploadByText.TitleTH, uploadByText.FirstnameTH, " ", uploadByText.LastnameTH));
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
            return accruedUpload;
        }
        public AccruedUploadViewModel InitialWarehouse(string whMatType)
        {
            AccruedUploadViewModel accruedUpload = new AccruedUploadViewModel();
            //var whList = new List<Models.Entities.Warehouse>();
            //using (var context = new PYMFEEEntities())
            //{
            //    whList = (from m in context.Warehouses
            //              where m.AccruedCategory == whMatType
            //              orderby m.Code
            //              select m).ToList();
            //}

            //accruedUpload.WarehouseCodeValueHelp = whList;

            return accruedUpload;
        }

        public override ValidationResult SaveCreate(AccruedUploadViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            AccruedService service = new AccruedService();
            var accruedList = new List<Models.Entities.FEE_ACCRUED_PLAN_UPLOAD>();
            var accruedDetailList = new List<Models.Entities.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG>();
            try
            {
                //var user = UserService.GetSessionUserInfo();
                //var convertResult = ConvertUploadItemToFormData(formData);

                //if (convertResult.ErrorFlag)
                //{
                //    result.ErrorFlag = true;
                //    result.ModelStateErrorList = convertResult.ModelStateErrorList;

                //    return result;
                //}


                //formData = convertResult.ReturnResult;
                //using (TransactionScope scope = new TransactionScope())
                //{
                //    var entaccruedUpload = new AccruedUpload();

                //    //Copy data from viewmodel to model - for header
                //    MVMMappingService.MoveData(formData, entaccruedUpload);

                //    //Save Header 
                //    using (var context = new PYMFEEEntities())
                //    {
                //        context.AccruedUploads.Add(entaccruedUpload);
                //        context.SaveChanges();

                //        formData.ID = entaccruedUpload.ID;

                //        //Save Item
                //        //Copy data from viewmodel to model - for line item
                //        accruedList = (from s in context.Accrueds
                //                        select s).ToList();
                //        accruedDetailList = (from t in context.AccruedItems
                //                              select t).ToList();
                //        int sequence = 1;
                //        foreach (var item in formData.AccruedUploadItemList)
                //        {

                //            if (item.DeleteFlag)
                //            {
                //                continue;
                //            }

                //            var entAccruedUploadItem = new AccruedUploadItem();
                //            item.AccruedUploadID = entaccruedUpload.ID;
                //            item.Sequence = sequence;
                //            MVMMappingService.MoveData(item, entAccruedUploadItem);
                //            context.AccruedUploadItems.Add(entAccruedUploadItem);
                //            context.SaveChanges();

                //            //Update Accrued
                //            AccruedViewModel accruedViewModel = new AccruedViewModel();
                //            AccruedItemViewModel accruedItemViewModel = new AccruedItemViewModel();
                //            var getaccruedList = accruedList.Where(s => ProductDetailService.AccruedCodeSplitEmpty(s.AccruedCode) == ProductDetailService.AccruedCodeSplitEmpty(item.AccruedCode)).ToList();
                //            int ID_accrued = 0;
                //            int ID_accruedItem = 0;
                //            DateTime pickupDate = Convert.ToDateTime(item.StartDate);

                //            if (getaccruedList.Any())
                //            {
                //                var Accrued = getaccruedList.FirstOrDefault();
                //                ID_accrued = Accrued.ID;
                //                decimal price = Convert.ToDecimal(item.PricePerUnit);


                //                var getaccruedDetailList = accruedDetailList.Where(m =>
                //                m.AccruedID == ID_accrued &&
                //                m.StartDate == pickupDate &&
                //                m.OrgID == item.OrgID &&
                //                m.CostCenter == item.CostCenter &&
                //                m.PricePerUnit == price &&
                //                m.WarehouseCode == formData.WarehouseCode).OrderByDescending(m => m.Procedure).ToList();


                //                accruedItemViewModel.Amount = Convert.ToDecimal(item.Amount);


                //                accruedItemViewModel.PricePerUnit = Convert.ToDecimal(item.PricePerUnit);
                //                accruedItemViewModel.DeductAmount = 0;

                //                #region Sum Total amoun in Accrued
                //                var db = new PYMFEEEntities();
                //                var accrued_amount = (from d in db.Accrueds
                //                                       select d
                //                          ).ToList();

                //                var sum_accrued_amount = accrued_amount.Where(d => ProductDetailService.AccruedCodeSplitEmpty(d.AccruedCode) == ProductDetailService.AccruedCodeSplitEmpty(item.AccruedCode)
                //                        ).FirstOrDefault();
                //                #endregion

                //                accruedViewModel.AmountTotal = Convert.ToString(sum_accrued_amount.TotalAmount + accruedItemViewModel.Amount);

                //                if (!getaccruedDetailList.Any())
                //                {
                //                    ID_accruedItem = 0;
                //                }
                //                else
                //                {
                //                    #region Sum Total amoun in AccruedItem
                //                    db = new PYMFEEEntities();
                //                    var accrued_item_amount = (from d in db.AccruedItems
                //                                                select d
                //                              ).ToList();

                //                    var sum_accrued_item_amount = accrued_item_amount.Where(d => d.AccruedID == ID_accrued &&
                //                        d.StartDate == pickupDate &&
                //                        d.OrgID == item.OrgID &&
                //                        d.CostCenter == item.CostCenter &&
                //                        d.PricePerUnit == price &&
                //                        d.WarehouseCode == formData.WarehouseCode).OrderByDescending(m => m.Procedure
                //                            ).FirstOrDefault();
                //                    #endregion
                //                    var AccruedDetail = getaccruedDetailList.FirstOrDefault();
                //                    ID_accruedItem = AccruedDetail.ID;
                //                    if (formData.UploadType == ResourceText.Add)
                //                    {
                //                        accruedItemViewModel.Amount = Convert.ToDecimal(item.Amount) + sum_accrued_item_amount.Amount;
                //                    }
                //                    else if (formData.UploadType == ResourceText.Replace)
                //                    {
                //                        accruedViewModel.AmountTotal = Convert.ToString((sum_accrued_amount.TotalAmount - AccruedDetail.Amount) + accruedItemViewModel.Amount);
                //                    }
                //                    accruedItemViewModel.DeductAmount = AccruedDetail.DeductAmount;
                //                }



                //                accruedViewModel.ID = ID_accrued;
                //                accruedViewModel.AccruedCode = Accrued.AccruedCode;
                //                accruedViewModel.AccruedType = Accrued.AccruedType;
                //                accruedViewModel.Status = Accrued.Status;
                //                accruedViewModel.UnitCode = Accrued.UnitCode;
                //                accruedViewModel.ProcedureType = ConstantVariableService.AccruedProcedureUpload;


                //                accruedItemViewModel.ID = ID_accruedItem;
                //                accruedItemViewModel.AccruedID = ID_accrued;
                //                accruedItemViewModel.StartDate = pickupDate;
                //                accruedItemViewModel.OrgID = item.OrgID;

                //                accruedItemViewModel.WarehouseCode = formData.WarehouseCode;
                //                accruedItemViewModel.CostCenter = item.CostCenter;
                //                accruedItemViewModel.Procedure = ConstantVariableService.AccruedProcedureUpload;


                //                accruedViewModel.AccruedItemList.Add(accruedItemViewModel);

                //                result =service.SaveEdit(accruedViewModel, modelState);
                //                if (result.ErrorFlag)
                //                {
                //                    result.ErrorFlag = true;
                //                    result.ModelStateErrorList = result.ModelStateErrorList;

                //                    return result;
                //                }
                //            }
                //            sequence++;

                //        }
                //    }
                //    // service.SaveEdit(formData,modelState);


                //    scope.Complete();

                //    //result.Message = ResourceText.SuccessfulSave;
                //    //result.MessageType = "S";

                //}
                ////Manage attachment
                //if (formData.ID != 0)
                //{
                //    var docFilePath = AttachmentService.GetDocumentFilePath(AccruedUploadViewModel.ProcessCode);
                //    foreach (var attachment in formData.AttachmentList)
                //    {
                //        attachment.ProcessCode = AccruedUploadViewModel.ProcessCode;
                //        //attachment.DataID = formData.ID;
                //        attachment.DataKey = formData.ID.ToString();
                //        attachment.DocumentFilePath = docFilePath;
                //    }
                //    AttachmentService.ManageAttachment(formData.AttachmentList);
                //}

                //result.Message = ResourceText.SuccessfulUpload;//+ " (" + ResourceText.DocumentNo + ": " + formData.JobDocNo + ")";
                //result.MessageType = "S";
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
                result.ErrorFlag = true;
            }

            return result;
        }

        public override ValidationResult SaveDelete(AccruedUploadViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    var entaccruedUpload = new FEE_ACCRUED_PLAN_UPLOAD();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entaccruedUpload);

                    using (var context = new PYMFEEEntities())
                    {
                        //Delete header            
                        context.Entry(entaccruedUpload).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();

                        //Delete item
                        context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG.RemoveRange(context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG.Where(m => m.ACCRUED_UPLOAD_ID == entaccruedUpload.ID));
                        context.SaveChanges();
                    }

                    //Commit Header and Item
                    scope.Complete();

                    result.Message = ResourceText.SuccessfulDelete;
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

        public override ValidationResult SaveEdit(AccruedUploadViewModel formData, ModelStateDictionary modelState)
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

                using (TransactionScope scope = new TransactionScope())
                {
                    FEE_ACCRUED_PLAN_UPLOAD entaccruedUpload = new FEE_ACCRUED_PLAN_UPLOAD();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entaccruedUpload);
                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {

                        context.Entry(entaccruedUpload).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();


                        //Save Item          
                        foreach (var item in formData.AccruedUploadItemList)
                        {
                            var entaccruedUploadItem = new FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG();

                            item.ACCRUED_UPLOAD_ID = entaccruedUpload.ID;
                            MVMMappingService.MoveData(item, entaccruedUploadItem);
                            if (item.DeleteFlag)
                            {
                                if (entaccruedUploadItem.ID != 0)
                                {
                                    context.Entry(entaccruedUploadItem).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                            else
                            {


                                if (entaccruedUploadItem.ID != 0)
                                {
                                    context.Entry(entaccruedUploadItem).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG.Add(entaccruedUploadItem);
                                }
                            }

                            context.SaveChanges();
                        }
                    }

                    //Commit Header and Item
                    scope.Complete();

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

        public override ValidationResult ValidateFormData(AccruedUploadViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            var _context = new PYMFEEEntities();
            try
            {

                //if (!modelState.IsValid)
                //{
                //    result.ModelStateErrorList = UtilityService.GetModelStateErrors(modelState);
                //    result.ErrorFlag = true;
                //}
                //else
                //{
                //    //Get item => deleteFlag != true
                //    var itemList = formData.AccruedUploadItemList.Where(m => m.DeleteFlag != true).ToList();

                //    //Check item
                //    int line = 1;
                //    foreach (var item in itemList)
                //    {
                //        //product id not empty
                //        if (item.AccruedCode == null)
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, ResourceText.ConsumableCode, line.ToString())));

                //            result.ErrorFlag = true;
                //        }
                //        if (item.StartDate == null)
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, ResourceText.PickUpDate, line.ToString())));

                //            result.ErrorFlag = true;
                //        }
                //        if (item.Amount == null)
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, ResourceText.Amount, line.ToString())));

                //            result.ErrorFlag = true;
                //        }
                //        else
                //        {
                //            decimal deductAmount = 0;
                //            decimal amount = 0;
                //            if (amount < deductAmount)
                //            {
                //                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.invalid_amount_error, ResourceText.Amount, line.ToString())));

                //                result.ErrorFlag = true;
                //            }
                //        }
                //        //unit allow null
                //        if (item.PricePerUnit == null)
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, ResourceText.PricePerUnit, line.ToString())));

                //            result.ErrorFlag = true;
                //        }
                //        if (item.OrgID == null)
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, ResourceText.PickUpOrg, line.ToString())));

                //            result.ErrorFlag = true;
                //        }

                //        if (item.CostCenter == null)
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_at_error, "Cost Center", line.ToString())));

                //            result.ErrorFlag = true;
                //        }


                //        line++;
                //    }


                //if (formData.FormAction == ConstantVariableService.FormStateEdit)
                //{

                //}
                //if (formData.FormAction == ConstantVariableService.FormStateCreate || formData.FormAction == ConstantVariableService.FormStateCopy)
                //{
                //    //var dupaccrued = _context.Accrueds.Where(m => m.AccruedType == formData.AccruedType
                //    //&& m.AccruedCode == formData.AccruedCode).ToList();
                //    //if (dupaccrued.Any())
                //    //{
                //    //    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, string.Concat(ResourceText.AccruedCategory, " ", ResourceText.ConsumableCode))));
                //    //    result.ErrorFlag = true;
                //    //}
                //    }

                //}


                var convertResult = ConvertUploadItemToFormData(formData);


                if (convertResult.ErrorFlag)
                {
                    result.ErrorFlag = true;
                    result.ModelStateErrorList = convertResult.ModelStateErrorList;

                    return result;
                }
                if (!convertResult.ErrorFlag)
                {
                    result.Message = ResourceText.SuccessfulUploadValidate;
                }

            }
            catch (Exception ex)
            {
                result.MessageType = ex.ToString();
                result.Message = ex.ToString();
            }

            return result;
        }
        public ValidationWithReturnResult<AccruedUploadViewModel> ConvertUploadItemToFormData(AccruedUploadViewModel uploadItem)
        {
            ValidationWithReturnResult<AccruedUploadViewModel> result = new ValidationWithReturnResult<AccruedUploadViewModel>();
            result.ReturnResult = new AccruedUploadViewModel();

            //Convert result
            ValidationWithReturnResult<decimal> decimalResult = new ValidationWithReturnResult<decimal>();

            try
            {
                //AccruedUploadViewModel stockUploadViewModel = new AccruedUploadViewModel();
                //User user = UserService.GetSessionUserInfo();
                //List<HREmployee> empList = new List<HREmployee>();
                //MVMMappingService.MoveData(uploadItem, stockUploadViewModel);
                //stockUploadViewModel.UploadStatus = ConstantVariableService.TransStatusCompleted;

                ////Import Job shop and product from excel
                //var uploadFile = uploadItem.AttachmentList.Where(m => m.DeleteFlag != true).FirstOrDefault();

                //if (uploadFile == null)
                //{
                //    result.ErrorFlag = true;
                //    result.ModelStateErrorList.Add(new ModelStateError("", ValidatorMessage.upload_file_notempty_error));

                //    return result;
                //}

                //var importResult = ImportExcel(uploadFile.SavedFileName, uploadItem.UploadType, uploadItem.WarehouseCode, uploadItem.AccruedType);

                //if (importResult.ErrorFlag)
                //{
                //    result.ErrorFlag = true;
                //    result.ModelStateErrorList.Add(new ModelStateError("", importResult.Message));

                //    return result;
                //}

                //var matuploadError = importResult.ReturnResult.Where(m => m.ErrorFlag == true).FirstOrDefault();
                //var matuploadErrorMs = importResult.ReturnResult.Where(m => m.ErrorFlag == true).ToList();
                //if (matuploadError != null)
                //{

                //    result.ErrorFlag = true;

                //    if (!string.IsNullOrEmpty(matuploadError.MessageAt))
                //    {
                //        foreach (var item in matuploadErrorMs)
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", item.MessageAt));
                //        }
                //    }
                //    else
                //    {
                //        result.ModelStateErrorList.Add(new ModelStateError("", ValidatorMessage.upload_file_content_error));
                //    }

                //    return result;
                //}

                //var accruedList = new List<Models.Entities.Accrued>();
                //var accruedDetailList = new List<Models.Entities.AccruedItem>();

                //using (var context = new PYMFEEEntities())
                //{
                //    accruedList = (from s in context.Accrueds
                //                    select s).ToList();
                //    accruedDetailList = (from t in context.AccruedItems
                //                          select t).ToList();
                //}
                //int line = 1;
                //foreach (var uploadmat in importResult.ReturnResult)
                //{

                //    //Valid Accrued

                //    var getaccruedList = accruedList.Where(s => ProductDetailService.AccruedCodeSplitEmpty(s.AccruedCode) == ProductDetailService.AccruedCodeSplitEmpty(uploadmat.AccruedCode)).ToList();
                //    int ID_accrued = 0;
                //    int ID_accruedItem = 0;
                //    DateTime pickupDate = Convert.ToDateTime(uploadmat.StartDate);

                //    if (getaccruedList.Any())
                //    {
                //        var Accrued = getaccruedList.FirstOrDefault();
                //        ID_accrued = Accrued.ID;
                //        decimal price = Convert.ToDecimal(uploadmat.PricePerUnit);


                //        var getaccruedDetail = accruedDetailList.Where(m =>
                //        m.AccruedID == ID_accrued &&
                //        m.StartDate == pickupDate &&
                //        m.OrgID == uploadmat.OrgID &&
                //        m.CostCenter == uploadmat.CostCenter &&
                //        m.PricePerUnit == price &&
                //        m.WarehouseCode == uploadItem.WarehouseCode).OrderByDescending(m => m.Procedure).ToList();



                //        if (!getaccruedDetail.Any())
                //        {
                //            ID_accruedItem = 0;
                //        }
                //        else
                //        {

                //            var AccruedDetail = getaccruedDetail.FirstOrDefault();
                //            ID_accruedItem = AccruedDetail.ID;

                //        }
                //        if (ID_accruedItem == 0 && string.Equals(uploadItem.UploadType, ResourceText.Replace, StringComparison.OrdinalIgnoreCase))
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.cannot_found_at_data, ResourceText.TitleAccruedUpload, line.ToString())));

                //            result.ErrorFlag = true;
                //        }
                //    }
                //    line++;

                //    //    AccruedUploadItemViewModel matuploadItem = new AccruedUploadItemViewModel();

                //    //    matuploadItem.AccruedCode = uploadmat.AccruedCode;
                //    //    matuploadItem.AccruedName = uploadmat.AccruedName;
                //    //    matuploadItem.Amount = uploadmat.Amount;
                //    //    matuploadItem.PricePerUnit = uploadmat.PricePerUnit;
                //    //    matuploadItem.StartDate = Convert.ToDateTime(uploadmat.StartDate).ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                //    //    matuploadItem.CostCenter = uploadmat.CostCenter;
                //    //    matuploadItem.OrgID = uploadmat.OrgID;
                //    //    matuploadItem.OrgName = uploadmat.OrgName;
                //    //if (string.IsNullOrEmpty(uploadmat.AccruedCode))
                //    //{
                //    //    result.ErrorFlag = true;
                //    //}
                //    //if (result.ErrorFlag)
                //    //{
                //    //    result.ModelStateErrorList.Add(new ModelStateError("", ValidatorMessage.upload_file_content_error));
                //    //    break;
                //    //}



                //    //else
                //    //{
                //    //    stockUploadViewModel.AccruedUploadItemList.Add(matuploadItem);
                //    //}

                //}

                //result.ReturnResult = stockUploadViewModel;
            }
            catch (Exception ex)
            {

            }

            return result;
        }


        public ValidationWithReturnResult<List<AccruedUploadItemViewModel>> ImportExcel(string fileName, string uploadType, string whCode, string matType)
        {
            ValidationWithReturnResult<List<AccruedUploadItemViewModel>> result = new ValidationWithReturnResult<List<AccruedUploadItemViewModel>>();
            result.ReturnResult = new List<AccruedUploadItemViewModel>();

            List<AccruedUploadItemViewModel> uploadItemList = new List<AccruedUploadItemViewModel>();

            try
            {
                //Check required field
                if (string.IsNullOrEmpty(uploadType))
                {
                    result.ErrorFlag = true;
                    result.Message = ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.UploadType);
                    result.ModelStateErrorList.Add(new ModelStateError("", result.Message));

                }
                if (string.IsNullOrEmpty(whCode))
                {
                    result.ErrorFlag = true;
                    //result.Message = ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.Warehouse);
                    result.ModelStateErrorList.Add(new ModelStateError("", result.Message));

                }

                if (string.IsNullOrEmpty(matType))
                {
                    result.ErrorFlag = true;
                    //result.Message = ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.AccruedCategory);
                    result.ModelStateErrorList.Add(new ModelStateError("", result.Message));

                }
                if (result.ErrorFlag)
                {
                    return result;
                }


                string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
                tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);
                //DataSet excelDS = ExcelService.ConvertExcelToDataSet(tempFilePath, fileName, 1);
                ValidationWithReturnResult<DataSet> resultDataSet = ExcelService.ConvertExcelToDataSet(tempFilePath, fileName, "Sheet1");
                DataSet excelDS = new DataSet();

                if (resultDataSet.ErrorFlag)
                {
                    result.ErrorFlag = true;
                    result.Message = resultDataSet.Message;

                    return result;
                }
                else
                {
                    excelDS = resultDataSet.ReturnResult;
                }

                //Convert dataset to data view
                int rowIndex = 1; //Start data at row 1 (begin row is 0)

                foreach (DataTable table in excelDS.Tables)
                {
                    //while (rowIndex < table.Rows.Count)
                    //{
                    //    DataRow row = table.Rows[rowIndex];
                    //    AccruedUploadItemViewModel uploadItem = new AccruedUploadItemViewModel();

                    //    uploadItem.AccruedCode = row[0].ToString();
                    //    uploadItem.Amount = row[1].ToString();
                    //    uploadItem.PricePerUnit = row[2].ToString();
                    //    uploadItem.StartDate = row[3].ToString();
                    //    uploadItem.CostCenter = row[4].ToString();
                    //    uploadItem.OrgID = row[5].ToString();


                    //    uploadItemList.Add(uploadItem);
                    //    rowIndex++;
                    //}
                    rowIndex = 1;
                }

                result = ValidateUploadItemFormat(uploadItemList, uploadType, whCode, matType);

            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString(), "");
            }

            return result;
        }
        public static ValidationWithReturnResult<DateTime> ConvertStringToDateTime(string text)
        {
            ValidationWithReturnResult<DateTime> result = new ValidationWithReturnResult<DateTime>();

            try
            {
                //Remove all comma
                string[] strDate = text.Split('/');
                if (strDate[0].Trim().Length == 1)
                {
                    strDate[0] = "0" + strDate[0];
                }
                if (strDate[1].Trim().Length == 1)
                {
                    strDate[1] = "0" + strDate[1];
                }
                string dateInput = string.Concat(strDate[2].Trim(), '-', strDate[1].Trim(), '-', strDate[0].Trim());

                result.ReturnResult = DateTime.ParseExact(dateInput, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));

                //DateTime.ParseExact("2009-05-08 14:40:52,531", "yyyy-MM-dd HH:mm:ss,fff",
                //                   System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString() + "/" + text, "");
            }

            return result;
        }
        public ValidationWithReturnResult<List<AccruedUploadItemViewModel>> ValidateUploadItemFormat(List<AccruedUploadItemViewModel> uploadItemList, string uploadType = "", string warehouseCode = "", string matType = "")
        {
            ValidationWithReturnResult<List<AccruedUploadItemViewModel>> result = new ValidationWithReturnResult<List<AccruedUploadItemViewModel>>();
            result.ReturnResult = new List<AccruedUploadItemViewModel>();

            //Convert result
            ValidationWithReturnResult<decimal> decimalResult = new ValidationWithReturnResult<decimal>();
            ValidationWithReturnResult<DateTime> datetimeResult = new ValidationWithReturnResult<DateTime>();

            try
            {
                //using (var context = new PYMFEEEntities())
                //{
                //    //Accrued All value help
                //    var matList = ProductService.GetProductValueHelp();
                //    List<string> getMatType = new List<string>();
                //    if (matType == ConstantVariableService.AccruedTypeSupport)
                //    {
                //        getMatType = new List<string> { "Z005", "Z007" };

                //    }

                //    else if (matType == ConstantVariableService.AccruedTypeProduct)
                //    {
                //        getMatType = new List<string> { "Z001" };

                //    }
                //    //Accrued MatType  value help
                //    var matgetTypeList = context.AccruedGens.Where(m => getMatType.Any(o => m.AccruedType == o)).OrderBy(m => m.DescriptionTH).ToList();
                //    //warehouse All value help
                //    var whList = context.Warehouses.Where(m => m.AccruedCategory == matType).OrderBy(m => m.Code).ToList();
                //    //Org All value help
                //    var orgList = HRService.GetAllOrgList();
                //    //CostCenter All value help
                //    var costCenterList = UtilityService.GetCostCenterList().OrderBy(m => m.Code).ToList();

                //    //Manage Accrued Stock
                //    var accrued = (from m in context.Accrueds select m).ToList();
                //    //Manage Accrued Stock Detail
                //    var accruedDetail = (from m in context.AccruedItems select m).ToList();

                //    int line = 1;
                //    foreach (var uploadItem in uploadItemList)
                //    {
                //        bool isValidAmount = false;
                //        //MATCODE --> Stock
                //        var matCode = uploadItem.AccruedCode.Trim();
                //        if (string.IsNullOrWhiteSpace(matCode))
                //        {
                //            uploadItem.ErrorFlag = true;
                //            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.ConsumableCode) + ConstantVariableService.HtmlNewLine;
                //            isValidAmount = true;
                //        }
                //        else
                //        {
                //            var submatCode = ProductDetailService.AccruedCodeSplitEmpty(matCode);
                //            var matDetail = matgetTypeList.Where(m => ProductDetailService.AccruedCodeSplitEmpty(m.AccruedCode) == submatCode).FirstOrDefault();
                //            var matAllDetail = matList.Where(m => ProductDetailService.AccruedCodeSplitEmpty(m.AccruedCode) == submatCode).FirstOrDefault();


                //            if (matDetail == null && matAllDetail != null)
                //            {
                //                uploadItem.AccruedName = matAllDetail.DescriptionTH;
                //                uploadItem.ErrorFlag = true;
                //                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.Accrued_invalid_type_error, ResourceText.ConsumableCode, ResourceText.AccruedCategory) + ConstantVariableService.HtmlNewLine;
                //                isValidAmount = true;
                //            }
                //            else if (matDetail == null && matAllDetail == null)
                //            {
                //                uploadItem.ErrorFlag = true;
                //                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.cannot_found_data, ResourceText.Accrued) + ConstantVariableService.HtmlNewLine;
                //                isValidAmount = true;
                //            }
                //            else
                //            {
                //                uploadItem.AccruedName = matDetail.DescriptionTH;
                //            }
                //        }

                //        //PRICEPERUNIT --> StockItem
                //        var priceperunit = uploadItem.PricePerUnit.Trim();
                //        if (string.IsNullOrWhiteSpace(priceperunit))
                //        {
                //            uploadItem.ErrorFlag = true;
                //            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.PricePerUnit) + ConstantVariableService.HtmlNewLine;
                //            isValidAmount = true;
                //        }
                //        else
                //        {
                //            decimalResult = UtilityService.ConvertStringToDecimal(priceperunit);
                //            if (decimalResult.ErrorFlag || decimalResult.ReturnResult <= 0)
                //            {
                //                uploadItem.ErrorFlag = true;
                //                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.PricePerUnit) + ConstantVariableService.HtmlNewLine;
                //                isValidAmount = true;
                //            }
                //            else
                //            {
                //                uploadItem.PricePerUnit = UtilityService.ConvertToNumberFormat(decimalResult.ReturnResult);
                //            }
                //        }
                //        //COSTCENTER --> StockItem
                //        var costctr = uploadItem.CostCenter.Trim();
                //        if (string.IsNullOrWhiteSpace(costctr))
                //        {
                //            uploadItem.ErrorFlag = true;
                //            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.BudgetCostCenter) + ConstantVariableService.HtmlNewLine;
                //            //isValidAmount = true;
                //        }
                //        else
                //        {
                //            var getCostCtr = costCenterList.Where(c => c.Code == uploadItem.CostCenter).FirstOrDefault();
                //            if (getCostCtr == null)
                //            {
                //                uploadItem.ErrorFlag = true;
                //                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.cannot_found_data, ResourceText.BudgetCostCenter) + ConstantVariableService.HtmlNewLine;
                //                //isValidAmount = true;
                //            }
                //        }
                //        //HRORG --> StockItem
                //        var org = uploadItem.OrgID.Trim();
                //        if (string.IsNullOrWhiteSpace(org))
                //        {
                //            uploadItem.ErrorFlag = true;
                //            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.PickUpOrg) + ConstantVariableService.HtmlNewLine;
                //            isValidAmount = true;
                //        }
                //        else
                //        {
                //            var getOrg = orgList.Where(c => c.OrgID == uploadItem.OrgID).FirstOrDefault();
                //            if (getOrg == null)
                //            {
                //                uploadItem.ErrorFlag = true;
                //                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.cannot_found_data, ResourceText.PickUpOrg) + ConstantVariableService.HtmlNewLine;
                //                isValidAmount = true;
                //            }
                //            else
                //            {
                //                uploadItem.OrgName = getOrg.OrgName;
                //            }
                //        }
                //        //PICKUPDATE --> StockItem
                //        var pickupdate = uploadItem.StartDate.Trim();
                //        if (string.IsNullOrWhiteSpace(pickupdate))
                //        {
                //            uploadItem.ErrorFlag = true;
                //            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.PickUpDate) + ConstantVariableService.HtmlNewLine;
                //            isValidAmount = true;
                //        }
                //        else
                //        {
                //            datetimeResult = ConvertStringToDateTime(pickupdate);
                //            if (datetimeResult.ErrorFlag)
                //            {
                //                uploadItem.ErrorFlag = true;
                //                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format_datetime, ResourceText.PickUpDate) + ConstantVariableService.HtmlNewLine;
                //                isValidAmount = true;
                //            }
                //        }
                //        //AMOUNT --> StockItem
                //        var amount = uploadItem.Amount.Trim();
                //        if (string.IsNullOrWhiteSpace(amount))
                //        {
                //            uploadItem.ErrorFlag = true;
                //            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.Amount) + ConstantVariableService.HtmlNewLine;
                //            isValidAmount = true;
                //        }
                //        else
                //        {
                //            decimalResult = UtilityService.ConvertStringToDecimal(amount);
                //            if (decimalResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                //            {
                //                uploadItem.ErrorFlag = true;
                //                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.Amount) + ConstantVariableService.HtmlNewLine;
                //                isValidAmount = true;
                //            }
                //            else
                //            {
                //                uploadItem.Amount = UtilityService.ConvertToNumberFormat(decimalResult.ReturnResult);
                //                if (!isValidAmount)
                //                {
                //                    DateTime? datePickup = Convert.ToDateTime(pickupdate);
                //                    var submatCode = ProductDetailService.AccruedCodeSplitEmpty(matCode);
                //                    var getaccrued = accrued.Where(m => ProductDetailService.AccruedCodeSplitEmpty(m.AccruedCode) == submatCode && m.AccruedType == matType).FirstOrDefault();
                //                    if (getaccrued != null)
                //                    {
                //                        var getaccruedDetailList = accruedDetail.Where(m => m.AccruedID == getaccrued.ID
                //                        && m.WarehouseCode == warehouseCode
                //                        && m.StartDate == datePickup
                //                        && m.OrgID == uploadItem.OrgID
                //                        && m.CostCenter == uploadItem.CostCenter
                //                        && m.PricePerUnit == Convert.ToDecimal(uploadItem.PricePerUnit)
                //                        ).ToList();
                //                        if (getaccruedDetailList.Any())
                //                        {
                //                            decimal netAmount = 0;
                //                            string strdeductAmount = string.Empty;
                //                            foreach (var item in getaccruedDetailList)
                //                            {
                //                                netAmount = uploadType == ResourceText.Replace ? (decimalResult.ReturnResult) : (item.Amount + decimalResult.ReturnResult ?? 0);
                //                                strdeductAmount = UtilityService.ConvertToNumberFormat(item.DeductAmount);
                //                                if (netAmount < item.DeductAmount)
                //                                {
                //                                    uploadItem.ErrorFlag = true;
                //                                    uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_amount_Accrued, strdeductAmount) + ConstantVariableService.HtmlNewLine;
                //                                    uploadItem.MessageAt = uploadItem.MessageAt + string.Format(ValidatorMessage.incorrect_amount_Accrued_at, strdeductAmount, line.ToString()) + ConstantVariableService.HtmlNewLine;


                //                                }
                //                            }

                //                        }
                //                    }
                //                }
                //            }
                //        }

                //        line++;
                //    }
                //}

                //result.ReturnResult = uploadItemList;
            }
            catch (Exception ex)
            {
                result.ModelStateErrorList.Add(new ModelStateError("", ValidatorMessage.upload_file_content_error));
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString(), "");
            }

            return result;
        }

        public AccruedUploadViewModel InitialListGetAccrued(string matType)
        {
            AccruedUploadViewModel accruedUpload = new AccruedUploadViewModel();

            List<ValueHelpViewModel> valueHelpList = new List<ValueHelpViewModel>();

            //using (var context = new PYMFEEEntities())
            //{
            //    List<string> getMatType = new List<string>();
            //    if (matType == ConstantVariableService.AccruedTypeSupport)
            //    {
            //        getMatType = new List<string> { "Z005", "Z007" };

            //    }

            //    else if (matType == ConstantVariableService.AccruedTypeProduct)
            //    {
            //        getMatType = new List<string> { "Z001" };

            //    }
            //    var matGen = context.AccruedGens.Where(m => getMatType.Any(o => m.AccruedType == o)).OrderBy(m => m.DescriptionTH).ToList();
            //    List<Product> prdList = new List<Product>();
            //    foreach (var mat in matGen)
            //    {
            //        Product prd = new Product();
            //        MVMMappingService.MoveData(mat, prd);
            //        prd.ProductNameWithCode = string.Concat(ProductDetailService.AccruedCodeSplitEmpty(mat.AccruedCode), " ", mat.DescriptionTH);
            //        prdList.Add(prd);

            //    }
            //    var whList = context.Warehouses.Where(m => m.AccruedCategory == matType).ToList();


            //}
            return accruedUpload;
        }


    }
}