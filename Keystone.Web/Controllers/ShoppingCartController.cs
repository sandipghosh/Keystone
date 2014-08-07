

namespace Keystone.Web.Controllers
{
    using Keystone.Web.Controllers.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using Keystone.Web.Utilities.PaymentGetway;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Transactions;
    using System.Web.Mvc;

    [SignInActionValidator()]
    public class ShoppingCartController : BaseController
    {
        private readonly ITemplateDataRepository _templateDataRepository;
        private readonly IUserAccountDataRepository _userAccountDataRepository;
        private readonly IUserAddressDataRepository _userAddressDataRepository;
        private readonly IDeliveryScheduleDataRepository _deliveryScheduleDataRepository;
        private readonly IPromoCodeDataRepository _promoCodeDataRepository;
        private readonly IOrderDataRepository _orderDataRepository;
        private readonly IOrderItemDataRepository _orderItemDataRepository;
        private readonly IOrderAppliedPromoDataRepository _orderAppliedPromoDataRepository;
        private readonly IShoppingCartDataRepository _shoppingCartDataRepository;
        private readonly IPaymentTypeDataRepository _paymentTypeDataRepository;
        private readonly IDraftDataRepository _draftDataRepository;
        private readonly PaypalApiCaller _payPalCaller = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartController" /> class.
        /// </summary>
        /// <param name="templateDataRepository">The template data repository.</param>
        /// <param name="deliveryScheduleDataRepository">The delivery schedule data repository.</param>
        /// <param name="userAccountDataRepository">The user account data repository.</param>
        /// <param name="userAddressDataRepository">The user address data repository.</param>
        /// <param name="promoCodeDataRepository">The promo code data repository.</param>
        /// <param name="orderDataRepository">The order data repository.</param>
        /// <param name="orderItemDataRepository">The order item data repository.</param>
        /// <param name="shoppingAppliedPromoDataRepository">The shopping applied promo data repository.</param>
        /// <param name="shoppingCartDataRepository">The shopping cart data repository.</param>
        /// <param name="paymentTypeDataRepository">The payment type data repository.</param>
        /// <param name="draftDataRepository">The draft data repository.</param>
        public ShoppingCartController(ITemplateDataRepository templateDataRepository,
            IDeliveryScheduleDataRepository deliveryScheduleDataRepository,
            IUserAccountDataRepository userAccountDataRepository,
            IUserAddressDataRepository userAddressDataRepository,
            IPromoCodeDataRepository promoCodeDataRepository,
            IOrderDataRepository orderDataRepository,
            IOrderItemDataRepository orderItemDataRepository,
            IOrderAppliedPromoDataRepository shoppingAppliedPromoDataRepository,
            IShoppingCartDataRepository shoppingCartDataRepository,
            IPaymentTypeDataRepository paymentTypeDataRepository,
            IDraftDataRepository draftDataRepository)
        {
            this._templateDataRepository = templateDataRepository;
            this._deliveryScheduleDataRepository = deliveryScheduleDataRepository;
            this._userAccountDataRepository = userAccountDataRepository;
            this._userAddressDataRepository = userAddressDataRepository;
            this._promoCodeDataRepository = promoCodeDataRepository;
            this._orderDataRepository = orderDataRepository;
            this._orderItemDataRepository = orderItemDataRepository;
            this._orderAppliedPromoDataRepository = shoppingAppliedPromoDataRepository;
            this._shoppingCartDataRepository = shoppingCartDataRepository;
            this._paymentTypeDataRepository = paymentTypeDataRepository;
            this._draftDataRepository = draftDataRepository;
            this._payPalCaller = new PaypalApiCaller();
        }

