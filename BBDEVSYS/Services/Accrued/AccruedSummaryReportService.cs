using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.ViewModels.AccruedReport;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.Models.Shared;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using System.IO;
using System.Globalization;
using CrystalDecisions.Shared;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.ViewModels.Invoice;
using System.Collections;
using System.Reflection;


namespace BBDEVSYS.Services.Accrued
{
    public class AccruedSummaryReportService//: AbstractControllerService<AccruedDetailReportViewModel>
    {
        public AccruedDetailReportViewModel InitialListSearch()
        {
            AccruedDetailReportViewModel model = new AccruedDetailReportViewModel();
            try
            {
                model.START_MONTH = 1;// DateTime.Today.Month;
                model.END_MONTH = DateTime.Today.Month;
                model.START_YEAR = DateTime.Today.Year;
                model.END_YEAR = DateTime.Today.Year;

                var list = new List<SelectListItem>();
                using (var context = new PYMFEEEntities())
                {
                    var getCompanyList = (from data in context.COMPANies where data.IsPaymentFee == true orderby data.Bussiness_Unit select data).ToList();
                    list.AddRange(getCompanyList.Select(com => new SelectListItem { Value = com.BAN_COMPANY.ToString(), Text = com.COMPANY_NAME_EN.ToString() }).ToList());
                    model.CompanyLst = list;

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

                    //--Get List FeeType
                    list = new List<SelectListItem>();

                    var feeList = new List<string>();
                    feeList.Add("ALL");
                    feeList.Add("Accrued");
                    //feeList.Add("Invoice Expensed");
                    feeList.Add("Actual");
                    list.AddRange(feeList
                          .Select((fee, index) => new SelectListItem
                          {
                              Value = (index + 1).ToString(),
                              Text = (fee).ToString()
                          }).ToList());

                    model.feeTypeLst = list;

                    //--Get List Channel
                    list = new List<SelectListItem>();

                    var channelList = new List<string>();
                    //channelList.Add("ALL");
                    //channelList.Add("Shop");
                    //channelList.Add("iService");
                    //channelList.Add("Kiosk+Wallet");
                    //channelList.Add("IVR");
                    //channelList.Add("BANK & 3rd by TMN");

                    var pymItemChannels = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true select m).ToList();
                    var getChnn = pymItemChannels.Select(m => m.CHANNELS).Distinct().ToList();
                    channelList.Add("ALL");
                    foreach (var item in getChnn)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            channelList.Add(item);
                        }
                    }

                    var selectListItems = channelList.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
                    //list.AddRange(channelList
                    //      .Select( (chn , index)=> new SelectListItem
                    //      {
                    //          Value = (index + 1).ToString(),
                    //          Text = (chn).ToString()
                    //     }).ToList());
                    list.AddRange(selectListItems);
                    model.channelsList = list;

                    list = new List<SelectListItem>();
                    var buList = new List<string>();
                    buList.Add("ALL");
                    buList.Add("Mobile");
                    buList.Add("Online");
                    buList.Add("TV");
                    var selectListbu = buList.Select(x => new SelectListItem() { Value = x, Text = x }).ToList();
                    list.AddRange(selectListbu);
                    model.BusinessUnitLst = list;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return model;
        }

