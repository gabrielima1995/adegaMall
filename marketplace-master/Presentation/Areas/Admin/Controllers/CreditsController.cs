using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using Presentation.Areas.Admin.Models;
using System;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class CreditsController : Controller
    {
        private readonly ICreditsPackRepository _creditsPackRepository;

        public CreditsController(ICreditsPackRepository creditsPackRepository)
        {
            _creditsPackRepository = creditsPackRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var list = _creditsPackRepository.List();

            return View("Index", list);
        }

        [HttpPost]
        public ActionResult CreatePack(CreditsPackViewModel model)
        {
            if (model.Price <= 0 || model.CreditsAmount <= 0)
            {
                ModelState.AddModelError("Error", "Preço e quantidade devem ser maiores que zero");
            }

            if (ModelState.IsValid)
            {
                var newPack = SimpleMapper.Map<CreditsPackViewModel, CreditsPack>(model);
                _creditsPackRepository.Add(newPack, true);
                return RedirectToAction("Index");
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult DeletePack(string packId)
        {
            var pack = _creditsPackRepository.GetById(new Guid(packId));
            if (pack != null)
            {
                _creditsPackRepository.Delete(pack, true);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdatePack(CreditsPackViewModel model)
        {
            if (model.Id == null || model.Id == Guid.Empty)
                ModelState.AddModelError("Id", "Invalid pack Id");

            if (ModelState.IsValid)
            {
                var pack = _creditsPackRepository.GetById(model.Id);
                pack.Name = model.Name;
                pack.Name_EN = model.Name_EN;
                pack.Price = model.Price;
                pack.Description = model.Description;
                pack.Description_EN = model.Description_EN;
                pack.CreditsAmount = model.CreditsAmount;
                pack.ValidityDays = model.ValidityDays;

                _creditsPackRepository.Update(pack, true);

                return RedirectToAction("Index");
            }

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult UpdatePack(string packId)
        {
            var pack = _creditsPackRepository.GetById(new Guid(packId));
            var packVM = SimpleMapper.Map<CreditsPack, CreditsPackViewModel>(pack);
            packVM.Id = pack.Id;

            return PartialView("_UpdatePackForm", packVM);
        }
    }
}