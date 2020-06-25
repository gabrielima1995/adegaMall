using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Status de um pedido.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Pedido processado.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_PROCESSED")]
        Processed = 99,

        /// <summary>
        /// Pedido pendente de ser enviado para antifraude.
        /// </summary>
        [DescriptionLocalized("ANTIFRAUD_REQUEST_PENDING")]
        AntifraudRequestPending = 8,

        /// <summary>
        /// Pedido enviado para o antifraude e aguardando análise
        /// </summary>
        [DescriptionLocalized("ANTIFRAUD_AWAITING_ANALISYS")]
        AntifraudAwaitingAnalisys = 9,

        /// <summary>
        /// Aprovado pelo antifraude.
        /// </summary>
        [DescriptionLocalized("ANTIFRAUD_APPROVED")]
        AntifraudApproved = 10,

        /// <summary>
        /// Recusado pelo antifraude.
        /// </summary>
        [DescriptionLocalized("ANTIFRAUD_DENIED")]
        AntifraudDenied = 11,

        /// <summary>
        /// Aguardando pagamento do comprador.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_PENDING_PAYMENT")]
        PendingPayment = 0,

        /// <summary>
        /// Pagamento realizado.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_COMPLETED_PAYMENT")]
        CompletedPayment = 1,

        /// <summary>
        /// Aguardando entrega.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_AWAITING_DELIVERY")]
        AwaitingDelivery = 2,

        /// <summary>
        /// Entrega a caminho.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_DELIVERY_ON_THE_WAY")]
        DeliveryOnTheWay = 3,

        /// <summary>
        /// Pedido entregue.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_DELIVERED")]
        Delivered = 4,

        /// <summary>
        /// Pedido cancelado.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_CANCELED")]
        Canceled = 5,

        /// <summary>
        /// Pedido rejeitado (venda devolvida ou locação rejeitada).
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_REJECTED")]
        Rejected = 6,

        /// <summary>
        /// Aguardando retirada.
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_AWAITING_PICK_UP")]
        AwaitingPickup = 7,

        /// <summary>
        /// Pedido devolvido (locação devolvida ao lojista).
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_RETURNED")]
        Returned = 12,

        /// <summary>
        /// Pedido concluído e pagamento liberado
        /// </summary>
        [DescriptionLocalized("ORDER_STATUS_FINISHED")]
        Completed = 14
    }
}