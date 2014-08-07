
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;
    using System.Collections.Generic;

    public class DraftModel : BaseModel
    {
        public DraftModel()
        {
            //this.CreatedOn = DateTime.Now;
            //this.StatusId = (int)StatusEnum.Active;
        }
        public int DraftId { get; set; }
        public int UserAccountId { get; set; }
        public int TemplateId { get; set; }
        public string DraftName { get; set; }

        public int DeliveryScheduleId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public TemplateModel Template { get; set; }
        public IEnumerable<DraftPagesModel> DraftPages { get; set; }
    }
}