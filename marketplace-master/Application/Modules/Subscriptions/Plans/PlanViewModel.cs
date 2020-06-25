using Domain.Entities.SubscriptionModule;
using Domain.Services;
using PaymentIntegrator.Core.Plans;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Subscriptions.Plans
{
    /// <summary>
    /// Plano de assinatura.
    /// </summary>
    public class PlanViewModel
    {
        public Guid Id { get; set; }

        [DisplayNameLocalized("PLAN_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string Name { get; set; }

        [DisplayNameLocalized("PLAN_AMOUNT_LABEL")]
        [RequiredLocalized]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [RangeLocalized(0, 10000)]
        public decimal Amount { get; set; }

        [DisplayNameLocalized("PLAN_DAYS_LABEL")]
        [RequiredLocalized]
        [RangeLocalized(0, 1825)]
        public int Days { get; set; }

        [DisplayNameLocalized("PLAN_TRIAL_DAYS_LABEL")]
        [RangeLocalized(0, 730)]
        public int TrialDays { get; set; }

        public string GatewayId { get; set; }

        public PlanViewModel()
        {

        }

        public PlanViewModel(IPlan plan)
            : this()
        {
            Name = plan.Name;
            Amount = plan.Amount;
            Days = plan.Days;
            TrialDays = plan.TrialDays;
            GatewayId = plan.GatewayId;
        }

        public PlanViewModel(Plan plan)
            : this((IPlan)plan)
        {
            Id = plan.Id;
        }

        public Plan Create(GlobalizationService strService)
        {
            var plan = new Plan(Name, Amount, Days, TrialDays, GatewayId);

            strService.SaveAllLangs(plan.NameLabel, plan.Name);
            strService.SaveAllLangs(plan.DescriptionLabel, "Cobrança de {1} {0} no seu Cartão de Crédito, a cada " + plan.Days + " dias.");

            return plan;
        }

        public void Update(Plan entity)
        {
            entity.Name = Name;
            entity.Amount = Amount;
            entity.Days = Days;
            entity.TrialDays = TrialDays;
            entity.GatewayId = GatewayId;
        }
    }
}