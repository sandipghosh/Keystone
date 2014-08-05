
namespace Keystone.Web.Utilities.PaymentGetway
{
    using System;
    using System.Web;

    public static class PaypalSettings
    {
        public static bool Sandbox { get { return CommonUtility.GetAppSetting<bool>("Paypal:Sandbox"); } }
        public static string EndPointUrl
        {
            get
            {
                return (Sandbox) ? CommonUtility.GetAppSetting<string>("Paypal:EndPointURL_SB") :
                    CommonUtility.GetAppSetting<string>("Paypal:EndPointURL");
            }
        }
        public static string Host
        {
            get
            {
                return (Sandbox) ? CommonUtility.GetAppSetting<string>("Paypal:Host_SB") :
                    CommonUtility.GetAppSetting<string>("Paypal:Host");
            }
        }
        public static string PaypalApiUsername { get { return CommonUtility.GetAppSetting<string>("Paypal:APIUsername"); } }
        public static string PaypalApiPassword { get { return CommonUtility.GetAppSetting<string>("Paypal:APIPassword"); } }
        public static string PaypalApiSignature { get { return CommonUtility.GetAppSetting<string>("Paypal:APISignature"); } }

        public static string ManagerEndPointUrl
        {
            get
            {
                return (Sandbox) ? CommonUtility.GetAppSetting<string>("Paypal:Manager:EndPointURL_SB") :
                    CommonUtility.GetAppSetting<string>("Paypal:Manager:EndPointURL");
            }
        }
        public static string ManagerHost
        {
            get
            {
                return (Sandbox) ? CommonUtility.GetAppSetting<string>("Paypal:Manager:Host_SB") :
                    CommonUtility.GetAppSetting<string>("Paypal:Manager:Host");
            }
        }
        public static string PaypalManagerEnvironment
        {
            get { return (Sandbox) ? "TEST" : "LIVE"; }
        }
        public static string PaypalManagerPartner { get { return CommonUtility.GetAppSetting<string>("Paypal:Manager:Partner"); } }
        public static string PaypalManagerMerchantLogin { get { return CommonUtility.GetAppSetting<string>("Paypal:Manager:MerchantLogin"); } }
        public static string PaypalManagerUser { get { return CommonUtility.GetAppSetting<string>("Paypal:Manager:User"); } }
        public static string PaypalManagerPassword { get { return CommonUtility.GetAppSetting<string>("Paypal:Manager:Password"); } }

