using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

using BBDEVSYS.Models.Entities;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Content.text;
using FluentValidation;
using System.Security.Principal;
using System.Web.Security;
using System.Web.Configuration;
using System.Text;
using System.Reflection;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.ComponentModel.DataAnnotations;

namespace BBDEVSYS.Services.Shared
{
    public static class UtilityService
    {
        //Constant Value
        private static readonly int DEFAULT_STRING_LENGHT = 10;

        //Get display name of model attribute
        public static string GetDisplayName<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> expression)
        {
            return ModelMetadata.FromLambdaExpression<TModel, TProperty>(
                expression,
                new ViewDataDictionary<TModel>(model)
                ).DisplayName;
        }

        public static string GetAttributeDisplayName(PropertyInfo property)
        {
            var atts = property.GetCustomAttributes(typeof(DisplayAttribute), true);

            if (atts.Length == 0)
                return "";

            return (atts[0] as DisplayAttribute).GetName();
        }

        //Get ModelState error message
        //public static List<ModelStateError> GetModelStateErrors(this ModelStateDictionary modelState)
        public static List<ModelStateError> GetModelStateErrors(ModelStateDictionary modelState)
        {
            var result = from ms in modelState
                         where ms.Value.Errors.Any()
                         let fieldKey = ms.Key
                         let errors = ms.Value.Errors
                         from error in errors
                         select new ModelStateError(fieldKey, error.ErrorMessage);

            return result.ToList();
        }
        public static int? GetColumnLength<T>(String columnName)
        {
            int? result = null;
            using (var context = new PYMFEEEntities())
            {
                var entType = typeof(T);
                var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                var items = objectContext.MetadataWorkspace.GetItems(DataSpace.CSpace);

                if (items == null)
                    return null;

                var q = items
                    .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                    .SelectMany(meta => ((EntityType)meta).Properties
                    .Where(p => p.Name == columnName && p.TypeUsage.EdmType.Name == "String"));

                var queryResult = q.Where(p =>
                {
                    var match = p.DeclaringType.Name == entType.Name;
                    if (!match)
                        match = entType.Name == p.DeclaringType.Name;

                    return match;

                })
                    .Select(sel => sel.TypeUsage.Facets["MaxLength"].Value)
                    .ToList();

                if (queryResult.Any())
                {
                    String checkMaxText = queryResult.First().ToString();
                    if (checkMaxText.ToUpper().IndexOf("MAX") >= 0)
                    {
                        result = 1000;
                    }
                    else
                    {
                        result = Convert.ToInt32(queryResult.First());
                    }
                }

                return result;
            }
        }

        public static void SetRuleForStringLength<T, TT>(AbstractValidator<T> validator)
        {

            var l = typeof(TT).GetProperties();

            foreach (var propertyInfoModel in typeof(TT).GetProperties().Where(m => m.PropertyType.FullName == "System.String"))
            {
                foreach (var propertyInfoViewModel in typeof(T).GetProperties().Where(p => p.Name == propertyInfoModel.Name))
                {
                    var parameterViewModel = Expression.Parameter(typeof(T), "p");
                    var propertyViewModel = Expression.Property(parameterViewModel, propertyInfoViewModel);
                    var conversionViewModel = Expression.Convert(propertyViewModel, typeof(String));
                    var lambdaViewModel = Expression.Lambda<Func<T, String>>(conversionViewModel, parameterViewModel);

                    var length = GetColumnLength<TT>(propertyInfoModel.Name);
                    var objectViewModel = Activator.CreateInstance(typeof(T));
                    validator.RuleFor(lambdaViewModel).Length(0, length ?? DEFAULT_STRING_LENGHT).WithLocalizedMessage(typeof(ValidatorMessage), "string_maxlength");
                }
            }
        }

