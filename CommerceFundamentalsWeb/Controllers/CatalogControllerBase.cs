using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers
{
    public class CatalogControllerBase<T> : ContentController<T> where T : CatalogContentBase
    {
        protected readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly AssetUrlResolver _assetUrlResolver;
        private readonly ThumbnailUrlResolver _thumbnailUrlResolver;

        public CatalogControllerBase(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _assetUrlResolver = assetUrlResolver;
            _thumbnailUrlResolver = thumbnailUrlResolver;
        }

        public string GetDefaultAsset(IAssetContainer asset)
        {
            return _assetUrlResolver.GetAssetUrl(asset);
        }

        public string GetNamedAsset(IAssetContainer asset, string name)
        {
            return _thumbnailUrlResolver.GetThumbnailUrl(asset, name);
        }

        public string GetUrl(ContentReference content)
        {
            return _urlResolver.GetUrl(content);
        }

        public List<NameAndUrls> GetNodes(ContentReference content)
        {
            var children = FilterForVisitor.Filter(_contentLoader.GetChildren<FashionNode>(content)).OfType<FashionNode>();

            var returnList =
                children.Select(
                    x =>
                        new NameAndUrls()
                        {
                            Name = x.DisplayName,
                            Url = GetUrl(x.ContentLink),
                            ImageUrl = GetDefaultAsset(x),
                            ImageThumbUrl = GetNamedAsset(x, "Thumbnail")
                        });

            return returnList.ToList();
        }

        public IEnumerable<EntryContentBase> GetVariants(ProductContent content)
        {
            var variants = content.GetVariants();
            var variantItems = _contentLoader.GetItems(variants, LanguageSelector.AutoDetect());
            return FilterForVisitor.Filter(variantItems).OfType<EntryContentBase>();
        }

        public List<NameAndUrls> GetEntries(ContentReference content)
        {
            var children = FilterForVisitor.Filter(_contentLoader.GetChildren<EntryContentBase>(content)).OfType<EntryContentBase>();

            var returnList =
                children.Select(
                    x =>
                        new NameAndUrls()
                        {
                            Name = x.DisplayName,
                            Url = GetUrl(x.ContentLink),
                            ImageUrl = GetDefaultAsset(x),
                            ImageThumbUrl = GetNamedAsset(x, "Thumbnail")
                        });

            return returnList.ToList();
        }
    }    
}