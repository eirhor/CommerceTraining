using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.ViewModels;
using CommerceFundamentalsWeb.Services;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        public ShirtVariationController(ICommerceService commerceService) : base(commerceService)
        {
        }

        public ActionResult Index(ShirtVariation currentContent)
        {
            var model = new ShirtVariationViewModel
            {
                name = currentContent.Name,
                MainBody = currentContent.MainBody,
                priceString = currentContent.GetDefaultPrice().ToString(),
                image = GetDefaultAsset(currentContent),
                CanBeMonogrammed = currentContent.CanBeMonogrammed
            };

            return View(model);
        }
    }
}