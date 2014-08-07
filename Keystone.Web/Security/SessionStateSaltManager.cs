
namespace Keystone.Web.Security
{
    using System;
    using System.Web;
    using Keystone.Web.Utilities;
    public class SessionStateSaltManager : IStateSaltManager
    {
        /// <summary>
        /// Sets the state of the salt to.
        /// </summary>
        /// <param name="currentContext">The current context.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string SetSaltToState(HttpContext currentContext, string value)
        {
            string encryptedQueryString = string.Empty;
            try
            {
                encryptedQueryString = HttpUtility.UrlEncode(Crypto.Encrypt(value));
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(currentContext, value);
            }
            return encryptedQueryString;
        }

        /// <summary>
        /// Gets the state of the salt from.
        /// </summary>
        /// <param name="currentContext">The current context.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetSaltFromState(HttpContext currentContext, string key)
        {
            string decryptedQueryString = string.Empty;
            try
            {
                string encryptedQueryString = HttpUtility.UrlDecode(currentContext.Request.QueryString[CommonUtility.SecureQueryStringKey]);
                decryptedQueryString = Crypto.Decrypt(encryptedQueryString);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(currentContext, key);
            }
            return decryptedQueryString;
        }
    }
}