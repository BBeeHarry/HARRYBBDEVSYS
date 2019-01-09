using BBDEVSYS.Models.Shared;
using FluentValidation.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace BBDEVSYS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            AreaRegistration.RegisterAllAreas();
            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FluentValidationModelValidatorProvider.Configure();
            
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("th-TH");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("th-TH");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-GB");

        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null && !authTicket.Expired)
                {
                    var roles = authTicket.UserData.Split(',');
                    HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                }
            }
        }
        protected void Application_Error()
        {
            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
                /* when the request is ajax the system can automatically handle a mistake with a JSON response. then overwrites the default response */
                if (requestContext.HttpContext.Request.IsAjaxRequest())
                {
                    //Do Nothing
                }
                else
                {
                    httpContext.Response.Redirect("~/Error/Error/Message");
                }
            }
        }

        //protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {


        //        // Get the forms authentication ticket.
        //        FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //        var identity = new GenericIdentity(authTicket.Name, "Forms");
        //        var principal = new  MyPrincipal(identity);
        //        // Get the custom user data encrypted in the ticket.
        //        string userData = ((FormsIdentity)(Context.User.Identity)).Ticket.UserData;
        //        // Deserialize the json data and set it on the custom principal.
        //        var serializer = new JavaScriptSerializer();
        //        principal.User = (User)serializer.Deserialize(userData, typeof(User));
        //        // Set the context user.
        //        Context.User = principal;
        //        ////////////////////////////////////////////////////////////////
        //        //FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

        //        //var serializeModel = JsonConvert.DeserializeObject<CustomSerializeModel>(authTicket.UserData);

        //        //CustomPrincipal principal = new CustomPrincipal(authTicket.Name);

        //        //principal.UserId = serializeModel.UserId;
        //        //principal.FirstName = serializeModel.FirstName;
        //        //principal.LastName = serializeModel.LastName;
        //        //principal.Roles = serializeModel.RoleName.ToArray<string>();

        //        //HttpContext.Current.User = principal;


        //    }
        //    else
        //    {
        //        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
        //    }
        //}
    }
}
