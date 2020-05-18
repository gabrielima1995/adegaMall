using Application.Config.Common;

namespace Application.Config.Modules.Product
{
    /// <summary>
    /// Configuração da página de produto.
    /// </summary>
    public class ProductPageConfig
    {
        /// <summary>
        /// Campo Quantidade na página do produto.
        /// </summary>
        public StatusConfig Quantity { get; set; } = new StatusConfig();
    }
}