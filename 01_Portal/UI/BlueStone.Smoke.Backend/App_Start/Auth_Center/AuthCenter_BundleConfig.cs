using System.Web;
using System.Web.Optimization;

namespace BlueStone.PMPortal.App_Start
{
    public class BundleConfig_v2
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var loginCssStyles = new StyleBundle("~/Content/css/login_css").Include(
              "~/Versions/Version2.0/Content/bootstrap/css/bootstrap.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/font-awesome/css/font-awesome.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/ionicons/css/ionicons.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/adminlet/css/AdminLTE.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/plugins/iCheck/square/blue.css", new CssRewriteUrlTransformWrapper()
            );

            var loginScript = new ScriptBundle("~/Content/script/login_js").Include(
                "~/Versions/Version2.0/Content/plugins/jQuery/jquery-2.1.4.js",
                "~/Versions/Version2.0/Content/bootstrap/js/bootstrap.js",
                "~/Versions/Version2.0/Content/plugins/iCheck/icheck.js",
                "~/Versions/Version2.0/Content/plugins/bootbox/bootbox.js",
                "~/Versions/Version2.0/Content/plugins/blockui/jquery.blockUI.js",
                "~/Versions/Version2.0/Content/my-script/jquery-extension.js"
            );

            var basicCssStyles = new StyleBundle("~/Content/css/basic_css").Include(
               "~/Versions/Version2.0/Content/bootstrap/css/bootstrap.min.css", new CssRewriteUrlTransformWrapper()
           ).Include(
              "~/Versions/Version2.0/Content/font-awesome/css/font-awesome.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/ionicons/css/ionicons.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/plugins/select2/select2.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/plugins/iCheck/square/blue.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/plugins/datatables/dataTables.bootstrap.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/plugins/jstree/style.min.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/plugins/jstree/table-tree.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/adminlet/css/AdminLTE.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Versions/Version2.0/Content/adminlet/css/skins/_all-skins.css", new CssRewriteUrlTransformWrapper()
            );

            var basicScript = new ScriptBundle("~/Content/script/basic_js").Include(
                "~/Versions/Version2.0/Content/plugins/jQuery/jquery-2.1.4.js",
                "~/Versions/Version2.0/Content/bootstrap/js/bootstrap.min.js",
                "~/Versions/Version2.0/Content/plugins/bootbox/bootbox.js",
                "~/Versions/Version2.0/Content/plugins/slimScroll/jquery.slimscroll.min.js",
                "~/Versions/Version2.0/Content/plugins/fastclick/fastclick.js",
                "~/Versions/Version2.0/Content/plugins/datatables/jquery.dataTables.js",
                "~/Versions/Version2.0/Content/plugins/datatables/dataTables.bootstrap.js",
                "~/Versions/Version2.0/Content/plugins/blockui/jquery.blockUI.js",
                "~/Versions/Version2.0/Content/plugins/select2/select2.full.js",
                "~/Versions/Version2.0/Content/plugins/iCheck/icheck.js",
                "~/Versions/Version2.0/Content/plugins/jstree/jstree.js",
                "~/Versions/Version2.0/Content/adminlet/js/app.js",
                "~/Versions/Version2.0/Content/adminlet/js/demo.js",
                "~/Versions/Version2.0/Content/my-script/jquery-extension.js",
                //knockout
                "~/Versions/Version2.0/Content/plugins/knockout/knockout.js",
                "~/Versions/Version2.0/Content/plugins/knockout/knockout.validation.js",
                "~/Versions/Version2.0/Content/plugins/knockout/knockout.validation.zh-CN.js",
                "~/Versions/Version2.0/Content/plugins/knockout/ko.mapping.js",
                "~/Versions/Version2.0/Content/my-script/knockout-extension.js"
            );

            bundles.Add(loginCssStyles);
            bundles.Add(loginScript);

            bundles.Add(basicCssStyles);
            bundles.Add(basicScript);
        }

        private class CssRewriteUrlTransformWrapper : IItemTransform
        {
            public string Process(string includedVirtualPath, string input)
            {
                return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
            }
        }
    }
}