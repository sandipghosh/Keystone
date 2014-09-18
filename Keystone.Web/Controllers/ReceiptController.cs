
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
                    TempData["OrderId"] = shoppingCart.OrderId.ToString();

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
                ex.ExceptionValueTracker(shoppingCartId);
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Paids the order receipt.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PaidOrderReceipt(int orderId)
        {
            string message = "Unable to access the receipt. Please try again.";
            try
            {
                ShoppingCartModel shoppingCart = _shoppingCartDataRepository
                    .GetList(x => x.OrderId == orderId).FirstOrDefaultCustom();

                return RedirectToAction("Index", "Receipt", new { shoppingCartId = shoppingCart.ShoppingCartId }); 
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(orderId);
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }
    }
}
