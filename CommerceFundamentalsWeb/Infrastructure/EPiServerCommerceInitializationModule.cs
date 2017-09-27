using System.Web.Mvc;
using System.Web.Routing;
using CommerceFundamentalsWeb.Business;
using EPiServer.Commerce.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Pricing;

namespace CommerceFundamentalsWeb.Infrastructure
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class EPiServerCommerceInitializationModule : IInitializableModule, IConfigurableModule
    {
        public void Initialize(InitializationEngine context)
        {
            CatalogRouteHelper.MapDefaultHierarchialRouter(RouteTable.Routes, false);
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.StructureMap()));

            context.StructureMap().Configure(x => x.For<IPriceService>().Use<RandomPriceService>());

        }
    }
}
