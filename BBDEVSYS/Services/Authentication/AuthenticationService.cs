using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BBDEVSYS.Models.Shared;
using BBDEVSYS.ViewModels.Authentication;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Web.Configuration;
using Newtonsoft.Json.Linq;
using BBDEVSYS.Content.text;
using System.Web.Security;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.Models.Entities;
using System.Security.Principal;

namespace BBDEVSYS.Services.Authentication
{
    public class AuthenticationService
    {
        public ValidationResult Login(AuthenticationViewModel authen, HttpResponseBase response)
        {
            ValidationResult result = new ValidationResult();
            string domainUser = "";
            result.ErrorFlag = true;

            //Clear previous login data
            this.Logout(response);

            byte[] passwordBytes = MachineKey.Protect(Encoding.UTF8.GetBytes(authen.Password), WebConfigurationManager.AppSettings["UserdataSALT"]);
            string protectedPassword = UtilityService.ConvertBytesToString(passwordBytes);

#if DEBUG            

            //Test convert bytes to string and decode
            string enct = UtilityService.ConvertBytesToString(passwordBytes);
            byte[] enc2 = UtilityService.ConvertStringToBytes(enct);

            byte[] dc = MachineKey.Unprotect(enc2, WebConfigurationManager.AppSettings["UserdataSALT"]);
            string originalString = Encoding.UTF8.GetString(dc);

            result.ErrorFlag = false;
            domainUser = authen.Username;
            #region mark
            //if (authen.Username.StartsWith("user", StringComparison.OrdinalIgnoreCase))
            //{
            //    domainUser = WebConfigurationManager.AppSettings["EmployeeDomain"] + authen.Username;
            //}
            //else if (authen.Username.StartsWith("agent", StringComparison.OrdinalIgnoreCase))
            //{
            //    domainUser = WebConfigurationManager.AppSettings["AgentDomain"] + authen.Username;
            //}
            //else if (authen.Username.StartsWith("audit", StringComparison.OrdinalIgnoreCase))
            //{
            //    domainUser = WebConfigurationManager.AppSettings["ExternalAuditDomain"] + authen.Username;
            //}
            #endregion

            string userData = string.Empty;
            using (var context = new PYMFEEEntities())
            {
                var validUser = context.ValidateUser(authen.Username, authen.Password).FirstOrDefault();

                if (!validUser.Equals("Non", StringComparison.OrdinalIgnoreCase))
                {
                    var userDataList = UserService.GetRole(authen.Username);
                    if (userDataList.Any())
                    {
                        userData = string.Join(",", userDataList.Select(m => m.RoleID).ToList());
                    }

                    result.ErrorFlag = false;
                }
                else
                {
                    result.Message = ValidatorMessage.login_fail;
                    result.ModelStateErrorList.Add(new ModelStateError("", ValidatorMessage.login_fail));
                    result.ErrorFlag = true;
                }
            }
            //FormsAuthentication.SetAuthCookie(authen.Username, false);

            //var debugTicket = new FormsAuthenticationTicket(1, authen.Username, DateTime.Now, DateTime.Now.AddMinutes(20), false, "Role_MA_Invoice,Role_DS_Invoice");
            //string encryptedDebugTicket = FormsAuthentication.Encrypt(debugTicket);
            //var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedDebugTicket);
            //response.Cookies.Add(authCookie);

            FormsAuthenticationTicket debugTicket = new FormsAuthenticationTicket
            (
                1, // version 
                domainUser, // name 
                DateTime.Now, // issueDate 
                DateTime.Now.Add(FormsAuthentication.Timeout), // expiration 
                false, // isPersistent 
                userData,//"Role_MA_Invoice,Role_DS_Invoice",//protectedPassword, // userData 
                FormsAuthentication.FormsCookiePath // cookiePath 
            );

            string encryptedDebugTicket = FormsAuthentication.Encrypt(debugTicket);

            var debugCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedDebugTicket)
            {
                HttpOnly = true, // always set this to true!
                Secure = false,
            };

            //GenericIdentity identity = new GenericIdentity(authen.Username);
            //string[] roles = { debugCookie.Value }; //or do it from the person object
            //GenericPrincipal principal = new System.Security.Principal.GenericPrincipal(identity, roles);
            //HttpContext.Current.User = principal;

            ////response.Cookies.Set(cookie);
            // Create the cookie.
            response.Cookies.Add(debugCookie);


            return result;
#endif

