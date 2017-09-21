using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Catalog;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.ServiceLocation;
using CommerceTraining.Models.Pages;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Web.Routing;
using EPiServer.Commerce.Catalog;
using Mediachase.Commerce.Catalog;
using System.Globalization;
using EPiServer.DataAccess;
using CommerceTraining.Models.ViewModels;

namespace CommerceTraining.Controllers
{
    public class BlouseProductController : MyControllerBase<BlouseProduct> 
    {

        public BlouseProductController(
        IContentLoader contentLoader
        , UrlResolver urlResolver
        , AssetUrlResolver assetUrlResolver
        , ThumbnailUrlResolver thumbnailUrlResolver 
        , AssetUrlConventions assetUrlConvensions
        )
            : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        { }

        public ActionResult Index()
        {

            return null; // dummy, remove when done
            
        }

        // For the optional C7 exercise
        public void CreateWithCode()
        {
            // ToDo: Use with Blouses in "Fund"... 
            string nodeName = "myNode";
            string productName = "myProduct";
            string skuName = "mySku";
        
        }
    }
}