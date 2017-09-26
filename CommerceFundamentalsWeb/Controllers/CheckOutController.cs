﻿using System;
using System.Collections.Generic;
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
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.Commerce;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Security;
// for the extension-method

namespace CommerceFundamentalsWeb.Controllers
{
    public class CheckOutController : PageController<CheckOutPage>
    {

        private const string DefaultCart = "Default";

        private readonly IContentLoader _contentLoader; // To get the StartPage --> Settings-links
        private readonly ICurrentMarket _currentMarket; // not in fund... yet
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IPromotionEngine _promotionEngine;
        private readonly IOrderGroupCalculator _orderGroupCalculator;
        private readonly ILineItemCalculator _lineItemCalculator;
        private readonly IInventoryProcessor _inventoryProcessor;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly IPlacedPriceProcessor _placedPriceProcessor;

        public CheckOutController(IContentLoader contentLoader
    , ICurrentMarket currentMarket
    , IOrderRepository orderRepository
    , IPlacedPriceProcessor placedPriceProcessor
    , IInventoryProcessor inventoryProcessor
    , ILineItemValidator lineItemValidator
    , IOrderGroupCalculator orderGroupCalculator
    , ILineItemCalculator lineItemCalculator
    , IOrderGroupFactory orderGroupFactory
    , IPaymentProcessor paymentProcessor
    , IPromotionEngine promotionEngine)
        {
            _contentLoader = contentLoader;
            _currentMarket = currentMarket;
            _orderRepository = orderRepository;
            _orderGroupCalculator = orderGroupCalculator;
            _orderGroupFactory = orderGroupFactory;
            _paymentProcessor = paymentProcessor;
            _promotionEngine = promotionEngine;
            _lineItemCalculator = lineItemCalculator;
            _inventoryProcessor = inventoryProcessor;
            _lineItemValidator = lineItemValidator;
            _placedPriceProcessor = placedPriceProcessor;
        }

        // ToDo: in the first exercise (E1) Ship & Pay
        public ActionResult Index(CheckOutPage currentPage)
        {
            var customerId = PrincipalInfo.CurrentPrincipal.GetContactId();
            var cart = _orderRepository.LoadCart<ICart>(customerId, "Default");

            if (cart == null)
            {
                return RedirectToAction("NoCart", "Cart");
            }

            var model = new CheckOutViewModel(currentPage)
            {
                // ToDo: Exercise (E1) - get shipments & payments
                ShippingRates = GetShippingRates(),
                ShipmentMethods = GetShipmentMethods(),
                PaymentMethods = GetPaymentMethods()
            };

            return View(model);
        }

        //Exercise (E2) Do CheckOut
        public ActionResult CheckOut(CheckOutViewModel model)
        {
            var customerId = PrincipalInfo.CurrentPrincipal.GetContactId();
            var cart = _orderRepository.LoadCart<ICart>(customerId, "Default");

            if (cart == null)
            {
                return RedirectToAction("NoCart", "Cart");
            }

            // ToDo: Add an OrderAddress
            CustomerAddress customerAddress;
            if (CustomerContext.Current.CurrentContact == null)
            {
                customerAddress = CustomerAddress.CreateInstance();
                customerAddress.AddressType = CustomerAddressTypeEnum.Shipping;
                customerAddress.Name = "Default";
                customerAddress.FirstName = "Eirik";
                customerAddress.LastName = "Horvath";
                customerAddress.CountryCode = "47";
                customerAddress.CountryName = "Norway";
                customerAddress.RegionName = "Buskerud";
                customerAddress.RegionCode = "0123";
                customerAddress.DaytimePhoneNumber = "90048775";
                customerAddress.Email = "eirik@geta.no";
            }
            else
            {
                
            }

            // ToDo: Define/update Shipping


            // ToDo: Add a Payment to the Order 


            // ToDo: Add a transaction scope and convert the cart to PO


            // ToDo: Housekeeping (Statuses for Shipping and PO, OrderNotes and save the order)


            // Final steps, navigate to the order confirmation page
            StartPage home = _contentLoader.Get<StartPage>(ContentReference.StartPage);
            ContentReference orderPageReference = home.Settings.orderPage;

            // the below is a dummy, change to "PO".OrderNumber when done
            string passingValue = String.Empty;

            return RedirectToAction("Index", new { node = orderPageReference, passedAlong = passingValue });
        }

        private OrderAddress GetMockAddress()
        {
            return new OrderAddress
            {
                City = "Hokksund",
                CountryCode = "47",
                CountryName = "Norway",
                Created = DateTime.Now,
                DaytimePhoneNumber = "90048775",
                CreatorId = PrincipalInfo.CurrentPrincipal.GetContactId().ToString(),
                Email = "eirik@geta.no",
                EveningPhoneNumber = "90048775",
                PostalCode = "3300",
                State = "Buskerud",
                Line1 = "Pålsplass 3B",
                Name = "Home",
                RegionCode = "0123",
                RegionName = "wat",
                FirstName = "Eirik",
                LastName = "Horvath"
            };
        }

        // Prewritten 
        private string ValidateCart(ICart cart)
        {
            var validationMessages = string.Empty;

            cart.ValidateOrRemoveLineItems((item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _lineItemValidator);

            cart.UpdatePlacedPriceOrRemoveLineItems(GetContact(), (item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _placedPriceProcessor);

            cart.UpdateInventoryOrRemoveLineItems((item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _inventoryProcessor);

            return validationMessages; 
        }

        private static string CreateValidationMessages(ILineItem item, ValidationIssue issue)
        {
            return string.Format("Line item with code {0} had the validation issue {1}.", item.Code, issue);
        }

        private void AdjustFirstShipmentInOrder(ICart cart, IOrderAddress orderAddress, Guid selectedShip)
        {
            
        }

        private void AddPaymentToOrder(ICart cart, Guid selectedPaymentGuid)
        { }

        private IOrderAddress AddAddressToOrder(ICart cart)
        {
            IOrderAddress shippingAddress = null;

            if (CustomerContext.Current.CurrentContact == null)
            {
                var firstShipment = cart.GetFirstShipment();

                firstShipment.ShippingAddress = GetMockAddress();

                cart.AddShipment(firstShipment);
            }
            else
            {

            }

            return shippingAddress;
        }

        private static CustomerContact GetContact()
        {
            return CustomerContext.Current.GetContactById(GetContactId());
        }

        private static Guid GetContactId()
        {
            return PrincipalInfo.CurrentPrincipal.GetContactId();
        }

        private IEnumerable<PaymentMethodDto.PaymentMethodRow> GetPaymentMethods()
        {
            var currentMarket = _currentMarket.GetCurrentMarket();
            var paymentMethods = PaymentManager.GetPaymentMethodsByMarket(currentMarket.MarketName);
            return paymentMethods.PaymentMethod;
        }

        private IEnumerable<ShippingMethodDto.ShippingMethodRow> GetShipmentMethods()
        {
            var currentMarket = _currentMarket.GetCurrentMarket();
            var shipmentMethods = ShippingManager.GetShippingMethodsByMarket(currentMarket.MarketName, false);
            return shipmentMethods.ShippingMethod;
        }

        private IEnumerable<ShippingRate> GetShippingRates()
        {
            var shipmentMethods = GetShipmentMethods();
            return shipmentMethods.Select(s => new ShippingRate(
                s.ShippingMethodId, 
                s.Name, 
                new Money(s.BasePrice, s.Currency)));
        }
    }
}