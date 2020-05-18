using Framework;
using PagarMe;
using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentIntegrator.PagarMe.Payments
{
    /// <summary>
    /// Helper para classes de pagamento.
    /// </summary>
    public static class PaymentHelper
    {
        /// <summary>
        /// Obtem regra de split para um pedido com recebedor.
        /// </summary>
        /// <param name="order">Pedido com recebedor.</param>
        public static SplitRuleCustom GenerateSellerSplitRule(this IOrder order)
        {
            if (order.Supplier == null)
                throw new ArgumentNullException(nameof(order.Supplier));

            var recipientAmount = order.Amount - order.GetSplitFees();
            order.SetProcessingGatewayFee(PagarMeConfig.RecipientChargeProcessingFee);

            return new SplitRuleCustom
            {
                Recipient = new Recipient { Id = order.Supplier.GatewayId },
                ChargeProcessingFee = PagarMeConfig.RecipientChargeProcessingFee,
                Liable = order.IsLiable(),
                Amount = recipientAmount.ToCents()
            };
        }

        /// <summary>
        /// Obtem as regras de split para vários pedidos com recebedores.
        /// </summary>
        /// <param name="orders">Pedidos com recebedores.</param>
        public static SplitRuleCustom[] GenerateSplitRulesOrNull(this IList<IOrder> orders)
        {
            var rules = orders
                .Where(i => i.Supplier != null)
                .Select(i => i.GenerateSellerSplitRule())
                .ToList();

            if (!rules.Any())
                return null;

            var startupCommission = orders.GetAmount().ToCents() - rules.Sum(i => i.Amount);
            rules.Add(new SplitRuleCustom
            {
                Recipient = new Recipient { Id = PagarMeConfig.StartupRecipient },
                ChargeProcessingFee = true,
                Liable = true,
                Amount = startupCommission
            });

            return rules.ToArray();
        }

        /// <summary>
        /// Valida um item para pagamento.
        /// </summary>
        /// <param name="paymentItem">Dados do item para pagamento.</param>
        public static ErrorDictionary Validate(this IPaymentItem paymentItem)
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(paymentItem.Item))
                errors.Add(nameof(paymentItem.Item), nameof(paymentItem.Item) + " é obrigatório.");

            if (paymentItem.Quantity <= 0)
                errors.Add(nameof(paymentItem.Quantity), nameof(paymentItem.Quantity) + " é obrigatório.");

            if (paymentItem.Price == 0 && paymentItem.PriceRequired)
                errors.Add(nameof(paymentItem.Price), nameof(paymentItem.Price) + " é obrigatório.");

            return errors;
        }

        /// <summary>
        /// Valida um pedido.
        /// </summary>
        /// <param name="order">Dados do pedido.</param>
        public static ErrorDictionary Validate(this IOrder order)
        {
            var errors = new ErrorDictionary();

            if (order.Items == null || !order.Items.Any())
            {
                errors.Add(nameof(order.Items), nameof(order.Items) + " é obrigatório.");
            }
            else
            {
                foreach (var item in order.Items)
                {
                    errors.Merge(item.Validate());
                }
            }

            if (order.Amount <= 0)
            {
                errors.Add(nameof(order.Amount), nameof(order.Amount) + " é obrigatório.");
            }

            return errors;
        }

        /// <summary>
        /// Converte um valor decimal em centavos.
        /// </summary>
        /// <param name="value">Valor em decimal.</param>
        public static int ToCents(this decimal value)
        {
            return (int)(value * 100);
        }

        /// <summary>
        /// Obtem o valor total dos pedidos.
        /// </summary>
        /// <param name="orders">Pedidos.</param>
        public static decimal GetAmount(this IList<IOrder> orders)
        {
            return orders
                .Sum(i => i.Amount);
        }

        /// <summary>
        /// Atribui o identificador da regra de split para cada pedido da lista.
        /// </summary>
        /// <param name="orders">Lista de pedidos.</param>
        /// <param name="splitRule">Regra de split.</param>
        public static void SetSplitId(this IList<IOrder> orders, SplitRuleCustom splitRule)
        {
            var order = orders.FirstOrDefault(i => i.Supplier != null && splitRule.Recipient != null && i.Supplier.GatewayId == splitRule.Recipient.Id);

            order?.SetSplitId(splitRule.Id);
        }
    }
}