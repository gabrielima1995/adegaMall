namespace V2.Infrastructure.Shipping
{
    public class ItemFrete
    {
        public double Peso { get; private set; }

        public decimal Valor { get; private set; }

        public double Comprimento { get; private set; }

        public double Largura { get; private set; }

        public double Altura { get; private set; }

        public double Volume => Comprimento * Altura * Largura;

        public ItemFrete() { }

        public ItemFrete(double peso, decimal valor, double comprimento, double largura, double altura)
        {
            Peso = peso;
            Valor = valor;
            Comprimento = comprimento;
            Largura = largura;
            Altura = altura;
        }
    }
}