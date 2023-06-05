using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jade_Dragon
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /*routes.MapRoute(
               name: "dang-nhap",
               url: "dang-nhap/{*pathInfo}",
               defaults: new { controller = "Account", action = "Login" },
              namespaces: new[] { "Jade_Dragon.Controllers" }
           );

            routes.MapRoute(
              name: "kiem-tra-dang-nhap",
              url: "kiem-tra-dang-nhap/{*pathInfo}",
              defaults: new { controller = "Account", action = "Login", username = UrlParameter.Optional, password = UrlParameter.Optional, ghinho = UrlParameter.Optional },
              namespaces: new[] { "Jade_Dragon.Controllers" }
          );*/

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "trangchu", action = "trangchu", id = UrlParameter.Optional }
            );

        }
    }
}
