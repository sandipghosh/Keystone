

namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;

    public abstract class Message : SafeDisposableBaseClass
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Message()
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Whom the message is to
        /// </summary>
        public virtual string To { get; set; }

        /// <summary>
        /// The subject of the email
        /// </summary>
        public virtual string Subject { get; set; }

        /// <summary>
        /// Whom the message is from
        /// </summary>
        public virtual string From { get; set; }

        /// <summary>
        /// Body of the text
        /// </summary>
        public virtual string Body { get; set; }

        #endregion
    }
}