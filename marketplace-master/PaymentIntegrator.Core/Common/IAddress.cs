namespace PaymentIntegrator.Core.Common
{
    /// <summary>
    /// Representa um endereço.
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// Logradouro.
        /// </summary>
        string Street { get; }

        /// <summary>
        /// Número do logradouro.
        /// </summary>
        string StreetNumber { get; }

        /// <summary>
        /// Complemento do endereço.
        /// </summary>
        string Complementary { get; }

        /// <summary>
        /// Bairro.
        /// </summary>
        string Neighborhood { get; }

        /// <summary>
        /// Cidade.
        /// </summary>
        string City { get; }

        /// <summary>
        /// Estado.
        /// </summary>
        string State { get; }

        /// <summary>
        /// Código postal.
        /// </summary>
        string ZipCode { get; }
    }
}