using Domain.Services;
using Presentation.Models;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class BannersController : Controller
    {
        private readonly BannerService _bannerService;

        public BannersController(BannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public PartialViewResult Banners()
        {
            var banners = _bannerService.ListActive();
            var viewModel = new BannerListVM(banners);
            return PartialView("_Banners", viewModel);
        }
    }
}