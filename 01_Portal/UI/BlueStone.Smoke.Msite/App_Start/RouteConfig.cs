using BlueStone.Utility.Web.Router;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlueStone.Smoke.Msite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteConfigurationSection section = (RouteConfigurationSection)ConfigurationManager.GetSection("routeConfig");
            routes.MapRoute(section);
        }
    }
}
