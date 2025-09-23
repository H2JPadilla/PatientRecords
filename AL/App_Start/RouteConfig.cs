using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AL
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Home Page",
             url: "",
             defaults: new { area = "Admin", controller = "Patient", action = "ViewPatient" }).DataTokens.Add("area", "Admin");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { area = "Admin", controller = "Patient", action = "ViewPatient", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               "404-PageNotFound",
               "{*url}",
               new { area = "Admin", controller = "Error", action = "ErrorView" }
           );
        }
    }
}
