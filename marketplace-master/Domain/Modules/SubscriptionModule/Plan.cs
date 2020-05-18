using PaymentIntegrator.Core.Plans;
using System;
using System.Collections.Generic;

namespace Domain.Entities.SubscriptionModule
{
    /// <summary>
    /// Representa um plano de assinatura.
    /// </summary>
    public class Plan : BaseEntity<Guid>, IPlan
    {
        /// <summary>
        /// Nome do plano.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Label para o nome do plano no arquivo de tradução.
        /// </summary>
        public string NameLabel { get; set; }

        /// <summary>
        /// Label para a descrição do plano no arquivo de tradução.
        /// </summary>
        public string DescriptionLabel { get; set; }

        /// <summary>
        /// Preço do plano.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Frequência de cobrança, em dias.
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Tempo de assinatura trial para o usuário, em dias.
        /// </summary>
        public int TrialDays { get; set; }

        /// <summary>
        /// Identificador do plano no gateway de pagamento.
        /// </summary>
        public string GatewayId { get; set; }

        /// <summary>
        /// Indica se o plano está habilitado no sistema.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Número de créidots que será adicionado a conta do assinante todo mês
        /// </summary>
        public int? CreditsPerMonth { get; set; }

        /// <summary>
        /// Número de dias que os créditos duram, se estiverem configurados para expirar
        /// </summary>
        public int? CreditsValidityDays { get; set; }

        /// <summary>
        /// Data em que o plano foi desabilitado no sistema.
        /// </summary>
        public DateTime? DisabledAt { get; private set; }

        public IList<Subscription> Subscriptions { get; private set; }

        /// <summary>
        /// Indica se é um plano com período de trial.
        /// </summary>
        public bool IsTrialPlan => TrialDays > 0;

        private Plan()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Enabled = true;
        }

        public Plan(string name, decimal amount, int days, int trialDays, string gatewayId)
            : this()
        {
            Name = name;
            Amount = amount;
            Days = days;
            TrialDays = trialDays;
            GatewayId = gatewayId;
            NameLabel = "PLAN_" + days + "_NAME";
            DescriptionLabel = "PLAN_" + days + "_DESCRIPTION";
        }

        public Plan(IPlan plan)
            : this(plan.Name, plan.Amount, plan.Days, plan.TrialDays, plan.GatewayId)
        {
        }

        /// <summary>
        /// Desabilita o plano no sistema.
        /// </summary>
        public void Disable()
        {
            Enabled = false;
            DisabledAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Habilita o plano no sistema.
        /// </summary>
        public void Enable()
        {
            Enabled = true;
            DisabledAt = null;
        }
    }
}