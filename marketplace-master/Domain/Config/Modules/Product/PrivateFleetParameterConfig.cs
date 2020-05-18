namespace Application.Config.Modules.Product
{
    /// <summary>
    /// Configuração de um raio de entrega de frota própria/privada.
    /// </summary>
    public class PrivateFleetParameterConfig
    {
        /// <summary>
        /// Raio de entrega, em quilômetros.
        /// </summary>
        public int Range { get; set; }
    }
}