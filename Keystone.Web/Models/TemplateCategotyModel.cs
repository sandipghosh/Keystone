
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;

    public class TemplateCategotyModel:BaseModel
    {
        public int TemplateCategotyId { get; set; }
        public string TemplateCategotyCode { get; set; }
        public int StatusId { get; set; }
    }
}