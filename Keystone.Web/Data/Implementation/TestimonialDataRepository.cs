
namespace Keystone.Web.Data.Implementation
{

    using Keystone.Web.Data.Context;
    using Keystone.Web.Data.Implementation.Base;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Models;

    public class TestimonialDataRepository
        : DataRepository<Testimonial, TestimonialModel>, ITestimonialDataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePriceDataRepository" /> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public TestimonialDataRepository(UnitOfWork<KeystoneDBEntities> unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}