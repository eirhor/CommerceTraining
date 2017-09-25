using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    public class ShirtProductController : CatalogControllerBase<ShirtProduct>
    {
        public ShirtProductController(IContentLoader contentLoader, IUrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) :
            base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        // GET: FashionNode
        public ActionResult Index(ShirtProduct currentContent)
        {
            return View(currentContent);
        }
    }
}