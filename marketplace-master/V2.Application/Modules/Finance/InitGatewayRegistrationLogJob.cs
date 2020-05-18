using Application.Config;
using Domain.Modules.Finance.Interfaces;
using System;

namespace V2.Application.Modules.Finance
{
    public class InitGatewayRegistrationLogJob
    {
        private readonly IGatewayRegistrationLogQueries registrationLogQueries;
        private readonly RegisterLogFromSellerDataCommandHandler registerLogFromSellerData;

        public InitGatewayRegistrationLogJob(
            IGatewayRegistrationLogQueries registrationLogQueries,
            RegisterLogFromSellerDataCommandHandler registerLogFromSellerData)
        {
            this.registrationLogQueries = registrationLogQueries;
            this.registerLogFromSellerData = registerLogFromSellerData;
        }

        public void RunIfNeeded()
        {
            if (!ShouldRun())
                return;

            var sellerIds = registrationLogQueries.ListSellerIdsPendingLog();
            if (sellerIds.IsFailure)
                throw new Exception("Falha ao executar o job InitGatewayRegistrationLog: " + sellerIds.Errors.ToSimpleString());

            foreach (var sellerId in sellerIds.Result)
                registerLogFromSellerData.Handle(sellerId);
        }

        private bool ShouldRun() => MaaSConfig.Current.Payment.SellerReceivesAnyPayment;
    }
}
