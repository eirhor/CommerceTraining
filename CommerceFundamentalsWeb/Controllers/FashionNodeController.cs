using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.ViewModels;
using CommerceFundamentalsWeb.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Security;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;

namespace CommerceFundamentalsWeb.Controllers
{
    public class FashionNodeController : CatalogControllerBase<FashionNode>
    {
        private IContentRepository _contentRepository;
        private readonly ReferenceConverter _referenceConverter;

        public FashionNodeController(IContentRepository contentRepository, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver, ReferenceConverter referenceConverter) : base(contentRepository, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
            _contentRepository = contentRepository;
            _referenceConverter = referenceConverter;
        }

        public ActionResult Index(FashionNode currentContent)
        {            
            NodeEntryCombo model = new NodeEntryCombo();

            model.Entries = GetEntries(currentContent.ContentLink);
            model.Nodes = GetNodes(currentContent.ContentLink);

            if (model.Entries.Any() || model.Nodes.Any())
            {
                return View(model);
            }
            else
            {
                NewBlouseObject newModel = new NewBlouseObject();
                newModel.Code = currentContent.Code;
                return View("CreateDefaultEntry", newModel);
            }
        }

        public ActionResult CreateDefaultEntry (FashionNode currentContent, NewBlouseObject model)
        {
            var parentReference = _referenceConverter.GetContentLink(model.Code);

            var product = _contentRepository.GetDefault<BlouseProduct>(parentReference);
            product.Name = model.NewName;
            product.Code = model.NewCode;

            var productContentReference = _contentRepository.Save(product, SaveAction.Publish, AccessLevel.NoAccess);

            var sizes = model.Sizes.Split(';');

            foreach (var size in sizes)
            {
                var sku = _contentRepository.GetDefault<ShirtVariation>(productContentReference);
                sku.Name = $"{model.NewName} {model.Color} {size}";
                sku.Code =$"{model.NewCode}{model.Color}{size}";
                sku.Color = model.Color;
                sku.Size = size;
                _contentRepository.Save(sku, SaveAction.Publish, AccessLevel.NoAccess);
            }


            return new RedirectResult(GetUrl(currentContent.ContentLink));
        }
    }
}