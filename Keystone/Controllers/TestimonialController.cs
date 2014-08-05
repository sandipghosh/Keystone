
namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class TestimonialController : BaseController
    {
        private readonly ITestimonialDataRepository _testimonialDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestimonialController"/> class.
        /// </summary>
        /// <param name="testimonialDataRepository">The testimonial data repository.</param>
        public TestimonialController(ITestimonialDataRepository testimonialDataRepository)
        {
            this._testimonialDataRepository = testimonialDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets the testimonials.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult GetTestimonials()
        {
            try
            {
                List<TestimonialModel> testimonials = this._testimonialDataRepository
                    .GetList(x => x.StatusId.Equals((int)StatusEnum.Active)).ToList();

                return PartialView("_TestimonialList", testimonials);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return null;
        }
    }

}

