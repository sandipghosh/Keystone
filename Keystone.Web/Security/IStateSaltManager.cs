
namespace Keystone.Web.Security
{
    using System.Web;
    public interface IStateSaltManager
    {
        /// <summary>
        /// Sets the state of the salt to.
        /// </summary>
        /// <param name="currentContext">The current context.</param>
        /// <param name="value">The value.</param>
        /// <param name="expirationLimit">The expiration limit.</param>
        /// <returns></returns>
        string SetSaltToState(HttpContext currentContext, string value);

        /// <summary>
        /// Gets the state of the salt from.
        /// </summary>
        /// <param name="currentContext">The current context.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetSaltFromState(HttpContext currentContext, string key);
    }
}
