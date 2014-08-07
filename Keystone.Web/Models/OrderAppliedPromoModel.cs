
namespace Keystone.Web.Models
{
    using System;
    using Keystone.Web.Models.Base;
    public class OrderAppliedPromoModel : BaseModel
    {
        public int OrderAppliedPromoId { get; set; }
        public int OrderId { get; set; }
        public int PromoCodeId { get; set; }
        public int UserAccountId { get; set; }
        public decimal AppliedAmount { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual PromoCodeModel PromoCode { get; set; }
        public virtual OrderModel Order { get; set; }
        public virtual UserAccountModel UserAccount { get; set; }
    }
}