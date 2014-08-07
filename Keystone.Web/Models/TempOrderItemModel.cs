

namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    public class TempOrderItemModel : BaseModel
    {
        public string OrderItemIdentifier { get; set; }
        public int TemplateId { get; set; }
        public int DeliveryScheduleId { get; set; }
        public int DraftId { get; set; }
        public string DisplayUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateTitle { get; set; }
        public string TemplateDesc { get; set; }
        public decimal TemplateWidth { get; set; }
        public decimal TemplateHeight { get; set; }
    }
}