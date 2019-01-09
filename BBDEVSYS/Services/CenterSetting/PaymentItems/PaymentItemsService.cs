using BBDEVSYS.Services.Abstract;
using BBDEVSYS.ViewModels.CenterSetting.PaymentItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.Models.Shared;
using System.Web.Mvc;
using BBDEVSYS.Services.Shared;
using System.Transactions;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Content.text;

namespace BBDEVSYS.Services.CenterSetting.PaymentItems
{
    public class PaymentItemsService : AbstractControllerService<PaymentItemsViewModel>
    {
        public override PaymentItemsViewModel GetDetail(int id)
        {
            PaymentItemsViewModel pymItemsModel = NewFormData();
            //User Type
            User userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var entPymItems = (from m in context.PAYMENT_ITEMS where  m.ID == id select m).FirstOrDefault();
                    if (entPymItems != null)
                    {
                        MVMMappingService.MoveData(entPymItems, pymItemsModel);
                        pymItemsModel.LASTMODIFIED_BY = userInfo.UserCode;
                        pymItemsModel.LASTMODIFIED_DATE = DateTime.Now;


                        var entPymItemsCharge = (from n in context.PAYMENT_ITEMS_CHAGE where n.COMPANY_CODE == entPymItems.COMPANY_CODE && n.PAYMENT_ITEMS_ID == id orderby n.SEQUENCE select n).ToList();
                        foreach (var item in entPymItemsCharge)
                        {
                            PaymentItemsChargeViewModel pymChargeModel = new PaymentItemsChargeViewModel();
                            MVMMappingService.MoveData(item, pymChargeModel);
                            pymChargeModel.ChargeTypeList = ValueHelpService.GetValueHelp(ConstantVariableService.CHARGETYPE).ToList();

                           
                            pymItemsModel.pymItemsChargeList.Add(pymChargeModel);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return pymItemsModel;
        }

        public override string GetList()
        {
            throw new NotImplementedException();
        }

        public override PaymentItemsViewModel NewFormData()
        {
            PaymentItemsViewModel model = new PaymentItemsViewModel();
            try
            {
                var userInfo = UserService.GetSessionUserInfo();
                var list = new List<SelectListItem>();
                using (var context = new PYMFEEEntities())
                {
                    var getCompanyList = (from data in context.COMPANies where data.IsPaymentFee == true orderby data.Bussiness_Unit select data).ToList();
                    list.AddRange(getCompanyList.Select(com => new SelectListItem { Value = com.BAN_COMPANY.ToString(), Text = com.COMPANY_NAME_EN.ToString() }).ToList());
                    model.CompanyLst = list;

                    var pym_List = ValueHelpService.GetValueHelp(ConstantVariableService.ISACTIONTYPE);
                    model.IsActionList = pym_List;

                    var durationList = ValueHelpService.GetValueHelp(ConstantVariableService.DURATIONTYPE, ConstantVariableService.ConfigStatusActive);
                    model.DurationList = durationList;
                }
                model.CREATE_BY = userInfo.UserCode;
                model.CREATE_DATE = DateTime.Now;
                //model.LASTMODIFIED_BY = userInfo.UserCode;
                //model.LASTMODIFIED_DATE = DateTime.Now;
                model.ChannelsList = ValueHelpService.GetValueHelp("CHANNELS").ToList();


            }
            catch (Exception)
            {

                throw;
            }
            return model;
        }

        public PaymentItemsViewModel InitialListSearch()
        {
            PaymentItemsViewModel model = new PaymentItemsViewModel();
            try
            {
                model.COMPANY_CODE = "";

                var list = new List<SelectListItem>();
                using (var context = new PYMFEEEntities())
                {
                    var getCompanyList = (from data in context.COMPANies where data.IsPaymentFee == true orderby data.Bussiness_Unit select data).ToList();
                    list.AddRange(getCompanyList.Select(com => new SelectListItem { Value = com.BAN_COMPANY.ToString(), Text = com.COMPANY_NAME_EN.ToString() }).ToList());
                    model.CompanyLst = list;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return model;
        }

        public override ValidationResult SaveCreate(PaymentItemsViewModel formData, ModelStateDictionary modelState)
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
                    var entPYMItems = new PAYMENT_ITEMS();
                    //Generate Auto Code
                    GeneratePaymentItemCode(formData);
                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entPYMItems);

                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {
                        context.PAYMENT_ITEMS.Add(entPYMItems);
                        context.SaveChanges();

                        //Save Item
                        //Copy data from viewmodel to model - for line item
                        int sequence = 1;

                        foreach (var item in formData.pymItemsChargeList)
                        {
                            if (item.DeleteFlag)
                            {
                                continue;
                            }

                            var entPYMItemsCharge = new PAYMENT_ITEMS_CHAGE();
                            item.PAYMENT_ITEMS_ID = entPYMItems.ID;
                            item.SEQUENCE = sequence;
                            item.COMPANY_CODE = formData.COMPANY_CODE;
                            item.PAYMENT_ITEMS_NAME = formData.PAYMENT_ITEMS_NAME;
                            MVMMappingService.MoveData(item, entPYMItemsCharge);
                            context.PAYMENT_ITEMS_CHAGE.Add(entPYMItemsCharge);
                            context.SaveChanges();

                            sequence++;
                        }
                    }

                    scope.Complete();

                    result.Message = ResourceText.SuccessfulSave;
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

        public void GeneratePaymentItemCode(PaymentItemsViewModel formData)
        {
            try
            {
                string fee = formData.PAYMENT_ITEMS_NAME.Trim();
                string pymName = fee.Replace(" ", "").Length > 14 ? fee.Replace(" ", "").Substring(0, 15) : fee.Replace(" ", "");

                using (var context = new PYMFEEEntities())
                {
                    var entpymItemsData = (from m in context.PAYMENT_ITEMS select m).ToList();

                    var getNewpymItems = entpymItemsData.Where(m => m.COMPANY_CODE == formData.COMPANY_CODE).ToList();
                    if (getNewpymItems.Any())
                    {
                        //int seqgetNewpymItems = getNewpymItems.Count() + 1;
                        //formData.GROUP_SEQ_CHANNELS = seqgetNewpymItems;

                        formData.GROUP_SEQ_CHANNELS = getNewpymItems.Max(m => m.GROUP_SEQ_CHANNELS) + 1;
                        formData.SEQ_CHANNELS = (getNewpymItems.Where(m => m.CHANNELS == formData.CHANNELS).ToList()).Any() ?
                            getNewpymItems.Where(m => m.CHANNELS == formData.CHANNELS).FirstOrDefault().SEQ_CHANNELS :
                            getNewpymItems.ToList().GroupBy(m => m.SEQ_CHANNELS).ToList().Max(m => m.Key) + 1;


                        formData.PAYMENT_ITEMS_CODE = formData.COMPANY_CODE + pymName + DateTime.Now.Date.ToString("yyyymmdd") + "_" + formData.GROUP_SEQ_CHANNELS;
                    }
                    else
                    {

                        formData.PAYMENT_ITEMS_CODE = formData.COMPANY_CODE + pymName + DateTime.Now.Date.ToString("yyyymmdd");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string GetList(string companyCode)
        {
            try
            {
                string dataList = "";
                List<PaymentItemsViewModel> paymentitemsList = GetPaymentItemsList(companyCode);
                dataList = DatatablesService.ConvertObjectListToDatatables<PaymentItemsViewModel>(paymentitemsList);

                return dataList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<PaymentItemsViewModel> GetPaymentItemsList(string companyCode)
        {
            List<PaymentItemsViewModel> pymItemsList = new List<PaymentItemsViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var entPaymentItems = (from m in context.PAYMENT_ITEMS orderby m.COMPANY_CODE, m.GROUP_SEQ_CHANNELS select m).ToList();
                    var entCompany = (from m in context.COMPANies where m.IsPaymentFee == true select m).ToList();
                    var entUsers = (from m in context.USERS select m).ToList();
                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        entPaymentItems = entPaymentItems.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        entCompany = entCompany.Where(m => m.BAN_COMPANY == companyCode).ToList();
                    }
                    int seq = 1;
                    foreach (var item in entPaymentItems)
                    {
                        var getUsers = entUsers.Where(m => m.USERID == item.LASTMODIFIED_BY).FirstOrDefault();
                        var getCompany = entCompany.Where(m => m.BAN_COMPANY == item.COMPANY_CODE).FirstOrDefault();
                        PaymentItemsViewModel pymItemsModel = new PaymentItemsViewModel();
                        MVMMappingService.MoveData(item, pymItemsModel);
                        pymItemsModel.COMPANY_NAME = getCompany == null ? "" : getCompany.COMPANY_NAME_EN;
                        pymItemsModel.LASTMODIFIED_BY_NAME = getUsers == null ? "" : getUsers.NAME;
                        pymItemsModel.ITEM = seq;
                        pymItemsList.Add(pymItemsModel);

                        seq++;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return pymItemsList;
        }

        public override ValidationResult SaveDelete(PaymentItemsViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            result = ValidateFormData(formData, modelState);
            if (result.ErrorFlag)
            {
                return result;
            }
            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    var entPYMItems = new PAYMENT_ITEMS();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entPYMItems);

                    using (var context = new PYMFEEEntities())
                    {
                        //Delete header            
                        context.Entry(entPYMItems).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();

                        //Delete item
                        context.PAYMENT_ITEMS_CHAGE.RemoveRange(context.PAYMENT_ITEMS_CHAGE.Where(m => m.COMPANY_CODE == entPYMItems.COMPANY_CODE && m.PAYMENT_ITEMS_ID == entPYMItems.ID));
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

        public override ValidationResult SaveEdit(PaymentItemsViewModel formData, ModelStateDictionary modelState)
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
                    PAYMENT_ITEMS entPYMItems = new PAYMENT_ITEMS();



                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entPYMItems);

                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {
                        context.Entry(entPYMItems).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();


                        #region Update any Table
                        //Update 

                        //#region //FEE_ACCRUED_PLAN_ITEM
                        //var entaccr = context.FEE_ACCRUED_PLAN_ITEM.Where(m => m.PAYMENT_ITEMS_NAME == entPYMItems.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == entPYMItems.COMPANY_CODE).ToList();
                        //foreach (var accr in entaccr)
                        //{
                        //    FEE_ACCRUED_PLAN_ITEM ent_accr_item = new FEE_ACCRUED_PLAN_ITEM();
                        //    ent_accr_item = context.FEE_ACCRUED_PLAN_ITEM.Find(accr.ACCRUED_ITEM_ID) ;
                        //    ent_accr_item.PAYMENT_ITEMS_NAME = entPYMItems.PAYMENT_ITEMS_NAME;
                        //    ent_accr_item.CCT_CODE = entPYMItems.CCT_CODE;
                        //    ent_accr_item.COST_CENTER = entPYMItems.COST_CENTER;
                        //    ent_accr_item.FUND_CODE = entPYMItems.FUND_CODE;
                        //    ent_accr_item.GL_ACCOUNT = entPYMItems.GL_ACCOUNT;
                        //    ent_accr_item.CHANNELS = entPYMItems.CHANNELS;
                        //    context.FEE_ACCRUED_PLAN_ITEM.Attach(ent_accr_item);
                        //    context.Entry(ent_accr_item).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();

                        //}
                        //#endregion
                        //#region//FEE_ACCRUED_PLAN_ITEM_SUB
                        //var entaccrsub = context.FEE_ACCRUED_PLAN_ITEM_SUB.Where(m => m.PAYMENT_ITEMS_NAME == entPYMItems.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == entPYMItems.COMPANY_CODE).ToList();
                        //foreach (var accr in entaccrsub)
                        //{
                        //    FEE_ACCRUED_PLAN_ITEM_SUB ent_accr_item = new FEE_ACCRUED_PLAN_ITEM_SUB();
                        //    ent_accr_item = context.FEE_ACCRUED_PLAN_ITEM_SUB.Find(accr.ID);
                        //    ent_accr_item.PAYMENT_ITEMS_NAME = entPYMItems.PAYMENT_ITEMS_NAME;
                        //    ent_accr_item.CCT_CODE = entPYMItems.CCT_CODE;
                        //    ent_accr_item.COST_CENTER = entPYMItems.COST_CENTER;
                        //    ent_accr_item.FUND_CODE = entPYMItems.FUND_CODE;
                        //    ent_accr_item.GL_ACCOUNT = entPYMItems.GL_ACCOUNT;
                        //    ent_accr_item.CHANNELS = entPYMItems.CHANNELS;
                        //    context.FEE_ACCRUED_PLAN_ITEM_SUB.Attach(ent_accr_item);
                        //    context.Entry(ent_accr_item).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();

                        //}
                        //#endregion
                        //#region //FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG
                        //var entaccrup = context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG.Where(m => m.FEE == entPYMItems.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == entPYMItems.COMPANY_CODE).ToList();
                        //foreach (var accr in entaccrup)
                        //{
                        //    FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG ent_accr_item = new FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG();
                        //    ent_accr_item = context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG.Find(accr.ID);
                        //    ent_accr_item.FEE = entPYMItems.PAYMENT_ITEMS_NAME;
                        //    ent_accr_item.CCT_CODE = entPYMItems.CCT_CODE;
                        //    ent_accr_item.FUND_CODE = entPYMItems.FUND_CODE;
                        //    ent_accr_item.GL_ACCOUNT = entPYMItems.GL_ACCOUNT;
                        //    context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG.Attach(ent_accr_item);
                        //    context.Entry(ent_accr_item).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();

                        //}
                        //#endregion
                        //#region//FEE_INVOICE
                        //var entinv = context.FEE_INVOICE.Where(m => m.PAYMENT_ITEMS_NAME == entPYMItems.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == entPYMItems.COMPANY_CODE).ToList();
                        //foreach (var accr in entinv)
                        //{
                        //    FEE_INVOICE ent_accr_item = new FEE_INVOICE();
                        //    ent_accr_item = context.FEE_INVOICE.Find(accr.ID);
                        //    ent_accr_item.PAYMENT_ITEMS_NAME = entPYMItems.PAYMENT_ITEMS_NAME;
                        //    ent_accr_item.CCT_CODE = entPYMItems.CCT_CODE;
                        //    ent_accr_item.COST_CENTER = entPYMItems.COST_CENTER;
                        //    ent_accr_item.FUND_CODE = entPYMItems.FUND_CODE;
                        //    ent_accr_item.GL_ACCOUNT = entPYMItems.GL_ACCOUNT;
                        //    ent_accr_item.CHANNELS = entPYMItems.CHANNELS;
                        //    context.FEE_INVOICE.Attach(ent_accr_item);
                        //    context.Entry(ent_accr_item).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();

                        //}
                        //#endregion
                        //#region //FEE_INVOICE_ITEM
                        //var entinvitem = context.FEE_INVOICE_ITEM.Where(m => m.PAYMENT_ITEMS_NAME == entPYMItems.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == entPYMItems.COMPANY_CODE).ToList();
                        //foreach (var accr in entinvitem)
                        //{
                        //    FEE_INVOICE_ITEM ent_accr_item = new FEE_INVOICE_ITEM();
                        //    ent_accr_item = context.FEE_INVOICE_ITEM.Find(accr.INV_ITEM_NO);
                        //    ent_accr_item.PAYMENT_ITEMS_NAME = entPYMItems.PAYMENT_ITEMS_NAME;
                        //    ent_accr_item.COST_CENTER = entPYMItems.COST_CENTER;
                        //    ent_accr_item.FUND_CODE = entPYMItems.FUND_CODE;
                        //    ent_accr_item.GL_ACCOUNT = entPYMItems.GL_ACCOUNT;
                        //    ent_accr_item.CHANNELS = entPYMItems.CHANNELS;
                        //    context.FEE_INVOICE_ITEM.Attach(ent_accr_item);
                        //    context.Entry(ent_accr_item).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();

                        //}
                        //#endregion
                        //#region//FEE_INVOICE_UPLOAD_ITEM_LOG
                        //var entinvtup = context.FEE_INVOICE_UPLOAD_ITEM_LOG.Where(m => m.FEE == entPYMItems.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == entPYMItems.COMPANY_CODE).ToList();
                        //foreach (var accr in entinvtup)
                        //{
                        //    FEE_INVOICE_UPLOAD_ITEM_LOG ent_accr_item = new FEE_INVOICE_UPLOAD_ITEM_LOG();
                        //    ent_accr_item = context.FEE_INVOICE_UPLOAD_ITEM_LOG.Find(accr.ID);
                        //    ent_accr_item.FEE = entPYMItems.PAYMENT_ITEMS_NAME;
                        //    ent_accr_item.CCT_CODE = entPYMItems.CCT_CODE;
                        //    ent_accr_item.FUND_CODE = entPYMItems.FUND_CODE;
                        //    ent_accr_item.GL_ACCOUNT = entPYMItems.GL_ACCOUNT;
                        //    ent_accr_item.CHANNELS = entPYMItems.CHANNELS;
                        //    context.FEE_INVOICE_UPLOAD_ITEM_LOG.Attach(ent_accr_item);
                        //    context.Entry(ent_accr_item).State = System.Data.Entity.EntityState.Modified;
                        //    context.SaveChanges();

                        //}
                        //#endregion

                        #endregion
                        //Save Item                        
                        int sequence = 1;

                        foreach (var item in formData.pymItemsChargeList)
                        {
                            var entPYMItemsCharge = new PAYMENT_ITEMS_CHAGE();
                            item.PAYMENT_ITEMS_ID = entPYMItems.ID;
                            item.SEQUENCE = sequence;
                            item.COMPANY_CODE = formData.COMPANY_CODE;
                            item.PAYMENT_ITEMS_NAME = formData.PAYMENT_ITEMS_NAME;
                            MVMMappingService.MoveData(item, entPYMItemsCharge);
                            if (item.DeleteFlag)
                            {
                                if (entPYMItemsCharge.ID != 0)
                                {
                                    //Delete item
                                    context.Entry(entPYMItemsCharge).State = System.Data.Entity.EntityState.Deleted;
                                    context.SaveChanges();

                                }
                            }
                            else
                            {
                                entPYMItemsCharge.SEQUENCE = sequence;
                                sequence++;
                                if (entPYMItemsCharge.ID != 0)
                                {
                                    context.Entry(entPYMItemsCharge).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    context.PAYMENT_ITEMS_CHAGE.Add(entPYMItemsCharge);
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

        public PaymentItemsChargeViewModel InitialItem(int sequence)//, int pymID, string companyCode)
        {
            PaymentItemsChargeViewModel pymItems = new PaymentItemsChargeViewModel();

            try
            {
                int seq = sequence;
                pymItems.SEQUENCE = seq;
                pymItems.ChargeTypeList = ValueHelpService.GetValueHelp("CHARGE_TYPE").ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return pymItems;
        }

        public override ValidationResult ValidateFormData(PaymentItemsViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            try
            {
                #region mark delete
                //if (string.Equals(formData.FormAction, ConstantVariableService.FormActionDelete, StringComparison.OrdinalIgnoreCase))
                //{
                //    using (var context = new PYMFEEEntities())
                //    {


                //        //FEE_ACCRUED_PLAN_ITEM
                //        var fund_inAccr = (from m in context.FEE_ACCRUED_PLAN_ITEM where m.PAYMENT_ITEMS_NAME == formData.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == m.COMPANY_CODE select m).ToList();
                //        //FEE_ACCRUED_PLAN_ITEM_SUB
                //        var fund_inAccrSub = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB where m.PAYMENT_ITEMS_NAME == formData.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == m.COMPANY_CODE select m).ToList();
                //        //FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG
                //        var fund_inAccrUpload = (from m in context.FEE_ACCRUED_PLAN_UPLOAD_ITEM_LOG where m.FEE == formData.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == formData.COMPANY_CODE select m).ToList();
                //        //FEE_INVOICE
                //        var fund_inInv = (from m in context.FEE_INVOICE where m.PAYMENT_ITEMS_NAME == formData.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == formData.COMPANY_CODE select m).ToList();
                //        //FEE_INVOICE_ITEM
                //        var fund_inInvitem = (from m in context.FEE_INVOICE_ITEM where m.PAYMENT_ITEMS_NAME == formData.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == formData.COMPANY_CODE select m).ToList();
                //        //FEE_INVOICE_UPLOAD_ITEM_LOG
                //        var fund_inInvupload = (from m in context.FEE_INVOICE_UPLOAD_ITEM_LOG where m.FEE == formData.PAYMENT_ITEMS_NAME && m.COMPANY_CODE == formData.COMPANY_CODE select m).ToList();

                //        if (
                //            fund_inAccr.Any() || fund_inAccrSub.Any() || fund_inAccrUpload.Any() || fund_inInv.Any() || fund_inInvitem.Any() || fund_inInvupload.Any()
                //            )
                //        {
                //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.cannot_request_delete, formData.PAYMENT_ITEMS_NAME)));
                //            result.ErrorFlag = true;
                //        }
                //    }
                //}
                //else
                //{ }
                #endregion
                if (!modelState.IsValid)
                {
                    result.ModelStateErrorList = UtilityService.GetModelStateErrors(modelState);
                    result.ErrorFlag = true;
                }
                else
                {
                    //Get item => deleteFlag != true
                    var itemList = formData.pymItemsChargeList.Where(m => m.DeleteFlag != true).ToList();
                    if (itemList == null || !itemList.Any()) //Check list is null or empty
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_listnull_error, ResourceText.TitlePaymentItemList)));

                        result.ErrorFlag = true;
                    }
                    else
                    {

                        //Check item
                        int line = 1;
                        foreach (var item in itemList)
                        {

                            //charge id not empty
                            if (item.PAYMENT_ITEMS_FEE_NAME == null)
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.PAYMENT_ITEMS_FEE_ITEM, line.ToString())));

                                result.ErrorFlag = true;
                            }
                            else
                            {
                                item.PAYMENT_ITEMS_FEE_NAME = item.PAYMENT_ITEMS_FEE_NAME.Trim();
                            }
                            //charge id not empty
                            if (item.CHARGE_TYPE == null)
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.Charge, line.ToString())));

                                result.ErrorFlag = true;
                            }

                            var dupPaymentItem = itemList.GroupBy(m => string.IsNullOrEmpty(m.PAYMENT_ITEMS_FEE_NAME) ? m.PAYMENT_ITEMS_FEE_NAME : m.PAYMENT_ITEMS_FEE_NAME.Trim()).Where(m => m.Count() > 1).ToList();
                            foreach (var itemDup in dupPaymentItem)
                            {
                                string payment_name = string.IsNullOrEmpty(item.PAYMENT_ITEMS_FEE_NAME) ? item.PAYMENT_ITEMS_FEE_NAME : item.PAYMENT_ITEMS_FEE_NAME.Trim();
                                //--wainting test upload invoice
                                if (payment_name == itemDup.Key)
                                {
                                    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_duplicate_error, ResourceText.PAYMENT_ITEMS_FEE_ITEM + " " + itemDup.Key, line.ToString())));
                                    result.ErrorFlag = true;
                                }
                            }

                            line++;
                        }
                        using (var context = new PYMFEEEntities())
                        {


                            //Dup In Data Invoice
                            if (formData.FormAction == ConstantVariableService.FormActionCreate || formData.FormAction == ConstantVariableService.FormActionCopy)
                            {
                                var pymItems = (from m in context.PAYMENT_ITEMS where m.COMPANY_CODE == formData.COMPANY_CODE && m.PAYMENT_ITEMS_NAME == formData.PAYMENT_ITEMS_NAME.Trim() select m).ToList();
                                if (pymItems.Any())
                                {
                                    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, ResourceText.PAYMENT_ITEMS_CODE + " " + formData.PAYMENT_ITEMS_NAME)));
                                    result.ErrorFlag = true;
                                }
                            }
                            //Dup In Data Invoice
                            if (formData.FormAction == ConstantVariableService.FormActionEdit)
                            {
                                var pymItems = (from m in context.PAYMENT_ITEMS where m.COMPANY_CODE == formData.COMPANY_CODE && m.PAYMENT_ITEMS_NAME == formData.PAYMENT_ITEMS_NAME.Trim() && m.ID != formData.ID select m).ToList();
                                if (pymItems.Any())
                                {
                                    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, ResourceText.PAYMENT_ITEMS_CODE + " " + formData.PAYMENT_ITEMS_NAME)));
                                    result.ErrorFlag = true;
                                }
                            }

                        }
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }
    }
}