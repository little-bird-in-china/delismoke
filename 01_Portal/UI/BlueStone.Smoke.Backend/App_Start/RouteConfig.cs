using System.Web.Mvc;
using System.Web.Routing;

namespace BlueStone.Smoke.Backend
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("json.rpc");
            routes.IgnoreRoute("json.rpc/help");

            routes.MapRoute(
                name: "Index",
                url: "Index",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }, namespaces: new string[] { "BlueStone.Smoke.Backend.Controllers" });

            routes.MapRoute(
               name: "CompanyAddress",
               url: "CompanyAddress",
               defaults: new { controller = "Company", action = "AddressManager" }
           );
            routes.MapRoute(
               name: "CompanyAddressManager",
               url: "Company/Address/{companySysNo}",
               defaults: new { controller = "Company", action = "AddressManager", companySysNo = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "CompanyReport",
               url: "CompanyReport",
               defaults: new { controller = "DayReport", action = "DayReport" }
           );

            routes.MapRoute(
               name: "CompanyMap",
               url: "CompanyMap",
               defaults: new { controller = "Company", action = "SmokeMap" }
           );

            routes.MapRoute(
               name: "CompanyMapManager",
               url: "Company/Map/{companySysNo}",
               defaults: new { controller = "Company", action = "SmokeMap", companySysNo = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "SmokeList",
               url: "SmokeList",
               defaults: new { controller = "Smoke", action = "List" }
           );

            routes.MapRoute(
               name: "SmokeListManager",
               url: "Smoke/List/{companySysNo}",
               defaults: new { controller = "Smoke", action = "List", companySysNo = UrlParameter.Optional }
           );
            routes.MapRoute(
              name: "CompanyInfo",
              url: "Company/CompanyInfo",
              defaults: new { controller = "Company", action = "Maintain" }
          );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "BlueStone.Smoke.Backend.Controllers" }
                );
        }
    }
}
