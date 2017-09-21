using System.Collections.Generic;
using EPiServer.Commerce.Order;
using Mediachase.Commerce;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class CartViewModel
    {
        public IEnumerable<ILineItem> LineItems { get; set; }
        public Money SubTotal { get; set; }
        public string WarningMessage { get; set; }
    }
}