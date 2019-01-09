using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Shared;
using System.Web.Security;
using BBDEVSYS.Models.Entities;
using Newtonsoft.Json;
using BBDEVSYS.ViewModels.Authentication;

namespace BBDEVSYS.Services.Shared
{
    public class AuthorizeService : AuthorizeAttribute
    {
        public string[] AllowRoleList { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            //Check authenticated cookie
            HttpCookie authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null)
            {
                HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                return false;

            }
            //FormsAuthentication.SetAuthCookie("ratchanok", false);
            //GenericIdentity identity = new GenericIdentity("ratchanok");
            //string[] roles = { authCookie.Value }; //or do it from the person object
            //GenericPrincipal principal = new GenericPrincipal(identity, roles);
            //httpContext.User = principal;

            IPrincipal user = httpContext.User;   
            if (!user.Identity.IsAuthenticated)
            {
                //return true;
                return false;
            }

            //User userInfo = UserService.GetSessionUserInfo(user.Identity.Name);
            User userInfo = UserService.GetSessionUserInfo();

            //mark  skip
            //if (string.IsNullOrEmpty(userInfo.UserType))
            //{
            //    return false;
            //}

            //#if DEBUG
            //return true;
            //#endif

            //Check super admin => can access any controller
            var superAdmin = userInfo.RoleList.FirstOrDefault(a => a.RoleID.Equals(ConstantVariableService.RoleSuperAdmin, StringComparison.OrdinalIgnoreCase));
            if (superAdmin != null)
            {
                return true;
            }
            if (AllowRoleList.Length <= 0)
            {
                return false;
            }

            //Check Role Role_Everyone
            var everyoneRole = AllowRoleList.Where(a => a.Equals(ConstantVariableService.RoleEveryone, StringComparison.OrdinalIgnoreCase)).ToList();
            if (everyoneRole != null && everyoneRole.Any())
            {
                return true;
            }

            var matchRoleList = AllowRoleList.Where(b => userInfo.RoleList.Any(a => b.Equals(a.RoleID, StringComparison.OrdinalIgnoreCase))).ToList();
            if (matchRoleList != null && matchRoleList.Any())
            {
                return true;
            }

            return false;
        }
        #region IPrincipal
        public class CustomPrincipal : IPrincipal
        {
            #region Identity Properties  

            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string[] Roles { get; set; }
            #endregion

            public IIdentity Identity
            {
                get; private set;
            }

            public bool IsInRole(string role)
            {
                if (Roles.Any(r => role.Contains(r)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public CustomPrincipal(string username)
            {
                Identity = new GenericIdentity(username);
            }
        }
        #endregion
        public static bool CheckDisplayOnly(string[] manageRole)
        {
            User userInfo = UserService.GetSessionUserInfo();

            var matchRoleList = manageRole.Where(b => userInfo.RoleList.Any(a => b.Equals(a.RoleID, StringComparison.OrdinalIgnoreCase))).ToList();
            if (matchRoleList != null && matchRoleList.Any())
            {
                return false; //allow manage data
            }
            else
            {
                return true; //display data only
            }
        }

        public static List<AppMenu> GetAppMenu(bool getUserMenu, User user = null)
        {
            List<AppMenu> userMenuList = new List<AppMenu>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var appMenus = (from m in context.AppMenus select m).ToList();
                    List<AppMenu> roleMenuList = new List<AppMenu>();

                    if (getUserMenu)
                    {
                        //Check menu which user have role to display
                        roleMenuList = appMenus.Where(b => user.RoleList.Any(a => a.RoleID.Equals(b.RoleForDisplay, StringComparison.OrdinalIgnoreCase))).ToList();
                    }
                    else
                    {
                        roleMenuList = appMenus;
                    }

                    var groupMenuList = appMenus.Where(b => b.MenuType.Equals("GROUP", StringComparison.OrdinalIgnoreCase)).ToList();

                    //Find parent menu
                    int maxParent = 10; //Prevent infinity loop
                    foreach (var item in roleMenuList)
                    {
                        bool findParent = true;
                        int countParent = 0;
                        var parentMenuCode = item.ParentMenuCode;

                        userMenuList.Add(item);

                        while (findParent && !string.IsNullOrEmpty(parentMenuCode) && countParent < maxParent)
                        {
                            AppMenu parentMenu = appMenus.FirstOrDefault(b => b.MenuCode.Equals(parentMenuCode, StringComparison.OrdinalIgnoreCase));

                            if (!parentMenu.MenuCode.Equals(ConstantVariableService.RootMenuCode, StringComparison.OrdinalIgnoreCase))
                            {
                                findParent = true;
                                parentMenuCode = parentMenu.ParentMenuCode;
                                userMenuList.Add(parentMenu);
                            }
                            else
                            {
                                findParent = false;
                            }

                            countParent++;
                        }
                    }

                    //Remove duplicate menu
                    userMenuList = userMenuList.Distinct().ToList();

                }
            }
            catch (Exception ex)
            {

            }

            return userMenuList;
        }

    }
}