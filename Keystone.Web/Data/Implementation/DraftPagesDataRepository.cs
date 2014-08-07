
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class DraftPagesDataRepository
        : DataRepository<DraftPage, DraftPagesModel>, IDraftPagesDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DraftPagesDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public DraftPagesDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}