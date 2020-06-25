using Domain.Entities;

namespace Application.Modules.Products.Search
{
    /// <summary>
    /// Dados da busca de produtos.
    /// </summary>
    public class NearbyProductsViewModel : NearbyProductsResultViewModel, IWithLang
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
        /// Filtro por distância.
        /// </summary>
        public long? LocationRange { get; set; }

        /// <summary>
        /// Idioma do usuário.
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Total de itens que podem ser filtrados.
        /// </summary>
        public int Total { get; set; }

        public NearbyProductsViewModel(NearbyProductsRequestViewModel request)
        {
            Lat = request.Lat;
            Lng = request.Lng;
            LocationRange = request.LocationRange;
            Lang = request.Lang;
        }
    }
}