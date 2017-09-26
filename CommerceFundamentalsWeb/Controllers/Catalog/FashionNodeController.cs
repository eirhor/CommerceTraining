using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    public class FashionNodeController : CatalogControllerBase<FashionNode>
    {
        public FashionNodeController(IContentLoader contentLoader, IUrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) :
            base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        // GET: FashionNode
        public ActionResult Index(FashionNode currentContent)
        {
            var nodeEntryCombo = new NodeEntryCombo
            {
                entries = GetEntries(currentContent.ContentLink),
                nodes = GetNodes(currentContent.ContentLink)
            };

            return View(nodeEntryCombo);
        }
    }
}