
using System;
using CommerceFundamentalsWeb.Services;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class PageViewModel<T> where T : PageData
    {

        // ToDo: rootChildren nodes (lab C)
        private readonly INavigationService _navigationService;

        public PageViewModel(T currentPage)
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

            CurrentPage = currentPage;
            Navigation = _navigationService.GetNavigation();
        }

        public T CurrentPage
        {
            get;
            set;
        }

        public NavigationViewModel Navigation { get; }
    }
}