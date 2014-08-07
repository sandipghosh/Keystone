
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class DeliveryScheduleDataRepository
        : DataRepository<DeliverySchedule, DeliveryScheduleModel>, IDeliveryScheduleDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeliveryScheduleDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public DeliveryScheduleDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}