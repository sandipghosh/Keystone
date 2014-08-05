

namespace Keystone.Web.Models
{
    using Keystone.Web.Models.Base;
    using System;
    using Keystone.Web.Utilities;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel.DataAnnotations;
    public class UserAccountModel : BaseModel
    {
        public int UserAccountId { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Password { get; set; }

        [Display(Name = "Email Id")]
        public string EmailId { get; set; }
        public bool IsNewUser { get; set; }
        public bool IsAdmin { get; set; }

        [Display(Name = "Contact Number")]
        public string PrimaryContact { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pin { get; set; }
        public string Country { get; set; }
        public int StatusId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string RowId { get; set; }

        public IEnumerable<UserAddressModel> UserAddresses { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture
                .TextInfo.ToTitleCase(string.Format("{0} {1}", this.FirstName, this.LastName));
        }

        public UserAddressModel GetAddress(AddressTypeEnum addressType)
        {
            UserAddressModel userAddress = null;
            try
            {
                userAddress = this.UserAddresses.LastOrDefault
                    (x => x.AddressTypeId.Equals((int)addressType));
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
            return userAddress;
        }
    }
}