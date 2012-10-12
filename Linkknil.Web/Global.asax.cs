using System.Web.Mvc;
using System.Web.Routing;
using CoolCode.ServiceModel.Mvc;
using CoolCode.ServiceModel.Services;
using Linkknil.Models;
using Linkknil.Razor.Models;
using Ninject;
using System.Data.Entity;
using Linkknil.StreamStore;

namespace Linkknil.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            RegisterDependencies();

            LinkknilContext.SetInitializer();
            //是否启用数据库视图提供者。如果启用，则可通过数据库加载视图。
            //System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new DbVirtualPathProvider("SqlCache"));

            //AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            JobConfig.RegisterJobs();
        }

        private static void RegisterDependencies() {
            IKernel kernel = new StandardKernel(new DefaultModule());
            kernel.Rebind<DbContext>().To<LinkknilContext>();
            kernel.Rebind<IEntityWatcher>().To<CustomEntityWatcher>();
            kernel.Bind<IFileService>().To<AliyunFileService>();
            //kernel.Rebind<IFileService>().To<WebFileService>();
            DependencyResolver.SetResolver(new NInjectDependencyResolver(kernel));
        }
    }
}