using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CSUploadFileToOneDriveAndShare
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "hashRoute",    // Route name
                "{controller}/{action}#access_token={detail}",    // URL with parameters
                new { controller = "Login", action = "LogIn", detail = "" }  // Parameter defaults
            );
        }
    }
}
