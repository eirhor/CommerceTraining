using System;
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
using Mediachase.BusinessFoundation.Data;
using Mediachase.BusinessFoundation.Data.Business;
using Mediachase.Commerce;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Security;
using TransactionScope = Mediachase.Data.Provider.TransactionScope;

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
            var cart = _orderRepository.LoadCart<ICart>(GetContactId(), "Default");

            if (cart == null)
            {
                return RedirectToAction("NoCart", "Cart");
            }

            var isAuthenticated = PrincipalInfo.CurrentPrincipal.Identity.IsAuthenticated;

            var orderAddress = AddAddressToOrder(cart);

            AdjustFirstShipmentInOrder(cart, orderAddress, model.SelectedShipId);

            AddPaymentToOrder(cart, model.SelectedPayId);

            _orderRepository.Save(cart);

            IPurchaseOrder purchaseOrder;
            OrderReference orderReference;
            using (var scope = new TransactionScope())
            {
                _inventoryProcessor.AdjustInventoryOrRemoveLineItem(
                    cart.GetFirstShipment(),
                    OrderStatus.InProgress,
                    (item, issue) => {});

                try
                {
                    cart.ProcessPayments();
                }
                catch (Exception ex)
                {
                    AdjustInventoryOrRemoveLineItem(cart);
                    throw new InvalidOperationException("Payment failed");
                }

                var processedPayments = cart.GetFirstForm()
                    .Payments
                    .Where(p => p.Status.Equals(PaymentStatus.Processed.ToString()))
                    .Sum(p => p.Amount);

                var cartTotal = cart.GetTotal(_orderGroupCalculator).Amount;

                if (processedPayments != cartTotal)
                {
                    AdjustInventoryOrRemoveLineItem(cart);
                    throw new InvalidOperationException("Invalid payment amount");
                }

                AdjustInventoryOrRemoveLineItem(cart, OrderStatus.Completed, false);

                orderReference = _orderRepository.SaveAsPurchaseOrder(cart);
                purchaseOrder = _orderRepository.Load<IPurchaseOrder>(orderReference.OrderGroupId);

                _orderRepository.Delete(cart.OrderLink);

                scope.Complete();
            }

            purchaseOrder.OrderStatus = OrderStatus.InProgress;
            purchaseOrder.GetFirstShipment().OrderShipmentStatus = OrderShipmentStatus.InventoryAssigned;

            var orderNote = _orderGroupFactory.CreateOrderNote(purchaseOrder);
            orderNote.Detail = $"Ordernote text ... {purchaseOrder.GetFirstShipment().ShipmentTrackingNumber}";
            orderNote.LineItemId = purchaseOrder.GetAllLineItems().First().LineItemId;
            orderNote.Title = "OrderNote";
            orderNote.Type = OrderNoteTypes.Custom.ToString();

            purchaseOrder.Notes.Add(orderNote);

            purchaseOrder.ExpirationDate = DateTime.Now.AddMonths(1);

            _orderRepository.Save(purchaseOrder);

            StartPage home = _contentLoader.Get<StartPage>(ContentReference.StartPage);
            ContentReference orderPageReference = home.Settings.orderPage;

            string passingValue = purchaseOrder.OrderNumber;

            return RedirectToAction("Index", new { node = orderPageReference, passedAlong = passingValue });
        }

        private void AdjustInventoryOrRemoveLineItem(ICart cart, OrderStatus orderStatus = OrderStatus.Cancelled, bool saveCart = true)
        {
            _inventoryProcessor.AdjustInventoryOrRemoveLineItem(
                cart.GetFirstShipment(), 
                orderStatus, 
                (item, issue) => {});

            if (saveCart)
            {
                _orderRepository.Save(cart);
            }
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
            var shipmentMethod = ShippingManager.GetShippingMethod(selectedShip).ShippingMethod.First();

            var shipping = cart.GetFirstShipment();

            shipping.ShippingMethodId = shipmentMethod.ShippingMethodId;
            shipping.ShippingAddress = orderAddress;

            var random = new Random();
            shipping.ShipmentTrackingNumber = $"Shipment_{random.Next(1, 99999)}";
        }

        private void AddPaymentToOrder(ICart cart, Guid selectedPaymentGuid)
        {
            var paymentMethod = PaymentManager.GetPaymentMethod(selectedPaymentGuid).PaymentMethod.First();
            var payment = _orderGroupFactory.CreatePayment(cart);

            payment.PaymentMethodId = paymentMethod.PaymentMethodId;
            payment.PaymentType = PaymentType.Other;
            payment.PaymentMethodName = paymentMethod.Name;

            payment.Amount = _orderGroupCalculator.GetTotal(cart).Amount;

            cart.AddPayment(payment);
        }

        private IOrderAddress AddAddressToOrder(ICart cart)
        {
            IOrderAddress shippingAddress = null;

            if (CustomerContext.Current.CurrentContact == null)
            {
                var firstShipment = cart.GetFirstShipment();

                if (firstShipment.ShippingAddress != null)
                {
                    return firstShipment.ShippingAddress;
                }

                shippingAddress = firstShipment.ShippingAddress = GetMockAddress();
            }
            else
            {
                if (CustomerContext.Current.CurrentContact.PreferredShippingAddress == null)
                {
                    var orderAddress = GetMockAddress();
                    var customerAddress = CustomerAddress.CreateInstance();
                    customerAddress.AddressType = CustomerAddressTypeEnum.Shipping;
                    OrderAddress.CopyOrderAddressToCustomerAddress(orderAddress, customerAddress);

                    CustomerContext.Current.CurrentContact.AddContactAddress(customerAddress);
                    CustomerContext.Current.CurrentContact.SaveChanges();

                    CustomerContext.Current.CurrentContact.PreferredShippingAddress = customerAddress;
                    CustomerContext.Current.CurrentContact.SaveChanges();

                    return orderAddress;
                }
                
                shippingAddress = new OrderAddress(CustomerContext.Current.CurrentContact.PreferredShippingAddress);
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
            var shipmentMethods = ShippingManager.GetShippingMethodsByMarket(currentMarket.MarketId.Value, false);
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