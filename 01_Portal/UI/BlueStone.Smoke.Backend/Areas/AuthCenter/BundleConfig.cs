using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace BlueStone.Smoke.Backend.Areas
{
    internal static class BundleConfig
    {
        internal static void RegisterBundles(BundleCollection bundles)
        {
            #region AuthCenter css与js 
            // var loginCssStyles = new StyleBundle("~/Content/css/login_css").Include(
            //  "~/Content/AuthCenter/bootstrap/css/bootstrap.css", new CssRewriteUrlTransformWrapper()
            //).Include(
            //  "~/Content/AuthCenter/font-awesome/css/font-awesome.css", new CssRewriteUrlTransformWrapper()
            //).Include(
            //  "~/Content/AuthCenter/ionicons/css/ionicons.css", new CssRewriteUrlTransformWrapper()
            //).Include(
            //  "~/Content/AuthCenter/adminlet/css/AdminLTE.css", new CssRewriteUrlTransformWrapper()
            //).Include(
            //  "~/Content/AuthCenter/plugins/iCheck/square/blue.css", new CssRewriteUrlTransformWrapper()
            //);

            // var loginScript = new ScriptBundle("~/Content/script/login_js").Include(
            //     "~/Content/AuthCenter/plugins/jQuery/jquery-2.1.4.js",
            //     "~/Content/AuthCenter/bootstrap/js/bootstrap.js",
            //     "~/Content/AuthCenter/plugins/iCheck/icheck.js",
            //     "~/Content/AuthCenter/plugins/bootbox/bootbox.js",
            //     "~/Content/AuthCenter/plugins/blockui/jquery.blockUI.js",
            //     "~/Content/AuthCenter/my-script/jquery-extension.js"
            // );

            bundles.Add(new StyleBundle("~/Content/basic_css").Include(
               "~/Content/AuthCenter/bootstrap/css/bootstrap.min.css", new CssRewriteUrlTransformWrapper()
           ).Include(
              "~/Content/AuthCenter/font-awesome/css/font-awesome.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/ionicons/css/ionicons.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/plugins/select2/select2.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/plugins/iCheck/square/blue.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/plugins/datatables/dataTables.bootstrap.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/plugins/jstree/style.min.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/plugins/jstree/table-tree.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/adminlet/css/AdminLTE.css", new CssRewriteUrlTransformWrapper()
            ).Include(
              "~/Content/AuthCenter/adminlet/css/skins/_all-skins.css", new CssRewriteUrlTransformWrapper()
           )
              //).Include(
            //    "~/Content/assets/css/bluestone.css", new CssRewriteUrlTransformWrapper()
            //)
            );

            bundles.Add(new ScriptBundle("~/bundles/basic_js").Include(
                "~/Content/AuthCenter/plugins/jQuery/jquery-2.1.4.js",
                "~/Content/AuthCenter/bootstrap/js/bootstrap.min.js",
                "~/Content/AuthCenter/plugins/bootbox/bootbox.js",
                "~/Content/AuthCenter/plugins/slimScroll/jquery.slimscroll.min.js",
                "~/Content/AuthCenter/plugins/fastclick/fastclick.js",
                "~/Content/AuthCenter/plugins/datatables/jquery.dataTables.js",
                "~/Content/AuthCenter/plugins/datatables/dataTables.bootstrap.js",
                "~/Content/AuthCenter/plugins/blockui/jquery.blockUI.js",
                "~/Content/AuthCenter/plugins/select2/select2.full.js",
                "~/Content/AuthCenter/plugins/iCheck/icheck.js",
                "~/Content/AuthCenter/plugins/jstree/jstree.js",
                "~/Content/AuthCenter/adminlet/js/app.js",
                "~/Content/AuthCenter/adminlet/js/demo.js",
                "~/Content/AuthCenter/my-script/jquery-extension.js",
                "~/Scripts/Custom/jquery-extension.js",
                //knockout
                "~/Content/AuthCenter/plugins/knockout/knockout.js",
                "~/Content/AuthCenter/plugins/knockout/knockout.validation.js",
                "~/Content/AuthCenter/plugins/knockout/knockout.validation.zh-CN.js",
                "~/Content/AuthCenter/plugins/knockout/ko.mapping.js",
                "~/Content/AuthCenter/my-script/knockout-extension.js"
            ));

            //bundles.Add(loginCssStyles);
            //bundles.Add(loginScript);


            #endregion
        }
    }
}