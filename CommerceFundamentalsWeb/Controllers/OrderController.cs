using System.Web.Mvc;
using CommerceFundamentalsWeb.Models.Pages;
using CommerceFundamentalsWeb.Models.ViewModels;
using EPiServer.Web.Mvc;

namespace CommerceFundamentalsWeb.Controllers
{
    public class OrderController : PageController<OrderPage>
    {
        public ActionResult Index(OrderPage currentPage, string passedAlong)
        {
            var model = new OrderViewModel()
            {
                TrackingNumber = passedAlong
            };

            return View(model);
        }
    }
}