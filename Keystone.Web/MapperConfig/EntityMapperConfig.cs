
namespace Keystone.Web.MapperConfig
{
    using System;
    using AutoMapper;
    using Keystone.Web.Models;
    using Keystone.Web.Data.Context;
    using Keystone.Web.Utilities;

    public class EntityMapperConfig : Profile
    {
        /// <summary>
        /// Gets the name of the profile.
        /// </summary>
        /// <value>
        /// The name of the profile.
        /// </value>
        public override string ProfileName
        {
            get
            {
                return "EntityMapperConfig";
            }
        }

        /// <summary>
        /// Override this method in a derived class and call the CreateMap method to associate that map with this profile.
        /// Avoid calling the <see cref="T:AutoMapper.Mapper" /> class from this method.
        /// </summary>
        protected override void Configure()
        {
            try
            {
                Mapper.CreateMap<Template, TemplateModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<TemplateCategoty, TemplateCategotyModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<TemplateType, TemplateTypeModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<TemplatePage, TemplatePageModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<TemplatePrice, TemplatePriceModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<DeliverySchedule, DeliveryScheduleModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<UserAccount, UserAccountModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<UserAddress, UserAddressModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<AddressType, AddressTypeModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<Draft, DraftModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<DraftPage, DraftPagesModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<Order, OrderModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<OrderItem, OrderItemModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<PromoCode, PromoCodeModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<ShoppingCart, ShoppingCartModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<OrderAppliedPromo, OrderAppliedPromoModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<PaymentType, PaymentTypeModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<Testimonial, TestimonialModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();

                Mapper.CreateMap<PaymentInfo, PaymentInfoModel>()
                    .IgnoreAllNonExisting().MapBothWays().IgnoreAllNonExisting();
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
        }
    }
}