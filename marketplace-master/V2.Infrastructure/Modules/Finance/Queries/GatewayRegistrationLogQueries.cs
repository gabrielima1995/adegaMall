using Application.Config;
using Dapper;
using Domain.Modules.Finance.Interfaces;
using Framework;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.Finance.Queries
{
    public class GatewayRegistrationLogQueries : IGatewayRegistrationLogQueries
    {
        public OperationResult<GatewayRegistrationLogDto> GetLatestLogForSeller(string sellerId)
        {
            using (var connection = MyDbConnection.Create())
            {
                var sql = @"SELECT TOP 1 * FROM GatewayRegistrationLog WHERE SellerId = @sellerId ORDER BY CreatedAt DESC";
                return connection.QueryFirst<GatewayRegistrationLogDto>(sql, new { sellerId });
            }
        }

        public OperationResult<InitialSellerGatewayRegistrationForLogging> GetInitialSellerGatewayRegistrationForLogging(string sellerId)
        {
            using (var connection = MyDbConnection.Create())
            {
                var sql = @"SELECT Id SellerId, SupplierGatewayId GatewayRecipientId, GatewayUserToken GatewayApiToken FROM Person WHERE Id = @sellerId";
                return connection.QueryFirst<InitialSellerGatewayRegistrationForLogging>(sql, new { sellerId });
            }
        }

        public OperationResult<SupplierAndBankAccountData> GetSupplierAndBankAccountDataFromGateway(string supplierGatewayId, string apiToken)
        {
            if (MaaSConfig.Current.Payment.GatewayIsPagarme())
                return new GatewayRegistrationLogQueries_Pagarme().GetSupplierAndBankAccountDataFromGateway(supplierGatewayId);

            if (MaaSConfig.Current.Payment.GatewayIsWirecard())
                return new GatewayRegistrationLogQueries_Wirecard().GetSupplierAndBankAccountDataFromGateway(supplierGatewayId, apiToken);

            throw new NotImplementedException();
        }

        public OperationResult<IReadOnlyCollection<string>> ListSellerIdsPendingLog()
        {
            using (var connection = MyDbConnection.Create())
            {
                var sql = @"
                    SELECT Person.Id FROM Person
                    LEFT JOIN GatewayRegistrationLog ON Person.Id = GatewayRegistrationLog.SellerId
                    WHERE Person.Discriminator = 'Seller'
                        AND Person.SupplierGatewayId IS NOT NULL
                        AND GatewayRegistrationLog.Id IS NULL";
                return connection.Query<string>(sql).ToList();
            }
        }

        public OperationResult<IReadOnlyCollection<GatewayRecipientIdAndApiTokenDto>> ListGatewayIdAndToken(string sellerId)
        {
            using (var connection = MyDbConnection.Create())
            {
                var sql = @"SELECT Id, SellerId, GatewayRecipientId, GatewayApiToken FROM GatewayRegistrationLog WHERE SellerId = @sellerId";
                return connection.Query<GatewayRecipientIdAndApiTokenDto>(sql, new { sellerId }).ToList();
            }
        }
    }

    internal class GatewayRegistrationLogQueries_Pagarme
    {
        internal OperationResult<SupplierAndBankAccountData> GetSupplierAndBankAccountDataFromGateway(string supplierGatewayId)
        {
            var auth = new HttpBasicAuthenticator(MaaSConfig.Current.Payment.PagarMe.ApiKey, "x");
            var client = new RestClient("https://api.pagar.me/1/") { Authenticator = auth };

            // https://docs.pagar.me/v2/reference#retornando-um-recebedor
            var response = client.Get(new RestRequest("recipients/{recipientId}")
                .AddParameter("recipientId", supplierGatewayId, ParameterType.UrlSegment));

            if (!response.IsSuccessful || response.StatusCode != HttpStatusCode.OK)
                return new ErrorDictionary("", response.Content);

            var json = response.AsDynamic();
            return new SupplierAndBankAccountData
            {
                RecipientId = json.id,
                BankAccountId = json.bank_account.id,
                FullDataJson = response.Content
            };
        }
    }

    internal class GatewayRegistrationLogQueries_Wirecard
    {
        internal OperationResult<SupplierAndBankAccountData> GetSupplierAndBankAccountDataFromGateway(string supplierGatewayId, string apiToken)
        {
            var auth = new OAuth2AuthorizationRequestHeaderAuthenticator(apiToken);
            var environment = MaaSConfig.Current.Payment.Moip.IsSandbox ? "sandbox" : "api";
            var client = new RestClient($"https://{environment}.moip.com.br/v2/") { Authenticator = auth };

            // https://dev.wirecard.com.br/reference#criar-conta-moip-transparente-2
            var accountResponse = client.Get(new RestRequest("accounts/{accountId}")
                .AddParameter("accountId", supplierGatewayId, ParameterType.UrlSegment));

            if (!accountResponse.IsSuccessful || accountResponse.StatusCode != HttpStatusCode.OK)
                return new ErrorDictionary("", accountResponse.Content);

            // https://dev.wirecard.com.br/reference#contas-banc%C3%A1rias-1
            var bankAccountsResponse = client.Get(new RestRequest("accounts/{accountId}/bankaccounts")
                .AddParameter("accountId", supplierGatewayId, ParameterType.UrlSegment));

            var accountJson = accountResponse.AsDynamic();
            var bankAccountsJson = bankAccountsResponse.AsDynamic();

            return new SupplierAndBankAccountData
            {
                RecipientId = accountJson.id,
                BankAccountId = bankAccountsJson.Count == 0 ? null : bankAccountsJson[0].id,
                FullDataJson = JsonConvert.SerializeObject(new { account = accountJson, bankAccounts = bankAccountsJson })
            };
        }
    }

    public static class IRestResponseExtensions
    {
        public static dynamic AsDynamic(this IRestResponse response)
        {
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
    }
}
