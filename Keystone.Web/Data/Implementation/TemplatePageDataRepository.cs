
namespace Keystone.Web.Data.Implementation
{
    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class TemplatePageDataRepository
        : DataRepository<TemplatePage, TemplatePageModel>, ITemplatePageDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePageDataRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public TemplatePageDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}