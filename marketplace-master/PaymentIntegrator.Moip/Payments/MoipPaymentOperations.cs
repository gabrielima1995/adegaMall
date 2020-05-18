using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Moip.Common;
using PaymentIntegrator.Moip.Common.Models;
using PaymentIntegrator.Moip.Orders;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace PaymentIntegrator.Moip.Payments
{
    public class MoipPaymentOperations : IPaymentOperations
    {
        public OperationResult<TransferDto> CreateTransferForPayable(string payableId)
        {
            var orderDetailsRequest = new MoipRequest($"orders/{payableId}", Method.GET);
            var orderDetailsResult = MoipClient.SendOAuthRequest(orderDetailsRequest);

            if (orderDetailsResult.StatusCode != HttpStatusCode.OK)
                return new OperationResult<TransferDto>("Error", "Error getting order details");

            dynamic orderDetails = JsonConvert.DeserializeObject(orderDetailsResult.Content);

            foreach (var escrow in orderDetails.escrows)
            {
                var releaseEscrowRequest = new MoipRequest($"escrows/{escrow.id}/release", Method.POST);
                releaseEscrowRequest.AddJsonBody(new { });
                var releaseEscrowResult = MoipClient.SendOAuthRequest(releaseEscrowRequest);
                if (releaseEscrowResult.StatusCode == HttpStatusCode.Created || releaseEscrowResult.StatusCode == HttpStatusCode.OK)
                {
                    dynamic releaseEscrowObject = JsonConvert.DeserializeObject(releaseEscrowResult.Content);

                    var result = new TransferDto()
                    {
                        Id = (string)escrow.id,
                        Status = (string)releaseEscrowObject.status,
                    };

                    return result;
                }
            }

            return null;
        }

        public OperationResult<PayableDto> GetPayableDetails(string transactionId, string recipientId, int installment)
        {
            if (installment > 1)
                return new OperationResult<PayableDto>("Error", "Moip escrows do not work for installments");

            var multiPaymentDetailsRequest = new MoipRequest($"multipayments/{transactionId}", Method.GET);
            var multiPaymentDetailsResult = MoipClient.SendOAuthRequest(multiPaymentDetailsRequest);
            if (multiPaymentDetailsResult.StatusCode == HttpStatusCode.OK)
            {
                dynamic paymentDetails = JsonConvert.DeserializeObject(multiPaymentDetailsResult.Content);

                foreach (var payment in paymentDetails.payments)
                {
                    if (payment.receivers[1].moipAccount.id != recipientId)
                        continue;

                    var orderId = payment._links.order.title;

                    var orderDetailsRequest = new MoipRequest($"orders/{orderId}", Method.GET);
                    var orderDetailsResult = MoipClient.SendOAuthRequest(orderDetailsRequest);

                    if (orderDetailsResult.StatusCode != HttpStatusCode.OK)
                        return new OperationResult<PayableDto>("Error", "Error getting order details");

                    dynamic orderDetails = JsonConvert.DeserializeObject(orderDetailsResult.Content);

                    var escrow = MoipOrder.CreateEscrow(orderDetails, recipientId);
                    return escrow;
                }
            }

            return new OperationResult<PayableDto>("Error", "Error getting order details");
        }

        public OperationResult<PayableDto> GetPayableDetails(string payableId)
        {
            var orderDetailsRequest = new MoipRequest($"orders/{payableId}", Method.GET);
            var orderDetailsResult = MoipClient.SendOAuthRequest(orderDetailsRequest);

            if (orderDetailsResult.StatusCode != HttpStatusCode.OK)
                return new OperationResult<PayableDto>("Error", "Error getting order details");

            dynamic orderDetails = JsonConvert.DeserializeObject(orderDetailsResult.Content);
            var escrow = MoipOrder.CreateEscrow(orderDetails, (string)orderDetails.receivers[1].moipAccount.id);
            return escrow;
        }

        public OperationResult<ITransaction> GetTransaction(string transactionId)
        {
            var paymentId = (PaymentId)transactionId;
            var request = GetPaymentRequest.Create(paymentId);
            var result = MoipClient.Send(request);

            if (result.IsFailure)
                return new OperationResult<ITransaction>("Error", "Unable to get payment details. " + result.GetErrorString());

            var transaction = new MoipTransaction(result);
            return transaction;
        }

        public OperationResult<ITransaction> Pay(IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions, IAddress buyerAddress = null)
        {
            var multiOrderRequest = CreateMultiOrderRequest.Create(orders, customer, paymentOptions);
            var multiOrderResult = MoipClient.Send(multiOrderRequest);

            if (multiOrderResult.IsFailure)
                return new OperationResult<ITransaction>("", "Não conseguimos configurar os pedidos para pagamento. " + multiOrderResult.GetErrorString());

            var multiPaymentRequest = CreateMultiPaymentRequest.Create(multiOrderResult.Data.Id, orders, customer, paymentOptions);
            var multiPaymentResult = MoipClient.Send(multiPaymentRequest);

            if (multiPaymentResult.IsFailure)
                return new OperationResult<ITransaction>("", "Erro ao criar pagamentos na Wirecard. " + multiPaymentResult.GetErrorString());

            if (multiPaymentResult.Data.PaymentIsDenied)
                return new OperationResult<ITransaction>("", "O pagamento não foi autorizado. Entre em contato com a operadora do seu cartão.");

            /* Na maior parte dos casos, o pagamento recém-criado retorna status de análise,
             * e é aprovado ou rejeitado segundos depois, mesmo a documentação dizendo que pode
             * levar até 48h para análise.
             * 
             * Visando entregar o resultado final para o comprador durante o processo de compra
             * ao invés de enviar um email logo mais, faz sentido aguardar e tentar novamente
             * algumas vezes para ver se pega o resultado final.
             */

            MoipTransaction transaction = null;
            var maxAttempts = paymentOptions.Method == PaymentMethod.CreditCard ? 3 : 1;

            for (int tryCount = 0; tryCount < maxAttempts; tryCount++)
            {
                Thread.Sleep(1500);

                var paymentStatusRequest =  GetPaymentRequest.Create((PaymentId)multiPaymentResult.Data.Id);
                var paymentStatusResponse = MoipClient.Send(paymentStatusRequest);

                transaction = new MoipTransaction(paymentStatusResponse);

                if (!transaction.IsPendingPayment())
                    break;
            }

            return transaction;
        }

        public OperationResult<ITransaction> Refund(IOrder order, ITransaction transaction, decimal totalRefund)
        {
            var multiPaymentDetailsRequest = new MoipRequest($"multipayments/{transaction.TransactionId}", Method.GET);
            var multiPaymentDetailsResult = MoipClient.SendOAuthRequest(multiPaymentDetailsRequest);
            if (multiPaymentDetailsResult.StatusCode == HttpStatusCode.OK)
            {
                dynamic paymentDetails = JsonConvert.DeserializeObject(multiPaymentDetailsResult.Content);

                foreach (var payment in paymentDetails.payments)
                {
                    var orderId = payment._links.order.title;

                    var orderDetailsRequest = new MoipRequest($"orders/{orderId}", Method.GET);
                    var orderDetailsResult = MoipClient.SendOAuthRequest(orderDetailsRequest);

                    if (orderDetailsResult.StatusCode != HttpStatusCode.OK)
                        return new OperationResult<ITransaction>("Error", "Error getting order details");

                    dynamic orderDetails = JsonConvert.DeserializeObject(orderDetailsResult.Content);

                    if (orderDetails.ownId == order.OrderId && orderDetails.status == "PAID")
                    {
                        var refundRequest = new MoipRequest($"orders/{orderDetails.id}/refunds", Method.POST);
                        var refundObject = MoipOrder.CreateRefund(order, transaction, totalRefund);
                        refundRequest.AddJsonBody(refundObject);

                        var refundResult = MoipClient.SendOAuthRequest(refundRequest);

                        if (refundResult.StatusCode == HttpStatusCode.OK || refundResult.StatusCode == HttpStatusCode.Created)
                        {
                            dynamic refund = JsonConvert.DeserializeObject(refundResult.Content);
                            decimal total = ((int)refund.amount.total).FromCents();
                            decimal startupFee = ((int)refund.receiversDebited[0].amount).FromCents();
                            decimal gatewayFee = total - startupFee - ((int)refund.receiversDebited[1].amount).FromCents();
                            transaction.SetRefundedValues(total, startupFee, gatewayFee);

                            return new OperationResult<ITransaction>(transaction);
                        }
                    }
                }
            }

            return new OperationResult<ITransaction>("Error", "Unable to refund payment");
        }

        public OperationResult<TransferDto> GetTransferDetails(string transferId)
            => throw new NotImplementedException();

        public OperationResult<ITransaction> Pay(IPaymentItem paymentItem, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress buyerAddress = null)
            => throw new NotImplementedException();

        public OperationResult<ITransaction> Pay(IList<IPaymentItem> paymentItems, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress buyerAddress = null)
            => throw new NotImplementedException();

        public OperationResult<ITransaction> Pay(IOrder order, ICustomer customer, IPaymentOptions paymentOptions, IAddress buyerAddress = null)
            => throw new NotImplementedException();

        public OperationResult<IReadOnlyCollection<TransferDto>> ListWithdraws(string supplierId, string supplierApiToken)
        {
            var result = MoipClient.SendOAuthRequest(new MoipRequest("transfers", Method.GET), supplierApiToken);

            if (result.StatusCode != HttpStatusCode.OK)
                return new ErrorDictionary("", "Erro ao listar as transferências no gateway de pagamentos.");

            var json = JsonConvert.DeserializeAnonymousType(result.Content, new
            {
                transfers = new[]
                {
                    new
                    {
                        id = "TRA-EG129FIMT6T4",
                        amount = 443613,
                        status = "COMPLETED",
                        role = "PAYER",
                        fee = 0,
                        updatedAt = DateTime.Now,
                        transferInstrument = new
                        {
                            method = "BANK_ACCOUNT",
                            bankAccount = new
                            {
                                id = "BKA-MHUFOWAB6Z67",
                                bankNumber = "001",
                                agencyNumber = "6816",
                                agencyCheckNumber = "0",
                                accountNumber = "1124568",
                                accountCheckNumber = "0",
                                type = "CHECKING",
                                holder = new
                                {
                                    fullname = "Fulano de Tal",
                                    taxDocument = new
                                    {
                                        type = "CNPJ",
                                        number = "11.196.366/0001-02"
                                    }
                                }
                            }
                        },
                        cancellationDetails = new
                        {
                            code = 5,
                            description = "CPF/CNPJ nao pertence ao titular da conta",
                            cancelledBy = "BANK"
                        }
                    }
                }
            });

            return json.transfers
                .Where(transfer => transfer.role == "PAYER" && transfer.transferInstrument.method == "BANK_ACCOUNT")
                .Select(transfer => new TransferDto
                {
                    Id = transfer.id,
                    Amount = transfer.amount,
                    Status = transfer.status,
                    SourceType = transfer.role,
                    Fee = transfer.fee,
                    DateCreated = transfer.updatedAt,
                    BankAccount = new TransferDto.TransferToBankAccount
                    {
                        Id = transfer.transferInstrument.bankAccount.id,
                        BankCode = transfer.transferInstrument.bankAccount.bankNumber,
                        Branch = transfer.transferInstrument.bankAccount.agencyNumber,
                        BranchCheckNumber = transfer.transferInstrument.bankAccount.agencyCheckNumber,
                        Account = transfer.transferInstrument.bankAccount.accountNumber,
                        AccountCheckNumber = transfer.transferInstrument.bankAccount.accountCheckNumber,
                        AccountType = transfer.transferInstrument.bankAccount.type,
                        AccountableName = transfer.transferInstrument.bankAccount.holder.fullname,
                        AccountableDocumentType = transfer.transferInstrument.bankAccount.holder.taxDocument.type,
                        AccountableDocumentNumber = transfer.transferInstrument.bankAccount.holder.taxDocument.number
                    },
                    ErrorMessage = transfer.cancellationDetails?.description ?? null
                }).ToList();
        }
    }
}