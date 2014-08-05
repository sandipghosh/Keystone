
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class TemplateDataRepository
        : DataRepository<Template, TemplateModel>, ITemplateDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public TemplateDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}