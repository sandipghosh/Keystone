
namespace Keystone.Web
{
    using SimpleInjector;
    using SimpleInjector.Integration.Web.Mvc;
    using System.Reflection;
    using System.Web.Mvc;
    using System;
    using Keystone.Web.Utilities;

    public class InjectorInitializer
    {
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                var container = new Container();
                container.RegisterPackages();
                container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
                container.RegisterMvcAttributeFilterProvider();
                container.Verify();

                MapperInitializer.RegisterMapper(container);
                DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            }
            catch (Exception ex)
            {
                ex.ExceptionValueTracker();
            }
        }
    }
}