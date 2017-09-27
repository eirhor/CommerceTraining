using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Business;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using CommerceFundamentalsWeb.Services;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Security;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Security;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        private readonly IContentLoader _contentLoader;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly ILineItemValidator _lineItemValidator;

        public ShirtVariationController(
            ICommerceService commerceService, 
            IContentLoader contentLoader,
            IOrderRepository orderRepository,
            IOrderGroupFactory orderGroupFactory,
            ILineItemValidator lineItemValidator) : 
            base(commerceService)
        {
            _contentLoader = contentLoader;
            _orderRepository = orderRepository;
            _orderGroupFactory = orderGroupFactory;
            _lineItemValidator = lineItemValidator;
        }

        public ActionResult Index(ShirtVariation currentContent)
        {
            var model = new ShirtVariationViewModel
            {
                name = currentContent.Name,
                MainBody = currentContent.MainBody,
                priceString = currentContent.GetDefaultPrice().UnitPrice.ToString(),
                image = GetDefaultAsset(currentContent),
                CanBeMonogrammed = currentContent.CanBeMonogrammed
            };

            return View(model);
        }


        public ActionResult AddToCart(ShirtVariation currentContent, decimal Quantity, string Monogram)
        {
            // ToDo: (lab D1) add a LineItem to the Cart
            var customerId = PrincipalInfo.CurrentPrincipal.GetContactId();
            var cart = _orderRepository.LoadOrCreateCart<ICart>(customerId, "Default");

            var code = currentContent.Code;
            var lineItems = cart.GetAllLineItems();

            var lineItem = lineItems.FirstOrDefault(l => l.Code.Equals(code));

            if (lineItem != null)
            {
                lineItem.Quantity += Quantity;
            }
            else
            {
                lineItem = _orderGroupFactory.CreateLineItem(code, cart);
                lineItem.Quantity = Quantity;
            }

            var validated = _lineItemValidator.Validate(lineItem, cart.Market, (item, issue) => { });

            if (validated == false)
            {
                ModelState.AddModelError(string.Empty, "Super helpful message");
                return View(currentContent);
            }

            lineItem.Properties["Monogram"] = Monogram;
            cart.AddLineItem(lineItem);

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


        public void AddToWishList(ShirtVariation currentContent)
        {

        }
    }
}