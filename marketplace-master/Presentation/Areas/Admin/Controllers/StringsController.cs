using Domain.Interfaces.Repositories;
using Domain.Services;
using Infra.Crosscutting.Globalization;
using NewContext.Core;
using NewContext.Domain.Languages;
using NewContext.Queries.Languages;
using Presentation.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class StringsController : Controller
    {
        private readonly IStringRepository stringsRepository;
        private readonly StrService stringService;
        private readonly IMessagesDispatcher messages;
        private static Dictionary<string, Tuple<string, string>> _defaultValues;

        private Dictionary<string, Tuple<string, string>> defaultValues
        {
            get
            {
                if (_defaultValues == null || _defaultValues.Count == 0)
                    _defaultValues = stringService.GetDefaultStrings();

                return _defaultValues;
            }
            set { }
        }

        public StringsController(
            IStringRepository stringsRepository,
            StrService stringService,
            IMessagesDispatcher messages)
        {
            this.stringsRepository = stringsRepository;
            this.stringService = stringService;
            this.messages = messages;
        }

        public async Task<ActionResult> Index()
        {
            ViewBag.Defaults = defaultValues;

            var dto = await messages.QueryAsync(new ListStringsForAdminTranslation());
            var viewModel = StringsViewModel.From(dto);

            return View(viewModel);
        }

        public async Task<ActionResult> RestoreDefault(string labelName)
        {
            var label = (Label)labelName;

            foreach (var language in Language.SupportedLanguages)
                await messages.CommandAsync(new ResetLocalizedString(language, label));

            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public async Task<ActionResult> Update(StringUpdateViewModel model)
        {
            var specifier = model.Lang.GetSpecifier();

            var result = await messages.CommandAsync(new ChangeLocalizedString(
                (Language)specifier, (Label)model.Label, model.Value));

            return new EmptyResult();
        }
    }
}