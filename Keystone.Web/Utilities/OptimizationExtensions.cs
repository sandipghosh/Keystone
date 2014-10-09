

namespace Keystone.Web.Utilities
{
    using Microsoft.Ajax.Utilities;
    using System;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Configuration;

    public static class OptimizationExtensions
    {
        /// <summary>
        /// Gets a value indicating whether this instance is js minify.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is js minify; otherwise, <c>false</c>.
        /// </value>
        public static bool IsJsMinify { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["JsMinify"] ?? "false"); } }
        /// <summary>
        /// Gets a value indicating whether this instance is CSS minify.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is CSS minify; otherwise, <c>false</c>.
        /// </value>
        public static bool IsCssMinify { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["CssMinify"] ?? "false"); } }

        /// <summary>
        /// Jses the minify.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="markup">The markup.</param>
        /// <returns></returns>
        public static MvcHtmlString JsMinify(this HtmlHelper helper, Func<object, object> markup)
        {
            if (helper == null || markup == null)
            {
                return MvcHtmlString.Empty;
            }

            var sourceJs = (markup.DynamicInvoke(helper.ViewContext) ?? String.Empty).ToString();

            if (!IsJsMinify)
            {
                return new MvcHtmlString(sourceJs);
            }

            var minifier = new Minifier();

            var minifiedJs = minifier.MinifyJavaScript(sourceJs, new CodeSettings
            {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            });

            return new MvcHtmlString(minifiedJs);
        }

        /// <summary>
        /// CSSs the minify.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="markup">The markup.</param>
        /// <returns></returns>
        public static MvcHtmlString CssMinify(this HtmlHelper helper, Func<object, object> markup)
        {
            if (helper == null || markup == null)
            {
                return MvcHtmlString.Empty;
            }

            var sourceCss = (markup.DynamicInvoke(helper.ViewContext) ?? String.Empty).ToString();

            if (!IsJsMinify)
            {
                return new MvcHtmlString(sourceCss);
            }

            var minifier = new Minifier();

            var minifiedCss = minifier.MinifyStyleSheet(sourceCss, new CssSettings
            {
                CommentMode = CssComment.None
            });

            return new MvcHtmlString(minifiedCss);
        }
    }
}