using Application.Config;
using Domain.Entities.SubscriptionModule;
using Infra.Crosscutting.Globalization;
using System;

namespace Application.Modules.Subscriptions
{
    /// <summary>
    /// Tipo de assinatura.
    /// </summary>
    public class SignModeViewModel
    {
        /// <summary>
        /// Identificador da assinatura.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Valor da assinatura.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Símbolo do valor monetário da assinatura.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// Recorrência da assinatura.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Unidade da recorrência da assinatura.
        /// </summary>
        public UnitOfDuration UnitOfDuration { get; set; }

        /// <summary>
        /// Título da assinatura.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Descrição da assinatura.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Créditos adicionados a conta mensalmente
        /// </summary>
        public int? CreditsPerMonth { get; set; }

        public bool IsTrial => TrialDays.HasValue && TrialDays.Value >= 1;

        public int? TrialDays { get; set; }

        /// <summary>
        /// Recorrência da assinatura, em meses.
        /// </summary>
        public int DurationMonths
        {
            get
            {
                switch (UnitOfDuration)
                {
                    case UnitOfDuration.D:
                        return Duration / 30;

                    case UnitOfDuration.W:
                        return Duration / 4;

                    case UnitOfDuration.M:
                        return Duration;

                    case UnitOfDuration.Y:
                        return Duration * 12;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Valor da assinatura por mês.
        /// </summary>
        public decimal PriceMonthly => Price / Math.Max(DurationMonths, 1);

        public SignModeViewModel()
        {
        }

        public SignModeViewModel(Plan plan, string lang)
        {
            Id = plan.Id;
            Price = plan.Amount;
            CurrencySymbol = MaaSConfig.Current.DefaultCurrency;
            Duration = plan.Days;
            UnitOfDuration = UnitOfDuration.D;
            //Title = GlobalizationHelper.GetString(plan.NameLabel, lang);
            Title = plan.Name;
            Description = GlobalizationHelper.GetString(plan.DescriptionLabel, lang);
            CreditsPerMonth = plan.CreditsPerMonth;
            TrialDays = plan.TrialDays;
        }
    }

    public enum UnitOfDuration
    {
        /// <summary>
        /// For days.
        /// </summary>
        D,

        /// <summary>
        /// For weeks.
        /// </summary>
        W,

        /// <summary>
        /// For months.
        /// </summary>
        M,

        /// <summary>
        /// For years.
        /// </summary>
        Y
    }
}