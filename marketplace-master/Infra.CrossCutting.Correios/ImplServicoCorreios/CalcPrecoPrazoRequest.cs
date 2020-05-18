using System;
using System.Collections.Generic;
using System.Linq;

namespace Infra.Crosscutting.Correios
{
    public class CalcPrecoPrazoRequest : ICalcPrecoPrazoRequest
    {
        public string CodigoEmpresa { get; set; }

        public string SenhaEmpresa { get; set; }

        public ServicosEntrega Servicos { get; set; }

        public string CepOrigem { get; set; }

        public string CepDestino { get; set; }

        public decimal Peso { get; set; }

        public FormatoEncomenda Formato { get; set; }

        public decimal Comprimento { get; set; }

        public decimal Altura { get; set; }

        public decimal Largura { get; set; }

        public decimal Diametro { get; set; }

        public bool MaoPropria { get; set; }

        public decimal? ValorDeclarado { get; set; }

        public bool AvisoRecebimento { get; set; }

        public IEnumerable<string> ServicosContratoEmpresa { get; set; } = Array.Empty<string>();

        public bool IsValid(out string[] errors)
        {
            bool isValid = true;
            var errorList = new List<string>();

            if (CodigoEmpresa == null || SenhaEmpresa == null)
            {
                if (!Servicos.HasFlag(ServicosEntrega.PacVarejo) &&
                    !Servicos.HasFlag(ServicosEntrega.Sedex10Varejo) &&
                    !Servicos.HasFlag(ServicosEntrega.SedexACobrarVarejo) &&
                    !Servicos.HasFlag(ServicosEntrega.SedexHojeVarejo) &&
                    !Servicos.HasFlag(ServicosEntrega.SedexVarejo))
                {
                    isValid = false;
                    errorList.Add("Nenhum código de serviço padrão especificado para consulta.");
                }
            }
            else
            {
                if (!(ServicosContratoEmpresa?.Any() ?? false))
                {
                    isValid = false;
                    errorList.Add("Nenhum código de serviço do contrato da empresa especificado para a consulta.");
                }
            }

            if (string.IsNullOrEmpty(CepOrigem))
            {
                isValid = false;
                errorList.Add("O CEP de origem deve ser especificado.");
            }

            if (string.IsNullOrEmpty(CepDestino))
            {
                isValid = false;
                errorList.Add("O CEP de destino deve ser especificado.");
            }

            if (Peso <= 0)
            {
                isValid = false;
                errorList.Add("O peso da encomenda deve ser especificado.");
            }

            if (Formato == FormatoEncomenda.CaixaPacote)
            {
                if (Altura <= 0 || Largura <= 0 || Comprimento <= 0)
                {
                    isValid = false;
                    errorList.Add("As dimensões (altura, largura e comprimento) devem ser especificadas para o formato de entrega \"Caixa ou Pacote\".");
                }
            }
            else if (Formato == FormatoEncomenda.RoloPrisma)
            {
                if (Comprimento <= 0 || Diametro <= 0)
                {
                    isValid = false;
                    errorList.Add("As dimensões (comprimento e diâmetro) devem ser especificadas para o formato de entrega \"Rolo ou Prisma\".");
                }
            }
            else if (Formato == FormatoEncomenda.Envelope)
            {
                if (Largura <= 0 || Comprimento <= 0)
                {
                    isValid = false;
                    errorList.Add("As dimensões (largura e comprimento) devem ser especificadas para o formato de entrega \"Envelope\".");
                }
            }

            errors = errorList.ToArray();
            return isValid;
        }
    }
}
