using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceFundamentalsWeb.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Markets;

namespace CommerceFundamentalsWeb.Services
{
    [ServiceConfiguration(typeof(ICommerceService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class CommerceService : ICommerceService
    {
        private readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly AssetUrlResolver _assetUrlResolver;
        private readonly ThumbnailUrlResolver _thumbnailUrlResolver;
        private readonly IMarketService _marketService;

        public CommerceService(
            IContentLoader contentLoader, 
            UrlResolver urlResolver, 
            AssetUrlResolver assetUrlResolver, 
            ThumbnailUrlResolver thumbnailUrlResolver,
            IMarketService marketService)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _assetUrlResolver = assetUrlResolver;
            _thumbnailUrlResolver = thumbnailUrlResolver;
            _marketService = marketService;
        }

        public string GetDefaultAsset(IAssetContainer container)
        {
            return _assetUrlResolver.GetAssetUrl(container);
        }

        public string GetNamedAsset(IAssetContainer container, string groups)
        {
            return _thumbnailUrlResolver.GetThumbnailUrl(container, groups);
        }

        public string GetUrl(ContentReference contentLink)
        {
            return _urlResolver.GetUrl(contentLink);
        }

        public IEnumerable<NameAndUrls> GetNodes(ContentReference contentLink)
        {
            var pages = FilterForVisitor.Filter(_contentLoader.GetChildren<NodeContent>(contentLink)).OfType<NodeContent>();

            return pages.Select(p => new NameAndUrls
            {
                name = p.Name,
                url = GetUrl(p.ContentLink),
                imageUrl = GetDefaultAsset(p),
                imageThumbUrl = GetNamedAsset(p, "Thumbnail")
            });
        }

        public IEnumerable<NameAndUrls> GetEntries(ContentReference contentLink)
        {
            var pages = FilterForVisitor.Filter(_contentLoader.GetChildren<EntryContentBase>(contentLink)).OfType<EntryContentBase>();

            return pages.Select(p => new NameAndUrls
            {
                name = p.Name,
                url = GetUrl(p.ContentLink),
                imageUrl = GetDefaultAsset(p),
                imageThumbUrl = GetNamedAsset(p, "Thumbnail")
            });
        }

        public IEnumerable<EntryContentBase> GetVariants(ProductContent product)
        {
            var variantLinks = product.GetVariants();

            return variantLinks.Select(v => _contentLoader.Get<EntryContentBase>(v));
        }
    }
}