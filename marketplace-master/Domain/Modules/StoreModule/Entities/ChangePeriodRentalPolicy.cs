using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.Modules.StoreModule.Entities
{
    public enum ChangePeriodRentalPolicy
    {
        /// <summary>
        /// Não aceito.
        /// </summary>
        [DescriptionLocalized("CHANGE_PERIOD_RENTAL_POLICY_DISALLOW_LABEL")]
        Disallow = -1,

        /// <summary>
        /// Aceito com antecedência mínima.
        /// </summary>
        [DescriptionLocalized("CHANGE_PERIOD_RENTAL_POLICY_ALLOW_IN_ADVANCE_LABEL")]
        AllowInAdvance = 0,

        /// <summary>
        /// Aceito a qualquer momento.
        /// </summary>
        [DescriptionLocalized("CHANGE_PERIOD_RENTAL_POLICY_ALLOW_LABEL")]
        Allow = 1
    }
}