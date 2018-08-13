using System.Web;
using System.Web.Mvc;
using BlueStone.Smoke.Msite.App_Start;

namespace BlueStone.Smoke.Msite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandleAttribute());
        }
    }
}
