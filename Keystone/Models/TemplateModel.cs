
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;
    using System.Collections.Generic;

    public class TemplateModel : BaseModel
    {
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
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }


        public TemplateCategotyModel TemplateCategoty { get; set; }
        public TemplateTypeModel TemplateType { get; set; }
        public IEnumerable<TemplatePageModel> TemplatePages { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1:0.0}\" - {2:0}\" {3}", this.TemplateTitle, this.TemplateWidth,
                this.TemplateHeight, this.TemplateCategoty.TemplateCategotyCode);
        }
    }
}