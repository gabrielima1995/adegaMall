using Domain.Services;
using Presentation.Models;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class EcommerceHomeController : Controller
    {
        private readonly EcommerceHomeService _ecommerceHomeService;

        public EcommerceHomeController(EcommerceHomeService ecommerceHomeService)
        {
            _ecommerceHomeService = ecommerceHomeService;
        }

        public PartialViewResult Products()
        {
            var products = _ecommerceHomeService.GetHomeProducts();
            var viewModel = new EcommerceHomeProductsVM(products);
            return PartialView("_Products", viewModel);
        }

        public PartialViewResult Search()
        {
            return PartialView("_Search");
        }
    }
}