using Domain.DTO;
using Framework.Geography;
using System;

namespace V2.Application.Modules.Search.Products
{
    public class ProductSearchRequest : SearchRequestBase
    {
        /// <summary>
        /// Filtro por parte do nome do produto.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Filtro por cidade (produtos por retirada no local)
        /// </summary>
        public string CityAndState { get; set; }

        /// <summary>
        /// Filtro por categoria.
        /// </summary>
        public int? Category { get; set; }

        public string CategorySlug { get; set; }

        /// <summary>
        /// Filtro por somente retirada no local.
        /// </summary>
        public bool OnlyOnsitePickup { get; set; }

        /// <summary>
        /// Área do mapa.
        /// </summary>
        public GeoBounds Bounds { get; set; }

        /// <summary>
        /// Localização.
        /// </summary>
        public GeoCoordinate Location { get; set; }

        /// <summary>
        /// Raio a partir da localização.
        /// </summary>
        public long? LocationRange { get; set; }

        /// <summary>
        /// Ordenação da busca.
        /// </summary>
        public ProductSortType Sorting { get; set; }

        /// <summary>
        /// Lista de variantes selecionadas nos filtros.
        /// </summary>
        public int[] VariantIds { get; set; }

        public string VariantSlug { get; set; }

        /// <summary>
        /// Limita apenas aos produtos do vendedor especificado.
        /// </summary>
        public string SellerId { get; set; }

        #region Disponibilidade da locação

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool OnlySatAndSun { get; set; }
        public bool OnlyMonToFri { get; set; }
        public bool OnlyMorning { get; set; }
        public bool OnlyAfternoon { get; set; }
        public bool OnlyNight { get; set; }

        #endregion Disponibilidade da locação
    }
}