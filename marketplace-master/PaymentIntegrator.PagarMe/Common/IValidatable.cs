using Framework;

namespace PaymentIntegrator.PagarMe.Common
{
    /// <summary>
    /// Representa um objeto onde é possível validá-lo.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Valida o objeto.
        /// </summary>
        /// <returns>Objeto de validação.</returns>
        ErrorDictionary Validate();
    }
}