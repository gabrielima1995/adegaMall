using Application.Config;
using Domain.DTO;
using Framework.Geography;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using V2.Application.Modules.Search.Products;

namespace Application.Modules.Products.Search
{
    /// <summary>
    /// Filtro para busca de produtos.
    /// </summary>
    public class SearchRequestViewModel : Application.Search.SearchRequest
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
        /// Filtro por latitude da região de entrega.
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// Filtro por longitude da região de entrega.
        /// </summary>
        public double? Lng { get; set; }

        /// <summary>
        /// Raio a partir da localização.
        /// </summary>
        public long? LocationRange { get; set; }

        /// <summary>
        /// Coordenada X do Primeiro Ponto da Boundary
        /// </summary>
        public double? BoundsAx { get; set; }

        /// <summary>
        /// Coordenada Y do Primeiro Ponto da Boundary
        /// </summary>
        public double? BoundsAy { get; set; }

        /// <summary>
        /// Coordenada X do Segundo Ponto da Boundary
        /// </summary>
        public double? BoundsBx { get; set; }

        /// <summary>
        /// Coordenada Y do Segundo Ponto da Boundary
        /// </summary>
        public double? BoundsBy { get; set; }

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
        /// FIltro por somente entrega física.
        /// </summary>
        public bool OnlyShippingOut { get; set; }

        /// <summary>
        /// Ordenação da busca.
        /// </summary>
        public ProductSortType SortType { get; set; }

        /// <summary>
        /// Lista de variantes selecionadas nos filtros.
        /// </summary>
        public IList<int> VariantIds { get; set; }

        public string VariantSlug { get; set; }

        /// <summary>
        /// Limita apenas aos produtos do vendedor especificado.
        /// </summary>
        public string SellerId { get; set; }

        #region Disponibilidade da locação

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public SearchDaysOfWeek SearchDaysOfWeek { get; set; }

        public bool OnlyMorning { get; set; }
        public bool OnlyAfternoon { get; set; }
        public bool OnlyNight { get; set; }

        #endregion Disponibilidade da locação

        public SearchRequestViewModel()
        {
            VariantIds = new List<int>();
        }

        protected override int GetLimitDefault()
        {
            return MaaSConfig.Current.Search.List.PageSize;
        }

        public override string GetNextUrlParams()
        {
            var q = new NameValueCollection
            {
                { "Product", Product },
                { "CityAndState", CityAndState },
                { "Lat", Lat },
                { "Lng", Lng },
                { "LocationRange", LocationRange },
                { "Category", Category },
                { "CategorySlug", CategorySlug },
                { "OnlyOnsitePickup", OnlyOnsitePickup },
                { "OnlyShippingOut", OnlyShippingOut },
                { "Limit", Limit },
                { "Page", Page + 1 },
                { "Area", "" },
                { "SellerId", SellerId },
                { "SortType", (int)SortType },
                { "BoundsAx", BoundsAx },
                { "BoundsAy", BoundsAy },
                { "BoundsBx", BoundsBx },
                { "BoundsBy", BoundsBy },
                { "StartDate", StartDate },
                { "EndDate", EndDate },
                { "SearchDaysOfWeek", SearchDaysOfWeek },
                { "OnlyMorning", OnlyMorning },
                { "OnlyAfternoon", OnlyAfternoon },
                { "OnlyNight", OnlyNight },
                { "VariantSlug", VariantSlug }
            };

            foreach (var item in VariantIds)
                q.Add("f", item);

            return q.ToQueryString();
        }

        public GeoBounds GetBoundsOrNull()
        {
            if (BoundsAx.HasValue && BoundsAy.HasValue && BoundsBx.HasValue && BoundsBy.HasValue)
                return new GeoBounds(
                    new GeoCoordinate(BoundsAy.Value, BoundsAx.Value),
                    new GeoCoordinate(BoundsBy.Value, BoundsBx.Value));
            return null;
        }

        public GeoCoordinate GetLocationOrNull()
        {
            return Lat.HasValue && Lng.HasValue
                ? new GeoCoordinate(Lat.Value, Lng.Value)
                : GetBoundsOrNull()?.GetCenter();
        }

        public ProductSearchRequest ToSearchRequest()
        {
            var location = LocationRange.HasValue ? GetLocationOrNull() : null;
            var bounds = location == null ? GetBoundsOrNull() : null;

            return new ProductSearchRequest
            {
                StartIndex = Math.Max(Page - 1, 0) * Limit,
                PageSize = Limit,
                Product = Product,
                CityAndState = CityAndState,
                Location = location,
                LocationRange = LocationRange,
                Bounds = bounds,
                Category = Category,
                CategorySlug = CategorySlug,
                OnlyOnsitePickup = OnlyOnsitePickup,
                Sorting = SortType,
                VariantIds = VariantIds?.ToArray(),
                VariantSlug = VariantSlug,
                SellerId = SellerId,
                StartDate = StartDate,
                EndDate = EndDate,
                OnlySatAndSun = SearchDaysOfWeek == SearchDaysOfWeek.OnlySatAndSun,
                OnlyMonToFri = SearchDaysOfWeek == SearchDaysOfWeek.OnlyMonToFri,
                OnlyMorning = OnlyMorning,
                OnlyAfternoon = OnlyAfternoon,
                OnlyNight = OnlyNight
            };
        }
    }
}