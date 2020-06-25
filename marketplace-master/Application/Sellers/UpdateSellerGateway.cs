using Application.Config;
using Application.Sellers.Moip;
using Application.Sellers.Pagarme;
using Domain.Entities;
using Domain.Events;
using Domain.Modules.Finance;
using Domain.Modules.Finance.Interfaces;
using Domain.Sellers;
using Framework;
using Infra.Crosscutting.Globalization;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Moip.Suppliers;
using PaymentIntegrator.PagarMe.Suppliers;
using System;

namespace Application.Sellers
{
    public class UpdateSellerGatewayCommand
    {
        public UpdateSellerGatewayCommand(Seller seller, SellerGatewayViewModel gatewayViewModel)
        {
            Seller = seller;
            GatewayViewModel = gatewayViewModel;
        }

        public Seller Seller { get; }
        public SellerGatewayViewModel GatewayViewModel { get; }
    }

    public class UpdateSellerGatewayCommandHandler
    {
        private readonly SellerService_Obsolete sellerService;
        private readonly IRepository<Seller> sellerRepository;
        private readonly ISupplierOperations supplierOperations;
        private readonly IGatewayRegistrationLogQueries registrationLogQueries;
        private readonly IGatewayRegistrationLogRepository registrationLogRepository;

        public UpdateSellerGatewayCommandHandler(
            SellerService_Obsolete sellerService,
            IRepository<Seller> sellerRepository,
            ISupplierOperations supplierOperations,
            IGatewayRegistrationLogQueries registrationLogQueries,
            IGatewayRegistrationLogRepository registrationLogRepository)
        {
            this.sellerService = sellerService;
            this.sellerRepository = sellerRepository;
            this.supplierOperations = supplierOperations;
            this.registrationLogQueries = registrationLogQueries;
            this.registrationLogRepository = registrationLogRepository;
        }

        public OperationResult Handle(UpdateSellerGatewayCommand command)
        {
            if (!MaaSConfig.Current.Payment.Enabled)
                return OperationResult.Success;

            if (!command.Seller.IntegratorSupplierRegistered)
                throw new InvalidOperationException("Esta operação só pode ser executada quando o vendedor já está cadastrado no gateway.");

            if (!MaaSConfig.Current.Payment.GatewayIsPagarme() && !MaaSConfig.Current.Payment.GatewayIsWirecard())
                throw UnsupportedGatewayException();

            var currentGateway = GetGateway(command.GatewayViewModel.SellerId);
            if (!command.GatewayViewModel.IsDifferentBankAccount(currentGateway))
                return OperationResult.Success;

            var canUpdate = CanUpdate(command, currentGateway);
            if (canUpdate.IsFailure)
                return canUpdate.Errors;

            var currentRegistration = registrationLogQueries.GetLatestLogForSeller(command.GatewayViewModel.SellerId);
            if (currentRegistration.IsFailure || currentRegistration.Result == null)
                return OperationResult.Fail("", GlobalizationHelper.GetString("GATEWAY_CANNOT_UPDATE_BANK_ACCOUNT_UNKNOWN"));

            var gatewayData = GatewayData.FromViewModelForCurrentGateway(command.GatewayViewModel, command.Seller);
            var updateOrCreateBankAccount = UpdateBankAccountOrCreateNewSupplier(gatewayData);
            if (updateOrCreateBankAccount.IsFailure)
                return updateOrCreateBankAccount.Errors;

            var seller = command.Seller;
            var updateSellerResult = UpdateSellerWithNewRegistration(seller, updateOrCreateBankAccount.Result);
            if (updateSellerResult.IsFailure)
                return updateSellerResult.Errors;

            var logResult = LogGatewayRegistration(command.GatewayViewModel.SellerId, updateOrCreateBankAccount.Result);
            if (logResult.IsFailure)
                return logResult.Errors;

            return OperationResult.Success;
        }

        private SellerGatewayViewModel GetGateway(string sellerId)
        {
            var seller = sellerService.Get(i => i.Id == sellerId);
            var model = new SellerGatewayViewModel(seller);

            if (model.GatewayRegistered && model.SellerReceivesMoipPayment)
            {
                var op = new MoipSupplierOperations().GetAccountAndBankAccount(seller);
                model.Moip = SellerMoipViewModel.Load(op.Result);
            }
            else if (model.GatewayRegistered && model.SellerReceivesPagarmePayment)
            {
                var op = new SupplierOperations().GetBankAccount(seller);
                model.Pagarme = SellerPagarmeViewModel.Load(op.Result);
            }

            return model;
        }

