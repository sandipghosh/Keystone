
namespace Keystone.Web.Utilities
{
    public static class SessionVariable
    {
        private static string _UserId = "UserId";
        private static string _UserName = "UserName";
        private static string _SelectedTemplate = "SelectedTemplate";
        private static string _CurrentDraft = "CurrentDraft";
        private static string _OrderItems = "OrderItems";
        private static string _CurrentOrderId = "CurrentOrderId";
        private static string _IsAdminUser = "IsAdminUser";
        private static string _CurrentShoppingCartData = "CurrentShoppingCartData";

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public static string UserId { get { return _UserId; } }
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public static string UserName { get { return _UserName; } }
        /// <summary>
        /// Gets the selected template.
        /// </summary>
        /// <value>
        /// The selected template.
        /// </value>
        public static string SelectedTemplate { get { return _SelectedTemplate; } }
        /// <summary>
        /// Gets the current draft.
        /// </summary>
        /// <value>
        /// The current draft.
        /// </value>
        public static string CurrentDraft { get { return _CurrentDraft; } }
        /// <summary>
        /// Gets the order items.
        /// </summary>
        /// <value>
        /// The order items.
        /// </value>
        public static string OrderItems { get { return _OrderItems; } }
        /// <summary>
        /// Gets the current order identifier.
        /// </summary>
        /// <value>
        /// The current order identifier.
        /// </value>
        public static string CurrentOrderId { get { return _CurrentOrderId; } }
        /// <summary>
        /// Gets the is admin user.
        /// </summary>
        /// <value>
        /// The is admin user.
        /// </value>
        public static string IsAdminUser { get { return _IsAdminUser; } }
        /// <summary>
        /// Gets the current chopping cart data.
        /// </summary>
        /// <value>
        /// The current chopping cart data.
        /// </value>
        public static string CurrentShoppingCartData { get { return _CurrentShoppingCartData; } }
    }
}