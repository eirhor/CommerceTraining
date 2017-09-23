using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Security;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Security;

namespace CommerceFundamentalsWeb.Controllers
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly IPromotionEngine _promotionEngine;

        public ShirtVariationController(IContentLoader contentLoader, 
            UrlResolver urlResolver, 
            AssetUrlResolver assetUrlResolver, 
            ThumbnailUrlResolver thumbnailUrlResolver, 
            IOrderRepository orderRepository,
            IOrderGroupFactory orderGroupFactory,
            ILineItemValidator lineItemValidator, IPromotionEngine promotionEngine) : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
            _orderRepository = orderRepository;
            _orderGroupFactory = orderGroupFactory;
            _lineItemValidator = lineItemValidator;
            _promotionEngine = promotionEngine;
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

            List<RewardDescription> rewardDescriptions = _promotionEngine.Evaluate(currentContent.ContentLink).ToList();

            if (rewardDescriptions.Any())
            {
                model.Messages = String.Join("<br/>",rewardDescriptions.Select(x=>x.Promotion.Name));
                model.DiscountPrice = rewardDescriptions.First().SavedAmount;
            }

            return View(model);
        }

        public ActionResult AddToCart(ShirtVariation currentContent, decimal quantity, string monogram)
        {
            // ToDo: (lab D1) add a LineItem to the Cart
            var customer = PrincipalInfo.CurrentPrincipal.GetContactId();
            var cart = _orderRepository.LoadOrCreateCart<ICart>(customer, "Default");

            var lineItem = GetOrAdjustLineItem(cart, currentContent.Code, quantity, monogram);

            cart.UpdatePlacedPriceOrRemoveLineItems((item,issue)=>HandelErrors(item,issue));

            _orderRepository.Save(cart);

            // if we want to redirect
            ContentReference cartRef = _contentLoader.Get<StartPage>(ContentReference.StartPage).Settings.cartPage;
            CartPage cartPage = _contentLoader.Get<CartPage>(cartRef);
            var name = cartPage.Name;
            var lang = ContentLanguage.PreferredCulture;
            string passingValue = cart.Name;

            // go to the cart page, if needed
            return RedirectToAction("Index", lang + "/" + name, new { passedAlong = passingValue });
        }

        private ILineItem GetOrAdjustLineItem(ICart cart, string code, decimal quantity, string monogram)
        {
            var item = cart.GetAllLineItems()
                .FirstOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                item = _orderGroupFactory.CreateLineItem(code, cart);
                item.Quantity = quantity;

                if (_lineItemValidator.Validate(item, cart.Market, (i, issue) => { }))
                {
                    cart.AddLineItem(item);
                }
            }

            item.Properties["Monogram"] = monogram;
            return item;
        }

        private void HandelErrors(ILineItem item, ValidationIssue issue)
        {          
        }


        public void AddToWishList(ShirtVariation currentContent)
        {
            // ToDo: (lab D1) add a LineItem to the Cart
            var customer = PrincipalInfo.CurrentPrincipal.GetContactId();
            var cart = _orderRepository.LoadOrCreateCart<ICart>(customer, "WishList");

            GetOrAdjustLineItem(cart, currentContent.Code, 1, string.Empty);

            cart.UpdatePlacedPriceOrRemoveLineItems((item, issue) => HandelErrors(item, issue));

            _orderRepository.Save(cart);            
        }
    }
}