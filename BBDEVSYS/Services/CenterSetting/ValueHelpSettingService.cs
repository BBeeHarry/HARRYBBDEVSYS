using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Abstract;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.CenterSetting;
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Services.CenterSetting
{
    public class ValueHelpSettingService : AbstractControllerService<ValueHelpSettingViewModel>
    {
        public override string GetList()
        {
            string dataList = "";
            List<ValueHelpSettingViewModel> valueHelp = GetValueHelpList();
            dataList = DatatablesService.ConvertObjectListToDatatables<ValueHelpSettingViewModel>(valueHelp);
            return dataList;
        }
        public override ValueHelpSettingViewModel GetDetail(int id)
        {
            ValueHelpSettingViewModel valueSetting = new ValueHelpSettingViewModel();
            var userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    ValueHelp _valHelp = new ValueHelp();
                    _valHelp = context.ValueHelps.Where(m =>
                    m.ID == id).OrderBy(a => a.Sequence).FirstOrDefault();
                    MVMMappingService.MoveData(_valHelp, valueSetting);
                    valueSetting.ValueHelpText = valueSetting.ValueText;
                    //Get line item
                    List<ValueHelp> valueItemList = (from m in context.ValueHelps
                                                                 where m.ValueType == valueSetting.ValueKey
                                                                 select m).OrderBy(m => m.Sequence).ToList();
                    valueSetting.CREATE_BY = userInfo.UserCode;
                    valueSetting.CREATE_DATE = DateTime.Now.Date;
                    foreach (var item in valueItemList)
                    {
                        ValueHelpSettingItemViewModel valueItem = new ValueHelpSettingItemViewModel();
                        MVMMappingService.MoveData(item, valueItem);
                        valueItem.StatusValueHelp = ValueHelpService.GetValueHelp("CONFIGSTATUS");
                        valueSetting.ValueHelpSettingList.Add(valueItem);
                        //Get line item

                    }


                }

            }
            catch (Exception ex)
            {

            }
            return valueSetting;

        }

        public List<ValueHelpSettingViewModel> GetValueHelpList()
        {

            List<ValueHelpSettingViewModel> valueHelp = new List<ValueHelpSettingViewModel>();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var _setValueHelp  = (from m in context.ValueHelps
                                      where m.ValueType == "AVAILUSERSETTING"
                                      select m).OrderBy(m => m.ValueText).ToList();
                    foreach (var item in _setValueHelp)
                    {
                        var _getValueHelp = new ValueHelpSettingViewModel();
                        MVMMappingService.MoveData(item, _getValueHelp);
                        valueHelp.Add(_getValueHelp);
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return valueHelp;
        }
        public override ValueHelpSettingViewModel NewFormData()
        {
            ValueHelpSettingViewModel valueHelpViewModel = new ValueHelpSettingViewModel();

            try
            {
               
            }
            catch (Exception ex)
            {

            }

            return valueHelpViewModel;
        }
     
        public override ValueHelpSettingViewModel InitialCopy(int fromID)
        {
            ValueHelpSettingViewModel valueHelpViewModel = GetDetail(fromID);
           

            return valueHelpViewModel;
        }
        public override ValidationResult SaveCreate(ValueHelpSettingViewModel formData, ModelStateDictionary modelState)
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


                    result.Message = ResourceText.SuccessfulSave;
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


    
        public override ValidationResult SaveEdit(ValueHelpSettingViewModel formData, ModelStateDictionary modelState)
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
                    ValueHelp _entValue = new ValueHelp();

                    //Copy data from viewmodel to model - for header
                    MVMMappingService.MoveData(formData, _entValue);
                    //Save Header 
                    using (var context = new PYMFEEEntities())
                    {
                        //---context.Entry(_entValue).State = System.Data.Entity.EntityState.Modified;
                        //---context.SaveChanges();
                        //---_entValue = (from m in context.ValueHelps where m.ID == formData.ID select m).FirstOrDefault();
                        context.Entry(_entValue).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        //Save Item                        
                        int sequence = 1;
                        foreach (var item in formData.ValueHelpSettingList)
                        {
                            var _entValueItem = new ValueHelp();
                            item.ValueType = _entValue.ValueKey;
                            item.CREATE_BY = formData.CREATE_BY;
                            item.CREATE_DATE = formData.CREATE_DATE;
                            item.MODIFIED_BY = formData.MODIFIED_BY;
                            item.MODIFIED_DATE = formData.MODIFIED_DATE;
                            MVMMappingService.MoveData(item, _entValueItem);
                            if (item.DeleteFlag)
                            {
                                if (_entValueItem.ID != 0)
                                {
                                    context.Entry(_entValueItem).State = System.Data.Entity.EntityState.Deleted;
                                }
                            }
                            else
                            {

                                _entValueItem.Sequence = sequence;
                                sequence++;


                                if (_entValueItem.ID != 0)
                                {
                                    context.Entry(_entValueItem).State = System.Data.Entity.EntityState.Modified;
                                }
                                else
                                {
                                    context.ValueHelps.Add(_entValueItem);
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

        public override ValidationResult SaveDelete(ValueHelpSettingViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            try
            {

                
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
                result.ErrorFlag = true;
            }

            return result;
        }


        public override ValidationResult ValidateFormData(ValueHelpSettingViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            var _context = new PYMFEEEntities();
            try
            {
               
                if (!modelState.IsValid)
                {
                    result.ModelStateErrorList = UtilityService.GetModelStateErrors(modelState);
                    result.ErrorFlag = true;
                }
                else
                {
                    var valueHelpText ="ข้อมูล";

                    //Get item => deleteFlag != true
                    var itemList = formData.ValueHelpSettingList.Where(m => m.DeleteFlag != true).ToList();

                    if (itemList == null || !itemList.Any()) //Check list is null or empty
                    {
                        result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_listnull_error, valueHelpText)));

                        result.ErrorFlag = true;
                    }
                    else
                    {
                        //Check item
                        int line = 1;
                        foreach (var item in itemList)
                        {
                            //targetProduct/Group id not empty
                            if (string.IsNullOrEmpty( item.ValueKey ) )
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, "Key", line.ToString())));

                                result.ErrorFlag = true;
                            }
                            if (string.IsNullOrEmpty(item.ValueText )  )
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, ResourceText.Text, line.ToString())));

                                result.ErrorFlag = true;
                            }
                            if (string.IsNullOrEmpty(item.Status))
                            {
                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_notempty_error, ResourceText.STATUS, line.ToString())));

                                result.ErrorFlag = true;
                            }

                            line++;
                        }

                        //Check targetProduct
                        line = 1;
                        var valueSettingList = formData.ValueHelpSettingList.Where(m => m.DeleteFlag != true).ToList();
                        var dupvalueTextList = valueSettingList.GroupBy( m => m.ValueText ).Where(m => m.Count() > 1).ToList();
                        var dupvalueKeyList = valueSettingList.GroupBy(m => m.ValueKey).Where(m => m.Count() > 1).ToList();

                        foreach (var item in dupvalueTextList)
                        {
                            var dupID = item.Key;
                            if (!string.IsNullOrEmpty(dupID))
                            {
                                var detail = valueSettingList.FirstOrDefault(m => m.ValueText == dupID);

                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_line_duplicate_error, detail.ValueText, line.ToString())));
                                result.ErrorFlag = true;
                            }
                            line++;
                        }
                        line = 1;
                        foreach (var item in dupvalueKeyList)
                        {
                            var dupID = item.Key;
                            if (!string.IsNullOrEmpty(dupID))
                            {
                                var detail = valueSettingList.FirstOrDefault(m => m.ValueKey == dupID);

                                result.ModelStateErrorList.Add(new ModelStateError("", string.Format(ValidatorMessage.item_line_duplicate_error, detail.ValueKey, line.ToString())));
                                result.ErrorFlag = true;
                            }
                            line++;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                result.MessageType = ex.ToString();
                result.Message = ex.ToString();
            }

            return result;
        }

     
        public ValueHelpSettingItemViewModel InitialItem()
        {
            ValueHelpSettingItemViewModel valueItem = new ValueHelpSettingItemViewModel();

            try
            {

                valueItem.StatusValueHelp = ValueHelpService.GetValueHelp("CONFIGSTATUS");

            }
            catch (Exception ex)
            {

            }

            return valueItem;
        }

    }
}