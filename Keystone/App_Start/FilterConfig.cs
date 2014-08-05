
namespace Keystone.Web
{
    using System.Web.Mvc;

    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomGlobalFilters());
            filters.Add(new FileDownloadAttribute());
        }
    }
}