        public static string Currency { get { return CommonUtility.GetAppSetting<string>("Paypal:Currency"); } }
        public static string PaypalReturnUrl
        {
            get
            {
                string successResponseUrlFormat = CommonUtility.GetAppSetting<string>("Paypal:ReturnUrl");
                return string.Format(successResponseUrlFormat, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
            }
        }
        public static string PaypalCancelUrl
        {
            get
            {
                string successCancelUrlFormat = CommonUtility.GetAppSetting<string>("Paypal:CancelUrl");
                return string.Format(successCancelUrlFormat, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
            }
        }

        public static string PaypalManagerReturnUrl
        {
            get
            {
                string successResponseUrlFormat = CommonUtility.GetAppSetting<string>("Paypal:Manager:ReturnUrl");
                return string.Format(successResponseUrlFormat, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
            }
        }
        public static string PaypalManagerCancelUrl
        {
            get
            {
                string successCancelUrlFormat = CommonUtility.GetAppSetting<string>("Paypal:Manager:CancelUrl");
                return string.Format(successCancelUrlFormat, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
            }
        }
        public static string PaypalManagerErrorUrl
        {
            get
            {
                string successCancelUrlFormat = CommonUtility.GetAppSetting<string>("Paypal:Manager:ErrorUrl");
                return string.Format(successCancelUrlFormat, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
            }
        }
    }

    public static class PaypalAttributes
    {
        public static string USER = "USER";
        public static string PWD = "PWD";
        public static string SIGNATURE = "SIGNATURE";
        public static string SUBJECT = "SUBJECT";
        public static string VERSION = "VERSION";

        public static string METHOD = "METHOD";
        public static string TOKEN = "TOKEN";
        public static string PAYERID = "PAYERID";
        public static string RETURNURL = "RETURNURL";
        public static string CANCELURL = "CANCELURL";
        public static string ERRORURL = "ERRORURL";
        public static string BRANDNAME = "BRANDNAME";
        public static string PAYMENTREQUEST_n_PAYMENTACTION = "PAYMENTREQUEST_0_PAYMENTACTION";
        public static string PAYMENTREQUEST_n_CURRENCYCODE = "PAYMENTREQUEST_0_CURRENCYCODE";

        public static string L_PAYMENTREQUEST_n_NAMEm = "L_PAYMENTREQUEST_0_NAME{0}";
        public static string L_PAYMENTREQUEST_n_NUMBERm = "L_PAYMENTREQUEST_0_NUMBER{0}";
        public static string L_PAYMENTREQUEST_n_DESCm = "L_PAYMENTREQUEST_n_DESCm{0}";
        public static string L_PAYMENTREQUEST_n_AMTm = "L_PAYMENTREQUEST_0_AMT{0}";
        public static string L_PAYMENTREQUEST_n_QTYm = "L_PAYMENTREQUEST_0_QTY{0}";

        public static string PAYMENTREQUEST_n_ITEMAMT = "PAYMENTREQUEST_0_ITEMAMT";
        public static string PAYMENTREQUEST_n_AMT = "PAYMENTREQUEST_0_AMT";

        public static string NOSHIPPING = "NOSHIPPING";
        public static string ADDROVERRIDE = "ADDROVERRIDE";
        public static string PAYMENTREQUEST_n_SHIPTONAME = "PAYMENTREQUEST_0_SHIPTONAME";
        public static string PAYMENTREQUEST_n_SHIPTOSTREET = "PAYMENTREQUEST_0_SHIPTOSTREET";
        public static string PAYMENTREQUEST_n_SHIPTOSTREET2 = "PAYMENTREQUEST_0_SHIPTOSTREET2";
        public static string PAYMENTREQUEST_n_SHIPTOCITY = "PAYMENTREQUEST_0_SHIPTOCITY";
        public static string PAYMENTREQUEST_n_SHIPTOSTATE = "PAYMENTREQUEST_0_SHIPTOSTATE";
        public static string PAYMENTREQUEST_n_SHIPTOZIP = "PAYMENTREQUEST_0_SHIPTOZIP";
        public static string PAYMENTREQUEST_n_SHIPTOCOUNTRYCODE = "PAYMENTREQUEST_0_SHIPTOCOUNTRYCODE";
        public static string PAYMENTREQUEST_n_SHIPTOPHONENUM = "PAYMENTREQUEST_0_SHIPTOPHONENUM";

        public static string PAYMENTINFO_n_TRANSACTIONID = "PAYMENTINFO_0_TRANSACTIONID";
        public static string PAYMENTINFO_n_TRANSACTIONTYPE = "PAYMENTINFO_0_TRANSACTIONTYPE";
        public static string PAYMENTINFO_n_PAYMENTTYPE = "PAYMENTINFO_0_PAYMENTTYPE";
        public static string PAYMENTINFO_n_ORDERTIME = "PAYMENTINFO_0_ORDERTIME";
        public static string PAYMENTINFO_n_AMT = "PAYMENTINFO_0_AMT";

        //Error Response
        public static string L_ERRORCODEn = "L_ERRORCODE0";
        public static string L_SHORTMESSAGEn = "L_SHORTMESSAGE0";
        public static string L_LONGMESSAGEn = "L_LONGMESSAGE0";
        public static string L_SEVERITYCODEn = "L_SEVERITYCODE0";

        //Direci Payment Attributes
        public static string PARTNER = "PARTNER";
        public static string VENDOR = "VENDOR";

        public static string TRXTYPE = "TRXTYPE";
        public static string AMT = "AMT";

        public static string CURRENCY = "CURRENCY";
        public static string CREATESECURETOKEN = "CREATESECURETOKEN";
        public static string SECURETOKENID = "SECURETOKENID";

        public static string BILLTOFIRSTNAME = "BILLTOFIRSTNAME";
        public static string BILLTOLASTNAME = "BILLTOLASTNAME";
        public static string BILLTOSTREET = "BILLTOSTREET";
        public static string BILLTOCITY = "BILLTOCITY";
        public static string BILLTOSTATE = "BILLTOSTATE";
        public static string BILLTOZIP = "BILLTOZIP";
        public static string BILLTOCOUNTRY = "BILLTOCOUNTRY";

        public static string SHIPTOFIRSTNAME = "SHIPTOFIRSTNAME";
        public static string SHIPTOLASTNAME = "SHIPTOLASTNAME";
        public static string SHIPTOSTREET = "SHIPTOSTREET";
        public static string SHIPTOCITY = "SHIPTOCITY";
        public static string SHIPTOSTATE = "SHIPTOSTATE";
        public static string SHIPTOZIP = "SHIPTOZIP";
        public static string SHIPTOCOUNTRY = "SHIPTOCOUNTRY";

        public static string PPREF = "PPREF";
        public static string RESULT = "RESULT";
        public static string RESPMSG = "RESPMSG";
        public static string TRANSTIME = "TRANSTIME";
    }

    public enum PaypalResponseAcknowledge
    {
        SUCCESS,
        SUCCESSWITHWARNING,
        FAILURE,
        FAILUREWITHWARNING,
        APPROVED
    }

    public class PaypalCheckoutResponse
    {
        private string _token = string.Empty;
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        public string Token
        {
            get
            {
                return Convert.ToString(Decoder["TOKEN"]) ?? string.Empty;
            }
            set { _token = value; }
        }

        private string _payerId = string.Empty;
        /// <summary>
        /// Gets or sets the payer identifier.
        /// </summary>
        /// <value>
        /// The payer identifier.
        /// </value>
        public string PayerId
        {
            get
            {
                return Convert.ToString(Decoder["PAYERID"]) ?? string.Empty;
            }
            set { _payerId = value; }
        }

        /// <summary>
        /// Gets or sets the decoder.
        /// </summary>
        /// <value>
        /// The decoder.
        /// </value>
        public NVPCodec Decoder { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PaypalCheckoutResponse"/> is status.
        /// </summary>
        /// <value>
        ///   <c>true</c> if status; otherwise, <c>false</c>.
        /// </value>
        public bool Status
        {
            get
            {
                string strAck = Decoder["ACK"].ToUpper();
                if (strAck != null && (strAck == PaypalResponseAcknowledge.SUCCESS.ToString()
                || strAck == PaypalResponseAcknowledge.SUCCESSWITHWARNING.ToString()))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets the redirection URL.
        /// </summary>
        /// <value>
        /// The redirection URL.
        /// </value>
        public string RedirectionUrl
        {
            get
            {
                if (this.Status)
                {
                    return string.Format("https://{0}/cgi-bin/webscr?cmd=_express-checkout&token={1}",
                        PaypalSettings.Host, this.Token);
                }
                else return string.Empty;
            }
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        /// <returns></returns>
        public string GetResponseMessage()
        {
            if (this.Decoder != null)
            {
                if (this.Status)
                    return string.Format("https://{0}/cgi-bin/webscr?cmd=_express-checkout&token={1}",
                        PaypalSettings.Host, this.Token);
                else
                    return string.Format("ErrorCode={0}&Desc={1}&Desc2={2}", Decoder["L_ERRORCODE0"],
                        Decoder["L_SHORTMESSAGE0"], Decoder["L_LONGMESSAGE0"]);
            }
            else
                return string.Empty;
        }
    }

    public class PaypalCreditCardCheckoutResponse
    {
        public NVPCodec Decoder { get; set; }

        public string SecurityTokanId { get; set; }

        public string SecurityToken
        {
            get
            {
                return Convert.ToString(Decoder["SECURETOKEN"]) ?? string.Empty;
            }
        }

        public bool Status
        {
            get
            {
                string strAck = Decoder["RESPMSG"].ToUpper();
                if (strAck != null && (strAck == PaypalResponseAcknowledge.APPROVED.ToString()))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        public string Acknowledgement
        {
            get
            {
                return (this.Status) ? PaypalResponseAcknowledge.SUCCESS.ToString()
                    : PaypalResponseAcknowledge.FAILURE.ToString();
            }
        }

        public string RedirectionUrl
        {
            get
            {
                if (this.Status)
                {
                    return string.Format("https://{0}?SECURETOKEN={1}&SECURETOKENID={2}&MODE={3}",
                        PaypalSettings.ManagerHost, this.SecurityToken, this.SecurityTokanId, PaypalSettings.PaypalManagerEnvironment);
                }
                else return string.Empty;
            }
        }

    }
}