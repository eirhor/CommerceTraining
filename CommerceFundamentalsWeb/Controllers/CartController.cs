using System;
using System.Linq;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Marketing;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.Web.Mvc;
using Mediachase.Commerce;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Security;

namespace CommerceFundamentalsWeb.Controllers
{
    public class CartController : PageController<CartPage>
    {
        
        private const string DefaultCartName = "Default";

        private readonly IOrderRepository _orderRepository;
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly IOrderGroupCalculator _orderGroupCalculator;
        private readonly IPromotionEngine _promotionEngine;
        private readonly IContentLoader _contentLoader;
        private readonly ILineItemCalculator _lineItemCalculator;
        private readonly IInventoryProcessor _inventoryProcessor;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly IPlacedPriceProcessor _placedPriceProcessor;

        public CartController(IOrderRepository orderRepository
            , IOrderGroupFactory orderGroupFactory
            , IOrderGroupCalculator orderGroupCalculator
            , IContentLoader contentLoader
            , ILineItemCalculator lineItemCalculator
            , IPlacedPriceProcessor placedPriceProcessor
            , IInventoryProcessor inventoryProcessor
            , ILineItemValidator lineItemValidator
            , IPromotionEngine promotionEngine)
        {
            _orderRepository = orderRepository;
            _orderGroupFactory = orderGroupFactory;
            _orderGroupCalculator = orderGroupCalculator;
            _contentLoader = contentLoader;
            _promotionEngine = promotionEngine;
            _lineItemCalculator = lineItemCalculator;
            _inventoryProcessor = inventoryProcessor;
            _lineItemValidator = lineItemValidator;
            _placedPriceProcessor = placedPriceProcessor;
        }
        public ActionResult Index(CartPage currentPage)
        {
            CartViewModel model = new CartViewModel();
            var contactId = GetContactId();
            var cart = _orderRepository.LoadCart<ICart>(contactId,DefaultCartName);

            if (cart == null)
            {
                return View("NoCart");
            }
            else
            {
                var warningMessages = ValidateCart(cart);

                if (string.IsNullOrEmpty(warningMessages))
                {
                    warningMessages = "No Messages";
                }

                var descriptions = _promotionEngine.Run(cart);

                Money totalDiscount = _orderGroupCalculator.GetOrderDiscountTotal(cart, cart.Currency);

                model.PromotionMessage = string.Join("<br/>", descriptions.Select(x => x.Promotion.Name));
                model.OrderDiscount = totalDiscount;
                model.LineItems = cart.GetAllLineItems();
                model.SubTotal = _orderGroupCalculator.GetSubTotal(cart);
                model.WarningMessage = warningMessages;

                _orderRepository.Save(cart);
            }
            // The below is a dummy, remove when lab D2 is done
            return View(model);
        }

        public ActionResult Checkout()
        {
            // Final steps and go to checkout
            StartPage home = _contentLoader.Get<StartPage>(ContentReference.StartPage);
            ContentReference theRef = home.Settings.checkoutPage;
            string passingValue = "Coding is fun"; // could pass something of the cart instead

            return RedirectToAction("Index", new { node = theRef, passedAlong = passingValue }); 
        }

        private string ValidateCart(ICart cart)
        {
            var validationMessages = string.Empty;

            cart.ValidateOrRemoveLineItems((item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _lineItemValidator);

            cart.UpdatePlacedPriceOrRemoveLineItems(GetContact(), (item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _placedPriceProcessor);

            // Should maybe do this during CheckOut, seems not to do a inv-request
            cart.UpdateInventoryOrRemoveLineItems((item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _inventoryProcessor);

            return validationMessages; // now this one also works
        }

        private static string CreateValidationMessages(ILineItem item, ValidationIssue issue)
        {
            return string.Format("Line item with code {0} had the validation issue {1}.", item.Code, issue);
        }

        protected static CustomerContact GetContact()
        {
            return CustomerContext.Current.GetContactById(GetContactId());
        }

        protected static Guid GetContactId()
        {
            return PrincipalInfo.CurrentPrincipal.GetContactId();
        }

    }
}