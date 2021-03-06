//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Keystone.Web.Data.Context
{
    using System;
    using System.Collections.Generic;
    
    public partial class Status
    {
        public Status()
        {
            this.AddressTypes = new HashSet<AddressType>();
            this.DeliverySchedules = new HashSet<DeliverySchedule>();
            this.Drafts = new HashSet<Draft>();
            this.DraftPages = new HashSet<DraftPage>();
            this.Orders = new HashSet<Order>();
            this.OrderAppliedPromoes = new HashSet<OrderAppliedPromo>();
            this.OrderItems = new HashSet<OrderItem>();
            this.OrderItemPages = new HashSet<OrderItemPage>();
            this.PaymentInfoes = new HashSet<PaymentInfo>();
            this.PromoCodes = new HashSet<PromoCode>();
            this.ShoppingCarts = new HashSet<ShoppingCart>();
            this.Templates = new HashSet<Template>();
            this.TemplatePages = new HashSet<TemplatePage>();
            this.TemplatePrices = new HashSet<TemplatePrice>();
            this.TemplateTypes = new HashSet<TemplateType>();
            this.Testimonials = new HashSet<Testimonial>();
            this.UserAccounts = new HashSet<UserAccount>();
            this.UserAddresses = new HashSet<UserAddress>();
        }
    
        public int StatusId { get; set; }
        public string StatusCode { get; set; }
    
        public virtual ICollection<AddressType> AddressTypes { get; set; }
        public virtual ICollection<DeliverySchedule> DeliverySchedules { get; set; }
        public virtual ICollection<Draft> Drafts { get; set; }
        public virtual ICollection<DraftPage> DraftPages { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<OrderAppliedPromo> OrderAppliedPromoes { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderItemPage> OrderItemPages { get; set; }
        public virtual ICollection<PaymentInfo> PaymentInfoes { get; set; }
        public virtual ICollection<PromoCode> PromoCodes { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public virtual ICollection<Template> Templates { get; set; }
        public virtual ICollection<TemplatePage> TemplatePages { get; set; }
        public virtual ICollection<TemplatePrice> TemplatePrices { get; set; }
        public virtual ICollection<TemplateType> TemplateTypes { get; set; }
        public virtual ICollection<Testimonial> Testimonials { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
        public virtual ICollection<UserAddress> UserAddresses { get; set; }
    }
}
