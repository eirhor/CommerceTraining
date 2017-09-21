using CommerceFundamentalsWeb.Models.Pages;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class CheckOutViewModel
    {
        
        public CheckOutPage CurrentPage { get; set; }

        // Lab E1 - create properties below


        
        public CheckOutViewModel()
        { }

        public CheckOutViewModel(CheckOutPage currentPage)
        {
            CurrentPage = currentPage;
        }


    }
}