
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class PaymentTypeDataRepository
        : DataRepository<PaymentType, PaymentTypeModel>, IPaymentTypeDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentTypeDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public PaymentTypeDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}