using Application.Config;
using Framework.Geography;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using V2.Application.Modules.Search.Products;

namespace Application.Modules.Products.Search
{
    /// <summary>
    /// Filtro para busca de produtos.
    /// </summary>
    public class NearbyProductsRequestViewModel : Application.Search.SearchRequest
    {
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

        protected override int GetLimitDefault()
        {
            return MaaSConfig.Current.NearbyProducts.TotalItems;
        }

        public override string GetNextUrlParams()
        {
            var q = new NameValueCollection
            {
                { "Lat", Lat },
                { "Lng", Lng },
                { "LocationRange", LocationRange },
                { "Limit", Limit },
                { "Page", Page + 1 },
                { "Area", "" },
                { "BoundsAx", BoundsAx },
                { "BoundsAy", BoundsAy },
                { "BoundsBx", BoundsBx },
                { "BoundsBy", BoundsBy },
            };

            return q.ToQueryString();
        }

        public GeoBounds GetBounds()
        {
            if (BoundsAx.HasValue && BoundsAy.HasValue && BoundsBx.HasValue && BoundsBy.HasValue)
            {
                return new GeoBounds(new GeoCoordinate(BoundsAy.Value, BoundsAx.Value),
                                     new GeoCoordinate(BoundsBy.Value, BoundsBx.Value));
            }

            return null;
        }

        public GeoCoordinate GetLocation()
        {
            return Lat.HasValue && Lng.HasValue
                ? new GeoCoordinate(Lat.Value, Lng.Value)
                : GetBounds()?.GetCenter();
        }

        public NearbyProductsRequest ToNearbyRequest()
        {
            return new NearbyProductsRequest
            {
                StartIndex = Math.Max(Page - 1, 0) * Limit,
                PageSize = Limit,
                Location = GetLocation(),
                LocationRange = LocationRange,
                Bounds = GetBounds()
            };
        }
    }
}