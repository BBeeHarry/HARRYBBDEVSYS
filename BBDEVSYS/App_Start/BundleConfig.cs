using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Bundles;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using System.Web;
using System.Web.Optimization;

namespace BBDEVSYS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //=======Login Scripts======
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Content/js/plugins/validator/jquery.validate*"));
            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          "~/Content/bootstrap.css",
            //          "~/Content/site.css"));
            //------------------------------------------------------------
            //bundles.UseCdn = true;

            //var nullBuilder = new NullBuilder();
            //var nullOrderer = new NullOrderer();

            //BundleResolver.Current = new CustomBundleResolver();

            //var commonStylesBundle = new CustomStyleBundle("~/bundles/css");
            //commonStylesBundle.Include(
            //    "~/Content/css/bootstrap.css",
            //    "~/Content/css/select2.css",
            //    "~/Content/css/datepicker3.css",
            //    "~/Content/css/AdminLTE.css",
            //    "~/Content/css/font-awesome.css",
            //    "~/Content/css/skins/skin-blue.css",
            //    "~/Content/css/skins/skin-yellow.css",
            //    "~/Content/css/fileupload/jquery.fileupload.css",
            //    "~/Content/css/datatables/dataTables.bootstrap.css",
            //    "~/Content/css/datatables/dataTables.responsive.css",
            //    "~/Content/css/jqueryui/*.css",
            //    "~/Content/css/app-style.less",
            //    "~/Content/css/multiselect/style.css",
            //    "~/Content/css/themify-icons.css");
            //commonStylesBundle.Orderer = nullOrderer;
            ////commonStylesBundle.Transforms.Add(new StyleTransformer()); 
            //bundles.Add(commonStylesBundle);


            //var jqueryScriptsBundle = new CustomScriptBundle("~/bundles/jquery");
            //jqueryScriptsBundle.Include(
            //    "~/Content/js/plugins/jquery/jquery-3.1.0.js",
            //    "~/Content/js/plugins/jquery/jquery-migrate-1.4.1.min.js");
            //jqueryScriptsBundle.Orderer = nullOrderer;
            //bundles.Add(jqueryScriptsBundle);

            //var jsScriptsBundle = new CustomScriptBundle("~/bundles/js");
            //jsScriptsBundle.Include(
            //    "~/Content/js/plugins/bootstrap/bootstrap.js",
            //    "~/Content/js/plugins/fastclick/fastclick.js",
            //    "~/Content/js/plugins/slimscroll/jquery.slimscroll.js",
            //    "~/Content/js/plugins/select2/select2.full.js",
            //    "~/Content/js/plugins/moment/moment.js",
            //    "~/Content/js/plugins/icheck/icheck.js",
            //    "~/Content/js/plugins/validator/jquery.validate-vsdoc.js",
            //    "~/Content/js/plugins/validator/jquery.validate.js",
            //    "~/Content/js/plugins/validator/jquery.validate.date.js",
            //    "~/Content/js/plugins/validator/jquery.validate.unobtrusive.js",
            //    "~/Content/js/plugins/blockui/jquery.blockUI.js",
            //    "~/Content/js/plugins/inputmask/jquery.inputmask.bundle.js",
            //    "~/Content/js/plugins/uiwidget/jquery.ui.widget.js",
            //    "~/Content/js/plugins/fileupload/jquery.iframe-transport.js",
            //    "~/Content/js/plugins/fileupload/jquery.fileupload.js",
            //    "~/Content/js/plugins/fileupload/jquery.fileupload-process.js",
            //    "~/Content/js/plugins/fileupload/jquery.fileupload-validate.js",
            //    "~/Content/js/plugins/fileupload/jquery.fileupload-ui.js",
            //    "~/Content/js/plugins/datatables/jquery.dataTables.js",
            //    "~/Content/js/plugins/datatables/dataTables.bootstrap.js",
            //    "~/Content/js/plugins/datatables/dataTables.responsive.js",
            //    "~/Content/js/plugins/jqueryui/jquery-ui.min.js",
            //    "~/Content/js/plugins/datatables/currency-sorting.js",
            //    "~/Content/js/plugins/datatables/date-dd-MMM-yyyy-sorting.js",
            //    "~/Content/js/plugins/datatables/dataTables.customizing.js",
            //    "~/Content/js/plugins/datepicker/jquery-ui-1.8.10.offset.datepicker.min.js",
            //    "~/Content/js/app.js",
            //    "~/Content/js/input.initialization.js",
            //    "~/Content/js/plugins/multiselect/multiselect.min.js");
            ////jsScriptsBundle.Orderer = nullOrderer;
            //bundles.Add(jsScriptsBundle);

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                       "~/Content/css/bootstrap.css",
                        "~/Content/css/select2.css",
                         //"~/Content/css/datepicker3.css",
                         "~/Content/css/datepicker/bootstrap-datepicker3.css",
                        "~/Content/css/AdminLTE.css",
                        "~/Content/css/font-awesome.css",
                        "~/Content/css/skins/skin-red.css",
                        "~/Content/css/skins/skin-yellow.css",
                        "~/Content/css/fileupload/jquery.fileupload.css",
                        "~/Content/css/datatables/dataTables.bootstrap.css",
                        "~/Content/css/datatables/dataTables.responsive.css",
                        "~/Content/css/jqueryui/*.css",
                        "~/Content/css/app-style.less",
                        "~/Content/css/multiselect/style.css",
                        "~/Content/css/themify-icons.css",
                        "~/Content/c3.css"
                         ));

            //bundles.Add(new StyleBundle("~/Content/"));


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Content/js/plugins/jquery/jquery-3.1.0.js",
                       "~/Content/js/plugins/jquery/jquery-migrate-1.4.1.min.js"
                        ));



            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                       "~/Content/js/plugins/bootstrap/bootstrap.js",
                         "~/Scripts/c3.js",
                        "~/Scripts/d3.js",
                        "~/Content/js/plugins/fastclick/fastclick.js",
                        "~/Content/js/plugins/slimscroll/jquery.slimscroll.js",
                        "~/Content/js/plugins/select2/select2.full.js",
                        "~/Content/js/plugins/moment/moment.js",
                        "~/Content/js/plugins/icheck/icheck.js",
                        "~/Content/js/plugins/validator/jquery.validate-vsdoc.js",
                        "~/Content/js/plugins/validator/jquery.validate.js",
                        "~/Content/js/plugins/validator/jquery.validate.date.js",
                        "~/Content/js/plugins/validator/jquery.validate.unobtrusive.js",
                        "~/Content/js/plugins/blockui/jquery.blockUI.js",
                        "~/Content/js/plugins/inputmask/jquery.inputmask.bundle.js",
                        "~/Content/js/plugins/uiwidget/jquery.ui.widget.js",
                        "~/Content/js/plugins/fileupload/jquery.iframe-transport.js",
                        "~/Content/js/plugins/fileupload/jquery.fileupload.js",
                        "~/Content/js/plugins/fileupload/jquery.fileupload-process.js",
                        "~/Content/js/plugins/fileupload/jquery.fileupload-validate.js",
                        "~/Content/js/plugins/fileupload/jquery.fileupload-ui.js",
                        "~/Content/js/plugins/datatables/jquery.dataTables.js",
                        "~/Content/js/plugins/datatables/dataTables.bootstrap.js",
                        "~/Content/js/plugins/datatables/dataTables.responsive.js",
                        "~/Content/js/plugins/jqueryui/jquery-ui.min.js",
                        "~/Content/js/plugins/datatables/currency-sorting.js",
                        "~/Content/js/plugins/datatables/date-dd-MMM-yyyy-sorting.js",
                        "~/Content/js/plugins/datatables/dataTables.customizing.js",
                        "~/Content/js/plugins/datepicker/jquery-ui-1.8.10.offset.datepicker.min.js",
                        //"~/Content/js/plugins/datepicker/bootstrap-datepicker.js",
                        //"~/Content/js/plugins/datepicker/en-boostrap-datepicker/bootstrap-datepicker.js",
                        //C:\BbHarryDevSYS\BBDEVSYS-Dev2\BBDEVSYS\Content\js\plugins\chartjs\Chart.js
                        //Chart
                        "~/Content/js/plugins/chartjs/Chart.js",
                        "~/Content/js/plugins/chartjs/canvasjs.min.js",
                        "~/Content/js/app.js",
                        "~/Content/js/input.initialization.js",
                        "~/Content/js/plugins/multiselect/multiselect.min.js"
                      
                        ));
        }
    }
}
