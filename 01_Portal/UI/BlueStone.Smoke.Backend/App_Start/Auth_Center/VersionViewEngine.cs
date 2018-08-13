using System.Web.Mvc;
using System.Web.Optimization;

namespace BlueStone.Smoke.Backend.App_Start
{
    public class VersionViewEngine : RazorViewEngine
    {
        //public VersionType current_version = VersionType.Version2;

        public VersionViewEngine()
        {
                
                ViewLocationFormats = new[]
                {
                "~/Areas/AuthCenter/Views/{1}/{0}.cshtml",
                "~/Areas/AuthCenter/Views/Shared/{0}.cshtml"
                };
                MasterLocationFormats = new[]
                {
                "~/Areas/AuthCenter/Views/{1}/{0}.cshtml",
                 "~/Areas/AuthCenter/Views/Shared/{0}.cshtml"
                };
                PartialViewLocationFormats = new[]
                {
                "~/Areas/AuthCenter/Views/{1}/{0}.cshtml",
                 "~/Areas/AuthCenter/Views/Shared/{0}.cshtml"
                };
            }
    }

}