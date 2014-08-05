
namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    [SignInActionValidator()]
    public class ReceiptController : BaseController
    {
        private readonly IShoppingCartDataRepository _shoppingCartDataRepository;
        private readonly IUserAddressDataRepository _userAddressDataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiptController"/> class.
        /// </summary>
        /// <param name="shoppingCartDataRepository">The shopping cart data repository.</param>
        public ReceiptController(IShoppingCartDataRepository shoppingCartDataRepository,
            IUserAddressDataRepository userAddressDataRepository)
        {
            this._shoppingCartDataRepository = shoppingCartDataRepository;
            this._userAddressDataRepository = userAddressDataRepository;
        }

        /// <summary>
        /// Indexes the specified shopping cart identifier.
        /// </summary>
        /// <param name="shoppingCartId">The shopping cart identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index(int shoppingCartId)
        {
            string message = "Unable to access the receipt. Please try again.";
            try
            {
                ShoppingCartModel shoppingCart = this._shoppingCartDataRepository.Get(shoppingCartId);
                if (shoppingCart != null)
                {
                    int shippingAddressId = shoppingCart.ShippingAddressId;
                    shoppingCart.ShippingAddress = this._userAddressDataRepository.Get(shippingAddressId);

                    int billingAddressId = shoppingCart.BillingAddressId;
                    shoppingCart.BillingAddress = this._userAddressDataRepository.Get(billingAddressId);

                    TempData["AttachmentType"] = AttachmentTypeEnum.PDF;
                    TempData["UserName"] = CommonUtility.GetSessionData<string>(SessionVariable.UserName);

                    Session.Remove(SessionVariable.SelectedTemplate);
                    Session.Remove(SessionVariable.CurrentDraft);
                    Session.Remove(SessionVariable.OrderItems);
                    Session.Remove(SessionVariable.CurrentOrderId);
                    Session.Remove(SessionVariable.CurrentShoppingCartData);

                    return View("_OrderConfirmationMailContentForUser", shoppingCart);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(ex);
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }
    }
}
