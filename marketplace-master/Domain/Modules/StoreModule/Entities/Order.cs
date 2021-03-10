using Application.Config;
using Domain.Entities.DisputeModule;
using Domain.Events;
using Domain.Interfaces;
using Domain.Services.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Suppliers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Domain.Modules.StoreModule.Coupons;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Pedido de produtos de um comprador para um vendedor.
    /// </summary>
    public class Order : BaseEntity<Guid>, IOrder
    {
        /// <summary>
        /// Comprador que fez o pedido.
        /// </summary>
        public Buyer Buyer { get; set; }

        /// <summary>
        /// Vendedor para quem foi feito o pedido.
        /// </summary>
        public Seller Seller { get; set; }

        /// <summary>
        /// Endereço de entrega do pedido.
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// Método de entrega do pedido.
        /// </summary>
        public ShippingMethod ShippingMethod { get; set; }

        /// <summary>
        /// Valor bruto do pedido (Valor bruto dos produtos).
        /// </summary>
        public decimal SubTotal => Products?.Sum(i => i.SubTotal) ?? 0;

        /// <summary>
        /// Valor do frete do pedido.
        /// </summary>
        public decimal Shipping { get; set; } // => Products?.Sum(i => i.Shipping) ?? 0;

        /// <summary>
        /// Prazo do Correios para o recebimento do produto
        /// </summary>
        public DateTime? DeliveryDate { get; set; } 

        /// <summary>
        /// Valor do desconto no pedido.
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Valor da taxa da startup (caso for marketplace).
        /// </summary>
        public decimal StartupFee { get; private set; }

        /// <summary>
        /// Juros de parcelamento e taxa de antecipação
        /// </summary>
        public decimal InstallmentFees { get; set; }

        /// <summary>
        /// Indica se o fornecedor é responsável pelas taxas da transação (caso for marketplace).
        /// </summary>
        public bool ProcessingGatewayFee { get; private set; }

        /// <summary>
        /// Identificador da regra de split (caso for marketplace).
        /// </summary>
        public string SplitId { get; private set; }

        /// <summary>
        /// Valor líquido do pedido (Valor dos produtos + Frete - Desconto).
        /// </summary>
        [NotMapped]
        public decimal Amount => SubTotal + Shipping + InstallmentFees - Discount;

        /// <summary>
        /// Tipo de pagamento do produto.
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }

        [NotMapped]
        public string PaymentMethodString => PaymentMethod.ToString();

        /// <summary>
        /// Status do pedido.
        /// </summary>
        public OrderStatus Status { get; private set; }

        /// <summary>
        /// Produtos do pedido.
        /// </summary>
        public IList<OrderProduct> Products { get; set; }

        /// <summary>
        /// Transação de pagamento do pedido.
        /// </summary>
        public OrderTransaction Transaction { get; set; }

        /// <summary>
        /// ID da transação de pagamento do pedido.
        /// </summary>
        [ForeignKey("Transaction")]
        public Guid? Transaction_Id { get; set; }

        /// <summary>
        /// Id da disputa relacionada ao pedido, se houver
        /// </summary>
        [ForeignKey("Dispute")]
        public Guid? Dispute_Id { get; private set; }

        public Dispute Dispute { get; private set; }

        public void SetDispute(Dispute dispute)
        {
            Dispute = dispute;
            Dispute_Id = dispute?.Id;
        }

        /// <summary>
        /// Cupom de desconto usado no pedido.
        /// </summary>
        public Coupon Coupon { get; private set; }

        /// <summary>
        /// Histórico do pedido.
        /// </summary>
        public IList<OrderHistory> Histories { get; set; }

        /// <summary>
        /// Avaliações do pedido (pelo vendedor e pelo comprador).
        /// </summary>
        public IList<OrderReview> Reviews { get; set; }

        /// <summary>
        /// Movimentações de estoque associadas ao pedido.
        /// </summary>
        public IList<StockTransaction> StockTransactions { get; set; }

        /// <summary>
        /// Movimentações de locação associadas ao pedido
        /// </summary>
        public IList<RentalTransaction> RentalTransactions { get; set; }

        /// <summary>
        /// Itens do pedido.
        /// </summary>
        public IList<IPaymentItem> Items => Products.Cast<IPaymentItem>().ToList();

        /// <summary>
        /// Dados do fornecedor (caso for marketplace).
        /// </summary>
        public ISupplier Supplier => Seller;

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public string OrderId => Id.ToString();

        public Cart Cart { get; set; }

        [ForeignKey("Cart")]
        public Guid? CartId { get; set; }

        public Guid? AntifraudTransactionId { get; set; }

        public bool IsPaid { get; set; }

        public ProductBusinessModel BusinessModel { get; set; }

        public OrderRentalInfo RentalInfo { get; set; }

        public DateTime? Deadline { get; private set; }

        public DateTime? AutoCompletionDate { get; private set; }

        public bool IsRental => BusinessModel == ProductBusinessModel.Rental;

        public bool IsSale => BusinessModel == ProductBusinessModel.Sale;

        public string TrackingCode { get; private set; }

        /// <summary>
        /// Informações ou instruções adicionais do pedido.
        /// </summary>
        public string ExtraInfo { get; set; }

        [NotMapped]
        public ErrorDictionary Errors { get; }

        public Order()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Products = new List<OrderProduct>();
            Histories = new List<OrderHistory>();
            Reviews = new List<OrderReview>();
            StockTransactions = new List<StockTransaction>();
            RentalTransactions = new List<RentalTransaction>();
            RentalInfo = new OrderRentalInfo();
            Errors = new ErrorDictionary();
        }

        public Order(
            Guid cartId, PaymentMethod method,
            Buyer buyer, Seller seller, Address shippingAddress,
            ShippingMethod shippingMethod, decimal shipping, ProductBusinessModel businessModel,
            bool isRental,
            Coupon coupon, decimal discount)
            : this()
        {
            CartId = cartId;
            Buyer = buyer;
            Seller = seller;

            Shipping = shipping;
            ShippingMethod = shippingMethod;

            if (ShippingMethod != ShippingMethod.OnSitePickup)
                ShippingAddress = shippingAddress;
            else
                ShippingAddress = seller.Address;

            PaymentMethod = method;
            Status = OrderStatus.Processed;
            ChangeStatus(OrderStatus.Processed);

            BusinessModel = businessModel;

            Coupon = coupon;
            Discount = discount;

            AddDomainEvent(new NewOrderRegisteredEvent(Id, Seller.Id, isRental));
        }

        /// <summary>
        /// Atribui o valor da taxa da startup, caso for marketplace.
        /// </summary>
        /// <param name="startupFee">Valor da taxa para a startup.</param>
        public void SetStartupFee(decimal startupFee)
        {
            StartupFee = startupFee;
        }

        /// <summary>
        /// Atribui o identificador da regra de split, caso for marketplace.
        /// </summary>
        /// <param name="splitId">Identificador da regra de split.</param>
        public void SetSplitId(string splitId)
        {
            SplitId = splitId;
        }

        public decimal GetSplitFees()
        {
            var percentFee = Seller.GetCurrentPaymentPercentFee();
            var amountFee = Seller.GetCurrentPaymentAmountFee();

            decimal fee;

            if (MaaSConfig.Current.Payment.Common.EnableShippingSplitToMarketplace)
            {
                fee = (Amount - InstallmentFees - Shipping) * (percentFee / 100M);
                fee += Shipping;
            }
            else fee = amountFee + ((Amount - InstallmentFees) * (percentFee / 100M));

            if (InstallmentFees > 0)
            {
                if (ProcessingGatewayFee) // Seller shares gateway fee
                {
                    var startupInstallmentFee = InstallmentFees * (fee / Amount); // Split proportionally
                    fee += startupInstallmentFee;
                }
                else fee += InstallmentFees; // Startup receives 100% of installment fees
            }

            fee = Math.Round(fee, 2);
            return fee;
        }

        /// <summary>
        /// Indica se o fornecedor é responsável pelas taxas da transação (caso for marketplace).
        /// </summary>
        /// <param name="processingGatewayFee">Verdadeiro se o fornecedor é responsável pelas taxas da transação</param>
        public void SetProcessingGatewayFee(bool processingGatewayFee)
        {
            ProcessingGatewayFee = processingGatewayFee;
        }

        /// <summary>
        /// Adiciona um produto para o pedido.
        /// </summary>
        /// <param name="item">Dados do produto no carrinho de compras.</param>
        public Order AddProduct(CartItem item, RentalService rentalService)
        {
            var product = new OrderProduct(item, rentalService);

            if (product.Errors.HasErrors)
                Errors.Merge(product.Errors);

            Products.Add(product);
            return this;
        }

        /// <summary>
        /// Muda o status do pedido.
        /// </summary>
        /// <param name="newStatus">Novo status.</param>
        /// <param name="person">Pessoa responsável, quando for o caso.</param>
        /// <param name="additionalInfo">Observação.</param>
        /// <returns></returns>
        public Order ChangeStatus(OrderStatus newStatus, Person person = null, string additionalInfo = null, bool noThrowEvent = false)
        {
            if (Status != newStatus)
            {
                Status = newStatus;

                if(!noThrowEvent)
                    AddDomainEvent(new OrderStatusChangedEvent(this, newStatus, person, additionalInfo));
                AddHistoryMessage(additionalInfo, person);

                if (newStatus == OrderStatus.CompletedPayment)
                    IsPaid = true;
                else if (newStatus == OrderStatus.DeliveryOnTheWay)
                    TrackingCode = additionalInfo;
            }

            return this;
        }

        /// <summary>
        /// Adiciona uma mensagem ao histórico do pedido para fins de registro/auditoria.
        /// </summary>
        public Order AddHistoryMessage(string message, Person person = null)
        {
            Histories.Add(new OrderHistory(Status, person, message));
            return this;
        }

        /// <summary>
        /// Valida a avaliação do pedido.
        /// </summary>
        /// <param name="review">Dados da avaliação do pedido.</param>
        /// <param name="shippingMethodAdapter">Adapter do método de entrega do pedido.</param>
        /// <returns></returns>
        private ErrorDictionary CheckReview(OrderReview review, IShippingMethodAdapter shippingMethodAdapter)
        {
            var errors = new ErrorDictionary();

            if (!shippingMethodAdapter.IsOrderCompleted(this))
                errors.Add("", CultureInfo.CurrentCulture.GetStringLocalized("ORDER_REVIEW_INVALID"));

            if (review.Reviewer.Id != Seller.Id && review.Reviewer.Id != Buyer.Id)
                errors.Add("", CultureInfo.CurrentCulture.GetStringLocalized("ORDER_REVIEW_INVALID"));

            if (Reviews.Any(i => i.Reviewer.Id == review.Reviewer.Id))
                errors.Add("", CultureInfo.CurrentCulture.GetStringLocalized("ORDER_ALREADY_REVIEWED"));

            return errors;
        }

        /// <summary>
        /// Salva uma avaliação para o pedido.
        /// </summary>
        /// <param name="reviewer">Dados do avaliador.</param>
        /// <param name="rate">Nota da avaliação (0 a 100).</param>
        /// <param name="description">Comentário da avaliação.</param>
        /// <param name="shippingMethodAdapter">Adapter do método de entrega do pedido.</param>
        /// <returns></returns>
        public OperationResult Review(Person reviewer, int rate, string description, IShippingMethodAdapter shippingMethodAdapter)
        {
            var review = new OrderReview(this, reviewer, rate, description);
            var validation = CheckReview(review, shippingMethodAdapter);

            if (!validation.HasErrors)
            {
                Reviews.Add(review);

                if (review.ByBuyer)
                    Seller.Review(review.Rate);

                if (review.BySeller)
                    Buyer.Review(review.Rate);
            }

            return validation;
        }

        /// <summary>
        /// Pedido entregue
        /// </summary>
        public void Delivered()
        {
            foreach (var product in Products)
                product.Delivered();
        }

        /// <summary>
        /// Pedido devolvido
        /// </summary>
        public void Returned()
        {
            foreach (var product in Products)
                product.Returned();

            if (IsRental)
            {
                RentalInfo.WasFullyReturned = true;
                RentalInfo.FullyReturnedAt = DateTime.UtcNow;
            }
        }

        public bool IsLiable()
        {
            return Seller.IsSellerLiable();
        }

        public void SetDeadlineAndAutoCompletionDates(DateTime quoteOrRentalDeadline)
        {
            var autoCompletionDate = quoteOrRentalDeadline;

            if (autoCompletionDate < DateTime.Today)
                autoCompletionDate = DateTime.Today;

            if (MaaSConfig.Current.Payment.Common.DeadlineAutoExtensionDays > 0)
                autoCompletionDate = autoCompletionDate.AddDays(MaaSConfig.Current.Payment.Common.DeadlineAutoExtensionDays);

            Deadline = quoteOrRentalDeadline;
            AutoCompletionDate = autoCompletionDate;
        }

        public void ExtendAutoCompletionDateForDispute()
        {
            var extendedDate = DateTime.UtcNow.AddDays(MaaSConfig.Current.Payment.Common.DeadlineAutoExtensionDays);
            if (AutoCompletionDate == null || AutoCompletionDate < extendedDate)
                AutoCompletionDate = extendedDate;
        }
    }
}