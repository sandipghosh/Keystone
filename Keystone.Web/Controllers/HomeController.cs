
namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Net;
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

        public ActionResult SendMail()
        {
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                message.Subject = "Test";
                message.From = new System.Net.Mail.MailAddress("info@gt-keystone.com", "info");
                message.Body = "Testing";
                //message.Priority = System.Web.Mail.MailPriority.Normal;
                //message.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
                //message.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");
                message.IsBodyHtml = false;
                message.To.Add("order@gt-keystone.com");

                System.Net.Mail.SmtpClient smtp =
                    new System.Net.Mail.SmtpClient("dedrelay.secureserver.net", 25);

                //System.Net.Mail.SmtpClient smtp =
                //    new System.Net.Mail.SmtpClient("relay-hosting.secureserver.net", 25);


                //System.Security.SecureString password = new System.Security.SecureString()
                //smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("info@gt-keystone.com", "lgtmltd51");
                //smtp.EnableSsl = false;
                smtp.Send(message);

                return Content("Success");
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return Content("failed");
        }
    }
}
