
namespace Keystone.Web.Security
{
    using System;
    using System.Web;
    using Keystone.Web.Utilities;
    using System.Configuration;
    public class SecureQueryStringModule : IHttpModule
    {
        private readonly IStateSaltManager _stateSaltManager;

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureQueryStringModule"/> class.
        /// </summary>
        public SecureQueryStringModule()
        {
            this._stateSaltManager = new SessionStateSaltManager();
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            //context.AcquireRequestState += new EventHandler(context_AcquireRequestState);
        }

        /// <summary>
        /// Handles the BeginRequest event of the context control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                if (HttpContext.Current.Request.QueryString["v"] == null)
                {
                    Process(DecryptProcess);
                    Process(EncryptProcess);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(sender, e);
            }
        }

        /// <summary>
        /// Processes the specified cripto process.
        /// </summary>
        /// <param name="criptoProcess">The cripto process.</param>
        private void Process(Action<IStateSaltManager, HttpContext, string, string> criptoProcess)
        {
            HttpContext context = HttpContext.Current;

            //if (!context.Request.IsAjaxRequest())
            {
                if (context.Request.RawUrl.Contains("?"))
                {
                    string queryString = ExtractQuery(context.Request.RawUrl);
                    string path = GetVirtualPath();

                    criptoProcess(this._stateSaltManager, context, path, queryString);
                }
            }
        }

        #region Encryption/Decryption Deligates
        /// <summary>
        /// The encrypt process
        /// </summary>
        Action<IStateSaltManager, HttpContext, string, string> EncryptProcess
            = (stateSaltManager, context, path, queryString) =>
            {
                try
                {
                    if (!queryString.StartsWith(CommonUtility.SecureQueryStringKey, StringComparison.OrdinalIgnoreCase) &&
                        context.Request.HttpMethod == "GET" &&
                        !System.Text.RegularExpressions.Regex.IsMatch(path, @"/.+\.([A-z]{2,4})$"))
                    {
                        // Encrypt the query string and redirects to the encrypted URL.
                        // Remove if you don't want all query strings to be encrypted automatically.
                        string encryptedQuery = stateSaltManager.SetSaltToState(context, queryString);
                        //Utility.LogToFile(string.Format("Path: {0} QueryString: {1}", path, encryptedQuery));
                        context.Response.Redirect(string.Format("{0}?{1}={2}", path, CommonUtility.SecureQueryStringKey, encryptedQuery), false);
                    }
                }
                catch (Exception ex)
                {
                    ex.ExceptionValueTracker(stateSaltManager, context, path, queryString);
                }
            };

        /// <summary>
        /// The decrypt process
        /// </summary>
        Action<IStateSaltManager, HttpContext, string, string> DecryptProcess
            = (stateSaltManager, context, path, queryString) =>
            {
                try
                {
                    if (queryString.StartsWith(CommonUtility.SecureQueryStringKey, StringComparison.OrdinalIgnoreCase))
                    {
                        // Decrypts the query string and rewrites the path.
                        string rawQuery = queryString.Replace(CommonUtility.SecureQueryStringKey + "=", string.Empty);
                        string decryptedQuery = stateSaltManager.GetSaltFromState(context, rawQuery);

                        //Utility.LogToFile(string.Format("Path: {0} QueryString: {1}", path, decryptedQuery));
                        if (string.IsNullOrEmpty(decryptedQuery))
                        {
                            //context.Response.Redirect(string.Format("{0}/Error/Index/{1}",
                            //    ConfigurationManager.AppSettings["VirtualDirectory"], "COMMON_ERROR_ACCESS_DENIED"));
                        }
                        else
                        {
                            //context.RewritePath(path, string.Empty, decryptedQuery);
                            context.RewritePath(string.Format("{0}?{1}", path, decryptedQuery));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ExceptionValueTracker(stateSaltManager, context, path, queryString);
                }
            };
        #endregion

        /// <summary>
        /// Parses a URL and returns the query string.
        /// </summary>
        /// <param name="url">The URL to parse.</param>
        /// <returns>
        /// The query string without the question mark.
        /// </returns>
        private static string ExtractQuery(string url)
        {
            int index = url.IndexOf("?") + 1;
            return url.Substring(index);
        }

        /// <summary>
        /// Parses a URL and returns the query string.
        /// </summary>
        /// <returns>
        /// The query string without the question mark.
        /// </returns>
        private static string GetVirtualPath()
        {
            string path = HttpContext.Current.Request.RawUrl;
            path = path.Substring(0, path.IndexOf("?"));
            //path = path.Substring(path.LastIndexOf("/") + 1);
            return path;
        }
    }
}