using Domain.DTO;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Presentation.Globalization;
using Presentation.Models;
using System;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class ApiTokensController : Controller
    {
        private readonly ApiTokenService _apiTokenService;
        private readonly IApiTokenRepository _apiTokenRepository;
        private readonly ISellerRepository _sellerRepository;

        public ApiTokensController(
            ApiTokenService apiTokenService,
            IApiTokenRepository apiTokenRepository,
            ISellerRepository sellerRepository)
        {
            _apiTokenService = apiTokenService;
            _apiTokenRepository = apiTokenRepository;
            _sellerRepository = sellerRepository;
        }

        public ActionResult Index(string sellerId = null)
        {
            return View(_apiTokenRepository.ListByActive(sellerId));
        }

        public ActionResult Create()
        {
            return View(new ApiTokenCreateVM
            {
                Sellers = _sellerRepository.ListForSelector()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApiTokenCreateVM model)
        {
            if (ModelState.IsValid)
            {
                var validation = _apiTokenService.Generate(model.Name, model.SellerId);

                if (validation.OK)
                {
                    this.SetSuccessMessage(true);
                    return RedirectToAction("Index", new { lang = Request.GetCurrentLang() });
                }

                this.AddErrors(validation);
            }

            model.Sellers = _sellerRepository.ListForSelector();

            return View(model);
        }

        public ActionResult Edit(Guid id)
        {
            var model = _apiTokenRepository.GetForEditing(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApiTokenEditing model)
        {
            if (ModelState.IsValid)
            {
                var validation = _apiTokenService.Update(model);

                if (validation.OK)
                {
                    this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                    return RedirectToAction("Edit", new { id = model.Id, lang = Request.GetCurrentLang() });
                }

                this.AddErrors(validation);
            }

            model.Sellers = _sellerRepository.ListForSelector();

            return View(model);
        }

        [HttpPost]
        public ActionResult Revoke(Guid id)
        {
            var validation = _apiTokenService.Revoke(id);

            if (validation.OK)
            {
                this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
            }
            else
            {
                this.SetErrorMessage(validation.Errors.ToSimpleString(), true);
            }

            return RedirectToAction("Index", new { lang = Request.GetCurrentLang() });
        }
    }
}