using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceFundamentalsWeb.Models;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;

namespace CommerceFundamentalsWeb.Services
{
    [ServiceConfiguration(typeof(INavigationService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class NavigationService : INavigationService
    {
        private readonly IContentRepository _contentRepository;
        private readonly IContentLoader _contentLoader;
        private readonly StartPage _startPage;

        public NavigationService(IContentRepository contentRepository, IContentLoader contentLoader)
        {
            if (contentRepository == null) throw new ArgumentNullException(nameof(contentRepository));
            if (contentLoader == null) throw new ArgumentNullException(nameof(contentLoader));

            _contentRepository = contentRepository;
            _contentLoader = contentLoader;
            _startPage = contentLoader.Get<StartPage>(ContentReference.StartPage);
        }

        public NavigationViewModel GetNavigation()
        {
            var childPages = _contentRepository.GetChildren<INavigationItem>(_startPage.ContentLink);
            var commercePages = FilterForVisitor.Filter(_contentRepository.GetChildren<IContent>(_startPage.Settings.topCategory));

            return new NavigationViewModel
            {
                CmsItems = childPages,
                CommerceItems = commercePages
            };
        }
    }
}