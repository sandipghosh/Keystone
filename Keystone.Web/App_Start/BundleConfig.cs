
namespace Keystone.Web
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterScriptBundles(bundles);
            RegisterStyleBundles(bundles);
        }

        #region Private Members
        /// <summary>
        /// Registers the script bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Common")
                .Include("~/Scripts/modernizr.js",
                "~/Scripts/consolelog.min.js",
                "~/Scripts/jquery-2.1.0.min.js",
                "~/Scripts/jquery-migrate-1.2.1.min.js",
                "~/Scripts/jquery.idletimer.js",
                "~/Scripts/jquery.idletimeout.js",
                "~/Scripts/jquery-ui-1.10.4.custom.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.blockUI.js",
                "~/Scripts/jquery.jcarousel.min.js",
                "~/Scripts/jquery.fs.boxer.min.js",
                "~/Scripts/jquery.cookie.js",
                "~/Scripts/jquery.fileDownload.js",
                "~/Scripts/jquery.form.min.js",
                "~/Scripts/linq.min.js",
                "~/Scripts/json3.js",
                "~/Scripts/common-script.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Fabric")
                //.Include("~/Scripts/fabric.min.js",
                .Include("~/Scripts/fabric.js",
                "~/Scripts/fabric-extension.js",
                "~/Scripts/jquery.ruler.js",
                "~/Scripts/simple-slider.min.js",
                "~/Scripts/dropzone.min.js",
                "~/Scripts/image-editor.js",
                "~/Scripts/ToolbarScript.js",
                "~/Scripts/EditorScript.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Knockout")
                .Include("~/Scripts/knockout-3.0.0.js",
                "~/Scripts/knockout.mapping-2.4.1.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Grid")
                .Include("~/Areas/Admin/Scripts/jquery.jqGrid.min.js",
                "~/Areas/Admin/Scripts/grid.locale-en.js",
                "~/Areas/Admin/Scripts/jquery.linq-2.2.0.2.min.js",
                "~/Areas/Admin/Scripts/grid-intregration.js"));
        }

        /// <summary>
        /// Registers the style bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Styles/Common")
                .Include("~/Styles/MainStyle.css",
                "~/Styles/jquery-ui-1.10.4.custom.min.css",
                "~/Styles/jquery.fs.boxer.css"));

            bundles.Add(new ScriptBundle("~/Styles/Editor")
                .Include("~/Styles/dropzone.css",
                "~/Styles/ruler.css",
                "~/Styles/ToolbarStyle.css"));

            bundles.Add(new ScriptBundle("~/Styles/Admin")
                .Include("~/Areas/Admin/Styles/ui.jqgrid.css",
                "~/Areas/Admin/Styles/AdminStyle.css"));
        }
        #endregion
    }
}