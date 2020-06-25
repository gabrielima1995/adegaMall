using Application.Config;
using Domain.DTO;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Framework;
using Infra.Crosscutting.Globalization;
using Presentation.Areas.Admin.Models;
using Presentation.Globalization;
using System.Globalization;
using System.Web.Mvc;
using V2.Application.Messages;

namespace Presentation.Areas.Admin.Controllers
{
    public class MessageTemplatesController : Controller
    {
        private readonly MessageTemplateService _messageTemplateService;
        private readonly IMessageTemplateRepository _messageTemplateRepository;
        private readonly IStringRepository _stringRepository;

        public MessageTemplatesController(
            MessageTemplateService messageTemplateService,
            IMessageTemplateRepository messageTemplateRepository,
            IStringRepository stringRepository)
        {
            _messageTemplateService = messageTemplateService;
            _messageTemplateRepository = messageTemplateRepository;
            _stringRepository = stringRepository;
        }

        public ActionResult Index()
        {
            var model = new MessageTemplateViewModel(
                MessagesContext.Current,
                MaaSConfig.Current.Localization.GetEnabledLanguages());

            return View(model);
        }

        public ActionResult Edit(string key, string l)
        {
            var model = _messageTemplateRepository.Get(key, l ?? CultureInfo.CurrentUICulture.GetCurrentLang());

            if (model == null)
            {
                return HttpNotFound();
            }

            return View(new MessageTemplateEditingViewModel(model, MessagesContext.Current, MaaSConfig.Current));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(MessageTemplateEditing model)
        {
            if (ModelState.IsValid)
            {
                var validation = _messageTemplateService.Update(model)
                    .Then(result =>
                    {
                        var specification = MessagesContext.Current.Specifications[model.Key];

                        GlobalizationHelper.UpdateString(_stringRepository, specification.DefaultSubjectLabel, model.Lang, model.SubjectTemplate ?? "");

                        MessagesContext.Current.UpdateCache(result);

                        return OperationResult.Success;
                    });

                if (validation.OK)
                {
                    this.SetSuccessMessage(Request.GetLocalizedString("CHANGES_SAVED_MESSAGE"), true);
                    return RedirectToAction("Edit", new { key = model.Key, l = model.Lang, lang = Request.GetCurrentLang() });
                }

                this.AddErrors(validation);
            }

            return View(new MessageTemplateEditingViewModel(model, MessagesContext.Current, MaaSConfig.Current));
        }
    }
}