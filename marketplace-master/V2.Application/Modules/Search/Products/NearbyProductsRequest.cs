using Application.Config;
using Framework.Geography;

namespace V2.Application.Modules.Search.Products
{
    public class NearbyProductsRequest : SearchRequestBase
    {
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

        public NearbyProductsRequest()
        {
            PageSize = MaaSConfig.Current.NearbyProducts.TotalItems;
        }
    }
}