using System;
using Application.Config.Common;

namespace Application.Config.Modules.Product
{
    /// <summary>
    /// Configuração de busca no módulo de produtos.
    /// </summary>
    [Obsolete("Migrado para SearchConfig")]
    public class ProductSearchConfig
    {
        /// <summary>
        /// Filtro busca em texto livre.
        /// </summary>
        public StatusConfig ByQuery { get; set; } = new StatusConfig();

        /// <summary>
        /// Filtro por cidade/estado.
        /// </summary>
        public StatusConfig ByCityAndState { get; set; } = new StatusConfig();

        /// <summary>
        /// Filtro por categoria.
        /// </summary>
        public StatusConfig ByCategories { get; set; } = new StatusConfig();

        /// <summary>
        /// Filtro somente por retirada no local.
        /// </summary>
        public StatusConfig ByOnlyOnsitePickup { get; set; } = new StatusConfig();

        /// <summary>
        /// Filtro somente por entrega física.
        /// </summary>
        public StatusConfig ByOnlyShippingOut { get; set; } = new StatusConfig();

        /// <summary>
        /// Se login é obrigatório para visualizar resultados da busca.
        /// </summary>
        public bool RequireLogin { get; set; } = false;

        /// <summary>
        /// Número de resultados por página.
        /// </summary>
        public int ResultsByPage { get; set; } = 21;
    }
}