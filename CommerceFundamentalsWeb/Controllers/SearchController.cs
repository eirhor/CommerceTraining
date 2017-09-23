using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Catalog;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Globalization;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
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
            // ToDo: SearchHelper and Criteria 
            SearchFilterHelper searchHelper = SearchFilterHelper.Current; // the easy way

            CatalogEntrySearchCriteria criteria = searchHelper.CreateSearchCriteria(keyWord
                , CatalogEntrySearchCriteria.DefaultSortOrder);

            criteria.RecordsToRetrieve = 25;
            criteria.StartingRecord = 0;
            //criteria.Locale = "en"; // needed
            criteria.Locale = ContentLanguage.PreferredCulture.Name;


            int count = 0; // "Out"
            bool cacheResult = true;
            TimeSpan timeSpan = new TimeSpan(0, 10, 0);

            // ToDo: Search 
            // One way of "doing it" ... retrieve it like ISearchResults (preferred, most certainly)
            ISearchResults searchResult = searchHelper.SearchEntries(criteria);
            ISearchDocument aDoc = searchResult.Documents.FirstOrDefault();
            int[] ints = searchResult.GetKeyFieldValues<int>();

            // ECF style Entries, old-school & legacy, not recommended... 
            // ...work with DTOs if not using the ContentModel
            Entries entries = CatalogContext.Current.GetCatalogEntries(ints // Note "ints"
                , new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));

            // Same thing ECF, old-style, not recommended... if not absolutely needed...
            // Use the helper and get the entries direct 
            // If entries are needed ... like for calculating discounts with StoreHelper()
            Entries entriesDirect = searchHelper.SearchEntries(criteria, out count // Note the different return-types ... akward!
                , new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo)
                , cacheResult, new TimeSpan());

            // CMS style (better)... using ReferenceConverter and ContentLoader 
            List<ContentReference> refs = new List<ContentReference>();
            ints.ToList().ForEach(i => refs.Add(_referenceConverter.GetContentLink(i, 0)));

            // LoaderOptions() is new in CMS 8
            // ILanguageSelector selector = ServiceLocator.Current.GetInstance<ILanguageSelector>(); // obsolete
            localContent = _contentLoader.GetItems(refs, new LoaderOptions()); // use this in CMS 8+

            // ToDo: Facets
            List<string> facetList = new List<string>();

            int facetGroups = searchResult.FacetGroups.Count();

            foreach (ISearchFacetGroup item in searchResult.FacetGroups)
            {
                foreach (var item2 in item.Facets)
                {
                    facetList.Add(String.Format("{0} {1} ({2})", item.Name, item2.Name, item2.Count));
                }
            }

            // ToDo: As a last step - un-comment and fill up the ViewModel
            var searchResultViewModel = new SearchResultViewModel();

            searchResultViewModel.TotalHits = new List<string> { "" }; // change
            searchResultViewModel.Nodes = localContent.OfType<FashionNode>();
            searchResultViewModel.Products = localContent.OfType<ShirtProduct>();
            searchResultViewModel.Variants = localContent.OfType<ShirtVariation>();
            searchResultViewModel.AllContent = localContent;
            searchResultViewModel.Facets = facetList;


            return View(searchResultViewModel);
        }
    }
}