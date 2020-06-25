using System.Linq;
using Domain.Services;
using Presentation.Models;
using System.Web.Mvc;
using Domain.Entities.StoreModule;
using Framework;

namespace Presentation.Controllers
{
    public class EcommerceHomeController : Controller
    {
        private readonly EcommerceHomeService _ecommerceHomeService;
        private readonly IRepository<ProductVariant> _productVariantRepository;

        public EcommerceHomeController(
            EcommerceHomeService ecommerceHomeService,
            IRepository<ProductVariant> productVariantRepository)
        {
            _ecommerceHomeService = ecommerceHomeService;
            _productVariantRepository = productVariantRepository;
        }

        public PartialViewResult Products()
        {
            var products = _ecommerceHomeService.GetHomeProducts();
            var viewModel = new EcommerceHomeProductsVM(products);
            return PartialView("_Products", viewModel);
        }

        public PartialViewResult Paises()
        {
            var items = _productVariantRepository
                .Query()
                .Where(i => i.Type.Slug == "pais" && !i.Deleted)
                .OrderBy(i => i.Name)
                .ToList();

            return PartialView("_Paises", items);
        }

        public PartialViewResult Search()
        {
            return PartialView("_Search");
        }
    }
}