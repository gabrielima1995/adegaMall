using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Moip.Common;
using PaymentIntegrator.Moip.Common.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PaymentIntegrator.Moip.Suppliers
{
    public class MoipSupplierOperations : ISupplierOperations
    {
        public OperationResult ChangeBankAccount(ISupplier supplier, IBankAccount bankAccount, IAccountable accountable = null)
        {
            if (supplier == null || bankAccount == null)
                return OperationResult.Fail("Invalid or incomplete information");

            // Request list of bank accounts from server
            var accountsListRequest = new MoipRequest($"accounts/{supplier.GatewayId}/bankaccounts", Method.GET);
            var accountsListResponse = MoipClient.SendOAuthRequest(accountsListRequest, supplier.GatewayUserToken);

            try
            {
                // Get first account (should be only one)
                dynamic accountsList = JsonConvert.DeserializeObject(accountsListResponse.Content);
                dynamic firstAccount = accountsList.First;

                // Generate PUT Request to update account remotely
                var updateAccountRequest = new MoipRequest($"bankaccounts/{(string)firstAccount.id}", Method.PUT);
                var updatedAccount = MoipBankAccount.UpdateBankAccount(firstAccount, bankAccount, accountable);
                string jsonString = JsonConvert.SerializeObject(updatedAccount, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                updateAccountRequest.AddParameter("application/json", jsonString, ParameterType.RequestBody);

                // Send request
                var response = MoipClient.SendOAuthRequest(updateAccountRequest, supplier.GatewayUserToken);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return OperationResult.Success;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<MoipErrorResponse>(response.Content);

                        return errorResponse.Result;
                    }
                    catch
                    {
                        return OperationResult.Fail("Error parsing Moip error response");
                    }
                }

                return OperationResult.Fail("Unable to change bank account");
            }
            catch
            {
                return OperationResult.Fail("Unable to change bank account");
            }
        }

        public OperationResult<SupplierWithBankAccountResult> Create(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null)
        {
            var createResult = CreateInternal(supplier, address, bankAccount, accountable, accountableAddress);
            if (createResult.IsFailure)
                return createResult.Errors;

            return GetSupplierWithBankAccount(supplier);
        }

        private OperationResult CreateInternal(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null)
        {
            if (supplier == null || address == null)
                return OperationResult.Fail("Invalid or incomplete information");

            var request = new MoipRequest("accounts", Method.POST);
            var requestObject = MoipSupplier.Create(supplier, address, bankAccount, accountable, accountableAddress);
            request.AddJsonBody(requestObject);

            var response = MoipClient.SendOAuthRequest(request);

            if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    dynamic createdSupplier = JsonConvert.DeserializeObject(response.Content);

                    var bankAccountRequest = new MoipRequest($"accounts/{createdSupplier.id.ToString()}/bankaccounts", Method.POST);
                    bankAccountRequest.AddJsonBody(MoipBankAccount.CreateBankAccount(supplier, accountable, bankAccount));

                    var bankAccountResponse = MoipClient.SendOAuthRequest(bankAccountRequest, (string)createdSupplier.accessToken);

                    if (bankAccountResponse.StatusCode == HttpStatusCode.Created || bankAccountResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var createdBankAccount = JsonConvert.DeserializeObject(bankAccountResponse.Content);
                        supplier.SetGatewayTokens((string)createdSupplier.id, "", "", (string)createdSupplier.accessToken);

                        return OperationResult.Success;
                    }

                    if (bankAccountResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        try
                        {
                            var errorResponse = JsonConvert.DeserializeObject<MoipErrorResponse>(bankAccountResponse.Content);
                            return errorResponse.Result;
                        }
                        catch
                        {
                            return OperationResult.Fail("Error parsing Moip error bankAccountResponse");
                        }
                    }

                    return OperationResult.Fail($"Error creating Moip Bank Account Supplier - HTTP {(int)response.StatusCode}");
                }
                catch
                {
                    return OperationResult.Fail("Error parsing wirecard response");
                }
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<MoipErrorResponse>(response.Content);
                    return errorResponse.Result;
                }
                catch
                {
                    return OperationResult.Fail("Error parsing Moip error response");
                }
            }

            return OperationResult.Fail($"Error creating Moip Supplier - HTTP {(int)response.StatusCode}");
        }

        public OperationResult<ISupplierStatus> GetStatus(ISupplier supplier)
        {
            throw new NotImplementedException();
        }

        public OperationResult SetAutoTransferEnabled(ISupplier supplier, bool enabled)
        {
            return OperationResult.Success;
        }

        public OperationResult Update(ISupplier supplier)
        {
            throw new NotImplementedException();
        }

        public OperationResult<AccountBalance> GetAvailableFunds(ISupplier supplier)
        {
            if (supplier == null || string.IsNullOrWhiteSpace(supplier.GatewayId))
            {
                return OperationResult<AccountBalance>.Fail("Error", "Invalid supplier");
            }

            var getFundsRequests = new MoipRequest("balances", Method.GET);
            var getFundsResponse = MoipClient.SendOAuthRequest(getFundsRequests, supplier.GatewayUserToken);
            if (getFundsResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                dynamic fundsObject = JsonConvert.DeserializeObject(getFundsResponse.Content);
                int currentFunds = Math.Abs((int)fundsObject[0].current - (int)fundsObject[0].unavailable);
                return new AccountBalance(currentFunds, (int)fundsObject[0].unavailable, (int)fundsObject[0].future);
            }

            return OperationResult<AccountBalance>.Fail("Error", "Erro ao obter saldo");
        }

        public OperationResult TransferBalance(ISupplier supplier, IAccountable accountable, IBankAccount bankAccount)
        {
            if (supplier == null || string.IsNullOrWhiteSpace(supplier.GatewayId) || string.IsNullOrWhiteSpace(supplier.GatewayUserToken))
                return OperationResult.Fail("Error", "Cadastro de vendedor inválido");

            var amountResult = GetAvailableFunds(supplier);
            if (amountResult.IsFailure)
                return OperationResult.Fail(amountResult.Errors[0].Value);

            var amount = amountResult.Result.AvailableBalance - amountResult.Result.UnavailableBalance;
            if (amount <= 200)
                return OperationResult.Fail("Error", "Saldo deve ser superior a R$2,00");

            var accountAndBankAccount = GetAccountAndBankAccount(supplier);
            if (accountAndBankAccount.IsFailure)
                return OperationResult.Fail("Error", accountAndBankAccount.Errors.ToSimpleString());
            if (accountAndBankAccount.Result.BankAccount == null)
                return OperationResult.Fail("Error", "Conta bancária inválida ou não informada");

            var transferBalanceRequest = new MoipRequest("transfers", Method.POST);
            transferBalanceRequest.AddJsonBody(new
            {
                amount,
                transferInstrument = new
                {
                    method = "BANK_ACCOUNT",
                    bankAccount = new { id = accountAndBankAccount.Result.BankAccount.Id }
                }
            });

            var transferBalanceResponse = MoipClient.SendOAuthRequest(transferBalanceRequest, supplier.GatewayUserToken);
            if (transferBalanceResponse.StatusCode == HttpStatusCode.OK || transferBalanceResponse.StatusCode == HttpStatusCode.Created)
                return OperationResult.Success;

            try
            {
                var json = JsonConvert.DeserializeObject<dynamic>(transferBalanceResponse.Content);
                var error = json.errors[0].description.ToString();
                return OperationResult.Fail("", error);
            }
            catch
            {
                return OperationResult.Fail("", "Erro desconhecido ao realizar o saque. Tente novamente mais tarde e, se o erro persistir, entre em contato com nossa equipe de suporte.");
            }
        }

        public OperationResult<MoipAccountAndBankAccount> GetAccountAndBankAccount(ISupplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.GatewayId) ||
                string.IsNullOrWhiteSpace(supplier.GatewayUserToken))
                return new OperationResult<MoipAccountAndBankAccount>("Invalid or incomplete information");

            var request = new MoipRequest($"accounts/{supplier.GatewayId}", Method.GET);
            var response = MoipClient.SendOAuthRequest(request);

            if (response.IsSuccessful)
            {
                try
                {
                    var account = JsonConvert.DeserializeObject<Account>(response.Content);

                    request = new MoipRequest($"accounts/{account.Id}/bankaccounts", Method.GET);
                    response = MoipClient.SendOAuthRequest(request, supplier.GatewayUserToken);

                    if (response.IsSuccessful)
                    {
                        var bankAccountList = JsonConvert.DeserializeObject<List<BankAccount>>(response.Content);
                        var bankAccount = bankAccountList.FirstOrDefault();
                        return new MoipAccountAndBankAccount(account, bankAccount);
                    }
                }
                catch
                {
                    return new OperationResult<MoipAccountAndBankAccount>("Error parsing wirecard response");
                }
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<MoipErrorResponse>(response.Content);
                    return new OperationResult<MoipAccountAndBankAccount>(errorResponse.Result);
                }
                catch
                {
                    return new OperationResult<MoipAccountAndBankAccount>("Error parsing Moip error response");
                }
            }

            return new OperationResult<MoipAccountAndBankAccount>($"Error creating Moip Supplier - HTTP {(int)response.StatusCode}");
        }

        public OperationResult<SupplierWithBankAccountResult> UpdateBankAccountOrCreateNewSupplier(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null)
        {
            var createResult = Create(supplier, address, bankAccount, accountable, accountableAddress);
            if (!createResult.OK)
                return createResult.Errors;

            return GetSupplierWithBankAccount(supplier);
        }

        private OperationResult<SupplierWithBankAccountResult> GetSupplierWithBankAccount(ISupplier supplier)
        {
            var auth = new OAuth2AuthorizationRequestHeaderAuthenticator(supplier.GatewayUserToken);
            var environment = MoipConfig.Current.IsSandbox ? "sandbox" : "api";
            var client = new RestClient($"https://{environment}.moip.com.br/v2/") { Authenticator = auth };

            // https://dev.wirecard.com.br/reference#criar-conta-moip-transparente-2
            var accountResponse = client.Get(new RestRequest("accounts/{accountId}")
                .AddParameter("accountId", supplier.GatewayId, ParameterType.UrlSegment));

            if (!accountResponse.IsSuccessful || accountResponse.StatusCode != HttpStatusCode.OK)
                return new ErrorDictionary("", accountResponse.Content);

            // https://dev.wirecard.com.br/reference#contas-banc%C3%A1rias-1
            var bankAccountsResponse = client.Get(new RestRequest("accounts/{accountId}/bankaccounts")
                .AddParameter("accountId", supplier.GatewayId, ParameterType.UrlSegment));

            var accountJson = accountResponse.AsDynamic();
            var bankAccountsJson = bankAccountsResponse.AsDynamic();

            return SupplierWithBankAccountResult.Wirecard(
                accountJson.id.ToString(),
                bankAccountsJson[0].id.ToString(),
                supplier.GatewayUserToken,
                JsonConvert.SerializeObject(new { account = accountJson, bankAccounts = bankAccountsJson }));
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