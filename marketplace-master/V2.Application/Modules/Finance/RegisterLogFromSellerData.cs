using Application.Config;
using Domain.Modules.Finance;
using Domain.Modules.Finance.Interfaces;
using Framework;
using System;

namespace V2.Application.Modules.Finance
{
    public class RegisterLogFromSellerDataCommand
    {
        public RegisterLogFromSellerDataCommand(string sellerId)
        {
            SellerId = sellerId;
        }
        public string SellerId { get; }
        public static implicit operator RegisterLogFromSellerDataCommand(string sellerId) => new RegisterLogFromSellerDataCommand(sellerId);
    }

    public class RegisterLogFromSellerDataCommandHandler
    {
        private readonly IGatewayRegistrationLogQueries registrationLogQueries;
        private readonly IGatewayRegistrationLogRepository registrationLogRepository;

        public RegisterLogFromSellerDataCommandHandler(
            IGatewayRegistrationLogQueries registrationLogQueries,
            IGatewayRegistrationLogRepository registrationLogRepository)
        {
            this.registrationLogQueries = registrationLogQueries;
            this.registrationLogRepository = registrationLogRepository;
        }

        public OperationResult Handle(RegisterLogFromSellerDataCommand command)
        {
            var initialRegistration = registrationLogQueries.GetInitialSellerGatewayRegistrationForLogging(command.SellerId);
            if (initialRegistration.IsFailure)
                return initialRegistration.Errors;

            var gatewayData = registrationLogQueries.GetSupplierAndBankAccountDataFromGateway(initialRegistration.Result.GatewayRecipientId, initialRegistration.Result.GatewayApiToken);
            if (gatewayData.IsFailure)
                return gatewayData.Errors;

            var log = BuildLogForActiveGateway(initialRegistration.Result, gatewayData.Result);
            return registrationLogRepository.Add(log);
        }

        private GatewayRegistrationLog BuildLogForActiveGateway(InitialSellerGatewayRegistrationForLogging initialRegistration, SupplierAndBankAccountData gatewayData)
        {
            if (MaaSConfig.Current.Payment.PagarMe.SellerReceivesPagarmePayment)
                return GatewayRegistrationLog.PagarmeLog(Guid.NewGuid(), initialRegistration.SellerId,
                    gatewayData.RecipientId, gatewayData.BankAccountId,
                    gatewayData.FullDataJson);

            else if (MaaSConfig.Current.Payment.Moip.SellerReceivesMoipPayment)
                return GatewayRegistrationLog.WirecardLog(Guid.NewGuid(), initialRegistration.SellerId,
                    gatewayData.RecipientId, gatewayData.BankAccountId,
                    initialRegistration.GatewayApiToken, gatewayData.FullDataJson);

            throw new NotImplementedException($"Método não implementado para o gateway de pagamentos '{MaaSConfig.Current.Payment.ActiveGateway}'.");
        }
    }
}
