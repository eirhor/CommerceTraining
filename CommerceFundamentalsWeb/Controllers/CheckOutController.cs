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
using Mediachase.Commerce;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Security;
using Mediachase.Data.Provider;

// for the extension-method

namespace CommerceFundamentalsWeb.Controllers
{
    public class CheckOutController : PageController<CheckOutPage>
    {

        private const string DefaultCart = "Default";

        private readonly IContentLoader _contentLoader; 
        private readonly ICurrentMarket _currentMarket; 
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
            // Try to load the cart  

            var model = new CheckOutViewModel(currentPage)
            {
                PaymentMethods = GetPaymentMethods(),
                ShippingMethods = GetShippingMethods(),
                ShippingRates = GetShippingRates()

            };

            return View(model);
        }

        private IEnumerable<ShippingRate> GetShippingRates()
        {
            var methods = GetShippingMethods();
            return methods.Select(x => new ShippingRate(x.ShippingMethodId, x.DisplayName, new Money(x.BasePrice, x.Currency)));            
        }

        private IEnumerable<ShippingMethodDto.ShippingMethodRow> GetShippingMethods()
        {
            ShippingMethodDto shippingMethodDto =
                ShippingManager.GetShippingMethodsByMarket(_currentMarket.GetCurrentMarket().MarketId.Value, false);

            return shippingMethodDto.ShippingMethod.Rows.OfType<ShippingMethodDto.ShippingMethodRow>();
        }

        private IEnumerable<PaymentMethodDto.PaymentMethodRow> GetPaymentMethods()
        {
            PaymentMethodDto paymentDto = PaymentManager.GetPaymentMethodsByMarket(_currentMarket.GetCurrentMarket().MarketId.Value);
            return paymentDto.PaymentMethod.Rows.OfType<PaymentMethodDto.PaymentMethodRow>();
        }
        





        //Exercise (E2) Do CheckOut
        public ActionResult CheckOut(CheckOutViewModel model)
        {
            var cart = _orderRepository.LoadCart<ICart>(GetContactId(), DefaultCart);

            var isAutenticated = PrincipalInfo.CurrentPrincipal.Identity.IsAuthenticated;

            var orderAddress = AddAddressToOrder(cart);

            AdjustFirstShipmentInOrder(cart,orderAddress,model.SelectedShippingId);
            
            AddPaymentToOrder(cart,model.SelectedPaymentId);

            var validationMessages =  ValidateCart(cart);

            if (!string.IsNullOrEmpty(validationMessages))
            {
                model.Warnings = validationMessages;
            }

            // Adding this for test
            var cartReference = _orderRepository.Save(cart);

            IPurchaseOrder purchaseOrder;
            OrderReference orderReference;
            using (var scope = new TransactionScope())
            {
                var validationIssues = new Dictionary<ILineItem, ValidationIssue>();

                _inventoryProcessor.AdjustInventoryOrRemoveLineItem(cart.GetFirstShipment()
                    , OrderStatus.InProgress, (item, issue) => validationIssues.Add(item, issue));

                if (validationIssues.Count >= 1)
                {
                    throw new Exception("Not possible right now");
                }

                try
                {
                    cart.ProcessPayments();
                }
                catch (Exception e)
                {
                    foreach (
                        var p in cart.GetFirstForm().Payments.Where(p => p.Status != PaymentStatus.Processed.ToString()))
                    {
                        cart
                    }
                    _orderRepository.Save(cart);
                    throw new Exception("Payment failed");
                }

                var totalProcessedAmount = cart.GetFirstForm().Payments.Where
                    (x => x.Status.Equals(PaymentStatus.Processed.ToString())).Sum(x => x.Amount);

                var cartTotal = cart.GetTotal();

                if (totalProcessedAmount != cart.GetTotal(_orderGroupCalculator).Amount)
                {                   
                    _inventoryProcessor.AdjustInventoryOrRemoveLineItem(cart.GetFirstShipment()
                        , OrderStatus.Cancelled, (item, issue) => validationIssues.Add(item, issue));

                    
                    throw new InvalidOperationException("Wrong amount"); // maybe change approach
                }


                // ...could do this here
                cart.GetFirstShipment().OrderShipmentStatus = OrderShipmentStatus.InventoryAssigned;

                // decrement inventory and let it go
                _inventoryProcessor.AdjustInventoryOrRemoveLineItem(cart.GetFirstShipment()
                    , OrderStatus.Completed, (item, issue) => validationIssues.Add(item, issue));
                
                orderReference = _orderRepository.SaveAsPurchaseOrder(cart);
                purchaseOrder = _orderRepository.Load<IPurchaseOrder>(orderReference.OrderGroupId);
                _orderRepository.Delete(cart.OrderLink);

                scope.Complete();
            }

            var note = _orderGroupFactory.CreateOrderNote(purchaseOrder);
            note.CustomerId = GetContactId();
            note.Title = "Order Created";
            note.Detail = "Order Created by Commerce Training Fundamentals";
            note.Type = OrderNoteTypes.Custom.ToString();

            purchaseOrder.Notes.Add(note);

            _orderRepository.Save(purchaseOrder);

            // Final steps, navigate to the order confirmation page
            StartPage home = _contentLoader.Get<StartPage>(ContentReference.StartPage);
            ContentReference orderPageReference = home.Settings.orderPage;

            // the below is a dummy, change to "PO".OrderNumber when done
            string passingValue = purchaseOrder.OrderNumber;

            return RedirectToAction("Index", new { node = orderPageReference, passedAlong = passingValue });
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
            // Need to set the guid (name is good to have too) of some "real shipmentment in the DB"
            // RoCe - this step is not needed, actually - code and lab-steps can be updated
            // We'll do it to show how it works
            var shippingMethod = ShippingManager.GetShippingMethod(selectedShip).ShippingMethod.First();

            IShipment theShip = cart.GetFirstShipment(); // ...as we get one "for free"

            // Need the choice of shipment from DropDowns
            theShip.ShippingMethodId = shippingMethod.ShippingMethodId;
            //theShip.ShippingMethodName = "TucTuc";

            theShip.ShippingAddress = orderAddress;

            #region Hard coded and cheating just to show

            
            
            Money cost00 = theShip.GetShippingCost(_currentMarket.GetCurrentMarket(), new Currency("USD"));
            Money cost000 = theShip.GetShippingCost(_currentMarket.GetCurrentMarket(), cart.Currency);
            #endregion

            Money cost0 = theShip.GetShippingCost(
                _currentMarket.GetCurrentMarket()
                , _currentMarket.GetCurrentMarket().DefaultCurrency); // to make it easy

            // done by the "default calculator"
            Money cost1 = theShip.GetShippingItemsTotal(_currentMarket.GetCurrentMarket().DefaultCurrency);

            theShip.ShipmentTrackingNumber = "ABC123";
        }


