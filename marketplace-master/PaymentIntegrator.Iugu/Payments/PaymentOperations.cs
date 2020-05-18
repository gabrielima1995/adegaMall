using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Iugu.Invoices;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace PaymentIntegrator.Iugu.Payments
{
    public class PaymentOperations : IPaymentOperations
    {
        public OperationResult<ITransaction> GetTransaction(string transactionId)
        {
            var invoiceOperations = new InvoiceOperations();

            var getInvoiceResult = invoiceOperations.GetInvoice(transactionId);
            if (!getInvoiceResult.OK) return getInvoiceResult.Errors;

            return new OperationResult<ITransaction>(getInvoiceResult.Result);
        }

        public OperationResult<ITransaction> Pay(IPaymentItem paymentItem, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress buyerAddress = null)
        {
            return Pay(new List<IPaymentItem> { paymentItem }, customer, paymentOptions, supplier, buyerAddress);
        }

        public OperationResult<ITransaction> Pay(IList<IPaymentItem> paymentItems, ICustomer customer, IPaymentOptions paymentOptions, ISupplier supplier = null, IAddress buyerAddress = null)
        {
            var supplierValidation = Validate(supplier);
            if (supplierValidation.HasErrors)
                return supplierValidation;

            var client = GetClient(supplier);
            var request = new ChargeRequest(paymentItems, customer, buyerAddress, paymentOptions);

            var requestValidation = request.Validate();
            if (requestValidation.HasErrors) return requestValidation;

            var postResult = client.Post<ChargeResponse>(request);
            if (postResult.IsFailure) return postResult.Errors;

            if (paymentOptions.Method != PaymentMethod.Boleto)
                postResult.Result.BoletoUrl = null;

            FillTransactionDataFromGateway(postResult.Result);
            postResult.Result.Installments = paymentOptions.Installments;

            ITransaction transaction = postResult.Result;
            return new OperationResult<ITransaction>(transaction);
        }

        private void FillTransactionDataFromGateway(ChargeResponse incompleteData)
        {
            var client = new RestClient("https://api.iugu.com/v1/")
            {
                Authenticator = new HttpBasicAuthenticator(
                    IuguConfig.IsTest ? IuguConfig.ApiKeyTest : IuguConfig.ApiKeyLive, "")
            };

            var request = new RestRequest($"invoices/{incompleteData.TransactionId}", Method.GET);

            for (var i = 0; i < 3; i++)
            {
                try
                {
                    var response = client.Execute(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var json = JsonConvert.DeserializeAnonymousType(response.Content, new
                        {
                            tax_cents = default(int?),
                            taxes_paid_cents = default(int?),
                            total_cents = default(int?),
                            total_paid_cents = default(int?)
                        });

                        incompleteData.Amount = (json.total_cents ?? json.total_paid_cents ?? 0) / 100M;
                        incompleteData.GatewayFee = (json.tax_cents ?? json.taxes_paid_cents ?? 0) / 100M;

                        break;
                    }
                }
                catch { Thread.Sleep(100); }
            }
        }

        public OperationResult<ITransaction> Pay(IOrder order, ICustomer customer, IPaymentOptions paymentOptions, IAddress buyerAddress = null)
        {
            return Pay(order.Items, customer, paymentOptions, order.Supplier, buyerAddress);
        }

        public OperationResult<ITransaction> Pay(IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions, IAddress buyerAddress = null)
        {
            var items = orders.SelectMany(x => x.Items).ToList();
            var shipping = orders.Where(x => x.Shipping > 0).Select(x => new PaymentItem { Item = "Frete", Price = x.Shipping, Quantity = 1 });

            if (shipping.Any())
                items.AddRange(shipping);

            return Pay(items, customer, paymentOptions, supplier: null, buyerAddress: buyerAddress);
        }

        public OperationResult<ITransaction> Refund(IOrder order, ITransaction transaction, decimal totalRefund)
        {
            var invoiceOperations = new InvoiceOperations();
            var getInvoiceResult = invoiceOperations.GetInvoice(transaction.TransactionId);
            if (getInvoiceResult.IsFailure) return getInvoiceResult.Errors;

            var invoice = getInvoiceResult.Result;
            if (invoice.Status == "refunded") goto return_invoice;
            if (invoice.Refundable == false) return new ErrorDictionary("", $"Invoice {invoice.TransactionId} is not refundable.");

            var client = GetClient(null, $"/invoices/{invoice.TransactionId}/refund");
            var postResult = client.Post<GetInvoiceResponse>(null);
            if (postResult.IsFailure) return postResult.Errors;
            invoice = postResult.Result;

        return_invoice:
            return new OperationResult<ITransaction>(invoice);
        }

        private IuguClient GetClient(ISupplier supplier, string resource = null)
        {
            var apiKey = IuguConfig.IsTest
                ? supplier?.GatewayTestApiToken ?? IuguConfig.ApiKeyTest
                : supplier?.GatewayLiveApiToken ?? IuguConfig.ApiKeyLive;
            return new IuguClient(resource ?? "/charge", apiKey);
        }

        private ErrorDictionary Validate(ISupplier supplier)
        {
            var errors = new ErrorDictionary();

            if (supplier != null)
            {
                if (string.IsNullOrEmpty(supplier.GatewayUserToken))
                    errors.Add("error", nameof(supplier.GatewayUserToken) + " não pode ser vazio");
            }

            return errors;
        }

        public OperationResult<PayableDto> GetPayableDetails(string transactionId, string recipientId, int installment)
        {
            throw new NotImplementedException("O gateway Iugu não é suportado.");
        }

        public OperationResult<TransferDto> GetTransferDetails(string transferId)
        {
            throw new NotImplementedException("O gateway Iugu não é suportado.");
        }

        public OperationResult<PayableDto> GetPayableDetails(string payableId)
        {
            throw new NotImplementedException("O gateway Iugu não é suportado.");
        }

        public OperationResult<TransferDto> CreateTransferForPayable(string payableId)
        {
            throw new NotImplementedException("O gateway Iugu não é suportado.");
        }

        public OperationResult<IReadOnlyCollection<TransferDto>> ListWithdraws(string supplierId, string supplierApiToken)
        {
            throw new NotImplementedException("O gateway Iugu não é suportado.");
        }
    }
}