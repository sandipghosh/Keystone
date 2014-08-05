using Keystone.Web.Controllers.Base;
using System.Web.Mvc;

namespace Keystone.Web.Controllers
{
    public class ShippingController : BaseController
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

    }
}
