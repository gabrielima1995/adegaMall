using Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip
{
    public class MoipErrorResponse
    {
        public IList<MoipError> Errors { get; set; }

        public OperationResult Result => new ErrorDictionary("", Errors.Select(GetDescription).ToArray());

        private string GetDescription(MoipError error)
        {
            if (Regex.IsMatch(error.Code, @"REG-6\d\d"))
            {
                if (error.Code == "REG-623") return "Wirecard: Número de telefone inválido.";
                if (error.Code == "REG-630") return "Wirecard: Número do endereço inválido.";
                if (error.Code == "REG-643") return "Wirecard: Número do documento inválido.";

                return $"Wirecard: Valor inválido fornecido - {error.Path}";
            }
            if (Regex.IsMatch(error.Code, @"REG-5\d\d"))
            {
                return $"Wirecard: Valor obrigatório não preenchido - {error.Path}";
            }
            if (Regex.IsMatch(error.Code, @"REG-4\d\d"))
            {
                return $"Wirecard: Valor já foi utilizado - - {error.Path}";
            }

            return $"Wirecard: {error.Code} - {error.Description}";
        }
    }

    public class MoipError
    {
        public string Code { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }
}