

namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class PromoCodeDataRepository
        : DataRepository<PromoCode, PromoCodeModel>, IPromoCodeDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromoCodeDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public PromoCodeDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}