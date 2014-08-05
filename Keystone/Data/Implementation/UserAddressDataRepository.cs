
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class UserAddressDataRepository
        : DataRepository<UserAddress, UserAddressModel>, IUserAddressDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserAddressDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public UserAddressDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}