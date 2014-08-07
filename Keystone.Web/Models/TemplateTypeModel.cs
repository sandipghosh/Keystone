
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System.Collections.Generic;

    public class TemplateTypeModel : BaseModel
    {
        public int TemplateTypeId { get; set; }
        public string TemplateCode { get; set; }
        public int StatusId { get; set; }

        //public virtual IEnumerable<TemplateModel> Templates { get; set; }
        public virtual IEnumerable<TemplatePriceModel> TemplatePrices { get; set; }
    }
}