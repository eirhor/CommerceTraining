using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;

namespace CommerceFundamentalsWeb.Models.Catalog
{
    [CatalogContentType(
        DisplayName = "Blouse Product",
        GUID = "BD70D68E-354B-4A69-AFB2-2AF56F5B993C",
        MetaClassName = "Blouse_Product")]
    public class BlouseProduct : ProductContent
    {

    }
}