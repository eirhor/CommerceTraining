using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace CommerceFundamentalsWeb.Models.Catalog
{
    [CatalogContentType(DisplayName = "ShirtProduct",
        GUID = "1B00E01F-2222-4327-AB36-915F56116E89",
        Description = "",
        MetaClassName = "Shirt_Product")]
    public class ShirtProduct : ProductContent
    {
        [CultureSpecific]
        [IncludeInDefaultSearch]
        [Searchable]
        [Tokenize]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }

        public virtual string ClothesType { get; set; }

        public virtual string Brand { get; set; }
    }
}