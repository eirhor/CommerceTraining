using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using CommerceFundamentalsWeb.Services;
using CommerceFundamentalsWeb.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class CatalogControllerBase<T> : ContentController<T> where T : CatalogContentBase
    {
        private readonly ICommerceService _commerceService;

        public CatalogControllerBase(ICommerceService commerceService)
        {
            _commerceService = commerceService;
        }

        public string GetDefaultAsset(IAssetContainer container)
        {
            return _commerceService.GetDefaultAsset(container);
        }

        public string GetNamedAsset(IAssetContainer container, string groups)
        {
            return _commerceService.GetNamedAsset(container, groups);
        }

        public string GetUrl(ContentReference contentLink)
        {
            return _commerceService.GetUrl(contentLink);
        }

        public IEnumerable<NameAndUrls> GetNodes(ContentReference contentLink)
        {
            return _commerceService.GetNodes(contentLink);
        }

        public IEnumerable<NameAndUrls> GetEntries(ContentReference contentLink)
        {
            return _commerceService.GetEntries(contentLink);
        }
    }
}