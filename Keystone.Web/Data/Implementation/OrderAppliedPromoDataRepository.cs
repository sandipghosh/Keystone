namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    public class OrderAppliedPromoDataRepository
        : DataRepository<OrderAppliedPromo, OrderAppliedPromoModel>, IOrderAppliedPromoDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderAppliedPromoDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public OrderAppliedPromoDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}