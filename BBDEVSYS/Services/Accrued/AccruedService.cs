using BBDEVSYS.Services.Abstract;
using BBDEVSYS.ViewModels.Accrued;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.Models.Shared;
using System.Web.Mvc;
using System.Globalization;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Services.Shared;
using System.Transactions;
using BBDEVSYS.Content.text;
using System.Web.Script.Serialization;
using System.Data;
using BBDEVSYS.ViewModels.AccruedReport;

using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Dynamic;
using System.Reflection;

namespace BBDEVSYS.Services.Accrued
{
    public class AccruedService : AbstractControllerService<AccruedViewModel>
    {
        public override AccruedViewModel GetDetail(int id)
        {
            AccruedViewModel accruedViewModel = NewFormData();
            var userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var feeAccrued = (from m in context.FEE_ACCRUED_PLAN
                                      where m.ACCRUED_ID == id
                                      select m).FirstOrDefault();

                    MVMMappingService.MoveData(feeAccrued, accruedViewModel);
                    accruedViewModel.MODIFIED_BY = userInfo.UserCode;
                    accruedViewModel.MODIFIED_DATE = DateTime.Now;
                    int accruedNo = 0;
                    string companyCode = "";
                    if (feeAccrued != null)
                    {
                        accruedNo = feeAccrued.ACCRUED_ID;
                        companyCode = feeAccrued.COMPANY_CODE;

                        var entuser = (from m in context.USERS where m.USERID == feeAccrued.APPROVED_BY select m).FirstOrDefault();
                        var entcompany = (from m in context.COMPANies where m.IsPaymentFee == true && m.BAN_COMPANY == companyCode select m).FirstOrDefault();
                        accruedViewModel.COMPANY_CODE_NAME = entcompany == null ? "" : entcompany.COMPANY_NAME_EN;
                        accruedViewModel.APPROVED_BY_NAME = entuser == null ? "" : entuser.NAME;
                    }

                    //Get line item
                    List<FEE_ACCRUED_PLAN_ITEM> accruedItemList = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                                                   where m.ACCRUED_ID == accruedNo
                                                                   select m).OrderBy(m => m.SEQUENCE).ToList();

                    //Get line item Sub
                    List<FEE_ACCRUED_PLAN_ITEM_SUB> accruedItemSubList = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB

                                                                          select m).OrderBy(m => m.SEQUENCE).ToList();

