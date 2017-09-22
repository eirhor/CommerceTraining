using System.Linq;
using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

//using CommerceTraining.SupportingClasses;

namespace CommerceFundamentalsWeb.Controllers
{
    public class StartPageController : PageController<StartPage>
    {
       
        public readonly IContentLoader _contentLoader;
        public readonly UrlResolver _urlResolver;

        ContentReference topCategory { get; set; } // used for listing of nodes at the start-page

        public StartPageController(
            IContentLoader contentLoader
            , UrlResolver urlResolver)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;

            topCategory = contentLoader.Get<StartPage>(ContentReference.StartPage).Settings.topCategory;
        }

        public string GetUrl(ContentReference contentReference)
        {
            return _urlResolver.GetUrl(contentReference);
        }

        public ActionResult Index(StartPage currentPage)
        {
            var model = new PageViewModel<StartPage>(currentPage)
            {
                MainBodyStartPage = currentPage.MainBody,
                myPageChildren = _contentLoader.GetChildren<IContent>(currentPage.ContentLink),
                
                topLevelCategories = _contentLoader.GetChildren<CatalogContentBase>(topCategory).OfType<NodeContent>(),
            };

            return View(model);
        }
    }
}