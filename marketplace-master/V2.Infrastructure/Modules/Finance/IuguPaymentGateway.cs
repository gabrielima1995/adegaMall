using Application.Config;
using Domain.Modules.Finance;
using Framework;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using V2.Application.Modules.Finance.Interfaces;
using V2.Application.Modules.Finance.Model;

namespace V2.Infrastructure.Modules.Finance
{
    public class IuguPaymentGateway : IPaymentGateway
    {
        public OperationResult<string> DoTransfer(TransferJournalEntry data)
        {
            var request = new RestRequest("transfers", Method.POST);

            request.AddJsonBody(new
            {
                receiver_id = data.GatewayReceiverId,
                amount_cents = (int)(data.TransferAmount * 100),
                custom_variables = new[]
                {
                    new { name = "comparison_key", value = data.KeyForGatewayComparison },
                    new { name = "installment", value = $"{data.InstallmentNumber}/{data.InstallmentsCount}" },
                    new { name = "order_id", value = data.OrderId.ToString() },
                    new { name = "iugu_transaction", value = data.GatewayTransactionId },
                    new { name = "seller", value = data.SellerId }
                }
            });

            var response = ExecuteRequest(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return new ErrorDictionary("", GetFormattedError(response));

            var jsonResult = JsonConvert.DeserializeAnonymousType(response.Content, new { id = "" });
            return jsonResult.id;
        }

        public IEnumerable<TransferDto> ListCompletedTransfersForOrder(Guid orderId)
        {
            var request = new RestRequest("transfers", Method.GET);

            request.AddQueryParameter("custom_variables_name", "order_id");
            request.AddQueryParameter("custom_variables_value", orderId.ToString());

            var response = ExecuteRequest(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(GetFormattedError(response));

            var json = JsonConvert.DeserializeAnonymousType(response.Content, new
            {
                sent = new[]
                {
                    new
                    {
                       id = "",
                       created_at = DateTime.Now,
                       amount_cents = 0M,
                       receiver = new { id = "", name = "" }
                    }
                }
            });

            var results = json.sent.Select(x => new TransferDto
            {
                Id = x.id,
                CreatedAt = x.created_at,
                Amount = x.amount_cents / 100,
                ReceiverId = x.receiver.id,
                ReceiverName = x.receiver.name
            }).ToList();

            return results;
        }

        public IEnumerable<ReceivableDto> ListReceivablesByTransactionId(string transactionId)
        {
            var request = new RestRequest($"invoices/{transactionId}", Method.GET);

            var response = ExecuteRequest(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(GetFormattedError(response));

            var json = JsonConvert.DeserializeAnonymousType(response.Content, new
            {
                financial_return_dates = new[]
                {
                    new
                    {
                        id = "",
                        installment = short.MinValue,
                        return_date = "",
                        status = "",
                        amount = "",
                        taxes = "",
                        executed_date = "",
                        advanced = false
                    }
                }
            });

            if (json.financial_return_dates == null)
                return new ReceivableDto[0];

            var ptBrCulture = CultureInfo.CreateSpecificCulture("pt-BR");
            var results = json.financial_return_dates.Select(x => new ReceivableDto
            {
                Id = x.id,
                InstallmentNumber = x.installment,
                TransferDueDate = DateTime.ParseExact(x.return_date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                Amount =
                    decimal.Parse(x.amount, NumberStyles.Currency, ptBrCulture) +
                    decimal.Parse(x.taxes, NumberStyles.Currency, ptBrCulture),
                IsAmountAvailableForTransfer = x.executed_date != null || x.advanced,
                IsAdvanced = x.advanced
            }).ToList();

            return results;
        }

        private IRestResponse ExecuteRequest(IRestRequest request)
        {
            var config = MaaSConfig.Current.Payment.Iugu;

            var client = new RestClient("https://api.iugu.com/v1/")
            {
                Authenticator = new HttpBasicAuthenticator(
                    config.IsLive ? config.ApiKeyLive : config.ApiKeyTest, "")
            };

            return client.Execute(request);
        }

        private string GetFormattedError(IRestResponse response)
        {
            return $"{response.StatusCode} ({response.StatusDescription}): {response.Content}";
        }
    }
}
