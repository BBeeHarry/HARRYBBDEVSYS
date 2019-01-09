using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BBDEVSYS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            //   routes.MapRoute(
            //       name: "Default",
            //       url: "{area}/{controller}/{action}/{id}",
            //       defaults: new { area = "", controller = "Home", action = "Index", id = UrlParameter.Optional }
            //   );


            //   routes.MapRoute(
            //    name: "URLWithParam",
            //    url: "{area}/{controller}/{action}/{id}"
            ////new { controller = "Test1", action = "Index" },
            ////new[] { "POCV1.Controllers.ChampTest" }
            //);

            //   routes.MapRoute(
            //       name: "URLWithoutParam",
            //       url: "{area}/{controller}/{action}",
            //       defaults: new { controller = "Authentication", action = "Index", id = UrlParameter.Optional }
            //   //new[] { "POCV1.Controllers.ChampTest" }
            //   );

            //   //"" means when you will not use any controller name or view name it will use above routing
            //   routes.MapRoute(
            //       name: "URLDefault",
            //       url: "",
            //       defaults: new { controller = "Authentication", action = "Default" }
            //   );

            routes.MapRoute(
                name: "URLWithParam",
                url: "{area}/{controller}/{action}/{id}"
            //new { controller = "Test1", action = "Index" },
            //new[] { "POCV1.Controllers.ChampTest" }
            );

            routes.MapRoute(
                name: "URLWithoutParam",
                url: "{area}/{controller}/{action}",
                defaults: new { controller = "Authentication", action = "Index", id = UrlParameter.Optional }
            //new[] { "POCV1.Controllers.ChampTest" }
            );

            //"" means when you will not use any controller name or view name it will use above routing
            routes.MapRoute(
                name: "URLDefault",
                url: "",
                defaults: new { controller = "Authentication", action = "Default" }
            );
            
            // Show a 404 error page for anything else.
            routes.MapRoute("URLError", "{*url}",
                new { controller = "Error", action = "Message" }
            );
        }
    }
}
