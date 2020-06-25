using NewContext.Core;
using NewContext.Domain.Languages;
using NewContext.Queries.Languages;
using Presentation.Areas.Admin.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class LanguagesController : BaseController
    {
        private const string viewPath = "~/Areas/Admin/Views/Strings/Languages.cshtml";
        private readonly IMessagesDispatcher messages;

        public LanguagesController(IMessagesDispatcher messages)
        {
            this.messages = messages;
        }

        public async Task<ActionResult> Index()
        {
            var model = await BuildViewModel();
            return View(viewPath, model);
        }

        [HttpPost]
        public async Task<ActionResult> Activate(string activateId)
        {
            var language = (Language)activateId;
            var result = await messages.CommandAsync(new ActivateLanguage(language));

            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error);
                var model = await BuildViewModel();
                return View(viewPath, model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Deactivate(string deactivateId)
        {
            var language = (Language)deactivateId;
            var result = await messages.CommandAsync(new DeactivateLanguage(language));

            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error);
                var model = await BuildViewModel();
                return View(viewPath, model);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> SetAsDefault(string setAsDefaultId)
        {
            var language = (Language)setAsDefaultId;
            var result = await messages.CommandAsync(new SetLanguageAsDefault(language));

            if (result.IsFailure)
            {
                ModelState.AddModelError("", result.Error);
                var model = await BuildViewModel();
                return View(viewPath, model);
            }

            return RedirectToAction("Index");
        }

        private async Task<LanguagesViewModel> BuildViewModel()
        {
            var languages = await messages.QueryAsync(new ListLanguagesForConfiguration());
            var model = new LanguagesViewModel(languages);
            return model;
        }
    }
}