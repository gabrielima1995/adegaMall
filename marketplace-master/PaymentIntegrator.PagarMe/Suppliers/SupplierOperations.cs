using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;

namespace PaymentIntegrator.PagarMe.Suppliers
{
    public class SupplierOperations : ISupplierOperations
    {
        private OperationResult<BankAccountRequest> CreateBankAccount(PagarMeClient client, ISupplier supplier, IBankAccount bankAccount, IAccountable accountable)
        {
            var request = new BankAccountRequest(client, supplier, bankAccount, accountable);

            var validation = request.Validate();
            if (validation.HasErrors)
                return new OperationResult<BankAccountRequest>(validation);

            var saveResult = client.Save(request);
            if (saveResult.OK)
                return new OperationResult<BankAccountRequest>(request);

            return saveResult.Errors;
        }

        public OperationResult Save(ISupplier supplier, IBankAccount bankAccount, IAccountable accountable)
        {
            var client = new PagarMeClient();
            var bankAccountResult = CreateBankAccount(client, supplier, bankAccount, accountable);

            if (bankAccountResult.OK)
            {
                var request = new RecipientRequest(client, supplier, bankAccountResult.Result);

                var validation = request.Validate();
                if (validation.HasErrors)
                    return new OperationResult(validation);

                var saveResult = client.Save(request);
                if (saveResult.OK)
                    supplier.SetGatewayTokens(request.Id, null, null, null);

                return saveResult;
            }

            return bankAccountResult;
        }

        public OperationResult<SupplierWithBankAccountResult> Create(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable, IAddress accountableAddress)
        {
            var createResult = Save(supplier, bankAccount, accountable);
            if (createResult.IsFailure)
                return createResult.Errors;

            var auth = new HttpBasicAuthenticator(PagarMeConfig.ApiKey, "x");
            var client = new RestClient("https://api.pagar.me/1/") { Authenticator = auth };

            // https://docs.pagar.me/v2/reference#retornando-um-recebedor
            var response = client.Get(new RestRequest("recipients/{recipientId}")
                .AddParameter("recipientId", supplier.GatewayId, ParameterType.UrlSegment));

            var recipient = response.AsDynamic();
            return SupplierWithBankAccountResult.Pagarme(recipient.id.ToString(), recipient.bank_account.id.ToString(), response.Content);
        }

        public OperationResult<ISupplierStatus> GetStatus(ISupplier supplier)
        {
            // Não implementado no Pagar.me.
            return new OperationResult<ISupplierStatus>("error", "Não implementado no Pagar.me");
        }

        public OperationResult Update(ISupplier supplier)
        {
            // Não implementado no Pagar.me.
            return new OperationResult();
        }

        public OperationResult ChangeBankAccount(ISupplier supplier, IBankAccount bankAccount, IAccountable accountable)
        {
            if (string.IsNullOrEmpty(supplier.GatewayId))
                return new OperationResult("error", "GatewayId é obrigatório");

            return Save(supplier, bankAccount, accountable);
        }

        public OperationResult SetAutoTransferEnabled(ISupplier supplier, bool enabled)
        {
            var client = new PagarMeClient();

            try
            {
                var recipient = client.GetService().Recipients.Find(supplier.GatewayId);
                if (recipient.TransferEnabled != enabled)
                {
                    recipient.TransferEnabled = enabled;
                    recipient.Save();
                }

                return OperationResult.Success;
            }
            catch
            {
                return new OperationResult("error", "Falha ao alterar status de transferencia automatica");
            }
        }

        public OperationResult<AccountBalance> GetAvailableFunds(ISupplier supplier)
        {
            var auth = new HttpBasicAuthenticator(PagarMeConfig.ApiKey, "x");
            var client = new RestClient("https://api.pagar.me/1/") { Authenticator = auth };

            // https://docs.pagar.me/v2/reference#saldo-geral-das-opera%C3%A7%C3%B5es
            var balance = client.Get(new RestRequest("recipients/{recipientId}/balance")
                .AddParameter("recipientId", supplier.GatewayId, ParameterType.UrlSegment))
                .AsDynamic();

            return new AccountBalance((int)balance.available.amount, 0, (int)balance.waiting_funds.amount);
        }

        public OperationResult TransferBalance(ISupplier supplier, IAccountable accountable, IBankAccount bankAccount)
        {
            throw new NotImplementedException();
        }

        public OperationResult<PagarmeBankAccount> GetBankAccount(ISupplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.GatewayId))
                return new OperationResult<PagarmeBankAccount>("Invalid or incomplete information");

            var client = new PagarMeClient();
            var recipient = client.GetService().Recipients.Find(supplier.GatewayId);
            return new OperationResult<PagarmeBankAccount>(new PagarmeBankAccount(recipient.BankAccount));
        }

        public OperationResult<SupplierWithBankAccountResult> UpdateBankAccountOrCreateNewSupplier(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null)
        {
            var auth = new HttpBasicAuthenticator(PagarMeConfig.ApiKey, "x");
            var client = new RestClient("https://api.pagar.me/1/") { Authenticator = auth };

            // https://docs.pagar.me/v2/reference#retornando-um-recebedor
            var recipient = client.Get(new RestRequest("recipients/{recipientId}")
                .AddParameter("recipientId", supplier.GatewayId, ParameterType.UrlSegment))
                .AsDynamic();

            var pagarmeBankAccount = Common.PagarmeBankAccount.From(supplier, accountable, bankAccount);
            var currentDocument = Common.PagarmeDocumentNumber.From((string)recipient.bank_account.document_number);

            IRestResponse recipientResponse;

            if (currentDocument.IsSameAs(supplier.DocumentNumber))
            {
                // https://docs.pagar.me/v2/reference#atualizando-um-recebedor
                recipientResponse = client.Put(new RestRequest("recipients/{recipientId}")
                    .AddParameter("recipientId", supplier.GatewayId, ParameterType.UrlSegment)
                    .AddJsonBody(new { bank_account = pagarmeBankAccount.AsJson() }));

                if (recipientResponse.StatusCode != HttpStatusCode.OK)
                    return new ErrorDictionary("", "Erro ao atualizar a conta bancária do recebedor. " + recipientResponse.Content);
            }
            else
            {
                // https://docs.pagar.me/v2/reference#criando-um-recebedor
                recipientResponse = client.Post(new RestRequest("recipients")
                    .AddJsonBody(new Common.PagarmeRecipient(Common.PagarmeRecipientTransferSettings.Default(), pagarmeBankAccount).AsJson()));

                if (recipientResponse.StatusCode != HttpStatusCode.OK)
                    return new ErrorDictionary("", "Erro ao cadastrar a conta bancária do recebedor. " + recipientResponse.Content);
            }

            var recipientResult = recipientResponse.AsDynamic();
            return SupplierWithBankAccountResult.Pagarme(recipientResult.id.ToString(), recipientResult.bank_account.id.ToString(), recipientResponse.Content);
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