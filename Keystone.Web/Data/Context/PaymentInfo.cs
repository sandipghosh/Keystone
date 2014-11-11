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
    
    public partial class PaymentInfo
    {
        public PaymentInfo()
        {
            this.ShoppingCarts = new HashSet<ShoppingCart>();
        }
    
        public int PaymentInfoId { get; set; }
        public int OrderId { get; set; }
        public string RequistedIp { get; set; }
        public string Acknowledgement { get; set; }
        public Nullable<System.DateTime> TransactionTime { get; set; }
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string TransactionErrorCode { get; set; }
        public string TransactionShortMessage { get; set; }
        public string TransactionLongMessage { get; set; }
        public decimal TransactionAmount { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public byte[] Version { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
