
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class PaymentInfoDataRepository
        : DataRepository<PaymentInfo, PaymentInfoModel>, IPaymentInfoDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderItemDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public PaymentInfoDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}