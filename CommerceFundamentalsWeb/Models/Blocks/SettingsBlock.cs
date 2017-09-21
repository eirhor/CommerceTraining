using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace CommerceFundamentalsWeb.Models.Blocks
{
    [ContentType(DisplayName = "SettingsBlock", GUID = "fdfd33be-91ca-4366-a3ac-ea126c66f0e7", Description = "")]
    public class SettingsBlock : BlockData
    {
        [UIHint(UIHint.CatalogContent)]
        public virtual ContentReference topCategory { get; set; }

        public virtual ContentReference cartPage { get; set; }

        public virtual ContentReference checkoutPage { get; set; }

        public virtual ContentReference orderPage { get; set; }

        public virtual ContentReference catalogStartPageLink { get; set; }

        //public virtual bool InitializedOrNot { get; set; } wait with this
    }
}