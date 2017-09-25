using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace CommerceFundamentalsWeb.Controllers.Catalog
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class CatalogControllerBase<T> : ContentController<T> where T : CatalogContentBase
    {
        public T CurrentContent
        {
            get
            {
                var pageRouteHelper = ServiceLocator.Current.GetInstance<IPageRouteHelper>();
                return pageRouteHelper.Page as T;
            }
        }
    }
}