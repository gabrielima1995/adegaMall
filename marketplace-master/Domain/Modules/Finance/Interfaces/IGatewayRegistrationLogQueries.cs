using Framework;
using System;
using System.Collections.Generic;

namespace Domain.Modules.Finance.Interfaces
{
    public interface IGatewayRegistrationLogQueries
    {
        OperationResult<InitialSellerGatewayRegistrationForLogging> GetInitialSellerGatewayRegistrationForLogging(string sellerId);
        OperationResult<GatewayRegistrationLogDto> GetLatestLogForSeller(string sellerId);
        OperationResult<SupplierAndBankAccountData> GetSupplierAndBankAccountDataFromGateway(string supplierGatewayId, string apiToken);
        OperationResult<IReadOnlyCollection<string>> ListSellerIdsPendingLog();
        OperationResult<IReadOnlyCollection<GatewayRecipientIdAndApiTokenDto>> ListGatewayIdAndToken(string sellerId);
    }

    public class InitialSellerGatewayRegistrationForLogging
    {
        public string SellerId { get; set; }
        public string GatewayRecipientId { get; set; }
        public string GatewayApiToken { get; set; }
    }

    public class GatewayRegistrationLogDto
    {
        public Guid Id { get; private set; }
        public string SellerId { get; private set; }
        public string Gateway { get; private set; }
        public string GatewayId { get; private set; }
        public string GatewayBankAccountId { get; private set; }
        public string GatewayApiToken { get; private set; }
        public string ExtraInfoJson { get; private set; }
        public DateTime LogDateTime { get; private set; }
    }

    public class GatewayRecipientIdAndApiTokenDto
    {
        public Guid Id { get; set; }
        public string SellerId { get; set; }
        public string GatewayRecipientId { get; set; }
        public string GatewayApiToken { get; set; }
    }

    public class SupplierAndBankAccountData
    {
        public string RecipientId { get; set; }
        public string BankAccountId { get; set; }
        public string FullDataJson { get; set; }
    }
}
