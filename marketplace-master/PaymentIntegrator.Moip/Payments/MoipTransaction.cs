using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Moip.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static PaymentIntegrator.Moip.Common.Models.GetMultiPaymentResult.GetMultiPaymentResultData;

namespace PaymentIntegrator.Moip.Payments
{
    public class MoipTransaction : ITransaction
    {
        private static readonly StringComparison caseInsensitive = StringComparison.OrdinalIgnoreCase;

        public string TransactionId { get; private set; }
        public string Status { get; private set; }
        public string Message { get; private set; }
        public decimal Amount { get; private set; }
        public decimal RefundedAmount { get; private set; }
        public decimal StartupFee { get; private set; }
        public decimal RefundedStartupFee { get; private set; }
        public string StartupSplitId { get; private set; }
        public decimal GatewayFee { get; private set; }
        public decimal RefundedGatewayFee { get; private set; }
        public string PostbackUrl { get; private set; }
        public string BoletoUrl { get; private set; }
        public string BoletoBarCode { get; private set; }
        public DateTime? BoletoExpirationDate { get; private set; }
        public int Installments { get; private set; }

        public ReadOnlyCollection<MultiPaymentItem> MultiPayments { get; private set; }

        public bool IsMultiPayment => true;

        public bool IsPaid() =>
            "authorized".Equals(Status, caseInsensitive) ||
            "settled".Equals(Status, caseInsensitive) ||
            "paid".Equals(Status, caseInsensitive);

        public bool IsPendingPayment() =>
            "waiting".Equals(Status, caseInsensitive) ||
            "in_analysis".Equals(Status, caseInsensitive);

        public bool IsPaymentCancelledOrRejected() =>
            "cancelled".Equals(Status, caseInsensitive) ||
            "not_paid".Equals(Status, caseInsensitive) ||
            "reverted".Equals(Status, caseInsensitive);

        public void SetRefundedValues(decimal refundedAmount, decimal refundedStartupFee, decimal refundedGatewayFee)
        {
            RefundedAmount = refundedAmount;
            RefundedStartupFee = refundedStartupFee;
            RefundedGatewayFee = refundedGatewayFee;
        }

        public MoipTransaction(GetMultiPaymentResult multiPaymentResult)
        {
            var data = multiPaymentResult.Data;
            var primaryReceiverEntries = data.Payments.SelectMany(payment => payment.Receivers.Where(receiver => receiver.IsPrimary));

            TransactionId = data.Id;
            Status = data.Status;
            Amount = data.Amount.Total.FromCents();
            GatewayFee = data.Payments.Sum(payment => payment.Amount.Fees).FromCents();
            StartupFee = primaryReceiverEntries.Sum(receiver => receiver.Amount.Total).FromCents();
            Installments = data.InstallmentCount;
            var payments = new List<MultiPaymentItem>();
            for (int i = 0; i < data.Payments.Length; i++)
            {
                payments.Add(new MultiPaymentItem(
                            data.Payments[i]._Links.Order.Title,
                            data.Payments[i].Amount.Total.FromCents(),
                            data.Payments[i].Receivers.First(x => x.Type.Equals("PRIMARY")).Amount.Total.FromCents()));
            }
            MultiPayments = payments.AsReadOnly();
            var cancellationDetails = data.Payments.Select(payment => payment.CancellationDetails).FirstOrDefault();
            if (cancellationDetails != null)
                Message = GetCancellationDetailsMessage(cancellationDetails);

            if (data.IsBoleto)
            {
                BoletoUrl = data.BoletoUrl;
                BoletoBarCode = data.BoletoBarCode;
                BoletoExpirationDate = data.BoletoExpirationDate;
            }
        }

        public static MoipTransaction CreateRefundTransaction(dynamic refund)
        {
            var transaction = new MoipTransaction()
            {
                TransactionId = (string)refund.id,
                Status = "REFUND_COMPLETED",
                Amount = ((int)refund.Amount).FromCents(),
                Installments = 1,
            };

            return transaction;
        }

        public MoipTransaction()
        {
        }

