using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BlueStone.Smoke.Backend
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "ApiDefault",
                routeTemplate: "api/{action}"
                , defaults: new { controller = "CommonApi" }
            );
        }
    }
}
