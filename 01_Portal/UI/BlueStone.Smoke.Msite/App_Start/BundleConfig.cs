using System.Web;
using System.Web.Optimization;

namespace BlueStone.Smoke.Msite
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;
            //微信
            bundles.Add(new ScriptBundle("~/content/wechatjs").Include(
                "~/Content/assets/js/jquery-3.1.0.min.js",
                 "~/content/wechatweb/libs/jquery/jquery-weui.js",
                  "~/content/wechatweb/libs/jquery/fastclick.js",
                   "~/content/wechatweb/libs/jquery/city-picker.js",
                   "~/content/wechatweb/libs/jweixin-1.2.0.js"


                ));
            bundles.Add(new StyleBundle("~/Content/wechatcss")
                 .Include("~/content/wechatweb/themes/default/css/jquery-weui.css", new CssRewriteUrlTransformWrapper())
                 .Include("~/content/wechatweb/themes/default/css/demos.css", new CssRewriteUrlTransformWrapper())
                );

            bundles.Add(new StyleBundle("~/content/scrollPageStyle").Include(
             "~/assets/css/scrollLoading.css", new CssRewriteUrlTransformWrapper()
    ));
            bundles.Add(new ScriptBundle("~/content/scrollPage").Include(
                   "~/assets/js/iscroll-probe.js"

          ));

        }
    }

    public class CssRewriteUrlTransformWrapper : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }
    }

}