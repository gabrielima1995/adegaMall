using Application.Modules.Subscriptions.Plans;
using Domain.Interfaces.Repositories;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class PlansController : Controller
    {
        private readonly IPlanAppService _planAppService;
        private readonly IPlanRepository _planRepository;

        public PlansController(
            IPlanAppService planAppService,
            IPlanRepository planRepository)
        {
            _planAppService = planAppService;
            _planRepository = planRepository;
        }

        public ActionResult Index()
        {
            return View(_planAppService.GetList());
        }

        [HttpPost]
        public ActionResult Enable(Guid id)
        {
            _planAppService.Enable(id);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Disable(Guid id)
        {
            _planAppService.Disable(id);

            return new EmptyResult();
        }

        [HttpPost]
        public async Task<ActionResult> Import()
        {
            await _planAppService.ImportFromGatewayAsync();

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateDays(string planId, int newValue)
        {
            if (string.IsNullOrWhiteSpace(planId))
                throw new ArgumentNullException("planId", "Plan Id cannot be null or empty");
            if (newValue <= 0)
                throw new ArgumentOutOfRangeException("newValue", "New value has to be a non-zero positive number");

            var plan = _planRepository.GetById(new Guid(planId));
            plan.CreditsValidityDays = newValue;
            var result = _planRepository.Update(plan, true);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateCredits(string planId, int newValue)
        {
            if (string.IsNullOrWhiteSpace(planId))
                throw new ArgumentNullException("planId", "Plan Id cannot be null or empty");
            if (newValue <= 0)
                throw new ArgumentOutOfRangeException("newValue", "New value has to be a non-zero positive number");

            var plan = _planRepository.GetById(new Guid(planId));
            plan.CreditsPerMonth = newValue;
            var result = _planRepository.Update(plan, true);

            return new EmptyResult();
        }
    }
}