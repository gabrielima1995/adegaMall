using Application.Config;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    /// <summary>
    /// Configurações da aplicação.
    /// </summary>
    public class ApplicationConfig
    {
        public int Id { get; set; }

        /// <summary>
        /// Taxa do dólar, em reais.
        /// </summary>
        public decimal BRL_USD { get; set; }

        /// <summary>
        /// Configuração do marketplace, no formato json.
        /// </summary>
        [Column(TypeName = "VARCHAR(MAX)")]
        public string MaaSConfig { get; set; }

        public MaaSConfig GetConfig()
        {
            return !string.IsNullOrEmpty(MaaSConfig)
                ? Application.Config.MaaSConfig.FromJson(MaaSConfig)
                : Application.Config.MaaSConfig.Empty();
        }
    }
}