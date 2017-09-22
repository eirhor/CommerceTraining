using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers
{
    public class ShirtProductController : CatalogControllerBase<ShirtProduct>
    {
        public ShirtProductController(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(ShirtProduct currentContent)
        {
            return View(currentContent);
        }
    }
}