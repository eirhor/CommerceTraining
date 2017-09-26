using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using CommerceFundamentalsWeb.Services;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Core;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    public class BlouseProductController : CatalogControllerBase<BlouseProduct>
    {


        public BlouseProductController(ICommerceService commerceService) : base(commerceService)
        {
        }

        public ActionResult Index(BlouseProduct currentContent, StartPage currentPage)
        {
            var model = new BlouseProductViewModel(currentContent, currentPage)
            {
                Variations = GetVariants(currentContent),
                CampaignLink = currentPage.campaignLink
            };

            return View(model);
        }
    }
}