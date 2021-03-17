using System;
using System.Collections.ObjectModel;

namespace PaymentIntegrator.Core.Payments
{
    /// <summary>
    /// Representa um transação no gateway de pagamento.
    /// </summary>
    public interface ITransaction
    {
        /// <summary>
        /// Identificador da transação no gateway de pagamento.
        /// </summary>
        string TransactionId { get; }

        /// <summary>
        /// Status da transação no gateway de pagamento.
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Motivo pelo qual a transação foi recusada.
        /// </summary>
        string RefuseReason { get; }

        /// <summary>
        /// Mensagem detalhada da transação no gateway de pagamento.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Valor total da transação.
        /// </summary>
        decimal Amount { get; }

        /// <summary>
        /// Valor estornado da transação.
        /// </summary>
        decimal RefundedAmount { get; }

        /// <summary>
        /// Valor da taxa da startup (caso for marketplace).
        /// </summary>
        decimal StartupFee { get; }

        /// <summary>
        /// Valor estornado da taxa da startup (caso for marketplace).
        /// </summary>
        decimal RefundedStartupFee { get; }

        /// <summary>
        /// Identificador da regra de split para a startup (caso for marketplace).
        /// </summary>
        string StartupSplitId { get; }

        /// <summary>
        /// Valor da taxa do gateway de pagamento.
        /// </summary>
        decimal GatewayFee { get; }

        /// <summary>
        /// Valor estornado da taxa do gateway de pagamento.
        /// </summary>
        decimal RefundedGatewayFee { get; }

        /// <summary>
        /// Indica se a transação já está paga.
        /// </summary>
        bool IsPaid();

        /// <summary>
        /// Indica se a transação está aguardando pagamento. Deve retornar false caso tenha sido rejeitada ou cancelada.
        /// </summary>
        bool IsPendingPayment();

        /// <summary>
        /// Indica se a transação teve seu pagamento cancelado, negado ou revertido.
        /// </summary>
        bool IsPaymentCancelledOrRejected();

        /// <summary>
        /// Indica se a transação teve seu pagamento recusado pelo serviço de antifraude.
        /// </summary>
        bool IsAntifraudRefuse();

        /// <summary>
        /// URL do sistema que receberá notificações a cada mudança no status da transação.
        /// </summary>
        string PostbackUrl { get; }

        /// <summary>
        /// URL para impressão do boleto.
        /// </summary>
        string BoletoUrl { get; }

        /// <summary>
        /// Código de barras do boleto.
        /// </summary>
        string BoletoBarCode { get; }

        /// <summary>
        /// Data de expiração do boleto.
        /// </summary>
        DateTime? BoletoExpirationDate { get; }

        int Installments { get; }

        ReadOnlyCollection<MultiPaymentItem> MultiPayments { get; }

        bool IsMultiPayment { get; }

        /// <summary>
        /// Atribui os valores estornados na transação.
        /// </summary>
        /// <param name="refundedAmount">Valor estornado da transação.</param>
        /// <param name="refundedStartupFee">Valor estornado da taxa da startup (caso for marketplace).</param>
        /// <param name="refundedGatewayFee">Valor estornado da taxa do gateway de pagamento.</param>
        void SetRefundedValues(decimal refundedAmount, decimal refundedStartupFee, decimal refundedGatewayFee);

        void SetValuesForMultiPayment(string id, decimal amount, decimal startupFee);
    }

    public class MultiPaymentItem 
    {
        public MultiPaymentItem(string orderId, decimal amount, decimal fees)
        {
            OrderId = orderId;
            Amount = amount;
            Fees = fees;
        }

        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fees { get; set; }
    }
}