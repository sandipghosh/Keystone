
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class OrderModel : BaseModel
    {
        public int OrderId { get; set; }
        public int UserAccountId { get; set; }
        public string OrderReferance { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        #region Additional Properties
        private int _PromoCodeId;
        /// <summary>
        /// Gets or sets the promo code identifier.
        /// </summary>
        /// <value>
        /// The promo code identifier.
        /// </value>
        public int PromoCodeId
        {
            get
            {
                if (_PromoCodeId == 0)
                {
                    int orderId = this.OrderId;
                    int userAccountId = this.UserAccountId;

                    if (this.OrderAppliedPromoes != null)
                    {
                        OrderAppliedPromoModel orderPromo = this.OrderAppliedPromoes
                            .FirstOrDefault(x => x.OrderId.Equals(orderId)
                                && x.UserAccountId.Equals(userAccountId)
                                && x.StatusId.Equals((int)StatusEnum.Active));

                        if (orderPromo != null)
                            _PromoCodeId = orderPromo.PromoCodeId;
                    }
                }
                return _PromoCodeId;
            }
            set { _PromoCodeId = value; }
        }

        /// <summary>
        /// Gets the promo code.
        /// </summary>
        /// <value>
        /// The promo code.
        /// </value>
        public string PromoCode
        {
            get
            {
                return this.OrderAppliedPromoes.FirstOrDefault() != null ?
                    this.OrderAppliedPromoes.FirstOrDefault().PromoCode.PromoCodeName : "";
            }
        }

        /// <summary>
        /// Gets my property.
        /// </summary>
        /// <value>
        /// My property.
        /// </value>
        public string OrderMonth { get { return this.OrderDate.ToString("MMM yy", CultureInfo.InvariantCulture); } }
        #endregion

        public string DisplayImage
        {
            get
            {
                string returnValue = string.Empty;
                try
                {
                    var draftPage = this.OrderItems.Select(x => x.Draft)
                        .SelectMany(x => x.DraftPages).FirstOrDefault();
                    returnValue = draftPage.DraftPreviewUrl;
                }
                catch (Exception ex) { }
                return returnValue;
            }
        }

        /// <summary>
        /// Gets or sets the user account.
        /// </summary>
        /// <value>
        /// The user account.
        /// </value>
        public UserAccountModel UserAccount { get; set; }

        /// <summary>
        /// Gets or sets the order items.
        /// </summary>
        /// <value>
        /// The order items.
        /// </value>
        public IEnumerable<OrderItemModel> OrderItems { get; set; }

        /// <summary>
        /// Gets or sets the order applied promoes.
        /// </summary>
        /// <value>
        /// The order applied promoes.
        /// </value>
        public virtual IEnumerable<OrderAppliedPromoModel> OrderAppliedPromoes { get; set; }
    }
}