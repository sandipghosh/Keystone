

namespace Keystone.Web.Utilities.PaymentGetway
{
    using Keystone.Web.Models;
    using Keystone.Web.Utilities;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Net.Http;

    public class PaypalApiCaller
    {
        private readonly string subject = "";
        private readonly string BNCode = "PP-ECWizard";
        private const int Timeout = 15000;

        /// <summary>
        /// Shortcuts the express checkout.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="token">The token.</param>
        /// <param name="retMsg">The ret MSG.</param>
        /// <returns></returns>
        public PaypalCheckoutResponse ShortcutExpressCheckout(ShoppingCartModel shoppingCart)
        {
            PaypalCheckoutResponse response = new PaypalCheckoutResponse();
            try
            {
                NVPCodec encoder = new NVPCodec();
                encoder[PaypalAttributes.METHOD] = "SetExpressCheckout";
                encoder[PaypalAttributes.RETURNURL] = PaypalSettings.PaypalReturnUrl;
                encoder[PaypalAttributes.CANCELURL] = PaypalSettings.PaypalCancelUrl;
                encoder[PaypalAttributes.BRANDNAME] = "Keystone Industries";
                encoder[PaypalAttributes.PAYMENTREQUEST_n_PAYMENTACTION] = "Sale";
                encoder[PaypalAttributes.PAYMENTREQUEST_n_CURRENCYCODE] = PaypalSettings.Currency;

                var myOrderList = shoppingCart.Order.OrderItems.ToList();
                foreach (OrderItemModel item in myOrderList)
                {
                    int index = myOrderList.IndexOf(item);
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_NAMEm, index)] = item.Draft.Template.ToString();
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_NUMBERm, index)] = item.Draft.Template.TemplateCode.ToString();
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_DESCm, index)] = item.Draft.Template.TemplateDesc.ToString();
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_AMTm, index)] = (item.Price / item.Quantity).ToString("0.00");
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_QTYm, index)] = item.Quantity.ToString();
                }

                OrderAppliedPromoModel promo = shoppingCart.Order.OrderAppliedPromoes.FirstOrDefault();
                if (promo != null)
                {
                    int promoItemIndex = myOrderList.Count();
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_NAMEm, promoItemIndex)] = "Promo code discount";
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_NUMBERm, promoItemIndex)] = promo.PromoCode.PromoCodeName;
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_AMTm, promoItemIndex)] = (promo.AppliedAmount * -1).ToString("0.00");
                    encoder[string.Format(PaypalAttributes.L_PAYMENTREQUEST_n_QTYm, promoItemIndex)] = "1";
                }

                encoder[PaypalAttributes.PAYMENTREQUEST_n_ITEMAMT] = shoppingCart.Order.TotalAmount.ToString("0.00");
                encoder[PaypalAttributes.PAYMENTREQUEST_n_AMT] = shoppingCart.Order.TotalAmount.ToString("0.00");
                encoder[PaypalAttributes.ADDROVERRIDE] = "1";
                encoder[PaypalAttributes.PAYMENTREQUEST_n_SHIPTONAME] = shoppingCart.Order.UserAccount.ToString().CharactorLimit(32); //32
                encoder[PaypalAttributes.PAYMENTREQUEST_n_SHIPTOSTREET] = shoppingCart.ShippingAddress.Address1.CharactorLimit(100); //100
                encoder[PaypalAttributes.PAYMENTREQUEST_n_SHIPTOCITY] = shoppingCart.ShippingAddress.City.CharactorLimit(40); //40
                encoder[PaypalAttributes.PAYMENTREQUEST_n_SHIPTOSTATE] = shoppingCart.ShippingAddress.State.CharactorLimit(40); //40
                encoder[PaypalAttributes.PAYMENTREQUEST_n_SHIPTOZIP] = shoppingCart.ShippingAddress.Pin.CharactorLimit(20); //20
                encoder[PaypalAttributes.PAYMENTREQUEST_n_SHIPTOCOUNTRYCODE] = ("US").CharactorLimit(2); //2
                encoder[PaypalAttributes.PAYMENTREQUEST_n_SHIPTOPHONENUM] = shoppingCart.ShippingAddress.SeconderyContact.CharactorLimit(20); //20

                string pStrrequestforNvp = encoder.Encode();
                string pStresponsenvp = HttpCall(pStrrequestforNvp, BuildPaypalCredentialsNvpString, PaypalSettings.EndPointUrl);

                NVPCodec decoder = new NVPCodec();
                decoder.Decode(pStresponsenvp);
                response.Decoder = decoder;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(shoppingCart);
            }
            return response;
        }

        /// <summary>
        /// Gets the checkout details.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="payerID">The payer ID.</param>
        /// <param name="decoder">The decoder.</param>
        /// <param name="retMsg">The ret MSG.</param>
        /// <returns></returns>
        public PaypalCheckoutResponse GetCheckoutDetails(string token)
        {
            PaypalCheckoutResponse response = new PaypalCheckoutResponse();
            try
            {
                NVPCodec encoder = new NVPCodec();
                encoder[PaypalAttributes.METHOD] = "GetExpressCheckoutDetails";
                encoder[PaypalAttributes.TOKEN] = token;

                string pStrrequestforNvp = encoder.Encode();
                string pStresponsenvp = HttpCall(pStrrequestforNvp, BuildPaypalCredentialsNvpString, PaypalSettings.EndPointUrl);

                NVPCodec decoder = new NVPCodec();
                decoder.Decode(pStresponsenvp);
                response.Decoder = decoder;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(token);
            }
            return response;
        }

        /// <summary>
        /// Does the checkout payment.
        /// </summary>
        /// <param name="finalPaymentAmount">The final payment amount.</param>
        /// <param name="token">The token.</param>
        /// <param name="payerID">The payer ID.</param>
        /// <param name="decoder">The decoder.</param>
        /// <param name="retMsg">The ret MSG.</param>
        /// <returns></returns>
        public PaypalCheckoutResponse DoCheckoutPayment(string finalPaymentAmount, string token, string payerID)
        {
            PaypalCheckoutResponse response = new PaypalCheckoutResponse();
            try
            {
                NVPCodec encoder = new NVPCodec();
                encoder[PaypalAttributes.METHOD] = "DoExpressCheckoutPayment";
                encoder[PaypalAttributes.TOKEN] = token;
                encoder[PaypalAttributes.PAYERID] = payerID;
                encoder[PaypalAttributes.PAYMENTREQUEST_n_AMT] = finalPaymentAmount;
                encoder[PaypalAttributes.PAYMENTREQUEST_n_CURRENCYCODE] = "USD";
                encoder[PaypalAttributes.PAYMENTREQUEST_n_PAYMENTACTION] = "Sale";

                string pStrrequestforNvp = encoder.Encode();
                string pStresponsenvp = HttpCall(pStrrequestforNvp, BuildPaypalCredentialsNvpString, PaypalSettings.EndPointUrl);

                NVPCodec decoder = new NVPCodec();
                decoder.Decode(pStresponsenvp);
                response.Decoder = decoder;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(finalPaymentAmount, token, payerID);
            }

            return response;
        }

        /// <summary>
        /// Credits the card payment.
        /// </summary>
        /// <param name="shoppingCart">The shopping cart.</param>
        /// <returns></returns>
        public PaypalCreditCardCheckoutResponse CreditCardPayment(ShoppingCartModel shoppingCart)
        {
            PaypalCreditCardCheckoutResponse response = new PaypalCreditCardCheckoutResponse();
            try
            {
                NVPCodec encoder = new NVPCodec();
                string securityTokenId = CommonUtility.GenarateRandomString(20,20);

                encoder[PaypalAttributes.TRXTYPE] = "A";
                encoder[PaypalAttributes.AMT] = shoppingCart.Order.TotalAmount.ToString("0.00");
                encoder[PaypalAttributes.CURRENCY] = PaypalSettings.Currency;
                encoder[PaypalAttributes.CREATESECURETOKEN] = "Y";
                encoder[PaypalAttributes.SECURETOKENID] = securityTokenId;
                encoder[PaypalAttributes.RETURNURL] = PaypalSettings.PaypalManagerReturnUrl;
                encoder[PaypalAttributes.CANCELURL] = PaypalSettings.PaypalManagerCancelUrl;
                encoder[PaypalAttributes.ERRORURL] = PaypalSettings.PaypalManagerErrorUrl;

                encoder[PaypalAttributes.BILLTOFIRSTNAME] = shoppingCart.Order.UserAccount.FirstName;
                encoder[PaypalAttributes.BILLTOLASTNAME] = shoppingCart.Order.UserAccount.LastName;
                encoder[PaypalAttributes.BILLTOSTREET] = shoppingCart.BillingAddress.Address1.CharactorLimit(100); //100
                encoder[PaypalAttributes.BILLTOCITY] = shoppingCart.BillingAddress.City.CharactorLimit(40); //40
                encoder[PaypalAttributes.BILLTOSTATE] = shoppingCart.BillingAddress.State.CharactorLimit(40); //40
                encoder[PaypalAttributes.BILLTOZIP] = shoppingCart.BillingAddress.Pin.CharactorLimit(20); //20
                encoder[PaypalAttributes.BILLTOCOUNTRY] = ("US").CharactorLimit(2); //2

                encoder[PaypalAttributes.SHIPTOFIRSTNAME] = shoppingCart.Order.UserAccount.FirstName;
                encoder[PaypalAttributes.SHIPTOLASTNAME] = shoppingCart.Order.UserAccount.LastName;
                encoder[PaypalAttributes.SHIPTOSTREET] = shoppingCart.ShippingAddress.Address1.CharactorLimit(100); //100
                encoder[PaypalAttributes.SHIPTOCITY] = shoppingCart.ShippingAddress.City.CharactorLimit(40); //40
                encoder[PaypalAttributes.SHIPTOSTATE] = shoppingCart.ShippingAddress.State.CharactorLimit(40); //40
                encoder[PaypalAttributes.SHIPTOZIP] = shoppingCart.ShippingAddress.Pin.CharactorLimit(20); //20
                encoder[PaypalAttributes.SHIPTOCOUNTRY] = ("US").CharactorLimit(2); //2

                string pStrrequestforNvp = encoder.Encode();
                string pStresponsenvp = HttpCall(pStrrequestforNvp, BuildPaypalManagerCredentialsNvpString, PaypalSettings.ManagerEndPointUrl);

                NVPCodec decoder = new NVPCodec();
                decoder.Decode(pStresponsenvp);
                response.Decoder = decoder;
                response.SecurityTokanId = securityTokenId;
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(shoppingCart);
            }
            return response;
        }

        /// <summary>
        /// HTTPs the call.
        /// </summary>
        /// <param name="nvpRequest">The NVP request.</param>
        /// <returns></returns>
        private string HttpCall(string nvpRequest, Func<string> buildCredential, string endpointUrl)
        {
            string strPost = string.Format("{0}&{1}&BUTTONSOURCE={2}", nvpRequest,
                buildCredential(), HttpUtility.UrlEncode(BNCode));

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(endpointUrl);
            objRequest.Timeout = Timeout;
            objRequest.Method = WebRequestMethods.Http.Post;
            objRequest.ContentLength = strPost.Length;

            try
            {
                using (StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream()))
                {
                    myWriter.Write(strPost);
                }
            }
            catch (Exception) { }

            //Retrieve the Response returned from the NVP API call to PayPal.
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            string result;

            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Builds the paypal credentials NVP string.
        /// </summary>
        /// <returns></returns>
        private string BuildPaypalCredentialsNvpString()
        {
            NVPCodec codec = new NVPCodec();

            if (!IsEmpty(PaypalSettings.PaypalApiUsername))
                codec[PaypalAttributes.USER] = PaypalSettings.PaypalApiUsername;

            if (!IsEmpty(PaypalSettings.PaypalApiPassword))
                codec[PaypalAttributes.PWD] = PaypalSettings.PaypalApiPassword;

            if (!IsEmpty(PaypalSettings.PaypalApiSignature))
                codec[PaypalAttributes.SIGNATURE] = PaypalSettings.PaypalApiSignature;

            if (!IsEmpty(subject))
                codec[PaypalAttributes.SUBJECT] = subject;

            codec[PaypalAttributes.VERSION] = "93.0";

            return codec.Encode();
        }

        /// <summary>
        /// Builds the paypal manager credentials NVP string.
        /// </summary>
        /// <returns></returns>
        private string BuildPaypalManagerCredentialsNvpString()
        {
            NVPCodec codec = new NVPCodec();

            if (!IsEmpty(PaypalSettings.PaypalManagerPartner))
                codec[PaypalAttributes.PARTNER] = PaypalSettings.PaypalManagerPartner;

            if (!IsEmpty(PaypalSettings.PaypalManagerMerchantLogin))
                codec[PaypalAttributes.VENDOR] = PaypalSettings.PaypalManagerMerchantLogin;

            if (!IsEmpty(PaypalSettings.PaypalManagerUser))
                codec[PaypalAttributes.USER] = PaypalSettings.PaypalManagerUser;

            if (!IsEmpty(PaypalSettings.PaypalManagerPassword))
                codec[PaypalAttributes.PWD] = PaypalSettings.PaypalManagerPassword;

            return codec.Encode();
        }

        /// <summary>
        /// Determines whether the specified s is empty.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static bool IsEmpty(string s)
        {
            return s == null || s.Trim() == string.Empty;
        }
    }
}