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
        DisplayName = "Fashion Node",
        GUID = "C0BF9AAB-D679-42F6-8C05-21B304F37D25",
        MetaClassName = "Fashion_Node")]
    public class FashionNode : NodeContent
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
    }
}