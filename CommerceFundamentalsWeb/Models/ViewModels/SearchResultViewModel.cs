using System.Collections.Generic;
using CommerceFundamentalsWeb.Models.Catalog;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class SearchResultViewModel
    {
        // no paging settings or other luxery
        // un-comment when the catalog models exist

        public IEnumerable<string> TotalHits { get; set; }
        public IEnumerable<FashionNode> Nodes { get; set; }
        public IEnumerable<ShirtProduct> Products { get; set; }
        public IEnumerable<ShirtVariation> Variants { get; set; }
        public IEnumerable<IContent> AllContent { get; set; }
        public IEnumerable<string> Facets { get; set; }

    }
}