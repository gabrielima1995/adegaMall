using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Transação de pagamento do(s) pedido(s) do comprador para o vendedor, no gateway de pagamento.
    /// </summary>
    public class OrderTransaction : BaseEntity<Guid>, ITransaction
    {
        private readonly bool isPaid;
        private readonly bool isPendingPayment;
        private readonly bool isPaymentCancelledOrRejected;

        /// <summary>
        /// Pedido(s) pago(s) pelo comprador para o(s) vendedor(es), nesta transação.
        /// </summary>
        public IList<Order> Orders { get; set; }

        /// <summary>
        /// Identificador da transação no gateway de pagamento.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Status da transação no gateway de pagamento.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Mensagem da transação no gateway de pagamento.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Valor total da transação do(s) pedido(s).
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Valor estornado da transação.
        /// </summary>
        public decimal RefundedAmount { get; set; }

        /// <summary>
        /// Valor da taxa da startup (caso for marketplace).
        /// </summary>
        public decimal StartupFee { get; set; }

        /// <summary>
        /// Valor estornado da taxa da startup (caso for marketplace).
        /// </summary>
        public decimal RefundedStartupFee { get; set; }

        /// <summary>
        /// Identificador da regra de split para a startup (caso for marketplace).
        /// </summary>
        public string StartupSplitId { get; set; }

        /// <summary>
        /// Valor da taxa do gateway de pagamento.
        /// </summary>
        public decimal GatewayFee { get; set; }

        /// <summary>
        /// Valor estornado da taxa do gateway de pagamento.
        /// </summary>
        public decimal RefundedGatewayFee { get; set; }

        /// <summary>
        /// URL do sistema que receberá notificações a cada mudança no status da transação.
        /// </summary>
        public string PostbackUrl { get; set; }

        /// <summary>
        /// URL de impressão do boleto, se este for o método de pagamento.
        /// </summary>
        public string BoletoUrl { get; set; }

        /// <summary>
        /// Código de barras do boleto, se este for o método de pagamento.
        /// </summary>
        public string BoletoBarCode { get; set; }

        /// <summary>
        /// Data de expiração do boleto, se este for o método de pagamento.
        /// </summary>
        [Column(TypeName = "datetime2")]
        public DateTime? BoletoExpirationDate { get; set; }

        public int Installments { get; set; }

        public ReadOnlyCollection<MultiPaymentItem> MultiPayments { get; }

        public bool IsMultiPayment { get; }

        /// <summary>
        /// Indica se a transação já está paga.
        /// </summary>
        public bool IsPaid() => isPaid;

        /// <summary>
        /// Indica se a transação está pendente de pagamento. Deve ser false caso a transação tenha sido rejeitada ou cancelada.
        /// </summary>
        public bool IsPendingPayment() => isPendingPayment;

        /// <summary>
        /// Indica se a transação teve seu pagamento cancelado, negado ou revertido.
        /// </summary>
        public bool IsPaymentCancelledOrRejected() => isPaymentCancelledOrRejected;

        public OrderTransaction()
        {
            Id = Guid.NewGuid();
        }

        public OrderTransaction(ITransaction transaction, IList<Order> orders)
            : this()
        {
            if (!transaction.IsMultiPayment)
                Orders = orders;

            TransactionId = transaction.TransactionId;
            Status = transaction.Status;
            Message = transaction.Message;
            Amount = transaction.Amount;
            StartupFee = transaction.StartupFee;
            StartupSplitId = transaction.StartupSplitId;
            GatewayFee = transaction.GatewayFee;

            PostbackUrl = transaction.PostbackUrl;
            BoletoBarCode = transaction.BoletoBarCode;
            BoletoUrl = transaction.BoletoUrl;
            BoletoExpirationDate = transaction.BoletoExpirationDate;

            Installments = transaction.Installments;

            isPaid = transaction.IsPaid();
            isPendingPayment = transaction.IsPendingPayment();
            isPaymentCancelledOrRejected = transaction.IsPaymentCancelledOrRejected();

            if (!transaction.IsMultiPayment)
            {
                foreach (var order in orders)
                    order.Transaction = this;
            }

        }

        /// <summary>
        /// Atribui os valores estornados na transação.
        /// </summary>
        /// <param name="refundedAmount">Valor estornado da transação.</param>
        /// <param name="refundedStartupFee">Valor estornado da taxa da startup (caso for marketplace).</param>
        /// <param name="refundedGatewayFee">Valor estornado da taxa do gateway de pagamento.</param>
        public void SetRefundedValues(decimal refundedAmount, decimal refundedStartupFee, decimal refundedGatewayFee)
        {
            RefundedAmount = refundedAmount;
            RefundedStartupFee = refundedStartupFee;
            RefundedGatewayFee = refundedGatewayFee;
        }

        public void SetValuesForMultiPayment(string id, decimal amount, decimal startupFee)
        {
            throw new InvalidOperationException("OrderTransaction class 'SetValuesForMultiPayment' does not have multipayments.");
        }
    }
}