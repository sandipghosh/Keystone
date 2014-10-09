
namespace Keystone.Web
{
    using System.Web.Optimization;
    using Keystone.Web.Utilities;

    public class BundleConfig
    {
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            try
            {
                RegisterScriptBundles(bundles);
                RegisterStyleBundles(bundles);
            }
            catch (System.Exception ex)
            {
                ex.ExceptionValueTracker(bundles);
            }
        }

        #region Private Members
        /// <summary>
        /// Registers the script bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        private static void RegisterScriptBundles(BundleCollection bundles)
        {
            try
            {
                Bundle scriptBundle = new Bundle("~/Scripts/Common", new JsMinify());
                scriptBundle.Include("~/Scripts/modernizr.js",
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
                    "~/Scripts/jquery.creditCardValidator.js",
                    "~/Scripts/linq.min.js",
                    "~/Scripts/json3.js",
                    "~/Scripts/common-script.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/Fabric", new JsMinify());
                scriptBundle.Include("~/Scripts/fabric.js",
                    "~/Scripts/fabric-extension.js",
                    "~/Scripts/jquery.ruler.js",
                    "~/Scripts/simple-slider.min.js",
                    "~/Scripts/dropzone.min.js",
                    "~/Scripts/image-editor.js",
                    "~/Scripts/ToolbarScript.js",
                    "~/Scripts/EditorScript.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/Knockout", new JsMinify());
                scriptBundle.Include("~/Scripts/knockout-3.0.0.js",
                    "~/Scripts/knockout.mapping-2.4.1.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/Grid", new JsMinify());
                scriptBundle.Include("~/Areas/Admin/Scripts/jquery.jqGrid.min.js",
                    "~/Areas/Admin/Scripts/grid.locale-en.js",
                    "~/Areas/Admin/Scripts/jquery.linq-2.2.0.2.min.js",
                    "~/Areas/Admin/Scripts/grid-intregration.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/ShoppingCart", new JsMinify());
                scriptBundle.Include("~/Scripts/ShoppingCart.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/HomeScript", new JsMinify());
                scriptBundle.Include("~/Scripts/HomeScript.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/Payment", new JsMinify());
                scriptBundle.Include("~/Scripts/Payment.js", "~/Scripts/jquery.creditCardValidator.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/PaymentManager", new JsMinify());
                scriptBundle.Include("~/Areas/Admin/Scripts/payment-manager.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/OrderManager", new JsMinify());
                scriptBundle.Include("~/Areas/Admin/Scripts/order-manager.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/CustomarManager", new JsMinify());
                scriptBundle.Include("~/Areas/Admin/Scripts/customer-manager.js");
                BundleTable.Bundles.Add(scriptBundle);

                scriptBundle = new Bundle("~/Scripts/TestimonialManager", new JsMinify());
                scriptBundle.Include("~/Areas/Admin/Scripts/testimonial-manager.js");
                BundleTable.Bundles.Add(scriptBundle);
            }
            catch (System.Exception ex)
            {
                ex.ExceptionValueTracker(bundles);
            }
        }

        /// <summary>
        /// Registers the style bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            try
            {
                Bundle styleBundle = new Bundle("~/Styles/Common", new CssMinify());
                styleBundle.Include("~/Styles/MainStyle.css",
                    "~/Styles/jquery-ui-1.10.4.custom.min.css",
                    "~/Styles/jquery.fs.boxer.css");
                BundleTable.Bundles.Add(styleBundle);

                styleBundle = new Bundle("~/Styles/Editor", new CssMinify());
                styleBundle.Include("~/Styles/dropzone.css",
                    "~/Styles/ruler.css",
                    "~/Styles/ToolbarStyle.css");
                BundleTable.Bundles.Add(styleBundle);

                styleBundle = new Bundle("~/Styles/Admin", new CssMinify());
                styleBundle.Include("~/Areas/Admin/Styles/ui.jqgrid.css",
                    "~/Areas/Admin/Styles/AdminStyle.css");
                BundleTable.Bundles.Add(styleBundle);

                styleBundle = new Bundle("~/Styles/TestimonialStyle", new CssMinify());
                styleBundle.Include("~/Styles/TestimonialStyle.css");
                BundleTable.Bundles.Add(styleBundle);
            }
            catch (System.Exception ex)
            {
                ex.ExceptionValueTracker(bundles);
            }
        }
        #endregion
    }
}