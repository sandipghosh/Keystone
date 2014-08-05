
namespace Keystone.Web.Models
{
    using System;
    using Keystone.Web.Models.Base;
    public class ShoppingCartModel : BaseModel
    {
        public ShoppingCartModel()
        {
            this.CreditCard = new CreditCardInfoModel();
        }
        public int ShoppingCartId { get; set; }
        public int UserAccountId { get; set; }
        public int PaymentTypeId { get; set; }
        public int PaymentInfoId { get; set; }
        public int OrderId { get; set; }
        public int? PromoCodeId { get; set; }
        public int BillingAddressId { get; set; }
        public int ShippingAddressId { get; set; }
        public string TransactionId { get; set; }
        public string CartReferance { get; set; }
        public DateTime ExpireOn { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }


        public OrderModel Order { get; set; }
        public PaymentInfoModel PaymentInfo { get; set; }
        public PaymentTypeModel PaymentType { get; set; }
        public PromoCodeModel PromoCode { get; set; }
        public UserAccountModel UserAccount { get; set; }
        public UserAddressModel BillingAddress { get; set; }
        public UserAddressModel ShippingAddress { get; set; }

        public CreditCardInfoModel CreditCard { get; set; }
    }
}