        private string GetCancellationDetailsMessage(CancellationDetailsData cancellation)
        {
            /* Os códigos de erro abaixo estão bem explicados para os clientes (empreendedores) no artigo de Intercom:
             * http://suporte.ideianoar.com.br/pt-BR/articles/3382511-erros-de-pagamento-na-integracao-wirecard
             */

            var code = int.Parse(cancellation.Code);
            switch (code)
            {
                case 1: // Os dados digitados pelo Comprador estão incorretos: Número do Cartão, CVV ou Data de Vencimento. Solicite ao Comprador verificar os dados informados e corrigí-los para concluir a compra.
                    return "Dados informados inválidos. Você digitou algo errado durante o preenchimento dos dados do seu Cartão. Certifique-se de que está usando o Cartão correto e faça uma nova tentativa.";

                case 2: // Houve uma falha na comunicação com o Banco, o Comprador deve fazer uma nova tentativa. Solicite ao Comprador tentar novamente. Caso não consiga, sugira outra forma de pagamento.
                    return "Houve uma falha de comunicação com o Banco Emissor do seu Cartão, tente novamente.";

                case 3: // Transação não autorizada pelas políticas do Banco Emissor. Dentre os possíveis motivos estão: análise de risco, comportamento de compra ou outro motivo semelhante. O Comprador deve entrar em contato com o Banco Emissor antes de tentar novamente. Solicite ao Comprador entrar em contato com o Banco parar liberar a transação e, depois, peça para concluí-la.
                    return "O pagamento não foi autorizado pelo Banco Emissor do seu Cartão. Entre em contato com o Banco para entender o motivo e refazer o pagamento.";

                case 4: // A validade do Cartão foi excedida.Ofereça outro meio de pagamento ao Comprador. Exemplo: outro Cartão, Boleto ou Transferência.
                    return "A validade do seu Cartão expirou. Escolha outra forma de pagamento para concluir o pagamento.";

                case 5: // O Banco Emissor não autorizou a compra. Um dos motivos possíveis é a falta de limite do Cartão para concluir o pagamento. Ofereça outro meio de pagamento para a compra ser finalizada.
                    return "O pagamento não foi autorizado. Entre em contato com o Banco Emissor do seu Cartão.";

                case 6: // O pagamento já foi realizado por outra transação. Informe o Comprador que a compra já foi concluída. Caso ele não encontre o outro pagamento, peça para entrar em contato com o Vendedor.
                    return "Esse pagamento já foi realizado. Caso não encontre nenhuma referência ao pagamento anterior, por favor entre em contato com o nosso Atendimento.";

                case 7: // A transação possuía um risco muito elevado e, após os procedimentos de análise, ela foi negada.
                    return "O pagamento não foi autorizado. Para mais informações, entre em contato com o nosso atendimento.";

                case 8: // O Comprador solicitou o cancelamento da transação diretamente ao Moip. Entre em contato com o Comprador para entender o ocorrido.
                    return "Você solicitou o cancelamento da sua transação diretamente no gateway de pagamentos.";

                case 9: // O Vendedor solicitou o cancelamento da transação diretamente ao Moip.
                    return "O vendedor solicitou o cancelamento da sua transação diretamente no gateway de pagamentos.";

                case 10: // Houve uma falha na comunicação do Moip. Solicite ao Comprador tentar finalizar a compra novamente. Caso ele não consiga, informe o Moip do ocorrido.
                    return "O pagamento não pode ser processado. Por favor, tente novamente. Caso o erro persista, entre em contato com o nosso atendimento.";

                case 11: // Houve uma falha desconhecida no Banco Emissor. Informe o comprador que houve uma falha desconhecida no Banco Emissor. Caso o erro persista, entre em contato com o Moip.
                    return "Houve uma falha de comunicação com o Banco Emissor do seu Cartão, tente novamente.";

                case 12: // O Cartão foi negado e não será possível concluir a compra com este Cartão. Informe o Comprador que houve um problema em seu Cartão e ele deve entrar em contato com o Banco para entender o ocorrido.
                    return "Pagamento não autorizado para este Cartão. Entre em contato com o Banco Emissor para mais esclarecimentos.";

                case 13: // A transação possui um valor inválido para o Banco Emissor: valor total da transação está abaixo do mínimo (menor que 1 real); valor da parcela da transação está abaixo do mínimo (5 reais); Valor total da transação é muito alto (exemplo R$999.999,00). Entre em contato com o Moip para esclarecer esse aspecto da sua integração.
                    return "Pagamento não autorizado. Entre em contato com o Atendimento e informe o ocorrido.";

                case 14: // O Cartão foi negado e não será possível concluir a compra com este Cartão. Informe o Comprador que houve um problema em seu Cartão e ele deve entrar em contato com o Banco para entender o ocorrido.
                    return "Pagamento não autorizado. Houve algum problema com seu cartão. Entre em contato com o Banco Emissor para mais esclarecimentos.";

                default:
                    return "O pagamento não pode ser concluído por um motivo desconhecido pelo sistema. Tente novamente. Se o problema persistir, entre em contato com o nosso atendimento.";
            }
        }

        public void SetValuesForMultiPayment(string id, decimal amount, decimal startupFee)
        {
            TransactionId = id;
            Amount = amount;
            StartupFee = startupFee;
        }
    }
}