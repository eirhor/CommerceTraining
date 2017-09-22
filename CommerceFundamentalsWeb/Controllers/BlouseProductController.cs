using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers
{
    public class BlouseProductController : CatalogControllerBase<BlouseProduct>
    {
        public BlouseProductController(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(BlouseProduct currentContent, StartPage currentPage)
        { 
            var model = new BlouseProductViewModel(currentContent,currentPage);
            model.ProductVariations = GetVariants(currentContent);
            model.CampaignLink = currentPage.campaignLink;

            return View(model);
        }
    }
}