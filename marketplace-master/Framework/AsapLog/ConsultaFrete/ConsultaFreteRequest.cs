namespace Framework.AsapLog.ConsultaFrete
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