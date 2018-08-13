using System.Web.Mvc;
using System.Web.Optimization;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter
{
    public class AuthCenterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AuthCenter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AuthCenter_Default",
                "AuthCenter/{controller}/{action}/{id}",
                new {controller="Account",action = "Login", id = UrlParameter.Optional },
               new string[] { "BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers" }
                
            );
            RegisterBundles();
        }

        public void RegisterBundles() {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}