        /// <summary>
        /// Indexes the specified order identifier.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult Index(int? orderId)
        {
            try
            {
                IList<OrderItemModel> currentSavedOrderItems = null;
                int? currentOrderId = null;

                if (orderId.HasValue)
                {
                    currentSavedOrderItems = this._orderItemDataRepository.GetList(x => x.OrderId.Equals(orderId.Value)
                        && x.StatusId.Equals((int)StatusEnum.Active)).ToList();

                    currentOrderId = orderId;

                    CommonUtility.SetSessionData<IList<OrderItemModel>>(SessionVariable.OrderItems, currentSavedOrderItems);
                    CommonUtility.SetSessionData<int?>(SessionVariable.CurrentOrderId, currentOrderId);
                }
                else
                {
                    currentSavedOrderItems = CommonUtility.GetSessionData<IList<OrderItemModel>>(SessionVariable.OrderItems);
                    currentOrderId = CommonUtility.GetSessionData<int?>(SessionVariable.CurrentOrderId);
                }

                if (currentOrderId.HasValue)
                {
                    var orderAppliedPromo = this._orderAppliedPromoDataRepository
                        .GetList(x => x.OrderId.Equals(currentOrderId.Value)
                         && x.StatusId.Equals((int)StatusEnum.Active));

                    if (!orderAppliedPromo.IsEmptyCollection())
                    {
                        ViewBag.PromoCodeId = orderAppliedPromo.FirstOrDefault().PromoCodeId;
                        ViewBag.PromoCodeValue = orderAppliedPromo.FirstOrDefault().PromoCode.PromoCodeName.AsString();
                        ViewBag.DiscountAmount = orderAppliedPromo.Sum(x => x.AppliedAmount);
                    }
                }

                var customCollection = currentSavedOrderItems.Select(x => new
                TempOrderItemModel
                {
                    OrderItemIdentifier = x.OrderItemIdentifier
                        ?? CommonUtility.GenarateRandomString(5, 5).ToUpper(),
                    TemplateId = x.TemplateId,
                    DeliveryScheduleId = x.DeliveryScheduleId,
                    DraftId = x.DraftId,
                    DisplayUrl = string.Format("{0}?{1}", x.DisplayUrl, DateTime.Now.Ticks.ToString()),
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TemplateCode = x.Template.TemplateCode,
                    TemplateTitle = x.Template.TemplateTitle,
                    TemplateDesc = x.Template.TemplateDesc,
                    TemplateWidth = x.Template.TemplateWidth,
                    TemplateHeight = x.Template.TemplateHeight
                });

                ViewBag.OrderId = currentOrderId.HasValue ? currentOrderId.Value : 0;
                ViewBag.UserName = CommonUtility.GetSessionData<string>(SessionVariable.UserName);
                ViewBag.SavedOrder = JsonConvert.SerializeObject(customCollection, Formatting.None,
                    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToBase64Encode();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return View();
        }

        /// <summary>
        /// Determines whether [is promo code exists] [the specified promo code name].
        /// </summary>
        /// <param name="promoCodeName">Name of the promo code.</param>
        /// <param name="ordrtId">The ordrt identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult IsPromoCodeExists(string promoCodeName, int ordrtId)
        {
            bool status = false;
            PromoCodeModel promoCode = null;
            string message = string.Empty;
            try
            {
                promoCode = this._promoCodeDataRepository
                    .GetList(x => x.PromoCodeName.Equals(promoCodeName)
                    && !x.StatusId.Equals((int)StatusEnum.Inactive)).FirstOrDefault();

                if (promoCode == null)
                    message = string.Format("\"{0}\" Promocode does not exists. Please try another", promoCodeName);
                else
                {
                    int userAccountId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                    List<OrderAppliedPromoModel> appliedPromo = null;

                    if (ordrtId > 0)
                    {
                        appliedPromo = this._orderAppliedPromoDataRepository.GetList(x =>
                            x.PromoCodeId.Equals(promoCode.PromoCodeId) && x.UserAccountId.Equals(userAccountId)
                            && !x.OrderId.Equals(ordrtId)).ToList();
                    }
                    else
                    {
                        appliedPromo = this._orderAppliedPromoDataRepository.GetList(x =>
                            x.PromoCodeId.Equals(promoCode.PromoCodeId) && x.UserAccountId.Equals(userAccountId)).ToList();
                    }

                    decimal balancePromoAmount = promoCode.PromoAmount - appliedPromo.Sum(x => x.AppliedAmount);
                    if (appliedPromo.IsEmptyCollection())
                    {
                        return Json(new { Status = true, PromoCode = new { PromoCodeId = promoCode.PromoCodeId, PromoAmount = balancePromoAmount } },
                            JsonRequestBehavior.AllowGet);
                    }
                    else
                        message = string.Format("\"{0}\" Promo balance is not available. Please try another", promoCodeName);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return Json(new { Status = status, PromoCode = promoCode, Message = message },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Inserts the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult InsertOrder(OrderModel order)
        {
            bool status = false;
            int orderId = 0;
            try
            {
                if (!IsOrderAlreadyExists(order))
                {
                    int userAccountId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                    order.CreatedBy = userAccountId;
                    order.CreatedOn = DateTime.Now;
                    order.StatusId = (int)StatusEnum.Active;
                    order.OrderDate = DateTime.Now;
                    order.UserAccountId = userAccountId;

                    foreach (var item in order.OrderItems)
                    {
                        item.CreatedBy = userAccountId;
                        item.CreatedOn = DateTime.Now;
                        item.StatusId = (int)StatusEnum.Active;
                    }

                    TransactionOptions options = new TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                        Timeout = new TimeSpan(0, 1, 0)
                    };

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                    {
                        this._orderDataRepository.Insert(order);
                        if (order.PromoCodeId > 0)
                        {
                            OrderAppliedPromoModel orderPromo = new OrderAppliedPromoModel
                            {
                                OrderId = order.OrderId,
                                PromoCodeId = order.PromoCodeId,
                                AppliedAmount = order.DiscountAmount,
                                UserAccountId = userAccountId,
                                CreatedBy = userAccountId,
                                CreatedOn = DateTime.Now,
                                StatusId = (int)StatusEnum.Active
                            };

                            this._orderAppliedPromoDataRepository.Insert(orderPromo);
                        }

                        scope.Complete();

                        CommonUtility.SetSessionData<int?>(SessionVariable.CurrentOrderId, order.OrderId);
                        orderId = order.OrderId;
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(order);
            }
            return Json(new { Status = status, OrderId = orderId }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updates the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult UpdateOrder(OrderModel order)
        {
            bool status = false;
            try
            {
                OrderModel existingOrder = this._orderDataRepository.Get(order.OrderId);
                if (existingOrder != null)
                {
                    int userAccountId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                    existingOrder.OrderReferance = order.OrderReferance;
                    existingOrder.OrderDate = DateTime.Now;
                    existingOrder.SubTotal = order.SubTotal;
                    existingOrder.DiscountAmount = order.DiscountAmount;
                    existingOrder.TotalAmount = order.TotalAmount;
                    existingOrder.UpdatedBy = (int?)userAccountId;
                    existingOrder.UpdatedOn = (DateTime?)DateTime.Now;
                    //existingOrder.OrderItems = null;

                    foreach (var item in order.OrderItems)
                    {
                        item.OrderId = order.OrderId;
                        item.CreatedBy = userAccountId;
                        item.CreatedOn = DateTime.Now;
                        item.StatusId = (int)StatusEnum.Active;
                    }

                    var existingOrderPromo = this._orderAppliedPromoDataRepository
                        .GetList(x => x.OrderId.Equals(order.OrderId) && x.StatusId.Equals((int)StatusEnum.Active))
                        .FirstOrDefault();

                    if (existingOrderPromo != null && order.PromoCodeId > 0)
                    {
                        existingOrderPromo.OrderId = order.OrderId;
                        existingOrderPromo.PromoCodeId = order.PromoCodeId;
                        existingOrderPromo.AppliedAmount = order.DiscountAmount;
                        existingOrderPromo.UserAccountId = userAccountId;
                    }

                    TransactionOptions options = new TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                        Timeout = new TimeSpan(0, 1, 0)
                    };

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                    {
                        this._orderDataRepository.Update(existingOrder);
                        this._orderItemDataRepository.Delete(existingOrder.OrderItems);
                        this._orderItemDataRepository.Insert(order.OrderItems);

                        if (order.PromoCodeId > 0)
                        {
                            if (existingOrderPromo != null)
                                this._orderAppliedPromoDataRepository.Update(existingOrderPromo);
                            else
                            {
                                OrderAppliedPromoModel orderPromo = new OrderAppliedPromoModel
                                {
                                    OrderId = order.OrderId,
                                    PromoCodeId = order.PromoCodeId,
                                    AppliedAmount = order.DiscountAmount,
                                    UserAccountId = userAccountId,
                                    CreatedBy = userAccountId,
                                    CreatedOn = DateTime.Now,
                                    StatusId = (int)StatusEnum.Active
                                };

                                this._orderAppliedPromoDataRepository.Insert(orderPromo);
                            }
                        }

                        scope.Complete();
                        status = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(order);
            }
            return Json(new { Status = status, OrderId = order.OrderId }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Saves the shopping cart.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult SaveShoppingCart()
        {
            bool status = false;
            string message = "Unable to process the payment. Inconvenience regretted.";
            try
            {
                ShoppingCartModel shoppingCart = CommonUtility.GetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData);
                if (shoppingCart != null && shoppingCart.ShoppingCartId == 0)
                {
                    shoppingCart.Order = null;
                    shoppingCart.ShippingAddress = null;
                    shoppingCart.UserAccount = null;

                    TransactionOptions options = new TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                        Timeout = new TimeSpan(0, 1, 0)
                    };

                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew, options))
                    {
                        //InsertDamyPaimentInfo(shoppingCart);
                        this._shoppingCartDataRepository.Insert(shoppingCart);
                        this.SendOrderConfirmationMail(shoppingCart.ShoppingCartId, AttachmentTypeEnum.PDF);
                        this.SendOrderConfirmationMail(shoppingCart.ShoppingCartId, AttachmentTypeEnum.Archive);

                        scope.Complete();
                        status = true;
                    }

                    return RedirectToAction("Index", "Receipt", new { shoppingCartId = shoppingCart.ShoppingCartId });
                    //return Json(new { Status = status, ShoppingCartId = shoppingCart.ShoppingCartId, Message = message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Payments the sbumission.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult PaymentSbumission(ShoppingCartModel shoppingCart)
        {
            string message = "Unable to process the payment. Inconvenience regretted.";
            try
            {
                shoppingCart = PrepareShoppingData(shoppingCart);
                if (shoppingCart.Order != null)
                {
                    if (shoppingCart.Order.TotalAmount > 0)
                    {
                        if (shoppingCart.PaymentTypeId == (int)PaymentTypeEnum.Paypal)
                        {
                            PaypalCheckoutResponse responseCheckout = this._payPalCaller.ShortcutExpressCheckout(shoppingCart);
                            if (responseCheckout != null && responseCheckout.Status == true)
                            {
                                if (!string.IsNullOrEmpty(responseCheckout.RedirectionUrl))
                                    return Redirect(responseCheckout.RedirectionUrl);
                            }
                            else
                            {
                                CommonFuntionality commonFunc = new CommonFuntionality();
                                TempData["ErrorMessage"] = commonFunc.PaymentErrorRedirection(responseCheckout, shoppingCart);
                                return RedirectToAction("Index", "Payment", new { orderId = shoppingCart.OrderId });
                            }
                        }
                        else if (shoppingCart.PaymentTypeId == (int)PaymentTypeEnum.CreditCard)
                        {
                            int billingAddressId = shoppingCart.BillingAddressId;
                            shoppingCart.BillingAddress = this._userAddressDataRepository.Get(billingAddressId);

                            PaypalCreditCardCheckoutResponse responseCheckout = this._payPalCaller.CreditCardPayment(shoppingCart);
                            if (responseCheckout != null && responseCheckout.Status == true)
                            {
                                if (!string.IsNullOrEmpty(responseCheckout.RedirectionUrl))
                                    return Redirect(responseCheckout.RedirectionUrl);
                                //return Content(responseCheckout.RedirectionUrl);
                            }
                            else
                            {
                                CommonFuntionality commonFunc = new CommonFuntionality();
                                //TempData["ErrorMessage"] = commonFunc.PaymentErrorRedirection(responseCheckout, shoppingCart);
                                return RedirectToAction("Index", "Payment", new { orderId = shoppingCart.OrderId });
                            }
                        }
                    }
                    else
                    {
                        InsertDamyPaimentInfo(shoppingCart);
                        return RedirectToAction("SaveShoppingCart", "ShoppingCart");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(shoppingCart);
            }
            return RedirectToAction("Index", "Error", new { errorMsg = message.ToBase64Encode() });
        }

        /// <summary>
        /// Gets the saved order.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult GetSavedOrder(int orderId)
        {
            OrderModel order = null;
            try
            {
                order = this._orderDataRepository.Get(orderId);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(orderId);
            }
            return PartialView("_OrderList", order);
        }

        /// <summary>
        /// Shows the order item preview.
        /// </summary>
        /// <param name="draftId">The draft identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult ShowOrderItemPreview(int draftId)
        {
            DraftModel draft = null;
            try
            {
                draft = this._draftDataRepository.Get(draftId);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return PartialView("_PreviewOrderItems", draft);
        }

        /// <summary>
        /// Edits the order item.
        /// </summary>
        /// <param name="draftId">The draft identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ContentResult EditOrderItem(int draftId)
        {
            List<OrderItemModel> currentSavedOrderItems = CommonUtility
                .GetSessionData<List<OrderItemModel>>(SessionVariable.OrderItems);

            if (!currentSavedOrderItems.IsEmptyCollection())
            {
                currentSavedOrderItems.Remove(currentSavedOrderItems.FirstOrDefault(x => x.DraftId.Equals(draftId)));
                CommonUtility.SetSessionData<IList<OrderItemModel>>(SessionVariable.OrderItems, currentSavedOrderItems);
            }

            string redirectUrl = Url.Action("SavedImageIndex", "Editor", new { draftId = draftId });
            return Content(redirectUrl.ToBase64Encode());
        }

        /// <summary>
        /// Deletes the order item.
        /// </summary>
        /// <param name="draftId">The draft identifier.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult DeleteOrderItem(int draftId)
        {
            return RedirectToAction("EditOrderItem", "ShoppingCart", new { draftId = draftId });
        }

        /// <summary>
        /// Saves the temporary order.
        /// </summary>
        /// <param name="tempOrderItem">The temporary order item.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post),
        OutputCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public JsonResult SaveTempOrder(TempOrderItemModel tempOrderItem)
        {
            try
            {
                List<OrderItemModel> orderItems = CommonUtility
                    .GetSessionData<List<OrderItemModel>>(SessionVariable.OrderItems);

                if (!orderItems.IsEmptyCollection())
                {
                    string orderIdentifier = tempOrderItem.OrderItemIdentifier;
                    OrderItemModel changedOrderItem = orderItems
                        .FirstOrDefault(x => x.OrderItemIdentifier.Equals(orderIdentifier));

                    if (changedOrderItem != null)
                    {
                        changedOrderItem.Quantity = tempOrderItem.Quantity;
                        changedOrderItem.Price = tempOrderItem.Price;
                    }

                    return Json(new { Status = true }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(tempOrderItem);
            }
            return Json(new { Status = false }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Downloads the ordered PDF file.
        /// </summary>
        /// <param name="orderId">The order identifier.</param>
        /// <returns></returns>
        [AllowAnonymousAccess()]
        public FileResult DownloadOrderedPDFFile(int orderId)
        {
            try
            {
                OrderModel selectedOrder = this._orderDataRepository.Get(orderId);

                if (selectedOrder != null)
                {
                    var imagePreviewPaths = selectedOrder.OrderItems
                        .SelectMany(x => x.Draft.DraftPages)
                        .OrderBy(x => x.TemplateId)
                        .ThenBy(x => x.TemplatePageId)
                        .ThenBy(x => x.TemplatePage.OrderIndex)
                        .Select(x => x.FinalImageUrl.ToBase64Encode()).ToList();

                    if (imagePreviewPaths != null)
                    {
                        System.IO.MemoryStream stream = CommonUtility.CreatePdfStream(imagePreviewPaths);

                        return File(stream, "application/pdf", string.Format("{0}.pdf",
                            CommonUtility.GenarateRandomString(10, 10).ToUpper()));

                        stream.Close();
                        stream.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(orderId);
            }
            return null;
        }

        #region Private Members
        /// <summary>
        /// Determines whether [is order already exists] [the specified order].
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        private bool IsOrderAlreadyExists(OrderModel order)
        {
            bool status = false;
            try
            {
                string orderRef = order.OrderReferance;

                int existingOrder = _orderDataRepository.GetCount(x => x.OrderReferance.Equals(orderRef)
                    && x.StatusId.Equals((int)StatusEnum.Active));

                return (existingOrder > 0);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(order);
            }
            return status;
        }

        /// <summary>
        /// Sends the order confirmation mail to user.
        /// </summary>
        /// <param name="shoppingCartId">The shopping cart identifier.</param>
        private void SendOrderConfirmationMail(int shoppingCartId, AttachmentTypeEnum attachmentType)
        {
            try
            {
                ShoppingCartModel shoppingCart = this._shoppingCartDataRepository.Get(shoppingCartId);

                if (shoppingCart != null)
                {
                    shoppingCart.Order = this._orderDataRepository.Get(shoppingCart.OrderId);
                    shoppingCart.UserAccount = this._userAccountDataRepository.Get(shoppingCart.UserAccountId);
                    shoppingCart.BillingAddress = this._userAddressDataRepository.Get(shoppingCart.BillingAddressId);
                    shoppingCart.ShippingAddress = this._userAddressDataRepository.Get(shoppingCart.ShippingAddressId);

                    shoppingCart.PaymentType = this._paymentTypeDataRepository.Get(shoppingCart.PaymentTypeId);
                    if (shoppingCart.PromoCodeId.HasValue)
                        shoppingCart.PromoCode = this._promoCodeDataRepository.Get(shoppingCart.PromoCodeId.Value);

                    EmailSender email = new EmailSender
                    {
                        SSL = bool.Parse(ConfigurationManager.AppSettings["MAIL_SERVER_SSL"].ToString()),
                        Subject = "Order confirmation",
                        To = (attachmentType == AttachmentTypeEnum.PDF) ? shoppingCart.UserAccount.EmailId :
                            ConfigurationManager.AppSettings["MarchentEmailId"].ToString()
                    };

                    List<MemoryStream> draftPDF = new List<MemoryStream>();

                    PrepareAttachment(shoppingCart.Order.OrderItems.ToList(),
                        draftPDF, attachmentType, email.Attachments);

                    string mailBody = Utilities.CommonUtility.RenderViewToString
                        ("_OrderConfirmationMailContentForUser", shoppingCart,
                        this, new Dictionary<string, object>() { 
                            { "UserName", shoppingCart.UserAccount.ToString() },
                            { "AttachmentType", attachmentType }});

                    email.SendMailAsync(mailBody, () =>
                    {
                        foreach (var item in draftPDF)
                        {
                            try
                            {
                                item.Close();
                                item.Dispose();
                            }
                            catch (Exception) { }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
        }

        /// <summary>
        /// Prepares the attachment.
        /// </summary>
        /// <param name="orderItems">The order items.</param>
        /// <param name="outputMemoryStream">The output memory stream.</param>
        /// <param name="attachmentType">Type of the attachment.</param>
        /// <param name="attachments">The attachments.</param>
        private void PrepareAttachment(List<OrderItemModel> orderItems,
            List<MemoryStream> outputMemoryStream, AttachmentTypeEnum attachmentType,
            List<System.Net.Mail.Attachment> attachments)
        {
            try
            {
                switch (attachmentType)
                {
                    case AttachmentTypeEnum.Archive:
                        var imagePaths = orderItems.Select(x =>
                            new OrderedImageModel
                            {
                                OrderedItemCode = x.OrderItemId,
                                OrderedImages = x.Draft.DraftPages
                                    .OrderBy(z => z.TemplateId)
                                    .ThenBy(z => z.TemplatePageId)
                                    .ThenBy(z => z.TemplatePage.OrderIndex)
                                    .Select(y => y.FinalImageUrl.ToBase64Encode()).ToList()
                            }).ToList<OrderedImageModel>();

                        outputMemoryStream.Add(CommonUtility.CreateArchiveStream(imagePaths));

                        attachments.Add(new System.Net.Mail.Attachment(outputMemoryStream.LastOrDefault(),
                            string.Format("{0}.zip", CommonUtility.ConvertToTimestamp(DateTime.Now))));

                        break;

                    case AttachmentTypeEnum.PDF:
                        foreach (OrderItemModel item in orderItems)
                        {
                            outputMemoryStream.Add(CommonUtility.CreatePdfStream(item.Draft.DraftPages
                                .OrderBy(x => x.TemplateId)
                                .ThenBy(x => x.TemplatePageId)
                                .ThenBy(x => x.TemplatePage.OrderIndex)
                                .Select(x => x.DraftPreviewUrl.ToBase64Encode()).ToList()));

                            attachments.Add(new System.Net.Mail.Attachment(outputMemoryStream.LastOrDefault(),
                                string.Format("{0}.pdf", CommonUtility.ConvertToTimestamp(DateTime.Now))));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(orderItems, outputMemoryStream, attachmentType, attachments);
            }
        }

        /// <summary>
        /// Inserts the damy paiment information.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart.</param>
        private void InsertDamyPaimentInfo(ShoppingCartModel shoppingCart)
        {
            PaymentInfoModel paymentInfo = new PaymentInfoModel()
            {
                OrderId = shoppingCart.OrderId,
                RequistedIp = CommonUtility.GetClientIPAddress(),
                Acknowledgement = "Success",
                StatusId = (int)StatusEnum.Active,
                TransactionId = "",
                TransactionType = "",
                TransactionErrorCode = "",
                TransactionShortMessage = "Success",
                TransactionLongMessage = "Payment successfully made.",
                TransactionTime = DateTime.Now,
                TransactionAmount = 0.00m,
                CreatedBy = shoppingCart.UserAccountId,
                CreatedOn = DateTime.Now
            };

            shoppingCart.PaymentInfo = paymentInfo;
        }

        /// <summary>
        /// Prepares the shopping data.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        private ShoppingCartModel PrepareShoppingData(ShoppingCartModel shoppingCart)
        {
            try
            {
                int userAccountId = CommonUtility.GetSessionData<int>(SessionVariable.UserId);
                shoppingCart.StatusId = (int)StatusEnum.Active;
                shoppingCart.UserAccountId = shoppingCart.CreatedBy = userAccountId;
                shoppingCart.CreatedOn = DateTime.Now;
                shoppingCart.CartReferance = CommonUtility.GenarateRandomString(15, 15).ToUpper();
                shoppingCart.PromoCodeId = shoppingCart.PromoCodeId.Value == 0 ? null : shoppingCart.PromoCodeId;

                int orderId = shoppingCart.OrderId;
                shoppingCart.Order = this._orderDataRepository.Get(orderId);

                int shoppingAddressId = shoppingCart.ShippingAddressId;
                shoppingCart.ShippingAddress = this._userAddressDataRepository.Get(shoppingAddressId);

                int billingAddressId = shoppingCart.BillingAddressId;
                shoppingCart.BillingAddress = this._userAddressDataRepository.Get(billingAddressId);

                shoppingCart.UserAccount = this._userAccountDataRepository.Get(userAccountId);

                CommonUtility.SetSessionData<ShoppingCartModel>(SessionVariable.CurrentShoppingCartData, shoppingCart);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(shoppingCart);
            }
            return shoppingCart;
        }
        #endregion
    }
}
