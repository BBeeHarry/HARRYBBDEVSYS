using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Resources;
using System.Transactions;
using System.Web.Script.Serialization;

using BBDEVSYS.Models.Entities;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Abstract;
using BBDEVSYS.ViewModels.Authorization;
using BBDEVSYS.ViewModels.Shared;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.Content.text;

namespace BBDEVSYS.Services.Authorization
{
    public class AppCompositeRoleService : AbstractControllerService<AppCompositeRoleViewModel>
    {
        public override AppCompositeRoleViewModel GetDetail(int id)
        {
            AppCompositeRoleViewModel result = NewFormData();
            var userInfo = UserService.GetSessionUserInfo();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var appMenu = context.AppCompositeRoles.Where(m => m.ID == id).FirstOrDefault();
                    if (appMenu != null)
                    {
                        MVMMappingService.MoveData(appMenu, result);
                        var appItems = context.AppCompositeRoleItems.Where(m => m.CompositeRoleID == id).ToList();
                        if (appItems != null)
                        {
                            if (appItems.Count > 0)
                            {
                                var newAppMenuItems = new List<AppMenuViewModel>();
                                foreach (var appMenuItem in result.AppMenuItems)
                                {
                                    var roleDisplay = appItems.Where(m => m.RoleID == appMenuItem.RoleForDisplay).FirstOrDefault();
                                    if (roleDisplay != null)
                                    {
                                        appMenuItem.CheckRoleForDisplay = true;
                                    }

                                    var roleManage = appItems.Where(m => m.RoleID == appMenuItem.RoleForManage).FirstOrDefault();
                                    if (roleManage != null)
                                    {
                                        appMenuItem.CheckRoleForManage = true;
                                    }

                                    newAppMenuItems.Add(appMenuItem);
                                }

                                //Update Parent Check Box
                                newAppMenuItems = UpdateParentList(newAppMenuItems);

                            }
                        }
                    }
                }//End Context
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public override string GetList()
        {
            string dataList = "";
            List<AppCompositeRoleViewModel> appCompositeRoleViewModelList = GetAppCompositeRoleList();

            dataList = DatatablesService.ConvertObjectListToDatatables<AppCompositeRoleViewModel>(appCompositeRoleViewModelList);

            return dataList;
        }

