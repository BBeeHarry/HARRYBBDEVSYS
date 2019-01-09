using BBDEVSYS.Services.Abstract;
using BBDEVSYS.ViewModels.InvoiceUpload;
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
using BBDEVSYS.Services.Invoice;
using BBDEVSYS.ViewModels.Invoice;
using BBDEVSYS.ViewModels.CenterSetting.PaymentItems;
using BBDEVSYS.Services.CenterSetting.PaymentItems;

namespace BBDEVSYS.Services.Invoice
{
    public class InvoiceUploadService : AbstractControllerService<InvoiceUploadViewModel>
    {
        public override InvoiceUploadViewModel GetDetail(int id)
        {
            InvoiceUploadViewModel InvoiceUpload = NewFormData();
            var userInfo = UserService.GetSessionUserInfo();
            try
            {
                //User user = UserService.GetSessionUserInfo();
                using (var context = new PYMFEEEntities())
                {
                    FEE_INVOICE_UPLOAD entInvoiceUpload = new FEE_INVOICE_UPLOAD();
                    entInvoiceUpload = (from data in context.FEE_INVOICE_UPLOAD
                                        where data.ID == id
                                        orderby data.ID
                                        select data).FirstOrDefault();
                    MVMMappingService.MoveData(entInvoiceUpload, InvoiceUpload);



                    //Get line item
                    var InvoiceUploadItemList = (from m in context.FEE_INVOICE_UPLOAD_ITEM_LOG
                                                 where m.INV_UPLOAD_ID == id
                                                 select m).OrderBy(m => m.ID).ToList();


                    foreach (var item in InvoiceUploadItemList)
                    {
                        InvoiceUploadItemViewModel InvoiceUploadItem = new InvoiceUploadItemViewModel();
                        MVMMappingService.MoveData(item, InvoiceUploadItem);

                        InvoiceUpload.InvoiceUploadItemList.Add(InvoiceUploadItem);
                        //Get line item

                    }

                }
                //Get attachment
                AttachmentViewModel getAttachmentOption = new AttachmentViewModel();
                getAttachmentOption.ProcessCode = InvoiceUploadViewModel.ProcessCode;
                //getAttachmentOption.DataID = id;
                getAttachmentOption.DataKey = id.ToString();
                getAttachmentOption.DocumentFilePath = AttachmentService.GetDocumentFilePath(InvoiceUploadViewModel.ProcessCode);

                InvoiceUpload.AttachmentList = AttachmentService.GetAttachmentList(getAttachmentOption);
            }
            catch (Exception ex)
            {

            }
            return InvoiceUpload;
        }