        private OperationResult CanUpdate(UpdateSellerGatewayCommand command, SellerGatewayViewModel currentGateway)
        {
            var hasPendingBalance = supplierOperations.GetAvailableFunds(command.Seller);
            if (hasPendingBalance.IsFailure)
                return new ErrorDictionary("", string.Format("{0}: {1}",
                    GlobalizationHelper.GetString("GATEWAY_CANNOT_UPDATE_BANK_ACCOUNT_UNKNOWN"),
                    hasPendingBalance.Errors.ToSimpleString()));

            return hasPendingBalance.Result.HasAnyBalance
                ? OperationResult.Fail("", GlobalizationHelper.GetString("GATEWAY_CANNOT_UPDATE_BANK_ACCOUNT_PENDING_PAYABLES"))
                : OperationResult.Success;
        }

        private OperationResult<SupplierWithBankAccountResult> UpdateBankAccountOrCreateNewSupplier(GatewayData gatewayData)
        {
            return supplierOperations.UpdateBankAccountOrCreateNewSupplier(gatewayData.Supplier, gatewayData.Address, gatewayData.BankAccount, gatewayData.Accountable, gatewayData.AccountableAddress);
        }

        private OperationResult UpdateSellerWithNewRegistration(Seller seller, SupplierWithBankAccountResult supplierWithBankAccount)
        {
            seller.SetGatewayTokens(supplierWithBankAccount.SupplierId, null, null, supplierWithBankAccount.ApiToken);

            seller.AddDomainEvent(new SellerUpdatedEvent(seller));

            sellerRepository.Update(seller);
            sellerRepository.SaveChanges();

            return OperationResult.Success;
        }

        private OperationResult LogGatewayRegistration(string sellerId, SupplierWithBankAccountResult supplierWithBankAccount)
        {
            GatewayRegistrationLog log = null;

            if (MaaSConfig.Current.Payment.PagarMe.SellerReceivesPagarmePayment)
                log = GatewayRegistrationLog.PagarmeLog(Guid.NewGuid(),
                    sellerId, supplierWithBankAccount.SupplierId, supplierWithBankAccount.BankAccountId, supplierWithBankAccount.ExtraInfoJson);

            if (MaaSConfig.Current.Payment.Moip.SellerReceivesMoipPayment)
                log = GatewayRegistrationLog.WirecardLog(Guid.NewGuid(),
                    sellerId, supplierWithBankAccount.SupplierId, supplierWithBankAccount.BankAccountId, supplierWithBankAccount.ApiToken, supplierWithBankAccount.ExtraInfoJson);

            if (log == null)
                throw UnsupportedGatewayException();

            return registrationLogRepository.Add(log);
        }

        private static Exception UnsupportedGatewayException()
        {
            return new NotImplementedException("Esta operação não foi implementada para o gateway " + MaaSConfig.Current.Payment.ActiveGateway);
        }

        private class GatewayData
        {
            public IAddress Address { get; set; }
            public IAddress AccountableAddress { get; set; }
            public ISupplier Supplier { get; set; }
            public IBankAccount BankAccount { get; set; }
            public IAccountable Accountable { get; set; }

            public static GatewayData FromViewModelForCurrentGateway(SellerGatewayViewModel viewModel, Seller seller)
            {
                if (MaaSConfig.Current.Payment.Moip.SellerReceivesMoipPayment)
                {
                    var data = new GatewayData
                    {
                        Supplier = new MoipSupplier(viewModel.Moip),
                        Address = new MoipAddress(viewModel.Moip),
                        BankAccount = new MoipBankAccount(viewModel.Moip),
                        Accountable = new MoipAccountable(viewModel.Moip),
                        AccountableAddress = new MoipAccountableAddress(viewModel.Moip)
                    };
                    data.Supplier.SetGatewayTokens(seller.SupplierGatewayId, null, null, seller.GatewayUserToken);
                    return data;
                }
                else if (MaaSConfig.Current.Payment.PagarMe.SellerReceivesPagarmePayment)
                {
                    var data = new GatewayData
                    {
                        Supplier = new PagarmeSupplier(viewModel.Pagarme),
                        BankAccount = new Pagarme.PagarmeBankAccount(viewModel.Pagarme),
                        Accountable = new PagarmeAccountable(viewModel.Pagarme)
                    };
                    data.Supplier.SetGatewayTokens(seller.SupplierGatewayId, null, null, seller.GatewayUserToken);
                    return data;
                }

                throw UnsupportedGatewayException();
            }
        }
    }
}