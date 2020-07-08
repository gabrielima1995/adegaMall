using System;
using System.Collections.Generic;

namespace V2.Infrastructure.Shipping
{
    internal class PacoteFrete
    {
        private const double MinComprimento = 16D;
        private const double MinLargura = 11D;
        private const double MinAltura = 2D;

        private readonly List<ItemFrete> _itens = new List<ItemFrete>();

        public IReadOnlyList<ItemFrete> Itens => _itens.AsReadOnly();

        public double Peso { get; private set; }

        public decimal Valor { get; private set; }

        public double Comprimento { get; private set; }

        public double Largura { get; private set; }

        public double Altura { get; private set; }

        public double Volume { get; private set; }

        public PacoteFrete()
        {
        }

        public PacoteFrete(IEnumerable<ItemFrete> itens)
        {
            foreach (var item in itens)
                AdicionarItem(item);
        }

        public void AdicionarItem(ItemFrete item)
        {
            _itens.Add(item);

            CalcularMedidas(item);
        }

        private void CalcularMedidas(ItemFrete item)
        {
            Peso += item.Peso;
            Valor += item.Valor;
            Volume += item.Volume;

            if (_itens.Count == 1)
            {
                Comprimento = item.Comprimento;
                Largura = item.Largura;
                Altura = item.Altura;
            }
            else
            {
                var dimensao = Math.Pow(Volume, (1D / 3));
                Comprimento = Largura = Altura = dimensao;
            }

            GarantirMedidasMinimas();
        }

        private void GarantirMedidasMinimas()
        {
            Comprimento = Math.Max(Comprimento, MinComprimento);
            Largura = Math.Max(Largura, MinLargura);
            Altura = Math.Max(Altura, MinAltura);
        }
    }
}