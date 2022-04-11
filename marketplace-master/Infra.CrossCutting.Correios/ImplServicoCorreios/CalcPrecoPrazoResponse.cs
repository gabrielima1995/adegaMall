using Infra.Crosscutting.Correios.Infra.CrossCutting.Correios.External;
using System.Collections.Generic;

namespace Infra.Crosscutting.Correios
{
    public class CalcPrecoPrazoResponse : ICalcPrecoPrazoResponse
    {
        public List<ICalcPrecoPrazoResponseItemServico> Servicos { get; set; }

        public CalcPrecoPrazoResponse()
        {
            Servicos = new List<ICalcPrecoPrazoResponseItemServico>();
        }

        private CalcPrecoPrazoResponse(ServicosEntrega services, string errorCode, string errorMessage) : this()
        {
            
        }

        internal CalcPrecoPrazoResponse(cResultado serviceResult) : this()
        {
            foreach (var s in serviceResult.Servicos)
            {
                var mapped = new CalcPrecoPrazoResponseItemServico(s);
                Servicos.Add(mapped);
            }
        }

        internal static CalcPrecoPrazoResponse CommunicationError(ServicosEntrega services, 
            string errorMessage, string mensagemErroProvedor)
        {
            var response = new CalcPrecoPrazoResponse();
            response.Servicos.Add(new CalcPrecoPrazoResponseItemServico(services, "comms", 
                errorMessage, mensagemErroProvedor));
            return response;
        }
    }
}
