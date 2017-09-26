﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class CatalogViewModel<TCatalogContent, TPageData> : PageViewModel<TPageData> 
        where TCatalogContent : CatalogContentBase
        where TPageData : PageData
    {
        public CatalogViewModel(TCatalogContent currentContent, TPageData currentPage) : base(currentPage)
        {
            CurrentContent = currentContent;
        }

        public TCatalogContent CurrentContent;


    }
}