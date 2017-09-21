using System.Collections.Generic;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;

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
            // ToDo: SearchHelper and Criteria 


            // ToDo: Search 


            // ToDo: Facets


            // ToDo: As a last step - un-comment and fill up the ViewModel
            var searchResultViewModel = new SearchResultViewModel();
            /*
            searchResultViewModel.totalHits = new List<string> { "" }; // change
            searchResultViewModel.nodes = localContent.OfType<FashionNode>();
            searchResultViewModel.products = localContent.OfType<FashionProduct>();
            searchResultViewModel.variants = localContent.OfType<FashionVariation>();
            searchResultViewModel.allContent = localContent;
            searchResultViewModel.facets = facetList;
            */

            return View(searchResultViewModel);
        }
    }
}