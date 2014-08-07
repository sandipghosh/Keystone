

namespace Keystone.Web.Models
{
    using System;
    using Keystone.Web.Utilities;

    public class CreditCardInfoModel
    {
        public CreditCardInfoModel()
        {
            this._ExpiryMonth = DateTime.Now.Month;
            this._ExpiryYear = DateTime.Now.Year;
        }
        //public string HolderName { get; set; }
        public string IPAddress { get { return CommonUtility.GetClientIPAddress(); } }
        public string CardAccountNumber { get; set; }
        public string CardType { get; set; }

        private int _ExpiryMonth;
        public int ExpiryMonth { get { return _ExpiryMonth; } set { _ExpiryMonth = value; } }

        private int _ExpiryYear;
        public int ExpiryYear { get { return _ExpiryYear; } set { _ExpiryYear = value; } }
        public string Expiary { get { return string.Format("{0:00}{1:0000}", this.ExpiryMonth, this.ExpiryYear); } }
        public string CvcNumber { get; set; }
    }
}