        public List<AppCompositeRoleViewModel> GetAppCompositeRoleList()
        {
            List<AppCompositeRoleViewModel> result = new List<AppCompositeRoleViewModel>();
            var help = NewFormData();
            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var appCompositeRoles = context.AppCompositeRoles.ToList();

                    foreach (var appCompositeRole in appCompositeRoles)
                    {
                        AppCompositeRoleViewModel appCompositeRoleViewModel = new AppCompositeRoleViewModel();
                        MVMMappingService.MoveData(appCompositeRole, appCompositeRoleViewModel);
                        var statusHelp = help.StatusValueHelp.Where(m => m.ValueKey == appCompositeRole.Status).FirstOrDefault();
                        appCompositeRoleViewModel.StatusText = statusHelp.ValueText;
                        result.Add(appCompositeRoleViewModel);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public override AppCompositeRoleViewModel NewFormData()
        {
            AppCompositeRoleViewModel appCompositeRoleViewModel = new AppCompositeRoleViewModel();
            try
            {
                appCompositeRoleViewModel.StatusValueHelp = ValueHelpService.GetValueHelp("CONFIGSTATUS");
                appCompositeRoleViewModel.AppMenuItems = GetAppMenu();
             
            }
            catch (Exception ex)
            {

            }
            return appCompositeRoleViewModel;
        }

        public override ValidationResult SaveCreate(AppCompositeRoleViewModel formData, ModelStateDictionary modelState)
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
                    using (var context = new PYMFEEEntities())
                    {
                        var appCompositeRole = new AppCompositeRole();
                        MVMMappingService.MoveData(formData, appCompositeRole);
                        context.AppCompositeRoles.Add(appCompositeRole);
                        context.SaveChanges();

                        //Save Items
                        if (appCompositeRole.ID > 0)
                        {
                            List<string> roleList = new List<string>();
                            if (!string.IsNullOrEmpty(formData.AppMenuSelectedJSON))
                            {
                                roleList = GetRoleListFromJSON(formData.AppMenuSelectedJSON);
                                List<AppCompositeRoleItem> items = new List<AppCompositeRoleItem>();
                                foreach (var role in roleList)
                                {
                                    if (string.IsNullOrEmpty(role)) continue;

                                    var itemExist = items.Where(m => m.RoleID == role).FirstOrDefault();
                                    if (itemExist != null) continue;

                                    var item = new AppCompositeRoleItem();
                                    item.CompositeRoleID = appCompositeRole.ID;
                                    item.RoleID = role;
                                    items.Add(item);
                                }
                                if (items.Count > 0)
                                {
                                    context.AppCompositeRoleItems.AddRange(items);
                                    context.SaveChanges();
                                }
                            }
                        }//End Save Items

                        scope.Complete();

                    }//End Transaction
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

        public override ValidationResult SaveDelete(AppCompositeRoleViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            try
            {
                if (formData.ID <= 0)
                {
                    result.Message = ValidatorMessage.id_not_found;
                    result.ModelStateErrorList.Add(new ModelStateError("", result.Message));
                    result.ErrorFlag = true;
                    return result;
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    using (var context = new PYMFEEEntities())
                    {
                        var appCompositeRole = new AppCompositeRole();
                        MVMMappingService.MoveData(formData, appCompositeRole);
                        context.Entry(appCompositeRole).State = System.Data.Entity.EntityState.Deleted;
                        context.SaveChanges();

                        //Delete Items
                        context.AppCompositeRoleItems.RemoveRange(context.AppCompositeRoleItems.Where(m => m.CompositeRoleID == appCompositeRole.ID).ToList());
                        context.SaveChanges();
                        
                        scope.Complete();

                    }//End Transaction
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

        public override ValidationResult SaveEdit(AppCompositeRoleViewModel formData, ModelStateDictionary modelState)
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

                if (formData.ID <= 0)
                {
                    result.Message = ValidatorMessage.id_not_found;
                    result.ModelStateErrorList.Add(new ModelStateError("", result.Message));
                    result.ErrorFlag = true;
                    return result;
                }

                using (TransactionScope scope = new TransactionScope())
                {
                    using (var context = new PYMFEEEntities())
                    {
                        var appCompositeRole = new AppCompositeRole();
                        MVMMappingService.MoveData(formData, appCompositeRole);
                        context.Entry(appCompositeRole).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();

                        //Delete Items
                        context.AppCompositeRoleItems.RemoveRange(context.AppCompositeRoleItems.Where(m => m.CompositeRoleID == appCompositeRole.ID).ToList());
                        context.SaveChanges();

                        //Save Items
                        if (appCompositeRole.ID > 0)
                        {
                            List<string> roleList = new List<string>();
                            if (!string.IsNullOrEmpty(formData.AppMenuSelectedJSON))
                            {
                                roleList = GetRoleListFromJSON(formData.AppMenuSelectedJSON);
                                List<AppCompositeRoleItem> items = new List<AppCompositeRoleItem>();
                                foreach (var role in roleList)
                                {
                                    if (string.IsNullOrEmpty(role)) continue;
                                    var item = new AppCompositeRoleItem();
                                    item.CompositeRoleID = appCompositeRole.ID;
                                    item.RoleID = role;
                                    items.Add(item);
                                }
                                if (items.Count > 0)
                                {
                                    context.AppCompositeRoleItems.AddRange(items);
                                    context.SaveChanges();
                                }
                            }
                        }//End Save Items

                        scope.Complete();

                    }//End Transaction
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

        public override ValidationResult ValidateFormData(AppCompositeRoleViewModel formData, ModelStateDictionary modelState)
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
                    var roleList = GetRoleListFromJSON(formData.AppMenuSelectedJSON);
                    if (roleList.Count <= 0)
                    {
                        result.Message = ValidatorMessage.item_not_found;
                        result.ModelStateErrorList.Add(new ModelStateError("", result.Message));
                        result.ErrorFlag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.ToString();
                result.ModelStateErrorList.Add(new ModelStateError("", result.Message));
                result.ErrorFlag = true;
            }

            return result;
        }


        public static List<AppMenuViewModel> GetAppMenu()
        {
            List<AppMenuViewModel> userMenuList = new List<AppMenuViewModel>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var appMenus = (from m in context.AppMenus select m).ToList();
                    var parentMenus = appMenus.Where(m => m.ParentMenuCode == null).FirstOrDefault();
                    userMenuList = GetAppMenuUnder(parentMenus, appMenus);
                }
            }
            catch (Exception ex)
            {

            }

            return userMenuList;
        }

        private static List<AppMenuViewModel> GetAppMenuUnder(AppMenu parent, List<AppMenu> appMenus, int level = 0)
        {
            List<AppMenuViewModel> result = new List<AppMenuViewModel>();
            ResourceManager rm = new ResourceManager(typeof(ResourceText));

            try
            {
                var newLevel = level + 1;
                var appMenuUnderList = appMenus.Where(m => m.ParentMenuCode == parent.MenuCode).ToList();
                if (appMenuUnderList.Count > 0)
                {
                    appMenuUnderList = appMenuUnderList.OrderBy(m => m.Sequence).ToList();
                    foreach (var appMenuUnder in appMenuUnderList)
                    {
                        AppMenuViewModel appMenu = new AppMenuViewModel();
                        MVMMappingService.MoveData(appMenuUnder, appMenu);
                        appMenu.Level = newLevel;
                        appMenu.MenuText = rm.GetString(appMenu.ResourceName);

                        if (!string.IsNullOrEmpty(appMenuUnder.RoleForManage) || appMenuUnder.MenuType.Equals("GROUP", StringComparison.OrdinalIgnoreCase))
                        {
                            appMenu.IsRoleForManage = true;
                        }

                        if (!string.IsNullOrEmpty(appMenuUnder.RoleForDisplay) || appMenuUnder.MenuType.Equals("GROUP", StringComparison.OrdinalIgnoreCase))
                        {
                            appMenu.IsRoleForDisplay = true;
                        }
                        result.Add(appMenu);

                        var appMenuNextUnderList = GetAppMenuUnder(appMenuUnder, appMenus, newLevel);
                        if (appMenuNextUnderList.Count > 0)
                        {
                            result.AddRange(appMenuNextUnderList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        private List<string> GetRoleListFromJSON(string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<string> roles = js.Deserialize<List<string>>(json);
            return roles;
        }

        private List<AppMenuViewModel> UpdateParentList(List<AppMenuViewModel> appMenuList)
        {
            var newAppMenuList = new List<AppMenuViewModel>();
            var parentAppMenuDisplayList = UpdateParent("DISPLAY", "ROOT", appMenuList.Where(m=>m.IsRoleForDisplay==true).ToList());
            var parentAppMenuManageList = UpdateParent("MANAGE", "ROOT", appMenuList.Where(m => m.IsRoleForManage == true).ToList());
            foreach (var appMenu in appMenuList)
            {
                var parentMenuDisplay = parentAppMenuDisplayList.Where(m => m.MenuCode == appMenu.MenuCode).FirstOrDefault();
                var parentMenuManage = parentAppMenuManageList.Where(m => m.MenuCode == appMenu.MenuCode).FirstOrDefault();
                if (parentMenuDisplay != null)
                {
                    appMenu.CheckRoleForDisplay = true;
                }
                if (parentMenuManage != null)
                {
                    appMenu.CheckRoleForManage = true;
                }
                newAppMenuList.Add(appMenu);
            }
            return newAppMenuList;
        }

        private List<AppMenuViewModel> UpdateParent(string roleType, string parentMenuCode, List<AppMenuViewModel> appMenuList)
        {
            List<AppMenuViewModel> newAppMenuList = new List<AppMenuViewModel>();
            try
            {
                int currentLevel = appMenuList.Where(m => m.ParentMenuCode == parentMenuCode).FirstOrDefault().Level;
                foreach (var appMenu in appMenuList.Where(m => m.ParentMenuCode == parentMenuCode).ToList())
                {
                    if (appMenu.MenuType.Equals("GROUP", StringComparison.OrdinalIgnoreCase))
                    {
                        var childInNextLevel = appMenuList.Where(m => m.Level > currentLevel).ToList();
                        var updateParentList = UpdateParent(roleType, appMenu.MenuCode, childInNextLevel);
                        if (childInNextLevel.Where(m => m.ParentMenuCode == appMenu.MenuCode).ToList().Count ==
                           updateParentList.Where(m => m.ParentMenuCode == appMenu.MenuCode).ToList().Count &&
                           updateParentList.Count>0)
                        {
                            newAppMenuList.Add(appMenu);
                        }
                        if(updateParentList.Count>0)
                            newAppMenuList.AddRange(updateParentList);
                    }
                    else
                    {
                        if (roleType == "DISPLAY")
                        {
                            if (appMenu.CheckRoleForDisplay)
                            {
                                newAppMenuList.Add(appMenu);
                            }
                        }
                        else
                        {
                            if (appMenu.CheckRoleForManage)
                            {
                                newAppMenuList.Add(appMenu);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return newAppMenuList;
        }

    }
}