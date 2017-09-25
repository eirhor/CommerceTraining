using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceFundamentalsWeb.Models.ViewModels;

namespace CommerceFundamentalsWeb.Services
{
    public interface INavigationService
    {
        NavigationViewModel GetNavigation();
    }
}
