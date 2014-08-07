

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

    public class PriceSummaryController : BaseController
    {
        private readonly ITemplateDataRepository _templateDataRepository;
        private readonly IDeliveryScheduleDataRepository _deliveryScheduleDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceSummaryController"/> class.
        /// </summary>
        /// <param name="templateDataRepository">The template data repository.</param>
        /// <param name="deliveryScheduleDataRepository">The delivery schedule data repository.</param>
        public PriceSummaryController(ITemplateDataRepository templateDataRepository,
            IDeliveryScheduleDataRepository deliveryScheduleDataRepository)
        {
            this._templateDataRepository = templateDataRepository;
            this._deliveryScheduleDataRepository = deliveryScheduleDataRepository;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Selecteds the index.
        /// </summary>
        /// <param name="templateid">The templateid.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SelectedIndex(int templateid)
        {
            try
            {
                IEnumerable<TemplateModel> templates = this._templateDataRepository
                    .GetList(x => x.StatusId.Equals((int)StatusEnum.Active));

                TemplateModel template = templates.FirstOrDefault(x => x.TemplateId.Equals(templateid));

                var quantity = CommonUtility.GetQuantityByTemplateAndDeliverySchedule
                    (templateid, (int)DeliveryScheduleEnum.StandardTurnaround, template);

                ViewBag.Quantity = new SelectList(quantity, "Value", "Text", quantity.SingleOrDefault(x => x.Selected == true));
                ViewBag.Templates = templates;
                ViewBag.Price = string.Format("${0:0.00}", quantity.FirstOrDefault(x => x.Selected == true).Price);
                ViewBag.DeliverySchedule = _deliveryScheduleDataRepository
                    .GetList(x => x.StatusId.Equals((int)StatusEnum.Active)).ToList();
                ViewBag.TemplateId = templateid;

                return View(template);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(templateid);
            }
            return null;
        }

        /// <summary>
        /// Goes to editor.
        /// </summary>
        /// <param name="templateid">The templateid.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public RedirectToRouteResult GoToEditor(int templateid)
        {
            try
            {
                TemplateModel selectedTemplate = _templateDataRepository.Get(templateid);
                CommonUtility.SetSessionData<TemplateModel>(SessionVariable.SelectedTemplate, selectedTemplate);
                return RedirectToAction("Index", "Editor");
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(templateid);
            }
            return null;
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <param name="deliveryScheduleId">The delivery schedule id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult GetPrice(int templateId, int deliveryScheduleId, int quantity)
        {
            decimal price = 0;
            try
            {
                price = CommonUtility.GetPriceByTemplateAndDeliveryScheduleAndQuantity
                    (templateId, deliveryScheduleId, quantity);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(templateId, deliveryScheduleId, quantity);
            }

            return Json(new { Price = price }, JsonRequestBehavior.AllowGet);
        }
    }
}
