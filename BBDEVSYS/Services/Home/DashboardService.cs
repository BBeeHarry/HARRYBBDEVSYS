
using BBDEVSYS.Models.Entities;
using BBDEVSYS.ViewModels.Dashboard;
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Services.Home
{
    public class DashboardService
    {
        public List<DahsboardBarchartViewModel> GetDataBarchart()
        {
            List<DahsboardBarchartViewModel> dashboardList = new List<DahsboardBarchartViewModel>();
            try
            {
                using (var contex = new PYMFEEEntities())
                {
                    var ent_inv = (from m in contex.FEE_INVOICE
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();

                    var max_acc = (from m in contex.FEE_ACCRUED_PLAN_ITEM
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();

                    var getmax_acc = (max_acc.Any() ? max_acc.Max(n => (n.ACCRUED_YEAR * 12) + n.ACCRUED_MONTH) : 0);
                    var ent_acc = (from m in contex.FEE_ACCRUED_PLAN_ITEM
                                   where ((m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH) == getmax_acc
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();
                    int monthS = 1;
                    int monthE = DateTime.Now.Month;

                    int yearS = 2018;
                    int yearE = DateTime.Now.Year;

                    int diffMonth = (((yearE * 12) + monthE) - ((yearS * 12) + monthS)) + 1;

                    int mnth = monthS;
                    int yr = yearS;
                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);

                  

                    for (int i = 1; i <= diffMonth; i++)
                    {
                        if (mnth == 13)
                        {
                            mnth = 1;
                            yr++;
                        }
                        var get_inv = ent_inv.Where(m => ((m.INV_YEAR * 12) + m.INV_MONTH) == ((yr * 12) + mnth)).ToList();
                        var get_acc = ent_acc.Where(m => ((m.INV_YEAR * 12) + m.INV_MONTH) == ((yr * 12) + mnth)).ToList();

                        decimal act = get_inv.Any() ? get_inv.Sum(s => (s.NET_AMOUNT ?? 0)) : 0;
                        decimal acc = get_acc.Any() ? get_acc.Sum(s => (s.INV_AMOUNT ?? 0)) : 0;


                        decimal sumall = act + acc;

                        decimal sumact = (sumall == 0) ? 0 : (act / sumall) * 100;
                        decimal sumacc = (sumall == 0) ? 0 : (acc / sumall) * 100;

                        sumact = Math.Round(sumact, 2, MidpointRounding.AwayFromZero);
                        sumacc = Math.Round(sumacc, 2, MidpointRounding.AwayFromZero);
                        DahsboardBarchartViewModel model = new DahsboardBarchartViewModel();
                        model.Actual = sumact;
                        model.Accrued = sumacc;
                        model.MonthNameFee = dateTimeInfo.AbbreviatedMonthNames[mnth-1] + yr.ToString().Substring(2, 2);
                        //CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mnth)+yr.ToString().Substring(2,2);
                        model.MonthFee = 100;

                       
                        dashboardList.Add(model);
                       
                        mnth++;
                    }


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dashboardList;
        }

        public List<DahsboardBarchartViewModel> GetDataBarchartChannels()
        {
            List<DahsboardBarchartViewModel> dashboardList = new List<DahsboardBarchartViewModel>();
            try
            {
                using (var contex = new PYMFEEEntities())
                {
                    var ent_inv = (from m in contex.FEE_INVOICE
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();

                    var max_acc = (from m in contex.FEE_ACCRUED_PLAN_ITEM
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();

                    var ent_pym = (from m in contex.PAYMENT_ITEMS
                                   select m).ToList();
                    var get_pym = ent_pym.GroupBy(g => g.PAYMENT_ITEMS_NAME).ToList();


                    var getmax_acc = (max_acc.Any() ? max_acc.Max(n => (n.ACCRUED_YEAR * 12) + n.ACCRUED_MONTH) : 0);
                    var ent_acc = (from m in contex.FEE_ACCRUED_PLAN_ITEM
                                   where ((m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH) == getmax_acc
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();
                    int monthS = 1;
                    int monthE = DateTime.Now.Month;

                    int yearS = 2018;
                    int yearE = DateTime.Now.Year;

                    int diffMonth = (((yearE * 12) + monthE) - ((yearS * 12) + monthS)) + 1;

                    int mnth = monthS;
                    int yr = yearS;
                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);


                    foreach (var item in get_pym)
                    {
                        var get_pymLst = ent_pym.Where(p => p.PAYMENT_ITEMS_NAME == item.Key).ToList();

                        //for (int i = 1; i <= diffMonth; i++)
                        //{
                        //    if (mnth == 13)
                        //    {
                        //        mnth = 1;
                        //        yr++;
                        //    }
                            var get_inv = ent_inv.Where(m => get_pymLst.Any(o=>m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE) 
                            //&&((m.INV_YEAR * 12) + m.INV_MONTH) == ((yr * 12) + mnth)
                            ).ToList();

                            var get_acc = ent_acc.Where(m => get_pymLst.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE) 
                            //&&((m.INV_YEAR * 12) + m.INV_MONTH) == ((yr * 12) + mnth)
                            ).ToList();

                            decimal act = get_inv.Any() ? get_inv.Sum(s => (s.NET_AMOUNT ?? 0)) : 0;
                            decimal acc = get_acc.Any() ? get_acc.Sum(s => (s.INV_AMOUNT ?? 0)) : 0;


                            decimal sumall = act + acc;

                            decimal sumact = (sumall == 0) ? 0 : (act / sumall) * 100;
                            decimal sumacc = (sumall == 0) ? 0 : (acc / sumall) * 100;

                            sumact = Math.Round(sumact, 2, MidpointRounding.AwayFromZero);
                            sumacc = Math.Round(sumacc, 2, MidpointRounding.AwayFromZero);
                            DahsboardBarchartViewModel model = new DahsboardBarchartViewModel();

                            model.PriceCatalog = item.Key;

                            model.Actual = sumact;
                            model.Accrued = sumacc;
                            //model.MonthNameFee = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yr.ToString().Substring(2, 2);
                            //CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mnth)+yr.ToString().Substring(2,2);
                            model.MonthFee = 100;


                            dashboardList.Add(model);

                        //    mnth++;
                        //}
                    }


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dashboardList;
        }

        public List<DahsboardBarchartViewModel> GetDataBarchartCompany()
        {
            List<DahsboardBarchartViewModel> dashboardList = new List<DahsboardBarchartViewModel>();
            try
            {
                using (var contex = new PYMFEEEntities())
                {
                    var ent_inv = (from m in contex.FEE_INVOICE
                                   //where m.IS_STATUS != "3"
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();

                    var max_acc = (from m in contex.FEE_ACCRUED_PLAN_ITEM
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();

                    var ent_pym = (from m in contex.PAYMENT_ITEMS
                                   select m).ToList();


                    var getmax_acc = (max_acc.Any() ? max_acc.Max(n => (n.ACCRUED_YEAR * 12) + n.ACCRUED_MONTH) : 0);
                    var ent_acc = (from m in contex.FEE_ACCRUED_PLAN_ITEM
                                   where ((m.ACCRUED_YEAR * 12) + m.ACCRUED_MONTH) == getmax_acc
                                   orderby m.INV_MONTH, m.INV_YEAR
                                   select m).ToList();

                    var ent_comp = (from m in contex.COMPANies where m.IsPaymentFee==true &&m.BAN_COMPANY=="TI" select m).ToList();

                    int monthS = 1;
                    int monthE = DateTime.Now.Month;

                    int yearS = 2018;
                    int yearE = DateTime.Now.Year;

                    int diffMonth = (((yearE * 12) + monthE) - ((yearS * 12) + monthS)) + 1;

                    int mnth = monthS;
                    int yr = yearS;
                    var culture = CultureInfo.GetCultureInfo("en-US");
                    var dateTimeInfo = DateTimeFormatInfo.GetInstance(culture);


                    foreach (var com in ent_comp)
                    {
                        var _ent_pym = ent_pym.Where(m => m.COMPANY_CODE == com.BAN_COMPANY).ToList();
                        var _ent_inv= ent_inv.Where(m => m.COMPANY_CODE == com.BAN_COMPANY).ToList();
                        var _ent_acc= ent_acc.Where(m => m.COMPANY_CODE == com.BAN_COMPANY).ToList();

                        var get_pym = _ent_pym.GroupBy(g => g.PAYMENT_ITEMS_NAME).ToList();

                        foreach (var item in get_pym)
                        {
                            var get_pymLst = _ent_pym.Where(p => p.PAYMENT_ITEMS_NAME == item.Key).ToList();

                            //for (int i = 1; i <= diffMonth; i++)
                            //{
                            //    if (mnth == 13)
                            //    {
                            //        mnth = 1;
                            //        yr++;
                            //    }
                            var get_inv = _ent_inv.Where(m => get_pymLst.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)
                            //&&((m.INV_YEAR * 12) + m.INV_MONTH) == ((yr * 12) + mnth)
                            ).ToList();

                            var get_acc = _ent_acc.Where(m => get_pymLst.Any(o => m.PAYMENT_ITEMS_CODE == o.PAYMENT_ITEMS_CODE)
                            //&&((m.INV_YEAR * 12) + m.INV_MONTH) == ((yr * 12) + mnth)
                            ).ToList();

                            decimal act = get_inv.Any() ? get_inv.Sum(s => (s.NET_AMOUNT ?? 0)) : 0;
                            decimal acc = get_acc.Any() ? get_acc.Sum(s => (s.INV_AMOUNT ?? 0)) : 0;

                            decimal netact = _ent_inv.Any() ? _ent_inv.Sum(s => (s.NET_AMOUNT ?? 0)) : 0;
                            decimal netacc = _ent_acc.Any() ? _ent_acc.Sum(s => (s.INV_AMOUNT ?? 0)) : 0;

                            decimal sumall = act + acc;

                            decimal sumact = (netact == 0) ? 0 : (act / netact) * 100;// (sumall == 0) ? 0 : (act / sumall) * 100;
                            decimal sumacc = (netacc == 0) ? 0 : (acc / netacc) * 100;// (sumall == 0) ? 0 : (acc / sumall) * 100;

                            sumact = Math.Round(sumact, 2, MidpointRounding.AwayFromZero);
                            sumacc = Math.Round(sumacc, 2, MidpointRounding.AwayFromZero);
                            sumall = Math.Round(sumall, 2, MidpointRounding.AwayFromZero);
                            DahsboardBarchartViewModel model = new DahsboardBarchartViewModel();
                            model.Company = com.COMPANY_NAME_EN;
                            model.PriceCatalog = item.Key;

                            model.Actual = sumact;
                            model.Accrued = sumacc;
                            model.Total = sumall;
                            //model.MonthNameFee = dateTimeInfo.AbbreviatedMonthNames[mnth - 1] + yr.ToString().Substring(2, 2);
                            //CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mnth)+yr.ToString().Substring(2,2);
                            model.MonthFee = 100;


                            dashboardList.Add(model);

                            //    mnth++;
                            //}
                        }
                    }


                }//using
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dashboardList;
        }
    }
}