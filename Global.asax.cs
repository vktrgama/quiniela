using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using quiniela.Services;

namespace quiniela
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RegisterDependencyResolver();
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Registers the dependency resolver.
        /// </summary>
        private void RegisterDependencyResolver()
        {
            var kernel = new StandardKernel();
            kernel.Bind<IQuinielaService>().To<QuinielaService>();
            DependencyResolver.SetResolver(new DependencyInjection.NinjectDependencyResolver(kernel));
        }
    }
}