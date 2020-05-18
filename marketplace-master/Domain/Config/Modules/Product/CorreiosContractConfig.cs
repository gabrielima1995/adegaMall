using Application.Config.Common;
using Newtonsoft.Json;

namespace Domain.Config.Modules.Product
{
    /// <summary>
    /// Define os dados de contrato com os Correios, para uso no WebService de cálculo de preço e prazo.
    /// </summary>
    public class CorreiosContractConfig : StatusConfig
    {
        public CorreiosContractConfig(bool enabled = false) : base(enabled)
        {
        }

        /// <summary>
        /// Seu código administrativo junto à ECT. O código está disponível no corpo do contrato firmado com os Correios.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Senha para acesso ao serviço, associada ao seu código administrativo.A senha inicial corresponde aos 8 primeiros dígitos do CNPJ informado no contrato. 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Código do serviço PAC de acordo com o contrato.
        /// </summary>
        public string PacServiceCode { get; set; }

        /// <summary>
        /// Código do serviço Sedex de acordo com o contrato.
        /// </summary>
        public string SedexServiceCode { get; set; }

        /// <summary>
        /// Indica se o código de contrato está pronto para ser usado: estando habilitado e contendo código e senha da empresa.
        /// </summary>
        [JsonIgnore]
        public bool UseContractCode => Enabled &&
            Code != null && Password != null &&
            PacServiceCode != null && SedexServiceCode != null;
    }
}