            try
            {
                string systemID = WebConfigurationManager.AppSettings["SystemID"];
                //string url = "";
                //byte[] responseBytes;
                //string responsefromserver;
                //JObject json;
                //string checkResult;
                bool loginOk = false;
                string loginMessage = ValidatorMessage.login_fail;

                //Encode password to base64
                var passwordUTF8 = System.Text.Encoding.UTF8.GetBytes(authen.Password);
                var passwordBase64 = System.Convert.ToBase64String(passwordUTF8);


                using (WebClient webClient = new WebClient())
                {

                    NameValueCollection formData = new NameValueCollection();
                    formData["username"] = authen.Username;
                    formData["password"] = passwordBase64;
                    formData["system_id"] = systemID;

                    //    //First authenticate with employee
                    //    url = WebConfigurationManager.AppSettings["ADEmployeeURL"];
                    //    responseBytes = webClient.UploadValues(url, "POST", formData);
                    //    responsefromserver = Encoding.UTF8.GetString(responseBytes);

                    //    json = JObject.Parse(responsefromserver);
                    //    checkResult = json["result"].Value<string>();

                    //    if (string.Equals(checkResult, "success", StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        domainUser = WebConfigurationManager.AppSettings["EmployeeDomain"] + authen.Username;
                    //        loginOk = true;
                    //    }

                    //    if (!loginOk)
                    //    {
                    //        //Second authenticate with agent
                    //        url = WebConfigurationManager.AppSettings["ADAgentURL"];
                    //        responseBytes = webClient.UploadValues(url, "POST", formData);
                    //        responsefromserver = Encoding.UTF8.GetString(responseBytes);

                    //        json = JObject.Parse(responsefromserver);
                    //        checkResult = json["result"].Value<string>();

                    //        if (string.Equals(checkResult, "success", StringComparison.OrdinalIgnoreCase))
                    //        {
                    //            domainUser = WebConfigurationManager.AppSettings["AgentDomain"] + authen.Username;
                    //            loginOk = true;
                    //        }
                    //    }

                    //    if (!loginOk)
                    //    {
                    //        //Third authenticate with external audit
                    //        url = WebConfigurationManager.AppSettings["ADExternalAuditURL"];
                    //        responseBytes = webClient.UploadValues(url, "POST", formData);
                    //        responsefromserver = Encoding.UTF8.GetString(responseBytes);

                    //        json = JObject.Parse(responsefromserver);
                    //        checkResult = json["result"].Value<string>();

                    //        if (string.Equals(checkResult, "success", StringComparison.OrdinalIgnoreCase))
                    //        {
                    //            domainUser = WebConfigurationManager.AppSettings["ExternalAuditDomain"] + authen.Username;
                    loginOk = true;
                    //        }
                    //    }

                    //Check UserRole

                    string _userData = string.Empty;
                    if (loginOk)
                    {

                        using (var context = new PYMFEEEntities())
                        {
                            var userDataList = UserService.GetRole(authen.Username);
                            if (userDataList.Any())
                            {
                                _userData = string.Join(",", userDataList.Select(m => m.RoleID).ToList());
                            }
                            var entUser = (from m in context.USERS where m.USERNAME == authen.Username select m).FirstOrDefault();
                            string userId = entUser == null ? "" : entUser.USERID;
                            var userCompRoleList = (from m in context.UserRoles
                                                    join n in context.AppCompositeRoles on m.CompositeRoleID equals n.ID
                                                    //Older UserName
                                                    where m.USERID == userId //&&
                                                    //n.Status == ConstantVariableService.ConfigStatusActive
                                                    select m).ToList();

                            if (!userCompRoleList.Any())
                            {
                                loginOk = false;
                                loginMessage = ValidatorMessage.login_fail_no_user_role;
                            }

                        }
                    }

                    if (!loginOk)
                    {
                        result.Message = ValidatorMessage.login_fail;
                        result.ModelStateErrorList.Add(new ModelStateError("", loginMessage));
                        result.ErrorFlag = true;
                    }
                    else
                    {
                        result.ErrorFlag = false;

                        //Set cookie
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket
                        (
                            1, // version 
                            authen.Username,//domainUser, // name 
                            DateTime.Now, // issueDate 
                            DateTime.Now.Add(FormsAuthentication.Timeout), // expiration 
                            false, // isPersistent 
                            _userData,//protectedPassword, // userData 
                            FormsAuthentication.FormsCookiePath // cookiePath 
                        );

                        string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                        {
                            HttpOnly = true, // always set this to true!
                            Secure = false,
                        };

                        // Create the cookie.
                        response.Cookies.Add(cookie);

                    }
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

        public ValidationResult Logout(HttpResponseBase response)
        {
            ValidationResult result = new ValidationResult();

            //Clear cache
            response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();

            //Clear session data
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Session.Abandon();

            FormsAuthentication.SignOut();

            return result;
        }


    }
}