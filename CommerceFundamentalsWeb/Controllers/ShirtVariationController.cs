using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        public ShirtVariationController(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(ShirtVariation currentContent)
        {
            ShirtVariationViewModel model = new ShirtVariationViewModel();
            model.Name = currentContent.DisplayName;
            model.CanBeMonogrammed = currentContent.CanBeMonogrammed;
            model.Image = GetDefaultAsset(currentContent);
            model.MainBody = currentContent.MainBody;
            model.Url = GetUrl(currentContent.ContentLink);
            model.PriceString = currentContent.GetDefaultPrice().UnitPrice.Amount.ToString("0.00");

            return View(model);
        }
    }
}