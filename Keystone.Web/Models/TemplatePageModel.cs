

namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;

    public class TemplatePageModel : BaseModel
    {
        public int TemplatePageId { get; set; }
        public int TemplateId { get; set; }
        public int PageHeight { get; set; }
        public int PageWidth { get; set; }
        public string TemplatePageUrl { get; set; }
        public string TemplatePageJson { get; set; }
        public int OrderIndex { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public TemplateModel Template { get; set; }
    }
}