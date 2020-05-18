using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentIntegrator.Core.Plans
{
    /// <summary>
    /// Operações de plano de assinatura no gateway de pagamento.
    /// </summary>
    public interface IPlanOperations
    {
        /// <summary>
        /// Obtem todos os planos de assinatura criados no gateway de pagamento.
        /// </summary>
        /// <param name="page">Número da página de resultados.</param>
        /// <param name="resultsByPage">Número de resultados por página.</param>
        /// <returns></returns>
        Task<IList<IPlan>> GetListAsync(int page = 1, int resultsByPage = 50);
    }
}