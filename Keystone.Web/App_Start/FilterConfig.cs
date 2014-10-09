
namespace Keystone.Web
{
    using System.Web.Mvc;
    using Keystone.Web.Utilities;

    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            try
            {
                filters.Add(new HandleErrorAttribute());
                filters.Add(new CustomGlobalFilters());
                filters.Add(new FileDownloadAttribute());
            }
            catch (System.Exception ex)
            {
                ex.ExceptionValueTracker(filters);
            }
        }
    }
}