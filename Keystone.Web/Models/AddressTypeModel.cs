
namespace Keystone.Web.Models
{
    using System;
    using Keystone.Web.Models.Base;

    public class AddressTypeModel : BaseModel
    {
        public int AddressTypeId { get; set; }
        public string AddressTypeCode { get; set; }
        public int StatusId { get; set; }
    }
}