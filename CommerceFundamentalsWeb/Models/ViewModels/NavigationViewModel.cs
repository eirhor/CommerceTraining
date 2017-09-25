using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class NavigationViewModel
    { 
        public IEnumerable<INavigationItem> CmsItems { get; set; }

        public IEnumerable<IContent> CommerceItems { get; set; }
    }
}