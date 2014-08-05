
namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class HomeController : BaseController
    {
        private readonly ITemplateDataRepository _templateDataRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="templateDataRepository">The template data repository.</param>
        public HomeController(ITemplateDataRepository templateDataRepository)
        {
            this._templateDataRepository = templateDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index()
        {
            try
            {
                IEnumerable<TemplateModel> templates= this._templateDataRepository.GetList();
                return View(templates);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return null;
        }
    }
}