        public static byte[] ConvertStringToBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ConvertBytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string GetCookieReferenceData()
        {
            string decodeString = "";

            try
            {
                IPrincipal user = HttpContext.Current.User;
                var formIdentity = (FormsIdentity)user.Identity;
                var userData = formIdentity.Ticket.UserData;
                var byteData = UtilityService.ConvertStringToBytes(userData);

                byte[] decodeData = MachineKey.Unprotect(byteData, WebConfigurationManager.AppSettings["UserdataSALT"]);
                decodeString = Encoding.UTF8.GetString(decodeData);
            }
            catch (Exception ex)
            {

            }


            return decodeString;
        }

        public static DateTime GetQuarterEndDate(short quarter, short year)
        {
            var qYear = year;
            var qMonth = (quarter * 3) + 1;
            if (qMonth > 12)
            {
                qMonth = 1;
                qYear = (short)(qYear + 1);
            }

            return new DateTime(qYear, qMonth, 1).AddDays(-1);
        }

        public static DateTime GetQuarterStartDate(short quarter, short year)
        {
            return GetQuarterEndDate(quarter, year).AddDays(1).AddMonths(-3);
        }

        public static List<short> GetPrevQuarter(short year, short quarter)
        {
            List<short> prevResult = new List<short>();
            try
            {
                short prevYear = 0, prevQuarter = 0;
                if (quarter == 1)
                {
                    prevQuarter = 4;
                    prevYear = (short)(year - 1);
                }
                else
                {
                    prevQuarter = (short)(quarter - 1);
                    prevYear = year;
                }

                prevResult.Add(prevYear);
                prevResult.Add(prevQuarter);
            }
            catch (Exception ex)
            {
                prevResult.Add(year);
                prevResult.Add(quarter);
            }
            return prevResult;
        }

