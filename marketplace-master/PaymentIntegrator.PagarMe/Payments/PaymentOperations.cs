using Framework;
using Newtonsoft.Json;
using PagarMe;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Suppliers;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PaymentIntegrator.PagarMe.Payments
{
    public class PaymentOperations : IPaymentOperations
    {
        public OperationResult<ITransaction> GetTransaction(string transactionId)
        {
            var request = new SimpleImpl.GetTransactionRequest(transactionId);
            var client = new SimpleImpl.Client();
            var response = client.Execute(request);
            var data = JsonConvert.DeserializeObject<SimpleImpl.Models.Transaction>(response.Content);

            return new TransactionResponse(new Transaction
            {
                Id = data.Id.ToString(),
                Status = SimpleImpl.Helpers.ParseTransactionStatus(data.Status),
                StatusReason = data.StatusReason,
                Amount = data.Amount,
                Cost = data.Cost,
                PostbackUrl = data.PostbackUrl,
                BoletoUrl = data.BoletoUrl,
                BoletoBarcode = data.BoletoBarcode
            });
        }

        public OperationResult<ITransaction> Pay(IPaymentItem paymentItem, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress address = null)
        {
            return Pay(new OrderData(paymentItem, supplier), customer, paymentOptions, address);
        }

        public OperationResult<ITransaction> Pay(IList<IPaymentItem> paymentItems, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress address = null)
        {
            return Pay(new OrderData(paymentItems, supplier), customer, paymentOptions, address);
        }

        public OperationResult<ITransaction> Pay(IOrder order, ICustomer customer, IPaymentOptions paymentOptions, IAddress address = null)
        {
            return Pay(new List<IOrder>(1) { order }, customer, paymentOptions, address);
        }

        public OperationResult<ITransaction> Pay(IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions, IAddress address = null)
        {
            var client = new PagarMeClient();
            var request = new TransactionRequest(client, orders, customer, address, paymentOptions);
            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
                return new OperationResult<ITransaction>(validationRequest);

            var validation = client.Save(request);

            if (validation.OK)
            {
                var transaction = new TransactionResponse(request, orders);
                return new OperationResult<ITransaction>(transaction);
            }

            return new OperationResult<ITransaction>(validation.Errors);
        }

        public OperationResult<ITransaction> Refund(IOrder order, ITransaction transaction, decimal totalRefund)
        {
            var client = new PagarMeClient();

            var transactionRefund = new TransactionCustom(client.GetService())
            {
                Id = transaction.TransactionId
            };

            var startupFeedPerc = order.StartupFee / order.Amount;
            var startupFeeRefund = Math.Round(totalRefund * startupFeedPerc, 2);
            var amountRefund = totalRefund - startupFeeRefund;

            var splitRules = new[]
            {
                new SplitRuleCustom(client.GetService())
                {
                    Id = transaction.StartupSplitId,
                    Amount = startupFeeRefund.ToCents(),
                    ChargeProcessingFee = true
                },
                new SplitRuleCustom(client.GetService())
                {
                    Id = order.SplitId,
                    Amount = amountRefund.ToCents(),
                    ChargeProcessingFee = order.ProcessingGatewayFee
                }
            };

            var validation = client.Execute(() => transactionRefund.Refund(totalRefund.ToCents(), splitRules));

            if (validation.OK)
            {
                transaction.SetRefundedValues(
                    transaction.RefundedAmount + totalRefund,
                    transaction.RefundedStartupFee + startupFeeRefund,
                    transaction.RefundedGatewayFee + (transactionRefund.Cost / 100M));
                return new OperationResult<ITransaction>(transaction);
            }

            return new OperationResult<ITransaction>(validation.Errors);
        }

        public OperationResult<PayableDto> GetPayableDetails(string payableId)
        {
            var service = new PagarMeClient().GetService();

            try
            {
                var payable = service.Payables.Find(payableId);

                if (payable == null)
                    return null;

                else return MapPayableToDto(payable);
            }
            catch /*Exceção é gerada se payable não for encontrado*/
            {
                return null;
            }
        }

        public OperationResult<PayableDto> GetPayableDetails(string transactionId, string recipientId, int installment)
        {
            var result = new PayableHelper().GetPayableDetails(transactionId,installment);
            return result;

            //forma antiga utilizando a sdk da pagarme
            //var service = new PagarMeClient().GetService();
            //var parsedTransactionId = int.Parse(transactionId);
            //var result = installment == 0
            //    ? service.Payables.Find(new Payable { TransactionId = parsedTransactionId, RecipientId = recipientId })
            //    : service.Payables.Find(new Payable { TransactionId = parsedTransactionId, RecipientId = recipientId, Installment = installment });
            //return MapPayableToDto(result);
        }

        public OperationResult<TransferDto> GetTransferDetails(string transferId)
        {
            var service = new PagarMeClient().GetService();
            var result = service.Transfers.Find(transferId);

            if (result == null)
                return null;

            //Defective Library PaymentDate cast workaround--
            var dict = result.ToDictionary(PagarMeUtils.FullSerialization);
            //--

            var fundingDate = (dict["funding_date"] != null) ? DateTime.Parse(dict["funding_date"].ToString()) : DateTime.MaxValue;
            var estimatedDate = (dict["funding_estimated_date"] != null) ? DateTime.Parse(dict["funding_estimated_date"].ToString()) : DateTime.MaxValue;

            return new TransferDto
            {
                Amount = result.Amount,
                DateCreated = result.DateCreated,
                Fee = result.Fee,
                FundingDate = fundingDate,
                FundingEstimatedDate = estimatedDate,
                Id = result.Id,
                SourceId = result.RecipientId,
                TargetId = result.RecipientId,
                Type = result.Type.ToString(),
                Status = result.Status.ToString(),
            };
        }

        private PayableDto MapPayableToDto(Payable payable)
        {
            //Defective Library PaymentDate cast workaround--
            var dict = payable.ToDictionary(PagarMeUtils.FullSerialization);
            //payable.PaymentDate = //dict["payment_date"].ToString();

            var dictInst = dict["installment"];
            int installments = 0;

            if (dictInst != null)
                installments = int.Parse(dictInst.ToString());
            //--

            return new PayableDto()
            {
                Amount = payable.Amount,
                AnticipationFee = payable.AnticipationFee,
                BulkAnticipationId = payable.BulkAnticipationId,
                Installment = installments,
                PayableType = payable.Type.ToString(),
                OriginalPaymentDate = payable.PaymentDate,
                PaymentDate = payable.PaymentDate,
                PaymentMethod = payable.PaymentMethod.ToString(),
                RecipientId = payable.RecipientId,
                SplitRuleId = payable.SplitRuleId,
                TransactionId = payable.TransactionId,
                PayableId = payable.Id,
                Status = payable.Status.ToString(),
                Fee = payable.Fee
            };
        }

        public OperationResult<TransferDto> CreateTransferForPayable(string payableId)
        {
            var payable = GetPayableDetails(payableId).Result;

            if (payable.Amount <= PagarMeConfig.TransferCostCents)
                return new ErrorDictionary("", $"O valor da transferência ({payable.Amount}) não pode ser menor que a taxa de transferência ({PagarMeConfig.TransferCostCents}).");

            var actualAmount = payable.Amount - PagarMeConfig.TransferCostCents;

            var recipientTransfers = new PagarMeClient().GetService()
                .Transfers.FindAll(new CustomTransfer { Amount = actualAmount, RecipientId = payable.RecipientId }).ToList();

            foreach (var existingTransfer in recipientTransfers)
            {
                if (existingTransfer.IsAssociatedToPayable(payableId))
                    return new OperationResult<TransferDto>(MapTransferToDto(existingTransfer));
            }

            var newTransfer = new CustomTransfer
            {
                Amount = actualAmount,
                RecipientId = payable.RecipientId
            };

            newTransfer.SetPayableIdMetadata(payableId);

            try
            {
                newTransfer.Save();
                return new OperationResult<TransferDto>(MapTransferToDto(newTransfer));
            }
            catch (PagarMeException e)
            {
                var mappedErrors = Common.PagarMeHelper.ToValidation(e.Error);
                return new OperationResult<TransferDto>(mappedErrors);
            }
            catch (Exception e)
            {
                return new OperationResult<TransferDto>("error", e.ToString());
            }
        }

        private TransferDto MapTransferToDto(Transfer transfer)
        {
            var dict = transfer.ToDictionary(PagarMeUtils.FullSerialization);

            var fundingEstimatedDate = DateTime.Parse(dict["funding_estimated_date"].ToString());
            DateTime? fundingDate = null;
            if (dict["funding_date"] != null)
                fundingDate = DateTime.Parse(dict["funding_date"].ToString());

            return new TransferDto()
            {
                Amount = transfer.Amount,
                DateCreated = transfer.DateCreated,
                Fee = transfer.Fee,
                FundingDate = fundingDate,
                FundingEstimatedDate = fundingEstimatedDate,
                Id = transfer.Id,
                SourceId = transfer.RecipientId,
                TargetId = transfer.RecipientId,
                Status = transfer.Status.ToString(),
                Type = transfer.Type.ToString(),
            };
        }

        public OperationResult<IReadOnlyCollection<TransferDto>> ListWithdraws(string supplierId, string supplierApiToken)
        {
            var response = new RestClient("https://api.pagar.me/1") { Authenticator = new HttpBasicAuthenticator("asdasda", "x") }
                .Get(new RestRequest("transfers").AddQueryParameter("recipient_id", supplierId));

            if (response.StatusCode != HttpStatusCode.OK)
                return new ErrorDictionary("", "Erro ao listar as transferências no gateway de pagamentos.");

            var json = JsonConvert.DeserializeAnonymousType(response.Content, new[]
            {
                new
                {
                    id = 65485,
                    amount = 100,
                    type = "ted",
                    status = "transferred",
                    source_type = "recipient",
                    source_id = "re_cix7pxz6f02ppcv6dn4ckcrcc",
                    target_type = "bank_account",
                    target_id = "17346045",
                    fee = 367,
                    date_created = DateTime.Now,
                    bank_account = new
                    {
                        id = 17346045,
                        bank_code = "000",
                        agencia = "0000",
                        agencia_dv = "2",
                        conta = "000000",
                        conta_dv = "0",
                        type = "conta_corrente",
                        document_type = "cpf",
                        document_number = "07235698403",
                        legal_name = "Fulano de Tal",
                        charge_transfer_fees = true
                    }
                }
            });

            return json
                .Where(transfer => transfer.type == "ted" && transfer.source_type == "recipient" && transfer.target_type == "bank_account")
                .Select(transfer => new TransferDto
                {
                    Id = transfer.id.ToString(),
                    Amount = transfer.amount,
                    Type = transfer.type,
                    Status = transfer.status,
                    SourceType = transfer.source_type,
                    SourceId = transfer.source_id,
                    TargetType = transfer.target_type,
                    TargetId = transfer.target_id,
                    Fee = transfer.bank_account.charge_transfer_fees ? transfer.fee : 0,
                    DateCreated = transfer.date_created,
                    BankAccount = new TransferDto.TransferToBankAccount
                    {
                        Id = transfer.bank_account.id.ToString(),
                        BankCode = transfer.bank_account.bank_code,
                        Branch = transfer.bank_account.agencia,
                        BranchCheckNumber = transfer.bank_account.agencia_dv,
                        Account = transfer.bank_account.conta,
                        AccountCheckNumber = transfer.bank_account.conta_dv,
                        AccountType = transfer.bank_account.type,
                        AccountableName = transfer.bank_account.legal_name,
                        AccountableDocumentType = transfer.bank_account.document_type,
                        AccountableDocumentNumber = transfer.bank_account.document_number
                    }
                }).ToList();
        }
    }

    internal class TransactionPayables : global::PagarMe.Base.Model
    {
        public TransactionPayables(string transactionId) : this(null, transactionId)
        {
        }

        public TransactionPayables(PagarMeService service, string transactionId) : base(service)
        {
            endpointBase = $"/transaction/{transactionId}/payables";
        }

        private readonly string endpointBase;
        protected override string Endpoint => endpointBase;
    }
}