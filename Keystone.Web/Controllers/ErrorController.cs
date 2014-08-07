
namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Utilities;
    using System.Web.Mvc;

    public class ErrorController : BaseController
    {
        /// <summary>
        /// Indexes the specified error MSG.
        /// </summary>
        /// <param name="errorMsg">The error MSG.</param>
        /// <returns></returns>
        public ActionResult Index(string errorMsg)
        {
            if (string.IsNullOrEmpty(errorMsg))
                errorMsg = ("Computer left idle message. Your session has timed out. Please log back in").ToBase64Encode();

            ViewBag.ErrorMessage = errorMsg;
            return View();
        }
    }
}
