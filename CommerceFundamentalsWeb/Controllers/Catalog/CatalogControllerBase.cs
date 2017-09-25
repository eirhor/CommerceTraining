using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
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
        private readonly IContentLoader _contentLoader;
        private readonly IUrlResolver _urlResolver;
        private readonly AssetUrlResolver _assetUrlResolver;
        private readonly ThumbnailUrlResolver _thumbnailUrlResolver;

        public CatalogControllerBase(IContentLoader contentLoader, IUrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _assetUrlResolver = assetUrlResolver;
            _thumbnailUrlResolver = thumbnailUrlResolver;
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
            var pages = FilterForVisitor.Filter(_contentLoader.GetChildren<NodeContent>(contentLink)).Cast<NodeContent>();

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
            var pages = FilterForVisitor.Filter(_contentLoader.GetChildren<EntryContentBase>(contentLink)).Cast<EntryContentBase>();

            return pages.Select(p => new NameAndUrls
            {
                name = p.Name,
                url = GetUrl(p.ContentLink),
                imageUrl = GetDefaultAsset(p),
                imageThumbUrl = GetNamedAsset(p, "Thumbnail")
            });
        }
    }
}