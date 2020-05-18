using Application.Config.Common;

namespace Domain.Config.Modules.Product
{
    /// <summary>
    /// Configurações para modelos de produtos.
    /// </summary>
    public class ProductTemplatesConfig : StatusConfig
    {
        public ProductTemplatesConfig() : base(false)
        {
        }

        /// <summary>
        /// Determina se os vendedores são forçados a usar modelos para criar produtos.
        /// Se esta chave estiver desligada, os vendedores poderão criar produtos em branco, ou seja, sem usar um modelo.
        /// </summary>
        public bool ForceSellerToUseTemplates { get; set; } = true;
    }
}
