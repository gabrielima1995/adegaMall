using Application.Config.Common;

namespace Application.Config.Modules.Product
{
    /// <summary>
    /// Configurações de variantes de produto.
    /// </summary>
    public class VariantsConfig : StatusConfig
    {
        /// <summary>
        /// Determina se o estoque pode ser controlado por variante.
        /// </summary>
        public bool StockByVariantEnabled { get; set; } = false;

        /// <summary>
        /// Determina a quantidade mínima de variantes para que elas virem um dropdownList
        /// </summary>
        public int enableForNItemsOrHigher { get; set; } = 10;
    }
}