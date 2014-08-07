
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    public class PaymentTypeModel : BaseModel
    {
        public int PaymentTypeId { get; set; }
        public string PaymentCode { get; set; }
        public int StatusId { get; set; }
    }
}