using System.Collections.Generic;

namespace Infra.Crosscutting.Correios
{
    public interface ICalcPrecoPrazoResponse
    {
        List<ICalcPrecoPrazoResponseItemServico> Servicos { get; set; }
    }
}