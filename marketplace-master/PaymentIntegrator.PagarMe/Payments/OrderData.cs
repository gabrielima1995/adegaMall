using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentIntegrator.PagarMe.Payments
{
    /// <summary>
    /// Wrapper para dados de um pedido, quando passado somente <see cref="IPaymentItem"/> ou <see cref="IEnumerable{IPaymentItem}"/> nos métodos de pagamento.
    /// </summary>
    public class OrderData : IOrder
    {
        public string OrderId { get; }

        public IList<IPaymentItem> Items { get; }

        public ISupplier Supplier { get; }

        public decimal Discount { get; } = 0;

        public decimal Shipping { get; } = 0;

        public decimal Amount => Items.Sum(i => i.Amount);

        public decimal StartupFee { get; private set; }

        public bool ProcessingGatewayFee { get; private set; }

        public string SplitId { get; private set; }

        public decimal InstallmentFees => 0;

        public OrderData(IList<IPaymentItem> items, ISupplier supplier)
        {
            OrderId = Guid.NewGuid().ToString();
            Items = items;
            Supplier = supplier;
        }

        public OrderData(IPaymentItem item, ISupplier supplier)
            : this(new List<IPaymentItem>(1)
            {
                item
            }, supplier)
        {
        }

        public void SetStartupFee(decimal startupFee)
        {
            StartupFee = startupFee;
        }

        public void SetSplitId(string splitId)
        {
            SplitId = splitId;
        }

        public void SetProcessingGatewayFee(bool processingGatewayFee)
        {
            ProcessingGatewayFee = processingGatewayFee;
        }

        public decimal GetSplitFees()
        {
            throw new NotImplementedException();
        }

        public bool IsLiable()
        {
            throw new NotImplementedException();
        }
    }
}