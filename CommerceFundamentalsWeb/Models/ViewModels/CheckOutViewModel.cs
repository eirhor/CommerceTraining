using System;
using System.Collections.Generic;
using CommerceFundamentalsWeb.Models.Pages;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class CheckOutViewModel
    {
        
        public CheckOutPage CurrentPage { get; set; }

        public IEnumerable<PaymentMethodDto.PaymentMethodRow> PaymentMethods { get; set; }

        public IEnumerable<ShippingMethodDto.ShippingMethodRow> ShippingMethods { get; set; }

        public IEnumerable<ShippingRate> ShippingRates { get; set; }

        public Guid SelectedShippingId { get; set; }

        public Guid SelectedPaymentId { get; set; }
        public string Warnings { get; set; }

        public CheckOutViewModel()
        { }

        public CheckOutViewModel(CheckOutPage currentPage)
        {
            CurrentPage = currentPage;
            SelectedPaymentId = Guid.Empty;
            SelectedShippingId = Guid.Empty;
            PaymentMethods = new List<PaymentMethodDto.PaymentMethodRow>();
            ShippingMethods = new List<ShippingMethodDto.ShippingMethodRow>();
        }


    }
}