        public static string GetPagetTitlePrefix(string formState)
        {
            string titlePrefix = "";

            if (string.Equals(formState, ConstantVariableService.FormStateCreate, StringComparison.OrdinalIgnoreCase))
            {
                titlePrefix = ResourceText.TitlePrefixCreate;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateDisplay, StringComparison.OrdinalIgnoreCase))
            {
                titlePrefix = ResourceText.TitlePrefixDisplay;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateEdit, StringComparison.OrdinalIgnoreCase))
            {
                titlePrefix = ResourceText.TitlePrefixEdit;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateCopy, StringComparison.OrdinalIgnoreCase))
            {
                titlePrefix = ResourceText.TitlePrefixCopy;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateDelete, StringComparison.OrdinalIgnoreCase))
            {
                titlePrefix = ResourceText.TitlePrefixDelete;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateList, StringComparison.OrdinalIgnoreCase))
            {
                titlePrefix = ResourceText.TitlePrefixList;
            }

            return titlePrefix;
        }

        //public static string GetDocumentNo(string processCode, short year = 0, bool onlyNumber = false)
        //{
        //    string documentNo = "";
        //    short yearSel = year;
        //    if (yearSel == 0 &&
        //        (!onlyNumber))
        //    {
        //        yearSel = Convert.ToInt16(DateTime.Now.Year);
        //    }

        //    try
        //    {
        //        var appDoc = new AppDocumentNo();
        //        using (var context = new PYMFEEEntities())
        //        {
        //            //disable detection of changes to improve performance
        //            context.Configuration.AutoDetectChangesEnabled = false;
        //            appDoc = context.AppDocumentNoes.Where(m => m.ProcessCode == processCode && m.Year == yearSel).FirstOrDefault();
        //            if (appDoc == null)
        //            {
        //                throw new Exception(String.Format(ValidatorMessage.cannot_found_processcode, processCode, yearSel.ToString()));
        //            }
        //        }

        //        using (var context = new PYMFEEEntities())
        //        {
        //            appDoc.LastRunningNo += 1;
        //            context.Entry(appDoc).State = System.Data.Entity.EntityState.Modified;
        //            //context.AppDocumentNoes.Attach(appDoc);

        //            //then perform the update
        //            context.SaveChanges();
        //        }
        //        if (onlyNumber)
        //        {
        //            if (appDoc != null)
        //            {
        //                var runningNo = appDoc.LastRunningNo ?? 1;
        //                var runningNoTxt = runningNo.ToString();
        //                documentNo = runningNoTxt.PadLeft(appDoc.RunningNoDigit ?? 0, '0');
        //            }
        //        }
        //        else
        //        {
        //            //Get Year Generate
        //            var yearStr = (yearSel + 543).ToString();
        //            var year2digi = yearStr.Substring(2, 2);
        //            //Return Current Document Number
        //            documentNo = appDoc.LastRunningNo.ToString();

        //            var numberFormat = new String('0', appDoc.RunningNoDigit ?? 0);
        //            documentNo = appDoc.DocumentNoPrefix + year2digi + (Convert.ToInt32(documentNo)).ToString(numberFormat);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return documentNo;
        //}
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
                                       System.Globalization.CultureInfo.CreateSpecificCulture("en-EN"));

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

        public static ValidationWithReturnResult<decimal> ConvertStringToDecimal(string text)
        {
            ValidationWithReturnResult<decimal> result = new ValidationWithReturnResult<decimal>();

            try
            {
                //Remove all comma
                text = text.Replace(",", "").Trim();
                result.ReturnResult = decimal.Parse(text);
            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString() + "/" + text, "");
            }

            return result;
        }

        public static ValidationWithReturnResult<short> ConvertStringToShort(string text)
        {
            ValidationWithReturnResult<short> result = new ValidationWithReturnResult<short>();

            try
            {
                //Remove all comma
                text = text.Replace(",", "").Trim();
                result.ReturnResult = short.Parse(text);
            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString() + "/" + text, "");
            }

            return result;
        }

        public static object GetObjectValue(object obj, string field)
        {
            var nameOfProperty = field;
            var propertyInfo = obj.GetType().GetProperty(nameOfProperty);
            var value = propertyInfo.GetValue(obj, null);
            return value;
        }

        public static string RenderPartialView(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, writer);
                viewResult.View.Render(viewContext, writer);

                return writer.GetStringBuilder().ToString();
            }
        }

        //public static List<CostCenterViewModel> GetCostCenterList()
        //{
        //    List<CostCenterViewModel> costCenterList = new List<CostCenterViewModel>();

        //    try
        //    {
        //        using (var context = new PYMFEEEntities())
        //        {
        //            var costCenter = (from m in context.CostCenters select m).ToList();
        //            foreach (var item in costCenter)
        //            {
        //                CostCenterViewModel costCenterViewModel = new CostCenterViewModel();

        //                MVMMappingService.MoveData(item, costCenterViewModel);

        //                costCenterViewModel.CodeDesc = item.Code + " " + item.Description;

        //                costCenterList.Add(costCenterViewModel);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return costCenterList;
        //}

        //public static List<Province> GetProvinceList()
        //{
        //    List<Province> provinceList = new List<Province>();

        //    try
        //    {
        //        using (var context = new PYMFEEEntities())
        //        {
        //            provinceList = (from m in context.Provinces select m).OrderBy(m => m.Name).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return provinceList;
        //}

        public static string ConvertToNumberFormat(decimal? number)
        {
            string numberFormat = "";

            try
            {
                if (number.HasValue)
                {
                    decimal value = number.Value;

                    if (value % 1 != 0)
                    {
                        numberFormat = value.ToString("#,##0.00");
                    }
                    else
                    {
                        numberFormat = value.ToString("#,##0");
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return numberFormat;
        }

        //public static MAX GetTax(string code, string type)
        //{
        //    var tax = new Max();
        //    var taxData = "";

        //    try
        //    {
        //        switch (type)
        //        {
        //            case ConstantVariableService.PayeeTypeAgent:
        //                using (var context = new PYMFEEEntities())
        //                {
        //                    var agentMaster = context.AgentMasters.Where(m => m.AgentCode == code).FirstOrDefault();
        //                    var agentInfo = context.AgentInfoes.Where(m => m.AgentCode == code).FirstOrDefault();
        //                    if (agentMaster != null)
        //                    {
        //                        taxData = agentMaster.TaxID;
        //                    }
        //                    if (agentInfo != null)
        //                    {
        //                        tax.Vendor = agentInfo.VendorCode;
        //                    }
        //                }
        //                break;
        //            case ConstantVariableService.PayeeTypeShop:
        //                using (var context = new PYMFEEEntities())
        //                {

        //                }
        //                break;
        //            case ConstantVariableService.PayeeTypeDeptstore:
        //                using (var context = new PYMFEEEntities())
        //                {

        //                }
        //                break;
        //            case ConstantVariableService.PayeeTypeVendor:
        //                using (var context = new PYMFEEEntities())
        //                {
        //                    var vendor = context.Vendors.Where(m => m.Code == code).FirstOrDefault();
        //                    if (vendor != null)
        //                    {
        //                        taxData = vendor.TaxID;
        //                        tax.Vendor = code;
        //                    }
        //                }
        //                break;
        //            case ConstantVariableService.PayeeTypeEmployee:
        //                using (var context = new PYMFEEEntities())
        //                {
        //                    var employee = context.HREmployees.Where(m => m.EmpNo == code).FirstOrDefault();
        //                    if (employee != null)
        //                    {
        //                        taxData = employee.IDCard;
        //                    }
        //                }
        //                break;
        //        }

        //        if (!string.IsNullOrEmpty(taxData))
        //        {
        //            var taxDataArr = taxData.Split('-');
        //            if (taxDataArr.Length > 1)
        //            {
        //                tax.TaxID = taxDataArr[0];
        //                tax.TaxCode = taxDataArr[1];

        //            }
        //            else
        //            {
        //                if (taxDataArr.Length == 1)
        //                {
        //                    tax.TaxID = taxDataArr[0];
        //                    tax.TaxCode = "00000";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    /*
        //        For Test 
        //    */
        //    //tax.TaxCode = "00003";
        //    return tax;
        //}

        public static string ConvertDateBDToSAPFormat(DateTime dateBD)
        {
            string result = "";
            try
            {
                int day = dateBD.Day;
                int month = dateBD.Month;
                int year = dateBD.Year;

                if (year > 2500)
                {
                    year = year - 543;
                }

                result = day.ToString() + "." + month.ToString() + "." + year.ToString();
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public static string ConvertDateToSAPDateFormat(DateTime date)
        {
            string result = "";
            try
            {
                int day = date.Day;
                int month = date.Month;
                int year = date.Year;

                if (year > 2500)
                {
                    year = year - 543;
                }

                result = year.ToString() + month.ToString().PadLeft(2, '0') + day.ToString().PadLeft(2, '0');
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        //public static bool CheckDayOff(DateTime keyDate, bool publicHoliday)
        //{
        //    bool flagDayOff = false;
        //    try
        //    {
        //        DateTime currentDate = DateTime.Now.Date;
        //        var dayOfHoliday = new Models.Database.HolidayCalendar();
        //        using (var context = new PYMFEEEntities())
        //        {
        //            dayOfHoliday = (from h in context.HolidayCalendars
        //                            where
        //                            h.HolidayDate == keyDate &&
        //                            h.PublicHoliday == publicHoliday
        //                            orderby
        //                            h.HolidayDate
        //                            select
        //              h).FirstOrDefault();

        //            if (keyDate.DayOfWeek == DayOfWeek.Saturday || keyDate.DayOfWeek == DayOfWeek.Sunday || dayOfHoliday != null)
        //            {
        //                flagDayOff = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //    return flagDayOff;

        //}
    }
}