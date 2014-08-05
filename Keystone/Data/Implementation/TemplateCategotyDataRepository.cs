
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class TemplateCategotyDataRepository
        : DataRepository<TemplateCategoty, TemplateCategotyModel>, ITemplateCategotyDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateCategotyDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public TemplateCategotyDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}