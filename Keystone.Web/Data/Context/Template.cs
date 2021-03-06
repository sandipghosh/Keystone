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
    
    public partial class Template
    {
        public Template()
        {
            this.Drafts = new HashSet<Draft>();
            this.DraftPages = new HashSet<DraftPage>();
            this.OrderItems = new HashSet<OrderItem>();
            this.TemplatePages = new HashSet<TemplatePage>();
        }
    
        public int TemplateId { get; set; }
        public int TemplateTypeId { get; set; }
        public int TemplateCategotyId { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateTitle { get; set; }
        public string TemplateDesc { get; set; }
        public int TotalPages { get; set; }
        public decimal TemplateHeight { get; set; }
        public decimal TemplateWidth { get; set; }
        public string GalleryUrl { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public byte[] Version { get; set; }
    
        public virtual ICollection<Draft> Drafts { get; set; }
        public virtual ICollection<DraftPage> DraftPages { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual Status Status { get; set; }
        public virtual TemplateCategoty TemplateCategoty { get; set; }
        public virtual TemplateType TemplateType { get; set; }
        public virtual ICollection<TemplatePage> TemplatePages { get; set; }
    }
}
