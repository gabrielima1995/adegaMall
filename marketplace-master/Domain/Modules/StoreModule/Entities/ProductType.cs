using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.Modules.StoreModule.Entities
{
    public enum ProductType
    {
        /// <summary>
        /// Produto padrão.
        /// </summary>
        [DescriptionLocalized("PRODUCT_TYPE_DEFAULT_LABEL")]
        Default = 0,

        /// <summary>
        /// Pacote de créditos.
        /// </summary>
        [DescriptionLocalized("PRODUCT_TYPE_PACKAGE_LABEL")]
        Package = 1
    }
}