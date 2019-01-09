using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BBDEVSYS.Models.Shared;
using System.Web.Mvc;
using BBDEVSYS.Content.text;
using System.Resources;
using BBDEVSYS.Models.Entities;
using System.Security.Principal;
using System.Web.Configuration;
using System.Web.Security;
using System.Text;

namespace BBDEVSYS.Services.Shared
{
    public class UserService
    {

        public static User GetUserInfo(string username, bool backgroundProcess = false)
        {
            User user = new User();

            user.ADUser = username;

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    //var employee = (from m in context.USERS
                    //                where m.ADUser == user.ADUser &&
                    //                      m.EmpStatus == "3" //Active
                    //                select m).FirstOrDefault();
                    //if (employee != null)
                    //{
                    //    user.UserType = ConstantVariableService.UserTypeEmployee;
                    //    user.UserCode = employee.EmpNo;
                    //    user.DisplayNameTH = employee.TitleTH + employee.FirstnameTH + " " + employee.LastnameTH;
                    //    user.DisplayNameEN = employee.TitleEN + employee.FirstnameEN + " " + employee.LastnameEN;
                    //    user.Email = employee.Email;
                    //}


                    ////Not employee then check with agent
                    //if (string.IsNullOrEmpty(user.UserCode))
                    //{
                    //    var agent = (from m in context.AgentInfoes
                    //                 join n in context.AgentMasters on m.AgentCode equals n.AgentCode
                    //                 where m.ADUser == user.ADUser &&
                    //                       m.Status == ConstantVariableService.AgentStatusOpen
                    //                 select new { AgentInfo = m, AgentMaster = n }).FirstOrDefault();
                    //    if (agent != null)
                    //    {
                    //        user.UserType = ConstantVariableService.UserTypeAgent;
                    //        user.UserCode = agent.AgentMaster.AgentCode;
                    //        user.DisplayNameTH = agent.AgentMaster.Name;
                    //        user.DisplayNameEN = agent.AgentInfo.NameEN;
                    //        user.Email = agent.AgentInfo.Email;
                    //    }
                    //}


                    ////Not employee, agent then check with external audit
                    //if (string.IsNullOrEmpty(user.UserCode))
                    //{
                    //    var today = DateTime.Today;
                    //    var externalAudit = (from m in context.ExternalAudits
                    //                         where m.ADUser == user.ADUser &&
                    //                               m.ADuserStartDate <= today &&
                    //                               m.ADUserEndDate >= today
                    //                         select m).FirstOrDefault();
                    //    if (externalAudit != null)
                    //    {
                    //        user.UserType = ConstantVariableService.UserTypeExternalAudit;
                    //        user.UserCode = externalAudit.ExternalAuditCode;
                    //        user.DisplayNameTH = externalAudit.NameTH;
                    //        user.DisplayNameEN = externalAudit.NameEN;
                    //        user.Email = externalAudit.Email;
                    //    }
                    //}

                    //Not employee, agent then check with user employee
                    if (string.IsNullOrEmpty(user.UserCode))
                    {
                        var today = DateTime.Today;
                        var emp = (from m in context.USERS
                                   where m.USERNAME == username
                                   select m).FirstOrDefault();
                        if (emp != null)
                        {
                            user.UserType = "";
                            user.UserCode = emp.USERID;
                            user.DisplayNameTH = emp.NAME;
                            user.DisplayNameEN = emp.NAME;
                            user.Email = emp.Email;
                        }
                    }

                    if (!backgroundProcess)
                    {
                        if (!string.IsNullOrEmpty(user.UserCode))
                        {
                            user.RoleList = GetRole(username);
                            user.SideMenuHTML = GetSideMenuHTML(user);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

//            For test role for display only
//            if (string.Equals(username, "BOONRAWD_LOCAL\\user_spst04", StringComparison.OrdinalIgnoreCase))
//                {
//                    user.RoleList = GetDisplaySingleRole();
//                    user.SideMenuHTML = GetSideMenuHTML(user);
//                }
//            End for test role display only


                //user.RoleList = GetRole(username);
                //user.SideMenuHTML = GetSideMenuHTML(user);

                return user;
        }

        public static User GetSessionUserInfo()
        {

            IPrincipal user = HttpContext.Current.User;
            string username = user.Identity.Name;

            //Set user information to session data
            User userInfo = (User)HttpContext.Current.Session["USERINFO"];
            if (userInfo == null) //First login then get user information and store in session data
            {
                userInfo = UserService.GetUserInfo(username);
                HttpContext.Current.Session["USERINFO"] = userInfo;
            }
            else
            {
                if (!string.Equals(userInfo.ADUser, username, StringComparison.OrdinalIgnoreCase)) //Clear session incase browser not automatic clear session
                {
                    HttpContext.Current.Session.Clear();
                    HttpContext.Current.Session.RemoveAll();

                    userInfo = UserService.GetUserInfo(username);
                    HttpContext.Current.Session["USERINFO"] = userInfo;
                }
            }

            return userInfo;
        }

        public static void SetSessionUserInfo()
        {
            IPrincipal user = HttpContext.Current.User;
            string username = user.Identity.Name;
            User userInfo;

            //Clear session
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();

            userInfo = UserService.GetUserInfo(username);
            HttpContext.Current.Session["USERINFO"] = userInfo;

        }


        public static List<AppSingleRole> GetRole(string username)
        {
            List<AppSingleRole> roleList = new List<AppSingleRole>();
            AppSingleRole role = new AppSingleRole();

#if DEBUG  
            //For test, add super admin role
            role = new AppSingleRole();
            role.RoleID = ConstantVariableService.RoleSuperAdmin;
            roleList.Add(role);

            roleList = GetSuperAdminSingleRole();
#endif

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var user = (from m in context.USERS where m.USERNAME == username select m).FirstOrDefault();
                    string userId = user == null ? "" : user.USERID;
                    var userCompRoleList = (from m in context.UserRoles
                                            join n in context.AppCompositeRoles on m.CompositeRoleID equals n.ID
                                            where m.USERID == userId //&&
                                            //n.Status == ConstantVariableService.ConfigStatusActive
                                            select m).ToList();

                    if (userCompRoleList.Any())
                    {
                        foreach(var compRole in userCompRoleList)
                        {
                            var compRoleItemList = (from m in context.AppCompositeRoleItems where m.CompositeRoleID == compRole.CompositeRoleID select m).ToList();

                            foreach(var compRoleItem in compRoleItemList)
                            {
                                role = new AppSingleRole();
                                role.RoleID = compRoleItem.RoleID;
                                roleList.Add(role);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return roleList;
        }

        public static string GetSideMenuHTML(User user)
        {
            var requestContext = HttpContext.Current.Request.RequestContext;
            new UrlHelper(requestContext).Action("MainPage", "Index");

            ResourceManager rm = new ResourceManager(typeof(ResourceText));
            string someString = rm.GetString("MenuCAEvaAndQuestionaire");

            string menuHtml = "";

            List<AppMenu> userMenuList = GetSideMenu(user);

            string parentMenuCode = ConstantVariableService.RootMenuCode;

            var menuList = userMenuList.Where(a => a.ParentMenuCode.Equals(parentMenuCode, StringComparison.OrdinalIgnoreCase)).OrderBy(a => a.Sequence).ToList();

            foreach (var item in menuList)
            {
                menuHtml = menuHtml + GenerateMenuHTML(userMenuList, item);
            }

            return menuHtml;
        }

        public static string GenerateMenuHTML(List<AppMenu> userMenuList, AppMenu menu)
        {
            string html = "";
            var requestContext = HttpContext.Current.Request.RequestContext;
            ResourceManager rm = new ResourceManager(typeof(ResourceText));

            string menuText = rm.GetString(menu.ResourceName);

            if (menu.MenuType.Equals("GROUP", StringComparison.OrdinalIgnoreCase))
            {
                List<AppMenu> childMenuList = userMenuList.Where(a => a.ParentMenuCode.Equals(menu.MenuCode, StringComparison.OrdinalIgnoreCase)).OrderBy(a => a.Sequence).ToList();

                html = "<li class=\"treeview\">" +
                            "<a href=\"#\">" +
                                "<i class=\"" + menu.Icon + "\"></i> <span class=\"true-left-menu-text\"> " + menuText + "</span>" +
                                "<span class=\"pull-right-container true-left-menu-angle\">" +
                                    "<i class=\"fa fa-angle-left pull-right\"></i>" +
                                "</span>" +
                            "</a>" +
                            "<ul class=\"treeview-menu\">";


                foreach (var item in childMenuList)
                {
                    html = html + GenerateMenuHTML(userMenuList, item);
                }

                html = html + "</ul></li>";
            }
            else if (menu.MenuType.Equals("ITEM", StringComparison.OrdinalIgnoreCase))
            {
                html = "<li>" +
                           "<a id = \"MENU_" + menu.Controller + "\" href = \"" + new UrlHelper(requestContext).Action(menu.Action, menu.Controller, new { Area = menu.Area }) + "\" >" +
                                "<i class=\"" + menu.Icon + "\"></i> <span class=\"true-left-menu-text\"> " + menuText + "</span>" +
                            "</a>" +
                       "</li>";
            }

            return html;

        }

        public static List<AppMenu> GetSideMenu(User user)
        {
            List<AppMenu> userMenuList = new List<AppMenu>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    var appMenus = (from m in context.AppMenus select m).ToList();

                    //Check menu which user have role to display
                    var roleMenuList = appMenus.Where(b => user.RoleList.Any(a => a.RoleID.Equals(b.RoleForDisplay, StringComparison.OrdinalIgnoreCase)) || 
                                                           user.RoleList.Any(a => a.RoleID.Equals(b.RoleForManage, StringComparison.OrdinalIgnoreCase))).ToList();

                    //var groupMenuList = appMenus.Where(b => b.MenuType.Equals("GROUP", StringComparison.OrdinalIgnoreCase)).ToList();
                    var groupMenuList = appMenus.Where(b => b.MenuType=="GROUP").ToList();

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

        public static List<AppSingleRole> GetSuperAdminSingleRole()
        {
            List<AppSingleRole> roleList = new List<AppSingleRole>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    roleList = (from m in context.AppSingleRoles select m).ToList();
                }

            }
            catch (Exception ex)
            {

            }

            var role = new AppSingleRole();
            role.RoleID = ConstantVariableService.RoleSuperAdmin;
            roleList.Add(role);

            return roleList;
        }

        //For test user without manage data role
        public static List<AppSingleRole> GetDisplaySingleRole()
        {
            List<AppSingleRole> roleList = new List<AppSingleRole>();

            try
            {
                using (var context = new PYMFEEEntities())
                {
                    roleList = (from m in context.AppSingleRoles where m.RoleID.Contains("Role_DS") select m).ToList();
                }

            }
            catch (Exception ex)
            {

            }

            return roleList;
        }


    }


}