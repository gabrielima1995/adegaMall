using Application.Config.Common;
using Newtonsoft.Json;

namespace Application.Config.Modules.Product
{
    /// <summary>
    /// Configuração de avaliação de pedido
    /// </summary>
    public class OrderReviewConfig : StatusConfig
    {
        /// <summary>
        /// Número máximo de estrelas para avaliação.
        /// </summary>
        public int MaxStars { get; set; } = 5;

        /// <summary>
        /// Peso de uma estrela.
        /// </summary>
        [JsonIgnore]
        public int StarFactor => 100 / MaxStars;

        /// <summary>
        /// Obtem o número de estrelas com base na nota.
        /// </summary>
        /// <param name="rate">Nota de 0 a 100.</param>
        /// <returns>Número de estrelas</returns>
        public int GetNumberOfStars(int rate)
        {
            return rate / StarFactor;
        }
    }
}