        public InvoiceUploadViewModel InitialSearch()
        {
            InvoiceUploadViewModel Invoice = new InvoiceUploadViewModel();
            var context = new PYMFEEEntities();
            try
            {
                //Invoice.InvoiceTypeValueHelp = ValueHelpService.GetValueHelp("InvoiceTYPE").OrderBy(m => m.ValueKey).ToList();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Invoice;
        }

        //public List<InvoiceUploadItemViewModel> GetUploadItemHeader(int month, int year)
        //{
        //    List<InvoiceUploadItemViewModel> columnList = new List<InvoiceUploadItemViewModel>();

        //    try
        //    {
        //        if (jobUploadGroupID == 0 || string.IsNullOrEmpty(keyDate))
        //        {
        //            JobUploadShopProductViewModel column = new JobUploadShopProductViewModel();

        //            column.ColumnNo = 1;
        //            column.ColumnName = ResourceText.MaterialCode;

        //            columnList.Add(column);

        //            return columnList;
        //        }

        //        var keyDateVal = DateTime.ParseExact(keyDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
        //        List<JobUploadGroupItem> groupItemList = new List<JobUploadGroupItem>();
        //        List<ProductDetailItemViewModel> productValueHelp = new List<ProductDetailItemViewModel>();

        //        using (var context = new BBDEVSYSDB())
        //        {
        //            groupItemList = (from m in context.JobUploadGroupItems where m.JobUploadGroupID == jobUploadGroupID select m).OrderBy(m => m.Sequence).ToList();

        //            productValueHelp = ProductDetailService.GetProductDetailItemList(keyDateVal);
        //        }

        //        foreach (var item in groupItemList)
        //        {
        //            JobUploadShopProductViewModel column = new JobUploadShopProductViewModel();

        //            var productDetail = productValueHelp.Where(m => m.MaterialCode == item.MaterialCode).FirstOrDefault();

        //            if (productDetail != null)
        //            {
        //                column.ColumnNo = item.Sequence;

        //                MVMMappingService.MoveData(productDetail, column);

        //                column.ColumnName = column.MaterialCodeName + ConstantVariableService.HtmlNewLine + productDetail.MeanPriceText;

        //                columnList.Add(column);
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return columnList;
        //}

        public override string GetList()
        {
            string dataList = "";
            List<InvoiceUploadViewModel> InvoiceList = GetInvoiceUploadList();

            dataList = DatatablesService.ConvertObjectListToDatatables<InvoiceUploadViewModel>(InvoiceList);

            return dataList;
        }


        public List<InvoiceUploadViewModel> GetInvoiceUploadList()
        {
            List<InvoiceUploadViewModel> InvoiceUploadList = new List<InvoiceUploadViewModel>();
            InvoiceUploadViewModel entInvoiceUpload = NewFormData();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var InvoiceUpload = (from m in context.FEE_INVOICE_UPLOAD
                                         orderby m.ID
                                         select m).ToList();

                    var company = (from m in context.COMPANies where m.IsPaymentFee == true select m).ToList();
                    var users = (from m in context.USERS select m).ToList();
                    foreach (var item in InvoiceUpload)
                    {
                        var InvoiceUploadModel = new InvoiceUploadViewModel();
                        MVMMappingService.MoveData(item, InvoiceUploadModel);
                        var userName = users.Where(m => m.USERID == item.UPLOAD_BY).FirstOrDefault();
                        InvoiceUploadModel.UploadByName = userName == null ? "" : userName.NAME;
                        InvoiceUploadModel.MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(item.INV_MONTH));
                        //Get attachment
                        AttachmentViewModel getAttachmentOption = new AttachmentViewModel();
                        getAttachmentOption.ProcessCode = InvoiceUploadViewModel.ProcessCode;
                        //getAttachmentOption.DataID = id;
                        getAttachmentOption.DataKey = item.ID.ToString();
                        getAttachmentOption.DocumentFilePath = AttachmentService.GetDocumentFilePath(InvoiceUploadViewModel.ProcessCode);

                        InvoiceUploadModel.AttachmentList = AttachmentService.GetAttachmentList(getAttachmentOption);

                        InvoiceUploadModel.FileName = InvoiceUploadModel.AttachmentList.FirstOrDefault().FileName;
                        InvoiceUploadList.Add(InvoiceUploadModel);



                    }
                }

            }
            catch (Exception ex)
            {

            }
            return InvoiceUploadList;
        }


        public override InvoiceUploadViewModel NewFormData()
        {
            InvoiceUploadViewModel InvoiceUpload = new InvoiceUploadViewModel();
            var userInfo = UserService.GetSessionUserInfo();
            InvoiceUpload.INV_MONTH = DateTime.Today.Month;
            InvoiceUpload.INV_YEAR = DateTime.Today.Year;
            InvoiceUpload.COMPANY_CODE = "";
            InvoiceUpload.UPLOAD_BY = userInfo.UserCode;

            var list = new List<SelectListItem>();
            using (var context = new PYMFEEEntities())
            {
                var getCompanyList = (from data in context.COMPANies where data.IsPaymentFee == true orderby data.Bussiness_Unit select data).ToList();
                list.AddRange(getCompanyList.Select(com => new SelectListItem { Value = com.BAN_COMPANY.ToString(), Text = com.COMPANY_NAME_EN.ToString() }).ToList());
                InvoiceUpload.CompanyLst = list;

                var userApp = (from m in context.USERS where m.GROUP_POSITION == "Approved" select m).ToList();
                InvoiceUpload.UserApprovedList = userApp.Select(m => new SelectListItem
                {
                    Value = m.USERID,
                    Text = m.USERID + " " + m.NAME
                }).ToList();

            }
            //--Get List Month
            list = new List<SelectListItem>();
            list.AddRange(DateTimeFormatInfo
                  .InvariantInfo
                  .MonthNames
                  .Where(m => !String.IsNullOrEmpty(m))
                  .Select((monthName, index) => new SelectListItem
                  {
                      Value = (index + 1).ToString(),
                      Text = monthName
                  }).ToList());
            InvoiceUpload.MonthLst = list;


            //--Get List Years
            list = new List<SelectListItem>();
            list.AddRange(Enumerable.Range(DateTime.Today.Year - 30, 200)
                  .Select(YearAD => new SelectListItem
                  {
                      Value = (YearAD).ToString(),
                      Text = (YearAD).ToString()
                  }).ToList());

            InvoiceUpload.YearLst = list;


            return InvoiceUpload;
        }

        public override ValidationResult SaveCreate(InvoiceUploadViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            InvoiceService service = new InvoiceService();
            var InvoiceList = new List<Models.Entities.FEE_INVOICE>();
            var InvoiceDetailList = new List<Models.Entities.FEE_INVOICE_ITEM>();
            try
            {
                var convertResult = ConvertUploadItemToFormData(formData);

                if (convertResult.ErrorFlag)
                {
                    result.ErrorFlag = true;
                    result.ModelStateErrorList = convertResult.ModelStateErrorList;

                    return result;
                }


                formData = convertResult.ReturnResult;

                using (TransactionScope scope = new TransactionScope())
                {
                    var entInvoiceUpload = new FEE_INVOICE_UPLOAD();


                    //entInvoiceUpload.

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entInvoiceUpload);

                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {
                        context.FEE_INVOICE_UPLOAD.Add(entInvoiceUpload);
                        context.SaveChanges();

                        formData.ID = entInvoiceUpload.ID;

                        //Save Item
                        //Copy data from viewmodel to model - for line item
                        InvoiceList = (from s in context.FEE_INVOICE
                                       select s).ToList();
                        InvoiceDetailList = (from t in context.FEE_INVOICE_ITEM
                                             select t).ToList();
                        int sequence = 1;

                        foreach (var item in formData.InvoiceUploadItemList)
                        {

                            if (item.DeleteFlag)
                            {
                                continue;
                            }

                            var entInvoiceUploadItem = new FEE_INVOICE_UPLOAD_ITEM_LOG();
                            item.INV_UPLOAD_ID = entInvoiceUpload.ID;
                            item.SEQUENCE = sequence;
                            MVMMappingService.MoveData(item, entInvoiceUploadItem);
                            context.FEE_INVOICE_UPLOAD_ITEM_LOG.Add(entInvoiceUploadItem);
                            context.SaveChanges();
                            sequence++;
                        }
                        //end foreach item invoice upload
                        //Generate Create Invoice
                        var invUpload = formData.InvoiceUploadItemList.GroupBy(m => new { m.INV_NO, m.INV_MONTH, m.INV_YEAR }).ToList();

                        var invModelList = new List<InvoiceViewModel>();
                        foreach (var inv in invUpload)
                        {
                            var entInvList = formData.InvoiceUploadItemList.Where(m => m.INV_NO == inv.Key.INV_NO && m.INV_MONTH == inv.Key.INV_MONTH && m.INV_YEAR == inv.Key.INV_YEAR).ToList();

                            var invModel = new InvoiceViewModel();

                            var getentInvList = entInvList.FirstOrDefault();

                            decimal net_amount = 0;
                            decimal deduct_amount = 0;
                            decimal incl_vat_amount = 0;
                            //decimal total_charge_amount = 0;

                            foreach (var item in entInvList)
                            {
                                if ( ConvertStrToDec(item.RATE_TRANS )==0)
                                {
                                    net_amount += ConvertStrToDec(item.RATE_AMT) == 0 ? ConvertStrToDec(item.ACTUAL_AMOUNT) 
                                        : ConvertStrToDec(item.ACTUAL_AMOUNT) * (ConvertStrToDec(item.RATE_AMT) / 100);

                                }
                                else
                                {
                                    net_amount += ConvertStrToDec(item.RATE_AMT) == 0 ? ConvertStrToDec(item.TRANSACTIONS) * ConvertStrToDec(item.RATE_TRANS) + ConvertStrToDec(item.ACTUAL_AMOUNT) 
                                        : ConvertStrToDec(item.TRANSACTIONS) * ConvertStrToDec(item.RATE_TRANS) + ConvertStrToDec(item.ACTUAL_AMOUNT) * (ConvertStrToDec(item.RATE_AMT) / 100);
                                }

                          
                            }

                            incl_vat_amount = net_amount * (1 + (ConvertStrToDec(getentInvList.VAT) / 100));
                            deduct_amount = incl_vat_amount - (net_amount * (ConvertStrToDec(getentInvList.WHT) / 100));




                            var entpymItemsData = (from m in context.PAYMENT_ITEMS select m).ToList();

                            var entpymItems = entpymItemsData.Where(m => m.PAYMENT_ITEMS_NAME == getentInvList.FEE.Trim()
                            && m.COMPANY_CODE == getentInvList.COMPANY_CODE
                            ).ToList();
                            var pymItemsModel = new PaymentItemsViewModel();
                            var pymItemsService = new PaymentItemsService();

                            


                            if (entpymItems.Any())
                            {
                                getentInvList.PAYMENT_ITEMS_CODE = entpymItems.FirstOrDefault().PAYMENT_ITEMS_CODE;
                                getentInvList.CHANNELS= entpymItems.FirstOrDefault().CHANNELS;
                                getentInvList.GL_ACCOUNT=entpymItems.FirstOrDefault().GL_ACCOUNT;
                                getentInvList.CCT_CODE=entpymItems.FirstOrDefault().COST_CENTER;
                                getentInvList.FUND_CODE=entpymItems.FirstOrDefault().FUND_CODE;
                            }
                            else
                            {
                                //not had data price catalog 
                                break;

                            }

                            if (getentInvList != null)
                            {
                                invModel.INV_NO = getentInvList.INV_NO;
                                invModel.PRO_NO = getentInvList.PRO_NO;
                                invModel.CCT_CODE = getentInvList.CCT_CODE;
                                invModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
                                invModel.INV_MONTH = ConvertStrToInt32(getentInvList.INV_MONTH);
                                invModel.INV_YEAR = ConvertStrToInt32(getentInvList.INV_YEAR);
                                invModel.PAYMENT_ITEMS_CODE = getentInvList.PAYMENT_ITEMS_CODE;
                                invModel.INV_REC_BY = getentInvList.INV_REC_BY;
                                invModel.INV_REC_DATE = ConvertStrParseDateTime(getentInvList.INV_REC_DATE);
                                invModel.INV_APPROVED_BY = getentInvList.INV_APPROVED_BY;
                                invModel.INV_APPROVED_DATE = ConvertStrParseDateTime(getentInvList.INV_APPROVED_DATE);
                                invModel.INV_DUE_DATE = ConvertStrParseDateTime(getentInvList.INV_DUE_DATE);
                                invModel.MODIFIED_BY = getentInvList.MODIFIED_BY;
                                invModel.MODIFIED_DATE = getentInvList.MODIFIED_DATE;
                                invModel.CREATE_BY = getentInvList.INV_REC_BY;
                                invModel.CREATE_DATE = DateTime.Now;
                                invModel.IS_STATUS = getentInvList.IS_STATUS;
                                invModel.CONDITION_PYM = getentInvList.CONDITION_PYM;
                                invModel.NET_AMOUNT = net_amount;// ConvertStrToDec(getentInvList.NET_AMOUNT);
                                invModel.REMARK = getentInvList.REMARK;
                                invModel.WHT = ConvertStrToDec(getentInvList.WHT);
                                invModel.DEDUCT_WHT_AMOUNT = deduct_amount;// ConvertStrToDec(getentInvList.DEDUCT_WHT_AMOUNT);
                                invModel.CURRENCY = getentInvList.CURRENCY;
                                invModel.INCLUDE_VAT_AMOUNT = incl_vat_amount;// ConvertStrToDec(getentInvList.INCLUDE_VAT_AMOUNT);
                                invModel.VAT = ConvertStrToDec(getentInvList.VAT);
                                invModel.PRO_REC_DATE = ConvertStrParseDateTime(getentInvList.PRO_REC_DATE);
                                invModel.DISCOUNT = ConvertStrToDec(getentInvList.DISCOUNT);
                                invModel.PRO_DUE_DATE = ConvertStrParseDateTime(getentInvList.PRO_DUE_DATE);
                                invModel.CHANNELS = getentInvList.CHANNELS;
                                invModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
                                invModel.COST_CENTER = getentInvList.CCT_CODE;
                                invModel.FUND_CODE = getentInvList.FUND_CODE;
                                invModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
                                invModel.FormAction = ConstantVariableService.FormActionCreate;
                                invModel.UPLOAD_TYPE = true;
                            }
                            int line = 1;
                            foreach (var ent in entInvList)
                            {
                                var invItemModel = new InvoiceDetailViewModel();
                                //MVMMappingService.MoveData(ent, invModel);
                                invItemModel.INV_NO = ent.INV_NO;
                                invItemModel.PAYMENT_ITEMS_FEE_ITEM = ent.CHARGED;
                                invItemModel.INV_ITEM_NO = ent.INV_NO + "_" + line;
                                invItemModel.SEQUENCE = line;
                                invItemModel.TRANSACTIONS = ConvertStrToDec(ent.TRANSACTIONS);
                                invItemModel.ACTUAL_AMOUNT = ConvertStrToDec(ent.ACTUAL_AMOUNT);
                                //invItemModel.TOTAL_CHARGE_AMOUNT = ConvertStrToDec(ent.TOTAL_CHARGE_AMOUNT);
                                if (ConvertStrToDec(ent.RATE_TRANS) == 0)
                                {
                                    invItemModel.TOTAL_CHARGE_AMOUNT = ConvertStrToDec(ent.RATE_AMT) == 0 ? ConvertStrToDec(ent.ACTUAL_AMOUNT)
                                        : ConvertStrToDec(ent.ACTUAL_AMOUNT) * (ConvertStrToDec(ent.RATE_AMT) / 100);

                                }
                                else
                                {
                                    invItemModel.TOTAL_CHARGE_AMOUNT = ConvertStrToDec(ent.RATE_AMT) == 0 ? ConvertStrToDec(ent.TRANSACTIONS) * ConvertStrToDec(ent.RATE_TRANS) + ConvertStrToDec(ent.ACTUAL_AMOUNT)
                                        : ConvertStrToDec(ent.TRANSACTIONS) * ConvertStrToDec(ent.RATE_TRANS) + ConvertStrToDec(ent.ACTUAL_AMOUNT) * (ConvertStrToDec(ent.RATE_AMT) / 100);
                                }
                                invItemModel.RATE_TRANS = ConvertStrToDec(ent.RATE_TRANS);
                                invItemModel.RATE_AMT = ConvertStrToDec(ent.RATE_AMT);
                                invItemModel.MODIFIED_BY = ent.MODIFIED_BY;
                                invItemModel.MODIFIED_DATE = ent.MODIFIED_DATE;
                                invItemModel.CREATE_BY = ent.CREATE_BY;
                                invItemModel.CREATE_DATE = ent.CREATE_DATE;
                                invItemModel.REMARK = ent.REMARK;
                                invItemModel.INV_MONTH = ConvertStrToInt32(getentInvList.INV_MONTH);
                                invItemModel.INV_YEAR = ConvertStrToInt32(getentInvList.INV_YEAR);
                                invItemModel.CHANNELS = getentInvList.CHANNELS;
                                invItemModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
                                invItemModel.COST_CENTER = getentInvList.CCT_CODE;
                                invItemModel.FUND_CODE = getentInvList.FUND_CODE;
                                invItemModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
                                invItemModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
                                invItemModel.PAYMENT_ITEMS_CODE = getentInvList.PAYMENT_ITEMS_CODE;

                                //MVMMappingService.MoveData(ent, invItemModel);
                                invModel.InvoiceDetailList.Add(invItemModel);
                                line++;
                            }
                            result = service.SaveCreate(invModel, modelState);
                            if (result.ErrorFlag)
                            {
                                result.ErrorFlag = true;
                                result.ModelStateErrorList = result.ModelStateErrorList;

                                return result;
                            }
                        }



                    }


                    scope.Complete();

                    //result.Message = ResourceText.SuccessfulSave;
                    //result.MessageType = "S";

                }
                //Manage attachment
                if (formData.ID != 0)
                {
                    var docFilePath = AttachmentService.GetDocumentFilePath(InvoiceUploadViewModel.ProcessCode);
                    foreach (var attachment in formData.AttachmentList)
                    {
                        attachment.ProcessCode = InvoiceUploadViewModel.ProcessCode;
                        //attachment.DataID = formData.ID;
                        attachment.DataKey = formData.ID.ToString();
                        attachment.DocumentFilePath = docFilePath;
                    }
                    AttachmentService.ManageAttachment(formData.AttachmentList);
                }

                result.Message = ResourceText.SuccessfulUpload;//+ " (" + ResourceText.DocumentNo + ": " + formData.JobDocNo + ")";
                result.MessageType = "S";
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
                result.ErrorFlag = true;
            }

            return result;
        }

        //public override ValidationResult SaveCreate(InvoiceUploadViewModel formData, ModelStateDictionary modelState)
        //{
        //    ValidationResult result = new ValidationResult();
        //    InvoiceService service = new InvoiceService();
        //    var InvoiceList = new List<Models.Entities.FEE_INVOICE>();
        //    var InvoiceDetailList = new List<Models.Entities.FEE_INVOICE_ITEM>();
        //    try
        //    {
        //        var convertResult = ConvertUploadItemToFormData(formData);

        //        if (convertResult.ErrorFlag)
        //        {
        //            result.ErrorFlag = true;
        //            result.ModelStateErrorList = convertResult.ModelStateErrorList;

        //            return result;
        //        }


        //        formData = convertResult.ReturnResult;

        //        using (TransactionScope scope = new TransactionScope())
        //        {
        //            var entInvoiceUpload = new FEE_INVOICE_UPLOAD();


        //            //entInvoiceUpload.

        //            //Copy data from viewmodel to model - for header
        //            MVMMappingService.MoveData(formData, entInvoiceUpload);

        //            //Save Header 
        //            using (var context = new PYMFEEEntities())
        //            {
        //                context.FEE_INVOICE_UPLOAD.Add(entInvoiceUpload);
        //                context.SaveChanges();

        //                formData.ID = entInvoiceUpload.ID;

        //                //Save Item
        //                //Copy data from viewmodel to model - for line item
        //                InvoiceList = (from s in context.FEE_INVOICE
        //                               select s).ToList();
        //                InvoiceDetailList = (from t in context.FEE_INVOICE_ITEM
        //                                     select t).ToList();
        //                int sequence = 1;

        //                foreach (var item in formData.InvoiceUploadItemList)
        //                {

        //                    if (item.DeleteFlag)
        //                    {
        //                        continue;
        //                    }

        //                    var entInvoiceUploadItem = new FEE_INVOICE_UPLOAD_ITEM_LOG();
        //                    item.INV_UPLOAD_ID = entInvoiceUpload.ID;
        //                    item.SEQUENCE = sequence;
        //                    MVMMappingService.MoveData(item, entInvoiceUploadItem);
        //                    context.FEE_INVOICE_UPLOAD_ITEM_LOG.Add(entInvoiceUploadItem);
        //                    context.SaveChanges();
        //                    sequence++;
        //                }
        //                //end foreach item invoice upload
        //                //Generate Create Invoice
        //                var invUpload = formData.InvoiceUploadItemList.GroupBy(m => new { m.INV_NO, m.INV_MONTH, m.INV_YEAR } ).ToList();

        //                var invModelList = new List<InvoiceViewModel>();
        //                foreach (var inv in invUpload)
        //                {
        //                    var entInvList = formData.InvoiceUploadItemList.Where(m => m.INV_NO == inv.Key.INV_NO && m.INV_MONTH == inv.Key.INV_MONTH && m.INV_YEAR == inv.Key.INV_YEAR).ToList();

        //                    var invModel = new InvoiceViewModel();

        //                    var getentInvList = entInvList.FirstOrDefault();
        //                    var entpymItemsData = (from m in context.PAYMENT_ITEMS  select m).ToList();

        //                    var entpymItems = entpymItemsData.Where(m => m.PAYMENT_ITEMS_NAME == getentInvList.FEE.Trim()
        //                    && m.COMPANY_CODE == getentInvList.COMPANY_CODE
        //                    ).ToList();
        //                    var pymItemsModel = new PaymentItemsViewModel();
        //                    var pymItemsService = new PaymentItemsService();
        //                    var chkentpymItems = entpymItems.Where(m => m.CHANNELS == getentInvList.CHANNELS
        //                    && m.COST_CENTER == getentInvList.CCT_CODE
        //                    && m.GL_ACCOUNT == getentInvList.GL_ACCOUNT).ToList();
        //                    if (!chkentpymItems.Any())
        //                    {


        //                        if (entpymItems.Any())
        //                        {


        //                            var pymItems = entInvList.FirstOrDefault();
        //                            MVMMappingService.MoveData(pymItems, pymItemsModel);
        //                            pymItemsModel.LASTMODIFIED_BY = "";
        //                            pymItemsModel.LASTMODIFIED_DATE = DateTime.Now;
        //                            pymItemsModel.Supplier = entpymItems.FirstOrDefault().Supplier;
        //                            pymItemsModel.ID = entpymItems.FirstOrDefault().ID;
        //                            pymItemsModel.PAYMENT_ITEMS_CODE = entpymItems.FirstOrDefault().PAYMENT_ITEMS_CODE;
        //                            pymItemsModel.PAYMENT_ITEMS_NAME = pymItems.FEE;

        //                            pymItemsModel.CHANNELS = getentInvList.CHANNELS;
        //                            pymItemsModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
        //                            pymItemsModel.COST_CENTER = getentInvList.CCT_CODE;
        //                            pymItemsModel.CCT_CODE = getentInvList.CCT_CODE;
        //                            pymItemsModel.FUND_CODE = getentInvList.FUND_CODE;

        //                            pymItemsModel.GROUP_SEQ_CHANNELS = entpymItems.FirstOrDefault().GROUP_SEQ_CHANNELS;
        //                            pymItemsModel.SEQ_CHANNELS = entpymItems.FirstOrDefault().SEQ_CHANNELS;

        //                            pymItemsModel.LASTMODIFIED_BY = getentInvList.INV_REC_BY;
        //                            pymItemsModel.LASTMODIFIED_DATE = DateTime.Now;
        //                            #region item Charge

        //                            int sequenceCharge = 1;
        //                            foreach (var item in entInvList)
        //                            {
        //                                var pymItemsChargeModel = new PaymentItemsChargeViewModel();
        //                                MVMMappingService.MoveData(item, pymItemsChargeModel);
        //                                pymItemsChargeModel.SEQUENCE = sequenceCharge;
        //                                pymItemsChargeModel.PAYMENT_ITEMS_NAME = item.FEE;
        //                                pymItemsChargeModel.PAYMENT_ITEMS_FEE_NAME = item.CHARGED;
        //                                pymItemsChargeModel.PAYMENT_ITEMS_ID = pymItemsModel.ID;

        //                                //pymItemsChargeModel.CHARGE_TYPE = "";
        //                                if (!string.IsNullOrWhiteSpace(item.TRANSACTIONS) && !string.IsNullOrWhiteSpace(item.RATE_TRANS))
        //                                {
        //                                    pymItemsChargeModel.CHARGE_TYPE = "TRXN";
        //                                }
        //                                else
        //                                {
        //                                    pymItemsChargeModel.CHARGE_TYPE = "MDR";
        //                                }

        //                                pymItemsModel.pymItemsChargeList.Add(pymItemsChargeModel);
        //                                sequenceCharge++;
        //                            }
        //                            #endregion

        //                            result = pymItemsService.SaveEdit(pymItemsModel, modelState);
        //                            if (result.ErrorFlag)
        //                            {
        //                                result.ErrorFlag = true;
        //                                result.ModelStateErrorList = result.ModelStateErrorList;

        //                                return result;
        //                            }

        //                        }
        //                        else
        //                        {
        //                            string pymName = getentInvList.FEE.Replace(" ", "").Length > 14 ? getentInvList.FEE.Replace(" ", "").Substring(0, 15) : getentInvList.FEE.Replace(" ", "");
        //                            var getNewpymItems = entpymItemsData.Where(m => m.COMPANY_CODE == getentInvList.COMPANY_CODE).ToList();
        //                            if (getNewpymItems.Any())
        //                            {

        //                                int seqgetNewpymItems = getNewpymItems.Count() + 1;
        //                                pymItemsModel.GROUP_SEQ_CHANNELS = seqgetNewpymItems;

        //                                pymItemsModel.PAYMENT_ITEMS_CODE = getentInvList.COMPANY_CODE + pymName + DateTime.Now.Date.ToString("yyyymmdd") + "_" + pymItemsModel.GROUP_SEQ_CHANNELS;
        //                            }
        //                            else
        //                            {

        //                                pymItemsModel.PAYMENT_ITEMS_CODE = getentInvList.COMPANY_CODE + pymName + DateTime.Now.Date.ToString("yyyymmdd");
        //                            }


        //                            pymItemsModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
        //                            pymItemsModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
        //                            pymItemsModel.LASTMODIFIED_BY = "";
        //                            pymItemsModel.LASTMODIFIED_DATE = null;
        //                            pymItemsModel.CREATE_BY = "";
        //                            pymItemsModel.CREATE_DATE = DateTime.Now;
        //                            pymItemsModel.REMARK = "";
        //                            pymItemsModel.Supplier = "";

        //                            pymItemsModel.CHANNELS = getentInvList.CHANNELS;
        //                            pymItemsModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
        //                            pymItemsModel.COST_CENTER = getentInvList.CCT_CODE;
        //                            pymItemsModel.CCT_CODE = getentInvList.CCT_CODE;
        //                            pymItemsModel.FUND_CODE = getentInvList.FUND_CODE;


        //                            pymItemsModel.CREATE_BY = getentInvList.INV_REC_BY;
        //                            pymItemsModel.CREATE_DATE = DateTime.Now;

        //                            #region item Charge

        //                            int sequenceCharge = 1;
        //                            foreach (var item in entInvList)
        //                            {
        //                                var pymItemsChargeModel = new PaymentItemsChargeViewModel();
        //                                MVMMappingService.MoveData(item, pymItemsChargeModel);
        //                                pymItemsChargeModel.SEQUENCE = sequenceCharge;
        //                                pymItemsChargeModel.PAYMENT_ITEMS_NAME = item.FEE;
        //                                pymItemsChargeModel.PAYMENT_ITEMS_FEE_NAME = item.CHARGED;

        //                                //pymItemsChargeModel.PAYMENT_ITEMS_ID = pymItemsModel.ID;
        //                                //pymItemsChargeModel.CHARGE_TYPE = "";
        //                                if (!string.IsNullOrWhiteSpace(item.TRANSACTIONS ) && !string.IsNullOrWhiteSpace(item.RATE_TRANS) )
        //                                {
        //                                    pymItemsChargeModel.CHARGE_TYPE = "TRXN";
        //                                }
        //                                else
        //                                {
        //                                    pymItemsChargeModel.CHARGE_TYPE = "MDR";
        //                                }

        //                                pymItemsModel.pymItemsChargeList.Add(pymItemsChargeModel);
        //                                sequenceCharge++;
        //                            }
        //                            #endregion

        //                            result = pymItemsService.SaveCreate(pymItemsModel, modelState);
        //                            if (result.ErrorFlag)
        //                            {
        //                                result.ErrorFlag = true;
        //                                result.ModelStateErrorList = result.ModelStateErrorList;

        //                                return result;
        //                            }
        //                        }
        //                        getentInvList.PAYMENT_ITEMS_CODE = pymItemsModel.PAYMENT_ITEMS_CODE;
        //                    }
        //                    else
        //                    {
        //                        getentInvList.PAYMENT_ITEMS_CODE = chkentpymItems.FirstOrDefault().PAYMENT_ITEMS_CODE;
        //                    }

        //                    if (getentInvList != null)
        //                    {
        //                        invModel.INV_NO = getentInvList.INV_NO;
        //                        invModel.PRO_NO = getentInvList.PRO_NO;
        //                        invModel.CCT_CODE = getentInvList.CCT_CODE;
        //                        invModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
        //                        invModel.INV_MONTH = ConvertStrToInt32(getentInvList.INV_MONTH);
        //                        invModel.INV_YEAR = ConvertStrToInt32(getentInvList.INV_YEAR);
        //                        invModel.PAYMENT_ITEMS_CODE = getentInvList.PAYMENT_ITEMS_CODE;
        //                        invModel.INV_REC_BY = getentInvList.INV_REC_BY;
        //                        invModel.INV_REC_DATE = ConvertStrParseDateTime(getentInvList.INV_REC_DATE);
        //                        invModel.INV_APPROVED_BY = getentInvList.INV_APPROVED_BY;
        //                        invModel.INV_APPROVED_DATE = ConvertStrParseDateTime(getentInvList.INV_APPROVED_DATE);
        //                        invModel.INV_DUE_DATE = ConvertStrParseDateTime(getentInvList.INV_DUE_DATE);
        //                        invModel.MODIFIED_BY = getentInvList.MODIFIED_BY;
        //                        invModel.MODIFIED_DATE = getentInvList.MODIFIED_DATE;
        //                        invModel.CREATE_BY = getentInvList.INV_REC_BY;
        //                        invModel.CREATE_DATE = DateTime.Now;
        //                        invModel.IS_STATUS = getentInvList.IS_STATUS;
        //                        invModel.CONDITION_PYM = getentInvList.CONDITION_PYM;
        //                        invModel.NET_AMOUNT = ConvertStrToDec(getentInvList.NET_AMOUNT);
        //                        invModel.REMARK = getentInvList.REMARK;
        //                        invModel.WHT = ConvertStrToDec(getentInvList.WHT);
        //                        invModel.DEDUCT_WHT_AMOUNT = ConvertStrToDec(getentInvList.DEDUCT_WHT_AMOUNT);
        //                        invModel.CURRENCY = getentInvList.CURRENCY;
        //                        invModel.INCLUDE_VAT_AMOUNT = ConvertStrToDec(getentInvList.INCLUDE_VAT_AMOUNT);
        //                        invModel.VAT = ConvertStrToDec(getentInvList.VAT);
        //                        invModel.PRO_REC_DATE = ConvertStrParseDateTime(getentInvList.PRO_REC_DATE);
        //                        invModel.DISCOUNT = ConvertStrToDec(getentInvList.DISCOUNT);
        //                        invModel.PRO_DUE_DATE = ConvertStrParseDateTime(getentInvList.PRO_DUE_DATE);
        //                        invModel.CHANNELS = getentInvList.CHANNELS;
        //                        invModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
        //                        invModel.COST_CENTER = getentInvList.CCT_CODE;
        //                        invModel.FUND_CODE = getentInvList.FUND_CODE;
        //                        invModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
        //                        invModel.FormAction = ConstantVariableService.FormActionCreate;
        //                        invModel.UPLOAD_TYPE = true;
        //                    }
        //                    int line = 1;
        //                    foreach (var ent in entInvList)
        //                    {
        //                        var invItemModel = new InvoiceDetailViewModel();
        //                        //MVMMappingService.MoveData(ent, invModel);
        //                        invItemModel.INV_NO = ent.INV_NO;
        //                        invItemModel.PAYMENT_ITEMS_FEE_ITEM = ent.CHARGED;
        //                        invItemModel.INV_ITEM_NO = ent.INV_NO + "_" + line;
        //                        invItemModel.SEQUENCE = line;
        //                        invItemModel.TRANSACTIONS = ConvertStrToDec(ent.TRANSACTIONS);
        //                        invItemModel.ACTUAL_AMOUNT = ConvertStrToDec(ent.ACTUAL_AMOUNT);
        //                        invItemModel.TOTAL_CHARGE_AMOUNT = ConvertStrToDec(ent.TOTAL_CHARGE_AMOUNT);
        //                        invItemModel.RATE_TRANS = ConvertStrToDec(ent.RATE_TRANS);
        //                        invItemModel.RATE_AMT = ConvertStrToDec(ent.RATE_AMT);
        //                        invItemModel.MODIFIED_BY = ent.MODIFIED_BY;
        //                        invItemModel.MODIFIED_DATE = ent.MODIFIED_DATE;
        //                        invItemModel.CREATE_BY = ent.CREATE_BY;
        //                        invItemModel.CREATE_DATE = ent.CREATE_DATE;
        //                        invItemModel.REMARK = ent.REMARK;
        //                        invItemModel.INV_MONTH = ConvertStrToInt32(getentInvList.INV_MONTH);
        //                        invItemModel.INV_YEAR = ConvertStrToInt32(getentInvList.INV_YEAR);
        //                        invItemModel.CHANNELS = getentInvList.CHANNELS;
        //                        invItemModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
        //                        invItemModel.COST_CENTER = getentInvList.CCT_CODE;
        //                        invItemModel.FUND_CODE = getentInvList.FUND_CODE;
        //                        invItemModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
        //                        invItemModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
        //                        invItemModel.PAYMENT_ITEMS_CODE = getentInvList.PAYMENT_ITEMS_CODE;

        //                        //MVMMappingService.MoveData(ent, invItemModel);
        //                        invModel.InvoiceDetailList.Add(invItemModel);
        //                        line++;
        //                    }
        //                    result = service.SaveCreate(invModel, modelState);
        //                    if (result.ErrorFlag)
        //                    {
        //                        result.ErrorFlag = true;
        //                        result.ModelStateErrorList = result.ModelStateErrorList;

        //                        return result;
        //                    }
        //                }



        //            }


        //            scope.Complete();

        //            //result.Message = ResourceText.SuccessfulSave;
        //            //result.MessageType = "S";

        //        }
        //        //Manage attachment
        //        if (formData.ID != 0)
        //        {
        //            var docFilePath = AttachmentService.GetDocumentFilePath(InvoiceUploadViewModel.ProcessCode);
        //            foreach (var attachment in formData.AttachmentList)
        //            {
        //                attachment.ProcessCode = InvoiceUploadViewModel.ProcessCode;
        //                //attachment.DataID = formData.ID;
        //                attachment.DataKey = formData.ID.ToString();
        //                attachment.DocumentFilePath = docFilePath;
        //            }
        //            AttachmentService.ManageAttachment(formData.AttachmentList);
        //        }

        //        result.Message = ResourceText.SuccessfulUpload;//+ " (" + ResourceText.DocumentNo + ": " + formData.JobDocNo + ")";
        //        result.MessageType = "S";
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Message = ex.ToString();
        //        result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
        //        result.ErrorFlag = true;
        //    }

        //    return result;
        //}
        public int ConvertStrToInt32(string res = "")
        {
            int valInt = 0;
            try
            {
                valInt = Convert.ToInt32(string.IsNullOrEmpty(res) ? "0" : res);
            }
            catch (Exception)
            {

                throw;
            }
            return valInt;

        }
        public decimal ConvertStrToDec(string res = "")
        {
            decimal valDec = 0;
            try
            {
                valDec = Convert.ToDecimal(string.IsNullOrEmpty(res) ? "0" : res);
            }
            catch (Exception)
            {

                throw;
            }
            return valDec;

        }
        public Nullable<DateTime> ConvertStrParseDateTime(string res = "")
        {
            Nullable<DateTime> valdt = new Nullable<DateTime>();
            try
            {
                //DateTime dt = Convert.ToDateTime(res);
                //DateTime dt1 = DateTime.Now;

                valdt = string.IsNullOrEmpty(res) ? valdt : (DateTime)DateTime.ParseExact(res, "dd/mm/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

                throw;
            }
            return valdt;

        }
        public override ValidationResult SaveDelete(InvoiceUploadViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    var entInvoiceUpload = new FEE_INVOICE_UPLOAD();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entInvoiceUpload);

                    using (var context = new PYMFEEEntities())
                    {
                        //Delete header            
                        context.Entry(entInvoiceUpload).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();

                        //Delete item
                        context.FEE_INVOICE_UPLOAD_ITEM_LOG.RemoveRange(context.FEE_INVOICE_UPLOAD_ITEM_LOG.Where(m => m.INV_UPLOAD_ID == entInvoiceUpload.ID));
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

        public override ValidationResult SaveEdit(InvoiceUploadViewModel formData, ModelStateDictionary modelState)
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
                    FEE_INVOICE_UPLOAD entInvoiceUpload = new FEE_INVOICE_UPLOAD();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entInvoiceUpload);
                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {

                        context.Entry(entInvoiceUpload).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();


                        //Save Item          
                        foreach (var item in formData.InvoiceUploadItemList)
                        {
                            var entInvoiceUploadItem = new FEE_INVOICE_UPLOAD_ITEM_LOG();

                            item.INV_UPLOAD_ID = entInvoiceUpload.ID;
                            MVMMappingService.MoveData(item, entInvoiceUploadItem);
                            if (item.DeleteFlag)
                            {
                                if (entInvoiceUploadItem.ID != 0)
                                {
                                    context.Entry(entInvoiceUploadItem).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                            else
                            {


                                if (entInvoiceUploadItem.ID != 0)
                                {
                                    context.Entry(entInvoiceUploadItem).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    context.FEE_INVOICE_UPLOAD_ITEM_LOG.Add(entInvoiceUploadItem);
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

        public override ValidationResult ValidateFormData(InvoiceUploadViewModel formData, ModelStateDictionary modelState)
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
                //    var itemList = formData.InvoiceUploadItemList.Where(m => m.DeleteFlag != true).ToList();

                //    //Check item
                //    int line = 1;
                //    foreach (var item in itemList)
                //    {
                //        //product id not empty
                //        if (item.InvoiceCode == null)
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
                //    //var dupInvoice = _context.Invoices.Where(m => m.InvoiceType == formData.InvoiceType
                //    //&& m.InvoiceCode == formData.InvoiceCode).ToList();
                //    //if (dupInvoice.Any())
                //    //{
                //    //    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, string.Concat(ResourceText.InvoiceCategory, " ", ResourceText.ConsumableCode))));
                //    //    result.ErrorFlag = true;
                //    //}
                //    }

                //}


                var convertResult = ConvertUploadItemToFormData(formData);

                #region Invoice Model
                using (var context = new PYMFEEEntities())
                {
                    InvoiceService service = new InvoiceService();
                    //Generate Create Invoice
                    var invUpload = formData.InvoiceUploadItemList.GroupBy(m => new { m.INV_NO, m.INV_MONTH, m.INV_YEAR }).ToList();

                    var invModelList = new List<InvoiceViewModel>();
                    foreach (var inv in invUpload)
                    {
                        var entInvList = formData.InvoiceUploadItemList.Where(m => m.INV_NO == inv.Key.INV_NO && m.INV_MONTH == inv.Key.INV_MONTH && m.INV_YEAR == inv.Key.INV_YEAR).ToList();

                        var invModel = new InvoiceViewModel();

                        var getentInvList = entInvList.FirstOrDefault();
                        //var entpymItemsData = (from m in context.PAYMENT_ITEMS select m).ToList();

                        //var entpymItems = entpymItemsData.Where(m => m.PAYMENT_ITEMS_NAME == getentInvList.FEE.Trim()
                        //&& m.COMPANY_CODE == getentInvList.COMPANY_CODE
                        //).ToList();
                        //var pymItemsModel = new PaymentItemsViewModel();
                        //var pymItemsService = new PaymentItemsService();
                        //var chkentpymItems = entpymItems.Where(m => m.CHANNELS == getentInvList.CHANNELS
                        //&& m.COST_CENTER == getentInvList.CCT_CODE
                        //&& m.GL_ACCOUNT == getentInvList.GL_ACCOUNT
                        //&& m.FUND_CODE == getentInvList.FUND_CODE).ToList();
                        //if (!chkentpymItems.Any())
                        //{


                        //    if (entpymItems.Any())
                        //    {


                        //        var pymItems = entInvList.FirstOrDefault();
                        //        MVMMappingService.MoveData(pymItems, pymItemsModel);
                        //        pymItemsModel.LASTMODIFIED_BY = "";
                        //        pymItemsModel.LASTMODIFIED_DATE = DateTime.Now;
                        //        pymItemsModel.Supplier = entpymItems.FirstOrDefault().Supplier;
                        //        pymItemsModel.ID = entpymItems.FirstOrDefault().ID;
                        //        pymItemsModel.PAYMENT_ITEMS_CODE = entpymItems.FirstOrDefault().PAYMENT_ITEMS_CODE;
                        //        pymItemsModel.PAYMENT_ITEMS_NAME = pymItems.FEE;
                        //        pymItemsModel.CHANNELS = getentInvList.CHANNELS;
                        //        pymItemsModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
                        //        pymItemsModel.COST_CENTER = getentInvList.CCT_CODE;
                        //        pymItemsModel.FUND_CODE = getentInvList.FUND_CODE;
                        //        result = pymItemsService.SaveEdit(pymItemsModel, modelState);
                        //        if (result.ErrorFlag)
                        //        {
                        //            result.ErrorFlag = true;
                        //            result.ModelStateErrorList = result.ModelStateErrorList;

                        //            return result;
                        //        }

                        //    }
                        //    else
                        //    {

                        //        //char[] whitespace = new char[] { ' ', '\t' };
                        //        //string[] ssizes = getentInvList.FEE.Split(whitespace);

                        //        //string pymName = string.Join("", ssizes).Substring(0, 15);
                        //        string pymName = getentInvList.FEE.Replace(" ", "").Length > 14 ? getentInvList.FEE.Replace(" ", "").Substring(0, 15) : getentInvList.FEE.Replace(" ", "");
                        //        var getNewpymItems = entpymItemsData.Where(m => m.COMPANY_CODE == getentInvList.COMPANY_CODE).ToList();
                        //        if (getNewpymItems.Any())
                        //        {
                        //            int seqgetNewpymItems = getNewpymItems.Count() + 1;
                        //            pymItemsModel.GROUP_SEQ_CHANNELS = seqgetNewpymItems;
                        //            pymItemsModel.PAYMENT_ITEMS_CODE = getentInvList.COMPANY_CODE + pymName + DateTime.Now.Date.ToString("yyyymmdd") + "_" + seqgetNewpymItems;
                        //        }
                        //        else
                        //        {

                        //            pymItemsModel.PAYMENT_ITEMS_CODE = getentInvList.COMPANY_CODE + pymName + DateTime.Now.Date.ToString("yyyymmdd");
                        //        }
                        //        pymItemsModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
                        //        pymItemsModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
                        //        pymItemsModel.LASTMODIFIED_BY = "";
                        //        pymItemsModel.LASTMODIFIED_DATE = null;
                        //        pymItemsModel.CREATE_BY = "";
                        //        pymItemsModel.CREATE_DATE = DateTime.Now;
                        //        pymItemsModel.REMARK = "";
                        //        pymItemsModel.Supplier = "";

                        //        pymItemsModel.CHANNELS = getentInvList.CHANNELS;
                        //        pymItemsModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
                        //        pymItemsModel.COST_CENTER = getentInvList.CCT_CODE;
                        //        pymItemsModel.FUND_CODE = getentInvList.FUND_CODE;
                        //        result = pymItemsService.SaveCreate(pymItemsModel, modelState);
                        //        if (result.ErrorFlag)
                        //        {
                        //            result.ErrorFlag = true;
                        //            result.ModelStateErrorList = result.ModelStateErrorList;

                        //            return result;
                        //        }
                        //    }
                        //    getentInvList.PAYMENT_ITEMS_CODE = pymItemsModel.PAYMENT_ITEMS_CODE;
                        //}
                        //else
                        //{
                        //    getentInvList.PAYMENT_ITEMS_CODE = chkentpymItems.FirstOrDefault().PAYMENT_ITEMS_CODE;
                        //}
                        //getentInvList.PAYMENT_ITEMS_CODE = chkentpymItems.FirstOrDefault().PAYMENT_ITEMS_CODE;
                        if (getentInvList != null)
                        {
                            invModel.INV_NO = getentInvList.INV_NO;
                            invModel.PRO_NO = getentInvList.PRO_NO;
                            invModel.CCT_CODE = getentInvList.CCT_CODE;
                            invModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
                            invModel.INV_MONTH = ConvertStrToInt32(getentInvList.INV_MONTH);
                            invModel.INV_YEAR = ConvertStrToInt32(getentInvList.INV_YEAR);
                            invModel.PAYMENT_ITEMS_CODE = getentInvList.PAYMENT_ITEMS_CODE;
                            invModel.INV_REC_BY = getentInvList.INV_REC_BY;
                            invModel.INV_REC_DATE = ConvertStrParseDateTime(getentInvList.INV_REC_DATE);
                            invModel.INV_APPROVED_BY = getentInvList.INV_APPROVED_BY;
                            invModel.INV_APPROVED_DATE = ConvertStrParseDateTime(getentInvList.INV_APPROVED_DATE);
                            invModel.INV_DUE_DATE = ConvertStrParseDateTime(getentInvList.INV_DUE_DATE);
                            invModel.MODIFIED_BY = getentInvList.MODIFIED_BY;
                            invModel.MODIFIED_DATE = getentInvList.MODIFIED_DATE;
                            invModel.CREATE_BY = getentInvList.INV_REC_BY;
                            invModel.CREATE_DATE = DateTime.Now;
                            invModel.IS_STATUS = getentInvList.IS_STATUS;
                            invModel.CONDITION_PYM = getentInvList.CONDITION_PYM;
                            invModel.NET_AMOUNT = ConvertStrToDec(getentInvList.NET_AMOUNT);
                            invModel.REMARK = getentInvList.REMARK;
                            invModel.WHT = ConvertStrToDec(getentInvList.WHT);
                            invModel.DEDUCT_WHT_AMOUNT = ConvertStrToDec(getentInvList.DEDUCT_WHT_AMOUNT);
                            invModel.CURRENCY = getentInvList.CURRENCY;
                            invModel.INCLUDE_VAT_AMOUNT = ConvertStrToDec(getentInvList.INCLUDE_VAT_AMOUNT);
                            invModel.VAT = ConvertStrToDec(getentInvList.VAT);
                            invModel.PRO_REC_DATE = ConvertStrParseDateTime(getentInvList.PRO_REC_DATE);
                            invModel.DISCOUNT = ConvertStrToDec(getentInvList.DISCOUNT);
                            invModel.PRO_DUE_DATE = ConvertStrParseDateTime(getentInvList.PRO_DUE_DATE);
                            invModel.CHANNELS = getentInvList.CHANNELS;
                            invModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
                            invModel.COST_CENTER = getentInvList.CCT_CODE;
                            invModel.FUND_CODE = getentInvList.FUND_CODE;
                            invModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
                            invModel.FormAction = ConstantVariableService.FormActionCreate;
                            invModel.UPLOAD_TYPE = true;
                        }
                        int line = 1;
                        foreach (var ent in entInvList)
                        {
                            var invItemModel = new InvoiceDetailViewModel();
                            invItemModel.INV_NO = ent.INV_NO;
                            invItemModel.PAYMENT_ITEMS_FEE_ITEM = ent.CHARGED;
                            invItemModel.INV_ITEM_NO = ent.INV_NO + "_" + line;
                            invItemModel.SEQUENCE = line;
                            invItemModel.TRANSACTIONS = ConvertStrToDec(ent.TRANSACTIONS);
                            invItemModel.ACTUAL_AMOUNT = ConvertStrToDec(ent.ACTUAL_AMOUNT);
                            invItemModel.TOTAL_CHARGE_AMOUNT = ConvertStrToDec(ent.TOTAL_CHARGE_AMOUNT);
                            invItemModel.RATE_TRANS = ConvertStrToDec(ent.RATE_TRANS);
                            invItemModel.RATE_AMT = ConvertStrToDec(ent.RATE_AMT);
                            invItemModel.MODIFIED_BY = ent.MODIFIED_BY;
                            invItemModel.MODIFIED_DATE = ent.MODIFIED_DATE;
                            invItemModel.CREATE_BY = ent.CREATE_BY;
                            invItemModel.CREATE_DATE = ent.CREATE_DATE;
                            invItemModel.REMARK = ent.REMARK;
                            invItemModel.INV_MONTH = ConvertStrToInt32(getentInvList.INV_MONTH);
                            invItemModel.INV_YEAR = ConvertStrToInt32(getentInvList.INV_YEAR);
                            invItemModel.CHANNELS = getentInvList.CHANNELS;
                            invItemModel.GL_ACCOUNT = getentInvList.GL_ACCOUNT;
                            invItemModel.COST_CENTER = getentInvList.CCT_CODE;
                            invItemModel.FUND_CODE = getentInvList.FUND_CODE;
                            invItemModel.PAYMENT_ITEMS_NAME = getentInvList.FEE;
                            invItemModel.COMPANY_CODE = getentInvList.COMPANY_CODE;
                            invItemModel.PAYMENT_ITEMS_CODE = getentInvList.PAYMENT_ITEMS_CODE;

                            //MVMMappingService.MoveData(ent, invItemModel);
                            invModel.InvoiceDetailList.Add(invItemModel);
                            line++;
                        }
                        result = service.ValidateFormData(invModel, modelState);
                        if (result.ErrorFlag)
                        {
                            result.ErrorFlag = true;
                            result.ModelStateErrorList = result.ModelStateErrorList;

                            return result;
                        }
                    }
                }
                #endregion

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
        public ValidationWithReturnResult<InvoiceUploadViewModel> ConvertUploadItemToFormData(InvoiceUploadViewModel uploadItem)
        {
            ValidationWithReturnResult<InvoiceUploadViewModel> result = new ValidationWithReturnResult<InvoiceUploadViewModel>();
            result.ReturnResult = new InvoiceUploadViewModel();
            var userInfo = UserService.GetSessionUserInfo();
            //Convert result Decimal
            ValidationWithReturnResult<decimal> decimalResult = new ValidationWithReturnResult<decimal>();
            //Convert result DatetTime
            ValidationWithReturnResult<DateTime> datetimeResult = new ValidationWithReturnResult<DateTime>();

            try
            {
                InvoiceUploadViewModel invUploadViewModel = new InvoiceUploadViewModel();

                //Import Job shop and product from excel
                var uploadFile = uploadItem.AttachmentList.Where(m => m.DeleteFlag != true).FirstOrDefault();

                if (uploadFile == null)
                {
                    result.ErrorFlag = true;
                    result.ModelStateErrorList.Add(new ModelStateError("", ValidatorMessage.upload_file_notempty_error));

                    return result;
                }
                else
                {
                    invUploadViewModel.FileName = uploadFile.FileName;
                }

                var importResult = ImportExcel(uploadFile.SavedFileName);
                uploadItem.INV_MONTH = DateTime.Now.Month;
                uploadItem.INV_YEAR = DateTime.Now.Year;
                uploadItem.CREATE_BY = userInfo.UserCode;
                uploadItem.CREATE_DATE = DateTime.Now;
                uploadItem.UPLOAD_BY = userInfo.UserCode;
                uploadItem.UPLOAD_DATE = DateTime.Now.Date;

                //--AddList Item Invoice Upload
                // uploadItem.InvoiceUploadItemList = new List<InvoiceUploadItemViewModel>();
                invUploadViewModel = uploadItem;
                if (importResult.ErrorFlag)
                {
                    result.ErrorFlag = true;
                    result.ModelStateErrorList.Add(new ModelStateError("", importResult.Message));

                    return result;
                }
                

                invUploadViewModel.InvoiceUploadItemList = importResult.ReturnResult;


                var invuploadError = importResult.ReturnResult.Where(m => m.ErrorFlag == true).FirstOrDefault();
                var invuploadErrorMs = importResult.ReturnResult.Where(m => m.ErrorFlag == true).ToList();
                if (invuploadError != null)
                {

                    result.ErrorFlag = true;

                    if (invuploadErrorMs.Any())
                    {
                        foreach (var item in invuploadErrorMs)
                        {
                            result.ModelStateErrorList.Add(new ModelStateError("", item.Message));
                        }
                    }
                    else
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", ValidatorMessage.upload_file_content_error));
                    }

                    return result;
                }

               
                result.ReturnResult = invUploadViewModel;

            }
            catch (Exception ex)
            {

            }

            return result;
        }


        public ValidationWithReturnResult<List<InvoiceUploadItemViewModel>> ImportExcel(string fileName)
        {
            ValidationWithReturnResult<List<InvoiceUploadItemViewModel>> result = new ValidationWithReturnResult<List<InvoiceUploadItemViewModel>>();
            result.ReturnResult = new List<InvoiceUploadItemViewModel>();

            List<InvoiceUploadItemViewModel> uploadItemList = new List<InvoiceUploadItemViewModel>();
            ValidationWithReturnResult<DataSet> resultDataSet = new ValidationWithReturnResult<DataSet>();
            int t = 1;
            try
            {
                //Check required field


                //if (string.IsNullOrEmpty(matType))
                //{
                //    result.ErrorFlag = true;
                //    //result.Message = ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.InvoiceCategory);
                //    result.ModelStateErrorList.Add(new ModelStateError("", result.Message));

                //}
                //var keyDateVal = DateTime.ParseExact(keyDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                if (result.ErrorFlag)
                {
                    return result;
                }


                string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
                tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);
                //DataSet excelDS = ExcelService.ConvertExcelToDataSet(tempFilePath, fileName, 1);
                resultDataSet = ExcelService.ConvertExcelToDataSet(tempFilePath, fileName, "Sheet1");
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
                    while (rowIndex < table.Rows.Count)
                    {
                        DataRow row = table.Rows[rowIndex];
                        InvoiceUploadItemViewModel uploadItem = new InvoiceUploadItemViewModel();

                        uploadItem.INV_MONTH = row[0].ToString();
                        uploadItem.INV_YEAR = row[1].ToString();
                        uploadItem.CHANNELS = row[2].ToString();
                        uploadItem.FEE = row[3].ToString();
                        uploadItem.SEQUENCE = Convert.ToInt32(row[4]==null?"0":row[4].ToString().Trim());
                        uploadItem.CHARGED = row[5].ToString();
                        uploadItem.TRANSACTIONS = row[6].ToString();
                        uploadItem.RATE_TRANS = row[7].ToString();
                        uploadItem.ACTUAL_AMOUNT = row[8].ToString();
                        uploadItem.RATE_AMT = row[9].ToString();
                        uploadItem.TOTAL_CHARGE_AMOUNT = row[10].ToString();
                        uploadItem.NET_AMOUNT = row[11].ToString();
                        uploadItem.DEDUCT_WHT_AMOUNT = row[12].ToString();
                        uploadItem.CURRENCY = row[13].ToString();
                        uploadItem.INCLUDE_VAT_AMOUNT = row[14].ToString();
                        uploadItem.GL_ACCOUNT = row[15].ToString();
                        uploadItem.CCT_CODE = row[16].ToString();
                        uploadItem.FUND_CODE = row[17].ToString();
                        uploadItem.COMPANY_CODE = row[18].ToString();
                        uploadItem.INV_NO = row[19].ToString();
                        uploadItem.PRO_NO = row[20].ToString();
                        uploadItem.PRO_REC_DATE = row[21].ToString();
                        uploadItem.INV_REC_BY = row[22].ToString();
                        uploadItem.INV_REC_DATE = row[23].ToString();
                        uploadItem.INV_APPROVED_BY = row[24].ToString();
                        uploadItem.INV_APPROVED_DATE = row[25].ToString();
                        uploadItem.INV_DUE_DATE = row[26].ToString();
                        uploadItem.CONDITION_PYM = row[27].ToString();
                        uploadItem.VAT = row[28].ToString();
                        uploadItem.WHT = row[29].ToString();
                        uploadItem.REMARK = row[30].ToString();
                        uploadItem.DISCOUNT = row[31].ToString();
                        uploadItem.PRO_DUE_DATE = row[32].ToString();
                        uploadItem.IS_STATUS = row[33].ToString();




                        uploadItemList.Add(uploadItem);
                        rowIndex++;
                        t++;
                    }
                    rowIndex = 1;
                }

                result = ValidateUploadItemFormat(uploadItemList);

            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString(), "");

                return result;
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
        public ValidationWithReturnResult<List<InvoiceUploadItemViewModel>> ValidateUploadItemFormat(List<InvoiceUploadItemViewModel> uploadItemList)
        {
            ValidationWithReturnResult<List<InvoiceUploadItemViewModel>> result = new ValidationWithReturnResult<List<InvoiceUploadItemViewModel>>();
            result.ReturnResult = new List<InvoiceUploadItemViewModel>();

            //Convert result
            ValidationWithReturnResult<decimal> decimalResult = new ValidationWithReturnResult<decimal>();
            ValidationWithReturnResult<DateTime> datetimeResult = new ValidationWithReturnResult<DateTime>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var entcompany = (from m in context.COMPANies where m.IsPaymentFee == true select m).ToList();

                    int line = 1;
                    foreach (var uploadItem in uploadItemList)
                    {
                        
                        //INV_MONTH
                        if (string.IsNullOrWhiteSpace(uploadItem.INV_MONTH))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.INV_MONTH) + ConstantVariableService.HtmlNewLine;
                          
                        }
                        else
                        {

                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.INV_MONTH.Trim());
                            if (decimalResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.INV_MONTH + " {ex.format is  1-12 }") + ConstantVariableService.HtmlNewLine;
                               
                            }
                            else
                            {
                                var chkMonth = Enumerable.Range(1, 12).ToList().Contains(Convert.ToInt32(decimalResult.ReturnResult));

                                if (!chkMonth)
                                {
                                    uploadItem.ErrorFlag = true;
                                    uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.INV_MONTH + " {ex.format is  1-12 }") + ConstantVariableService.HtmlNewLine;
                                    
                                }


                                else
                                {
                                    uploadItem.INV_MONTH = uploadItem.INV_MONTH.Trim();
                                }
                            }
                        }
                        //INV_YEAR
                        if (string.IsNullOrWhiteSpace(uploadItem.INV_YEAR))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.INV_YEAR) + ConstantVariableService.HtmlNewLine;
                        
                        }
                        else
                        {

                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.INV_YEAR.Trim());
                            if (decimalResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.INV_YEAR) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                var chkYear = uploadItem.INV_YEAR.Trim().Length;

                                if (chkYear > 4)
                                {
                                    uploadItem.ErrorFlag = true;
                                    uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.INV_YEAR) + ConstantVariableService.HtmlNewLine;
                                   
                                }


                                else
                                {
                                    uploadItem.INV_YEAR = uploadItem.INV_YEAR.Trim();
                                }
                            }
                        }

                        //CHANNELS
                        if (string.IsNullOrWhiteSpace(uploadItem.CHANNELS))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.CHANNELS) + ConstantVariableService.HtmlNewLine;
                           
                        }
                        else
                        {
                            uploadItem.CHANNELS = uploadItem.CHANNELS.Trim();
                        }
                        //FEE
                        if (string.IsNullOrWhiteSpace(uploadItem.FEE))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.FEE) + ConstantVariableService.HtmlNewLine;
                           
                        }
                        else
                        {
                            uploadItem.FEE = uploadItem.FEE.Trim();
                        }
                        //CHARGE
                        if (string.IsNullOrWhiteSpace(uploadItem.CHARGED))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.PAYMENT_ITEMS_FEE_ITEM) + ConstantVariableService.HtmlNewLine;
                           
                        }
                        else
                        {
                            uploadItem.CHARGED = uploadItem.CHARGED.Trim();
                        }
                        //TRANSACTION
                        if (string.IsNullOrWhiteSpace(uploadItem.TRANSACTIONS) )
                        {
                            if (!string.IsNullOrWhiteSpace(uploadItem.RATE_TRANS))
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.TRANSACTION) + ConstantVariableService.HtmlNewLine;
                            }
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.TRANSACTIONS.Trim());
                            if (decimalResult.ErrorFlag)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.TRANSACTION) + ConstantVariableService.HtmlNewLine;
                               
                            }
                            else
                            {
                                uploadItem.TRANSACTIONS = uploadItem.TRANSACTIONS.Trim();
                            }
                        }

                        ////RATE TRANS
                        uploadItem.RATE_TRANS = uploadItem.RATE_TRANS.Trim();
                        //if (string.IsNullOrWhiteSpace(uploadItem.RATE_TRANS))
                        //{
                        //    uploadItem.ErrorFlag = true;
                        //    uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.RATE_TRANS) + ConstantVariableService.HtmlNewLine;

                        //}
                        //else
                        //{
                        //    decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.RATE_TRANS.Trim());
                        //    if (decimalResult.ErrorFlag)
                        //    {
                        //        uploadItem.ErrorFlag = true;
                        //        uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.RATE_TRANS) + ConstantVariableService.HtmlNewLine;

                        //    }
                        //    else
                        //    {
                        //        uploadItem.RATE_TRANS = uploadItem.RATE_TRANS.Trim();
                        //    }
                        //}

                        //ACTUAL AMOUNT
                        if (string.IsNullOrWhiteSpace(uploadItem.ACTUAL_AMOUNT))
                        {
                            if (!string.IsNullOrWhiteSpace(uploadItem.RATE_AMT))
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.ACTUAL_AMOUNT) + ConstantVariableService.HtmlNewLine;
                            }
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.ACTUAL_AMOUNT.Trim());
                            if (decimalResult.ErrorFlag)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.ACTUAL_AMOUNT) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                uploadItem.ACTUAL_AMOUNT = uploadItem.ACTUAL_AMOUNT.Trim();
                            }
                        }
                        ////RATE AMT
                        uploadItem.RATE_AMT = uploadItem.RATE_AMT.Trim();
                        //if (string.IsNullOrWhiteSpace(uploadItem.RATE_AMT))
                        //{
                        //    uploadItem.ErrorFlag = true;
                        //    uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.RATE_AMT) + ConstantVariableService.HtmlNewLine;

                        //}
                        //else
                        //{
                        //    decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.RATE_AMT.Trim());
                        //    if (decimalResult.ErrorFlag)
                        //    {
                        //        uploadItem.ErrorFlag = true;
                        //        uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.RATE_AMT) + ConstantVariableService.HtmlNewLine;

                        //    }
                        //    else
                        //    {
                        //        uploadItem.RATE_AMT = uploadItem.RATE_AMT.Trim();
                        //    }
                        //}

                        //TOTAL CHARGE
                        if (string.IsNullOrWhiteSpace(uploadItem.TOTAL_CHARGE_AMOUNT))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.TOTAL_CHARGE_AMOUNT) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.TOTAL_CHARGE_AMOUNT.Trim());
                            if (decimalResult.ErrorFlag)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.TOTAL_CHARGE_AMOUNT) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                uploadItem.TOTAL_CHARGE_AMOUNT = uploadItem.TOTAL_CHARGE_AMOUNT.Trim();
                            }
                        }
                        //NET AMOUNT
                        if (string.IsNullOrWhiteSpace(uploadItem.NET_AMOUNT))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.NET_AMOUNT) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.NET_AMOUNT.Trim());
                            if (decimalResult.ErrorFlag)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.NET_AMOUNT) + ConstantVariableService.HtmlNewLine;
                               
                            }
                            else
                            {
                                uploadItem.NET_AMOUNT = uploadItem.NET_AMOUNT.Trim();
                            }
                        }
                        //DEDUCT AMT WHT 3 %
                        if (string.IsNullOrWhiteSpace(uploadItem.DEDUCT_WHT_AMOUNT))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.NET_CHARGE_AMOUNT) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.DEDUCT_WHT_AMOUNT.Trim());
                            if (decimalResult.ErrorFlag)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.NET_CHARGE_AMOUNT) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                uploadItem.DEDUCT_WHT_AMOUNT = uploadItem.DEDUCT_WHT_AMOUNT.Trim();
                            }
                        }
                        //INCLUD VAT
                        if (string.IsNullOrWhiteSpace(uploadItem.INCLUDE_VAT_AMOUNT))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.INCLUDE_VAT_AMOUNT) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.INCLUDE_VAT_AMOUNT.Trim());
                            if (decimalResult.ErrorFlag)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.INCLUDE_VAT_AMOUNT) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                uploadItem.INCLUDE_VAT_AMOUNT = uploadItem.INCLUDE_VAT_AMOUNT.Trim();
                            }
                        }
                        //CURENCY
                        if (string.IsNullOrWhiteSpace(uploadItem.CURRENCY))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.CURRENCY) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {

                            uploadItem.CURRENCY = uploadItem.CURRENCY.Trim();

                        }
                        //GL ACCOUNT
                        if (string.IsNullOrWhiteSpace(uploadItem.GL_ACCOUNT))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.GLAccount) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {

                            uploadItem.GL_ACCOUNT = uploadItem.GL_ACCOUNT.Trim();

                        }
                        //CCT
                        if (string.IsNullOrWhiteSpace(uploadItem.CCT_CODE))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.CCT_CODE) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {

                            uploadItem.CCT_CODE = uploadItem.CCT_CODE.Trim();

                        }
                        ////FUND CODE
                        //if (string.IsNullOrWhiteSpace(uploadItem.FUND_CODE))
                        //{
                        //    uploadItem.ErrorFlag = true;
                        //    uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.FUND) + ConstantVariableService.HtmlNewLine;
                            
                        //}
                        //else
                        //{

                        //    uploadItem.FUND_CODE = uploadItem.FUND_CODE.Trim();

                        //}
                        //COMPANY 
                        if (string.IsNullOrWhiteSpace(uploadItem.COMPANY_CODE))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.COMPANY_CODE) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            var validCompany = entcompany.Where(m => m.BAN_COMPANY == uploadItem.COMPANY_CODE.Trim().ToUpper()).FirstOrDefault();
                            if (validCompany == null)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.cannot_found_data, ResourceText.COMPANY_CODE) + ConstantVariableService.HtmlNewLine;
                                //
                            }
                            else
                            {
                                uploadItem.COMPANY_CODE = uploadItem.COMPANY_CODE.Trim().ToUpper();
                            }

                        }
                        //INV NO.
                        if (string.IsNullOrWhiteSpace(uploadItem.INV_NO))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.INV_NO) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            uploadItem.INV_NO = uploadItem.INV_NO.Trim();
                        }
                        ////PO NO.
                        uploadItem.PRO_NO = uploadItem.PRO_NO.Trim();
                        //if (string.IsNullOrWhiteSpace(uploadItem.PRO_NO))
                        //{
                        //    //uploadItem.ErrorFlag = true;
                        //    //uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.PRO_NO) + ConstantVariableService.HtmlNewLine;
                        //    //
                        //}
                        //else
                        //{

                        //    uploadItem.PRO_NO = uploadItem.PRO_NO.Trim();
                        //}
                        //IS_STATUS.
                        if (string.IsNullOrWhiteSpace(uploadItem.IS_STATUS))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.STATUS) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.IS_STATUS.Trim());
                            if (decimalResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.STATUS) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                var chkStatus = Enumerable.Range(0, 4).ToList().Contains(Convert.ToInt32(decimalResult.ReturnResult));
                                if (!chkStatus)
                                {
                                    uploadItem.ErrorFlag = true;
                                    uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.STATUS + " {ex. IS_STATUS =1 format is [0,1,2,3] }" )+ ConstantVariableService.HtmlNewLine;
                                    
                                }
                                else
                                {
                                    uploadItem.IS_STATUS = uploadItem.IS_STATUS.Trim();
                                }
                            }
                        }
                        //PRO_REC_DATE
                        if (!string.IsNullOrWhiteSpace(uploadItem.PRO_REC_DATE))
                        {
                            datetimeResult = UtilityService.ConvertStringToDateTime(uploadItem.PRO_REC_DATE.Trim());
                            if (datetimeResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format_datetime, ResourceText.PRO_REC_DATE) + ConstantVariableService.HtmlNewLine;
                                
                            }
                        }
                        //PRO_DUE_DATE
                        if (!string.IsNullOrWhiteSpace(uploadItem.PRO_DUE_DATE))
                        {
                            datetimeResult = UtilityService.ConvertStringToDateTime(uploadItem.PRO_DUE_DATE.Trim());
                            if (datetimeResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format_datetime, ResourceText.PRO_DUE_DATE) + ConstantVariableService.HtmlNewLine;
                                
                            }
                        }
                        //INV_REC_DATE
                        if (!string.IsNullOrWhiteSpace(uploadItem.INV_REC_DATE))
                        {
                            datetimeResult = UtilityService.ConvertStringToDateTime(uploadItem.INV_REC_DATE.Trim());
                            if (datetimeResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format_datetime, ResourceText.INV_REC_DATE) + ConstantVariableService.HtmlNewLine;
                                
                            }
                        }
                        //INV_DUE_DATE
                        if (!string.IsNullOrWhiteSpace(uploadItem.INV_DUE_DATE))
                        {
                            datetimeResult = UtilityService.ConvertStringToDateTime(uploadItem.INV_DUE_DATE.Trim());
                            if (datetimeResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format_datetime, ResourceText.INV_DUE_DATE) + ConstantVariableService.HtmlNewLine;
                                
                            }
                        }
                        ////INV_APPROVED_DATE
                        //if (!string.IsNullOrWhiteSpace(uploadItem.INV_APPROVED_DATE))
                        //{
                        //    datetimeResult = UtilityService.ConvertStringToDateTime(uploadItem.INV_APPROVED_DATE.Trim());
                        //    if (datetimeResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                        //    {
                        //        uploadItem.ErrorFlag = true;
                        //        uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format_datetime, ResourceText.INV_APPROVED_DATE) + ConstantVariableService.HtmlNewLine;

                        //    }
                        //}
                        //else
                        //{
                        //    if (uploadItem.IS_STATUS.Trim() == "3")
                        //    {
                        //        uploadItem.ErrorFlag = true;
                        //        uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.INV_APPROVED_DATE) + ConstantVariableService.HtmlNewLine;

                        //    }
                        //}
                        ////INV_APPROVED_BY
                        //if (string.IsNullOrWhiteSpace(uploadItem.INV_APPROVED_BY))
                        //{
                        //    if (uploadItem.IS_STATUS.Trim() == "3")
                        //    {
                        //        uploadItem.ErrorFlag = true;
                        //        uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.INV_APPROVED_BY) + ConstantVariableService.HtmlNewLine;

                        //    }
                        //}
                        //DISCOUNT
                        if (!string.IsNullOrWhiteSpace(uploadItem.DISCOUNT))
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.DISCOUNT.Trim());
                            if (decimalResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.DISCOUNT) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                uploadItem.DISCOUNT = uploadItem.DISCOUNT.Trim();
                            }
                        }
                        //VAT
                        if (string.IsNullOrWhiteSpace(uploadItem.VAT))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.VAT) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.VAT.Trim());
                            if (decimalResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.VAT) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                uploadItem.VAT = uploadItem.VAT.Trim();
                            }
                        }
                        //WHT
                        if (string.IsNullOrWhiteSpace(uploadItem.WHT))
                        {
                            uploadItem.ErrorFlag = true;
                            uploadItem.Message = uploadItem.Message + ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.WHT) + ConstantVariableService.HtmlNewLine;
                            
                        }
                        else
                        {
                            decimalResult = UtilityService.ConvertStringToDecimal(uploadItem.WHT.Trim());
                            if (decimalResult.ErrorFlag)//|| decimalResult.ReturnResult <= 0)
                            {
                                uploadItem.ErrorFlag = true;
                                uploadItem.Message = uploadItem.Message + string.Format(ValidatorMessage.incorrect_format, ResourceText.WHT) + ConstantVariableService.HtmlNewLine;
                                
                            }
                            else
                            {
                                uploadItem.WHT = uploadItem.WHT.Trim();
                            }
                        }

                      


                        line++;
                    
                    }
                }

                result.ReturnResult = uploadItemList;
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



    }
}