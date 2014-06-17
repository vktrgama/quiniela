using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using quiniela.Services;
using quiniela.Helpers;

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

            var _quinielaService = DependencyResolver.Current.GetService<IQuinielaService>();

            // Update Date/time for each match
            // _quinielaService.UpdateMatchDates();
            // Load in memory for future comparison
            // Application["MatchDates"] = _quinielaService.GetMatchDatesInMem();
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