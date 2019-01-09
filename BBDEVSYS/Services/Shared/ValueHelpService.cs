using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BBDEVSYS.Models.Entities;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.ViewModels.Shared;
using BBDEVSYS.Services.Shared;
using System.Configuration;

namespace BBDEVSYS.Services.Shared
{
    public class ValueHelpService
    {

        public static List<ValueHelpViewModel> GetValueHelp(string valueType, string status = "")
        {
            List<ValueHelpViewModel> valueHelpList = new List<ValueHelpViewModel>();

            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    status = ConstantVariableService.ConfigStatusActive; ;
                }

                using (var context = new PYMFEEEntities())
                {
                    IOrderedQueryable<ValueHelp> valueHelps;

                    if (string.Equals(valueType, "ALL", StringComparison.OrdinalIgnoreCase))
                    {
                        valueHelps = (from m in context.ValueHelps
                                      where m.Status == status
                                      select m).OrderBy(m => m.Sequence);
                    }
                    else
                    {
                        valueHelps = (from m in context.ValueHelps
                                      where m.ValueType == valueType
                                      && m.Status == status
                                      select m).OrderBy(m => m.Sequence);
                    }


                    foreach (var value in valueHelps)
                    {
                        ValueHelpViewModel valueHelp = new ValueHelpViewModel();
                        valueHelp.ID = value.ID;
                        valueHelp.ValueKey = value.ValueKey;
                        valueHelp.ValueText = value.ValueText;
                        valueHelp.ValueType = value.ValueType;
                        valueHelp.Status = value.Status;

                        valueHelpList.Add(valueHelp);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return valueHelpList;
        }

        public static ValueHelpViewModel GetValueHelpText(string valueType, string valueKey, string status = "")
        {
            ValueHelpViewModel valueHelp = new ValueHelpViewModel();

            try
            {
                valueHelp = ValueHelpService.GetValueHelp(valueType, status).Single(m => m.ValueKey == valueKey);
            }
            catch (Exception ex)
            {

            }

            return valueHelp;
        }
        public static ValueHelpViewModel GetValueHelpTextDis(string valueType, string valueText, string status = "")
        {
            ValueHelpViewModel valueHelp = new ValueHelpViewModel();

            try
            {
                valueHelp = ValueHelpService.GetValueHelp(valueType, status).Single(m => m.ValueText == valueText);
            }
            catch (Exception ex)
            {

            }

            return valueHelp;
        }




    }
}