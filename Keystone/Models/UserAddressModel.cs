

namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;

    public class UserAddressModel:BaseModel, ICloneable
    {
        public int UserAddressId { get; set; }
        public int UaserAccountId { get; set; }
        public int AddressTypeId { get; set; }
        public bool IsCurrentAddress { get; set; }
        public string SeconderyContact { get; set; }
        public string Address1 { get; set; }
        //public string Address2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pin { get; set; }
        public string Country { get; set; }
        public int StatusId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public AddressTypeModel AddressType { get; set; }
        //public UserAccountModel UserAccount { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}