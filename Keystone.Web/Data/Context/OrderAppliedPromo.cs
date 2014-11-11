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
    
    public partial class OrderAppliedPromo
    {
        public int OrderAppliedPromoId { get; set; }
        public int OrderId { get; set; }
        public int PromoCodeId { get; set; }
        public int UserAccountId { get; set; }
        public decimal AppliedAmount { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public byte[] Version { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual PromoCode PromoCode { get; set; }
        public virtual Status Status { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
