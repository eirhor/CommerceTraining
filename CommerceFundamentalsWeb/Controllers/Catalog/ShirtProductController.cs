using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    public class FashionNodeController : CatalogControllerBase<FashionNode>
    {
        // GET: FashionNode
        public ActionResult Index()
        {
            return View(CurrentContent);
        }
    }
}