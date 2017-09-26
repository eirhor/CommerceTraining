using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class BlouseProductViewModel : CatalogViewModel<BlouseProduct, StartPage>
    {
        public BlouseProductViewModel(BlouseProduct currentContent, StartPage currentPage) : base(currentContent, currentPage)
        {
        }

        public IEnumerable<EntryContentBase> Variations { get; set; }
        public ContentReference CampaignLink { get; set; }
    }
}