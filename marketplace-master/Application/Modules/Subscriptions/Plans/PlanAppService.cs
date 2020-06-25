using Domain.Entities.SubscriptionModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.Subscriptions.Plans;
using Domain.Services;
using Framework;
using Infra.Crosscutting.Globalization;
using PaymentIntegrator.Core.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Modules.Subscriptions.Plans
{
    public class PlanAppService : IPlanAppService
    {
        private readonly PlanService _planService;
        private readonly IPlanOperations _planOperations;
        private readonly GlobalizationService _strService;
        private readonly IStringRepository _stringRepository;

        public PlanAppService(
            PlanService planService,
            IPlanOperations planOperations,
            GlobalizationService globalizationService,
            IStringRepository stringRepository)
        {
            _planService = planService;
            _planOperations = planOperations;
            _strService = globalizationService;
            _stringRepository = stringRepository;
        }

        public async Task<OperationResult> ImportFromGatewayAsync()
        {
            var plans = await _planOperations.GetListAsync();

            if (plans == null || !plans.Any())
                return new OperationResult("", "Nenhum plano encontrado!");

            foreach (var plan in plans)
            {
                var model = new PlanViewModel(plan);

                _planService.Save(
                    i => i.GatewayId == model.GatewayId,
                    () => model.Create(_strService),
                    model.Update);
            }

            var list = GetList();

            GlobalizationHelper.LoadStrings(_stringRepository);

            foreach (var storedPlan in list)
            {
                if (plans.All(x => x.GatewayId != storedPlan.GatewayId))
                {
                    try
                    {
                        _planService.Delete(i => i.Id == storedPlan.Id);
                    }
                    catch (Exception)
                    {
                        _planService.Disable(storedPlan.Id);
                    }
                }
            }

            return new OperationResult();
        }

        public OperationResult<Plan> Save(PlanViewModel model)
        {
            var plan = _planService.Save(
                i => i.Id == model.Id,
                () => model.Create(_strService),
                model.Update);

            return plan;
        }

        public IList<Plan> GetList()
        {
            return _planService.GetPlans(false);
        }

        public void Enable(Guid planId)
        {
            _planService.Enable(planId);
        }

        public void Disable(Guid planId)
        {
            _planService.Disable(planId);
        }
    }
}