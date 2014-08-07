
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;
    using System.Collections.Generic;

    public class OrderItemModel : BaseModel
    {
        public string OrderItemIdentifier { get; set; }
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int TemplateId { get; set; }
        public int DeliveryScheduleId { get; set; }
        public int DraftId { get; set; }
        public string DisplayUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public DeliveryScheduleModel DeliverySchedule { get; set; }
        public DraftModel Draft { get; set; }
        public OrderModel Order { get; set; }
        public TemplateModel Template { get; set; }
    }
}