                    //get Payment Items
                    var entpymItems = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true && m.COMPANY_CODE == companyCode select m).ToList();
                    foreach (var item in accruedItemList)
                    {
                        var getpymItems = entpymItems.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).FirstOrDefault();
                        AccruedDetailViewModel accruedItemViewModel = new AccruedDetailViewModel();
                        MVMMappingService.MoveData(item, accruedItemViewModel);
                        accruedItemViewModel.Supplier = getpymItems == null ? "" : getpymItems.Supplier;
                        accruedItemViewModel.GL_ACCOUNT = getpymItems == null ? "" : getpymItems.GL_ACCOUNT;
                        accruedItemViewModel.COST_CENTER = getpymItems == null ? "" : getpymItems.COST_CENTER;
                        accruedItemViewModel.FUND_CODE = getpymItems == null ? "" : getpymItems.FUND_CODE;
                        accruedItemViewModel.CHANNELS = getpymItems == null ? "" : getpymItems.CHANNELS;
                        accruedItemViewModel.PAYMENT_ITEMS_NAME = getpymItems == null ? "" : getpymItems.PAYMENT_ITEMS_NAME;
                        accruedItemViewModel.COST_CENTER_FUND = getpymItems == null ? "" : string.IsNullOrEmpty(getpymItems.FUND_CODE) ? getpymItems.COST_CENTER : string.Concat(getpymItems.COST_CENTER, "/", getpymItems.FUND_CODE);

                        //string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);

                        var getaccruedItemSubList = accruedItemSubList.Where(m => m.ACCRUED_ITEM_ID == item.ACCRUED_ITEM_ID).ToList();
                        foreach (var sub in getaccruedItemSubList)
                        {
                            AccruedDetailSubViewModel accruedItemSubViewModel = new AccruedDetailSubViewModel();
                            MVMMappingService.MoveData(sub, accruedItemSubViewModel);
                            accruedItemViewModel.AccruedItemSubList.Add(accruedItemSubViewModel);
                        }
                        var jsonSerialiser = new JavaScriptSerializer();
                        string AccruedItemJSON = jsonSerialiser.Serialize(accruedItemViewModel);

                        accruedItemViewModel.AccruedJSON = AccruedItemJSON;
                        accruedViewModel.AccruedItemList.Add(accruedItemViewModel);


                    }
                }

            }
            catch (Exception ex)
            {

            }

            return accruedViewModel;
        }
        public AccruedViewModel InitialListSearch()
        {
            AccruedViewModel model = new AccruedViewModel();
            try
            {

                model.PERIOD_MONTH = DateTime.Today.Month;
                model.PERIOD_YEAR = DateTime.Today.Year;
                model.COMPANY_CODE = "";

                var list = new List<SelectListItem>();
                using (var context = new PYMFEEEntities())
                {
                    var getCompanyList = (from data in context.COMPANies where data.IsPaymentFee == true orderby data.Bussiness_Unit select data).ToList();
                    list.AddRange(getCompanyList.Select(com => new SelectListItem { Value = com.BAN_COMPANY.ToString(), Text = com.COMPANY_NAME_EN.ToString() }).ToList());
                    model.CompanyLst = list;
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


        public AccruedViewModel InitialInvoicePaymentItemsList(string companyCode, int month, int year)
        {
            AccruedViewModel accruedList = new AccruedViewModel();
            var userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var getPaymentItemList = (from data in context.PAYMENT_ITEMS where data.IS_ACTIVE == true && data.COMPANY_CODE == companyCode orderby data.ID select data).ToList();
                    var getcttList = (from data in context.COST_CENTER where data.COMPANY_CODE == companyCode select data).ToList();

                    var getFeeInvList = (from data in context.FEE_INVOICE where data.COMPANY_CODE == companyCode && data.INV_MONTH == month && data.INV_YEAR == year select data).ToList();


                    #region accrued
                    accruedList.ID = 0;
                    //accruedList.ACCRUED_NO = "Acc" + companyCode + DateTime.Now.Date.ToString("ddmmyyyy");
                    accruedList.PERIOD_MONTH = month;
                    accruedList.PERIOD_YEAR = year;
                    accruedList.COMPANY_CODE = companyCode;

                    accruedList.TOTAL_AMT = 0;//wait calculate item
                    accruedList.MODIFIED_BY = userInfo.UserCode;
                    accruedList.MODIFIED_DATE = DateTime.Now;
                    accruedList.CREATE_BY = userInfo.UserCode;
                    accruedList.CREATE_DATE = DateTime.Now;
                    accruedList.APPROVED_BY = ConstantVariableService.APPROVERID;
                    accruedList.APPROVED_DATE = null;

                    //accruedList.REMARK = "";



                    accruedList.FormAction = ConstantVariableService.FormActionCreate;
                    accruedList.FormState = ConstantVariableService.FormStateCreate;
                    #endregion

                    int i = 1;
                    foreach (var item in getPaymentItemList)
                    {


                        var accDetailModel = new AccruedDetailViewModel();

                        #region item
                        //accDetailModel.ACCRUED_ITEM_ID = accruedList.ACCRUED_ID ;
                        accDetailModel.ACCRUED_ID = accruedList.ACCRUED_ID;
                        accDetailModel.INV_NO = "";//wait
                        accDetailModel.PRO_NO = "";//wait
                        accDetailModel.SEQUENCE = i;
                        accDetailModel.ACCRUED_MONTH = month;
                        accDetailModel.ACCRUED_YEAR = year;
                        accDetailModel.PERIOD_ACCRUED = month.ToString().PadLeft(2, '0') + "/" + year.ToString();
                        accDetailModel.CCT_CODE = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;

                        accDetailModel.COMPANY_CODE = companyCode;
                        accDetailModel.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;
                        accDetailModel.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                        //accDetailModel.TRANSACTIONS = 0;
                        //accDetailModel.AMOUNT = 0;
                        //accDetailModel.INV_AMOUNT = 0;
                        accDetailModel.MODIFIED_BY = "";
                        accDetailModel.MODIFIED_DATE = null;
                        accDetailModel.CREATE_BY = "";
                        accDetailModel.CREATE_DATE = DateTime.Now.Date;
                        accDetailModel.CURRENCY = "THB";
                        accDetailModel.REMARK_INVOICE = "";//wait
                        accDetailModel.ISPLAN = null;//wait
                        accDetailModel.EDITION = 0;//wait
                        accDetailModel.REMARK = "";//wait
                        #endregion

                        //accDetailModel.GLAccount = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                        //accDetailModel.CostCenter = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;

                        accruedList.AccruedItemList.Add(accDetailModel);


                        //accruedList.AccruedItem.SEQUENCE = i;
                        //accruedList.AccruedItem.PERIOD_ACCRUED = month.ToString().PadLeft(2, '0') + "/" + year.ToString();
                        //accruedList.AccruedItem.GLAccount = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                        //accruedList.AccruedItem.CostCenter = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;
                        //accruedList.AccruedItem.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                        //accruedList.AccruedItem.INV_AMOUNT = i;
                        //accruedList.AccruedItem.TRANSACTIONS = i;
                        //accruedList.AccruedItem.AMOUNT = i;
                        //accruedList.AccruedItem.CURRENCY = "THB";
                        //accruedList.AccruedItem.EDITION = 0;
                        //accruedList.AccruedItem.REMARK = "";


                        i++;


                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return accruedList;
        }
        public byte[] SubmitFormFileContent_test(int id)
        {
            byte[] filecontent = null;

            DynamicEntity MCB = new DynamicEntity("Student");
            var myclass = MCB.CreateObject(new string[3] { "ID", "Name", "Address" }, new Type[3] { typeof(int), typeof(string), typeof(string) });
            Type TP = myclass.GetType();

            foreach (PropertyInfo PI in TP.GetProperties())
            {
                Console.WriteLine(PI.Name);
            }


            var props = new Dictionary<string, Type>() {
                    { "Title", typeof(string) },
                    { "Text", typeof(string) },
                    { "Tags", typeof(string[]) }
                };

            createType("AccruedReportViewModel", props);


            //dynamic expando = new ExpandoObject();
            //expando.Name = "Brian";
            //expando.Country = "USA";
            //// Add properties dynamically to expando
            //AddProperty(expando, "Language", "English");

            //var values = new Dictionary<string, object>();
            //values.Add("Title", "Hello World!");
            //values.Add("Text", "My first post");
            //values.Add("Tags", new[] { "hello", "world" });

            //var post = new DynamicEntity(values);

            //dynamic dynPost = post;
            //var text = dynPost.Text;



            return filecontent;
        }

        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }

        static void createType(string name, IDictionary<string, Type> props)
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.Core.dll" }, "Test.Dynamic.dll", false);
            parameters.GenerateExecutable = false;

            var compileUnit = new CodeCompileUnit();
            var ns = new CodeNamespace("BBDEVSYS.ViewModels.AccruedReport");
            compileUnit.Namespaces.Add(ns);
            ns.Imports.Add(new CodeNamespaceImport("System"));

            var classType = new CodeTypeDeclaration(name);
            classType.Attributes = MemberAttributes.Public;
            ns.Types.Add(classType);

            foreach (var prop in props)
            {
                var fieldName = "_" + prop.Key;
                var field = new CodeMemberField(prop.Value, fieldName);
                classType.Members.Add(field);

                var property = new CodeMemberProperty();
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.Type = new CodeTypeReference(prop.Value);
                property.Name = prop.Key;
                property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)));
                property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName), new CodePropertySetValueReferenceExpression()));
                classType.Members.Add(property);
            }

            var results = csc.CompileAssemblyFromDom(parameters, compileUnit);
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => Console.WriteLine(error.ErrorText));
        }


        public byte[] SubmitFormFileContent(int id)//(AccruedViewModel formData)
        {
            byte[] filecontent = null;
            string compCode;
            try
            {
                DataSet ds = new DataSet();
                AccruedViewModel formData = new AccruedViewModel();

                int yearStart, yearEnd, monthStart, monthEnd;
                using (var context = new PYMFEEEntities())
                {

                    DataTable _dt = new DataTable();
                    formData = GetDetail(id);
                    compCode = formData.COMPANY_CODE;
                    yearStart = formData.AccruedItemList.Min(m => m.INV_YEAR);
                    yearEnd = formData.AccruedItemList.Max(m => m.INV_YEAR);
                    monthStart = formData.AccruedItemList.Where(m => m.INV_YEAR == yearStart).Min(m => m.INV_MONTH);
                    monthEnd = formData.AccruedItemList.Where(m => m.INV_YEAR == yearEnd).Max(m => m.INV_MONTH);
                    #region Accrued Report Summary



                    #region mapping data report summary
                    List<AccruedSummaryReportViewModel> modelSummaryReportList = new List<AccruedSummaryReportViewModel>();
                    int seq = 1;
                    foreach (var item in formData.AccruedItemList)
                    {
                        AccruedSummaryReportViewModel modelSummaryReport = new AccruedSummaryReportViewModel();

                        MVMMappingService.MoveData(item, modelSummaryReport);
                        modelSummaryReport.ItemNo = seq;
                        modelSummaryReport.PAYMENT_ITEMS_NAME = string.Concat(item.PAYMENT_ITEMS_NAME, " ", item.PERIOD_ACCRUED);
                        modelSummaryReportList.Add(modelSummaryReport);
                        seq++;
                    }
                    #endregion

                    _dt = ReportService.ToDataTable(modelSummaryReportList);
                    _dt.TableName = "Summary";
                    //ds = new DataSet();
                    ds.Tables.Add(_dt);
                    #endregion

                    #region Accrued Report Detail

                    var service = new AccruedSummaryReportService();
                    List<AccruedReportViewModel> _data = GetAccruedReportList_Summary(compCode, monthStart, yearStart, monthEnd, yearEnd);

                    if (_data.Any())
                    {
                        _dt = new DataTable();
                        _dt = ReportService.ToDataTableObject(_data);
                        _dt.TableName = "Detail";
                        ds.Tables.Add(_dt);
                    }

                    #endregion

                } //End  
                if (ds.Tables.Count > 0)
                {
                    // filecontent = ExcelExportHelper.AccruedExportExcel(ds, "  Accrued Expense " + Convert.ToString(formData.PERIOD_YEAR).Substring(2, 2), false, formData);

                    filecontent = ExcelExportHelper.AccruedExportExcel(ds, "  Accrued Expense " + Convert.ToString(yearEnd).Substring(2, 2), false, formData);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return filecontent;
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
                                if (arrMonthTrxn.ToList().All(m => m == 0))
                                { break; }


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
                                        //--add 4 mar 2019 total grand
                                        arrMonthGrnadTotal[iCharge] += (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);

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
                            modelTotal.CHARGE = "Total Fee";
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
                        modelGrand.CHARGE = "Grand Total Fee";
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

        public List<AccruedReportViewModel> GetAccruedReportList_Summary_waitdynamic_column(string companyCode, int monthS, int yearS, int monthE, int yearE, string chnn = "ALL", int fee = 1, string bu = "ALL")
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

                    var feeItemList = (from m in context.PAYMENT_ITEMS_CHAGE
                                       orderby m.SEQUENCE
                                       select m).ToList();

                    //var entFeeInv = (from m in context.FEE_INVOICE
                    //                 where m.INV_MONTH >= monthS && m.INV_MONTH <= monthE && m.INV_YEAR >= yearS && m.INV_YEAR <= yearE
                    //                 orderby m.INV_MONTH, m.INV_YEAR
                    //                 select m).ToList();
                    //var entFeeAccr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                    //                  where m.INV_MONTH >= monthS && m.INV_MONTH <= monthE && m.INV_YEAR >= yearS && m.INV_YEAR <= yearE
                    //                  orderby m.ACCRUED_MONTH, m.ACCRUED_YEAR
                    //                  select m).ToList();
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

                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var get_month = (yearE * 12 + monthE) - (yearS * 12 + monthS);
                    var _diffmonths = get_month + 1;
                    int currentMonth = DateTime.Now.Date.Month;
                    decimal[] arrMonthGrandTrxn = new decimal[_diffmonths];
                    decimal[] arrMonthGrnadTotal = new decimal[_diffmonths];

                    #region Detail
                    foreach (var item in feeList)
                    {

                        decimal[] arrMonthTotalTrxn = new decimal[_diffmonths];
                        decimal[] arrMonthTotal = new decimal[_diffmonths];
                        int rowfirst = 0;

                        bool chkTotalTrx = false;
                        var feeInvList = entFeeInv.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var feeAccrList = entFeeAccr.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();
                        //foreach (var item_fee in entFeeInv.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).ToList())
                        //{
                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)
                                                 orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                 select n).ToList();

                        var get_entFeeAccrItem = (from n in entFeeAccrItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)
                                                  orderby n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending, n.SEQUENCE
                                                  select n).ToList();
                        ////group data charge list
                        //var group_charge = get_entFeeAccrItem.GroupBy(m => m.PAYMENT_ITEMS_FEE_ITEM).ToList();
                        var group_charge = (from m in feeItemList where m.PAYMENT_ITEMS_ID == item.ID orderby m.CHARGE_TYPE descending select m).ToList();
                        //var group_Trxncharge = get_entFeeAccrItem.Where(m => (m.TRANSACTIONS ?? 0) != 0 && (m.RATE_TRANS ?? 0) != 0).GroupBy(m => m.PAYMENT_ITEMS_FEE_ITEM).ToList();
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

                            //foreach (var item_chrge in data)
                            //{
                            //    if (data_charge.CHARGE_TYPE == "TRXN")
                            //    {
                            //        arrMonthTrxn[(item_chrge.INV_MONTH ?? 0) - 1] = (item_chrge.TRANSACTIONS ?? 0);
                            //        arrMonthTotalTrxn[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TRANSACTIONS ?? 0);
                            //        arrMonthGrandTrxn[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TRANSACTIONS ?? 0);
                            //    }
                            //    if (data_charge.CHARGE_TYPE == "MDR")
                            //    {
                            //        chkTotalTrx = false;
                            //        arrMonthAMT[(item_chrge.INV_MONTH ?? 0) - 1] = (item_chrge.ACTUAL_AMOUNT ?? 0);
                            //    }

                            //}//charge

                            #region value before calculate avg  Transaction and Amount
                            //charge 
                            int mS = monthS;
                            int mE = monthE;
                            int yS = yearS;
                            int yE = yearE;
                            int mth = 0;
                            int yr = yS;
                            for (int n = 1; n <= _diffmonths; n++)
                            {
                                mth = n;
                                if (n == 13)
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
                            }
                            #endregion

                            #region Trxn
                            int iTrxn = 0;

                            foreach (var trx in arrMonthTrxn.ToArray())
                            {
                                if (arrMonthTrxn.ToList().All(m => m == 0))
                                { break; }


                                if (trx != 0)
                                {
                                    //Accrued
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    model.GetType().GetProperty(monthIndex).SetValue(model,
                                    Convert.ToString(string.Format("{0:#,##0.####}", trx)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => m.INV_MONTH == iTrxn + 1).FirstOrDefault();
                                    arrMonthTotalTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);
                                    arrMonthGrandTrxn[iTrxn] += (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0);
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    model.GetType().GetProperty(monthIndex).SetValue(model,
                                    Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.TRANSACTIONS ?? 0 : 0))));

                                }
                                //}

                                iTrxn++;
                            }



                            #endregion
                            #region amt
                            foreach (var amt in arrMonthAMT.ToArray())
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }

                                if (amt != 0)
                                {
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    model.GetType().GetProperty(monthIndex).SetValue(model,
                                    Convert.ToString(string.Format("{0:#,##0.####}", amt)));
                                }
                                else
                                {
                                    //Invoice
                                    var get_data_inv = data_inv.Where(m => m.INV_MONTH == iTrxn + 1).FirstOrDefault();
                                    arrMonthAMT[iTrxn] = (get_data_inv != null ? get_data_inv.ACTUAL_AMOUNT ?? 0 : 0);

                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iTrxn];

                                    model.GetType().GetProperty(monthIndex).SetValue(model,
                                    Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.ACTUAL_AMOUNT ?? 0 : 0))));



                                }
                                //}

                                iTrxn++;

                            }
                            #endregion

                            modelList.Add(model);
                            #region Total Trxn
                            if (rowfirst == group_Trxncharge.Count())
                            {
                                chkTotalTrx = true;
                            }
                            if (chkTotalTrx)
                            {
                                var inclmodelList = new List<AccruedReportViewModel>();
                                model = new AccruedReportViewModel();
                                model.CHARGE = "Total Trxn";
                                for (int i = 0; i < 12; i++)
                                {
                                    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];

                                    model.GetType().GetProperty(monthIndex).SetValue(model,
                                               Convert.ToString(string.Format("{0:#,##0.####}", arrMonthTotalTrxn[i])));
                                }
                                inclmodelList.Add(model);
                                modelList.AddRange(inclmodelList);

                            }
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
                                        : " Rate Charge (" + Convert.ToString(string.Format("{0:#,##0.####}", data.Where(m => (m.RATE_AMT ?? 0) != 0).FirstOrDefault().RATE_AMT))) + "% )";
                                }

                                foreach (var item_chrge in data)
                                {

                                    //Chrage
                                    arrMonthCharge[(item_chrge.INV_MONTH ?? 0) - 1] = (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                    //Total
                                    arrMonthTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);

                                    //Grnad Total
                                    arrMonthGrnadTotal[(item_chrge.INV_MONTH ?? 0) - 1] += (item_chrge.TOTAL_CHARGE_AMOUNT ?? 0);


                                }//charge
                                int iCharge = 0;

                                #region charge
                                foreach (var chrge in arrMonthCharge.ToArray())
                                {
                                    if (arrMonthCharge.ToList().All(m => m == 0))
                                    { break; }

                                    if (chrge != 0)
                                    {
                                        //Accrued
                                        string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                        model.GetType().GetProperty(monthIndex).SetValue(model,
                                        Convert.ToString(string.Format("{0:#,##0.####}", chrge)));
                                    }
                                    else
                                    {
                                        //Invoice
                                        var get_data_inv = data_inv.Where(m => m.INV_MONTH == iCharge + 1).FirstOrDefault();
                                        arrMonthCharge[iCharge] = (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);
                                        arrMonthTotal[iCharge] += (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0);

                                        string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                        model.GetType().GetProperty(monthIndex).SetValue(model,
                                        Convert.ToString(string.Format("{0:#,##0.####}", (get_data_inv != null ? get_data_inv.TOTAL_CHARGE_AMOUNT ?? 0 : 0))));
                                        //if (iCharge > 2)
                                        //{
                                        //    decimal avgAMT = 0;
                                        //    int avgIndexE = iCharge - 1;
                                        //    int avgIndexS = iCharge - 3;
                                        //    List<decimal> avgAMTList = new List<decimal>();
                                        //    for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //    {
                                        //        avgAMTList.Add(arrMonthCharge[a]);
                                        //    }
                                        //    avgAMT = avgAMTList.Average();
                                        //    arrMonthCharge[iCharge] = avgAMT;
                                        //    arrMonthTotal[iCharge] += avgAMT;
                                        //    arrMonthGrnadTotal[iCharge] += avgAMT;
                                        //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iCharge];

                                        //    model.GetType().GetProperty(monthIndex).SetValue(model,
                                        //    Convert.ToString(string.Format("{0:#,##0.####}", avgAMT)));
                                        //}

                                    }
                                    //}

                                    iCharge++;

                                }
                                #endregion
                                modelList.Add(model);
                            }//fee
                            #endregion

                            #region Total
                            int indexTotal = 0;
                            var modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "Total Fee";
                            var modelTotalList = new List<AccruedReportViewModel>();
                            foreach (var total in arrMonthTotal.ToArray())
                            {
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[indexTotal];

                                modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal,
                                Convert.ToString(string.Format("{0:#,##0.####}", total)));

                                indexTotal++;

                            }
                            modelTotalList.Add(modelTotal);
                            modelList.AddRange(modelTotalList);
                            #endregion

                            #region PO
                            modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "PO No.";
                            modelTotalList = new List<AccruedReportViewModel>();


                            for (int i = 1; i <= 12; i++)
                            {

                                //var chkAccrList = feeAccrList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                                //if (chkAccrList != null)
                                //{
                                //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                                //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkAccrList == null ? "" : chkAccrList.PRO_NO);
                                //}
                                //else
                                //{
                                var chkInvList = feeInvList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                                modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkInvList == null ? "" : chkInvList.PRO_NO);

                                //}

                            }
                            modelTotalList.Add(modelTotal);

                            modelList.AddRange(modelTotalList);
                            #endregion

                            #region INV
                            modelTotal = new AccruedReportViewModel();
                            modelTotal.CHARGE = "Inv No.";
                            modelTotalList = new List<AccruedReportViewModel>();
                            for (int i = 1; i <= 12; i++)
                            {
                                //var chkAccrList = feeAccrList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                                //if (chkAccrList != null)
                                //{
                                //    string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                                //    modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkAccrList == null ? "" : chkAccrList.INV_NO);
                                //}
                                //else
                                //{
                                var chkInvList = feeInvList.Where(m => m.INV_MONTH == (i)).FirstOrDefault();
                                string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i - 1];
                                modelTotal.GetType().GetProperty(monthIndex).SetValue(modelTotal, chkInvList == null ? "" : chkInvList.INV_NO);

                                //}
                            }
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
                        foreach (var grandTrxn in arrMonthGrandTrxn.ToArray())
                        {
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iGrand];

                            modelGrand.GetType().GetProperty(monthIndex).SetValue(modelGrand,
                            Convert.ToString(string.Format("{0:#,##0.####}", grandTrxn)));


                            iGrand++;

                        }
                        modelGrandList.Add(modelGrand);

                        modelList.AddRange(modelGrandList);

                        modelGrand = new AccruedReportViewModel();
                        modelGrand.CHARGE = "Grand Total";
                        modelGrandList = new List<AccruedReportViewModel>();
                        iGrand = 0;
                        foreach (var grandTotal in arrMonthGrnadTotal.ToArray())
                        {
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[iGrand];

                            modelGrand.GetType().GetProperty(monthIndex).SetValue(modelGrand,
                            Convert.ToString(string.Format("{0:#,##0.####}", grandTotal)));

                            iGrand++;

                        }
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
        public List<AccruedDetailViewModel> InitialAccruedItemsList(string companyCode, int month, int year, string formState, int accrued_id = 0)
        {
            List<AccruedDetailViewModel> accruedItemList = new List<AccruedDetailViewModel>();
            User userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    // --Payment Items Get Description

                    var payment_items = (from m in context.PAYMENT_ITEMS
                                         where m.IS_ACTIVE == true &&
                                         m.COMPANY_CODE == companyCode
                                         orderby m.GROUP_SEQ_CHANNELS
                                         select m).ToList();
                    #region mark inv & acc
                    //// --Invoice Get Invoice List
                    //var entFeeInv = (from m in context.FEE_INVOICE
                    //                 where //m.INV_MONTH <= month && m.INV_YEAR <= year
                    //                 (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                    //                 && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                    //                 && m.COMPANY_CODE == companyCode
                    //                 orderby m.INV_MONTH, m.INV_YEAR
                    //                 select m).ToList();

                    //// --Invoice Get Accrued List
                    //var entFeeAcrr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                    //                  where //m.INV_MONTH <= month && m.INV_YEAR <= year
                    //                  (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                    //                  && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                    //                  && m.COMPANY_CODE == companyCode
                    //                  orderby m.INV_MONTH, m.INV_YEAR
                    //                  select m).ToList();
                    // --Invoice Get Invoice List
                    #endregion
                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where m.COMPANY_CODE == companyCode
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();

                    // --Invoice Get Accrued List
                    var entFeeAcrr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                      where m.COMPANY_CODE == companyCode

                                      orderby m.INV_MONTH, m.INV_YEAR
                                      select m).ToList();


                    // --Invoice Get Invoice Item List
                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM where m.COMPANY_CODE == companyCode select m).ToList();

                    // --Accrued Get Accrued Item List
                    var entFeeAccruedSubItem = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB where m.COMPANY_CODE == companyCode select m).ToList();

                    //var payment_items = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE==true  select m).ToList();
                    var payment_items_charge = (from m in context.PAYMENT_ITEMS_CHAGE where m.COMPANY_CODE == companyCode select m).ToList();

                    //Payment Items Charge List
                    if (payment_items.Any())
                    {
                        payment_items_charge = payment_items_charge.Where(m => payment_items.Any(i => m.PAYMENT_ITEMS_ID == i.ID)).ToList();
                    }



                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var qtyMonth = (year * 12 + month) - (2018 * 12 + 1);

                    int currentMonth = DateTime.Now.Date.Month;
                    int months = qtyMonth + 1;
                    int years = 2018;

                    #region Detail
                    int sequence = 1;
                    foreach (var item in payment_items)
                    {
                        var feeInvList = entFeeInv.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var feeAccrList = entFeeAcrr.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        if (item.DURATION == "M")
                        {
                            feeInvList = (from m in feeInvList
                                          where (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                                            && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                                          orderby m.INV_MONTH, m.INV_YEAR
                                          select m).ToList();

                            // --Invoice Get Accrued List
                            feeAccrList = (from m in feeAccrList
                                           where (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                                              && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                                           orderby m.INV_MONTH, m.INV_YEAR
                                           select m).ToList();
                        }
                        else if (item.DURATION == "Y")
                        {
                            feeInvList = (from m in feeInvList
                                          where m.INV_YEAR >= (year - 3) &&
                                          m.INV_YEAR <= year
                                          orderby m.INV_MONTH, m.INV_YEAR
                                          select m).ToList();

                            // --Invoice Get Accrued List
                            feeAccrList = (from m in feeAccrList
                                           where m.INV_YEAR >= (year - 3) &&
                                           m.INV_YEAR <= year
                                           orderby m.INV_MONTH, m.INV_YEAR
                                           select m).ToList();
                        }


                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)//n.INV_NO == item_fee.INV_NO 

                                                 orderby n.SEQUENCE, n.INV_MONTH, n.INV_YEAR, n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending
                                                 select n).ToList();

                        var get_entFeeAcrrItem = (from n in entFeeAccruedSubItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)//n.INV_NO == item_fee.INV_NO 

                                                  orderby n.SEQUENCE, n.INV_MONTH, n.INV_YEAR, n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending
                                                  select n).ToList();
                        if (item.DURATION == "M")
                        {
                            #region Trxn + Amt
                            var data = (from m in get_entFeeInvItem
                                        select m).ToList();


                            var data_accr = (from m in get_entFeeAcrrItem
                                             select m).ToList();

                            var get_pymitem = payment_items.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).FirstOrDefault();

                            decimal[] arrMonthTrxn = new decimal[months];
                            decimal[] arrMonthAMT = new decimal[months];

                            decimal[] arrMonthRateTrxn = new decimal[months];
                            decimal[] arrMonthRateAMT = new decimal[months];
                            decimal[] arrMonthCharge = new decimal[months];

                            #region amt
                            int _month = 1;
                            int _year = years;

                            for (int i = 1; i <= months; i++)
                            {
                                if (i == 13)
                                {
                                    _month = 1;
                                    _year = _year + 1;
                                }
                                var data_assign_inv = data.Where(m => m.INV_MONTH == _month && m.INV_YEAR == _year).ToList();

                                var data_assign_accr = data_accr.Where(m => m.INV_MONTH == _month && m.INV_YEAR == _year).ToList();

                                if (data_assign_inv.Any())
                                {
                                    arrMonthTrxn[i - 1] = data_assign_inv.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[i - 1] = data_assign_inv.Sum(m => (m.ACTUAL_AMOUNT ?? 0));

                                    arrMonthCharge[i - 1] = data_assign_inv.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }
                                else if (data_assign_accr.Any())
                                {

                                    int ym_accrued = data_accr.Max(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH).Value;
                                    var getdata_accr = data_assign_accr.Where(m => (m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH == ym_accrued
                                    && m.INV_MONTH == _month && m.INV_YEAR == _year
                                    ).ToList();

                                    arrMonthTrxn[i - 1] = getdata_accr.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[i - 1] = getdata_accr.Sum(m => (m.ACTUAL_AMOUNT ?? 0));

                                    arrMonthCharge[i - 1] = getdata_accr.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }

                                _month++;
                            }

                            #region Generate Value Amount on Every Month

                            int iTrxn = 0;
                            foreach (var arr in arrMonthTrxn.ToArray())
                            {
                                if (arrMonthTrxn.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;

                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;


                                    }
                                    else
                                    {

                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthAMT.ToArray())
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;
                                    }
                                    else
                                    {


                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthCharge.ToArray())
                            {
                                if (arrMonthCharge.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {

                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;
                                    }
                                    else
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            #endregion


                            #endregion

                            //Check Data Had in Invoice
                            var chkitem_chrge = feeInvList.Where(m =>
                             //m.INV_MONTH >= 1 && m.INV_MONTH <= month && m.INV_YEAR >= 2018 && m.INV_YEAR <= year
                             (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1 && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                            ).ToList();
                            int _monthc = 1;
                            int _yearc = years;
                            for (int i = 1; i <= months; i++)
                            {
                                if (i == 13)
                                {
                                    _monthc = 1;
                                    _yearc = _yearc + 1;
                                }
                                var item_chrge = feeInvList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_MONTH == _monthc && m.INV_YEAR == _yearc).FirstOrDefault();
                                var item_acccharge = feeAccrList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.ACCRUED_ID == accrued_id && m.INV_MONTH == _monthc && m.INV_YEAR == _yearc).FirstOrDefault();
                                var model = new AccruedDetailViewModel();

                                if (item_chrge != null)

                                {

                                    if (item_chrge.IS_STATUS != "3")
                                    {
                                        if (item_chrge.IS_STATUS == "4")
                                        {
                                            _monthc++;
                                            continue;
                                        }
                                        else
                                        {
                                            #region case not complete

                                            model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                            model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                            model.PERIOD_ACCRUED = string.Concat(item_chrge.INV_MONTH.ToString().PadLeft(2, '0'), " / ", item_chrge.INV_YEAR);
                                            model.INV_MONTH = (item_chrge.INV_MONTH ?? 0);
                                            model.INV_YEAR = (item_chrge.INV_YEAR ?? 0);

                                            model.SEQUENCE = sequence;
                                            model.CHANNELS = item.CHANNELS;
                                            model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                            model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                            model.CURRENCY = "THB";
                                            model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                            model.CCT_CODE = item.CCT_CODE;
                                            model.GL_ACCOUNT = item.GL_ACCOUNT;
                                            model.COST_CENTER = item.COST_CENTER;
                                            model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                            model.ACCRUED_MONTH = month;// _monthc;
                                            model.ACCRUED_YEAR = year;// _yearc;
                                            model.COMPANY_CODE = companyCode;
                                            model.INV_NO = item_chrge.INV_NO;
                                            model.PRO_NO = item_chrge.PRO_NO;


                                            model.TRANSACTIONS = arrMonthTrxn[i - 1];
                                            model.AMOUNT = arrMonthAMT[i - 1];
                                            model.INV_AMOUNT = arrMonthCharge[i - 1];

                                            if (get_entFeeInvItem.Sum(m => (m.TRANSACTIONS ?? 0)) == 0 && get_entFeeInvItem.Sum(m => (m.ACTUAL_AMOUNT ?? 0)) == 0)
                                            {
                                                model.REMARK = "ประมาณการย้อนหลัง 3 เดือน";
                                                model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 เดือน";

                                                model.ISPLAN = true;
                                            }
                                            else
                                            {
                                                model.REMARK = "Invoice " + item_chrge.INV_MONTH.ToString().PadLeft(2, '0') + "/ " + item_chrge.INV_YEAR;
                                                model.REMARK_INVOICE = "Inv No. " + item_chrge.INV_NO;

                                                model.ISPLAN = false;

                                            }
                                            if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                            {
                                                model.CREATE_BY = userInfo.UserCode;
                                                model.CREATE_DATE = DateTime.Now;
                                            }
                                            else
                                            {
                                                model.MODIFIED_BY = userInfo.UserCode;
                                                //model.MODIFIED_DATE = DateTime.Now;
                                            }
                                            model.MODIFIED_DATE = DateTime.Now;
                                            #region sub accrued
                                            //var dataAccruedSub = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_NO == model.INV_NO ).ToList();
                                            //var acdataAccruedSub = data_accr.Where(m =>  m.INV_MONTH == model.INV_MONTH && m.INV_YEAR == model.INV_YEAR).ToList();
                                            var dataAccruedSub = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_NO == model.INV_NO).ToList();
                                            var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();


                                            var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                            foreach (var sub in dataAccruedSub)
                                            {
                                                var entAccruedItemSub = new AccruedDetailSubViewModel();

                                                var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_ITEM).FirstOrDefault();

                                                MVMMappingService.MoveData(sub, entAccruedItemSub);
                                                entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;

                                                //entAccruedItemSub.CCT_CODE = sub.COST_CENTER;
                                                entAccruedItemSub.ACCRUED_MONTH = month;// _monthc;
                                                entAccruedItemSub.ACCRUED_YEAR = year;// _yearc;
                                                entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                                entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                                entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                                entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                                entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                                entAccruedItemSub.PRO_NO = model.PRO_NO;
                                                entAccruedItemSub.REMARK = model.REMARK;
                                                entAccruedItemSubLst.Add(entAccruedItemSub);
                                            }
                                            model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                            #endregion
                                            //Get Json Model AccruedDetail
                                            var jsonSerialiser = new JavaScriptSerializer();
                                            string AccruedJSON = jsonSerialiser.Serialize(model);
                                            model.AccruedJSON = AccruedJSON;

                                            model.EDITION = 0;
                                            accruedItemList.Add(model);
                                            sequence++;
                                            #endregion
                                        }
                                    }//charge
                                }
                                else
                                {
                                    model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                    model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                    model.PERIOD_ACCRUED = string.Concat(_monthc.ToString().PadLeft(2, '0'), " / ", _yearc);
                                    model.INV_MONTH = _monthc;
                                    model.INV_YEAR = _yearc;
                                    model.SEQUENCE = sequence;
                                    model.CHANNELS = item.CHANNELS;
                                    model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                    model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                    model.CURRENCY = "THB";
                                    model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                    model.CCT_CODE = item.CCT_CODE;
                                    model.GL_ACCOUNT = item.GL_ACCOUNT;
                                    model.COST_CENTER = item.COST_CENTER;
                                    model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                    model.ACCRUED_MONTH = month;// _monthc;
                                    model.ACCRUED_YEAR = year;// _yearc;
                                    model.COMPANY_CODE = companyCode;


                                    model.TRANSACTIONS = arrMonthTrxn[i - 1];
                                    model.AMOUNT = arrMonthAMT[i - 1];
                                    model.INV_AMOUNT = arrMonthCharge[i - 1];


                                    model.REMARK = "ประมาณการย้อนหลัง 3 เดือน";
                                    model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 เดือน";

                                    model.ISPLAN = true;

                                    model.EDITION = 0;

                                    if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                    {
                                        model.CREATE_BY = userInfo.UserCode;
                                        model.CREATE_DATE = DateTime.Now;
                                    }
                                    else
                                    {
                                        model.MODIFIED_BY = userInfo.UserCode;
                                        //model.MODIFIED_DATE = DateTime.Now;
                                    }
                                    model.MODIFIED_DATE = DateTime.Now;

                                    #region sub accrued
                                    int subsequence = 1;
                                    var dataAccruedSub = payment_items_charge.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).ToList();
                                    var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();

                                    var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                    foreach (var sub in dataAccruedSub)
                                    {
                                        var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).FirstOrDefault();
                                        decimal[] arrMonthTrxnSub = new decimal[months];
                                        decimal[] arrMonthAMTSub = new decimal[months];
                                        decimal[] arrMonthChargeSub = new decimal[months];

                                        decimal[] arrMonthTrxn_RateSub = new decimal[months];
                                        decimal[] arrMonthAMT_RateSub = new decimal[months];


                                        #region amt
                                        var dataSubList = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).OrderBy(m => m.ID).ToList();
                                        int _mnth = 1;
                                        int _yr = years;

                                        for (int r = 1; r <= months; r++)
                                        {
                                            if (r == 13)
                                            {
                                                _mnth = 1;
                                                _yr = _yr + 1;
                                            }
                                            var dataSub = dataSubList.Where(h => (h.INV_YEAR * 12) + h.INV_MONTH == (_yr * 12) + _mnth).FirstOrDefault();

                                            //var e = dataSubList.Where(h => (h.INV_YEAR * 12) + h.INV_MONTH == (_yr * 12) + _mnth).ToList();
                                            if (dataSub != null)
                                            {
                                                arrMonthTrxnSub[r - 1] = (dataSub.TRANSACTIONS ?? 0);
                                                arrMonthAMTSub[r - 1] = (dataSub.ACTUAL_AMOUNT ?? 0);
                                                arrMonthChargeSub[r - 1] = (dataSub.TOTAL_CHARGE_AMOUNT ?? 0);

                                                arrMonthTrxn_RateSub[r - 1] = (dataSub.RATE_TRANS ?? 0);
                                                arrMonthAMT_RateSub[r - 1] = (dataSub.RATE_AMT ?? 0);

                                            }
                                            else
                                            {
                                                //if (r == i)
                                                //{ }
                                                //check must not exists had Payment and had not Sub Detail
                                                var existInv = feeInvList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE
                                                && (m.INV_YEAR * 12) + m.INV_MONTH == (_yr * 12) + _mnth
                                                && m.IS_STATUS != "3").OrderBy(m => m.ID).ToList();



                                                #region check must not exists had Payment and had not Sub Detail
                                                var get_dataSubList = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE
                                                && (m.INV_YEAR * 12) + m.INV_MONTH == (_yr * 12) + _mnth).OrderBy(m => m.ID).ToList();
                                                if (!get_dataSubList.Any())
                                                {
                                                    #region sub cal trans & amt & rate & net


                                                    int iTrxnSub = r - 1;

                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxnSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxnSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxnSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxnSub[iTrxnSub] = avgVal;

                                                    }

                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMTSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMTSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMTSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMTSub[iTrxnSub] = avgVal;
                                                    }



                                                    #region avg rate

                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxn_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxn_RateSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxn_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxn_RateSub[iTrxnSub] = avgVal;
                                                    }

                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMT_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMT_RateSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMT_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMT_RateSub[iTrxnSub] = avgVal;

                                                    }

                                                    #endregion


                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthChargeSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthChargeSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthChargeSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthChargeSub[iTrxnSub] = avgVal;

                                                    }


                                                    #endregion
                                                }
                                                else
                                                {
                                                    if (existInv.Any())
                                                    {

                                                        #region cal avg 3 month sub tran & amt & rate & net
                                                        int _mnthS = _mnth - 3;
                                                        _mnthS = _mnthS < 0 ? 12 + _mnthS : _mnthS;
                                                        int _mnthE = _mnth - 1;
                                                        _mnthE = _mnthE < 0 ? 12 + _mnthE : _mnthE;
                                                        int _yrS = years;
                                                        int _yrE = _yr;
                                                        if (_mnthS == 0)
                                                        {
                                                            _mnthS = 12;
                                                            _yrS = _yrS - 1;
                                                        }
                                                        if (_mnthE == 0)
                                                        {
                                                            _mnthE = 12;
                                                            _yrE = _yrE - 1;
                                                        }

                                                        List<decimal> valueTrxnSubAvg = new List<decimal>();
                                                        List<decimal> valueAMTSubAvg = new List<decimal>();
                                                        List<decimal> valueChargeSubAvg = new List<decimal>();
                                                        List<decimal> valueTrxn_RateSubAvg = new List<decimal>();
                                                        List<decimal> valueAMT_RateSubAvg = new List<decimal>();

                                                        int y = _yrS;
                                                        int m = _mnthS;
                                                        int loop = 0;
                                                        while (loop < 3)
                                                        {
                                                            if (m == 13)
                                                            {
                                                                m = 1;
                                                                y = y + 1;
                                                            }
                                                            var _dataSub = dataSubList.Where(x => (x.INV_YEAR * 12) + x.INV_MONTH == (y * 12) + m).FirstOrDefault();
                                                            if (_dataSub != null)
                                                            {
                                                                valueTrxnSubAvg.Add(_dataSub.TRANSACTIONS ?? 0);
                                                                valueAMTSubAvg.Add(_dataSub.ACTUAL_AMOUNT ?? 0);
                                                                valueChargeSubAvg.Add(_dataSub.TOTAL_CHARGE_AMOUNT ?? 0);
                                                                valueTrxn_RateSubAvg.Add(_dataSub.RATE_TRANS ?? 0);
                                                                valueAMT_RateSubAvg.Add(_dataSub.RATE_AMT ?? 0);
                                                            }
                                                            else
                                                            {
                                                                valueTrxnSubAvg.Add(0);
                                                                valueAMTSubAvg.Add(0);
                                                                valueChargeSubAvg.Add(0);
                                                                valueTrxn_RateSubAvg.Add(0);
                                                                valueAMT_RateSubAvg.Add(0);
                                                            }
                                                            m++;
                                                            loop++;
                                                        }
                                                        arrMonthTrxnSub[r - 1] = (valueTrxnSubAvg.Average());
                                                        arrMonthAMTSub[r - 1] = (valueAMTSubAvg.Average());
                                                        arrMonthChargeSub[r - 1] = (valueChargeSubAvg.Average());

                                                        arrMonthTrxn_RateSub[r - 1] = (valueTrxn_RateSubAvg.Average());
                                                        arrMonthAMT_RateSub[r - 1] = (valueAMT_RateSubAvg.Average());
                                                        //var _dataSub = dataSubList.Where(h =>
                                                        //(h.INV_YEAR * 12) + h.INV_MONTH >= (_yrS * 12) + _mnthS &&
                                                        //(h.INV_YEAR * 12) + h.INV_MONTH <= (_yrE * 12) + _mnthE).ToList();
                                                        //if (_dataSub.Any())
                                                        //{
                                                        //    arrMonthTrxnSub[r - 1] = (_dataSub.Average(g => g.TRANSACTIONS ?? 0));
                                                        //    arrMonthAMTSub[r - 1] = (_dataSub.Average(g => g.ACTUAL_AMOUNT ?? 0));
                                                        //    arrMonthChargeSub[r - 1] = (_dataSub.Average(g => g.TOTAL_CHARGE_AMOUNT ?? 0));

                                                        //    arrMonthTrxn_RateSub[r - 1] = (_dataSub.Average(g => g.RATE_TRANS ?? 0));
                                                        //    arrMonthAMT_RateSub[r - 1] = (_dataSub.Average(g => g.RATE_AMT ?? 0));
                                                        //}

                                                        #endregion
                                                    }
                                                }
                                                #endregion


                                            }
                                            _mnth++;
                                        }

                                        #region sub cal trans & amt & rate & net mark
                                        //foreach (var dataSub in dataSubList)
                                        //{
                                        //    arrMonthTrxnSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TRANSACTIONS ?? 0);
                                        //    arrMonthAMTSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.ACTUAL_AMOUNT ?? 0);
                                        //    arrMonthChargeSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TOTAL_CHARGE_AMOUNT ?? 0);

                                        //    arrMonthTrxn_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_TRANS ?? 0);
                                        //    arrMonthAMT_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_AMT ?? 0);
                                        //}

                                        //int iTrxnSub = 0;

                                        //foreach (var arr in arrMonthTrxnSub.ToArray())
                                        //{
                                        //    if (arrMonthTrxnSub.ToList().All(m => m == 0))
                                        //    { break; }
                                        //    if (arr == 0)
                                        //    {
                                        //        if (iTrxnSub > 2)
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub - 1;
                                        //            int avgIndexS = iTrxnSub - 3;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthTrxnSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthTrxnSub[iTrxnSub] = avgVal;
                                        //        }
                                        //        else
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                        //            int avgIndexS = 0;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthTrxnSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthTrxnSub[iTrxnSub] = avgVal;

                                        //        }
                                        //    }
                                        //    iTrxnSub++;
                                        //}
                                        //iTrxnSub = 0;
                                        //foreach (var arr in arrMonthAMTSub.ToArray())
                                        //{
                                        //    if (arrMonthAMTSub.ToList().All(m => m == 0))
                                        //    { break; }
                                        //    if (arr == 0)
                                        //    {
                                        //        if (iTrxnSub > 2)
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub - 1;
                                        //            int avgIndexS = iTrxnSub - 3;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthAMTSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthAMTSub[iTrxnSub] = avgVal;
                                        //        }
                                        //        else
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                        //            int avgIndexS = 0;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthAMTSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthAMTSub[iTrxnSub] = avgVal;

                                        //        }
                                        //    }
                                        //    iTrxnSub++;
                                        //}


                                        //#region avg rate

                                        //iTrxnSub = 0;
                                        //foreach (var arr in arrMonthTrxn_RateSub.ToArray())
                                        //{
                                        //    if (arrMonthTrxn_RateSub.ToList().All(m => m == 0))
                                        //    { break; }
                                        //    if (arr == 0)
                                        //    {
                                        //        if (iTrxnSub > 2)
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub - 1;
                                        //            int avgIndexS = iTrxnSub - 3;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthTrxn_RateSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthTrxn_RateSub[iTrxnSub] = avgVal;
                                        //        }
                                        //        else
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                        //            int avgIndexS = 0;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthTrxn_RateSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthTrxn_RateSub[iTrxnSub] = avgVal;
                                        //        }

                                        //    }
                                        //    iTrxnSub++;
                                        //}
                                        //iTrxnSub = 0;
                                        //foreach (var arr in arrMonthAMT_RateSub.ToArray())
                                        //{
                                        //    if (arrMonthAMT_RateSub.ToList().All(m => m == 0))
                                        //    { break; }
                                        //    if (arr == 0)
                                        //    {
                                        //        if (iTrxnSub > 2)
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub - 1;
                                        //            int avgIndexS = iTrxnSub - 3;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthAMT_RateSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthAMT_RateSub[iTrxnSub] = avgVal;
                                        //        }
                                        //        else
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                        //            int avgIndexS = 0;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthAMT_RateSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthAMT_RateSub[iTrxnSub] = avgVal;

                                        //        }
                                        //    }
                                        //    iTrxnSub++;
                                        //}

                                        //#endregion

                                        //iTrxnSub = 0;
                                        //foreach (var arr in arrMonthChargeSub.ToArray())
                                        //{
                                        //    if (arrMonthChargeSub.ToList().All(m => m == 0))
                                        //    { break; }
                                        //    if (arr == 0)
                                        //    {
                                        //        if (iTrxnSub > 2)
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub - 1;
                                        //            int avgIndexS = iTrxnSub - 3;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthChargeSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthChargeSub[iTrxnSub] = avgVal;
                                        //        }
                                        //        else
                                        //        {
                                        //            decimal avgVal = 0;
                                        //            int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                        //            int avgIndexS = 0;
                                        //            List<decimal> avgList = new List<decimal>();
                                        //            for (int a = avgIndexS; a <= avgIndexE; a++)
                                        //            {
                                        //                avgList.Add(arrMonthChargeSub[a]);
                                        //            }
                                        //            avgVal = avgList.Average();
                                        //            arrMonthChargeSub[iTrxnSub] = avgVal;

                                        //        }
                                        //    }
                                        //    iTrxnSub++;
                                        //}



                                        #endregion

                                        #endregion


                                        var entAccruedItemSub = new AccruedDetailSubViewModel();
                                        MVMMappingService.MoveData(model, entAccruedItemSub);
                                        entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;
                                        entAccruedItemSub.ACCRUED_MONTH = month;// _monthc;
                                        entAccruedItemSub.ACCRUED_YEAR = year;// _yearc;
                                        entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                        entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                        entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                        entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                        entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                        entAccruedItemSub.PRO_NO = model.PRO_NO;
                                        entAccruedItemSub.INV_NO = model.INV_NO;
                                        entAccruedItemSub.INV_MONTH = _monthc;
                                        entAccruedItemSub.INV_YEAR = _yearc;
                                        entAccruedItemSub.SEQUENCE = subsequence;
                                        entAccruedItemSub.PAYMENT_ITEMS_FEE_ITEM = sub.PAYMENT_ITEMS_FEE_NAME;
                                        entAccruedItemSub.TRANSACTIONS = arrMonthTrxnSub[i - 1];
                                        entAccruedItemSub.ACTUAL_AMOUNT = arrMonthAMTSub[i - 1];
                                        entAccruedItemSub.TOTAL_CHARGE_AMOUNT = arrMonthChargeSub[i - 1];
                                        entAccruedItemSub.RATE_TRANS = arrMonthTrxn_RateSub[i - 1];
                                        entAccruedItemSub.RATE_AMT = arrMonthAMT_RateSub[i - 1];
                                        entAccruedItemSubLst.Add(entAccruedItemSub);

                                        subsequence++;
                                    }
                                    model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                    #endregion
                                    //Get Json Model AccruedDetail
                                    var jsonSerialiser = new JavaScriptSerializer();
                                    string AccruedJSON = jsonSerialiser.Serialize(model);
                                    model.AccruedJSON = AccruedJSON;


                                    accruedItemList.Add(model);
                                    sequence++;
                                }

                                _monthc++; // keep value month

                            }//end for

                            #endregion
                        }
                        else if (item.DURATION == "Y")
                        {
                            #region Trxn + Amt Duration of Year
                            var data = (from m in get_entFeeInvItem
                                        select m).ToList();

                            var data_accr = (from m in get_entFeeAcrrItem
                                             select m).ToList();

                            var get_pymitem = payment_items.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).FirstOrDefault();

                            var getmonths = (year - 2018) + 1;

                            decimal[] arrMonthTrxn = new decimal[getmonths];
                            decimal[] arrMonthAMT = new decimal[getmonths];
                            decimal[] arrMonthCharge = new decimal[getmonths];

                            int _month = 1;
                            #region amt


                            int yearS = (year - 3) > 2018 ? (year - 3) : 2018;

                            int _year = yearS;
                            int loop = 0;
                            for (int i = yearS; i <= year; i++)
                            {
                                var data_assign_inv = data.Where(m => m.INV_YEAR == i).ToList();
                                var data_assign_accr = data_accr.Where(m => m.INV_YEAR == i).ToList();
                                if (data_assign_inv.Any())
                                {
                                    arrMonthTrxn[loop] = data_assign_inv.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[loop] = data_assign_inv.Sum(m => (m.ACTUAL_AMOUNT ?? 0));
                                    arrMonthCharge[loop] = data_assign_inv.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }
                                else if (data_assign_accr.Any())
                                {
                                    arrMonthTrxn[loop] = data_assign_accr.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[loop] = data_assign_accr.Sum(m => (m.ACTUAL_AMOUNT ?? 0));
                                    arrMonthCharge[loop] = data_assign_accr.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }

                                loop++;
                            }

                            #region Generate Value Amount on Every Month

                            int iTrxn = 0;
                            foreach (var arr in arrMonthTrxn.ToArray())
                            {
                                if (arrMonthTrxn.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;
                                    }
                                    else
                                    {

                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthAMT.ToArray())
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;
                                    }
                                    else
                                    {


                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthCharge.ToArray())
                            {
                                if (arrMonthCharge.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;
                                    }
                                    else
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            #endregion


                            #endregion


                            int _yearc = getmonths;
                            int _yearloop = 1;
                            for (int i = 2018; i <= year; i++)
                            {

                                var item_chrgeofYear = feeInvList.Where(m => m.INV_YEAR == i).ToList();

                                var model = new AccruedDetailViewModel();
                                for (int j = 1; j <= 12; j++)
                                {
                                    var item_chrge = item_chrgeofYear.Where(m => m.INV_MONTH == j).FirstOrDefault();
                                    var item_acccharge = feeAccrList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.ACCRUED_ID == accrued_id && m.INV_MONTH == j && m.INV_YEAR == i).FirstOrDefault();

                                    if (item_chrge != null)
                                    {

                                        if (item_chrge.IS_STATUS != "3")
                                        {
                                            if (item_chrge.IS_STATUS == "4")
                                            {
                                                _yearc++;
                                                j++;
                                                continue;
                                            }
                                            else
                                            {
                                                #region case not complete
                                                model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                                model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                                model.PERIOD_ACCRUED = Convert.ToString(item_chrge.INV_YEAR);
                                                model.INV_MONTH = (item_chrge.INV_MONTH ?? 0);
                                                model.INV_YEAR = (item_chrge.INV_YEAR ?? 0);

                                                model.SEQUENCE = sequence;
                                                model.CHANNELS = item.CHANNELS;
                                                model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                                model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                                model.CURRENCY = "THB";
                                                model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                                model.CCT_CODE = item.CCT_CODE;
                                                model.GL_ACCOUNT = item.GL_ACCOUNT;
                                                model.COST_CENTER = item.COST_CENTER;
                                                model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                                model.ACCRUED_MONTH = month;// generate accrued advanced
                                                model.ACCRUED_YEAR = year;
                                                model.COMPANY_CODE = companyCode;
                                                model.INV_NO = item_chrge.INV_NO;
                                                model.PRO_NO = item_chrge.PRO_NO;


                                                model.TRANSACTIONS = arrMonthTrxn[_yearloop - 1];
                                                model.AMOUNT = arrMonthAMT[_yearloop - 1];
                                                model.INV_AMOUNT = arrMonthCharge[_yearloop - 1];

                                                if (get_entFeeInvItem.Sum(m => (m.TRANSACTIONS ?? 0)) == 0 && get_entFeeInvItem.Sum(m => (m.ACTUAL_AMOUNT ?? 0)) == 0)
                                                {
                                                    model.REMARK = "ประมาณการย้อนหลัง 3 ปี";
                                                    model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 ปี";

                                                    model.ISPLAN = true;
                                                }
                                                else
                                                {
                                                    model.REMARK = "Invoice " + item_chrge.INV_YEAR;
                                                    model.REMARK_INVOICE = "Inv No. " + item_chrge.INV_NO;

                                                    model.ISPLAN = false;

                                                }
                                                if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    model.CREATE_BY = userInfo.UserCode;
                                                    model.CREATE_DATE = DateTime.Now;
                                                }
                                                else
                                                {
                                                    model.MODIFIED_BY = userInfo.UserCode;
                                                    //model.MODIFIED_DATE = DateTime.Now;
                                                }
                                                model.MODIFIED_DATE = DateTime.Now;
                                                #region sub accrued
                                                var dataAccruedSub = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_NO == model.INV_NO).ToList();
                                                var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();

                                                var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                                foreach (var sub in dataAccruedSub)
                                                {
                                                    var entAccruedItemSub = new AccruedDetailSubViewModel();

                                                    var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_ITEM).FirstOrDefault();

                                                    MVMMappingService.MoveData(sub, entAccruedItemSub);
                                                    entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;
                                                    //MVMMappingService.MoveData(sub, entAccruedItemSub);
                                                    //entAccruedItemSub.ID = 0;
                                                    //entAccruedItemSub.CCT_CODE = sub.COST_CENTER;
                                                    entAccruedItemSub.ACCRUED_MONTH = month;
                                                    entAccruedItemSub.ACCRUED_YEAR = year;
                                                    entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                                    entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                                    entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                                    entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                                    entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                                    entAccruedItemSub.PRO_NO = model.PRO_NO;
                                                    entAccruedItemSub.REMARK = model.REMARK;
                                                    entAccruedItemSubLst.Add(entAccruedItemSub);
                                                }
                                                model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                                #endregion
                                                //Get Json Model AccruedDetail
                                                var jsonSerialiser = new JavaScriptSerializer();
                                                string AccruedJSON = jsonSerialiser.Serialize(model);
                                                model.AccruedJSON = AccruedJSON;

                                                model.EDITION = 0;
                                                accruedItemList.Add(model);
                                                sequence++;
                                                #endregion
                                            }
                                        }//charge

                                        break;
                                    }
                                    else
                                    {
                                        model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                        model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                        model.PERIOD_ACCRUED = Convert.ToString(i);
                                        model.INV_MONTH = j;
                                        model.INV_YEAR = i;
                                        model.SEQUENCE = sequence;
                                        model.CHANNELS = item.CHANNELS;
                                        model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                        model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                        model.CURRENCY = "THB";
                                        model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                        model.CCT_CODE = item.CCT_CODE;
                                        model.GL_ACCOUNT = item.GL_ACCOUNT;
                                        model.COST_CENTER = item.COST_CENTER;
                                        model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                        model.ACCRUED_MONTH = month;
                                        model.ACCRUED_YEAR = year;
                                        model.COMPANY_CODE = companyCode;


                                        model.TRANSACTIONS = arrMonthTrxn[_yearloop - 1];
                                        model.AMOUNT = arrMonthAMT[_yearloop - 1];
                                        model.INV_AMOUNT = arrMonthCharge[_yearloop - 1];


                                        model.REMARK = "ประมาณการย้อนหลัง 3 ปี";
                                        model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 ปี";

                                        model.ISPLAN = true;

                                        model.EDITION = 0;

                                        if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                        {
                                            model.CREATE_BY = userInfo.UserCode;
                                            model.CREATE_DATE = DateTime.Now;
                                        }
                                        else
                                        {
                                            model.MODIFIED_BY = userInfo.UserCode;
                                            //model.MODIFIED_DATE = DateTime.Now;
                                        }
                                        model.MODIFIED_DATE = DateTime.Now;
                                        #region sub accrued
                                        int subsequence = 1;
                                        var dataAccruedSub = payment_items_charge.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).ToList();

                                        var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();

                                        var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                        foreach (var sub in dataAccruedSub)
                                        {

                                            var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).FirstOrDefault();


                                            decimal[] arrMonthTrxnSub = new decimal[months];
                                            decimal[] arrMonthAMTSub = new decimal[months];
                                            decimal[] arrMonthChargeSub = new decimal[months];

                                            decimal[] arrMonthTrxn_RateSub = new decimal[months];
                                            decimal[] arrMonthAMT_RateSub = new decimal[months];

                                            #region cal sub accrued mark
                                            //var dataSubList = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).OrderBy(m => m.ID).ToList();
                                            //int _mnth = 1;
                                            //int _yr = years;

                                            //for (int r = 1; r <= months; r++)
                                            //{
                                            //    if (r == 13)
                                            //    {
                                            //        _mnth = 1;
                                            //        _yr = _yr + 1;
                                            //    }
                                            //    var dataSub = dataSubList.Where(h => (h.INV_YEAR * 12) + h.INV_MONTH == (_yr * 12) + _mnth).FirstOrDefault();

                                            //    var e = dataSubList.Where(h => (h.INV_YEAR * 12) + h.INV_MONTH == (_yr * 12) + _mnth).ToList();
                                            //    if (dataSub != null)
                                            //    {
                                            //        arrMonthTrxnSub[r - 1] = (dataSub.TRANSACTIONS ?? 0);
                                            //        arrMonthAMTSub[r - 1] = (dataSub.ACTUAL_AMOUNT ?? 0);
                                            //        arrMonthChargeSub[r - 1] = (dataSub.TOTAL_CHARGE_AMOUNT ?? 0);

                                            //        arrMonthTrxn_RateSub[r - 1] = (dataSub.RATE_TRANS ?? 0);
                                            //        arrMonthAMT_RateSub[r - 1] = (dataSub.RATE_AMT ?? 0);

                                            //    }
                                            //    else
                                            //    {
                                            //        //check must not exists had Payment and had not Sub Detail
                                            //        #region check must not exists had Payment and had not Sub Detail
                                            //        var get_dataSubList = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE
                                            //        && (m.INV_YEAR * 12) + m.INV_MONTH == (_yr * 12) + _mnth).OrderBy(m => m.ID).ToList();
                                            //        if (!get_dataSubList.Any())
                                            //        {
                                            //            int _mnthS = _mnth - 3;
                                            //            _mnthS = _mnthS < 0 ? 12 + _mnthS : _mnthS;
                                            //            int _mnthE = _mnth - 1;
                                            //            _mnthE = _mnthE < 0 ? 12 + _mnthE : _mnthE;
                                            //            int _yrS = years;
                                            //            int _yrE = _yr;
                                            //            if (_mnthS == 0)
                                            //            {
                                            //                _mnthS = 12;
                                            //                _yrS = _yrS - 1;
                                            //            }
                                            //            if (_mnthE == 0)
                                            //            {
                                            //                _mnthE = 12;
                                            //                _yrE = _yrE - 1;
                                            //            }

                                            //            List<decimal> valueTrxnSubAvg = new List<decimal>();
                                            //            List<decimal> valueAMTSubAvg = new List<decimal>();
                                            //            List<decimal> valueChargeSubAvg = new List<decimal>();
                                            //            List<decimal> valueTrxn_RateSubAvg = new List<decimal>();
                                            //            List<decimal> valueAMT_RateSubAvg = new List<decimal>();

                                            //            int y = _yrS;
                                            //            int m = _mnthS;
                                            //            int yloop = 0;
                                            //            while (yloop < 3)
                                            //            {
                                            //                if (m == 13)
                                            //                {
                                            //                    m = 1;
                                            //                    y = y + 1;
                                            //                }
                                            //                var _dataSub = dataSubList.Where(x => (x.INV_YEAR * 12) + x.INV_MONTH == (y * 12) + m).FirstOrDefault();
                                            //                if (_dataSub != null)
                                            //                {
                                            //                    valueTrxnSubAvg.Add(_dataSub.TRANSACTIONS ?? 0);
                                            //                    valueAMTSubAvg.Add(_dataSub.ACTUAL_AMOUNT ?? 0);
                                            //                    valueChargeSubAvg.Add(_dataSub.TOTAL_CHARGE_AMOUNT ?? 0);
                                            //                    valueTrxn_RateSubAvg.Add(_dataSub.RATE_TRANS ?? 0);
                                            //                    valueAMT_RateSubAvg.Add(_dataSub.RATE_AMT ?? 0);
                                            //                }
                                            //                else
                                            //                {
                                            //                    valueTrxnSubAvg.Add(0);
                                            //                    valueAMTSubAvg.Add(0);
                                            //                    valueChargeSubAvg.Add(0);
                                            //                    valueTrxn_RateSubAvg.Add(0);
                                            //                    valueAMT_RateSubAvg.Add(0);
                                            //                }
                                            //                m++;
                                            //                yloop++;
                                            //            }
                                            //            arrMonthTrxnSub[r - 1] = (valueTrxnSubAvg.Average());
                                            //            arrMonthAMTSub[r - 1] = (valueAMTSubAvg.Average());
                                            //            arrMonthChargeSub[r - 1] = (valueChargeSubAvg.Average());

                                            //            arrMonthTrxn_RateSub[r - 1] = (valueTrxn_RateSubAvg.Average());
                                            //            arrMonthAMT_RateSub[r - 1] = (valueAMT_RateSubAvg.Average());

                                            //        }
                                            //        #endregion
                                            //    }
                                            //    _mnth++;
                                            //}
                                            #endregion


                                            //-------------------wrong
                                            #region amt
                                            foreach (var dataSub in data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).OrderBy(m => m.ID).ToList())
                                            {
                                                arrMonthTrxnSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TRANSACTIONS ?? 0);
                                                arrMonthAMTSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.ACTUAL_AMOUNT ?? 0);
                                                arrMonthChargeSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TOTAL_CHARGE_AMOUNT ?? 0);

                                                arrMonthTrxn_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_TRANS ?? 0);
                                                arrMonthAMT_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_AMT ?? 0);
                                            }

                                            int iTrxnSub = 0;
                                            foreach (var arr in arrMonthTrxnSub.ToArray())
                                            {
                                                if (arrMonthTrxnSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxnSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxnSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxnSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxnSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }
                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthAMTSub.ToArray())
                                            {
                                                if (arrMonthAMTSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMTSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMTSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMTSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMTSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }


                                            #region avg rate

                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthTrxn_RateSub.ToArray())
                                            {
                                                if (arrMonthTrxn_RateSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxn_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxn_RateSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxn_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxn_RateSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }
                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthAMT_RateSub.ToArray())
                                            {
                                                if (arrMonthAMT_RateSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMT_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMT_RateSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMT_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMT_RateSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }

                                            #endregion


                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthChargeSub.ToArray())
                                            {
                                                if (arrMonthChargeSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthChargeSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthChargeSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthChargeSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthChargeSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }

                                            #endregion


                                            var entAccruedItemSub = new AccruedDetailSubViewModel();
                                            MVMMappingService.MoveData(model, entAccruedItemSub);
                                            entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;
                                            entAccruedItemSub.ACCRUED_MONTH = month;
                                            entAccruedItemSub.ACCRUED_YEAR = year;
                                            entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                            entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                            entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                            entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                            entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                            entAccruedItemSub.PRO_NO = model.PRO_NO;
                                            entAccruedItemSub.INV_NO = model.INV_NO;
                                            entAccruedItemSub.INV_MONTH = j;
                                            entAccruedItemSub.INV_YEAR = i;
                                            entAccruedItemSub.SEQUENCE = subsequence;
                                            entAccruedItemSub.PAYMENT_ITEMS_FEE_ITEM = sub.PAYMENT_ITEMS_FEE_NAME;
                                            entAccruedItemSub.TRANSACTIONS = arrMonthTrxnSub[_yearloop - 1];
                                            entAccruedItemSub.ACTUAL_AMOUNT = arrMonthAMTSub[_yearloop - 1];
                                            entAccruedItemSub.TOTAL_CHARGE_AMOUNT = arrMonthChargeSub[_yearloop - 1];
                                            entAccruedItemSub.RATE_TRANS = arrMonthTrxn_RateSub[_yearloop - 1];
                                            entAccruedItemSub.RATE_AMT = arrMonthAMT_RateSub[_yearloop - 1];
                                            entAccruedItemSubLst.Add(entAccruedItemSub);

                                            subsequence++;
                                        }
                                        model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                        #endregion
                                        //Get Json Model AccruedDetail
                                        var jsonSerialiser = new JavaScriptSerializer();
                                        string AccruedJSON = jsonSerialiser.Serialize(model);
                                        model.AccruedJSON = AccruedJSON;


                                        accruedItemList.Add(model);
                                        sequence++;

                                        break;
                                    }

                                    // _monthc++; // keep value month
                                }//end for month
                                _yearloop++;
                            }//end for year


                            #endregion
                        }
                    }//fee payment channels
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return accruedItemList;
        }

        public List<AccruedDetailViewModel> InitialAccruedItemsList_Old(string companyCode, int month, int year, string formState, int accrued_id = 0)
        {
            //AccruedViewModel accruedList = new AccruedViewModel();
            List<AccruedDetailViewModel> accruedItemList = new List<AccruedDetailViewModel>();
            User userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    // --Payment Items Get Description

                    var payment_items = (from m in context.PAYMENT_ITEMS
                                         where m.IS_ACTIVE == true &&
                                         m.COMPANY_CODE == companyCode
                                         orderby m.GROUP_SEQ_CHANNELS
                                         select m).ToList();
                    #region mark inv & acc
                    //// --Invoice Get Invoice List
                    //var entFeeInv = (from m in context.FEE_INVOICE
                    //                 where //m.INV_MONTH <= month && m.INV_YEAR <= year
                    //                 (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                    //                 && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                    //                 && m.COMPANY_CODE == companyCode
                    //                 orderby m.INV_MONTH, m.INV_YEAR
                    //                 select m).ToList();

                    //// --Invoice Get Accrued List
                    //var entFeeAcrr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                    //                  where //m.INV_MONTH <= month && m.INV_YEAR <= year
                    //                  (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                    //                  && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                    //                  && m.COMPANY_CODE == companyCode
                    //                  orderby m.INV_MONTH, m.INV_YEAR
                    //                  select m).ToList();
                    // --Invoice Get Invoice List
                    #endregion
                    var entFeeInv = (from m in context.FEE_INVOICE
                                     where m.COMPANY_CODE == companyCode
                                     orderby m.INV_MONTH, m.INV_YEAR
                                     select m).ToList();

                    // --Invoice Get Accrued List
                    var entFeeAcrr = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                      where m.COMPANY_CODE == companyCode

                                      orderby m.INV_MONTH, m.INV_YEAR
                                      select m).ToList();


                    // --Invoice Get Invoice Item List
                    var entFeeInvItem = (from m in context.FEE_INVOICE_ITEM where m.COMPANY_CODE == companyCode select m).ToList();

                    // --Accrued Get Accrued Item List
                    var entFeeAccruedSubItem = (from m in context.FEE_ACCRUED_PLAN_ITEM_SUB where m.COMPANY_CODE == companyCode select m).ToList();

                    //var payment_items = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE==true  select m).ToList();
                    var payment_items_charge = (from m in context.PAYMENT_ITEMS_CHAGE where m.COMPANY_CODE == companyCode select m).ToList();

                    //Payment Items Charge List
                    if (payment_items.Any())
                    {
                        payment_items_charge = payment_items_charge.Where(m => payment_items.Any(i => m.PAYMENT_ITEMS_ID == i.ID)).ToList();
                    }



                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var qtyMonth = (year * 12 + month) - (2018 * 12 + 1);

                    int currentMonth = DateTime.Now.Date.Month;
                    int months = qtyMonth + 1;
                    int years = 2018;

                    #region Detail
                    int sequence = 1;
                    foreach (var item in payment_items)
                    {
                        var feeInvList = entFeeInv.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var feeAccrList = entFeeAcrr.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        if (item.DURATION == "M")
                        {
                            feeInvList = (from m in feeInvList
                                          where (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                                            && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                                          orderby m.INV_MONTH, m.INV_YEAR
                                          select m).ToList();

                            // --Invoice Get Accrued List
                            feeAccrList = (from m in feeAccrList
                                           where (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1
                                              && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                                           orderby m.INV_MONTH, m.INV_YEAR
                                           select m).ToList();
                        }
                        else if (item.DURATION == "Y")
                        {
                            feeInvList = (from m in feeInvList
                                          where m.INV_YEAR >= (year - 3) &&
                                          m.INV_YEAR <= year
                                          orderby m.INV_MONTH, m.INV_YEAR
                                          select m).ToList();

                            // --Invoice Get Accrued List
                            feeAccrList = (from m in feeAccrList
                                           where m.INV_YEAR >= (year - 3) &&
                                           m.INV_YEAR <= year
                                           orderby m.INV_MONTH, m.INV_YEAR
                                           select m).ToList();
                        }


                        var get_entFeeInvItem = (from n in entFeeInvItem
                                                 where feeInvList.Any(f => n.INV_NO == f.INV_NO)//n.INV_NO == item_fee.INV_NO 

                                                 orderby n.SEQUENCE, n.INV_MONTH, n.INV_YEAR, n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending
                                                 select n).ToList();

                        var get_entFeeAcrrItem = (from n in entFeeAccruedSubItem
                                                  where feeAccrList.Any(f => n.ACCRUED_ITEM_ID == f.ACCRUED_ITEM_ID)//n.INV_NO == item_fee.INV_NO 

                                                  orderby n.SEQUENCE, n.INV_MONTH, n.INV_YEAR, n.RATE_TRANS descending, n.TRANSACTIONS descending, n.RATE_AMT descending, n.ACTUAL_AMOUNT descending
                                                  select n).ToList();
                        if (item.DURATION == "M")
                        {
                            #region Trxn + Amt
                            var data = (from m in get_entFeeInvItem
                                        select m).ToList();

                            var data_accr = (from m in get_entFeeAcrrItem
                                             select m).ToList();

                            var get_pymitem = payment_items.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).FirstOrDefault();

                            decimal[] arrMonthTrxn = new decimal[months];
                            decimal[] arrMonthAMT = new decimal[months];
                            decimal[] arrMonthCharge = new decimal[months];

                            #region amt
                            int _month = 1;
                            int _year = years;

                            for (int i = 1; i <= months; i++)
                            {
                                if (i == 13)
                                {
                                    _month = 1;
                                    _year = _year + 1;
                                }
                                var data_assign_inv = data.Where(m => m.INV_MONTH == _month && m.INV_YEAR == _year).ToList();
                                var data_assign_accr = data_accr.Where(m => m.INV_MONTH == _month && m.INV_YEAR == _year).ToList();
                                if (data_assign_inv.Any())
                                {
                                    arrMonthTrxn[i - 1] = data_assign_inv.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[i - 1] = data_assign_inv.Sum(m => (m.ACTUAL_AMOUNT ?? 0));
                                    arrMonthCharge[i - 1] = data_assign_inv.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }
                                else if (data_assign_accr.Any())
                                {
                                    arrMonthTrxn[i - 1] = data_assign_accr.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[i - 1] = data_assign_accr.Sum(m => (m.ACTUAL_AMOUNT ?? 0));
                                    arrMonthCharge[i - 1] = data_assign_accr.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }

                                _month++;
                            }

                            #region Generate Value Amount on Every Month

                            int iTrxn = 0;
                            foreach (var arr in arrMonthTrxn.ToArray())
                            {
                                if (arrMonthTrxn.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;
                                    }
                                    else
                                    {

                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthAMT.ToArray())
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;
                                    }
                                    else
                                    {


                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthCharge.ToArray())
                            {
                                if (arrMonthCharge.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;
                                    }
                                    else
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            #endregion


                            #endregion

                            //Check Data Had in Invoice
                            var chkitem_chrge = feeInvList.Where(m =>
                             //m.INV_MONTH >= 1 && m.INV_MONTH <= month && m.INV_YEAR >= 2018 && m.INV_YEAR <= year
                             (m.INV_YEAR * 12) + m.INV_MONTH >= (2018 * 12) + 1 && (m.INV_YEAR * 12) + m.INV_MONTH <= (year * 12) + month
                            ).ToList();
                            int _monthc = 1;
                            int _yearc = years;
                            for (int i = 1; i <= months; i++)
                            {
                                if (i == 13)
                                {
                                    _monthc = 1;
                                    _yearc = _yearc + 1;
                                }
                                var item_chrge = feeInvList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_MONTH == _monthc && m.INV_YEAR == _yearc).FirstOrDefault();
                                var item_acccharge = feeAccrList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.ACCRUED_ID == accrued_id && m.INV_MONTH == _monthc && m.INV_YEAR == _yearc).FirstOrDefault();
                                var model = new AccruedDetailViewModel();

                                if (item_chrge != null)
                                {

                                    if (item_chrge.IS_STATUS != "3")
                                    {
                                        if (item_chrge.IS_STATUS == "4")
                                        {
                                            _monthc++;
                                            continue;
                                        }
                                        else
                                        {
                                            #region case not complete

                                            model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                            model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                            model.PERIOD_ACCRUED = string.Concat(item_chrge.INV_MONTH.ToString().PadLeft(2, '0'), " / ", item_chrge.INV_YEAR);
                                            model.INV_MONTH = (item_chrge.INV_MONTH ?? 0);
                                            model.INV_YEAR = (item_chrge.INV_YEAR ?? 0);

                                            model.SEQUENCE = sequence;
                                            model.CHANNELS = item.CHANNELS;
                                            model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                            model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                            model.CURRENCY = "THB";
                                            model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                            model.CCT_CODE = item.CCT_CODE;
                                            model.GL_ACCOUNT = item.GL_ACCOUNT;
                                            model.COST_CENTER = item.COST_CENTER;
                                            model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                            model.ACCRUED_MONTH = month;// _monthc;
                                            model.ACCRUED_YEAR = year;// _yearc;
                                            model.COMPANY_CODE = companyCode;
                                            model.INV_NO = item_chrge.INV_NO;
                                            model.PRO_NO = item_chrge.PRO_NO;


                                            model.TRANSACTIONS = arrMonthTrxn[i - 1];
                                            model.AMOUNT = arrMonthAMT[i - 1];
                                            model.INV_AMOUNT = arrMonthCharge[i - 1];

                                            if (get_entFeeInvItem.Sum(m => (m.TRANSACTIONS ?? 0)) == 0 && get_entFeeInvItem.Sum(m => (m.ACTUAL_AMOUNT ?? 0)) == 0)
                                            {
                                                model.REMARK = "ประมาณการย้อนหลัง 3 เดือน";
                                                model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 เดือน";

                                                model.ISPLAN = true;
                                            }
                                            else
                                            {
                                                model.REMARK = "Invoice " + item_chrge.INV_MONTH.ToString().PadLeft(2, '0') + "/ " + item_chrge.INV_YEAR;
                                                model.REMARK_INVOICE = "Inv No. " + item_chrge.INV_NO;

                                                model.ISPLAN = false;

                                            }
                                            if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                            {
                                                model.CREATE_BY = userInfo.UserCode;
                                                model.CREATE_DATE = DateTime.Now;
                                            }
                                            else
                                            {
                                                model.MODIFIED_BY = userInfo.UserCode;
                                                //model.MODIFIED_DATE = DateTime.Now;
                                            }
                                            model.MODIFIED_DATE = DateTime.Now;
                                            #region sub accrued
                                            //var dataAccruedSub = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_NO == model.INV_NO ).ToList();
                                            //var acdataAccruedSub = data_accr.Where(m =>  m.INV_MONTH == model.INV_MONTH && m.INV_YEAR == model.INV_YEAR).ToList();
                                            var dataAccruedSub = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_NO == model.INV_NO).ToList();
                                            var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();


                                            var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                            foreach (var sub in dataAccruedSub)
                                            {
                                                var entAccruedItemSub = new AccruedDetailSubViewModel();

                                                var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_ITEM).FirstOrDefault();

                                                MVMMappingService.MoveData(sub, entAccruedItemSub);
                                                entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;

                                                //entAccruedItemSub.CCT_CODE = sub.COST_CENTER;
                                                entAccruedItemSub.ACCRUED_MONTH = month;// _monthc;
                                                entAccruedItemSub.ACCRUED_YEAR = year;// _yearc;
                                                entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                                entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                                entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                                entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                                entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                                entAccruedItemSub.PRO_NO = model.PRO_NO;
                                                entAccruedItemSub.REMARK = model.REMARK;
                                                entAccruedItemSubLst.Add(entAccruedItemSub);
                                            }
                                            model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                            #endregion
                                            //Get Json Model AccruedDetail
                                            var jsonSerialiser = new JavaScriptSerializer();
                                            string AccruedJSON = jsonSerialiser.Serialize(model);
                                            model.AccruedJSON = AccruedJSON;

                                            model.EDITION = 0;
                                            accruedItemList.Add(model);
                                            sequence++;
                                            #endregion
                                        }
                                    }//charge
                                }
                                else
                                {
                                    model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                    model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                    model.PERIOD_ACCRUED = string.Concat(_monthc.ToString().PadLeft(2, '0'), " / ", _yearc);
                                    model.INV_MONTH = _monthc;
                                    model.INV_YEAR = _yearc;
                                    model.SEQUENCE = sequence;
                                    model.CHANNELS = item.CHANNELS;
                                    model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                    model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                    model.CURRENCY = "THB";
                                    model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                    model.CCT_CODE = item.CCT_CODE;
                                    model.GL_ACCOUNT = item.GL_ACCOUNT;
                                    model.COST_CENTER = item.COST_CENTER;
                                    model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                    model.ACCRUED_MONTH = month;// _monthc;
                                    model.ACCRUED_YEAR = year;// _yearc;
                                    model.COMPANY_CODE = companyCode;


                                    model.TRANSACTIONS = arrMonthTrxn[i - 1];
                                    model.AMOUNT = arrMonthAMT[i - 1];
                                    model.INV_AMOUNT = arrMonthCharge[i - 1];


                                    model.REMARK = "ประมาณการย้อนหลัง 3 เดือน";
                                    model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 เดือน";

                                    model.ISPLAN = true;

                                    model.EDITION = 0;

                                    if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                    {
                                        model.CREATE_BY = userInfo.UserCode;
                                        model.CREATE_DATE = DateTime.Now;
                                    }
                                    else
                                    {
                                        model.MODIFIED_BY = userInfo.UserCode;
                                        //model.MODIFIED_DATE = DateTime.Now;
                                    }
                                    model.MODIFIED_DATE = DateTime.Now;
                                    #region sub accrued
                                    int subsequence = 1;
                                    var dataAccruedSub = payment_items_charge.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).ToList();
                                    var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();

                                    var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                    foreach (var sub in dataAccruedSub)
                                    {
                                        var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).FirstOrDefault();
                                        decimal[] arrMonthTrxnSub = new decimal[months];
                                        decimal[] arrMonthAMTSub = new decimal[months];
                                        decimal[] arrMonthChargeSub = new decimal[months];

                                        decimal[] arrMonthTrxn_RateSub = new decimal[months];
                                        decimal[] arrMonthAMT_RateSub = new decimal[months];

                                        #region amt
                                        foreach (var dataSub in data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).OrderBy(m => m.ID).ToList())
                                        {
                                            arrMonthTrxnSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TRANSACTIONS ?? 0);
                                            arrMonthAMTSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.ACTUAL_AMOUNT ?? 0);
                                            arrMonthChargeSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TOTAL_CHARGE_AMOUNT ?? 0);

                                            arrMonthTrxn_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_TRANS ?? 0);
                                            arrMonthAMT_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_AMT ?? 0);
                                        }

                                        int iTrxnSub = 0;
                                        foreach (var arr in arrMonthTrxnSub.ToArray())
                                        {
                                            if (arrMonthTrxnSub.ToList().All(m => m == 0))
                                            { break; }
                                            if (arr == 0)
                                            {
                                                if (iTrxnSub > 2)
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub - 1;
                                                    int avgIndexS = iTrxnSub - 3;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthTrxnSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthTrxnSub[iTrxnSub] = avgVal;
                                                }
                                                else
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                    int avgIndexS = 0;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthTrxnSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthTrxnSub[iTrxnSub] = avgVal;

                                                }
                                            }
                                            iTrxnSub++;
                                        }
                                        iTrxnSub = 0;
                                        foreach (var arr in arrMonthAMTSub.ToArray())
                                        {
                                            if (arrMonthAMTSub.ToList().All(m => m == 0))
                                            { break; }
                                            if (arr == 0)
                                            {
                                                if (iTrxnSub > 2)
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub - 1;
                                                    int avgIndexS = iTrxnSub - 3;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthAMTSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthAMTSub[iTrxnSub] = avgVal;
                                                }
                                                else
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                    int avgIndexS = 0;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthAMTSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthAMTSub[iTrxnSub] = avgVal;

                                                }
                                            }
                                            iTrxnSub++;
                                        }


                                        #region avg rate

                                        iTrxnSub = 0;
                                        foreach (var arr in arrMonthTrxn_RateSub.ToArray())
                                        {
                                            if (arrMonthTrxn_RateSub.ToList().All(m => m == 0))
                                            { break; }
                                            if (arr == 0)
                                            {
                                                if (iTrxnSub > 2)
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub - 1;
                                                    int avgIndexS = iTrxnSub - 3;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthTrxn_RateSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthTrxn_RateSub[iTrxnSub] = avgVal;
                                                }
                                                else
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                    int avgIndexS = 0;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthTrxn_RateSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthTrxn_RateSub[iTrxnSub] = avgVal;

                                                }
                                            }
                                            iTrxnSub++;
                                        }
                                        iTrxnSub = 0;
                                        foreach (var arr in arrMonthAMT_RateSub.ToArray())
                                        {
                                            if (arrMonthAMT_RateSub.ToList().All(m => m == 0))
                                            { break; }
                                            if (arr == 0)
                                            {
                                                if (iTrxnSub > 2)
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub - 1;
                                                    int avgIndexS = iTrxnSub - 3;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthAMT_RateSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthAMT_RateSub[iTrxnSub] = avgVal;
                                                }
                                                else
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                    int avgIndexS = 0;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthAMT_RateSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthAMT_RateSub[iTrxnSub] = avgVal;

                                                }
                                            }
                                            iTrxnSub++;
                                        }

                                        #endregion


                                        iTrxnSub = 0;
                                        foreach (var arr in arrMonthChargeSub.ToArray())
                                        {
                                            if (arrMonthChargeSub.ToList().All(m => m == 0))
                                            { break; }
                                            if (arr == 0)
                                            {
                                                if (iTrxnSub > 2)
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub - 1;
                                                    int avgIndexS = iTrxnSub - 3;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthChargeSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthChargeSub[iTrxnSub] = avgVal;
                                                }
                                                else
                                                {
                                                    decimal avgVal = 0;
                                                    int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                    int avgIndexS = 0;
                                                    List<decimal> avgList = new List<decimal>();
                                                    for (int a = avgIndexS; a <= avgIndexE; a++)
                                                    {
                                                        avgList.Add(arrMonthChargeSub[a]);
                                                    }
                                                    avgVal = avgList.Average();
                                                    arrMonthChargeSub[iTrxnSub] = avgVal;

                                                }
                                            }
                                            iTrxnSub++;
                                        }

                                        #endregion


                                        var entAccruedItemSub = new AccruedDetailSubViewModel();
                                        MVMMappingService.MoveData(model, entAccruedItemSub);
                                        entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;
                                        entAccruedItemSub.ACCRUED_MONTH = month;// _monthc;
                                        entAccruedItemSub.ACCRUED_YEAR = year;// _yearc;
                                        entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                        entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                        entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                        entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                        entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                        entAccruedItemSub.PRO_NO = model.PRO_NO;
                                        entAccruedItemSub.INV_NO = model.INV_NO;
                                        entAccruedItemSub.INV_MONTH = _monthc;
                                        entAccruedItemSub.INV_YEAR = _yearc;
                                        entAccruedItemSub.SEQUENCE = subsequence;
                                        entAccruedItemSub.PAYMENT_ITEMS_FEE_ITEM = sub.PAYMENT_ITEMS_FEE_NAME;
                                        entAccruedItemSub.TRANSACTIONS = arrMonthTrxnSub[i - 1];
                                        entAccruedItemSub.ACTUAL_AMOUNT = arrMonthAMTSub[i - 1];
                                        entAccruedItemSub.TOTAL_CHARGE_AMOUNT = arrMonthChargeSub[i - 1];
                                        entAccruedItemSub.RATE_TRANS = arrMonthTrxn_RateSub[i - 1];
                                        entAccruedItemSub.RATE_AMT = arrMonthAMT_RateSub[i - 1];
                                        entAccruedItemSubLst.Add(entAccruedItemSub);

                                        subsequence++;
                                    }
                                    model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                    #endregion
                                    //Get Json Model AccruedDetail
                                    var jsonSerialiser = new JavaScriptSerializer();
                                    string AccruedJSON = jsonSerialiser.Serialize(model);
                                    model.AccruedJSON = AccruedJSON;


                                    accruedItemList.Add(model);
                                    sequence++;
                                }

                                _monthc++; // keep value month

                            }//end for

                            #endregion
                        }
                        else if (item.DURATION == "Y")
                        {
                            #region Trxn + Amt Duration of Year
                            var data = (from m in get_entFeeInvItem
                                        select m).ToList();

                            var data_accr = (from m in get_entFeeAcrrItem
                                             select m).ToList();

                            var get_pymitem = payment_items.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).FirstOrDefault();

                            var getmonths = (year - 2018) + 1;

                            decimal[] arrMonthTrxn = new decimal[getmonths];
                            decimal[] arrMonthAMT = new decimal[getmonths];
                            decimal[] arrMonthCharge = new decimal[getmonths];

                            int _month = 1;
                            #region amt


                            int yearS = (year - 3) > 2018 ? (year - 3) : 2018;

                            int _year = yearS;
                            int loop = 0;
                            for (int i = yearS; i <= year; i++)
                            {
                                var data_assign_inv = data.Where(m => m.INV_YEAR == i).ToList();
                                var data_assign_accr = data_accr.Where(m => m.INV_YEAR == i).ToList();
                                if (data_assign_inv.Any())
                                {
                                    arrMonthTrxn[loop] = data_assign_inv.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[loop] = data_assign_inv.Sum(m => (m.ACTUAL_AMOUNT ?? 0));
                                    arrMonthCharge[loop] = data_assign_inv.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }
                                else if (data_assign_accr.Any())
                                {
                                    arrMonthTrxn[loop] = data_assign_accr.Sum(m => (m.TRANSACTIONS ?? 0));
                                    arrMonthAMT[loop] = data_assign_accr.Sum(m => (m.ACTUAL_AMOUNT ?? 0));
                                    arrMonthCharge[loop] = data_assign_accr.Sum(m => (m.TOTAL_CHARGE_AMOUNT ?? 0));
                                }

                                loop++;
                            }

                            #region Generate Value Amount on Every Month

                            int iTrxn = 0;
                            foreach (var arr in arrMonthTrxn.ToArray())
                            {
                                if (arrMonthTrxn.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;
                                    }
                                    else
                                    {

                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthTrxn[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthTrxn[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthAMT.ToArray())
                            {
                                if (arrMonthAMT.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;
                                    }
                                    else
                                    {


                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthAMT[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthAMT[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            iTrxn = 0;
                            foreach (var arr in arrMonthCharge.ToArray())
                            {
                                if (arrMonthCharge.ToList().All(m => m == 0))
                                { break; }
                                if (arr == 0)
                                {
                                    if (iTrxn > 2)
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn - 1;
                                        int avgIndexS = iTrxn - 3;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;
                                    }
                                    else
                                    {
                                        decimal avgVal = 0;
                                        int avgIndexE = iTrxn == 0 ? iTrxn : iTrxn - 1;
                                        int avgIndexS = 0;
                                        List<decimal> avgList = new List<decimal>();
                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                        {
                                            avgList.Add(arrMonthCharge[a]);
                                        }
                                        avgVal = avgList.Average();
                                        arrMonthCharge[iTrxn] = avgVal;

                                    }
                                }
                                iTrxn++;
                            }
                            #endregion


                            #endregion


                            int _yearc = getmonths;
                            int _yearloop = 1;
                            for (int i = 2018; i <= year; i++)
                            {

                                var item_chrgeofYear = feeInvList.Where(m => m.INV_YEAR == i).ToList();

                                var model = new AccruedDetailViewModel();
                                for (int j = 1; j <= 12; j++)
                                {
                                    var item_chrge = item_chrgeofYear.Where(m => m.INV_MONTH == j).FirstOrDefault();
                                    var item_acccharge = feeAccrList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.ACCRUED_ID == accrued_id && m.INV_MONTH == j && m.INV_YEAR == i).FirstOrDefault();

                                    if (item_chrge != null)
                                    {

                                        if (item_chrge.IS_STATUS != "3")
                                        {
                                            if (item_chrge.IS_STATUS == "4")
                                            {
                                                _yearc++;
                                                j++;
                                                continue;
                                            }
                                            else
                                            {
                                                #region case not complete
                                                model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                                model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                                model.PERIOD_ACCRUED = Convert.ToString(item_chrge.INV_YEAR);
                                                model.INV_MONTH = (item_chrge.INV_MONTH ?? 0);
                                                model.INV_YEAR = (item_chrge.INV_YEAR ?? 0);

                                                model.SEQUENCE = sequence;
                                                model.CHANNELS = item.CHANNELS;
                                                model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                                model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                                model.CURRENCY = "THB";
                                                model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                                model.CCT_CODE = item.CCT_CODE;
                                                model.GL_ACCOUNT = item.GL_ACCOUNT;
                                                model.COST_CENTER = item.COST_CENTER;
                                                model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                                model.ACCRUED_MONTH = month;// generate accrued advanced
                                                model.ACCRUED_YEAR = year;
                                                model.COMPANY_CODE = companyCode;
                                                model.INV_NO = item_chrge.INV_NO;
                                                model.PRO_NO = item_chrge.PRO_NO;


                                                model.TRANSACTIONS = arrMonthTrxn[_yearloop - 1];
                                                model.AMOUNT = arrMonthAMT[_yearloop - 1];
                                                model.INV_AMOUNT = arrMonthCharge[_yearloop - 1];

                                                if (get_entFeeInvItem.Sum(m => (m.TRANSACTIONS ?? 0)) == 0 && get_entFeeInvItem.Sum(m => (m.ACTUAL_AMOUNT ?? 0)) == 0)
                                                {
                                                    model.REMARK = "ประมาณการย้อนหลัง 3 ปี";
                                                    model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 ปี";

                                                    model.ISPLAN = true;
                                                }
                                                else
                                                {
                                                    model.REMARK = "Invoice " + item_chrge.INV_YEAR;
                                                    model.REMARK_INVOICE = "Inv No. " + item_chrge.INV_NO;

                                                    model.ISPLAN = false;

                                                }
                                                if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    model.CREATE_BY = userInfo.UserCode;
                                                    model.CREATE_DATE = DateTime.Now;
                                                }
                                                else
                                                {
                                                    model.MODIFIED_BY = userInfo.UserCode;
                                                    //model.MODIFIED_DATE = DateTime.Now;
                                                }
                                                model.MODIFIED_DATE = DateTime.Now;
                                                #region sub accrued
                                                var dataAccruedSub = data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.INV_NO == model.INV_NO).ToList();
                                                var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();

                                                var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                                foreach (var sub in dataAccruedSub)
                                                {
                                                    var entAccruedItemSub = new AccruedDetailSubViewModel();

                                                    var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_ITEM).FirstOrDefault();

                                                    MVMMappingService.MoveData(sub, entAccruedItemSub);
                                                    entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;
                                                    //MVMMappingService.MoveData(sub, entAccruedItemSub);
                                                    //entAccruedItemSub.ID = 0;
                                                    //entAccruedItemSub.CCT_CODE = sub.COST_CENTER;
                                                    entAccruedItemSub.ACCRUED_MONTH = month;
                                                    entAccruedItemSub.ACCRUED_YEAR = year;
                                                    entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                                    entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                                    entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                                    entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                                    entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                                    entAccruedItemSub.PRO_NO = model.PRO_NO;
                                                    entAccruedItemSub.REMARK = model.REMARK;
                                                    entAccruedItemSubLst.Add(entAccruedItemSub);
                                                }
                                                model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                                #endregion
                                                //Get Json Model AccruedDetail
                                                var jsonSerialiser = new JavaScriptSerializer();
                                                string AccruedJSON = jsonSerialiser.Serialize(model);
                                                model.AccruedJSON = AccruedJSON;

                                                model.EDITION = 0;
                                                accruedItemList.Add(model);
                                                sequence++;
                                                #endregion
                                            }
                                        }//charge

                                        break;
                                    }
                                    else
                                    {
                                        model.ACCRUED_ID = item_acccharge != null ? item_acccharge.ACCRUED_ID : 0;
                                        model.ACCRUED_ITEM_ID = item_acccharge != null ? item_acccharge.ACCRUED_ITEM_ID : 0;
                                        model.PERIOD_ACCRUED = Convert.ToString(i);
                                        model.INV_MONTH = j;
                                        model.INV_YEAR = i;
                                        model.SEQUENCE = sequence;
                                        model.CHANNELS = item.CHANNELS;
                                        model.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                                        model.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;

                                        model.CURRENCY = "THB";
                                        model.Supplier = get_pymitem == null ? "" : get_pymitem.Supplier;
                                        model.CCT_CODE = item.CCT_CODE;
                                        model.GL_ACCOUNT = item.GL_ACCOUNT;
                                        model.COST_CENTER = item.COST_CENTER;
                                        model.COST_CENTER_FUND = string.IsNullOrEmpty(item.FUND_CODE) ? item.COST_CENTER : string.Concat(item.COST_CENTER, "/", item.FUND_CODE);


                                        model.ACCRUED_MONTH = month;
                                        model.ACCRUED_YEAR = year;
                                        model.COMPANY_CODE = companyCode;


                                        model.TRANSACTIONS = arrMonthTrxn[_yearloop - 1];
                                        model.AMOUNT = arrMonthAMT[_yearloop - 1];
                                        model.INV_AMOUNT = arrMonthCharge[_yearloop - 1];


                                        model.REMARK = "ประมาณการย้อนหลัง 3 ปี";
                                        model.REMARK_INVOICE = "ประมาณการย้อนหลัง 3 ปี";

                                        model.ISPLAN = true;

                                        model.EDITION = 0;

                                        if (string.Equals(formState, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                                        {
                                            model.CREATE_BY = userInfo.UserCode;
                                            model.CREATE_DATE = DateTime.Now;
                                        }
                                        else
                                        {
                                            model.MODIFIED_BY = userInfo.UserCode;
                                            //model.MODIFIED_DATE = DateTime.Now;
                                        }
                                        model.MODIFIED_DATE = DateTime.Now;
                                        #region sub accrued
                                        int subsequence = 1;
                                        var dataAccruedSub = payment_items_charge.Where(m => m.PAYMENT_ITEMS_NAME == item.PAYMENT_ITEMS_NAME).ToList();

                                        var acdataAccruedSub = data_accr.Where(m => m.ACCRUED_ITEM_ID == model.ACCRUED_ITEM_ID).ToList();

                                        var entAccruedItemSubLst = new List<AccruedDetailSubViewModel>();
                                        foreach (var sub in dataAccruedSub)
                                        {

                                            var getaccSub = acdataAccruedSub.Where(m => m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).FirstOrDefault();


                                            decimal[] arrMonthTrxnSub = new decimal[getmonths];
                                            decimal[] arrMonthAMTSub = new decimal[getmonths];
                                            decimal[] arrMonthChargeSub = new decimal[getmonths];

                                            decimal[] arrMonthTrxn_RateSub = new decimal[getmonths];
                                            decimal[] arrMonthAMT_RateSub = new decimal[getmonths];

                                            #region amt
                                            foreach (var dataSub in data.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE && m.PAYMENT_ITEMS_FEE_ITEM == sub.PAYMENT_ITEMS_FEE_NAME).OrderBy(m => m.ID).ToList())
                                            {
                                                arrMonthTrxnSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TRANSACTIONS ?? 0);
                                                arrMonthAMTSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.ACTUAL_AMOUNT ?? 0);
                                                arrMonthChargeSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.TOTAL_CHARGE_AMOUNT ?? 0);

                                                arrMonthTrxn_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_TRANS ?? 0);
                                                arrMonthAMT_RateSub[(dataSub.INV_MONTH ?? 0) - 1] = (dataSub.RATE_AMT ?? 0);
                                            }

                                            int iTrxnSub = 0;
                                            foreach (var arr in arrMonthTrxnSub.ToArray())
                                            {
                                                if (arrMonthTrxnSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxnSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxnSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxnSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxnSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }
                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthAMTSub.ToArray())
                                            {
                                                if (arrMonthAMTSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMTSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMTSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMTSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMTSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }


                                            #region avg rate

                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthTrxn_RateSub.ToArray())
                                            {
                                                if (arrMonthTrxn_RateSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxn_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxn_RateSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthTrxn_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthTrxn_RateSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }
                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthAMT_RateSub.ToArray())
                                            {
                                                if (arrMonthAMT_RateSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMT_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMT_RateSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthAMT_RateSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthAMT_RateSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }

                                            #endregion


                                            iTrxnSub = 0;
                                            foreach (var arr in arrMonthChargeSub.ToArray())
                                            {
                                                if (arrMonthChargeSub.ToList().All(m => m == 0))
                                                { break; }
                                                if (arr == 0)
                                                {
                                                    if (iTrxnSub > 2)
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub - 1;
                                                        int avgIndexS = iTrxnSub - 3;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthChargeSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthChargeSub[iTrxnSub] = avgVal;
                                                    }
                                                    else
                                                    {
                                                        decimal avgVal = 0;
                                                        int avgIndexE = iTrxnSub == 0 ? iTrxnSub : iTrxnSub - 1;
                                                        int avgIndexS = 0;
                                                        List<decimal> avgList = new List<decimal>();
                                                        for (int a = avgIndexS; a <= avgIndexE; a++)
                                                        {
                                                            avgList.Add(arrMonthChargeSub[a]);
                                                        }
                                                        avgVal = avgList.Average();
                                                        arrMonthChargeSub[iTrxnSub] = avgVal;

                                                    }
                                                }
                                                iTrxnSub++;
                                            }

                                            #endregion


                                            var entAccruedItemSub = new AccruedDetailSubViewModel();
                                            MVMMappingService.MoveData(model, entAccruedItemSub);
                                            entAccruedItemSub.ID = getaccSub != null ? getaccSub.ID : 0;
                                            entAccruedItemSub.ACCRUED_MONTH = month;
                                            entAccruedItemSub.ACCRUED_YEAR = year;
                                            entAccruedItemSub.CREATE_BY = model.CREATE_BY;
                                            entAccruedItemSub.CREATE_DATE = model.CREATE_DATE;
                                            entAccruedItemSub.MODIFIED_BY = model.MODIFIED_BY;
                                            entAccruedItemSub.MODIFIED_DATE = model.MODIFIED_DATE;
                                            entAccruedItemSub.NET_AMOUNT = model.INV_AMOUNT;
                                            entAccruedItemSub.PRO_NO = model.PRO_NO;
                                            entAccruedItemSub.INV_NO = model.INV_NO;
                                            entAccruedItemSub.INV_MONTH = j;
                                            entAccruedItemSub.INV_YEAR = i;
                                            entAccruedItemSub.SEQUENCE = subsequence;
                                            entAccruedItemSub.PAYMENT_ITEMS_FEE_ITEM = sub.PAYMENT_ITEMS_FEE_NAME;
                                            entAccruedItemSub.TRANSACTIONS = arrMonthTrxnSub[_yearloop - 1];
                                            entAccruedItemSub.ACTUAL_AMOUNT = arrMonthAMTSub[_yearloop - 1];
                                            entAccruedItemSub.TOTAL_CHARGE_AMOUNT = arrMonthChargeSub[_yearloop - 1];
                                            entAccruedItemSub.RATE_TRANS = arrMonthTrxn_RateSub[_yearloop - 1];
                                            entAccruedItemSub.RATE_AMT = arrMonthAMT_RateSub[_yearloop - 1];
                                            entAccruedItemSubLst.Add(entAccruedItemSub);

                                            subsequence++;
                                        }
                                        model.AccruedItemSubList.AddRange(entAccruedItemSubLst);
                                        #endregion
                                        //Get Json Model AccruedDetail
                                        var jsonSerialiser = new JavaScriptSerializer();
                                        string AccruedJSON = jsonSerialiser.Serialize(model);
                                        model.AccruedJSON = AccruedJSON;


                                        accruedItemList.Add(model);
                                        sequence++;

                                        break;
                                    }

                                    // _monthc++; // keep value month
                                }//end for month
                                _yearloop++;
                            }//end for year


                            #endregion
                        }
                    }//fee payment channels
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return accruedItemList;
        }


        public AccruedViewModel GetDetail(int month, int year, string companyCode, string formState)
        {
            AccruedViewModel accruedList = new AccruedViewModel();
            var userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var getPaymentItemList = (from data in context.PAYMENT_ITEMS where data.IS_ACTIVE == true && data.COMPANY_CODE == companyCode orderby data.ID select data).ToList();
                    var getcttList = (from data in context.COST_CENTER where data.COMPANY_CODE == companyCode select data).ToList();

                    //var getFeeInvList = (from data in context.FEE_INVOICE where data.COMPANY_CODE == companyCode && data.INV_MONTH == month && data.INV_YEAR == year select data).ToList();


                    #region accrued
                    accruedList.ACCRUED_ID = 0;
                    //accruedList.ACCRUED_NO = "Acc" + companyCode + DateTime.Now.Date.ToString("ddmmyyyy");
                    accruedList.PERIOD_MONTH = month;
                    accruedList.PERIOD_YEAR = year;
                    accruedList.COMPANY_CODE = companyCode;

                    accruedList.TOTAL_AMT = 0;//wait calculate item
                    accruedList.MODIFIED_BY = userInfo.UserCode;
                    accruedList.MODIFIED_DATE = DateTime.Now;
                    accruedList.CREATE_BY = userInfo.UserCode;
                    accruedList.CREATE_DATE = DateTime.Now;
                    accruedList.APPROVED_BY = ConstantVariableService.APPROVERID;
                    accruedList.APPROVED_DATE = null;




                    accruedList.FormAction = formState;
                    accruedList.FormState = formState;
                    #endregion

                    int i = 1;
                    foreach (var item in getPaymentItemList)
                    {


                        var accDetailModel = new AccruedDetailViewModel();

                        #region item
                        //accDetailModel.ACCRUED_ITEM_NO = accruedList.ACCRUED_NO + "_" + i.ToString();
                        accDetailModel.ACCRUED_ID = accruedList.ACCRUED_ID;
                        accDetailModel.INV_NO = "";//wait
                        accDetailModel.PRO_NO = "";//wait
                        accDetailModel.SEQUENCE = i;
                        accDetailModel.ACCRUED_MONTH = month;
                        accDetailModel.ACCRUED_YEAR = year;
                        accDetailModel.PERIOD_ACCRUED = month.ToString().PadLeft(2, '0') + "/" + year.ToString();
                        accDetailModel.INV_MONTH = month;
                        accDetailModel.INV_YEAR = year;
                        accDetailModel.CCT_CODE = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;

                        accDetailModel.COMPANY_CODE = companyCode;
                        accDetailModel.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;
                        accDetailModel.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                        //accDetailModel.TRANSACTIONS = 0;
                        //accDetailModel.AMOUNT = 0;
                        //accDetailModel.INV_AMOUNT = 0;
                        accDetailModel.MODIFIED_BY = "";
                        accDetailModel.MODIFIED_DATE = null;
                        accDetailModel.CREATE_BY = "";
                        accDetailModel.CREATE_DATE = DateTime.Now.Date;
                        accDetailModel.CURRENCY = "THB";
                        accDetailModel.REMARK_INVOICE = "";//wait
                        accDetailModel.ISPLAN = null;//wait
                        accDetailModel.EDITION = 0;//wait
                        accDetailModel.REMARK = "";//wait
                        #endregion

                        //accDetailModel.GLAccount = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                        //accDetailModel.CostCenter = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;

                        accruedList.AccruedItemList.Add(accDetailModel);


                        i++;


                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return accruedList;
        }

        public string GetList(string companyCode, int month, int year)
        {
            try
            {
                string dataList = "";
                List<AccruedViewModel> paymentitemsList = GetAccruedList(companyCode, month, year);
                dataList = DatatablesService.ConvertObjectListToDatatables<AccruedViewModel>(paymentitemsList);

                return dataList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<AccruedViewModel> GetAccruedList(string companyCode, int month, int year)
        {
            List<AccruedViewModel> entAccruedList = new List<ViewModels.Accrued.AccruedViewModel>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var entComp = (from m in context.COMPANies where m.IsPaymentFee == true && m.BAN_COMPANY == companyCode select m).ToList();
                    var entUser = (from m in context.USERS select m).ToList();

                    var entData = (from m in context.FEE_ACCRUED_PLAN select m).ToList();
                    entData = entData.Where(m => m.COMPANY_CODE == companyCode).ToList();
                    //if (!string.IsNullOrEmpty(companyCode))
                    //{
                    //    entData = entData.Where(m => m.COMPANY_CODE == companyCode).ToList();
                    //}
                    if (month != 0)
                    {
                        entData = entData.Where(m => m.PERIOD_MONTH == month).ToList();
                    }
                    if (year != 0)
                    {
                        entData = entData.Where(m => m.PERIOD_YEAR == year).ToList();
                    }
                    int i = 1;
                    foreach (var item in entData)
                    {
                        var getUserName = entUser.Where(m => m.USERID == item.CREATE_BY).FirstOrDefault();
                        var accruedData = new AccruedViewModel();
                        MVMMappingService.MoveData(item, accruedData);
                        accruedData.ItemNo = i;
                        accruedData.PERIOD_MONTH_NAME = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(month));
                        accruedData.CREATE_BY_NAME = getUserName == null ? "" : getUserName.USERNAME;
                        accruedData.COMPANY_CODE_NAME = entComp.Count() > 0 ? entComp.FirstOrDefault().COMPANY_NAME_EN : "";
                        entAccruedList.Add(accruedData);
                        i++;

                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return entAccruedList;
        }

        private List<AccruedDetailViewModel> GetPaymentItemsDetailList(string companyCode, int month, int year)
        {
            List<AccruedDetailViewModel> getAccList = new List<AccruedDetailViewModel>();
            List<AccruedViewModel> accList = new List<AccruedViewModel>();
            AccruedViewModel accruedList = new AccruedViewModel();
            AccruedDetailViewModel accruedItemList = new AccruedDetailViewModel();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var getPaymentItemList = (from data in context.PAYMENT_ITEMS where data.IS_ACTIVE == true && data.COMPANY_CODE == companyCode orderby data.ID select data).ToList();
                    //var getcttList = (from data in context.COST_CENTER where data.COMPANY_CODE == companyCode select data).ToList();

                    var getFeeInvList = (from data in context.FEE_INVOICE where data.COMPANY_CODE == companyCode && data.INV_MONTH == month && data.INV_YEAR == year select data).ToList();



                    #region item
                    int i = 1;
                    foreach (var item in getPaymentItemList)
                    {
                        #region Accrued
                        accruedList.PERIOD_MONTH = month;
                        accruedList.PERIOD_YEAR = year;
                        accruedList.COMPANY_CODE = companyCode;
                        //accruedList.ACCRUED_NO = "Acc" + companyCode + DateTime.Now.Date.ToString("ddmmyyyy");


                        accruedList.CREATE_BY = "";
                        accruedList.CREATE_DATE = DateTime.Now;
                        accruedList.ID = 0;
                        //accruedList.REMARK = "";
                        accruedList.APPROVED_BY = ConstantVariableService.APPROVERID;
                        accruedList.APPROVED_DATE = null;
                        accruedList.TOTAL_AMT = 0;

                        accruedList.FormAction = ConstantVariableService.FormActionCreate;
                        accruedList.FormState = ConstantVariableService.FormStateCreate;



                        #endregion

                        var accDetailModel = new AccruedDetailViewModel();

                        //accDetailModel.ACCRUED_ITEM_NO = accruedList.ACCRUED_NO + "_" + i.ToString();
                        accDetailModel.ACCRUED_ID = accruedList.ACCRUED_ID;

                        accDetailModel.SEQUENCE = i;
                        accDetailModel.PERIOD_ACCRUED = month.ToString() + "/" + year.ToString();
                        accDetailModel.INV_YEAR = year;
                        accDetailModel.INV_MONTH = month;
                        //accDetailModel.GLAccount = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                        //accDetailModel.CostCenter = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;
                        accDetailModel.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;
                        accDetailModel.INV_AMOUNT = 0;
                        accDetailModel.TRANSACTIONS = 0;
                        accDetailModel.AMOUNT = 0;
                        accDetailModel.CURRENCY = "THB";
                        accDetailModel.EDITION = 0;
                        accDetailModel.REMARK = "";

                        accDetailModel.ACCRUED_MONTH = month;
                        accDetailModel.ACCRUED_YEAR = year;


                        accDetailModel.CCT_CODE = item.CCT_CODE;//getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;
                        accDetailModel.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;
                        accDetailModel.COST_CENTER = item.COST_CENTER;
                        //accruedList.AccruedItem.SEQUENCE = i;
                        //accruedList.AccruedItem.PERIOD_ACCRUED = month.ToString() + "/" + year.ToString();
                        //accruedList.AccruedItem.GLAccount = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                        //accruedList.AccruedItem.CostCenter = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;
                        //accruedList.AccruedItem.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;

                        //accruedList.AccruedItem.INV_AMOUNT = i;
                        //accruedList.AccruedItem.TRANSACTIONS = i;
                        //accruedList.AccruedItem.AMOUNT = i;
                        //accruedList.AccruedItem.CURRENCY = "THB";
                        //accruedList.AccruedItem.EDITION = 0;
                        //accruedList.AccruedItem.REMARK = "";



                        /*
                         * 
                         * accDetailModel.RANSACTIONS ;
                         accDetailModel.AMOUNT ;
                         accDetailModel.INV_AMOUNT ;
                         accDetailModel.CURRENCY ;
                         accDetailModel.EDITION ;
                         accDetailModel.REMARK ;
                         accDetailModel.INV_NO ;
                         accDetailModel.PRO_NO ;
                         accDetailModel.REMARK_INVOICE ;
                         accDetailModel.ISPLAN ;


                         */

                        getAccList.Add(accDetailModel);
                        accruedList.AccruedItemList.Add(accDetailModel);

                        #endregion
                        i++;
                    }
                    accList.Add(accruedList);


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return getAccList;
        }
        private List<AccruedViewModel> GetPaymentItemsList(string companyCode, int month, int year)
        {
            List<AccruedViewModel> getAccList = new List<AccruedViewModel>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var getPaymentItemList = (from data in context.PAYMENT_ITEMS where data.IS_ACTIVE == true && data.COMPANY_CODE == companyCode orderby data.ID select data).ToList();
                    var getcttList = (from data in context.COST_CENTER where data.COMPANY_CODE == companyCode select data).ToList();

                    var getFeeInvList = (from data in context.FEE_INVOICE where data.COMPANY_CODE == companyCode && data.INV_MONTH == month && data.INV_YEAR == year select data).ToList();



                    int i = 1;
                    foreach (var item in getPaymentItemList)
                    {
                        AccruedViewModel accruedList = new AccruedViewModel();

                        #region accrued
                        accruedList.ACCRUED_ID = 0;
                        //accruedList.ACCRUED_NO = "Acc" + companyCode + DateTime.Now.Date.ToString("ddmmyyyy");
                        accruedList.PERIOD_MONTH = month;
                        accruedList.PERIOD_YEAR = year;
                        accruedList.COMPANY_CODE = companyCode;

                        accruedList.TOTAL_AMT = 0;//wait calculate item
                        accruedList.MODIFIED_BY = "";//wait
                        accruedList.MODIFIED_DATE = null;//wait
                        accruedList.CREATE_BY = "";//wait
                        accruedList.CREATE_DATE = DateTime.Now;
                        accruedList.APPROVED_BY = ConstantVariableService.APPROVERID;
                        accruedList.APPROVED_DATE = null;

                        //accruedList.REMARK = "";



                        accruedList.FormAction = ConstantVariableService.FormActionCreate;
                        accruedList.FormState = ConstantVariableService.FormStateCreate;
                        #endregion

                        var accDetailModel = new AccruedDetailViewModel();

                        #region item
                        //accDetailModel.ACCRUED_ITEM_NO = accruedList.ACCRUED_NO + "_" + i.ToString();
                        accDetailModel.ACCRUED_ID = accruedList.ACCRUED_ID;
                        accDetailModel.INV_NO = "";//wait
                        accDetailModel.PRO_NO = "";//wait
                        accDetailModel.SEQUENCE = i;
                        accDetailModel.ACCRUED_MONTH = month;
                        accDetailModel.ACCRUED_YEAR = year;
                        accDetailModel.PERIOD_ACCRUED = month.ToString().PadLeft(2, '0') + "/" + year.ToString();
                        accDetailModel.INV_MONTH = month;
                        accDetailModel.INV_YEAR = year;
                        accDetailModel.CCT_CODE = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;

                        accDetailModel.COMPANY_CODE = companyCode;
                        accDetailModel.PAYMENT_ITEMS_CODE = item.PAYMENT_ITEMS_CODE;
                        accDetailModel.TRANSACTIONS = 0;
                        accDetailModel.AMOUNT = 0;
                        accDetailModel.INV_AMOUNT = 0;
                        accDetailModel.MODIFIED_BY = "";
                        accDetailModel.MODIFIED_DATE = null;
                        accDetailModel.CREATE_BY = "";
                        accDetailModel.CREATE_DATE = DateTime.Now.Date;
                        accDetailModel.CURRENCY = "THB";
                        accDetailModel.REMARK_INVOICE = "";//wait
                        accDetailModel.ISPLAN = null;//wait
                        accDetailModel.EDITION = 0;//wait
                        accDetailModel.REMARK = "";//wait
                        #endregion

                        //accDetailModel.GLAccount = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                        //accDetailModel.CostCenter = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;

                        accruedList.AccruedItemList.Add(accDetailModel);


                        //accruedList.SEQUENCE = i;
                        //accruedList.PERIOD_ACCRUED = month.ToString().PadLeft(2, '0') + "/" + year.ToString();
                        //accruedList.GLAccount = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().GL_ACCOUNT;
                        //accruedList.CostCenter = getcttList.Where(m => m.CCT_CODE == item.CCT_CODE).FirstOrDefault().COST_CENTER1;
                        //accruedList.PAYMENT_ITEMS_NAME = item.PAYMENT_ITEMS_NAME;

                        //accruedList.INV_AMOUNT = i;
                        //accruedList.TRANSACTIONS = i;
                        //accruedList.AMOUNT = i;
                        //accruedList.CURRENCY = "THB";
                        //accruedList.EDITION = 0;
                        //accruedList.REMARKS = "";


                        getAccList.Add(accruedList);



                        i++;


                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return getAccList;
        }
        private string getNameStatus(bool? isChecked)
        {
            string status = "Pending";
            try
            {
                if (isChecked != null)
                {
                    status = (isChecked ?? null) == true ? "Completed" : "Waiting";
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
                List<AccruedViewModel> paymentitemsList = GetPaymentItemsList();

                dataList = DatatablesService.ConvertObjectListToDatatables<AccruedViewModel>(paymentitemsList);

                return dataList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private List<AccruedViewModel> GetPaymentItemsList()
        {
            throw new NotImplementedException();
        }

        public override AccruedViewModel NewFormData()
        {
            AccruedViewModel model = new AccruedViewModel();
            User user = UserService.GetSessionUserInfo();
            try
            {

                model.PERIOD_MONTH = DateTime.Today.Month;//DateTime.Today.AddMonths(1).Month;
                model.PERIOD_YEAR = DateTime.Today.Year;//DateTime.Today.Month == 12 ? DateTime.Today.AddYears(1).Year : DateTime.Today.Year;
                model.COMPANY_CODE = "";
                model.CREATE_BY = user.UserCode;
                model.CREATE_DATE = DateTime.Now;
                model.APPROVED_BY = ConstantVariableService.APPROVERID;
                model.APPROVED_DATE = DateTime.Now;
                model.UPLOAD_TYPE = false;
                model.MODIFIED_BY = user.UserCode;
                model.MODIFIED_DATE = DateTime.Now;


                var list = new List<SelectListItem>();
                using (var context = new PYMFEEEntities())
                {
                    var getCompanyList = (from data in context.COMPANies where data.IsPaymentFee == true orderby data.Bussiness_Unit select data).ToList();
                    list.AddRange(getCompanyList.Select(com => new SelectListItem { Value = com.BAN_COMPANY.ToString(), Text = com.COMPANY_NAME_EN.ToString() }).ToList());
                    model.CompanyLst = list;

                    var userApp = (from m in context.USERS where m.GROUP_POSITION == "Approved" select m).ToList();
                    model.UserApprovedList = userApp.Select(m => new SelectListItem
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

        public override ValidationResult SaveCreate(AccruedViewModel formData, ModelStateDictionary modelState)
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
                    var entFeeAccrued = new FEE_ACCRUED_PLAN();
                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entFeeAccrued);

                    //if (string.IsNullOrEmpty(formData.INV_APPROVED_BY ))
                    //{
                    //    entFeeInv.IS_STATUS = false;
                    //}
                    //else
                    //{
                    //    entFeeInv.IS_STATUS = true;
                    //}

                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {
                        context.FEE_ACCRUED_PLAN.Add(entFeeAccrued);
                        context.SaveChanges();

                        //Save Item
                        //Copy data from viewmodel to model - for line item
                        int sequence = 1;

                        foreach (var item in formData.AccruedItemList)
                        {
                            // item.ISPLAN = false;
                            if (item.DeleteFlag)
                            {
                                continue;
                            }

                            var entFeeAccruedItem = new FEE_ACCRUED_PLAN_ITEM();
                            item.ACCRUED_ID = entFeeAccrued.ACCRUED_ID;
                            item.SEQUENCE = sequence;
                            //item.ACCRUED_ITEM_ID = entFeeAccrued.ac + "_" + sequence;

                            MVMMappingService.MoveData(item, entFeeAccruedItem);
                            context.FEE_ACCRUED_PLAN_ITEM.Add(entFeeAccruedItem);
                            context.SaveChanges();

                            sequence++;

                            #region Accrtued Item Sub
                            int subsequence = 1;
                            foreach (var sub in item.AccruedItemSubList)
                            {
                                var entfeeAccruedSub = new FEE_ACCRUED_PLAN_ITEM_SUB();
                                sub.ACCRUED_ITEM_ID = entFeeAccruedItem.ACCRUED_ITEM_ID;
                                sub.SEQUENCE = subsequence;
                                MVMMappingService.MoveData(sub, entfeeAccruedSub);

                                subsequence++;

                                context.FEE_ACCRUED_PLAN_ITEM_SUB.Add(entfeeAccruedSub);
                                context.SaveChanges();
                            }
                            #endregion

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

        public override ValidationResult SaveDelete(AccruedViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    var entfeeAccrued = new FEE_ACCRUED_PLAN();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entfeeAccrued);

                    using (var context = new PYMFEEEntities())
                    {
                        //Delete header            
                        context.Entry(entfeeAccrued).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();


                        //Delete item
                        context.FEE_ACCRUED_PLAN_ITEM.RemoveRange(context.FEE_ACCRUED_PLAN_ITEM.Where(m => m.ACCRUED_ID == entfeeAccrued.ACCRUED_ID));
                        context.SaveChanges();

                        var entAccruedItem = (from m in context.FEE_ACCRUED_PLAN_ITEM
                                              where m.ACCRUED_ID == entfeeAccrued.ACCRUED_ID
                                              select m).ToList();


                        //Delete item Sub
                        context.FEE_ACCRUED_PLAN_ITEM_SUB.RemoveRange(context.FEE_ACCRUED_PLAN_ITEM_SUB.Where(m => entAccruedItem.Any(i => m.ACCRUED_ITEM_ID == i.ACCRUED_ITEM_ID)));
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

        public override ValidationResult SaveEdit(AccruedViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            var userInfo = UserService.GetSessionUserInfo();
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
                    FEE_ACCRUED_PLAN entfeeAccrued = new FEE_ACCRUED_PLAN();



                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, entfeeAccrued);


                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {

                        context.Entry(entfeeAccrued).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        //Save Item                        
                        int sequence = 1;
                        var connfeeAccruedItem = new List<FEE_ACCRUED_PLAN_ITEM>();
                        using (var conn = new PYMFEEEntities())
                        {
                            connfeeAccruedItem = (from m in conn.FEE_ACCRUED_PLAN_ITEM where m.ACCRUED_ID == entfeeAccrued.ACCRUED_ID select m).ToList();
                        }
                        foreach (var ent in connfeeAccruedItem.Where(n => formData.AccruedItemList.All(m => n.ACCRUED_ITEM_ID != m.ACCRUED_ITEM_ID)).ToList())
                        {

                            var entfeeAccruedItem = new FEE_ACCRUED_PLAN_ITEM();
                            ent.ACCRUED_ID = entfeeAccrued.ACCRUED_ID;
                            ent.SEQUENCE = sequence;
                            MVMMappingService.MoveData(ent, entfeeAccruedItem);
                            if (entfeeAccruedItem.ACCRUED_ITEM_ID != 0)
                            {
                                //Delete item
                                context.Entry(entfeeAccruedItem).State = System.Data.Entity.EntityState.Deleted;
                                context.SaveChanges();
                                //Delete item sub
                                context.FEE_ACCRUED_PLAN_ITEM_SUB.RemoveRange(context.FEE_ACCRUED_PLAN_ITEM_SUB.Where(m => m.ACCRUED_ITEM_ID == entfeeAccruedItem.ACCRUED_ITEM_ID));

                            }
                            context.SaveChanges();
                        }

                        foreach (var item in formData.AccruedItemList)
                        {
                            // item.DeleteFlag = !chkexist.Any() ? true : false;
                            var entfeeAccruedItem = new FEE_ACCRUED_PLAN_ITEM();
                            item.ACCRUED_ID = entfeeAccrued.ACCRUED_ID;
                            item.SEQUENCE = sequence;
                            //item.ACCRUED_ITEM_NO = entfeeAccrued.ACCRUED_NO + "_" + sequence;
                            MVMMappingService.MoveData(item, entfeeAccruedItem);
                            if (item.DeleteFlag)
                            {
                                if (entfeeAccruedItem.ACCRUED_ITEM_ID != 0)
                                {
                                    //Delete item
                                    context.Entry(entfeeAccruedItem).State = System.Data.Entity.EntityState.Deleted;
                                    context.SaveChanges();
                                    //Delete item sub
                                    context.FEE_ACCRUED_PLAN_ITEM_SUB.RemoveRange(context.FEE_ACCRUED_PLAN_ITEM_SUB.Where(m => m.ACCRUED_ITEM_ID == entfeeAccruedItem.ACCRUED_ITEM_ID));

                                }
                                context.SaveChanges();
                            }
                            else
                            {
                                #region Accrtued Item Sub
                                int subsequence = 1;
                                foreach (var sub in item.AccruedItemSubList)
                                {
                                    var entfeeAccruedSub = new FEE_ACCRUED_PLAN_ITEM_SUB();
                                    sub.ACCRUED_ITEM_ID = entfeeAccruedItem.ACCRUED_ITEM_ID;
                                    sub.SEQUENCE = subsequence;
                                    MVMMappingService.MoveData(sub, entfeeAccruedSub);

                                    subsequence++;
                                    if (entfeeAccruedSub.ID != 0)
                                    {
                                        context.Entry(entfeeAccruedSub).State = System.Data.Entity.EntityState.Modified;
                                    }
                                    else
                                    {
                                        context.FEE_ACCRUED_PLAN_ITEM_SUB.Add(entfeeAccruedSub);
                                    }
                                    context.SaveChanges();
                                }
                                #endregion


                                entfeeAccruedItem.SEQUENCE = sequence;
                                sequence++;
                                if (entfeeAccruedItem.ACCRUED_ITEM_ID != 0)
                                {
                                    context.Entry(entfeeAccruedItem).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    context.FEE_ACCRUED_PLAN_ITEM.Add(entfeeAccruedItem);
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

        public override ValidationResult ValidateFormData(AccruedViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            try
            {
                if (!modelState.IsValid)
                {
                    result.ModelStateErrorList = UtilityService.GetModelStateErrors(modelState);
                    result.ErrorFlag = true;
                }
                else
                {
                    //Get item => deleteFlag != true
                    var itemList = formData.AccruedItemList.Where(m => m.DeleteFlag != true).ToList();
                    if (itemList == null || !itemList.Any()) //Check list is null or empty
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.cannot_found_gl_detail)));

                        result.ErrorFlag = true;
                    }
                    else
                    {
                        if (string.Equals(formData.FormAction, ConstantVariableService.FormActionEdit, StringComparison.OrdinalIgnoreCase))
                        {
                            //if (((formData.PERIOD_YEAR - DateTime.Now.Year) * 12) + formData.PERIOD_MONTH - DateTime.Now.Month < 0)
                            //{
                            //    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.cannot_action_error) + " เกินช่วงเวลาการทำจ่ายแล้ว"));

                            //    result.ErrorFlag = true;
                            //}
                            using (var contex = new PYMFEEEntities())
                            {
                                var ent_accrued = (from m in contex.FEE_ACCRUED_PLAN
                                                   where m.ACCRUED_ID != formData.ACCRUED_ID && m.PERIOD_MONTH == formData.PERIOD_MONTH && m.PERIOD_YEAR == formData.PERIOD_YEAR && m.COMPANY_CODE == formData.COMPANY_CODE
                                                   select m).ToList();
                                if (ent_accrued.Any())
                                {
                                    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, string.Concat(ResourceText.PERIOD_MONTH, " /", ResourceText.PERIOD_YEAR, " /", ResourceText.COMPANY_CODE))));

                                    result.ErrorFlag = true;
                                }
                            }
                        }
                        if (string.Equals(formData.FormAction, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
                        {
                            //if (((formData.PERIOD_YEAR - DateTime.Now.Year) * 12) + formData.PERIOD_MONTH - DateTime.Now.Month < 0)
                            //{
                            //    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.cannot_action_error) + " เกินช่วงเวลาการทำจ่ายแล้ว"));

                            //    result.ErrorFlag = true;
                            //}
                            using (var contex = new PYMFEEEntities())
                            {
                                var ent_accrued = (from m in contex.FEE_ACCRUED_PLAN
                                                   where m.PERIOD_MONTH == formData.PERIOD_MONTH && m.PERIOD_YEAR == formData.PERIOD_YEAR && m.COMPANY_CODE == formData.COMPANY_CODE
                                                   select m).ToList();
                                if (ent_accrued.Any())
                                {
                                    result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.duplicate_error, string.Concat(ResourceText.PERIOD_MONTH, " /", ResourceText.PERIOD_YEAR, " /", ResourceText.COMPANY_CODE))));

                                    result.ErrorFlag = true;
                                }
                            }

                        }
                        if (formData.TOTAL_AMT == 0 || itemList.All(m => m.INV_AMOUNT == 0))
                        {
                            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.cannot_found, "จำนวนเงินค่าใช้จ่าย")));

                            result.ErrorFlag = true;
                        }
                        ////Check item
                        //int line = 1;
                        //foreach (var item in itemList)
                        //{
                        //    if ((item.RATE_AMT ?? 0) == 0 && (item.ACTUAL_AMOUNT ?? 0) == 0 && (item.RATE_TRANS ?? 0) == 0 && (item.TRANSACTIONS ?? 0) == 0)

                        //    {
                        //        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.TRANSACTION, line.ToString())));
                        //        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.RATE_TRANS, line.ToString())));
                        //        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.ACTUAL_AMOUNT, line.ToString())));
                        //        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.RATE_AMT, line.ToString())));
                        //        result.ErrorFlag = true;
                        //    }
                        //    //product id not empty
                        //    if (item.PAYMENT_ITEMS_FEE_ITEM == null)
                        //    {
                        //        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.PAYMENT_ITEMS_FEE_ITEM, line.ToString())));

                        //        result.ErrorFlag = true;
                        //    }

                        //    if ((item.RATE_TRANS ?? 0) != 0 && (item.TRANSACTIONS ?? 0) == 0)
                        //    {
                        //        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.TRANSACTION, line.ToString())));

                        //        result.ErrorFlag = true;
                        //    }
                        //    if ((item.RATE_AMT ?? 0) != 0 && (item.ACTUAL_AMOUNT ?? 0) == 0)
                        //    {
                        //        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_seq_at_error, ResourceText.ACTUAL_AMOUNT, line.ToString())));

                        //        result.ErrorFlag = true;
                        //    }

                        //    var dupPaymentItem = itemList.GroupBy(m => m.PAYMENT_ITEMS_FEE_ITEM).Where(m => m.Count() > 1).ToList();
                        //    foreach (var itemDup in dupPaymentItem)
                        //    {
                        //        if (item.PAYMENT_ITEMS_FEE_ITEM == itemDup.Key)
                        //        {
                        //            result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_sequence_duplicate_error, ResourceText.PAYMENT_ITEMS_FEE_ITEM, line.ToString())));
                        //            result.ErrorFlag = true;
                        //        }
                        //    }

                        //    line++;
                        //}
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