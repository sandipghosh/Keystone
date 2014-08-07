
namespace Keystone.Web.Models
{
    using System;
    using Keystone.Web.Models.Base;

    public class DeliveryScheduleModel:BaseModel
    {
        public int DeliveryScheduleId { get; set; }
        public string DeliveryCode { get; set; }
        public int DeliveryFrom { get; set; }
        public int DeliveryTo { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1}-{2} days)", 
                this.DeliveryCode, this.DeliveryFrom, this.DeliveryTo);
        }
    }
}