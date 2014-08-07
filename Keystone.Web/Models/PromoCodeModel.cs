
namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    public class PromoCodeModel:BaseModel
    {
        public int PromoCodeId { get; set; }
        public string PromoCodeName { get; set; }
        public decimal PromoAmount { get; set; }
        public int StatusId { get; set; }
    }
}