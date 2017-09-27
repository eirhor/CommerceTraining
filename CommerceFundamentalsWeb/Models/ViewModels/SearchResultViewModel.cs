using System.Collections.Generic;
using CommerceFundamentalsWeb.Models.Catalog;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class SearchResultViewModel
    {
        // no paging settings or other luxery
        // un-comment when the catalog models exist

        public IEnumerable<string> totalHits { get; set; }
        public IEnumerable<FashionNode> nodes { get; set; }
        public IEnumerable<ShirtProduct> products { get; set; }
        public IEnumerable<ShirtVariation> variants { get; set; }
        public IEnumerable<IContent> allContent { get; set; }
        public IEnumerable<string> facets { get; set; }

    }
}