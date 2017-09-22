using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class ShirtVariationViewModel
    {        
        public string PriceString { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool CanBeMonogrammed { get; set; }
        public XhtmlString MainBody { get; set; }

    }
}