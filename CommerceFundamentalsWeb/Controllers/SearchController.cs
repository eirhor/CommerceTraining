using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Website.Search;
using Mediachase.Search;
using Mediachase.Search.Extensions;

namespace CommerceFundamentalsWeb.Controllers
{
    public class SearchController : PageController<SearchPage>
    {
        public IEnumerable<IContent> localContent { get; set; }
        public readonly IContentLoader _contentLoader;
        public readonly ReferenceConverter _referenceConverter;
        public readonly UrlResolver _urlResolver;

        public SearchController(IContentLoader contentLoader, ReferenceConverter referenceConverter, UrlResolver urlResolver)
        {
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
            _urlResolver = urlResolver;
        }

        public ActionResult Index(SearchPage currentPage)
        {
            var model = new SearchPageViewModel
            {
                CurrentPage = currentPage,
            };

            return View(model);
        }

        public ActionResult Search(string keyWord)
        {
            var filterHelper = SearchFilterHelper.Current;

            var searchCriteria = filterHelper.CreateSearchCriteria(keyWord, CatalogEntrySearchCriteria.DefaultSortOrder);
            searchCriteria.StartingRecord = 0;
            searchCriteria.RecordsToRetrieve = 25;
            searchCriteria.Locale = "en";

            var searchResult = filterHelper.SearchEntries(searchCriteria);
            var keyFieldPks = searchResult.GetKeyFieldValues<int>();
            var searchReferences = keyFieldPks.Select(i => _referenceConverter.GetEntryContentLink(i));
            localContent = _contentLoader.GetItems(searchReferences, new LoaderOptions());

            var facetGroupsCount = searchResult.FacetGroups.Count();
            var facetList = searchResult.FacetGroups
                .SelectMany(f => f.Facets
                    .Select(sf => $"{f.Name} {sf.Name} {sf.Count}"));

            var searchResultViewModel = new SearchResultViewModel();

            if (localContent != null && localContent.Any())
            {
                searchResultViewModel.totalHits = new List<string> { "" }; // change
                searchResultViewModel.nodes = localContent.OfType<FashionNode>();
                searchResultViewModel.products = localContent.OfType<ShirtProduct>();
                searchResultViewModel.variants = localContent.OfType<ShirtVariation>();
                searchResultViewModel.allContent = localContent;
                searchResultViewModel.facets = facetList;
            }

            return View(searchResultViewModel);
        }
    }
}