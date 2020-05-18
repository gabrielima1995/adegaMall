using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Iugu.Common;
using System;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class SupplierOperations : ISupplierOperations
    {
        private OperationResult CreateSubAccount(ISupplier supplier)
        {
            var client = new IuguClient("/marketplace/create_account", IuguConfig.ApiKeyLive); // somente cria sub conta em produção
            var request = new SubAccountRequest(supplier);

            var requestValidation = request.Validate();
            if (requestValidation.HasErrors) return requestValidation;

            var postResult = client.Post<SubAccountResponse>(request);

            if (postResult.OK)
            {
                var response = postResult.Result;
                supplier.SetGatewayTokens(response.AccountId, response.LiveApiToken, response.TestApiToken, response.UserToken);
            }

            return postResult;
        }

        private OperationResult RequestVerification(ISupplier supplier, IAddress address, IBankAccount bankAccount,
            IAccountable accountable)
        {
            var supplierValidation = Validate(supplier);
            if (supplierValidation.HasErrors) return new OperationResult(supplierValidation);

            var client = new IuguClient($"/accounts/{supplier.GatewayId}/request_verification", supplier.GatewayUserToken);
            var request = new RequestVerificationRequest(supplier, address, bankAccount, accountable);

            var requestValidation = request.Validate();
            if (requestValidation.HasErrors) return new OperationResult(requestValidation);

            var validation = client.Post<RequestVerificationResponse>(request);
            return validation;
        }

        private ErrorDictionary Validate(ISupplier supplier)
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(supplier.GatewayUserToken))
                errors.Add("error", "GatewayUserToken não pode ser vazio");

            if (string.IsNullOrEmpty(supplier.GatewayId))
                errors.Add("error", "GatewayId não pode ser vazio");

            return errors;
        }

        public OperationResult<SupplierWithBankAccountResult> Create(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null)
        {
            /* Na versão 2.32.1 a plataforma estava sendo atualizada para permitir alteração dos dados para recebimento.
             * A interface passou a retornar o SupplierWithBankAccountResult no método Create.
             * O gateway IUGU já não era mais usado. Portanto, nenhum esforço foi feito para adaptar o gateway.
             * Caso volte a funcionar, implementar o retorno SupplierWithBankAccountResult.
             */
            throw new NotImplementedException();

            //var accountCreationResult = CreateSubAccount(supplier);
            //if (accountCreationResult.IsFailure) return accountCreationResult.Errors;

            //var requestVerificationResult = RequestVerification(supplier, address, bankAccount, accountable);
            //if (requestVerificationResult.IsFailure) return requestVerificationResult.Errors;

            //var updateResult = Update(supplier); // Esta chamada executa configuração da nova conta.
            //if (updateResult.IsFailure) return updateResult.Errors;

            //return OperationResult.Success;
        }

        public OperationResult<ISupplierStatus> GetStatus(ISupplier supplier)
        {
            var supplierValidation = Validate(supplier);
            if (supplierValidation.HasErrors) return supplierValidation;

            var client = new IuguClient($"/accounts/{supplier.GatewayId}", supplier.GatewayUserToken);
            var getResult = client.Get<SubAccountStatusResponse>();

            ISupplierStatus status = getResult.Result;
            return new OperationResult<ISupplierStatus>(status);
        }

        public OperationResult Update(ISupplier supplier)
        {
            var errors = Validate(supplier);
            if (errors.HasErrors) return errors;

            var client = new IuguClient("/accounts/configuration", supplier.GatewayUserToken);
            var configurationRequest = new SubAccountConfigurationRequest();

            var requestValidation = configurationRequest.Validate();
            if (requestValidation.HasErrors) return requestValidation;

            return client.Post<SubAccountStatusResponse>(configurationRequest);
        }

        public OperationResult ChangeBankAccount(ISupplier supplier, IBankAccount bankAccount, IAccountable accountable = null)
        {
            var errors = Validate(supplier);
            if (errors.HasErrors) return errors;

            var client = new IuguClient("/bank_verification", supplier.GatewayUserToken);
            var request = new BankVerificationRequest(bankAccount);

            var requestValidation = request.Validate();
            if (requestValidation.HasErrors) return requestValidation;

            return client.Post<SuccessResponse>(request);
        }

        public OperationResult SetAutoTransferEnabled(ISupplier supplier, bool enabled)
        {
            throw new System.NotImplementedException();
        }

        public OperationResult<AccountBalance> GetAvailableFunds(ISupplier supplier)
        {
            throw new System.NotImplementedException();
        }

        public OperationResult TransferBalance(ISupplier supplier, IAccountable accountable, IBankAccount bankAccount)
        {
            throw new System.NotImplementedException();
        }

        public OperationResult<dynamic> Get(ISupplier supplier)
        {
            throw new System.NotImplementedException();
        }

        public OperationResult<SupplierWithBankAccountResult> UpdateBankAccountOrCreateNewSupplier(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable = null, IAddress accountableAddress = null)
        {
            /* 26 set 2019
             * Não implementado porque o gateway Iugu não é mais suportado pela plataforma.
             * As classes ainda existem porque fazem parte da biblioteca payment integrator.
             * Futuramente podem ser removidas se não fizer mais sentido para a plataforma em definitivo.
             */
            throw new System.NotImplementedException();
        }
    }
}