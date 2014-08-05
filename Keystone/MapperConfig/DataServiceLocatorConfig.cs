
namespace Keystone.Web.MapperConfig
{
    using System;
    using AutoMapper;
    using SimpleInjector;
    using SimpleInjector.Advanced;
    using SimpleInjector.Packaging;
    using Keystone.Web.Data.Interface;
    using Keystone.Web.Data.Implementation;
    using Keystone.Web.Utilities;

    public class DataServiceLocatorConfig : IPackage
    {
        /// <summary>
        /// Registers the set of services in the specified <paramref name="container" />.
        /// </summary>
        /// <param name="container">The container the set of services is registered into.</param>
        public void RegisterServices(Container container)
        {
            try
            {
                this.RegisterServiceLocator(container);
                this.AddMapperProfile(container);
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(container);
            }
        }

        /// <summary>
        /// Registers the service locator.
        /// </summary>
        /// <param name="container">The container.</param>
        private void RegisterServiceLocator(Container container)
        {
            try
            {
                container.Register<ITemplateDataRepository, TemplateDataRepository>();
                container.Register<ITemplatePageDataRepository, TemplatePageDataRepository>();
                container.Register<ITemplatePriceDataRepository, TemplatePriceDataRepository>();
                container.Register<IDeliveryScheduleDataRepository, DeliveryScheduleDataRepository>();

                container.Register<IDraftDataRepository, DraftDataRepository>();
                container.Register<IDraftPagesDataRepository, DraftPagesDataRepository>();

                container.Register<IUserAccountDataRepository, UserAccountDataRepository>();
                container.Register<IUserAddressDataRepository, UserAddressDataRepository>();

                container.Register<IOrderDataRepository, OrderDataRepository>();
                container.Register<IOrderItemDataRepository, OrderItemDataRepository>();
                container.Register<IPromoCodeDataRepository, PromoCodeDataRepository>();
                container.Register<IShoppingCartDataRepository, ShoppingCartDataRepository>();
                container.Register<IOrderAppliedPromoDataRepository, OrderAppliedPromoDataRepository>();
                container.Register<IPaymentTypeDataRepository, PaymentTypeDataRepository>();
                container.Register<IPaymentInfoDataRepository, PaymentInfoDataRepository>();

                container.Register<ITestimonialDataRepository, TestimonialDataRepository>();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(container);
            }
        }

        /// <summary>
        /// Adds the mapper profile.
        /// </summary>
        /// <param name="container">The container.</param>
        private void AddMapperProfile(Container container)
        {
            try
            {
                container.AppendToCollection(typeof(Profile),
                    Lifestyle.Singleton.CreateRegistration(typeof(Profile), 
                    typeof(EntityMapperConfig), container));
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker(container);
            }
        }
    }
}