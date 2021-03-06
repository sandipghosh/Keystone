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
    
    public partial class DraftPage
    {
        public int DraftPageId { get; set; }
        public int DraftId { get; set; }
        public int TemplateId { get; set; }
        public int TemplatePageId { get; set; }
        public string DraftPreviewUrl { get; set; }
        public string FinalImageUrl { get; set; }
        public string DraftJsonString { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public byte[] Version { get; set; }
    
        public virtual Draft Draft { get; set; }
        public virtual Status Status { get; set; }
        public virtual Template Template { get; set; }
        public virtual TemplatePage TemplatePage { get; set; }
    }
}
