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
        DisplayName = "Shirt Product",
        GUID = "9759DCA0-0B93-4CEB-82B8-9D4B4C38FF88",
        MetaClassName = "Shirt_Product")]
    public class ShirtProduct : ProductContent
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

        [Display(
            Name = "Clothes type",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual string ClothesType { get; set; }

        [Display(
            Name = "Brand",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string Brand { get; set; }
    }
}