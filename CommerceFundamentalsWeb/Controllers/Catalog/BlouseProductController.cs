﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Services;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    public class BlouseProductController : CatalogControllerBase<BlouseProduct>
    {
        public BlouseProductController(ICommerceService commerceService) : base(commerceService)
        {
        }

        public ActionResult Index(ShirtProduct currentContent)
        {
            return View(currentContent);
        }
    }
}