        public AccruedDetailReportViewModel InitialCompany(string bu)
        {
            AccruedDetailReportViewModel viewModel = new AccruedDetailReportViewModel();

            try
            {
                using (var contex = new PYMFEEEntities())
                {
                    var entCompany = (from m in contex.COMPANies where m.IsPaymentFee == true select m).ToList();

                    if (bu != "ALL")
                    {
                        entCompany = entCompany.Where(m => m.Bussiness_Unit == bu).ToList();
                    }
                    var selectItemsCompany = entCompany.Select(x => new SelectListItem() { Value = x.BAN_COMPANY, Text = x.COMPANY_NAME_EN }).ToList();
                    viewModel.BusinessUnitLst = selectItemsCompany;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return viewModel;
        }

        public void SubmitForm(AccruedDetailReportViewModel formData)
        {

            try
            {
                #region GetReportData
                DataTable getDataReport = new DataTable();

                #endregion

                #region Export EXCEL

                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public string GetList(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            try
            {
                string dataList = "";
                List<AccruedDetailReportViewModel> accruedSummaryReportList = GetAccruedSummaryReportList(companyCode, monthS, yearS, monthE, yearE, chnn, fee, bu);

                dataList = DatatablesService.ConvertObjectListToDatatables<AccruedDetailReportViewModel>(accruedSummaryReportList);

                return dataList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<AccruedDetailReportViewModel> GetAccruedSummaryReportList(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedDetailReportViewModel> modelList = new List<AccruedDetailReportViewModel>();
            try
            {
                //DataTable dt = DataGenerateTesting();


                //modelList = DatatablesService.ToListof<AccruedDetailReportViewModel>(dt);

                using (var context = new PYMFEEEntities())
                {

                    //get Company

                    var companyData = (from com in context.COMPANies where com.IsPaymentFee == true select com).ToList();
                    var feeList = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where m.INV_MONTH >= monthS && m.INV_MONTH <= monthE
                                            && m.INV_YEAR >= yearS && m.INV_YEAR <= yearE
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();

                    if (bu != "ALL")
                    {

                        var comBuLst = companyData.Where(m => m.Bussiness_Unit == bu).ToList();
                        entFeeInv = entFeeInv.Where(m => comBuLst.Any(c => m.COMPANY_CODE == c.BAN_COMPANY)).ToList();
                        feeList = (from m in feeList where comBuLst.Any(c => m.COMPANY_CODE == c.BAN_COMPANY) orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();

                    }
                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();

                    }
                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();

                    }


                    if (fee > 1)
                    {
                        if (fee == 4)
                        {
                            entFeeInv = (from m in entFeeInv
                                         where m.IS_STATUS == "3"
                                         orderby m.INV_MONTH, m.INV_YEAR
                                         select m).ToList();
                        }
                        else
                             if (fee == 3)
                        {
                            entFeeInv = (from m in entFeeInv
                                         where m.IS_STATUS != "3"
                                         orderby m.INV_MONTH, m.INV_YEAR
                                         select m).ToList();
                        }

                    }


                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    int rowfirst = 0;
                    foreach (var item in feeList)
                    {
                        rowfirst = 0;
                        var feeInv = (from m in entFeeInv
                                      join n in entFeeInvItem on m.INV_NO equals n.INV_NO
                                      where m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE
                                      orderby n.SEQUENCE
                                      select new { m, n }).ToList();

                        var feeInvGroup = feeInv.GroupBy(m => m.n.PAYMENT_ITEMS_FEE_ITEM).ToList();

                        decimal sumChageAmt1 = 0;
                        decimal sumChageAmt2 = 0;
                        decimal sumChageAmt3 = 0;
                        decimal sumChageAmt4 = 0;
                        decimal sumChageAmt5 = 0;
                        decimal sumChageAmt6 = 0;
                        decimal sumChageAmt7 = 0;
                        decimal sumChageAmt8 = 0;


                        List<AccruedDetailReportViewModel> modelTotalList = new List<AccruedDetailReportViewModel>();
                        foreach (var subitem in feeInvGroup)
                        {
                            rowfirst++;
                            var model = new AccruedDetailReportViewModel();
                            model.CHANNELS = rowfirst == 1 ? item.CHANNELS : "";
                            model.FEE = rowfirst == 1 ? item.PAYMENT_ITEMS_NAME : "";
                            var feeInvList = feeInv.Where(m => m.n.PAYMENT_ITEMS_FEE_ITEM == subitem.Key).ToList();

                            model.CHARGE = subitem.Key;


                            foreach (var sub in feeInvList)
                            {
                                if (sub.m.INV_MONTH == 1)
                                {
                                    model.Jan = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt1 = sumChageAmt1 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                }
                                if (sub.m.INV_MONTH == 2)
                                {
                                    model.Feb = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt2 = sumChageAmt2 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);

                                }
                                if (sub.m.INV_MONTH == 3)
                                {
                                    model.Mar = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt3 = sumChageAmt3 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                }
                                if (sub.m.INV_MONTH == 4)
                                {
                                    model.Apr = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt4 = sumChageAmt4 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                }
                                if (sub.m.INV_MONTH == 5)
                                {
                                    model.May = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt5 = sumChageAmt5 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                }
                                if (sub.m.INV_MONTH == 6)
                                {
                                    model.Jun = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt6 = sumChageAmt6 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                }
                                if (sub.m.INV_MONTH == 7)
                                {
                                    model.Jul = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt7 = sumChageAmt7 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);

                                }
                                if (sub.m.INV_MONTH == 8)
                                {
                                    model.Aug = Convert.ToString(sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    sumChageAmt8 = sumChageAmt8 + (sub.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                }

                            }


                            modelList.Add(model);
                            if (feeInvGroup.Count() == rowfirst)
                            {
                                AccruedDetailReportViewModel modelTotal = new AccruedDetailReportViewModel();
                                modelTotal.CHARGE = "Total";
                                modelTotal.Jan = Convert.ToString(sumChageAmt1);
                                modelTotal.Feb = Convert.ToString(sumChageAmt2);
                                modelTotal.Mar = Convert.ToString(sumChageAmt3);
                                modelTotal.Apr = Convert.ToString(sumChageAmt4);
                                modelTotal.May = Convert.ToString(sumChageAmt5);
                                modelTotal.Jun = Convert.ToString(sumChageAmt6);
                                modelTotal.Jul = Convert.ToString(sumChageAmt7);
                                modelTotal.Aug = Convert.ToString(sumChageAmt8);

                                modelTotalList.Add(modelTotal);
                            }

                        }
                        modelList.AddRange(modelTotalList);

                    }


                }





            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }

        public byte[] SubmitFormFileContent(AccruedDetailReportViewModel formData)
        {
            byte[] filecontent = null;
            try
            {
                DataSet ds = new DataSet();

                using (var context = new PYMFEEEntities())
                {
                    var companyData = (from com in context.COMPANies where com.IsPaymentFee == true orderby com.Bussiness_Unit select com).ToList();

                    if (formData.FEE_TYPE == "1")//All Report
                    {
                        // ds = GetReportList(formData);


                        #region Accrued & Actual

                        if (string.IsNullOrEmpty(formData.COMPANY_CODE))
                        {

                            var comBuLst = companyData.ToList();
                            if (formData.BUSINESS_UNIT != "ALL")
                            {
                                comBuLst = comBuLst.Where(m => m.Bussiness_Unit == formData.BUSINESS_UNIT).ToList();
                            }
                            #region  Summary All Report
                            List<AccruedReportViewModel> data = GetAllAccruedandActualReportList("", formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                            if (data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(data);
                                _dt.TableName = "All";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                            #endregion


                            foreach (var item in comBuLst)
                            {
                                List<AccruedReportViewModel> _data = GetAccruedandActualReportList(item.BAN_COMPANY, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                                if (_data.Any())
                                {
                                    DataTable _dt = new DataTable();
                                    _dt = ReportService.ToDataTable(_data);
                                    _dt.TableName = item.contraction;
                                    //ds = new DataSet();
                                    ds.Tables.Add(_dt);
                                }
                            }
                        }
                        else
                        {
                            #region  Summary All Report
                            List<AccruedReportViewModel> data = GetAllAccruedandActualReportList(formData.COMPANY_CODE, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                            if (data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(data);
                                _dt.TableName = "All";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                            #endregion

                            List<AccruedReportViewModel> _data = GetAccruedandActualReportList(formData.COMPANY_CODE, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                            if (_data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(_data);
                                _dt.TableName = companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault() != null ? companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault().contraction : "";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                        }
                        //}
                        #endregion
                    }
                    else if (formData.FEE_TYPE == "2")//Accrued Value
                    {
                        #region Accrued

                        if (string.IsNullOrEmpty(formData.COMPANY_CODE))
                        {

                            var comBuLst = companyData.ToList();
                            if (formData.BUSINESS_UNIT != "ALL")
                            {
                                comBuLst = comBuLst.Where(m => m.Bussiness_Unit == formData.BUSINESS_UNIT).ToList();
                            }
                            #region  Summary All Report
                            List<AccruedReportViewModel> data = GetAllAccruedReportList_Summary("", formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                            if (data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(data);
                                _dt.TableName = "All";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                            #endregion

                            foreach (var item in comBuLst)
                            {
                                List<AccruedReportViewModel> _data = GetAccruedReportList_Summary(item.BAN_COMPANY, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);
                                if (_data.Any())
                                {
                                    DataTable _dt = new DataTable();
                                    _dt = ReportService.ToDataTable(_data);
                                    _dt.TableName = item.contraction;
                                    //ds = new DataSet();
                                    ds.Tables.Add(_dt);
                                }
                            }
                        }
                        else
                        {
                            #region  Summary All Report
                            List<AccruedReportViewModel> data = GetAllAccruedReportList_Summary(formData.COMPANY_CODE, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                            if (data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(data);
                                _dt.TableName = "All";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                            #endregion

                            List<AccruedReportViewModel> _data = GetAccruedReportList_Summary(formData.COMPANY_CODE, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);
                            if (_data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(_data);
                                _dt.TableName = companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault() != null ? companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault().contraction : "";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                        }
                        //}
                        #endregion

                    }
                    else
                    {
                        #region Actual

                        if (string.IsNullOrEmpty(formData.COMPANY_CODE))
                        {

                            var comBuLst = companyData.ToList();
                            if (formData.BUSINESS_UNIT != "ALL")
                            {
                                comBuLst = comBuLst.Where(m => m.Bussiness_Unit == formData.BUSINESS_UNIT).ToList();
                            }
                            #region  Summary All Report
                            List<AccruedReportViewModel> data = GetAllActualReportList("", formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                            if (data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(data);
                                _dt.TableName = "All";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                            #endregion
                            foreach (var item in comBuLst)
                            {
                                List<AccruedReportViewModel> _data = GetActualReportList(item.BAN_COMPANY, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);
                                if (_data.Any())
                                {
                                    DataTable _dt = new DataTable();
                                    _dt = ReportService.ToDataTable(_data);
                                    _dt.TableName = item.contraction;
                                    //ds = new DataSet();
                                    ds.Tables.Add(_dt);
                                }
                            }
                        }
                        else
                        {
                            #region  Summary All Report
                            List<AccruedReportViewModel> data = GetAllActualReportList(formData.COMPANY_CODE, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);

                            if (data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(data);
                                _dt.TableName = "All";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                            #endregion
                            List<AccruedReportViewModel> _data = GetActualReportList(formData.COMPANY_CODE, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);
                            if (_data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(_data);
                                _dt.TableName = companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault() != null ? companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault().contraction : "";
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                        }
                        //}
                        #endregion

                    }
                } //End  
                if (ds.Tables.Count > 0)
                {
                    string feeType = "";
                    if (formData.FEE_TYPE == "1")
                    {
                        feeType = "ALL";
                    }
                    else if (formData.FEE_TYPE == "2")
                    {

                        feeType = "Accrued";

                    }
                    else
                    {

                        feeType = "Actual";
                    }


                    filecontent = ExcelExportHelper.ExportExcel(ds, feeType + "  Expense " + Convert.ToString(formData.START_YEAR).Substring(2, 2), false, formData);//, columns);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return filecontent;
        }

        private DataSet GetReportList(AccruedDetailReportViewModel formData)
        {
            DataSet ds = new DataSet();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var companyData = (from com in context.COMPANies where com.IsPaymentFee == true select com).ToList();

                    if (formData.BUSINESS_UNIT != "ALL" && string.IsNullOrEmpty(formData.COMPANY_CODE))
                    {

                        var comBuLst = companyData.Where(m => m.Bussiness_Unit == formData.BUSINESS_UNIT).ToList();

                        foreach (var item in comBuLst)
                        {
                            List<AccruedReportViewModel> _data = GetAccruedReportList_Summary(item.BAN_COMPANY, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);
                            if (_data.Any())
                            {
                                DataTable _dt = new DataTable();
                                _dt = ReportService.ToDataTable(_data);
                                _dt.TableName = item.contraction;
                                //ds = new DataSet();
                                ds.Tables.Add(_dt);
                            }
                        }
                    }
                    else
                    {
                        List<AccruedReportViewModel> _data = GetAccruedReportList_Summary(formData.COMPANY_CODE, formData.START_MONTH, formData.START_YEAR, formData.END_MONTH, formData.END_YEAR, formData.CHANNELSValue, Convert.ToInt32(formData.FEE_TYPE), formData.BUSINESS_UNIT);
                        if (_data.Any())
                        {
                            DataTable _dt = new DataTable();
                            _dt = ReportService.ToDataTable(_data);
                            _dt.TableName = companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault() != null ? companyData.Where(m => m.BAN_COMPANY == formData.COMPANY_CODE).FirstOrDefault().contraction : "";
                            //ds = new DataSet();
                            ds.Tables.Add(_dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ds;
        }
        public List<AccruedReportViewModel> GetAllAccruedReportList_Summary(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedReportViewModel> modelList = new List<AccruedReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    #region prepare entity
                    var feeList = (from m in context.PAYMENT_ITEMS
                                   where m.IS_ACTIVE == true
                                   orderby m.GROUP_SEQ_CHANNELS
                                   select m).ToList();

                    var feeItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       orderby m.SEQUENCE
                                       select m).ToList();

                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();
                    var entFeeAccr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                      where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                      orderby m.ACCRUED_MONTH, m.ACCRUED_YEAR
                                      select m).ToList();


                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    var entFeeAccrItem = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB select m).ToList();

                    entFeeInv = entFeeInv.Where(m => m.IS_STATUS != "3").ToList();

                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        feeItemList = (from m in feeItemList where m.COMPANY_CODE == companyCode && feeList.Any(p => m.PAYMENT_ITEMS_ID == p.ID) orderby m.SEQUENCE select m).ToList();
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        entFeeAccr = entFeeAccr.Where(m => m.COMPANY_CODE == companyCode).ToList();

                    }
                    else
                    {
                        if (bu != "ALL")
                        {
                            var companyData = (from com in context.COMPANies where com.IsPaymentFee == true orderby com.Bussiness_Unit select com).ToList();
                            companyData = companyData.Where(m => m.Bussiness_Unit == bu).ToList();

                            feeList = (from m in feeList where companyData.Any(c => m.COMPANY_CODE == c.BAN_COMPANY) orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                            entFeeInv = entFeeInv.Where(m => companyData.Any(c => m.COMPANY_CODE == c.BAN_COMPANY)).ToList();
                        }
                    }




                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        entFeeInv = entFeeInv.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                        entFeeAccr = entFeeAccr.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                    }

                    //check list fee in accrued
                    if (entFeeAccr.Any())
                    {
                        feeList = feeList.Where(m => entFeeAccr.Any(a => a.PAYMENT_ITEMS_CODE == m.PAYMENT_ITEMS_CODE)).ToList();

                        //get Doc Accrued Max Period
                        var maxDocAccrued = entFeeAccr.Max(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH);
                        entFeeAccr = entFeeAccr.Where(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH == maxDocAccrued).ToList();
                    }


                    feeList = feeList.OrderBy(m => m.GROUP_SEQ_CHANNELS).ToList();

                    #endregion

                    var feePriceCatList = feeList.GroupBy(g => new { g.PAYMENT_ITEMS_NAME, g.CHANNELS }).ToList();

                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var get_month = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                    var _diffmonths = get_month + 1;
                    int currentMonth = DateTime.Now.Date.Month;
                    decimal[] arrMonthGrandTrxn = new decimal[_diffmonths];
                    decimal[] arrMonthGrnadTotal = new decimal[get_month + 1];



                    #region Detail
                    foreach (var item in feePriceCatList)
                    {
                        var allpricecat = feeList.Where(m => m.PAYMENT_ITEMS_NAME == item.Key.PAYMENT_ITEMS_NAME).ToList();

                        decimal[] arrMonthTotalTrxn = new decimal[_diffmonths];
                        decimal[] arrMonthTotal = new decimal[_diffmonths];
                        int rowfirst = 0;

                        var feeInvList = entFeeInv.Where(m => allpricecat.Any(p => m.PAYMENT_ITEMS_CODE == p.PAYMENT_ITEMS_CODE)).ToList();

                        var feeAccrList = entFeeAccr.Where(m => allpricecat.Any(p => m.PAYMENT_ITEMS_CODE == p.PAYMENT_ITEMS_CODE)).ToList();

                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)
                                                 orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                 select n).ToList();

                        var get_entFeeAccrItem = (from n in entFeeAccrItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                  select n).ToList();
                        //===group data charge list===
                        var group_charge = (from m in feeItemList where allpricecat.Any(p => m.PAYMENT_ITEMS_ID == p.ID) orderby m.CHARGE_TYPE descending select m).ToList();
                        var group_Trxncharge = group_charge.Where(m => m.CHARGE_TYPE == "TRXN").ToList();


                        #region Trxn + Amt

                        decimal[] arrMonthTrxn = new decimal[_diffmonths];
                        decimal[] arrMonthAMT = new decimal[_diffmonths];



                        var model = new AccruedReportViewModel();
                        rowfirst++;
                        model.CHANNELS = rowfirst == 1 ? item.Key.CHANNELS : "";
                        model.FEE = rowfirst == 1 ? item.Key.PAYMENT_ITEMS_NAME : "";


                        int _mnth = monthS;
                        int _yrr = yearS;
                        int _iLoop = 0;


                        #region set value column month total transaction

                        model.CHARGE = "Trxns Fee";
                        while (_iLoop < _diffmonths)
                        {
                            if (_mnth == 13)
                            {
                                _mnth = 1;
                                _yrr++;
                            }
                            string yStr = _yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;

                            var item_acc_chrge = get_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();

                            var item_inv_chrge = get_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();
                            decimal sumtrxn = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TRANSACTIONS ?? 0) : item_inv_chrge.Sum(m => m.TRANSACTIONS ?? 0);

                            model.GetType().GetProperty((monthIndex)).SetValue(model,
                        Convert.ToString(string.Format("{0:#,##0.####}", sumtrxn)));

                            _iLoop++;
                            _mnth++;
                        }
                        modelList.Add(model);


                        #endregion


                        #region set value column month amount mdr

                        model = new AccruedReportViewModel();

                        model.CHARGE = "Amount MDR";
                        while (_iLoop < _diffmonths)
                        {
                            if (_mnth == 13)
                            {
                                _mnth = 1;
                                _yrr++;
                            }
                            string yStr = _yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;

                            var item_acc_chrge = get_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();

                            var item_inv_chrge = get_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();
                            decimal sumamount = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.ACTUAL_AMOUNT ?? 0) : item_inv_chrge.Sum(m => m.ACTUAL_AMOUNT ?? 0);

                            model.GetType().GetProperty((monthIndex)).SetValue(model,
                        Convert.ToString(string.Format("{0:#,##0.####}", sumamount)));

                            _iLoop++;
                            _mnth++;
                        }
                        modelList.Add(model);


                        #endregion


                        #endregion

                        #region Total
                        model = new AccruedReportViewModel();
                        model.CHARGE = "Total";
                        #region set value column month Total 
                        int mnth = monthS;
                        int yrr = yearS;
                        int iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;

                            var item_acc_chrge = get_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();

                            var item_inv_chrge = get_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();
                            decimal sumamtcharge = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0) : item_inv_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0);



                            model.GetType().GetProperty((monthIndex)).SetValue(model,
                        Convert.ToString(string.Format("{0:#,##0.####}", sumamtcharge)));

                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelList.Add(model);
                        #endregion


                    }//fee channels
                    #endregion

                    #region Summary Grand

                    var sum_entFeeInvItem = (from n in entFeeInvItem
                                             where entFeeInv.Any(f => n.INV_NO == f.INV_NO)
                                             orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                             select n).ToList();

                    var sum_entFeeAccrItem = (from n in entFeeAccrItem
                                              where entFeeAccr.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                              orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                              select n).ToList();
                    var modelGrand = new AccruedReportViewModel();
                    modelGrand.CHARGE = "Total All Trxn.";
                    var modelGrandList = new List<AccruedReportViewModel>();

                    #region set value column month Total Grand Trxn
                    int grandmnth = monthS;
                    int grandyrr = yearS;
                    int grandiLoop = 0;
                    while (grandiLoop < _diffmonths)
                    {
                        if (grandmnth == 13)
                        {
                            grandmnth = 1;
                            grandyrr++;
                        }
                        string yStr = grandyrr.ToString().Substring(2, 2);
                        string monthIndex = dateTimeInfo.AbbreviatedMonthNames[grandmnth - 1] + yStr;

                        var item_acc_chrge = sum_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (grandyrr * 12) + grandmnth).ToList();

                        var item_inv_chrge = sum_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (grandyrr * 12) + grandmnth).ToList();
                        decimal grandtrxn = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TRANSACTIONS ?? 0) : item_inv_chrge.Sum(m => m.TRANSACTIONS ?? 0);



                        modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                    Convert.ToString(string.Format("{0:#,##0.####}", grandtrxn)));

                        grandiLoop++;
                        grandmnth++;
                    }

                    #endregion


                    modelGrandList.Add(modelGrand);

                    modelList.AddRange(modelGrandList);

                    modelGrand = new AccruedReportViewModel();
                    modelGrand.CHARGE = "Grand Total";
                    modelGrandList = new List<AccruedReportViewModel>();


                    #region set value column month Total Grand 
                    grandmnth = monthS;
                    grandyrr = yearS;
                    grandiLoop = 0;
                    while (grandiLoop < _diffmonths)
                    {
                        if (grandmnth == 13)
                        {
                            grandmnth = 1;
                            grandyrr++;
                        }
                        string yStr = grandyrr.ToString().Substring(2, 2);
                        string monthIndex = dateTimeInfo.AbbreviatedMonthNames[grandmnth - 1] + yStr;
                        var item_acc_chrge = sum_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (grandyrr * 12) + grandmnth).ToList();

                        var item_inv_chrge = sum_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (grandyrr * 12) + grandmnth).ToList();
                        decimal grandamount = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0) : item_inv_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0);

                        modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                        Convert.ToString(string.Format("{0:#,##0.####}", grandamount)));
                        grandiLoop++;
                        grandmnth++;
                    }

                    #endregion

                    modelGrandList.Add(modelGrand);

                    modelList.AddRange(modelGrandList);

                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }

        public List<AccruedReportViewModel> GetAccruedReportList_Summary(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedReportViewModel> modelList = new List<AccruedReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    #region prepare entity
                    var feeList = (from m in context.PAYMENT_ITEMS
                                   where m.IS_ACTIVE == true
                                   orderby m.GROUP_SEQ_CHANNELS
                                   select m).ToList();

                    var feeItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       orderby m.SEQUENCE
                                       select m).ToList();

                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();
                    var entFeeAccr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                      where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                      orderby m.ACCRUED_MONTH, m.ACCRUED_YEAR
                                      select m).ToList();


                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    var entFeeAccrItem = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB select m).ToList();

                    entFeeInv = entFeeInv.Where(m => m.IS_STATUS != "3").ToList();

                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        feeItemList = (from m in feeItemList where m.COMPANY_CODE == companyCode && feeList.Any(p => m.PAYMENT_ITEMS_ID == p.ID) orderby m.SEQUENCE select m).ToList();
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        entFeeAccr = entFeeAccr.Where(m => m.COMPANY_CODE == companyCode).ToList();

                    }




                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        entFeeInv = entFeeInv.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                        entFeeAccr = entFeeAccr.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                    }

                    //check list fee in accrued
                    if (entFeeAccr.Any())
                    {
                        feeList = feeList.Where(m => entFeeAccr.Any(a => a.PAYMENT_ITEMS_CODE == m.PAYMENT_ITEMS_CODE)).ToList();

                        //get Doc Accrued Max Period
                        var maxDocAccrued = entFeeAccr.Max(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH);
                        entFeeAccr = entFeeAccr.Where(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH == maxDocAccrued).ToList();
                    }


                    feeList = feeList.OrderBy(m => m.GROUP_SEQ_CHANNELS).ToList();

                    #endregion

                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var get_month = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                    var _diffmonths = get_month + 1;
                    int currentMonth = DateTime.Now.Date.Month;
                    decimal[] arrMonthGrandTrxn = new decimal[_diffmonths];
                    decimal[] arrMonthGrnadTotal = new decimal[get_month + 1];

                    #region Detail
                    foreach (var item in feeList)
                    {

                        decimal[] arrMonthTotalTrxn = new decimal[_diffmonths];
                        decimal[] arrMonthTotal = new decimal[_diffmonths];
                        int rowfirst = 0;

                        bool chkTotalTrx = false;
                        var feeInvList = entFeeInv.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var feeAccrList = entFeeAccr.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)
                                                 orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                 select n).ToList();

                        var get_entFeeAccrItem = (from n in entFeeAccrItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                  select n).ToList();
                        //===group data charge list===
                        var group_charge = (from m in feeItemList where m.PAYMENT_ITEMS_ID == item.ID orderby m.CHARGE_TYPE descending select m).ToList();
                        var group_Trxncharge = group_charge.Where(m => m.CHARGE_TYPE == "TRXN").ToList();


                        #region Trxn + Amt

                        foreach (var data_charge in group_charge)
                        {
                            decimal[] arrMonthTrxn = new decimal[_diffmonths];
                            decimal[] arrMonthAMT = new decimal[_diffmonths];

                            //==accrued sub list==
                            var data_acc = get_entFeeAccrItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();
                            //==invoice sub list==
                            var data_inv = get_entFeeInvItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();

                            var model = new AccruedReportViewModel();
                            rowfirst++;
                            model.CHANNELS = rowfirst == 1 ? item.CHANNELS : "";
                            model.FEE = rowfirst == 1 ? item.PAYMENT_ITEMS_NAME : "";
                            model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME;


                            #region value before calculate avg  Transaction and Amount
                            //===charge ===
                            int mS = monthS;
                            int mE = monthE;
                            int yS = yearS;
                            int yE = yearE;
                            int mth = mS;
                            int yr = yS;
                            for (int n = 1; n <= _diffmonths; n++)
                            {
                                //mth = n;
                                if (mth == 13)
                                {
                                    mth = 1;
                                    yr = yr + 1;
                                }
                                var item_chrge = data_acc.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                if (item_chrge != null)
                                {
                                    if (data_charge.CHARGE_TYPE == "TRXN")
                                    {
                                        arrMonthTrxn[n - 1] = (item_chrge.TRANSACTIONS ?? 0);
                                        arrMonthTotalTrxn[n - 1] += (item_chrge.TRANSACTIONS ?? 0);
                                        arrMonthGrandTrxn[n - 1] += (item_chrge.TRANSACTIONS ?? 0);
                                    }
                                    if (data_charge.CHARGE_TYPE == "MDR")
                                    {
                                        chkTotalTrx = false;
                                        arrMonthAMT[n - 1] = (item_chrge.ACTUAL_AMOUNT ?? 0);
                                    }
                                }
                                mth++;
                            }
                            #endregion

                            #region Trxn
                            int iTrxn = 0;
                            int _mnth = monthS;
                            int _yrr = yearS;
                            int _iLoop = 0;
                            foreach (var trx in arrMonthTrxn.ToArray())
                            {
                                //if (arrMonthTrxn.ToList().All(m => m == 0))
                                //{ break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }
                                if (trx != 0)
                                {
                                    ////Accrued
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];
                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", trx)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (_yrr * 12) + _mnth).FirstOrDefault();

                                    arrMonthTrxn[iTrxn] = (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);

                                    arrMonthTotalTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);
                                    arrMonthGrandTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);
                                }
                                //}
                                _mnth++;
                                iTrxn++;
                            }


                            #region set value column month transaction
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            while (_iLoop < _diffmonths)
                            {
                                //if (arrMonthTrxn.ToList().All(m => m == 0))
                                //{ break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }
                                string yStr = _yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;


                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                               Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTrxn[_iLoop])));

                                _iLoop++;
                                _mnth++;
                            }

                            #endregion
                            #endregion


                            #region amt
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            iTrxn = 0;
                            foreach (var amt in arrMonthAMT.ToArray())
                            {
                                //if (arrMonthAMT.ToList().All(m => m == 0))
                                //{ break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }

                                if (amt != 0)
                                {
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", amt)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (_yrr * 12) + _mnth).FirstOrDefault();

                                    arrMonthAMT[iTrxn] = (get_data_inv != null ? (get_data_inv.ACTUAL_AMOUNT ?? 0) : 0);


                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.ACTUAL_AMOUNT ?? 0 : 0))));



                                }
                                //}
                                _mnth++;
                                iTrxn++;

                            }
                            #region set value column month amount
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            while (_iLoop < _diffmonths)
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }
                                string yStr = _yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;


                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                                Convert.ToString(string.Format("{0:#,##0.####}", arrMonthAMT[_iLoop])));

                                _iLoop++;
                                _mnth++;
                            }

                            #endregion


                            #endregion

                            modelList.Add(model);
                            #region Total Trxn
                            if (rowfirst == group_Trxncharge.Count())
                            {
                                chkTotalTrx = true;
                            }

                            #region set value column month total transaction
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            if (chkTotalTrx)
                            {
                                chkTotalTrx = false;
                                var inclmodelList = new List<AccruedReportViewModel>();
                                model = new AccruedReportViewModel();
                                model.CHARGE = "Total Trxn";

                                while (_iLoop < _diffmonths)
                                {
                                    if (_mnth == 13)
                                    {
                                        _mnth = 1;
                                        _yrr++;
                                    }
                                    string yStr = _yrr.ToString().Substring(2, 2);
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;

                                    model.GetType().GetProperty((monthIndex)).SetValue(model,
                                Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotalTrxn[_iLoop])));

                                    _iLoop++;
                                    _mnth++;
                                }
                                inclmodelList.Add(model);
                                modelList.AddRange(inclmodelList);
                            }

                            #endregion

                            #endregion



                        }//fee
                        #endregion


                        //if (feeAccrList.Any())
                        //{
                        #region Charge Trxn + Amt
                        foreach (var data_charge in group_charge)
                        {
                            decimal[] arrMonthCharge = new decimal[_diffmonths];
                            var data = get_entFeeAccrItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();


                            var data_inv = get_entFeeInvItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();

                            var model = new AccruedReportViewModel();
                            rowfirst++;
                            model.CHANNELS = "";
                            model.FEE = "";
                            if (data.Where(m => (m.TRANSACTIONS ?? 0) != 0 && (m.RATE_TRANS ?? 0) != 0).FirstOrDefault() != null)
                            {
                                model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME + " Rate Charge (" + Convert.ToString(string.Format("{0:#,##0.####}", data.Where(m => (m.TRANSACTIONS ?? 0) != 0).FirstOrDefault().RATE_TRANS)) + ")";

                            }
                            else
                            {
                                model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME + ((data.Where(m => (m.RATE_AMT ?? 0) != 0).FirstOrDefault() == null) ? ""
                                    : " Rate Charge (" + Convert.ToString(string.Format("{0:#,##0.####}", data.Where(m => (m.RATE_AMT ?? 0) != 0).FirstOrDefault().RATE_AMT)) + "% )");
                            }


                            int mS = monthS;
                            int mE = monthE;
                            int yS = yearS;
                            int yE = yearE;
                            int mth = mS;
                            int yr = yS;
                            for (int n = 1; n <= _diffmonths; n++)
                            {
                                //mth = n;
                                if (mth == 13)
                                {
                                    mth = 1;
                                    yr = yr + 1;
                                }
                                var item_chrge = data.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                if (item_chrge != null)
                                {
                                    //Chrage
                                    arrMonthCharge[n - 1] = (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                    //Total
                                    arrMonthTotal[n - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                    //Grnad Total
                                    arrMonthGrnadTotal[n - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);
                                }
                                mth++;
                            }

                            //foreach (var item_chrge in data)
                            //{

                            //    //Chrage
                            //    arrMonthCharge[(item_chrge.INV_MONTH ?? 0) - 1] = (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                            //    //Total
                            //    arrMonthTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                            //    //Grnad Total
                            //    arrMonthGrnadTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                            //}//charge




                            #region charge

                            mth = mS;
                            yr = yS;
                            int iCharge = 0;
                            foreach (var chrge in arrMonthCharge.ToArray())
                            {
                                //if (arrMonthCharge.ToList().All(m => m == 0))
                                //{ break; }
                                if (mth == 13)
                                {
                                    mth = 1;
                                    yr++;
                                }
                                if (chrge != 0)
                                {
                                    ////Accrued
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", chrge)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                    arrMonthCharge[iCharge] = (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                    arrMonthTotal[iCharge] += (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                    arrMonthGrnadTotal[iCharge] += (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0))));

                                }
                                //}
                                mth++;
                                iCharge++;

                            }
                            #region set value column month charge amount
                            int chgmnth = monthS;
                            int chgyrr = yearS;
                            int chgiLoop = 0;
                            while (chgiLoop < _diffmonths)
                            {
                                if (arrMonthCharge.ToList().All(m => m == 0))
                                { break; }
                                if (chgmnth == 13)
                                {
                                    chgmnth = 1;
                                    chgyrr++;
                                }
                                string yStr = chgyrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[chgmnth - 1] + yStr;

                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                            Convert.ToString(string.Format("{0:#,##0.####}", arrMonthCharge[chgiLoop])));

                                chgiLoop++;
                                chgmnth++;
                            }

                            #endregion


                            #endregion
                            modelList.Add(model);
                        }//fee
                        #endregion

                        #region Total
                        var modelTotal = new AccruedReportViewModel();
                        modelTotal.CHARGE = "Total";
                        var modelTotalList = new List<AccruedReportViewModel>();
                        //foreach (var total in arrMonthTotal.ToArray())
                        //{
                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[indexTotal];

                        //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal,
                        //    Convert.ToString(string.Format("{0:#,##0.####}", total)));

                        //    indexTotal++;

                        //}
                        #region set value column month Total 
                        int mnth = monthS;
                        int yrr = yearS;
                        int iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            var getfeeInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();

                            modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal,
                        Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotal[iLoop])));

                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelTotalList.Add(modelTotal);
                        modelList.AddRange(modelTotalList);
                        #endregion

                        #region PO
                        modelTotal = new AccruedReportViewModel();
                        modelTotal.CHARGE = "PO No.";
                        modelTotalList = new List<AccruedReportViewModel>();



                        #region set value column month PO 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {

                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            var chkAccrList = feeAccrList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                            string getPO = string.Empty;
                            if (chkAccrList != null)
                            {
                                getPO = chkAccrList.PRO_NO;
                            }
                            else
                            {
                                var chkInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                getPO = chkInvList == null ? "" : chkInvList.PRO_NO;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal, getPO);
                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelTotalList.Add(modelTotal);

                        modelList.AddRange(modelTotalList);
                        #endregion

                        #region INV
                        modelTotal = new AccruedReportViewModel();
                        modelTotal.CHARGE = "Inv No.";
                        modelTotalList = new List<AccruedReportViewModel>();

                        #region set value column month INV No. 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {

                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            var chkAccrList = feeAccrList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                            string getINVNo = string.Empty;
                            if (chkAccrList != null)
                            {
                                getINVNo = chkAccrList.INV_NO;
                            }
                            else
                            {
                                var chkInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                getINVNo = chkInvList == null ? "" : chkInvList.INV_NO;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal, getINVNo);
                            iLoop++;
                            mnth++;
                        }

                        #endregion

                        modelTotalList.Add(modelTotal);

                        modelList.AddRange(modelTotalList);
                        #endregion
                        //}//end  if (feeAccrList.Any())

                    }//fee channels
                    #endregion

                    #region Summary Grand
                    if (arrMonthGrandTrxn.ToList().All(m => m == 0))
                    { }
                    else
                    {

                        var modelGrand = new AccruedReportViewModel();
                        modelGrand.CHARGE = "Total All Trxn.";
                        var modelGrandList = new List<AccruedReportViewModel>();
                        int iGrand = 0;
                        //foreach (var grandTrxn in arrMonthGrandTrxn.ToArray())
                        //{
                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iGrand];

                        //    modelGrand.GetType().GetProperty(monthIndex).SetValue(modelGrand,
                        //    Convert.ToString(string.Format("{0:#,##0.####}", grandTrxn)));


                        //    iGrand++;

                        //}

                        //----get value invoice accrued <> 3

                        var feeInvList = entFeeInv.ToList();
                        #region set value column month Total Grand Trxn
                        int mnth = monthS;
                        int yrr = yearS;
                        int iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;


                            modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                        Convert.ToString(string.Format("{0:#,##0.####}", arrMonthGrandTrxn[iLoop])));

                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelGrandList.Add(modelGrand);

                        modelList.AddRange(modelGrandList);

                        modelGrand = new AccruedReportViewModel();
                        modelGrand.CHARGE = "Grand Total";
                        modelGrandList = new List<AccruedReportViewModel>();
                        iGrand = 0;
                        //foreach (var grandTotal in arrMonthGrnadTotal.ToArray())
                        //{
                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iGrand];

                        //    modelGrand.GetType().GetProperty(monthIndex).SetValue(modelGrand,
                        //    Convert.ToString(string.Format("{0:#,##0.####}", grandTotal)));

                        //    iGrand++;

                        //}
                        #region set value column month Total Grand 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                            Convert.ToString(string.Format("{0:#,##0.####}", arrMonthGrnadTotal[iLoop])));
                            iLoop++;
                            mnth++;
                        }

                        #endregion

                        modelGrandList.Add(modelGrand);

                        modelList.AddRange(modelGrandList);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }

        public List<AccruedReportViewModel> GetAllAccruedandActualReportList(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedReportViewModel> modelList = new List<AccruedReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    #region prepare entity
                    var feeList = (from m in context.PAYMENT_ITEMS
                                   where m.IS_ACTIVE == true
                                   orderby m.GROUP_SEQ_CHANNELS
                                   select m).ToList();

                    var feeItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       orderby m.SEQUENCE
                                       select m).ToList();

                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();
                    var entFeeAccr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                      where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                      orderby m.ACCRUED_MONTH, m.ACCRUED_YEAR
                                      select m).ToList();


                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    var entFeeAccrItem = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB select m).ToList();


                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        feeItemList = (from m in feeItemList where m.COMPANY_CODE == companyCode && feeList.Any(p => m.PAYMENT_ITEMS_ID == p.ID) orderby m.SEQUENCE select m).ToList();
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        entFeeAccr = entFeeAccr.Where(m => m.COMPANY_CODE == companyCode).ToList();

                    }
                    else
                    {
                        if (bu != "ALL")
                        {
                            var companyData = (from com in context.COMPANies where com.IsPaymentFee == true orderby com.Bussiness_Unit select com).ToList();
                            companyData = companyData.Where(m => m.Bussiness_Unit == bu).ToList();

                            feeList = (from m in feeList where companyData.Any(c => m.COMPANY_CODE == c.BAN_COMPANY) orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                            entFeeInv = entFeeInv.Where(m => companyData.Any(c => m.COMPANY_CODE == c.BAN_COMPANY)).ToList();
                        }
                    }

                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        entFeeInv = entFeeInv.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                        entFeeAccr = entFeeAccr.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                    }

                    //check list fee in accrued
                    if (entFeeAccr.Any())
                    {
                        feeList = feeList.Where(m => entFeeAccr.Any(a => a.PAYMENT_ITEMS_CODE == m.PAYMENT_ITEMS_CODE)).ToList();

                        //get Doc Accrued Max Period
                        var maxDocAccrued = entFeeAccr.Max(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH);
                        entFeeAccr = entFeeAccr.Where(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH == maxDocAccrued).ToList();
                    }


                    feeList = feeList.OrderBy(m => m.GROUP_SEQ_CHANNELS).ToList();


                    var feePriceCatList = feeList.GroupBy(g => new { g.PAYMENT_ITEMS_NAME, g.CHANNELS }).ToList();


                    #endregion

                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var get_month = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                    var _diffmonths = get_month + 1;
                    int currentMonth = DateTime.Now.Date.Month;
                    decimal[] arrMonthGrandTrxn = new decimal[_diffmonths];
                    decimal[] arrMonthGrnadTotal = new decimal[get_month + 1];

                    #region Detail
                    foreach (var item in feePriceCatList)
                    {

                        var allpricecat = feeList.Where(m => m.PAYMENT_ITEMS_NAME == item.Key.PAYMENT_ITEMS_NAME).ToList();

                        decimal[] arrMonthTotalTrxn = new decimal[_diffmonths];
                        decimal[] arrMonthTotal = new decimal[_diffmonths];
                        int rowfirst = 0;
                        
                        var feeInvList = entFeeInv.Where(m => allpricecat.Any(p => m.PAYMENT_ITEMS_CODE == p.PAYMENT_ITEMS_CODE)).ToList();

                        var feeAccrList = entFeeAccr.Where(m => allpricecat.Any(p => m.PAYMENT_ITEMS_CODE == p.PAYMENT_ITEMS_CODE)).ToList();

                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)
                                                 orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                 select n).ToList();

                        var get_entFeeAccrItem = (from n in entFeeAccrItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                  select n).ToList();
                        //===group data charge list===
                        var group_charge = (from m in feeItemList where allpricecat.Any(p => m.PAYMENT_ITEMS_ID == p.ID) orderby m.CHARGE_TYPE descending select m).ToList();
                        var group_Trxncharge = group_charge.Where(m => m.CHARGE_TYPE == "TRXN").ToList();


                        #region Trxn + Amt



                        var model = new AccruedReportViewModel();
                        rowfirst++;
                        model.CHANNELS = rowfirst == 1 ? item.Key.CHANNELS : "";
                        model.FEE = rowfirst == 1 ? item.Key.PAYMENT_ITEMS_NAME : "";

                        #region value before calculate avg  Transaction and Amount
                        //===charge ===
                        int mS = monthS;
                        int mE = monthE;
                        int yS = yearS;
                        int yE = yearE;
                        int mth = mS;
                        int yr = yS;

                        #endregion



                        #region set value column month total transaction
                        int mnth = monthS;
                        int yrr = yearS;
                        int iLoop = 0;
                        model.CHARGE = "Trxn Fee";

                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;

                            var item_acc_chrge = get_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();

                            var item_inv_chrge = get_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();
                            decimal sumtrxn = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TRANSACTIONS ?? 0) : item_inv_chrge.Sum(m => m.TRANSACTIONS ?? 0);

                            model.GetType().GetProperty((monthIndex)).SetValue(model,
                            Convert.ToString(string.Format("{0:#,##0.####}", sumtrxn)));

                            iLoop++;
                            mnth++;
                        }
                        modelList.Add(model);


                        #endregion

                        #region set value column month amount mdr
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                       
                            model = new AccruedReportViewModel();
                            model.CHARGE = "Amount MDR";

                            while (iLoop < _diffmonths)
                            {
                                if (mnth == 13)
                                {
                                    mnth = 1;
                                    yrr++;
                                }
                                string yStr = yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;

                                var item_acc_chrge = get_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();

                                var item_inv_chrge = get_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();
                                decimal sumamount = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.ACTUAL_AMOUNT ?? 0) : item_inv_chrge.Sum(m => m.ACTUAL_AMOUNT ?? 0);

                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                                Convert.ToString(string.Format("{0:#,##0.####}", sumamount)));

                                iLoop++;
                                mnth++;
                            }
                            modelList.Add(model);
                        

                        #endregion



                        #endregion


                        #region Total

                        model = new AccruedReportViewModel();
                        model.CHARGE = "Total";

                        #region set value column month Total 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;

                            var item_acc_chrge = get_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();

                            var item_inv_chrge = get_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yrr * 12) + mnth).ToList();
                            decimal sumamtcharge = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0) : item_inv_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0);

                            model.GetType().GetProperty((monthIndex)).SetValue(model,
                        Convert.ToString(string.Format("{0:#,##0.####}", sumamtcharge)));

                            iLoop++;
                            mnth++;
                        }

                        #endregion
                        modelList.Add(model);
                        #endregion



                    }//fee channels
                    #endregion

                    #region Summary Grand

                    var sum_entFeeInvItem = (from n in entFeeInvItem
                                             where entFeeInv.Any(f => n.INV_NO == f.INV_NO)
                                             orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                             select n).ToList();

                    var sum_entFeeAccrItem = (from n in entFeeAccrItem
                                              where entFeeAccr.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                              orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                              select n).ToList();

                    var modelGrand = new AccruedReportViewModel();
                    modelGrand.CHARGE = "Total All Trxn.";
                    var modelGrandList = new List<AccruedReportViewModel>();



                    #region set value column month Total Grand Trxn
                    int _mnth = monthS;
                    int _yrr = yearS;
                    int _iLoop = 0;
                    while (_iLoop < _diffmonths)
                    {
                        if (_mnth == 13)
                        {
                            _mnth = 1;
                            _yrr++;
                        }
                        string yStr = _yrr.ToString().Substring(2, 2);
                        string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;
                        var item_acc_chrge = sum_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();

                        var item_inv_chrge = sum_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();
                        decimal grandtran = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TRANSACTIONS ?? 0) : item_inv_chrge.Sum(m => m.TRANSACTIONS ?? 0);



                        modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                    Convert.ToString(string.Format("{0:#,##0.####}", grandtran)));

                        _iLoop++;
                        _mnth++;
                    }

                    #endregion


                    modelGrandList.Add(modelGrand);

                    modelList.AddRange(modelGrandList);

                    modelGrand = new AccruedReportViewModel();
                    modelGrand.CHARGE = "Grand Total";
                    modelGrandList = new List<AccruedReportViewModel>();

                    #region set value column month Total Grand 
                    _mnth = monthS;
                    _yrr = yearS;
                    _iLoop = 0;
                    while (_iLoop < _diffmonths)
                    {
                        if (_mnth == 13)
                        {
                            _mnth = 1;
                            _yrr++;
                        }
                        string yStr = _yrr.ToString().Substring(2, 2);
                        string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;

                        var item_acc_chrge = sum_entFeeAccrItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();

                        var item_inv_chrge = sum_entFeeInvItem.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (_yrr * 12) + _mnth).ToList();
                        decimal grandamount = item_acc_chrge.Any() ? item_acc_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0) : item_inv_chrge.Sum(m => m.TOTAL_CHARGE_AMOUNT ?? 0);

                        modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                        Convert.ToString(string.Format("{0:#,##0.####}", grandamount)));
                        _iLoop++;
                        _mnth++;
                    }

                    #endregion

                    modelGrandList.Add(modelGrand);

                    modelList.AddRange(modelGrandList);

                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }


        public List<AccruedReportViewModel> GetAccruedandActualReportList(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedReportViewModel> modelList = new List<AccruedReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    #region prepare entity
                    var feeList = (from m in context.PAYMENT_ITEMS
                                   where m.IS_ACTIVE == true
                                   orderby m.GROUP_SEQ_CHANNELS
                                   select m).ToList();

                    var feeItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       orderby m.SEQUENCE
                                       select m).ToList();

                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();
                    var entFeeAccr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                      where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                      orderby m.ACCRUED_MONTH, m.ACCRUED_YEAR
                                      select m).ToList();


                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    var entFeeAccrItem = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB select m).ToList();


                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        feeItemList = (from m in feeItemList where m.COMPANY_CODE == companyCode && feeList.Any(p => m.PAYMENT_ITEMS_ID == p.ID) orderby m.SEQUENCE select m).ToList();
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        entFeeAccr = entFeeAccr.Where(m => m.COMPANY_CODE == companyCode).ToList();

                    }




                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        entFeeInv = entFeeInv.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                        entFeeAccr = entFeeAccr.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                    }

                    //check list fee in accrued
                    if (entFeeAccr.Any())
                    {
                        feeList = feeList.Where(m => entFeeAccr.Any(a => a.PAYMENT_ITEMS_CODE == m.PAYMENT_ITEMS_CODE)).ToList();

                        //get Doc Accrued Max Period
                        var maxDocAccrued = entFeeAccr.Max(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH);
                        entFeeAccr = entFeeAccr.Where(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH == maxDocAccrued).ToList();
                    }


                    feeList = feeList.OrderBy(m => m.GROUP_SEQ_CHANNELS).ToList();

                    #endregion

                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var get_month = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                    var _diffmonths = get_month + 1;
                    int currentMonth = DateTime.Now.Date.Month;
                    decimal[] arrMonthGrandTrxn = new decimal[_diffmonths];
                    decimal[] arrMonthGrnadTotal = new decimal[get_month + 1];

                    #region Detail
                    foreach (var item in feeList)
                    {

                        decimal[] arrMonthTotalTrxn = new decimal[_diffmonths];
                        decimal[] arrMonthTotal = new decimal[_diffmonths];
                        int rowfirst = 0;

                        bool chkTotalTrx = false;
                        var feeInvList = entFeeInv.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var feeAccrList = entFeeAccr.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)
                                                 orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                 select n).ToList();

                        var get_entFeeAccrItem = (from n in entFeeAccrItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                  select n).ToList();
                        //===group data charge list===
                        var group_charge = (from m in feeItemList where m.PAYMENT_ITEMS_ID == item.ID orderby m.CHARGE_TYPE descending select m).ToList();
                        var group_Trxncharge = group_charge.Where(m => m.CHARGE_TYPE == "TRXN").ToList();


                        #region Trxn + Amt

                        foreach (var data_charge in group_charge)
                        {
                            decimal[] arrMonthTrxn = new decimal[_diffmonths];
                            decimal[] arrMonthAMT = new decimal[_diffmonths];

                            //==accrued sub list==
                            var data_acc = get_entFeeAccrItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();
                            //==invoice sub list==
                            var data_inv = get_entFeeInvItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();

                            var model = new AccruedReportViewModel();
                            rowfirst++;
                            model.CHANNELS = rowfirst == 1 ? item.CHANNELS : "";
                            model.FEE = rowfirst == 1 ? item.PAYMENT_ITEMS_NAME : "";
                            model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME;


                            #region value before calculate avg  Transaction and Amount
                            //===charge ===
                            int mS = monthS;
                            int mE = monthE;
                            int yS = yearS;
                            int yE = yearE;
                            int mth = mS;
                            int yr = yS;
                            for (int n = 1; n <= _diffmonths; n++)
                            {
                                //mth = n;
                                if (mth == 13)
                                {
                                    mth = 1;
                                    yr = yr + 1;
                                }
                                var item_chrge = data_acc.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                if (item_chrge != null)
                                {
                                    if (data_charge.CHARGE_TYPE == "TRXN")
                                    {
                                        arrMonthTrxn[n - 1] = (item_chrge.TRANSACTIONS ?? 0);
                                        arrMonthTotalTrxn[n - 1] += (item_chrge.TRANSACTIONS ?? 0);
                                        arrMonthGrandTrxn[n - 1] += (item_chrge.TRANSACTIONS ?? 0);
                                    }
                                    if (data_charge.CHARGE_TYPE == "MDR")
                                    {
                                        chkTotalTrx = false;
                                        arrMonthAMT[n - 1] = (item_chrge.ACTUAL_AMOUNT ?? 0);
                                    }
                                }
                                mth++;
                            }
                            #endregion

                            #region Trxn
                            int iTrxn = 0;
                            int _mnth = monthS;
                            int _yrr = yearS;
                            int _iLoop = 0;
                            foreach (var trx in arrMonthTrxn.ToArray())
                            {
                                //if (arrMonthTrxn.ToList().All(m => m == 0))
                                //{ break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }
                                if (trx != 0)
                                {
                                    ////Accrued
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];
                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", trx)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (_yrr * 12) + _mnth).FirstOrDefault();

                                    arrMonthTrxn[iTrxn] = (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);

                                    arrMonthTotalTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);
                                    arrMonthGrandTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);
                                }
                                //}
                                _mnth++;
                                iTrxn++;
                            }


                            #region set value column month transaction
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            while (_iLoop < _diffmonths)
                            {
                                //if (arrMonthTrxn.ToList().All(m => m == 0))
                                //{ break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }
                                string yStr = _yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;


                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                               Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTrxn[_iLoop])));

                                _iLoop++;
                                _mnth++;
                            }

                            #endregion
                            #endregion


                            #region amt
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            iTrxn = 0;
                            foreach (var amt in arrMonthAMT.ToArray())
                            {
                                //if (arrMonthAMT.ToList().All(m => m == 0))
                                //{ break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }

                                if (amt != 0)
                                {
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", amt)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (_yrr * 12) + _mnth).FirstOrDefault();

                                    arrMonthAMT[iTrxn] = (get_data_inv != null ? (get_data_inv.ACTUAL_AMOUNT ?? 0) : 0);


                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.ACTUAL_AMOUNT ?? 0 : 0))));



                                }
                                //}
                                _mnth++;
                                iTrxn++;

                            }
                            #region set value column month amount
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            while (_iLoop < _diffmonths)
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }
                                if (_mnth == 13)
                                {
                                    _mnth = 1;
                                    _yrr++;
                                }
                                string yStr = _yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;


                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                                Convert.ToString(string.Format("{0:#,##0.####}", arrMonthAMT[_iLoop])));

                                _iLoop++;
                                _mnth++;
                            }

                            #endregion


                            #endregion

                            modelList.Add(model);
                            #region Total Trxn
                            if (rowfirst == group_Trxncharge.Count())
                            {
                                chkTotalTrx = true;
                            }

                            #region set value column month total transaction
                            _mnth = monthS;
                            _yrr = yearS;
                            _iLoop = 0;
                            if (chkTotalTrx)
                            {
                                chkTotalTrx = false;
                                var inclmodelList = new List<AccruedReportViewModel>();
                                model = new AccruedReportViewModel();
                                model.CHARGE = "Total Trxn";

                                while (_iLoop < _diffmonths)
                                {
                                    if (_mnth == 13)
                                    {
                                        _mnth = 1;
                                        _yrr++;
                                    }
                                    string yStr = _yrr.ToString().Substring(2, 2);
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_mnth - 1] + yStr;

                                    model.GetType().GetProperty((monthIndex)).SetValue(model,
                                Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotalTrxn[_iLoop])));

                                    _iLoop++;
                                    _mnth++;
                                }
                                inclmodelList.Add(model);
                                modelList.AddRange(inclmodelList);
                            }

                            #endregion

                            #endregion



                        }//fee
                        #endregion


                        //if (feeAccrList.Any())
                        //{
                        #region Charge Trxn + Amt
                        foreach (var data_charge in group_charge)
                        {
                            decimal[] arrMonthCharge = new decimal[_diffmonths];
                            var data = get_entFeeAccrItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();


                            var data_inv = get_entFeeInvItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();

                            var model = new AccruedReportViewModel();
                            rowfirst++;
                            model.CHANNELS = "";
                            model.FEE = "";
                            if (data.Where(m => (m.TRANSACTIONS ?? 0) != 0 && (m.RATE_TRANS ?? 0) != 0).FirstOrDefault() != null)
                            {
                                model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME + " Rate Charge (" + Convert.ToString(string.Format("{0:#,##0.####}", data.Where(m => (m.TRANSACTIONS ?? 0) != 0).FirstOrDefault().RATE_TRANS)) + ")";

                            }
                            else
                            {
                                model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME + ((data.Where(m => (m.RATE_AMT ?? 0) != 0).FirstOrDefault() == null) ? ""
                                    : " Rate Charge (" + Convert.ToString(string.Format("{0:#,##0.####}", data.Where(m => (m.RATE_AMT ?? 0) != 0).FirstOrDefault().RATE_AMT)) + "% )");
                            }


                            int mS = monthS;
                            int mE = monthE;
                            int yS = yearS;
                            int yE = yearE;
                            int mth = mS;
                            int yr = yS;
                            for (int n = 1; n <= _diffmonths; n++)
                            {
                                //mth = n;
                                if (mth == 13)
                                {
                                    mth = 1;
                                    yr = yr + 1;
                                }
                                var item_chrge = data.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                if (item_chrge != null)
                                {
                                    //Chrage
                                    arrMonthCharge[n - 1] = (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                    //Total
                                    arrMonthTotal[n - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                    //Grnad Total
                                    arrMonthGrnadTotal[n - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);
                                }
                                mth++;
                            }

                            //foreach (var item_chrge in data)
                            //{

                            //    //Chrage
                            //    arrMonthCharge[(item_chrge.INV_MONTH ?? 0) - 1] = (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                            //    //Total
                            //    arrMonthTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                            //    //Grnad Total
                            //    arrMonthGrnadTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                            //}//charge




                            #region charge

                            mth = mS;
                            yr = yS;
                            int iCharge = 0;
                            foreach (var chrge in arrMonthCharge.ToArray())
                            {
                                //if (arrMonthCharge.ToList().All(m => m == 0))
                                //{ break; }
                                if (mth == 13)
                                {
                                    mth = 1;
                                    yr++;
                                }
                                if (chrge != 0)
                                {
                                    ////Accrued
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", chrge)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                    arrMonthCharge[iCharge] = (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                    arrMonthTotal[iCharge] += (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                    arrMonthGrnadTotal[iCharge] += (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0))));

                                }
                                //}
                                mth++;
                                iCharge++;

                            }
                            #region set value column month charge amount
                            int chgmnth = monthS;
                            int chgyrr = yearS;
                            int chgiLoop = 0;
                            while (chgiLoop < _diffmonths)
                            {
                                if (arrMonthCharge.ToList().All(m => m == 0))
                                { break; }
                                if (chgmnth == 13)
                                {
                                    chgmnth = 1;
                                    chgyrr++;
                                }
                                string yStr = chgyrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[chgmnth - 1] + yStr;

                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                            Convert.ToString(string.Format("{0:#,##0.####}", arrMonthCharge[chgiLoop])));

                                chgiLoop++;
                                chgmnth++;
                            }

                            #endregion


                            #endregion
                            modelList.Add(model);
                        }//fee
                        #endregion

                        #region Total
                        var modelTotal = new AccruedReportViewModel();
                        modelTotal.CHARGE = "Total";
                        var modelTotalList = new List<AccruedReportViewModel>();
                        //foreach (var total in arrMonthTotal.ToArray())
                        //{
                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[indexTotal];

                        //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal,
                        //    Convert.ToString(string.Format("{0:#,##0.####}", total)));

                        //    indexTotal++;

                        //}
                        #region set value column month Total 
                        int mnth = monthS;
                        int yrr = yearS;
                        int iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            var getfeeInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();

                            modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal,
                        Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotal[iLoop])));

                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelTotalList.Add(modelTotal);
                        modelList.AddRange(modelTotalList);
                        #endregion

                        #region PO
                        modelTotal = new AccruedReportViewModel();
                        modelTotal.CHARGE = "PO No.";
                        modelTotalList = new List<AccruedReportViewModel>();



                        #region set value column month PO 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {

                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            var chkAccrList = feeAccrList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                            string getPO = string.Empty;
                            if (chkAccrList != null)
                            {
                                getPO = chkAccrList.PRO_NO;
                            }
                            else
                            {
                                var chkInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                getPO = chkInvList == null ? "" : chkInvList.PRO_NO;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal, getPO);
                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelTotalList.Add(modelTotal);

                        modelList.AddRange(modelTotalList);
                        #endregion

                        #region INV
                        modelTotal = new AccruedReportViewModel();
                        modelTotal.CHARGE = "Inv No.";
                        modelTotalList = new List<AccruedReportViewModel>();

                        #region set value column month INV No. 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {

                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            var chkAccrList = feeAccrList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                            string getINVNo = string.Empty;
                            if (chkAccrList != null)
                            {
                                getINVNo = chkAccrList.INV_NO;
                            }
                            else
                            {
                                var chkInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                getINVNo = chkInvList == null ? "" : chkInvList.INV_NO;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal, getINVNo);
                            iLoop++;
                            mnth++;
                        }

                        #endregion

                        modelTotalList.Add(modelTotal);

                        modelList.AddRange(modelTotalList);
                        #endregion
                        //}//end  if (feeAccrList.Any())

                    }//fee channels
                    #endregion

                    #region Summary Grand
                    if (arrMonthGrandTrxn.ToList().All(m => m == 0))
                    { }
                    else
                    {

                        var modelGrand = new AccruedReportViewModel();
                        modelGrand.CHARGE = "Total All Trxn.";
                        var modelGrandList = new List<AccruedReportViewModel>();
                        int iGrand = 0;
                        //foreach (var grandTrxn in arrMonthGrandTrxn.ToArray())
                        //{
                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iGrand];

                        //    modelGrand.GetType().GetProperty(monthIndex).SetValue(modelGrand,
                        //    Convert.ToString(string.Format("{0:#,##0.####}", grandTrxn)));


                        //    iGrand++;

                        //}

                        //----get value invoice accrued <> 3

                        var feeInvList = entFeeInv.ToList();
                        #region set value column month Total Grand Trxn
                        int mnth = monthS;
                        int yrr = yearS;
                        int iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;


                            modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                        Convert.ToString(string.Format("{0:#,##0.####}", arrMonthGrandTrxn[iLoop])));

                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelGrandList.Add(modelGrand);

                        modelList.AddRange(modelGrandList);

                        modelGrand = new AccruedReportViewModel();
                        modelGrand.CHARGE = "Grand Total";
                        modelGrandList = new List<AccruedReportViewModel>();
                        iGrand = 0;

                        #region set value column month Total Grand 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                            Convert.ToString(string.Format("{0:#,##0.####}", arrMonthGrnadTotal[iLoop])));
                            iLoop++;
                            mnth++;
                        }

                        #endregion

                        modelGrandList.Add(modelGrand);

                        modelList.AddRange(modelGrandList);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }

        public List<AccruedReportViewModel> GetActualandAccruedReportList_Summary(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedReportViewModel> modelList = new List<AccruedReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    #region prepare entity
                    var feeList = (from m in context.PAYMENT_ITEMS
                                   where m.IS_ACTIVE == true
                                   orderby m.GROUP_SEQ_CHANNELS
                                   select m).ToList();

                    var feeItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       orderby m.SEQUENCE
                                       select m).ToList();

                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();
                    var entFeeAccr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                      where (m.INV_YEAR * 12 + m.INV_MONTH) >= (yearS * 12 + monthS) && (m.INV_YEAR * 12 + m.INV_MONTH) <= (yearE * 12 + monthE)
                                      orderby m.ACCRUED_MONTH, m.ACCRUED_YEAR
                                      select m).ToList();


                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    var entFeeAccrItem = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB select m).ToList();

                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        feeItemList = (from m in feeItemList where m.COMPANY_CODE == companyCode && feeList.Any(p => m.PAYMENT_ITEMS_ID == p.ID) orderby m.SEQUENCE select m).ToList();
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();
                        entFeeAccr = entFeeAccr.Where(m => m.COMPANY_CODE == companyCode).ToList();

                    }
                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        entFeeInv = entFeeInv.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                        entFeeAccr = entFeeAccr.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();
                    }
                    //check list fee in accrued
                    if (entFeeAccr.Any())
                    {
                        feeList = feeList.Where(m => entFeeAccr.Any(a => a.PAYMENT_ITEMS_CODE == m.PAYMENT_ITEMS_CODE)).ToList();
                    }
                    feeList = feeList.OrderBy(m => m.GROUP_SEQ_CHANNELS).ToList();

                    #endregion

                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var get_month = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                    var _diffmonths = get_month + 1;
                    int currentMonth = DateTime.Now.Date.Month;
                    decimal[] arrMonthGrandTrxn = new decimal[_diffmonths];
                    decimal[] arrMonthGrnadTotal = new decimal[get_month + 1];

                    #region Detail
                    foreach (var item in feeList)
                    {

                        decimal[] arrMonthTotalTrxn = new decimal[_diffmonths];
                        decimal[] arrMonthTotal = new decimal[_diffmonths];
                        int rowfirst = 0;

                        bool chkTotalTrx = false;
                        var feeInvList = entFeeInv.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var feeAccrList = entFeeAccr.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)
                                                 orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                 select n).ToList();

                        var get_entFeeAccrItem = (from n in entFeeAccrItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                  select n).ToList();
                        //===group data charge list===
                        var group_charge = (from m in feeItemList where m.PAYMENT_ITEMS_ID == item.ID orderby m.CHARGE_TYPE descending select m).ToList();
                        var group_Trxncharge = group_charge.Where(m => m.CHARGE_TYPE == "TRXN").ToList();
                        #region Trxn + Amt

                        foreach (var data_charge in group_charge)
                        {
                            decimal[] arrMonthTrxn = new decimal[_diffmonths];
                            decimal[] arrMonthAMT = new decimal[_diffmonths];

                            //==accrued sub list==
                            var data_acc = get_entFeeAccrItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();
                            //==invoice sub list==
                            var data_inv = get_entFeeInvItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();

                            var model = new AccruedReportViewModel();
                            rowfirst++;
                            model.CHANNELS = rowfirst == 1 ? item.CHANNELS : "";
                            model.FEE = rowfirst == 1 ? item.PAYMENT_ITEMS_NAME : "";
                            model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME;


                            #region value before calculate avg  Transaction and Amount
                            //===charge ===
                            int mS = monthS;
                            int mE = monthE;
                            int yS = yearS;
                            int yE = yearE;
                            int mth = mS;
                            int yr = yS;
                            for (int n = 1; n <= _diffmonths; n++)
                            {
                                //mth = n;
                                if (mth == 13)
                                {
                                    mth = 1;
                                    yr = yr + 1;
                                }
                                var item_chrge = data_acc.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                if (item_chrge != null)
                                {
                                    if (data_charge.CHARGE_TYPE == "TRXN")
                                    {
                                        arrMonthTrxn[n - 1] = (item_chrge.TRANSACTIONS ?? 0);
                                        arrMonthTotalTrxn[n - 1] += (item_chrge.TRANSACTIONS ?? 0);
                                        arrMonthGrandTrxn[n - 1] += (item_chrge.TRANSACTIONS ?? 0);
                                    }
                                    if (data_charge.CHARGE_TYPE == "MDR")
                                    {
                                        chkTotalTrx = false;
                                        arrMonthAMT[n - 1] = (item_chrge.ACTUAL_AMOUNT ?? 0);
                                    }
                                }
                                mth++;
                            }
                            #endregion

                            #region Trxn
                            int iTrxn = 0;

                            foreach (var trx in arrMonthTrxn.ToArray())
                            {
                                //if (arrMonthTrxn.ToList().All(m => m == 0))
                                //{ break; }


                                if (trx != 0)
                                {
                                    //Accrued
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];
                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", trx)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => m.INV_MONTH == iTrxn + 1).FirstOrDefault();
                                    arrMonthTrxn[iTrxn] = (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);

                                    arrMonthTotalTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);
                                    arrMonthGrandTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);

                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];
                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0))));

                                }
                                //}

                                iTrxn++;
                            }


                            #region set value column month transaction
                            int mnth = monthS;
                            int yrr = yearS;
                            int iLoop = 0;
                            while (iLoop < _diffmonths)
                            {
                                if (arrMonthTrxn.ToList().All(m => m == 0))
                                { break; }
                                if (mnth == 13)
                                {
                                    mnth = 1;
                                    yrr++;
                                }
                                string yStr = yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                                Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTrxn[iLoop])));
                                iLoop++;
                                mnth++;
                            }

                            #endregion
                            #endregion


                            #region amt
                            foreach (var amt in arrMonthAMT.ToArray())
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }

                                if (amt != 0)
                                {
                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", amt)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => m.INV_MONTH == iTrxn + 1).FirstOrDefault();
                                    arrMonthAMT[iTrxn] = (get_data_inv != null ? get_data_inv.ACTUAL_AMOUNT ?? 0 : 0);

                                    //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    //model.GetType().GetProperty(monthIndex).SetValue(model,
                                    //Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.ACTUAL_AMOUNT ?? 0 : 0))));



                                }
                                //}

                                iTrxn++;

                            }
                            #region set value column month amount
                            mnth = monthS;
                            yrr = yearS;
                            iLoop = 0;
                            while (iLoop < _diffmonths)
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }
                                if (mnth == 13)
                                {
                                    mnth = 1;
                                    yrr++;
                                }
                                string yStr = yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                                model.GetType().GetProperty((monthIndex)).SetValue(model,
                                Convert.ToString(string.Format("{0:#,##0.####}", arrMonthAMT[iLoop])));
                                iLoop++;
                                mnth++;
                            }

                            #endregion


                            #endregion

                            modelList.Add(model);
                            #region Total Trxn
                            if (rowfirst == group_Trxncharge.Count())
                            {
                                chkTotalTrx = true;
                            }
                            //if (chkTotalTrx)
                            //{
                            //    var inclmodelList = new List<AccruedReportViewModel>();
                            //    model = new AccruedReportViewModel();
                            //    model.CHARGE = "Total Trxn";
                            //    for (int i = 0; i < 12; i++)
                            //    {
                            //        string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];

                            //        model.GetType().GetProperty(monthIndex).SetValue(model,
                            //                   Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotalTrxn[i])));
                            //    }
                            //    inclmodelList.Add(model);
                            //    modelList.AddRange(inclmodelList);

                            //}
                            #region set value column month total transaction
                            mnth = monthS;
                            yrr = yearS;
                            iLoop = 0;
                            if (chkTotalTrx)
                            {
                                var inclmodelList = new List<AccruedReportViewModel>();
                                model = new AccruedReportViewModel();
                                model.CHARGE = "Total Trxn";

                                while (iLoop < _diffmonths)
                                {
                                    if (mnth == 13)
                                    {
                                        mnth = 1;
                                        yrr++;
                                    }
                                    string yStr = yrr.ToString().Substring(2, 2);
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                                    model.GetType().GetProperty((monthIndex)).SetValue(model,
                                    Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotalTrxn[iLoop])));
                                    iLoop++;
                                    mnth++;
                                }
                                inclmodelList.Add(model);
                                modelList.AddRange(inclmodelList);
                            }

                            #endregion

                            #endregion
                        }//fee
                        #endregion
                        if (feeAccrList.Any())
                        {
                            #region Charge Trxn + Amt
                            foreach (var data_charge in group_charge)
                            {
                                decimal[] arrMonthCharge = new decimal[_diffmonths];
                                var data = get_entFeeAccrItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();


                                var data_inv = get_entFeeInvItem.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == data_charge.PAYMENT_ITEMS_FEE_NAME).ToList();

                                var model = new AccruedReportViewModel();
                                rowfirst++;
                                model.CHANNELS = "";
                                model.FEE = "";
                                if (data.Where(m => (m.TRANSACTIONS ?? 0) != 0 && (m.RATE_TRANS ?? 0) != 0).FirstOrDefault() != null)
                                {
                                    model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME + " Rate Charge (" + Convert.ToString(string.Format("{0:#,##0.####}", data.Where(m => (m.TRANSACTIONS ?? 0) != 0).FirstOrDefault().RATE_TRANS)) + ")";

                                }
                                else
                                {
                                    model.CHARGE = data_charge.PAYMENT_ITEMS_FEE_NAME + ((data.Where(m => (m.RATE_AMT ?? 0) != 0).FirstOrDefault() == null) ? ""
                                        : " Rate Charge (" + Convert.ToString(string.Format("{0:#,##0.####}", data.Where(m => (m.RATE_AMT ?? 0) != 0).FirstOrDefault().RATE_AMT)) + "% )");
                                }


                                int mS = monthS;
                                int mE = monthE;
                                int yS = yearS;
                                int yE = yearE;
                                int mth = mS;
                                int yr = yS;
                                for (int n = 1; n <= _diffmonths; n++)
                                {
                                    //mth = n;
                                    if (mth == 13)
                                    {
                                        mth = 1;
                                        yr = yr + 1;
                                    }
                                    var item_chrge = data.Where(q => (q.INV_YEAR * 12) + q.INV_MONTH == (yr * 12) + mth).FirstOrDefault();
                                    if (item_chrge != null)
                                    {
                                        //Chrage
                                        arrMonthCharge[n - 1] = (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                        //Total
                                        arrMonthTotal[n - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                        //Grnad Total
                                        arrMonthGrnadTotal[n - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);
                                    }
                                    mth++;
                                }

                                //foreach (var item_chrge in data)
                                //{

                                //    //Chrage
                                //    arrMonthCharge[(item_chrge.INV_MONTH ?? 0) - 1] = (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                //    //Total
                                //    arrMonthTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                //    //Grnad Total
                                //    arrMonthGrnadTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                //}//charge


                                int iCharge = 0;

                                #region charge
                                foreach (var chrge in arrMonthCharge.ToArray())
                                {
                                    if (arrMonthCharge.ToList().All(m => m == 0))
                                    { break; }

                                    if (chrge != 0)
                                    {
                                        ////Accrued
                                        //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                        //model.GetType().GetProperty(monthIndex).SetValue(model,
                                        //Convert.ToString(string.Format("{0:#,##0.####}", chrge)));
                                    }
                                    else
                                    {
                                        //Invoice
                                        var get_data_inv = data_inv.Where(m => m.INV_MONTH == iCharge + 1).FirstOrDefault();
                                        arrMonthCharge[iCharge] = (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                        arrMonthTotal[iCharge] += (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);

                                        //string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                        //model.GetType().GetProperty(monthIndex).SetValue(model,
                                        //Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0))));


                                    }
                                    //}

                                    iCharge++;

                                }
                                #region set value column month charge amount
                                int chgmnth = monthS;
                                int chgyrr = yearS;
                                int chgiLoop = 0;
                                while (chgiLoop < _diffmonths)
                                {
                                    if (arrMonthCharge.ToList().All(m => m == 0))
                                    { break; }
                                    if (chgmnth == 13)
                                    {
                                        chgmnth = 1;
                                        chgyrr++;
                                    }
                                    string yStr = chgyrr.ToString().Substring(2, 2);
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[chgmnth - 1] + yStr;
                                    model.GetType().GetProperty((monthIndex)).SetValue(model,
                                    Convert.ToString(string.Format("{0:#,##0.####}", arrMonthCharge[chgiLoop])));
                                    chgiLoop++;
                                    chgmnth++;
                                }

                                #endregion


                                #endregion
                                modelList.Add(model);
                            }//fee
                            #endregion

                            #region Total
                            int indexTotal = 0;
                            var modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "Total";
                            var modelTotalList = new List<AccruedReportViewModel>();
                            //foreach (var total in arrMonthTotal.ToArray())
                            //{
                            //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[indexTotal];

                            //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal,
                            //    Convert.ToString(string.Format("{0:#,##0.####}", total)));

                            //    indexTotal++;

                            //}
                            #region set value column month Total 
                            int mnth = monthS;
                            int yrr = yearS;
                            int iLoop = 0;
                            while (iLoop < _diffmonths)
                            {
                                if (mnth == 13)
                                {
                                    mnth = 1;
                                    yrr++;
                                }
                                string yStr = yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                                modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal,
                                Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotal[iLoop])));
                                iLoop++;
                                mnth++;
                            }

                            #endregion


                            modelTotalList.Add(modelTotal);
                            modelList.AddRange(modelTotalList);
                            #endregion

                            #region PO
                            modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "PO No.";
                            modelTotalList = new List<AccruedReportViewModel>();


                            //for (int i = 1; i <= 12; i++)
                            //{

                            //    //var chkAccrList = feeAccrList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                            //    //if (chkAccrList != null)
                            //    //{
                            //    //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                            //    //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkAccrList == null ? "" : chkAccrList.PRO_NO);
                            //    //}
                            //    //else
                            //    //{
                            //    var chkInvList = feeInvList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                            //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                            //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkInvList == null ? "" : chkInvList.PRO_NO);

                            //    //}

                            //}

                            #region set value column month PO 
                            mnth = monthS;
                            yrr = yearS;
                            iLoop = 0;
                            while (iLoop < _diffmonths)
                            {

                                if (mnth == 13)
                                {
                                    mnth = 1;
                                    yrr++;
                                }
                                var chkAccrList = feeAccrList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                string getPO = string.Empty;
                                if (chkAccrList != null)
                                {
                                    getPO = chkAccrList.PRO_NO;
                                }
                                else
                                {
                                    var chkInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                    getPO = chkInvList == null ? "" : chkInvList.PRO_NO;
                                }
                                string yStr = yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                                modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal, getPO);
                                iLoop++;
                                mnth++;
                            }

                            #endregion


                            modelTotalList.Add(modelTotal);

                            modelList.AddRange(modelTotalList);
                            #endregion

                            #region INV
                            modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "Inv No.";
                            modelTotalList = new List<AccruedReportViewModel>();
                            //for (int i = 1; i <= 12; i++)
                            //{
                            //    //var chkAccrList = feeAccrList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                            //    //if (chkAccrList != null)
                            //    //{
                            //    //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                            //    //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkAccrList == null ? "" : chkAccrList.INV_NO);
                            //    //}
                            //    //else
                            //    //{
                            //    var chkInvList = feeInvList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                            //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                            //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkInvList == null ? "" : chkInvList.INV_NO);

                            //    //}
                            //}
                            #region set value column month INV No. 
                            mnth = monthS;
                            yrr = yearS;
                            iLoop = 0;
                            while (iLoop < _diffmonths)
                            {

                                if (mnth == 13)
                                {
                                    mnth = 1;
                                    yrr++;
                                }
                                var chkAccrList = feeAccrList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                string getINVNo = string.Empty;
                                if (chkAccrList != null)
                                {
                                    getINVNo = chkAccrList.INV_NO;
                                }
                                else
                                {
                                    var chkInvList = feeInvList.Where(m => (m.INV_YEAR * 12) + m.INV_MONTH == (yrr * 12) + mnth).FirstOrDefault();
                                    getINVNo = chkInvList == null ? "" : chkInvList.INV_NO;
                                }
                                string yStr = yrr.ToString().Substring(2, 2);
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                                modelTotal.GetType().GetProperty((monthIndex)).SetValue(modelTotal, getINVNo);
                                iLoop++;
                                mnth++;
                            }

                            #endregion

                            modelTotalList.Add(modelTotal);

                            modelList.AddRange(modelTotalList);
                            #endregion
                        }

                    }//fee channels
                    #endregion

                    #region Summary Grand
                    if (arrMonthGrandTrxn.ToList().All(m => m == 0))
                    { }
                    else
                    {

                        var modelGrand = new AccruedReportViewModel();
                        modelGrand.CHARGE = "Total All Trxn.";
                        var modelGrandList = new List<AccruedReportViewModel>();
                        int iGrand = 0;
                        //foreach (var grandTrxn in arrMonthGrandTrxn.ToArray())
                        //{
                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iGrand];

                        //    modelGrand.GetType().GetProperty(monthIndex).SetValue(modelGrand,
                        //    Convert.ToString(string.Format("{0:#,##0.####}", grandTrxn)));


                        //    iGrand++;

                        //}

                        #region set value column month Total Grand Trxn
                        int mnth = monthS;
                        int yrr = yearS;
                        int iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                            Convert.ToString(string.Format("{0:#,##0.####}", arrMonthGrandTrxn[iLoop])));
                            iLoop++;
                            mnth++;
                        }

                        #endregion


                        modelGrandList.Add(modelGrand);

                        modelList.AddRange(modelGrandList);

                        modelGrand = new AccruedReportViewModel();
                        modelGrand.CHARGE = "Grand Total";
                        modelGrandList = new List<AccruedReportViewModel>();
                        iGrand = 0;
                        //foreach (var grandTotal in arrMonthGrnadTotal.ToArray())
                        //{
                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iGrand];

                        //    modelGrand.GetType().GetProperty(monthIndex).SetValue(modelGrand,
                        //    Convert.ToString(string.Format("{0:#,##0.####}", grandTotal)));

                        //    iGrand++;

                        //}
                        #region set value column month Total Grand 
                        mnth = monthS;
                        yrr = yearS;
                        iLoop = 0;
                        while (iLoop < _diffmonths)
                        {
                            if (mnth == 13)
                            {
                                mnth = 1;
                                yrr++;
                            }
                            string yStr = yrr.ToString().Substring(2, 2);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yStr;
                            modelGrand.GetType().GetProperty((monthIndex)).SetValue(modelGrand,
                            Convert.ToString(string.Format("{0:#,##0.####}", arrMonthGrnadTotal[iLoop])));
                            iLoop++;
                            mnth++;
                        }

                        #endregion

                        modelGrandList.Add(modelGrand);

                        modelList.AddRange(modelGrandList);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }


        public List<AccruedReportViewModel> GetAllActualReportList(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedReportViewModel> modelList = new List<AccruedReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var feeList = (from m in context.PAYMENT_ITEMS
                                   where m.IS_ACTIVE == true
                                   orderby m.GROUP_SEQ_CHANNELS
                                   select m).ToList();
                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where (m.INV_YEAR * 12) + m.INV_MONTH >= (yearS * 12) + monthS
                                     && (m.INV_YEAR * 12) + m.INV_MONTH <= (yearE * 12) + monthE
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();


                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();

                    }
                    else
                    {
                        if (bu != "ALL")
                        {
                            var companyData = (from com in context.COMPANies where com.IsPaymentFee == true orderby com.Bussiness_Unit select com).ToList();
                            companyData = companyData.Where(m => m.Bussiness_Unit == bu).ToList();

                            feeList = (from m in feeList where companyData.Any(c => m.COMPANY_CODE == c.BAN_COMPANY) orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                            entFeeInv = entFeeInv.Where(m => companyData.Any(c => m.COMPANY_CODE == c.BAN_COMPANY)).ToList();
                        }
                    }
                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();

                        entFeeInv = entFeeInv.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();

                    }
                    feeList = feeList.OrderBy(m => m.GROUP_SEQ_CHANNELS).ToList();



                    var feePriceCatList = feeList.GroupBy(g => new { g.PAYMENT_ITEMS_NAME, g.CHANNELS }).ToList();


                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    int rowfirst = 0;
                    foreach (var item in feePriceCatList)
                    {
                        #region Trxn & Actual AMT
                        rowfirst = 0;

                        var allpricecat = feeList.Where(m => m.PAYMENT_ITEMS_NAME == item.Key.PAYMENT_ITEMS_NAME).ToList();

                        var feeInv = (from m in entFeeInv
                                      join n in entFeeInvItem on m.INV_NO equals n.INV_NO
                                      where allpricecat.Any(p => m.PAYMENT_ITEMS_CODE == p.PAYMENT_ITEMS_CODE)
                                      orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                      select new { m, n }).ToList();



                        rowfirst++;
                        var model = new AccruedReportViewModel();
                        model.CHANNELS = rowfirst == 1 ? item.Key.CHANNELS : "";
                        model.FEE = rowfirst == 1 ? item.Key.PAYMENT_ITEMS_NAME : "";


                        //Data Trxn
                        #region feeInv Trxn
                        var culture = CultureInfo.GetCultureInfo("en-US");
                        var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                        var getmonth = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                        var mnth = getmonth + 1;
                        int _getmnth = monthS;
                        int _getyr = yearS;
                        #region Total Trxn

                        _getmnth = monthS;
                        _getyr = yearS;
                        for (int i = 1; i <= mnth; i++)
                        {
                            if (_getmnth == 13)
                            {
                                _getmnth = 1;
                                _getyr = _getyr + 1;
                            }
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                            var valueFeeLst = feeInv.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                            model.CHARGE = "Trxn Fee";
                            model.GetType().GetProperty(monthIndex).SetValue(model,
                          Convert.ToString(string.Format("{0:#,##0.####}", (valueFeeLst.Sum(m => m.n.TRANSACTIONS ?? 0)))));

                            _getmnth++;
                        }//row month
                        modelList.Add(model);


                        //End sum total trxn

                        #endregion


                        #region Amount MDR




                        model = new AccruedReportViewModel();
                        _getmnth = monthS;
                        _getyr = yearS;
                        for (int i = 1; i <= mnth; i++)
                        {
                            if (_getmnth == 13)
                            {
                                _getmnth = 1;
                                _getyr = _getyr + 1;
                            }
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                            var valueFeeLst = feeInv.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                            model.CHARGE = "Amount MDR";
                            model.GetType().GetProperty(monthIndex).SetValue(model,
                  Convert.ToString(string.Format("{0:#,##0.####}", (valueFeeLst.Sum(m => m.n.ACTUAL_AMOUNT ?? 0)))));

                            _getmnth++;
                        }//row month
                        modelList.Add(model);



                        //End sum total trxn

                        #endregion
                        #endregion

                        #endregion

                        //End Loop

                        //total charging

                        model = new AccruedReportViewModel();


                        model.CHARGE = "Total";
                        _getmnth = monthS;
                        _getyr = yearS;
                        for (int i = 1; i <= mnth; i++)
                        {
                            if (_getmnth == 13)
                            {
                                _getmnth = 1;
                                _getyr = _getyr + 1;
                            }
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                            var valueInvLst = feeInv.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                            if (valueInvLst.Any())
                            {
                                model.GetType().GetProperty(monthIndex).SetValue(model,
                       Convert.ToString(string.Format("{0:#,##0.####}", (valueInvLst.Sum(m => m.n.TOTAL_CHARGE_AMOUNT ?? 0)))));

                            }
                            else
                            {
                                model.GetType().GetProperty(monthIndex).SetValue(model,
                      Convert.ToString(string.Format("{0:#,##0.####}", 0)));
                            }
                            _getmnth++;
                        }
                        modelList.Add(model);

                    }
                    //End Fee List
                    #region Gran Total

                    var InvLst = (from m in entFeeInv
                                  join n in entFeeInvItem on m.INV_NO equals n.INV_NO
                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                  select new { m, n }).ToList();
                    if (chnn != "ALL")
                    {
                        InvLst = InvLst.Where(v => feeList.Any(m => v.m.PAYMENT_ITEMS_CODE == m.PAYMENT_ITEMS_CODE)).ToList();
                    }
                    if (InvLst.Any())
                    {

                        List<AccruedReportViewModel> granmodelList = new List<AccruedReportViewModel>();
                        var granmodel = new AccruedReportViewModel();
                        granmodel.CHARGE = "Total All Trxn";
                        var culture = CultureInfo.GetCultureInfo("en-US");
                        var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                        var getmonth = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                        var mnth = getmonth + 1;
                        int _getmnth = monthS;
                        int _getyr = yearS;
                        for (int i = 1; i <= mnth; i++)
                        {
                            if (_getmnth == 13)
                            {
                                _getmnth = 1;
                                _getyr = _getyr + 1;
                            }
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                            var valueInvLst = InvLst.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                            if (valueInvLst.Any())
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                       Convert.ToString(string.Format("{0:#,##0.####}", (valueInvLst.Sum(m => m.n.TRANSACTIONS ?? 0)))));

                            }
                            else
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                      Convert.ToString(string.Format("{0:#,##0.####}", 0)));
                            }
                            _getmnth++;
                        }

                        granmodelList.Add(granmodel);


                        granmodel = new AccruedReportViewModel();
                        granmodel.CHARGE = "Grand Total";
                        _getmnth = monthS;
                        _getyr = yearS;
                        for (int i = 1; i <= mnth; i++)
                        {
                            if (_getmnth == 13)
                            {
                                _getmnth = 1;
                                _getyr = _getyr + 1;
                            }
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                            var valueInvLst = InvLst.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                            if (valueInvLst.Any())
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                       Convert.ToString(string.Format("{0:#,##0.####}", (valueInvLst.Sum(m => m.n.TOTAL_CHARGE_AMOUNT ?? 0)))));

                            }
                            else
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                      Convert.ToString(string.Format("{0:#,##0.####}", 0)));
                            }
                            _getmnth++;
                        }

                        granmodelList.Add(granmodel);

                        modelList.AddRange(granmodelList);
                    }
                    #endregion



                }





            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }

        public List<AccruedReportViewModel> GetActualReportList(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
        {
            List<AccruedReportViewModel> modelList = new List<AccruedReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var feeList = (from m in context.PAYMENT_ITEMS
                                   where m.IS_ACTIVE == true
                                   orderby m.GROUP_SEQ_CHANNELS
                                   select m).ToList();
                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where (m.INV_YEAR * 12) + m.INV_MONTH >= (yearS * 12) + monthS
                                     && (m.INV_YEAR * 12) + m.INV_MONTH <= (yearE * 12) + monthE
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();


                    if (!string.IsNullOrEmpty(companyCode))
                    {
                        feeList = (from m in feeList where m.COMPANY_CODE == companyCode orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                        entFeeInv = entFeeInv.Where(m => m.COMPANY_CODE == companyCode).ToList();

                    }
                    //else
                    //{
                    //    if (bu != "ALL")
                    //    {
                    //        var companyData = (from com in context.COMPANies where com.IsPaymentFee == true orderby com.Bussiness_Unit select com).ToList();
                    //        companyData = companyData.Where(m => m.Bussiness_Unit == bu).ToList();

                    //        feeList = (from m in feeList where companyData.Any(c=> m.COMPANY_CODE == c.BAN_COMPANY) orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();
                    //        entFeeInv = entFeeInv.Where(m => companyData.Any(c => m.COMPANY_CODE == c.BAN_COMPANY) ).ToList();
                    //    }
                    //}
                    if (chnn != "ALL")
                    {
                        feeList = (from m in feeList where m.CHANNELS == chnn orderby m.CHANNELS, m.PAYMENT_ITEMS_NAME select m).ToList();

                        entFeeInv = entFeeInv.Where(m => feeList.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)).ToList();

                    }
                    feeList = feeList.OrderBy(m => m.GROUP_SEQ_CHANNELS).ToList();


                    //if (fee > 1)
                    //{
                    //    if (fee == 2)
                    //    {
                    //        entFeeInv = (from m in entFeeInv
                    //                     where m.IS_STATUS != "3"
                    //                     orderby m.INV_MONTH, m.INV_YEAR
                    //                     select m).ToList();
                    //    }
                    //}




                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM select m).ToList();
                    int rowfirst = 0;
                    foreach (var item in feeList)
                    {
                        #region Trxn & Actual AMT
                        rowfirst = 0;
                        var feeInv = (from m in entFeeInv
                                      join n in entFeeInvItem on m.INV_NO equals n.INV_NO
                                      where m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE
                                      orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                      select new { m, n }).ToList();

                        var feeInvGroup = feeInv.GroupBy(m => new { m.n.PAYMENT_ITEMS_FEE_ITEM }).ToList();

                        List<AccruedReportViewModel> modelTotalList = new List<AccruedReportViewModel>();

                        bool flagTotalTrxnRow = false;
                        //int totalTrxnRow = 0;
                        var feeInvGroupTrxnCount = feeInv.Where(m => m.n.TRANSACTIONS != 0)
                             .GroupBy(m => new { m.n.PAYMENT_ITEMS_FEE_ITEM }).ToList();
                        foreach (var subitem in feeInvGroup)
                        {
                            rowfirst++;
                            var model = new AccruedReportViewModel();
                            model.CHANNELS = rowfirst == 1 ? item.CHANNELS : "";
                            model.FEE = rowfirst == 1 ? item.PAYMENT_ITEMS_NAME : "";
                            var feeInvList = feeInv.Where(m => m.n.PAYMENT_ITEMS_FEE_ITEM == subitem.Key.PAYMENT_ITEMS_FEE_ITEM).ToList();




                            model.CHARGE = subitem.Key.PAYMENT_ITEMS_FEE_ITEM;//--1

                            //Data Trxn
                            #region feeInv Trxn
                            var culture = CultureInfo.GetCultureInfo("en-US");
                            var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                            var getmonth = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                            var mnth = getmonth + 1;
                            int _getmnth = monthS;
                            int _getyr = yearS;
                            for (int i = 1; i <= mnth; i++)
                            {
                                if (_getmnth == 13)
                                {
                                    _getmnth = 1;
                                    _getyr = _getyr + 1;
                                }
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                                var valueFeeLst = feeInvList.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).FirstOrDefault();
                                if (valueFeeLst != null)
                                {
                                    if ((valueFeeLst.n.TRANSACTIONS ?? 0) != 0)
                                    {
                                        model.GetType().GetProperty(monthIndex).SetValue(model,
                               Convert.ToString(string.Format("{0:#,##0.####}", (valueFeeLst.n.TRANSACTIONS ?? 0))));


                                        //totalChageAmt1 = totalChageAmt1 + (valueFeeLst.n.TRANSACTIONS ?? 0);
                                        //sumChageAmt1 = sumChageAmt1 + (valueFeeLst.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                        ////Sum All Trans
                                        //sumAllTrxn1 += (valueFeeLst.n.TRANSACTIONS ?? 0);
                                        ////Sum All Grand Total
                                        //sumAllTotal1 += (valueFeeLst.n.TOTAL_CHARGE_AMOUNT ?? 0);
                                    }
                                    else
                                    {
                                        flagTotalTrxnRow = false;
                                        model.GetType().GetProperty(monthIndex).SetValue(model,
                               Convert.ToString(string.Format("{0:#,##0.####}", (valueFeeLst.n.ACTUAL_AMOUNT ?? 0))));

                                    }
                                }
                                _getmnth++;

                            }
                            modelList.Add(model);
                            #region Total Trxn
                            if (rowfirst == feeInvGroupTrxnCount.Count())
                            {
                                //++totalTrxnRow;
                                flagTotalTrxnRow = true;
                            }


                            if (flagTotalTrxnRow)
                            {

                                AccruedReportViewModel modelTotal = new AccruedReportViewModel();
                                _getmnth = monthS;
                                _getyr = yearS;
                                for (int i = 1; i <= mnth; i++)
                                {
                                    if (_getmnth == 13)
                                    {
                                        _getmnth = 1;
                                        _getyr = _getyr + 1;
                                    }
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                                    var valueFeeLst = feeInv.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                                    modelTotal.CHARGE = "Total Trxn";
                                    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal,
                          Convert.ToString(string.Format("{0:#,##0.####}", (valueFeeLst.Sum(m => m.n.TRANSACTIONS ?? 0)))));

                                    _getmnth++;
                                }//row month
                                modelTotalList.Add(modelTotal);

                                modelList.AddRange(modelTotalList);


                                //End sum total trxn
                            }
                            #endregion
                            #endregion
                        }
                        #endregion

                        //End Loop

                        //total charging

                        #region charging 
                        foreach (var subitem in feeInvGroup)
                        {
                            rowfirst++;
                            var model = new AccruedReportViewModel();
                            model.CHANNELS = rowfirst == 1 ? item.CHANNELS : "";
                            model.FEE = rowfirst == 1 ? item.PAYMENT_ITEMS_NAME : "";
                            var feeInvList = feeInv.Where(m => m.n.PAYMENT_ITEMS_FEE_ITEM == subitem.Key.PAYMENT_ITEMS_FEE_ITEM).ToList();


                            var chargetranName = feeInvList.GroupBy(m => m.n.RATE_TRANS).Where(m => (m.Key ?? 0) != 0).ToList();
                            var chargeamtName = feeInvList.GroupBy(m => m.n.RATE_AMT).Where(m => (m.Key ?? 0) != 0).ToList();



                            model.CHARGE = subitem.Key.PAYMENT_ITEMS_FEE_ITEM;//--1


                            if (chargetranName.Any())
                            {
                                List<string> chgTrxn = new List<string>();
                                string strchgTrxn = "";
                                foreach (var trx in chargetranName)
                                {
                                    chgTrxn.Add(string.Format("{0:#,##0.####}", trx.Key));
                                }
                                strchgTrxn = string.Join(" ,", chgTrxn);
                                model.CHARGE = subitem.Key.PAYMENT_ITEMS_FEE_ITEM + "Rate Charged ( " + strchgTrxn + " )";//--1
                            }
                            if (chargeamtName.Any())
                            {
                                List<string> chgAmt = new List<string>();
                                string strchgAmt = string.Format("{0:#,##0.#}", chargeamtName.Max(m => (m.Key ?? 0))) + " %";


                                model.CHARGE = subitem.Key.PAYMENT_ITEMS_FEE_ITEM + " Rate Charged ( " + strchgAmt + " )";//--1
                            }
                            if (!chargetranName.Any() && !chargeamtName.Any())
                            { continue; }
                            var culture = CultureInfo.GetCultureInfo("en-US");
                            var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                            var getmonth = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                            var mnth = getmonth + 1;
                            int _getmnth = monthS;
                            int _getyr = yearS;
                            for (int i = 1; i <= mnth; i++)
                            {
                                if (_getmnth == 13)
                                {
                                    _getmnth = 1;
                                    _getyr = _getyr + 1;
                                }
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                                string poIndex = "" + _getmnth.ToString();
                                string invIndex = "" + _getmnth.ToString();
                                var valueFeeLst = feeInvList.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).FirstOrDefault();

                                if (valueFeeLst != null)
                                {
                                    model.GetType().GetProperty(monthIndex).SetValue(model,
                           Convert.ToString(string.Format("{0:#,##0.####}", (valueFeeLst.n.TOTAL_CHARGE_AMOUNT ?? 0))));


                                }
                                _getmnth++;
                            }
                            modelList.Add(model);

                            //End Loop
                        }
                        #endregion

                        if (feeInvGroup.Count() * 2 == rowfirst && feeInvGroup.Count > 0)
                        {
                            modelTotalList = new List<AccruedReportViewModel>();
                            AccruedReportViewModel modelTotal = new AccruedReportViewModel();


                            modelTotal.CHARGE = "Total";
                            var culture = CultureInfo.GetCultureInfo("en-US");
                            var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                            var getmonth = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                            var mnth = getmonth + 1;
                            int _getmnth = monthS;
                            int _getyr = yearS;
                            for (int i = 1; i <= mnth; i++)
                            {
                                if (_getmnth == 13)
                                {
                                    _getmnth = 1;
                                    _getyr = _getyr + 1;
                                }
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                                var valueInvLst = feeInv.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                                if (valueInvLst.Any())
                                {
                                    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal,
                           Convert.ToString(string.Format("{0:#,##0.####}", (valueInvLst.Sum(m => m.n.TOTAL_CHARGE_AMOUNT ?? 0)))));

                                }
                                else
                                {
                                    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal,
                          Convert.ToString(string.Format("{0:#,##0.####}", 0)));
                                }
                                _getmnth++;
                            }
                            modelTotalList.Add(modelTotal);
                            //PO 
                            modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "PO No.";
                            _getmnth = monthS;
                            _getyr = yearS;
                            for (int i = 1; i <= mnth; i++)
                            {
                                if (_getmnth == 13)
                                {
                                    _getmnth = 1;
                                    _getyr = _getyr + 1;
                                }
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                                var valueInvLst = feeInv.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).FirstOrDefault();


                                if (valueInvLst != null)
                                {
                                    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, valueInvLst.m.PRO_NO);
                                }
                                else
                                {
                                    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, "");
                                }
                                _getmnth++;
                            }


                            modelTotalList.Add(modelTotal);
                            //INV 
                            modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "Inv No.";
                            _getmnth = monthS;
                            _getyr = yearS;
                            for (int i = 1; i <= mnth; i++)
                            {
                                if (_getmnth == 13)
                                {
                                    _getmnth = 1;
                                    _getyr = _getyr + 1;
                                }
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                                var valueInvLst = feeInv.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).FirstOrDefault();


                                if (valueInvLst != null)
                                {
                                    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, valueInvLst.m.INV_NO);
                                }
                                else
                                {
                                    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, "");
                                }
                                _getmnth++;
                            }

                            modelTotalList.Add(modelTotal);

                        }


                        modelList.AddRange(modelTotalList);

                    }
                    //End Fee List
                    #region Gran Total

                    var InvLst = (from m in entFeeInv
                                  join n in entFeeInvItem on m.INV_NO equals n.INV_NO
                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                  select new { m, n }).ToList();
                    if (chnn != "ALL")
                    {
                        InvLst = InvLst.Where(v => feeList.Any(m => v.m.PAYMENT_ITEMS_CODE == m.PAYMENT_ITEMS_CODE)).ToList();
                    }
                    if (InvLst.Any())
                    {

                        List<AccruedReportViewModel> granmodelList = new List<AccruedReportViewModel>();
                        var granmodel = new AccruedReportViewModel();
                        granmodel.CHARGE = "Total All Trxn";
                        var culture = CultureInfo.GetCultureInfo("en-US");
                        var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                        var getmonth = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                        var mnth = getmonth + 1;
                        int _getmnth = monthS;
                        int _getyr = yearS;
                        for (int i = 1; i <= mnth; i++)
                        {
                            if (_getmnth == 13)
                            {
                                _getmnth = 1;
                                _getyr = _getyr + 1;
                            }
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                            var valueInvLst = InvLst.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                            if (valueInvLst.Any())
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                       Convert.ToString(string.Format("{0:#,##0.####}", (valueInvLst.Sum(m => m.n.TRANSACTIONS ?? 0)))));

                            }
                            else
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                      Convert.ToString(string.Format("{0:#,##0.####}", 0)));
                            }
                            _getmnth++;
                        }

                        granmodelList.Add(granmodel);

                        //granmodel = new AccruedReportViewModel();
                        //granmodel.CHARGE = "Total All Amount (MDR)";
                        //granmodel.Jan = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt1));
                        //granmodel.Feb = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt2));
                        //granmodel.Mar = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt3));
                        //granmodel.Apr = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt4));
                        //granmodel.May = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt5));
                        //granmodel.Jun = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt6));
                        //granmodel.Jul = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt7));
                        //granmodel.Aug = Convert.ToString(string.Format("{0:#,##0.####}", sumAllAmt8));

                        //modelTotalList.Add(granmodel);

                        granmodel = new AccruedReportViewModel();
                        granmodel.CHARGE = "Grand Total";
                        _getmnth = monthS;
                        _getyr = yearS;
                        for (int i = 1; i <= mnth; i++)
                        {
                            if (_getmnth == 13)
                            {
                                _getmnth = 1;
                                _getyr = _getyr + 1;
                            }
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[_getmnth - 1] + Convert.ToString(_getyr).Substring(2, 2);
                            var valueInvLst = InvLst.Where(m => (m.n.INV_YEAR * 12) + m.n.INV_MONTH == (_getyr * 12) + _getmnth).ToList();

                            if (valueInvLst.Any())
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                       Convert.ToString(string.Format("{0:#,##0.####}", (valueInvLst.Sum(m => m.n.TOTAL_CHARGE_AMOUNT ?? 0)))));

                            }
                            else
                            {
                                granmodel.GetType().GetProperty(monthIndex).SetValue(granmodel,
                      Convert.ToString(string.Format("{0:#,##0.####}", 0)));
                            }
                            _getmnth++;
                        }

                        granmodelList.Add(granmodel);

                        modelList.AddRange(granmodelList);
                    }
                    #endregion



                }





            }
            catch (Exception ex)
            {

                throw ex;
            }
            return modelList;
        }


        public ValidationResult ValidateFormData(AccruedDetailReportViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            try
            {

                if (!modelState.IsValid)
                {
                    result.ModelStateErrorList = UtilityService.GetModelStateErrors(modelState);
                    result.ErrorFlag = true;
                }
            }
            catch (Exception ex)
            {
                result.MessageType = ex.ToString();
                result.Message = ex.ToString();
            }

            return result;
        }

    }
}