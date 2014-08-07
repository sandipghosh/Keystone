

namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;
    using System.Globalization;

    public class PaymentInfoModel : BaseModel
    {
        public int PaymentInfoId { get; set; }
        public int OrderId { get; set; }
        public string RequistedIp { get; set; }
        public string Acknowledgement { get; set; }
        public DateTime TransactionTime { get; set; }
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string TransactionErrorCode { get; set; }
        public string TransactionShortMessage { get; set; }
        public string TransactionLongMessage { get; set; }
        public decimal TransactionAmount { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public OrderModel Order { get; set; }

        public string PaymentMonth { get { return this.TransactionTime.ToString("MMM yy", CultureInfo.InvariantCulture); } }

        public override string ToString()
        {
            return string.Format("Error Code: {0}#{1}", this.TransactionErrorCode, this.TransactionShortMessage);
        }
    }
}