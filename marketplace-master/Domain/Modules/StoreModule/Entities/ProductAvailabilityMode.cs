using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.Modules.StoreModule.Entities
{
    public enum ProductAvailabilityMode
    {
        /// <summary>
        /// Disponibilidade automática, com base nos períodos livre da agenda.
        /// </summary>
        [DescriptionLocalized("PRODUCT_AVAILABILITY_MODE_AUTO_LABEL")]
        Auto = 0,

        /// <summary>
        /// Disponibilidade manual, com base no cadastro de disponbilidades do produto.
        /// </summary>
        [DescriptionLocalized("PRODUCT_AVAILABILITY_MODE_MANUAL_LABEL")]
        Manual = 1
    }
}