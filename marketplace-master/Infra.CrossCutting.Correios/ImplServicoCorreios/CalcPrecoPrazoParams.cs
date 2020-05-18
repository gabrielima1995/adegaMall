using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infra.Crosscutting.Correios
{
    internal class CalcPrecoPrazoParams
    {
        public string nCdEmpresa { get; set; }

        public string sDsSenha { get; set; }

        public string nCdServico { get; set; }

        public string sCepOrigem { get; set; }

        public string sCepDestino { get; set; }

        public string nVlPeso { get; set; }

        public int nCdFormato { get; set; }

        public decimal nVlComprimento { get; set; }

        public decimal nVlAltura { get; set; }

        public decimal nVlLargura { get; set; }

        public decimal nVlDiametro { get; set; }

        public string sCdMaoPropria { get; set; }

        public decimal nVlValorDeclarado { get; set; }

        public string sCdAvisoRecebimento { get; set; }

        public CalcPrecoPrazoParams()
        {
        }

        public CalcPrecoPrazoParams(ICalcPrecoPrazoRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var ptBr = new CultureInfo("pt-BR");

            nCdEmpresa = request.CodigoEmpresa ?? string.Empty;

            sDsSenha = request.SenhaEmpresa ?? string.Empty;

            nCdServico = GetCdServico(request);

            sCepOrigem = SanitizeZipCode(request.CepOrigem);

            sCepDestino = SanitizeZipCode(request.CepDestino);

            nVlPeso = request.Peso.ToString("N2", ptBr);

            nCdFormato = (int)request.Formato;

            nVlComprimento = request.Formato == FormatoEncomenda.Envelope ? 0 : request.Comprimento;

            nVlAltura = request.Formato == FormatoEncomenda.Envelope ? 0 : request.Altura;

            nVlLargura = request.Largura;

            nVlDiametro = request.Diametro;

            sCdMaoPropria = request.MaoPropria ? "S" : "N";

            nVlValorDeclarado = request.ValorDeclarado ?? 0M;

            sCdAvisoRecebimento = request.AvisoRecebimento ? "S" : "N";
        }

        private string GetCdServico(ICalcPrecoPrazoRequest request)
        {
            var flags = new List<string>(5);
            if (request.CodigoEmpresa == null && request.SenhaEmpresa == null)
            {
                foreach (ServicosEntrega value in Enum.GetValues(typeof(ServicosEntrega)).Cast<ServicosEntrega>())
                    if (request.Servicos.HasFlag(value))
                        flags.Add(((int)value).ToString());
            }
            else flags.AddRange(request.ServicosContratoEmpresa);

            return string.Join(",", flags);
        }

        private string SanitizeZipCode(string zipCode)
        {
            var regex = new Regex(@"^[0-9]+$");
            var sanitized = regex.Replace(zipCode ?? string.Empty, string.Empty);
            return sanitized;
        }
    }
}
