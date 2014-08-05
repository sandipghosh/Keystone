namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;
    public class ShoppingCartDataRepository
    : DataRepository<ShoppingCart, ShoppingCartModel>, IShoppingCartDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShoppingCartDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public ShoppingCartDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}