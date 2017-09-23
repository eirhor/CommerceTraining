using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace CommerceFundamentalsWeb.Models.Catalog
{
    [CatalogContentType(DisplayName = "ShirtVariation",
        GUID = "A53DC910-3F38-4EA4-ABC0-63D0F3D965FB",
        Description = "",
        MetaClassName = "Shirt_Variation")]
    public class ShirtVariation : VariationContent
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

        [IncludeInDefaultSearch]
        [Searchable]        
        public virtual string Size { get; set; }
        [IncludeInDefaultSearch]
        [Searchable]
        public virtual string Color { get; set; }
        public virtual bool CanBeMonogrammed { get; set; }
        
        [Searchable]
        [Tokenize]
        [IncludeInDefaultSearch]
        public virtual  string ThematicTag { get; set; }
    }
}