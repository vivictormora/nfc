﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PKGServer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Privacy",
                url: "privacy/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Privacy", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Conditions",
                url: "conditions/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Conditions", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}