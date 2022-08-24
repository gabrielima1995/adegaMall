using Domain.Modules.Integration;
using Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class IntegrationParameterFromGoogleController : BaseController
    {
        private readonly IIntegrationParameterFromGoogleService _integrationService;

        public IntegrationParameterFromGoogleController(IntegrationParameterFromGoogleService integrationService)
        {
            this._integrationService = integrationService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(IntegrationParameterFromGoogleVM model)
        {
            var result = _integrationService.Update(model);

            if (result.IsFailure)
            {
                return null;
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        public ActionResult Index()
        {
            ViewBag.TipoAcao = "I";

            return View(_integrationService.List(true));
        }

        [HttpPost]
        public ActionResult Create(IntegrationParameterFromGoogleVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id.ToString().ToString().Equals("00000000-0000-0000-0000-000000000000") || model.Id == null)
                    {

                        var lista = _integrationService.List(true);

                        if (lista?.Count == 0)
                        {
                            var ret = _integrationService.SaveNew(model);
                        }
                    }
                    else
                    {
                        var ret = _integrationService.Update(model);
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            _integrationService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
