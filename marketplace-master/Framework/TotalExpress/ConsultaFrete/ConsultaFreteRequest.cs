using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.TotalExpress.ConsultaFrete
{
    public class ConsultaFreteRequest
    {
        public decimal Peso { get; set; }
        public decimal Valor { get; set; }
        public string Cep { get; set; }
        public int Altura { get; set; }
        public int Largura { get; set; }
        public int Profundidade { get; set; }
    }
}
