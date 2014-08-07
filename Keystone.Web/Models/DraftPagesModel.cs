
namespace Keystone.Web.Models
{
    using System;
    using Keystone.Web.Models.Base;
    public class DraftPagesModel : BaseModel
    {
        public DraftPagesModel()
        {
            this.CreatedOn = DateTime.Now;
            this.StatusId = (int)StatusEnum.Active;
        }
        public int DraftPageId { get; set; }
        public int DraftId { get; set; }
        public int TemplateId { get; set; }
        public int TemplatePageId { get; set; }
        public string DraftPreviewUrl { get; set; }
        public string FinalImageUrl { get; set; }
        public string DraftJsonString { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public DraftModel Draft { get; set; }
        public TemplateModel Template { get; set; }
        public TemplatePageModel TemplatePage { get; set; }
    }
}