        private void AddPaymentToOrder(ICart cart, Guid selectedPaymentGuid)
        {
            if (cart.GetFirstForm().Payments.Any())
            {
                // should maybe clean up in the cart here
            }

            var selectedPaymentMethod =
                PaymentManager.GetPaymentMethod(selectedPaymentGuid).PaymentMethod.First();

            var payment = _orderGroupFactory.CreatePayment(cart);

            payment.PaymentMethodId = selectedPaymentMethod.PaymentMethodId;
            payment.PaymentType = PaymentType.Other;
            payment.PaymentMethodName = selectedPaymentMethod.Name; // check if str "soliciting" still works

            // ...we also have - cart.GetTotal(_orderGroupCalculator)
            payment.Amount = _orderGroupCalculator.GetTotal(cart).Amount; // need a debug here

            cart.AddPayment(payment);
            // could add payment.BillingAddress = theAddress ... if we had it here
        }


        private IOrderAddress AddAddressToOrder(ICart cart)
        {
            IOrderAddress shippingAddress;

            if (CustomerContext.Current.CurrentContact == null)
            {
                // Anonymous... one way of "doing it"... for example, if no other address exist
                var shipment = cart.GetFirstShipment(); // ... moved to shipment - prev. = .OrderAddresses.Add(

                if (shipment.ShippingAddress != null)
                {
                    //return false/true; // Should clean up? ... did earlier for ship & pay
                }


                //Shipment oldShip = shipment as Shipment;
                shippingAddress = shipment.ShippingAddress = // should be an else here... below?
                    new OrderAddress
                    {
                        CountryCode = "USA",
                        CountryName = "United States",
                        Name = "SomeCustomerAddressName",
                        DaytimePhoneNumber = "123456",
                        FirstName = "John",
                        LastName = "Smith",
                        Email = "John@company.com",
                    };

            }
            else
            {
                // Logged in
                if (CustomerContext.Current.CurrentContact.PreferredShippingAddress == null)
                {
                    // no pref. address set... so we set one for the contact
                    CustomerAddress newCustAddress =
                    CustomerAddress.CreateForApplication();
                    newCustAddress.AddressType = CustomerAddressTypeEnum.Shipping | CustomerAddressTypeEnum.Public; // mandatory
                    newCustAddress.ContactId = CustomerContext.Current.CurrentContact.PrimaryKeyId;
                    newCustAddress.CountryCode = "SWE";
                    newCustAddress.CountryName = "Sweden";
                    newCustAddress.Name = "new customer address"; // mandatory
                    newCustAddress.DaytimePhoneNumber = "123456";
                    newCustAddress.FirstName = CustomerContext.Current.CurrentContact.FirstName;
                    newCustAddress.LastName = CustomerContext.Current.CurrentContact.LastName;
                    newCustAddress.Email = "GuitarWorld@Thule.com";

                    // note: Line1 & City is what is shown in CM at a few places... not the Name
                    CustomerContext.Current.CurrentContact.AddContactAddress(newCustAddress);
                    CustomerContext.Current.CurrentContact.SaveChanges();

                    // ... needs to be in this order
                    CustomerContext.Current.CurrentContact.PreferredShippingAddress = newCustAddress;
                    CustomerContext.Current.CurrentContact.SaveChanges(); // need this ...again 

                    // then, for the cart
                    //.Cart.OrderAddresses.Add(new OrderAddress(newCustAddress)); - OLD
                    shippingAddress = new OrderAddress(newCustAddress); // - NEW
                }
                else
                {
                    // 3:rd vay there is a preferred address set (and, a fourth alternative exists... do later )
                    shippingAddress = new OrderAddress(
                        CustomerContext.Current.CurrentContact.PreferredShippingAddress);
                }
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
    }
}