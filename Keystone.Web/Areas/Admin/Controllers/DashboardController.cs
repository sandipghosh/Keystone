
namespace Keystone.Web.Areas.Admin.Controllers
{
    using Keystone.Web.Controllers.Base;
    using System.Web.Mvc;

    [SignInActionValidator(true)]
    public class DashboardController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
