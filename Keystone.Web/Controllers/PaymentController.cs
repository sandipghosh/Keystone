

namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using Keystone.Web.Utilities.PaymentGetway;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Web.Mvc;

    [SignInActionValidator()]
    public class PaymentController : BaseController
    {
        private readonly IOrderDataRepository _orderDataRepository;
        private readonly IUserAccountDataRepository _userAccountDataRepository;
        private readonly IShoppingCartDataRepository _shoppingCartDataRepository;
        private readonly IPaymentInfoDataRepository _paymentInfoDataRepositor;
        private readonly PaypalApiCaller _payPalCaller = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController" /> class.
        /// </summary>
        /// <param name="userAccountDataRepository">The user account data repository.</param>
        /// <param name="orderDataRepository">The order data repository.</param>
        /// <param name="shoppingCartDataRepository">The shopping cart data repository.</param>
        public PaymentController(IUserAccountDataRepository userAccountDataRepository,
            IOrderDataRepository orderDataRepository,
            IShoppingCartDataRepository shoppingCartDataRepository,
            IPaymentInfoDataRepository paymentInfoDataRepositor)
        {
            this._userAccountDataRepository = userAccountDataRepository;
            this._orderDataRepository = orderDataRepository;
            this._shoppingCartDataRepository = shoppingCartDataRepository;
            this._paymentInfoDataRepositor = paymentInfoDataRepositor;
            this._payPalCaller = new PaypalApiCaller();
        }

        /// <summary>
        /// Indexes the specified order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index(int orderId)
        {
            OrderModel currentOrder = null;
            try
            {
                currentOrder = this._orderDataRepository.Get(orderId);

                List<OrderItemModel> currentOrderItems = CommonUtility.GetSessionData<List<OrderItemModel>>(SessionVariable.OrderItems);
                if (currentOrderItems.IsEmptyCollection() && currentOrder != null)
                {
                    CommonUtility.SetSessionData<List<OrderItemModel>>(SessionVariable.OrderItems, currentOrder.OrderItems.ToList());
                    CommonUtility.SetSessionData<int>(SessionVariable.CurrentOrderId, currentOrder.OrderId);
                }
                ViewBag.OrderId = orderId;
                ViewBag.UserName = this._userAccountDataRepository.Get
                    (CommonUtility.GetSessionData<int>(SessionVariable.UserId)).ToString();

                ViewBag.Error = TempData["ErrorMessage"].AsString();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(orderId);
            }
            return View(currentOrder);
        }

        /// <summary>
        /// Gets the change address popup.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetChangeAddressPopup(string type)
        {
            try
            {
                int? userId = CommonUtility.GetSessionData<int?>(SessionVariable.UserId);
                if (userId.HasValue)
                {
                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    TextInfo textInfo = cultureInfo.TextInfo;

                    UserAddressModel userAddress = null;
                    var user = this._userAccountDataRepository.Get(userId.Value);

                    if (user != null)
                    {
                        userAddress = user.GetAddress((AddressTypeEnum)Enum.Parse(typeof(AddressTypeEnum), type));
                        //userAddress = user.UserAddresses.FirstOrDefault
                        //    (x => x.AddressTypeId.Equals(Enum.Parse(typeof(AddressTypeEnum), type)));

                        if (userAddress == null)
                            userAddress = new UserAddressModel { UaserAccountId = userId.Value };
                    }

                    ViewBag.DialogTitle = textInfo.ToTitleCase(string.Format("{0} address", type));
                    return PartialView("_Address", userAddress);
                }
                else
                { return PartialView("_ResponseAlert", string.Format("Please sign in to change the {0} address. Access denied", type)); }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(type);
            }
            return null;
        }

        /// <summary>
        /// Shows the content of the mail.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ShowMailContent(int orderId)
        {
            OrderModel currentOrder = null;
            try
            {
                currentOrder = this._orderDataRepository.Get(orderId);
                TempData["UserName"] = this._userAccountDataRepository.Get
                    (CommonUtility.GetSessionData<int>(SessionVariable.UserId)).ToString();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(orderId);
            }
            return View("_OrderConfirmationMailContentForUser", currentOrder);
        }

        /// <summary>
        /// Paypals the success response.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PaypalSuccessResponse()
        {
            string message = "Unable to process the payment. Inconvenience regretted.";
            CommonFuntionality commonFunc = new CommonFuntionality();
            try
            {
                ShoppingCartModel shoppingCart = CommonUtility.GetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData);
                if (shoppingCart == null)
                {
                    return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
                }

                string token = HttpContext.Request["token"].ToString();
                string payerId = HttpContext.Request["PayerID"].ToString();

                PaypalCheckoutResponse responseCheckoutData = this._payPalCaller.GetCheckoutDetails(token);
                if (responseCheckoutData != null && responseCheckoutData.Status == true)
                {
                    string finalAmount = responseCheckoutData.Decoder["AMT"].ToString();

                    //DateTime orderDate = Convert.ToDateTime(responseCheckoutData.Decoder["TIMESTAMP"].ToString());
                    //string username = User.Identity.Name;
                    //string firstName = responseCheckoutData.Decoder["FIRSTNAME"].ToString();
                    //string lastName = responseCheckoutData.Decoder["LASTNAME"].ToString();
                    //string address = responseCheckoutData.Decoder["SHIPTOSTREET"].ToString();
                    //string city = responseCheckoutData.Decoder["SHIPTOCITY"].ToString();
                    //string state = responseCheckoutData.Decoder["SHIPTOSTATE"].ToString();
                    //string postalCode = responseCheckoutData.Decoder["SHIPTOZIP"].ToString();
                    //string country = responseCheckoutData.Decoder["SHIPTOCOUNTRYCODE"].ToString();
                    //myOrder.Email = responseCheckoutData.Decoder["EMAIL"].ToString();

                    PaypalCheckoutResponse responseCheckoutPayment = this._payPalCaller.DoCheckoutPayment(finalAmount, token, payerId);
                    if (responseCheckoutPayment != null && responseCheckoutPayment.Status == true)
                    {
                        PaymentInfoModel paymentInfo = commonFunc.PreparePaimentInfo(responseCheckoutPayment, shoppingCart);
                        shoppingCart.PaymentInfo = paymentInfo;
                        shoppingCart.TransactionId = paymentInfo.TransactionId;

                        CommonUtility.SetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData, shoppingCart);
                        return RedirectToAction("SaveShoppingCart", "ShoppingCart");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = commonFunc.PaymentErrorRedirection(responseCheckoutPayment, shoppingCart);
                        return RedirectToAction("Index", "Payment", new { orderId = shoppingCart.OrderId });
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = commonFunc.PaymentErrorRedirection(responseCheckoutData, shoppingCart);
                    return RedirectToAction("Index", "Payment", new { orderId = shoppingCart.OrderId });
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Paypals the failure response.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PaypalFailureResponse()
        {
            try
            {
                var shoppingCart = CommonUtility.GetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData);
                TempData["ErrorMessage"] = "Transaction has been cancelled";
                return RedirectToAction("Index", "ShoppingCart", new { orderId = shoppingCart.OrderId });
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return null;
        }

        /// <summary>
        /// Paypals the manager success response.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PaypalManagerSuccessResponse()
        {
            string message = "Unable to process the payment. Inconvenience regretted.";
            CommonFuntionality commonFunc = new CommonFuntionality();
            try
            {
                ShoppingCartModel shoppingCart = CommonUtility.GetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData);
                if (shoppingCart == null)
                {
                    return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
                }

                PaypalCreditCardCheckoutResponse responseCheckoutData = new PaypalCreditCardCheckoutResponse();
                NVPCodec decoder = new NVPCodec();
                decoder.Decode(Request.QueryString);
                responseCheckoutData.Decoder = decoder;

                if (responseCheckoutData != null && responseCheckoutData.Status == true)
                {
                    PaymentInfoModel paymentInfo = commonFunc.PreparePaimentInfo(responseCheckoutData, shoppingCart);
                    shoppingCart.PaymentInfo = paymentInfo;
                    shoppingCart.TransactionId = paymentInfo.TransactionId;

                    CommonUtility.SetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData, shoppingCart);
                    return RedirectToAction("SaveShoppingCart", "ShoppingCart");
                }
                else
                {
                    TempData["ErrorMessage"] = commonFunc.PaymentErrorRedirection(responseCheckoutData, shoppingCart);
                    return RedirectToAction("Index", "Payment", new { orderId = shoppingCart.OrderId });
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Paypals the manager cancel response.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PaypalManagerCancelResponse()
        {
            var shoppingCart = CommonUtility.GetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData);
            TempData["ErrorMessage"] = "Transaction has been cancelled";
            return RedirectToAction("Index", "ShoppingCart", new { orderId = shoppingCart.OrderId });
        }

        /// <summary>
        /// Paypals the manager error response.
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PaypalManagerErrorResponse()
        {
            var shoppingCart = CommonUtility.GetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData);
            TempData["ErrorMessage"] = "Transaction has been cancelled";
            return RedirectToAction("Index", "ShoppingCart", new { orderId = shoppingCart.OrderId });
        }

        /// <summary>
        /// Pays the by card.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PayByCard(ShoppingCartModel shoppingCart)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(shoppingCart);
            }
            return null;
        }
    }
}
