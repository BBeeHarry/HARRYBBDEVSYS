using BBDEVSYS.Services.Abstract;
using BBDEVSYS.ViewModels.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Shared;
using System.Data;
using System.Globalization;
using System.Web.Mvc;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Content.text;
using System.Transactions;
using BBDEVSYS.ViewModels.Shared;
using System.Web.Script.Serialization;
using System.Collections;

namespace BBDEVSYS.Services.Invoice
{
    public class InvoiceService : AbstractControllerService<InvoiceViewModel>
    {
        public override InvoiceViewModel GetDetail(int id)
        {
            InvoiceViewModel invModel = new InvoiceViewModel();

            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return invModel;
        }

        public string GetList(string companyCode, int monthS, int monthE, int yearS, int yearE, string pymName, string status)
        {
            try
            {

                //User Type
                User user = UserService.GetSessionUserInfo();
                string dataList = "";
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<string> statusList = js.Deserialize<List<string>>(status);
                List<InvoiceViewModel> paymentitemsList = new List<InvoiceViewModel>();
                List<InvoiceViewModel> data = new List<InvoiceViewModel>();
                if (statusList.Any())
                {
                    int dataCount = 0;
                    foreach (var item in statusList)
                    {
                        data = GetPaymentItemsList(companyCode, monthS, monthE, yearS, yearE, pymName, item, dataCount);
                        paymentitemsList.AddRange(data);
                        dataCount = paymentitemsList.Count();
                    }
                }
                else
                {
                    paymentitemsList = GetPaymentItemsList(companyCode, monthS, monthE, yearS, yearE, pymName, string.Empty, 0);
                }

                dataList = DatatablesService.ConvertObjectListToDatatables<InvoiceViewModel>(paymentitemsList);

                return dataList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<InvoiceViewModel> GetPaymentItemsList(string companyCode, int monthS, int monthE, int yearS, int yearE, string pymName, string status, int seq_item)
        {
            List<InvoiceViewModel> getInvList = new List<InvoiceViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    //User Type
                    User user = UserService.GetSessionUserInfo();
                    //JavaScriptSerializer js = new JavaScriptSerializer();
                    //List<string> statusList = js.Deserialize<List<string>>(status);



                    var getPaymentItemList = (from data in context.PAYMENT_ITEMS where data.IS_ACTIVE == true orderby data.GROUP_SEQ_CHANNELS, data.ID select data).ToList();
                    var getFeeInvList = (from data in context.FEE_INVOICE select data).ToList();
                    var entUser = (from m in context.USERS select m).ToList();
                    var entCompany = (from m in context.COMPANies where m.IsPaymentFee == true orderby m.Bussiness_Unit select m).ToList();

                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        getPaymentItemList = getPaymentItemList.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        getFeeInvList = getFeeInvList.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        entCompany = entCompany.Where(m => m.BAN_COMPANY == companyCode).ToList();
                    }

                    if (!string.IsNullOrEmpty(pymName))
                    {
                        getPaymentItemList = getPaymentItemList.Where(m => m.PAYMENT_ITEMS_NAME == pymName).ToList();
                        getFeeInvList = getFeeInvList.Where(m => getPaymentItemList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                    }

                    if (!string.IsNullOrEmpty(status) && status != "0")
                    {
                        getFeeInvList = getFeeInvList.Where(m => m.IS_STATUS == status).ToList();

                        getPaymentItemList = getPaymentItemList.Where(m => getFeeInvList.GroupBy(g => g.PAYMENT_ITEMS_CODE).Any(o => m.PAYMENT_ITEMS_CODE == o.Key)).ToList();
                        entCompany = entCompany.Where(m => getFeeInvList.GroupBy(g => g.COMPANY_CODE).Any(o => m.BAN_COMPANY == o.Key)).ToList();
                    }



                    int seq = seq_item + 1;

                    var get_month = (yearE * 12 + monthE) - (yearS * 12 + monthS);

                    foreach (var item in getPaymentItemList)
                    {
                        int countmonth = monthS;
                        int countyear = yearS;
                        for (int i = 0; i <= get_month; i++)
                        {

                            if (countmonth > 12)
                            {
                                countyear = yearS + 1;
                                countmonth = 1;
                            }
                            var getFeeInvDataList = (from m in getFeeInvList
                                                     where m.INV_MONTH == countmonth && m.INV_YEAR == countyear
                                                     select m).ToList();


                            if (getFeeInvDataList.Count > 0)
                            {

                                var getFeeInvItem = !string.IsNullOrEmpty(companyCode) ? getFeeInvDataList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.COMPANY_CODE == item.COMPANY_CODE).FirstOrDefault()
                          : getFeeInvDataList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).FirstOrDefault();


                                if (getFeeInvItem != null)
                                {

                                    if (status == "0" && getFeeInvItem.IS_STATUS != "0")
                                    {
                                        countmonth++;
                                        continue;
                                    }

                                    var getUserName = entUser.Where(m => m.USERID == getFeeInvItem.INV_REC_BY).FirstOrDefault();
                                    var getCompany = entCompany.Where(m => m.BAN_COMPANY == getFeeInvItem.COMPANY_CODE).FirstOrDefault();

                                    var invModel = new InvoiceViewModel();
                                    //AuthoAdmin
                                    invModel.AuthAdmin = user.AuthorizeAdmin;

                                    invModel.ITEM = seq;
                                    invModel.GL_ACCOUNT = item.GL_ACCOUNT;
                                    invModel.COST_CENTER = item.COST_CENTER;
                                    invModel.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;
                                    invModel.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;

                                    invModel.INV_MONTH = getFeeInvItem.INV_MONTH ?? 0;
                                    invModel.COMPANY_CODE = getFeeInvItem.COMPANY_CODE;
                                    invModel.INV_YEAR = getFeeInvItem.INV_YEAR ?? 0;
                                    invModel.NET_AMOUNT = getFeeInvItem.NET_AMOUNT ?? 0;
                                    invModel.INV_REC_BY_TEXT = getUserName == null ? "" : getUserName.NAME;
                                    invModel.INV_REC_DATE = getFeeInvItem.INV_REC_DATE;
                                    invModel.INV_APPROVED_DATE = getFeeInvItem.INV_APPROVED_DATE;
                                    invModel.PRO_REC_DATE = getFeeInvItem.PRO_REC_DATE;
                                    invModel.IS_STATUS = getFeeInvItem.IS_STATUS;

                                    invModel.STATUS = getNameStatus(getFeeInvItem.IS_STATUS);
                                    invModel.ID = getFeeInvItem.ID;
                                    invModel.INV_NO = getFeeInvItem.INV_NO;
                                    invModel.COMPANY_NAME = getCompany != null ? getCompany.COMPANY_NAME_EN : "";
                                    invModel.MONTH_NAME = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName((getFeeInvItem.INV_MONTH ?? 0));
                                    //invModel.MONTH_NAME = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName((getFeeInvItem.INV_MONTH??0));

                                    if (invModel.IS_STATUS == "0")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-primary'>" + invModel.STATUS + "</span>";
                                    }
                                    else if (invModel.IS_STATUS == "1")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-warning'>" + invModel.STATUS + "</span>";
                                    }
                                    else if (invModel.IS_STATUS == "2")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-warning'>" + invModel.STATUS + "</span>";
                                    }
                                    else if (invModel.IS_STATUS == "3")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-success'>" + invModel.STATUS + "</span>";
                                    }
                                    else if (invModel.IS_STATUS == "4")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-danger'>" + invModel.STATUS + "</span>";
                                    }

