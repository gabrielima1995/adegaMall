using Domain.DTO;
using Domain.Services;
using Presentation.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class BannersController : Controller
    {
        private readonly BannerService _bannerService;

        public BannersController(BannerService bannerService)
        {
            _bannerService = bannerService;
        }

        public ActionResult Index()
        {
            var list = _bannerService.ListForAdmin();

            var viewModel = new BannerIndexVM
            {
                Items = list.Select(x => new BannerVM(x)).ToList()
            };

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var banner = _bannerService.PrepareNew();

            var viewModel = new BannerCreateEditVM { Banner = new BannerVM(banner) };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(BannerCreateEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _bannerService.Create(viewModel.Banner.ToDomainModel(), GetFileUpload());

                if (result.OK) return RedirectToAction("Index");

                ModelState.AddErrors(result);
            }

            return View(viewModel);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null) return HttpNotFound();

            var banner = _bannerService.GetById(id.Value);

            if (banner == null) return HttpNotFound();

            var viewModel = new BannerCreateEditVM { Banner = new BannerVM(banner) };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(BannerCreateEditVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _bannerService.Update(viewModel.Banner.ToDomainModel(), GetFileUpload());

                if (result.OK) return RedirectToAction("Index");

                ModelState.AddErrors(result);
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid? id)
        {
            if (id == null) return HttpNotFound();

            var banner = _bannerService.GetById(id.Value);

            if (banner == null) return HttpNotFound();

            var result = _bannerService.Delete(banner);
            if (result.OK) return RedirectToAction("Index");

            ModelState.AddErrors(result);
            var viewModel = new BannerDeleteViewModel { Banner = new BannerVM(banner) };

            return View(viewModel);
        }

        private FileUpload GetFileUpload()
        {
            if (Request.Files.Count > 0)
            {
                var imgFile = Request.Files[0];
                var fileUpload = new FileUpload(imgFile.InputStream, imgFile.FileName, imgFile.ContentType);
                return fileUpload;
            }

            return null;
        }
    }
}