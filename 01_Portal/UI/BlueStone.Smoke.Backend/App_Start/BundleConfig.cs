using System.Web;
using System.Web.Optimization;

namespace BlueStone.Smoke.Backend
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                     "~/Content/assets/js/skins.min.js",
                     "~/Content/assets/js/jquery-3.1.0.min.js",
                     "~/Content/assets/js/jquery.json.min.js",
                     "~/Scripts/Custom/jquery-extension.js",
                     "~/Content/assets/js/bootstrap.min.js",
                     "~/Content/assets/js/beyond.min.js",
                     "~/Content/assets/js/select2/select2.js",
                      "~/Content/AuthCenter/plugins/iCheck/icheck.js",
                    //格式化日期
                    "~/Content/assets/js/datetime/moment.js",
                     //弹框
                     "~/Content/assets/js/bootbox/bootbox.js",
                     "~/Content/assets/js/jquery.blockui.min.js",
                     "~/Content/assets/js/toastr/toastr.js",
                     //ko
                     "~/Content/assets/js/knockout-3.4.0.js",
                     "~/Scripts/Custom/knockout.extend.js",
                     "~/Content/assets/js/knockout.validation.js",
                     "~/Content/assets/js/knockout.validation.zh-CN.js",
                    "~/Content/AuthCenter/plugins/knockout/ko.mapping.js",
                    "~/Content/AuthCenter/my-script/knockout-extension.js",
                     //bootstrap验证
                     "~/Content/assets/js/validation/bootstrapValidator.js"
                     ));
            //其它插件
            bundles.Add(new ScriptBundle("~/bundles/My97DatePicker").Include(
                    "~/Content/assets/js/My97DatePicker/WdatePicker.js",
                    "~/Scripts/Custom/WdatePicker.helper.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                     //datatabe
                     "~/Content/assets/js/datatables/js/jquery.dataTables.min.js",
                     "~/Content/assets/js/datatables/js/dataTables.bootstrap.min.js",
                     "~/Content/assets/js/datatables/js/dataTables.select.min.js",
                     "~/Scripts/Custom/datatable-helper.js"
                    //"~/Scripts/Custom/datatable-helper.bak.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/ueditor").Include(
                    //ueditor
                    "~/Content/assets/ueditor/ueditor.config.js",
                    "~/Content/assets/ueditor/ueditor.all.js",
                    "~/Content/assets/ueditor/lang/zh-cn/zh-cn.js"
                    ));


            bundles.Add(new ScriptBundle("~/bundles/jstree").Include(
                  //jstree
                  "~/Content/assets/js/jstree/jstree.min.js"
                  ));
            bundles.Add(new ScriptBundle("~/bundles/fileinput").Include(
                 // , "~/Content/assets/js/bootstrap-fileinput-4.4.3/themes/explorer/theme.js"
                 "~/Content/assets/js/bootstrap-fileinput-4.4.3/js/fileinput.js",
                 "~/Content/assets/js/bootstrap-fileinput-4.4.3/js/locales/zh.js",
                 "~/Content/assets/js/bootstrap-fileinput-4.4.9/js/plugins/sortable.min.js"

                 //,"~/Content/assets/js/bootstrap-fileinput-4.4.3/themes/explorer-fa/theme.js"
                 //,"~/Content/assets/js/bootstrap-fileinput-4.4.3/themes/fa/theme.js"

                 ));

            bundles.Add(new StyleBundle("~/Content/ueditor").Include(
                    //ueditor
                    "~/Content/assets/ueditor/themes/default/css/ueditor.min.css"
            ));

            bundles.Add(new StyleBundle("~/Content/datatable")
                .Include("~/Content/assets/js/datatables/css/dataTables.bootstrap.min.css", new CssRewriteUrlTransformWrapper())
            );
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/assets/css/bootstrap.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/assets/css/font-awesome.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/assets/css/weather-icons.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/assets/css/beyond.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/assets/css/typicons.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/assets/css/animate.min.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/assets/css/bluestone.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/assets/css/layout.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/AuthCenter/plugins/iCheck/square/blue.css", new CssRewriteUrlTransformWrapper())
            );

            bundles.Add(new StyleBundle("~/Content/jstree").Include(
                  //datatable
                  "~/Content/assets/js/jstree/css/style.min.css",
                  "~/Content/assets/js/jstree/css/add.css"
            ));
            bundles.Add(new StyleBundle("~/Content/ueditor").Include(
                  //datatable
                  "~/Content/assets/ueditor/themes/default/css/ueditor.css"
            ));
            bundles.Add(new StyleBundle("~/Content/fileinput").Include(
                 "~/Content/assets/js/bootstrap-fileinput-4.4.3/css/fileinput.css"
                 , "~/Content/assets/js/bootstrap-fileinput-4.4.3/themes/explorer/theme.css"
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