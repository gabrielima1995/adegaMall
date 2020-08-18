using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Resolvers;
using BundleTransformer.Core.Transformers;
using System.Web.Optimization;

namespace Presentation
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleResolver.Current = new CustomBundleResolver();
            RegisterSiteBundles(bundles);
            RegisterAdminBundles(bundles);
        }

        private static void RegisterSiteBundles(BundleCollection bundles)
        {
            RegisterScriptBundle(bundles, "~/bundles/js",
                "~/assets/js/string.replaceAll.js",
                "~/assets/js/jquery-2.2.4.min.js",
                "~/assets/js/jquery.mask.js",
                "~/assets/js/bootstrap.min.js",
                "~/assets/js/bootstrap-datepicker.min.js",
                "~/assets/js/bootstrap-slider.min.js",
                "~/assets/js/owl.carousel.min.js",
                "~/assets/js/toastr.min.js",
                "~/assets/js/sweetalert.min.js",
                "~/assets/js/jquery.fitvids.js",
                "~/assets/js/jquery.fittext.js",
                "~/assets/js/autosize.min.js",
                "~/assets/js/select2/select2.full.js",
                "~/assets/js/jquery.redirect.js",
                "~/assets/js/jquery.resetValidation.js",
                "~/assets/js/maas.uploadfy.js",
                "~/assets/js/equalheights.js");

            RegisterScriptBundle(bundles, "~/bundles/globalize",
                "~/assets/js/cldr.js",
                "~/assets/js/cldr/event.js",
                "~/assets/js/cldr/supplemental.js",
                "~/assets/js/globalize.js",
                "~/assets/js/globalize/number.js",
                "~/assets/js/globalize/date.js",
                "~/assets/js/globalize/currency.js");

            RegisterScriptBundle(bundles, "~/bundles/mvc-ajax",
                "~/assets/js/jquery.unobtrusive-ajax.js");

            RegisterScriptBundle(bundles, "~/bundles/jqueryval",
                "~/assets/js/jquery.validate.js",
                "~/assets/js/jquery.validate.unobtrusive.js",
                "~/assets/js/jquery.validate.globalize.js",
                "~/assets/js/expressive.annotations.validate.js");

            RegisterScriptBundle(bundles, "~/bundles/datatable",
                "~/assets/js/jquery.dataTables.min.js",
                "~/assets/js/dataTables.bootstrap.min.js");

            RegisterScriptBundle(bundles, "~/bundles/datatable-buttons",
                "~/assets/js/jszip.min.js",
                "~/assets/js/dataTables.buttons.min.js",
                "~/assets/js/buttons.bootstrap.min.js",
                "~/assets/js/buttons.html5.min.js");

            RegisterScriptBundle(bundles, "~/bundles/globalize_pt-br",
                "~/assets/js/globalize.pt-br.js",
                "~/assets/js/messages_pt_BR.js",
                "~/assets/js/select2/i18n/pt-BR.js");

            RegisterScriptBundle(bundles, "~/bundles/globalize_ca-es",
                "~/assets/js/globalize.pt-br.js",
                "~/assets/js/messages_ca.js",
                "~/assets/js/select2/i18n/ca.js");

            RegisterScriptBundle(bundles, "~/bundles/globalize_es-es",
                "~/assets/js/globalize.pt-br.js",
                "~/assets/js/messages_es.js",
                "~/assets/js/select2/i18n/es.js");

            RegisterScriptBundle(bundles, "~/bundles/globalize_en-us",
                "~/assets/js/globalize.en-us.js");

            RegisterScriptBundle(bundles, "~/bundles/scripts",
                "~/assets/js/plugins.js",
                "~/assets/js/scripts.js");

            RegisterScriptBundle(bundles, "~/bundles/scripts/saas",
                "~/assets/js/saas/*.js");

            RegisterScriptBundle(bundles, "~/bundles/scripts/maps-addons",
                "~/assets/js/gmaps-popup.js",
                "~/assets/js/markerclusterer.js");

            RegisterScriptBundle(bundles, "~/bundles/scripts/maps-addresshelper",
                "~/assets/js/gmaps-addresshelper.js");

            #region datetime-picker

            RegisterScriptBundle(bundles, "~/bundles/scripts/datetime-picker",
                "~/Scripts/moment-with-locales.js",
                "~/Scripts/bootstrap-datetimepicker.js");

            RegisterCssBundle(bundles, "~/Content/datetime-picker",
                "~/Content/bootstrap-datetimepicker.css",
                "~/Content/bootstrap-datetimepicker-fix.css");

            #endregion datetime-picker

            #region fullcalendar

            RegisterScriptBundle(bundles, "~/bundles/scripts/fullcalendar",
                "~/assets/js/momentjs/moment.min.js",
                "~/assets/js/fullcalendar/fullcalendar.js",
                "~/assets/js/fullcalendar/locale/pt-br.js");

            RegisterCssBundle(bundles, "~/Content/fullcalendar",
                "~/assets/css/fullcalendar/fullcalendar.min.css");

            #endregion fullcalendar

            #region fullcalendar-scheduler

            RegisterScriptBundle(bundles, "~/bundles/scripts/fullcalendar-scheduler",
                "~/assets/js/fullcalendar-scheduler/core/main.js",
                "~/assets/js/fullcalendar-scheduler/timeline/main.js",
                "~/assets/js/fullcalendar-scheduler/resource-common/main.js",
                "~/assets/js/fullcalendar-scheduler/resource-timeline/main.js",
                "~/assets/js/fullcalendar-scheduler/core/locales/pt-br.js");

            RegisterCssBundle(bundles, "~/Content/fullcalendar-scheduler",
                "~/assets/css/fullcalendar-scheduler/core/main.css",
                "~/assets/css/fullcalendar-scheduler/timeline/main.css",
                "~/assets/css/fullcalendar-scheduler/resource-timeline/main.css");

            #endregion fullcalendar-scheduler

            RegisterCssBundle(bundles, "~/Content/css",
                "~/assets/css/bootstrap.min.css",
                "~/assets/css/bootstrap-panel-with-nav-tabs.css",
                "~/assets/css/bootstrap-datepicker.min.css",
                "~/assets/css/bootstrap-slider.min.css",
                "~/assets/css/bootstrap-submenu.css",
                "~/assets/css/owl.carousel.min.css",
                "~/assets/css/toastr.min.css",
                "~/assets/css/sweetalert.css",
                "~/assets/css/select2/select2.min.css",
                "~/assets/css/select2/select2-bootstrap.css",
                "~/assets/css/stylesheet.sass");

            RegisterCssBundle(bundles, "~/Content/datatable",
                "~/assets/css/dataTables.bootstrap.min.css");

            RegisterCssBundle(bundles, "~/Content/datatable-buttons",
                "~/assets/css/buttons.bootstrap.min.css");

            RegisterCssBundle(bundles, "~/Content/search-page",
                "~/assets/css/searchpage.css");
        }

        private static void RegisterAdminBundles(BundleCollection bundles)
        {
            RegisterCssBundle(bundles, "~/bundles/admin/css",
                "~/Areas/admin/assets/css/bootstrap.min.css",
                "~/Areas/Admin/assets/css/font-awesome.min.css",
                "~/Areas/Admin/assets/css/ionicons.min.css",
                "~/Areas/Admin/assets/css/dataTables.bootstrap.min.css",
                "~/Areas/Admin/assets/css/bootstrap-datepicker.min.css",
                "~/Areas/Admin/assets/css/bootstrap-colorpicker.min.css",
                "~/assets/css/buttons.bootstrap.min.css",
                "~/assets/css/toastr.min.css",
               "~/assets/css/sweetalert.css",
                "~/Areas/Admin/assets/css/select2.min.css",
                // Admin LTE
                "~/Areas/Admin/assets/css/adminlte.css",
                "~/Areas/Admin/assets/css/adminlte-skin-blue.min.css",
                // Custom
                "~/Areas/Admin/assets/css/custom.css");

            RegisterScriptBundle(bundles, "~/bundles/admin/js",
                "~/Areas/Admin/assets/js/jquery.min.js",
                "~/Areas/Admin/assets/js/bootstrap.min.js",
                "~/Areas/Admin/assets/js/adminlte.min.js",
                "~/assets/js/jquery.unobtrusive-ajax.js",
                // SaaS custom
                "~/Areas/Admin/assets/js/saas/*.js",
                // Other components
                "~/Areas/Admin/assets/js/bootstrap-datepicker.min.js",
                "~/Areas/Admin/assets/js/bootstrap-colorpicker.min.js",
                "~/Areas/Admin/assets/js/select2.full.min.js",
                "~/assets/js/autosize.min.js",
                "~/assets/js/toastr.min.js",
                "~/assets/js/sweetalert.min.js",
                "~/assets/js/jquery.redirect.js",
                "~/assets/js/maas.uploadfy.js",
                "~/Areas/Admin/assets/js/jquery.mask.js",
                "~/Areas/Admin/assets/js/equalheights.js",
                // Golbalize
                "~/assets/js/cldr.js",
                "~/assets/js/cldr/event.js",
                "~/assets/js/cldr/supplemental.js",
                "~/assets/js/globalize.js",
                "~/assets/js/globalize/number.js",
                "~/assets/js/globalize/date.js",
                "~/assets/js/globalize/currency.js",
                // Jquery Val
                "~/assets/js/jquery.validate.js",
                "~/assets/js/jquery.validate.unobtrusive.js",
                "~/assets/js/jquery.validate.globalize.js",
                "~/assets/js/expressive.annotations.validate.js",
                // DataTables
                "~/Areas/Admin/assets/js/jquery.dataTables.min.js",
                "~/Areas/Admin/assets/js/dataTables.bootstrap.min.js",
                // DataTable Buttons
                "~/assets/js/jszip.min.js",
                "~/assets/js/dataTables.buttons.min.js",
                "~/assets/js/buttons.bootstrap.min.js",
                "~/assets/js/buttons.html5.min.js",
                // DataTable plugins
                "~/assets/js/dataTables.dateUkTypeDetect.js",
                // Globalize PT-BR
                "~/assets/js/globalize.pt-br.js",
                "~/assets/js/messages_pt_BR.js",
                "~/assets/js/select2/i18n/pt-BR.js",
                // Scripts
                "~/assets/js/plugins.js",
                "~/Areas/Admin/assets/js/scripts.js");
        }

        private static void RegisterCssBundle(BundleCollection bundles, string name, params string[] includes)
        {
            RegisterBundle(bundles, false, name, includes);
        }

        private static void RegisterScriptBundle(BundleCollection bundles, string name, params string[] includes)
        {
            RegisterBundle(bundles, true, name, includes);
        }

        private static void RegisterBundle(BundleCollection bundles, bool isScript, string name, params string[] includes)
        {
            var bundle = new Bundle(name);

            bundle.Include(includes);
            bundle.Builder = new NullBuilder();
            bundle.Orderer = new NullOrderer();

            if (isScript) bundle.Transforms.Add(new ScriptTransformer());
            else bundle.Transforms.Add(new StyleTransformer());

            bundles.Add(bundle);
        }
    }
}