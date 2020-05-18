using Framework;
using PagarMe;

namespace PaymentIntegrator.PagarMe.Common
{
    internal static class PagarMeHelper
    {
        /// <summary>
        /// Converte um erro do Pagar.me em objeto de validação.
        /// </summary>
        /// <param name="error">Erro do Pagar.me.</param>
        /// <returns>Objeto de validação.</returns>
        public static OperationResult ToValidation(this PagarMeError error)
        {
            var errors = new ErrorDictionary();

            if (error.Errors?.Length > 0)
            {
                foreach (var errorDetail in error.Errors)
                {
                    errors.Add("", errorDetail.Parameter + ": " + errorDetail.Message);
                }
            }

            return new OperationResult(errors);
        }
    }
}