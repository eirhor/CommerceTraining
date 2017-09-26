using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceFundamentalsWeb.SupportingClasses;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;

namespace CommerceFundamentalsWeb.Services
{
    public interface ICommerceService
    {
        string GetDefaultAsset(IAssetContainer container);
        string GetNamedAsset(IAssetContainer container, string groups);
        string GetUrl(ContentReference contentLink);
        IEnumerable<NameAndUrls> GetNodes(ContentReference contentLink);
        IEnumerable<NameAndUrls> GetEntries(ContentReference contentLink);
    }
}