                                    getInvList.Add(invModel);
                                    seq++;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(status) || status == "0")
                                    {
                                        var getCompany = entCompany.Where(m => m.BAN_COMPANY == item.COMPANY_CODE).FirstOrDefault();

                                        var invModel = new InvoiceViewModel();
                                        invModel.ITEM = seq;
                                        invModel.GL_ACCOUNT = item.GL_ACCOUNT;
                                        invModel.COST_CENTER = item.COST_CENTER;
                                        invModel.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;
                                        invModel.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;

                                        invModel.ID = 0;

                                        invModel.NET_AMOUNT = 0;

                                        invModel.STATUS = "Pending";
                                        invModel.IS_STATUS = "0";
                                        invModel.COMPANY_CODE = item.COMPANY_CODE;
                                        invModel.COMPANY_NAME = getCompany != null ? getCompany.COMPANY_NAME_EN : "";
                                        invModel.INV_MONTH = countmonth;
                                        invModel.MONTH_NAME = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(countmonth);
                                        invModel.INV_YEAR = countyear;

                                        if (invModel.IS_STATUS == "0")
                                        {
                                            invModel.STATUS_NAME = "<span class='label label-primary'>" + invModel.STATUS + "</span>";
                                        }
                                        else if (invModel.IS_STATUS == "1")
                                        {
                                            invModel.STATUS_NAME = "<span class='label label-warning'>" + invModel.STATUS + "</span>";
                                        }
                                        else if (invModel.IS_STATUS == "2")
                                        {
                                            invModel.STATUS_NAME = "<span class='label label-warning'>" + invModel.STATUS + "</span>";
                                        }
                                        else if (invModel.IS_STATUS == "3")
                                        {
                                            invModel.STATUS_NAME = "<span class='label label-success'>" + invModel.STATUS + "</span>";
                                        }

                                        getInvList.Add(invModel);
                                        seq++;

                                    }
                                }// end if
                            }
                            else
                            {
                                if (item.DURATION == "Y")
                                {
                                    int mCount = countmonth - 1;
                                    int yCount = countyear;
                                    if (countmonth == 1 && countyear != yearS)
                                    {
                                        mCount = 12;
                                        yCount = countyear - 1;
                                    }

                                    var nongetFeeInvDataList = (from m in getFeeInvList
                                                                where m.INV_MONTH == mCount && m.INV_YEAR == yCount
                                                                select m).ToList();


                                    if (nongetFeeInvDataList.Any())
                                    {
                                        countmonth++;
                                        continue;
                                    }
                                    var ofYeargetFeeInvDataList = (from m in getFeeInvList
                                                                   where m.INV_YEAR == yCount
                                                                   select m).ToList();
                                    if (ofYeargetFeeInvDataList.Any())
                                    {
                                        countmonth++;
                                        continue;
                                    }
                                }
                                if (string.IsNullOrEmpty(status) || status == "0")
                                {
                                    var getCompany = entCompany.Where(m => m.BAN_COMPANY == item.COMPANY_CODE).FirstOrDefault();

                                    var invModel = new InvoiceViewModel();
                                    invModel.ITEM = seq;
                                    invModel.GL_ACCOUNT = item.GL_ACCOUNT;
                                    invModel.COST_CENTER = item.COST_CENTER;
                                    invModel.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;
                                    invModel.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;

                                    invModel.ID = 0;

                                    invModel.NET_AMOUNT = 0;

                                    invModel.STATUS = "Pending";
                                    invModel.IS_STATUS = "0";
                                    invModel.COMPANY_CODE = item.COMPANY_CODE;
                                    invModel.COMPANY_NAME = getCompany != null ? getCompany.COMPANY_NAME_EN : "";
                                    invModel.INV_MONTH = countmonth;
                                    invModel.MONTH_NAME = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(countmonth);
                                    invModel.INV_YEAR = countyear;

                                    if (invModel.IS_STATUS == "0")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-primary'>" + invModel.STATUS + "</span>";
                                    }
                                    else if (invModel.IS_STATUS == "1")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-warning'>" + invModel.STATUS + "</span>";
                                    }
                                    else if (invModel.IS_STATUS == "2")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-warning'>" + invModel.STATUS + "</span>";
                                    }
                                    else if (invModel.IS_STATUS == "3")
                                    {
                                        invModel.STATUS_NAME = "<span class='label label-success'>" + invModel.STATUS + "</span>";
                                    }

                                    getInvList.Add(invModel);
                                    seq++;

                                }
                            }// end if
                            countmonth = countmonth + 1;


                        }//end for
                    }// end foreah





                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return getInvList;
        }

        public InvoiceViewModel GetPaymentItemsList(string comCode)
        {
            InvoiceViewModel model = new InvoiceViewModel();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    if (!string.IsNullOrEmpty(comCode))
                    {
                        var pymItems = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true && m.COMPANY_CODE == comCode orderby m.GROUP_SEQ_CHANNELS select m).ToList();
                        var getpymItemList = pymItems.Select(m => new SelectListItem { Value = m.PAYMENT_ITEMS_NAME, Text = m.PAYMENT_ITEMS_NAME }).ToList();
                        model.PaymentItemList = getpymItemList;
                    }
                    else
                    {
                        var pymItems = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true orderby m.GROUP_SEQ_CHANNELS select m).ToList();
                        var getpymItemList = pymItems.Select(m => m.PAYMENT_ITEMS_NAME).Distinct().Select(m => new SelectListItem { Value = m, Text = m }).ToList();

                        model.PaymentItemList = getpymItemList;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return model;
        }

        private string getNameStatus(string isChecked = "0")
        {
            string status = "Pending";
            try
            {
                //if (isChecked == "1")
                //{
                //    status = "Waiting PO";
                //}
                //if (isChecked == "2")
                //{
                //    status = "Waiting Approved";
                //}
                //if (isChecked == "3")
                //{
                //    status = "Completed";
                //}
                var statusList = ValueHelpService.GetValueHelp("STATUS_INVOICE").Where(m => m.ValueKey == isChecked).FirstOrDefault();

                if (statusList != null)
                {
                    status = statusList.ValueText;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return status;
        }



        public override string GetList()
        {

            try
            {
                string dataList = "";


                return dataList;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public InvoiceViewModel InitialListSearch()
        {
            InvoiceViewModel model = new InvoiceViewModel();
            try
            {
                model.INV_MONTH_FROM = 1;
                model.INV_MONTH_TO = DateTime.Today.Month;
                model.INV_YEAR_FROM = DateTime.Today.Year;
                model.INV_YEAR_TO = DateTime.Today.Year;

                var list = new List<SelectListItem>();
                using (var context = new PYMFEEEntities())
                {
                    var getCompanyList = (from data in context.COMPANies where data.IsPaymentFee == true orderby data.Bussiness_Unit select data).ToList();
                    list.AddRange(getCompanyList.Select(com => new SelectListItem { Value = com.BAN_COMPANY.ToString(), Text = com.COMPANY_NAME_EN.ToString() }).ToList());
                    model.CompanyLst = list;

                    var pymItem = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true orderby m.GROUP_SEQ_CHANNELS select m).ToList();
                    var pymList = pymItem.Select(m => m.PAYMENT_ITEMS_NAME).Distinct().Select(m => new SelectListItem { Value = m, Text = m }).ToList();

                    model.PaymentItemList = pymList;



                }
                //--Get Status Invoice
                var getStatus = ValueHelpService.GetValueHelp(ConstantVariableService.STATUSTYPE).ToList();
                model.StatusList = getStatus;

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
                model.MonthLst = list;


                //--Get List Years
                list = new List<SelectListItem>();
                list.AddRange(Enumerable.Range(DateTime.Today.Year - 30, 200)
                      .Select(YearAD => new SelectListItem
                      {
                          Value = (YearAD).ToString(),
                          Text = (YearAD).ToString()
                      }).ToList());

                model.YearLst = list;



            }
            catch (Exception ex)
            {

                throw ex;
            }
            return model;
        }

        public InvoiceDetailViewModel InitialItem(int sequence, string companyCode, string pymItems)
        {
            InvoiceDetailViewModel invoiceItem = new InvoiceDetailViewModel();

            try
            {
                int seq = sequence;
                var list = new List<SelectListItem>();
                var chargeLst = new List<string>();

                using (var context = new PYMFEEEntities())
                {
                    var pymList = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true && m.COMPANY_CODE == companyCode && m.PAYMENT_ITEMS_NAME == pymItems select m).ToList();
                    var pymItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       where m.COMPANY_CODE == companyCode
                                       && m.IS_ACTIVE != false
                                       orderby m.SEQUENCE, m.ID
                                       select m).ToList();

                    pymItemList = pymItemList.Where(m => pymList.Any(p => m.PAYMENT_ITEMS_ID == p.ID)).ToList();

                    foreach (var item in pymItemList)
                    {
                        chargeLst.Add(item.PAYMENT_ITEMS_FEE_NAME);
                    }

                    list.AddRange(chargeLst.Select((charge, index) => new SelectListItem
                    {
                        Value = charge,//(index + 1).ToString(),
                        Text = charge
                    }).ToList());

                    invoiceItem.PaymentItemsFeeItemList = list;

                    invoiceItem.SEQUENCE = seq;
                }

            }
            catch (Exception ex)
            {
            }

            return invoiceItem;
        }

        public InvoiceViewModel GetDetail(int recordKey, int monthID, int yearID, string companyCode, string paymentItemCode, string formState)
        {
            InvoiceViewModel invModel = NewFormData();
            try
            {
                //User Type
                User user = UserService.GetSessionUserInfo();

                using (var context = new PYMFEEEntities())
                {
                    var getFeeInvList = (from data in context.FEE_INVOICE where data.ID == recordKey select data).FirstOrDefault();
                    invModel.CONDITION_PYM = "Within 30 days Due Net";

                    invModel.CREATE_BY = user.UserCode;
                    invModel.CREATE_DATE = DateTime.Now;

                    invModel.FormAction = ConstantVariableService.FormActionCreate;
                    invModel.FormState = ConstantVariableService.FormStateCreate;
                    invModel.INV_APPROVED_BY = ConstantVariableService.APPROVERID;
                    //--Get Data Invoice
                    if (getFeeInvList != null)
                    {
                        MVMMappingService.MoveData(getFeeInvList, invModel);

                        var getFeeInvItemList = (from data in context.FEE_INVOICE_ITEM where data.INV_NO == getFeeInvList.INV_NO select data).ToList();
                        invModel.FormAction = formState;
                        invModel.FormState = formState;

                        List<InvoiceDetailViewModel> feeInvItemLst = new List<InvoiceDetailViewModel>();
                        foreach (var item in getFeeInvItemList)
                        {
                            var getFeeInvItem = new InvoiceDetailViewModel();
                            MVMMappingService.MoveData(item, getFeeInvItem);
                            getListChargeItem(getFeeInvItem);

                            feeInvItemLst.Add(getFeeInvItem);

                        }
                        invModel.InvoiceDetailList.AddRange(feeInvItemLst);

                        companyCode = invModel.COMPANY_CODE;
                        monthID = invModel.INV_MONTH;
                        yearID = invModel.INV_YEAR;
                        paymentItemCode = invModel.PAYMENT_ITEMS_CODE;

                        //get Upload
                        if ((invModel.UPLOAD_TYPE ?? false))
                        {
                            invModel.UPLOAD_TYPE_NAME = "Data Upload Automation";
                        }
                        else
                        {
                            invModel.UPLOAD_TYPE_NAME = "Data Input Manual";
                        }

                    }

                    var getCompany = (from data in context.COMPANies where data.IsPaymentFee == true && data.BAN_COMPANY == companyCode select data).FirstOrDefault();
                    var getPaymentItems = (from data in context.PAYMENT_ITEMS where data.IS_ACTIVE == true && data.PAYMENT_ITEMS_CODE == paymentItemCode && data.COMPANY_CODE == companyCode select data).FirstOrDefault();

                    int paymentitemId = getPaymentItems != null ? getPaymentItems.ID : 0;
                    var getPaymentItemsCharge = (from data in context.PAYMENT_ITEMS_CHAGE where data.PAYMENT_ITEMS_ID == paymentitemId && data.COMPANY_CODE == companyCode orderby data.SEQUENCE select data).ToList();
                    if (formState != ConstantVariableService.FormStateDisplay)
                    {
                        getPaymentItemsCharge = getPaymentItemsCharge.Where(m => m.IS_ACTIVE != false).ToList();

                    }

                    var getcttList = (from data in context.COST_CENTER where data.COMPANY_CODE == companyCode select data).ToList();
                    invModel.COMPANY_NAME = getCompany != null ? getCompany.COMPANY_NAME_EN : "";
                    invModel.PAYMENT_ITEMS_NAME = getPaymentItems != null ? getPaymentItems.PAYMENT_ITEMS_NAME : "";
                    invModel.MONTH_NAME = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(monthID));
                    invModel.YEAR_NAME = yearID.ToString();

                    invModel.COMPANY_CODE = companyCode;
                    invModel.INV_MONTH = monthID;
                    invModel.INV_YEAR = yearID;
                    invModel.PAYMENT_ITEMS_CODE = paymentItemCode;

                    invModel.CCT_CODE = getPaymentItems.CCT_CODE;
                    invModel.CHANNELS = getPaymentItems.CHANNELS;

                    invModel.GL_ACCOUNT = getPaymentItems.GL_ACCOUNT;// getcttList.Where(m => m.CCT_CODE == invModel.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                    invModel.COST_CENTER = getPaymentItems.COST_CENTER; //getcttList.Where(m => m.CCT_CODE == invModel.CCT_CODE).FirstOrDefault().COST_CENTER1;

                    invModel.UPLOAD_TYPE = false;

                    invModel.INV_REC_BY = user.UserCode;

                    #region generate List Charge Auto
                    if (string.IsNullOrEmpty(invModel.INV_NO))
                    {
                        List<InvoiceDetailViewModel> feeInvItemLst = new List<InvoiceDetailViewModel>();




                        var list = new List<SelectListItem>();
                        int seq = 1;
                        foreach (var item in getPaymentItemsCharge)
                        {
                            var chargeLst = new List<string>();
                            var getFeeInvItem = new InvoiceDetailViewModel();
                            item.SEQUENCE = seq;
                            MVMMappingService.MoveData(item, getFeeInvItem);

                            getFeeInvItem.PAYMENT_ITEMS_FEE_ITEM = item.PAYMENT_ITEMS_FEE_NAME;
                            //getListChargeItem(getFeeInvItem);
                            chargeLst.Add(item.PAYMENT_ITEMS_FEE_NAME);
                            list.AddRange(chargeLst.Select((charge, index) => new SelectListItem
                            {
                                Value = charge,//(index + 1).ToString(),
                                Text = charge
                            }).ToList());

                            getFeeInvItem.PaymentItemsFeeItemList = list;

                            feeInvItemLst.Add(getFeeInvItem);

                            seq++;
                        }
                        invModel.InvoiceDetailList.AddRange(feeInvItemLst);
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return invModel;
        }
        public void getListChargeItem(InvoiceDetailViewModel lst)
        {

            try
            {
                var list = new List<SelectListItem>();
                var chargeLst = new List<string>();

                using (var context = new PYMFEEEntities())
                {
                    var pymList = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true && m.COMPANY_CODE == lst.COMPANY_CODE && m.PAYMENT_ITEMS_CODE == lst.PAYMENT_ITEMS_CODE select m).ToList();
                    var pymItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       where m.COMPANY_CODE == lst.COMPANY_CODE
                                       && m.IS_ACTIVE != false
                                       orderby m.SEQUENCE, m.ID
                                       select m).ToList();

                    pymItemList = pymItemList.Where(m => pymList.Any(p => m.PAYMENT_ITEMS_ID == p.ID)).ToList();

                    foreach (var item in pymItemList)
                    {
                        chargeLst.Add(item.PAYMENT_ITEMS_FEE_NAME);
                    }

                    list.AddRange(chargeLst.Select((charge, index) => new SelectListItem
                    {
                        Value = charge,//(index + 1).ToString(),
                        Text = charge
                    }).ToList());

                    lst.PaymentItemsFeeItemList = list;
                }

            }
            catch (Exception ex)
            {
            }

        }
        public List<InvoiceViewModel> GetPaymentItemsList()
        {
            List<InvoiceViewModel> getInvList = new List<InvoiceViewModel>();
            try
            {
                #region Test 

                InvoiceViewModel model = new InvoiceViewModel();

                model.ITEM = 1;
                model.GL_ACCOUNT = "630080060";
                model.COST_CENTER = "19ITC60000";
                model.PAYMENT_ITEMS_NAME = "TEST DESC";
                model.NET_AMOUNT = 100;
                model.INV_REC_BY_TEXT = "Mrs. Test";
                model.INV_REC_DATE = DateTime.ParseExact("2018-07-01", "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                model.STATUS = "Pending"; ;
                model.INV_APPROVED_DATE = DateTime.ParseExact("2018-07-01", "yyyy-mm-dd", System.Globalization.CultureInfo.InvariantCulture);
                model.STATUS = "Pending";
                model.ID = 1;
                model.PAYMENT_ITEMS_CODE = "TEST DESC";


                getInvList.Add(model);

                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return getInvList;
        }

        public override InvoiceViewModel NewFormData()
        {
            InvoiceViewModel invModel = new InvoiceViewModel();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var userApp = (from m in context.USERS where m.GROUP_POSITION == "Approved" select m).ToList();
                    invModel.UserApprovedList = userApp.Select(m => new SelectListItem
                    {
                        Value = m.USERID,
                        Text = m.USERID + " " + m.NAME
                    }).ToList();

                    invModel.INV_DUE_DATE = DateTime.Now;
                    invModel.PRO_REC_DATE = DateTime.Now;
                    invModel.PRO_DUE_DATE = DateTime.Now;
                    invModel.INV_REC_DATE = DateTime.Now;

                    invModel.StatusList = ValueHelpService.GetValueHelp("STATUS_INVOICE");
                    invModel.IS_STATUS = "0";
                    invModel.INV_APPROVED_BY = ConstantVariableService.APPROVERID;
                    //invModel.INV_APPROVED_DATE = DateTime.Now;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return invModel;
        }

        public override ValidationResult SaveCreate(InvoiceViewModel formData, ModelStateDictionary modelState)
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
                    var entFeeInv = new FEE_INVOICE();
                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entFeeInv);
                    //entFeeInv.IS_STATUS = "2";
                    if (!(formData.UPLOAD_TYPE ?? false))
                    {

                        //entfeeInv.IS_STATUS = "1";
                        if (entFeeInv.IS_STATUS == "3")
                        {
                            entFeeInv.IS_STATUS = formData.IS_STATUS;
                        }
                        else if (entFeeInv.IS_STATUS == "4")
                        {
                            entFeeInv.IS_STATUS = formData.IS_STATUS;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(formData.PRO_NO))
                            {
                                entFeeInv.IS_STATUS = "1";
                            }
                            if (!string.IsNullOrEmpty(formData.PRO_NO))
                            {
                                entFeeInv.IS_STATUS = "2";
                            }
                            if (formData.INV_APPROVED_DATE != null)
                            {
                                entFeeInv.IS_STATUS = "3";
                            }
                            //Status Not Accrued
                            if (formData.REMARK == "Not Accrued")
                            {
                                entFeeInv.IS_STATUS = "0";
                            }
                            //Status Not Accrued Not Create PRO No.
                            if (formData.REMARK == "For Accrued")
                            {
                                entFeeInv.IS_STATUS = "1";
                            }
                        }
                    }


                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {
                        context.FEE_INVOICE.Add(entFeeInv);
                        context.SaveChanges();

                        //Save Item
                        //Copy data from viewmodel to model - for line item
                        int sequence = 1;
                        if (formData.InvoiceDetailList != null)
                        {
                            foreach (var item in formData.InvoiceDetailList)
                            {

                                if (item.DeleteFlag)
                                {
                                    continue;
                                }

                                var entFeeInvItem = new FEE_INVOICE_ITEM();
                                item.INV_NO = entFeeInv.INV_NO;
                                item.SEQUENCE = sequence;
                                item.INV_ITEM_NO = entFeeInv.INV_NO + entFeeInv.INV_MONTH + "_" + sequence;
                                item.INV_MONTH = entFeeInv.INV_MONTH;
                                item.INV_YEAR = entFeeInv.INV_YEAR;
                                item.PAYMENT_ITEMS_CODE = entFeeInv.PAYMENT_ITEMS_CODE;
                                item.COMPANY_CODE = entFeeInv.COMPANY_CODE;
                                //item.CHANNELS = entFeeInv.CHANNELS;
                                //item.GL_ACCOUNT = entFeeInv.GL_ACCOUNT;
                                //item.COST_CENTER = entFeeInv.COST_CENTER;
                                //item.FUND_CODE = entFeeInv.FUND_CODE;


                                MVMMappingService.MoveData(item, entFeeInvItem);
                                context.FEE_INVOICE_ITEM.Add(entFeeInvItem);
                                context.SaveChanges();

                                sequence++;

                            }
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

        public override ValidationResult SaveDelete(InvoiceViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    var entfeeInv = new FEE_INVOICE();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entfeeInv);

                    using (var context = new PYMFEEEntities())
                    {
                        //Delete header            
                        context.Entry(entfeeInv).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();

                        //Delete item
                        context.FEE_INVOICE_ITEM.RemoveRange(context.FEE_INVOICE_ITEM.Where(m => m.INV_NO == entfeeInv.INV_NO));
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

        public override ValidationResult SaveEdit(InvoiceViewModel formData, ModelStateDictionary modelState)
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
                    FEE_INVOICE entfeeInv = new FEE_INVOICE();



                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entfeeInv);





                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {

                        context.Entry(entfeeInv).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        //Save Item                        
                        int sequence = 1;

                        foreach (var item in formData.InvoiceDetailList)
                        {
                            var entfeeInvItem = new FEE_INVOICE_ITEM();
                            item.INV_NO = entfeeInv.INV_NO;
                            item.SEQUENCE = sequence;
                            item.INV_ITEM_NO = item.INV_NO + "_" + sequence;
                            item.INV_MONTH = entfeeInv.INV_MONTH;
                            item.INV_YEAR = entfeeInv.INV_YEAR;
                            item.PAYMENT_ITEMS_CODE = entfeeInv.PAYMENT_ITEMS_CODE;
                            item.COMPANY_CODE = entfeeInv.COMPANY_CODE;
                            //item.CHANNELS = entfeeInv.CHANNELS;
                            //item.GL_ACCOUNT = entfeeInv.GL_ACCOUNT;
                            //item.COST_CENTER = entfeeInv.COST_CENTER;
                            //item.FUND_CODE = entfeeInv.FUND_CODE;

                            MVMMappingService.MoveData(item, entfeeInvItem);
                            if (item.DeleteFlag)

                            {
                                if (entfeeInvItem.ID != 0)
                                {
                                    context.Entry(entfeeInvItem).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                            else
                            {

                                entfeeInvItem.SEQUENCE = sequence;
                                sequence++;
                                if (entfeeInvItem.ID != 0)
                                {
                                    context.Entry(entfeeInvItem).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    item.INV_ITEM_NO = item.INV_ITEM_NO;
                                    context.FEE_INVOICE_ITEM.Add(entfeeInvItem);
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

        public override ValidationResult SaveClose(InvoiceViewModel formData, ModelStateDictionary modelState)
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
                    FEE_INVOICE entfeeInv = new FEE_INVOICE();



                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entfeeInv);
                    entfeeInv.IS_STATUS = "3";


                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {

                        context.Entry(entfeeInv).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        #region Update Item
                        ////Save Item                        
                        //int sequence = 1;

                        //foreach (var item in formData.InvoiceDetailList)
                        //{
                        //    var entfeeInvItem = new FEE_INVOICE_ITEM();
                        //    item.INV_NO = entfeeInv.INV_NO;
                        //    item.SEQUENCE = sequence;
                        //    item.INV_ITEM_NO = entfeeInv.INV_NO + "_" + sequence;
                        //    MVMMappingService.MoveData(item, entfeeInvItem);
                        //    if (item.DeleteFlag)
                        //    {
                        //        if (entfeeInvItem.ID != 0)
                        //        {
                        //            context.Entry(entfeeInvItem).State = System.Data.Entity.EntityState.Deleted;
                        //        }
                        //    }
                        //    else
                        //    {

                        //        entfeeInvItem.SEQUENCE = sequence;
                        //        sequence++;
                        //        if (entfeeInvItem.ID != 0)
                        //        {
                        //            context.Entry(entfeeInvItem).State = System.Data.Entity.EntityState.Modified;
                        //        }
                        //        else
                        //        {
                        //            context.FEE_INVOICE_ITEM.Add(entfeeInvItem);
                        //        }
                        //    }

                        //    context.SaveChanges();

                        //}
                        #endregion

                    }

                    //Commit Header and Item
                    scope.Complete();

                    result.Message = ResourceText.SuccessfulCloseInvoice;
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

        public override ValidationResult ValidateFormData(InvoiceViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            try
            {
                var pym_items = new List<PAYMENT_ITEMS_CHAGE>();
                using (var context = new PYMFEEEntities())
                {
                    pym_items = (from m in context.PAYMENT_ITEMS
                                 join n in context.PAYMENT_ITEMS_CHAGE
                                 on m.ID equals n.PAYMENT_ITEMS_ID
                                 where m.IS_ACTIVE == true && n.COMPANY_CODE == formData.COMPANY_CODE && m.PAYMENT_ITEMS_CODE == formData.PAYMENT_ITEMS_CODE
                                 select n).ToList();

                }
                if (!modelState.IsValid)
                {
                    result.ModelStateErrorList = UtilityService.GetModelStateErrors(modelState);
                    result.ErrorFlag = true;
                }
                else
                {
                    if (formData.IS_STATUS == "4")
                    {
                        return result;
                    }
                    //Get item => deleteFlag != true
                    var itemList = formData.InvoiceDetailList.Where(m => m.DeleteFlag != true).ToList();
                    if (itemList == null || !itemList.Any()) //Check list is null or empty
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_listnull_error, string.Concat(ResourceText.TitlePrefixList, ResourceText.Charge))));

                        result.ErrorFlag = true;
                    }
                    else
                    {

                        //Check item
                        int line = 1;
                        foreach (var item in itemList)
                        {
                            if ((item.RATE_AMT ?? 0) == 0 && (item.ACTUAL_AMOUNT ?? 0) == 0 && (item.RATE_TRANS ?? 0) == 0 && (item.TRANSACTIONS ?? 0) == 0)

                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.TRANSACTION, line.ToString())));
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.RATE_TRANS, line.ToString())));
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.ACTUAL_AMOUNT, line.ToString())));
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.RATE_AMT, line.ToString())));
                                result.ErrorFlag = true;
                            }
                            //product id not empty
                            if (item.PAYMENT_ITEMS_FEE_ITEM == null)
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.PAYMENT_ITEMS_FEE_ITEM, line.ToString())));

                                result.ErrorFlag = true;
                            }

                            if ((item.RATE_TRANS ?? 0) != 0 && (item.TRANSACTIONS ?? 0) == 0)
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.TRANSACTION, line.ToString())));

                                result.ErrorFlag = true;
                            }
                            if ((item.RATE_AMT ?? 0) != 0 && (item.ACTUAL_AMOUNT ?? 0) == 0)
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.ACTUAL_AMOUNT, line.ToString())));

                                result.ErrorFlag = true;
                            }
                            //if ((item.TOTAL_CHARGE_AMOUNT ?? 0) == 0)
                            //{
                            //    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.cannot_found_data, ResourceText.TOTAL_CHARGE_AMOUNT, line.ToString())));
                            //    result.ErrorFlag = true;
                            //}

                            #region check value transaction & amount
                            if (pym_items != null)
                            {
                                var get_pym = pym_items.Where(m => m.PAYMENT_ITEMS_FEE_NAME == item.PAYMENT_ITEMS_FEE_ITEM).FirstOrDefault();
                                if (get_pym != null)
                                {
                                    if (string.Equals(get_pym.CHARGE_TYPE, "TRXN", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if ((item.RATE_AMT ?? 0) != 0 || (item.ACTUAL_AMOUNT ?? 0) != 0)
                                        {
                                            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.incorrect_format_at, ResourceText.PAYMENT_ITEMS_FEE_ITEM, line.ToString())));

                                            result.ErrorFlag = true;
                                        }
                                    }
                                    if (string.Equals(get_pym.CHARGE_TYPE, "MDR", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if ((item.RATE_TRANS ?? 0) != 0)
                                        {
                                            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.incorrect_format_at, ResourceText.PAYMENT_ITEMS_FEE_ITEM, line.ToString())));

                                            result.ErrorFlag = true;
                                        }
                                    }
                                }
                            }
                            #endregion

                            var dupPaymentItem = itemList.GroupBy(m => m.PAYMENT_ITEMS_FEE_ITEM).Where(m => m.Count() > 1).ToList();
                            foreach (var itemDup in dupPaymentItem)
                            {
                                //--wainting test upload invoice
                                if (item.PAYMENT_ITEMS_FEE_ITEM == itemDup.Key)
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
                            if (formData.FormAction == ConstantVariableService.FormActionCreate)
                            {
                                var InvoiceNo = (from m in context.FEE_INVOICE where m.INV_NO == formData.INV_NO.Trim() select m).ToList();
                                if (InvoiceNo.Any())
                                {
                                    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, ResourceText.INV_NO + " " + formData.INV_NO)));
                                    result.ErrorFlag = true;
                                }
                            }
                            //Dup In Data Invoice
                            if (formData.FormAction == ConstantVariableService.FormActionEdit)
                            {
                                var InvoiceNo = (from m in context.FEE_INVOICE where m.INV_NO == formData.INV_NO.Trim() && m.ID != formData.ID select m).ToList();
                                if (InvoiceNo.Any())
                                {
                                    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, ResourceText.INV_NO + " " + formData.INV_NO)));
                                    result.ErrorFlag = true;
                                }
                               var checkInvoiceNo = (from m in context.FEE_INVOICE where m.INV_NO == formData.INV_NO.Trim() && m.ID == formData.ID select m).FirstOrDefault();
                                string status = checkInvoiceNo == null ?string.Empty : checkInvoiceNo.IS_STATUS;
                                if (formData.IS_STATUS == status)
                                {
                                    if (!(formData.UPLOAD_TYPE ?? false))
                                    {
                                        if (string.IsNullOrEmpty(formData.PRO_NO))
                                        {
                                            formData.IS_STATUS = "1";
                                        }
                                        if (!string.IsNullOrEmpty(formData.PRO_NO))
                                        {
                                            formData.IS_STATUS = "2";
                                        }
                                        if (formData.INV_APPROVED_DATE != null)
                                        {
                                            formData.IS_STATUS = "3";
                                        }
                                        //Status Not Accrued
                                        if (formData.REMARK == "Not Accrued")
                                        {
                                            formData.IS_STATUS = "0";
                                        }
                                        //Status Not Accrued Not Create PRO No.
                                        if (formData.REMARK == "For Accrued")
                                        {
                                            formData.IS_STATUS = "1";
                                        }

                                    }
                                }
                                
                            }
                            //Check Invoice Close
                            if (formData.FormAction == ConstantVariableService.FormActionClosed)
                            {
                                validateCheckPreStatus(formData, result, formData.FormAction);
                            }
                            else
                            {
                                //Chech Status
                                if (formData.IS_STATUS == "3")
                                {
                                    validateCheckPreStatus(formData, result, formData.FormAction);

                                }
                                //Chech Status
                                else if (formData.IS_STATUS == "2")
                                {
                                    validateCheckPreStatus(formData, result, formData.FormAction);
                                }
                                //Chech Status
                                else if (formData.IS_STATUS == "1")
                                {
                                    validateCheckPreStatus(formData, result, formData.FormAction);
                                }
                                //else if (formData.INV_APPROVED_BY!=null && formData.INV_APPROVED_DATE == null)
                                //{
                                //    validateCheckPreStatus(formData, result, formData.FormAction);
                                //}
                                else if (formData.INV_APPROVED_BY == null && formData.INV_APPROVED_DATE != null)
                                {
                                    validateCheckPreStatus(formData, result, formData.FormAction);
                                }
                                else if (formData.INV_APPROVED_BY != null && formData.INV_APPROVED_DATE != null)
                                {
                                    validateCheckPreStatus(formData, result, formData.FormAction);
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

        private void validateCheckPreStatus(InvoiceViewModel formData, ValidationResult result, string formAction)
        {
            try
            {
                bool chkError = false;
                if (!(formData.UPLOAD_TYPE ?? false))
                {
                    if (formAction == ConstantVariableService.FormActionClosed)
                    {
                        formData.IS_STATUS = "3";
                    }
                    var getStatus = ValueHelpService.GetValueHelp("STATUS_INVOICE").Where(m => m.ValueKey == formData.IS_STATUS).FirstOrDefault();
                    int intStatus = Convert.ToInt32(formData.IS_STATUS);
                    string statusName = getStatus == null ? "" : getStatus.ValueText;
                    if (string.IsNullOrEmpty(formData.PRO_NO) && intStatus > 1)
                    {
                        chkError = true;
                    }
                    //if (string.IsNullOrEmpty(formData.INV_APPROVED_BY) && intStatus > 1)
                    //{
                    //    chkError = true;
                    //}
                    if (formData.INV_APPROVED_DATE != null && string.IsNullOrEmpty(formData.INV_APPROVED_BY))
                    {
                        chkError = true;
                    }
                    else
                    {
                        if (formData.INV_APPROVED_DATE != null)
                        {
                            if (formData.INV_APPROVED_DATE.Value.Date > DateTime.Now.Date)
                            {
                                chkError = true;
                            }
                        }
                    }
                    if (chkError)
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", "ไม่สามารถทำรายการ แก้ไขสถานะ " + statusName + "ได้  เนื่องจากข้อมูลไม่ถูกต้อง "));
                        //string.Format(ValidatorMessage.duplicate_error, ResourceText.INV_NO + " " + formData.INV_NO)));
                        result.ErrorFlag = true;
                    }
                    if (string.IsNullOrEmpty(formData.PRO_NO) && intStatus > 1)
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.notempty_error_in, ResourceText.PRO_NO)));
                        result.ErrorFlag = true;
                    }
                    if (formData.INV_APPROVED_DATE != null && string.IsNullOrEmpty(formData.INV_APPROVED_BY))
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.notempty_error_in, ResourceText.INV_APPROVED_BY)));
                        result.ErrorFlag = true;
                    }
                    if (formData.INV_APPROVED_DATE == null && formData.IS_STATUS == "3")
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.notempty_error_in, ResourceText.INV_APPROVED_DATE)));
                        result.ErrorFlag = true;
                    }
                    else
                    {
                        if (formData.INV_APPROVED_DATE != null)
                        {
                            if (formData.INV_APPROVED_DATE.Value.Date > DateTime.Now.Date)
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.incorrect_format, ResourceText.INV_APPROVED_DATE)));
                                result.ErrorFlag = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}