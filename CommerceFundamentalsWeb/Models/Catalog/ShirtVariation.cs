
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace CommerceFundamentalsWeb.Models.Catalog
{
    [CatalogContentType(
        DisplayName = "Shirt Variation",
        GUID = "1EF5535D-24B3-4194-868A-E8F393DB3A07",
        MetaClassName = "Shirt_Variation")]
    public class ShirtVariation : VariationContent
    {
        [CultureSpecific]
        [IncludeInDefaultSearch]
        [Searchable]
        [Tokenize]
        [Display(
            Name = "Main body",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }

        [IncludeInDefaultSearch]
        [Display(
            Name = "Size",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual string Size { get; set; }

        [IncludeInDefaultSearch]
        [Display(
            Name = "Color",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string Color { get; set; }

        [Display(
            Name = "Can be monogrammed",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        public virtual bool CanBeMonogrammed { get; set; }

        [Searchable]
        [Tokenize]
        [IncludeValuesInSearchResults]
        [Display(
            Name = "Thematic tag",
            GroupName = SystemTabNames.Content,
            Order = 40)]
        public virtual string ThematicTag { get; set; }
    }
}