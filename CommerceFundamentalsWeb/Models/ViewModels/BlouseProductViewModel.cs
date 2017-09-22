using System.Collections.Generic;
using System.Linq;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class BlouseProductViewModel : CatalogViewModel<BlouseProduct, StartPage>
    {
        public IEnumerable<EntryContentBase> ProductVariations { get; set; }
        public ContentReference CampaignLink { get; set; }

        public BlouseProductViewModel(BlouseProduct currentContent, StartPage currentPage) : base(currentContent, currentPage)
        {
            CampaignLink = currentPage.campaignLink;
        }
    }
}