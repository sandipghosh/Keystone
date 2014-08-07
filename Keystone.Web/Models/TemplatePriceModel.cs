
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;

    public class TemplatePriceModel:BaseModel
    {
        public int TemplatePriceId { get; set; }
        public int TemplateId { get; set; }
        public int DeliveryScheduleId { get; set; }
        public string TemplatePriceCode { get; set; }
        public int TemplateHeight { get; set; }
        public int TemplateWidth { get; set; }
        public int PrintQuantity { get; set; }
        public decimal Price { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public DeliveryScheduleModel DeliverySchedule { get; set; }
        //public TemplateModel Template { get; set; }
    }
}