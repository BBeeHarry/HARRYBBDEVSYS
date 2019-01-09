using BBDEVSYS.Models.Entities;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.AccruedReport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Services.Home
{
    public class HomeService
    {
        public string GetList()
        {
            try
            {
                string dataList = "";
                List<AccruedDetailReportViewModel> invList = GetInvoiceList();

                dataList = DatatablesService.ConvertObjectListToDatatables<AccruedDetailReportViewModel>(invList);

                return dataList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<AccruedDetailReportViewModel> GetInvoiceList()
        {
            List<AccruedDetailReportViewModel> modelList = new List<AccruedDetailReportViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {

                    var ent_company = (from m in context.COMPANies where m.IsPaymentFee == true select m).ToList();

                    var ent_invList = (from m in context.FEE_INVOICE orderby m.INV_MONTH, m.INV_YEAR select m).ToList();

                    //var ent_invItemList = (from m in context.FEE_INVOICE_ITEM orderby m.INV_MONTH, m.INV_YEAR select m).ToList();

                    var ent_pymList = (from m in context.PAYMENT_ITEMS where m.IS_ACTIVE == true  orderby m.GROUP_SEQ_CHANNELS select m).ToList();

                    //var ent_pymChargeList = (from m in context.PAYMENT_ITEMS_CHAGE select m).ToList();


                    string getComp = "TI";

                    var get_invList = (from m in ent_invList where m.INV_MONTH >= 1 && m.INV_MONTH <= DateTime.Now.Date.Month && m.COMPANY_CODE == getComp select m).ToList();

                    ent_pymList = ent_pymList.Where(m => m.COMPANY_CODE == getComp).ToList();

                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);
                    var get_month = (DateTime.Now.Date.Year * 12 + DateTime.Now.Date.Month) - (DateTime.Now.Date.Year * 12 + 1);

                    foreach (var item in ent_pymList)
                    {
                       
                        var data_inv = get_invList.Where(m => m.PAYMENT_ITEMS_CODE == item.PAYMENT_ITEMS_CODE).ToList();

                        var model = new AccruedDetailReportViewModel();
                        model.FEE = "Inv. No.";
                        for (int i = 0; i <= get_month; i++)
                        {
                            string inv = "";
                            //Convert.ToString(string.Format("{0:#,##0.####}", amt)));
                            var data_invMonth = data_inv.Where(m => m.INV_MONTH == i + 1 && m.INV_YEAR == DateTime.Now.Date.Year).FirstOrDefault();
                            inv = data_invMonth == null ? "" : data_invMonth.INV_NO;
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];

                            model.GetType().GetProperty(monthIndex).SetValue(model, inv);

                        }
                        model.CHARGE = "1";
                        modelList.Add(model);

                        model = new AccruedDetailReportViewModel();
                        model.FEE = "Inv. Pro.";
                        for (int i = 0; i <= get_month; i++)
                        {
                            string pro = "";
                            //Convert.ToString(string.Format("{0:#,##0.####}", amt)));
                            var data_invMonth = data_inv.Where(m => m.INV_MONTH == i + 1 && m.INV_YEAR == DateTime.Now.Date.Year).FirstOrDefault();
                            pro = data_invMonth == null ? "" : data_invMonth.PRO_NO;
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];

                            model.GetType().GetProperty(monthIndex).SetValue(model, pro);

                        }
                        model.CHARGE = "2";
                        modelList.Add(model);

                        model = new AccruedDetailReportViewModel();
                        model.FEE = item.PAYMENT_ITEMS_NAME;
                        for (int i = 0; i <= get_month; i++)
                        {
                            decimal amt = 0;
                            //Convert.ToString(string.Format("{0:#,##0.####}", amt)));
                            var data_invMonth = data_inv.Where(m => m.INV_MONTH == i + 1 && m.INV_YEAR == DateTime.Now.Date.Year).FirstOrDefault();
                            amt = data_invMonth == null ?0 :( data_invMonth.NET_AMOUNT??0);
                            string monthIndex = dateTimeInfo.AbbreviatedMonthNames[i];

                            model.GetType().GetProperty(monthIndex).SetValue(model, Convert.ToString(string.Format("{0:#,##0.####}", amt)));

                        }
                        model.CHARGE = "3";
                        modelList.Add(model);

                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return modelList;
        }
    }
}