

namespace Keystone.Web
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Keystone.Web.Utilities;
    using System.Web;

    public class CustomGlobalFilters : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"];
            filterContext.Controller.ViewBag.Title = string.Format("Welcome to Keystone Industries | {0}", controllerName);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = false, Inherited = false)]
    public class SignInActionValidator : ActionFilterAttribute
    {
        private bool _validateAdimnUser = false;
        public SignInActionValidator(bool validateAdimnUser = false)
        {
            this._validateAdimnUser = validateAdimnUser;
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAccess), false)
                    || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAccess), false);
                if (skipAuthorization)
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    RedirectToRouteResult redirect = new RedirectToRouteResult(new RouteValueDictionary{
                    { "action", "ShowSignInScreen" },
                    { "controller", "UserAccount" },
                    { "area", "" },
                    { "referalUrl", filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri.ToBase64Encode() }
                });

                    if (filterContext.RequestContext.HttpContext.Session == null)
                        filterContext.Result = redirect;
                    else
                    {
                        if (filterContext.RequestContext.HttpContext.Session[SessionVariable.UserId] == null)
                            filterContext.Result = redirect;
                        else
                            if (this._validateAdimnUser)
                            {
                                if (!Convert.ToBoolean(filterContext.RequestContext.HttpContext.Session[SessionVariable.IsAdminUser]))
                                    filterContext.Result = redirect;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(filterContext);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class AllowAnonymousAccess : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class FileDownloadAttribute : ActionFilterAttribute
    {
        public FileDownloadAttribute(string cookieName = "fileDownload", string cookiePath = "/")
        {
            CookieName = cookieName;
            CookiePath = cookiePath;
        }

        public string CookieName { get; set; }

        public string CookiePath { get; set; }

        /// <summary>
        /// If the current response is a FileResult (an MVC base class for files) then write a
        /// cookie to inform jquery.fileDownload that a successful file download has occured
        /// </summary>
        /// <param name="filterContext"></param>
        private void CheckAndHandleFileResult(ActionExecutedContext filterContext)
        {
            try
            {
                var httpContext = filterContext.HttpContext;
                var response = httpContext.Response;

                if (filterContext.Result is FileResult)
                    //jquery.fileDownload uses this cookie to determine that a file download has completed successfully
                    response.AppendCookie(new HttpCookie(CookieName, "true") { Path = CookiePath });
                else
                    //ensure that the cookie is removed in case someone did a file download without using jquery.fileDownload
                    if (httpContext.Request.Cookies[CookieName] != null)
                    {
                        response.AppendCookie(new HttpCookie(CookieName, "true") { Expires = DateTime.Now.AddYears(-1), Path = CookiePath });
                    }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(filterContext);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                CheckAndHandleFileResult(filterContext);
                base.OnActionExecuted(filterContext);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(filterContext);
            }
        }
    }
}