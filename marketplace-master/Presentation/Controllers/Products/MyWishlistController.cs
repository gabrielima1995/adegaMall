using Application.Config;
using System.Web.Mvc;
using V2.Application.Modules.ProductWishModule;

namespace Presentation.Controllers.Products
{
    [AuthorizeBuyer]
    public class MyWishlistController : BaseController
    {
        private const string ViewPathFmt = "~/Views/Products/MyWishlist/{0}.cshtml";

        private MaaSConfig Config => MaaSConfig.Current;

        private readonly ProductWishAppService _appService;

        public MyWishlistController(ProductWishAppService appService)
        {
            _appService = appService;
        }

        public ActionResult Index()
        {
            if (!Config.Product.Wishlist.Enabled) return HttpNotFound();

            var buyerId = GetPersonId();
            var model = _appService.GetWishList(buyerId, 1, 16);
            model.NextPageUrl = BuildNextPageUrl(1, model.TotalItems);

            return View(string.Format(ViewPathFmt, "Index"), model);
        }

        public ActionResult GetWishlistPaged(int page, int pageSize = 16)
        {
            if (!Config.Product.Wishlist.Enabled) return HttpNotFound();

            var buyerId = GetPersonId();
            var model = _appService.GetWishList(buyerId, page, pageSize);
            model.NextPageUrl = BuildNextPageUrl(page, model.TotalItems, pageSize);

            return PartialView(string.Format(ViewPathFmt, "_Products"), model);
        }

        private string BuildNextPageUrl(int currentPage, int totalItems, int pageSize = 16)
        {
            var itemsShown = currentPage * pageSize;
            var hasNextPage = itemsShown < totalItems;

            if (!hasNextPage) return null;

            var baseUrl = Url.ActionLocalized("GetWishlistPaged", "MyWishlist", new { page = currentPage + 1, pageSize });
            return baseUrl;
        }
    }
}