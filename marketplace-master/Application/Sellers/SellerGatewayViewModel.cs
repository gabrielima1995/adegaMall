using Application.Config;
using Application.Sellers.Moip;
using Application.Sellers.Pagarme;
using Domain.Entities;
using System;

namespace Application.Sellers
{
    public class SellerGatewayViewModel
    {
        public string SellerId { get; set; }
        public bool GatewayRegistered { get; set; }
        public SellerMoipViewModel Moip { get; set; }
        public SellerPagarmeViewModel Pagarme { get; set; }
        public Address SellerAddress { get; set; }

        public bool SellerReceivesPayment => SellerReceivesMoipPayment || SellerReceivesPagarmePayment;
        public bool SellerReceivesMoipPayment =>
            MaaSConfig.Current.Product.Enabled &&
            MaaSConfig.Current.Payment.Enabled &&
            MaaSConfig.Current.Payment.GatewayIsWirecard() &&
            (MaaSConfig.Current.Payment.Moip.CreditCard.Enabled || MaaSConfig.Current.Payment.Moip.Boleto.Enabled);
        public bool SellerReceivesPagarmePayment =>
            MaaSConfig.Current.Product.Enabled &&
            MaaSConfig.Current.Payment.Enabled &&
            MaaSConfig.Current.Payment.GatewayIsPagarme() &&
            (MaaSConfig.Current.Payment.PagarMe.CreditCard.Enabled || MaaSConfig.Current.Payment.PagarMe.Boleto.Enabled);

        public SellerGatewayViewModel()
        {
            if (SellerReceivesMoipPayment)
                Moip = SellerMoipViewModel.Enable();

            if (SellerReceivesPagarmePayment)
                Pagarme = SellerPagarmeViewModel.Enable();
        }

        public SellerGatewayViewModel(Seller seller)
            : this()
        {
            SellerId = seller.Id;
            GatewayRegistered = !string.IsNullOrEmpty(seller.SupplierGatewayId);
        }

        public void Update(Seller seller)
        {
            if (SellerReceivesMoipPayment)
                Moip.ToSeller(seller);

            else if (SellerReceivesPagarmePayment)
                Pagarme.ToSeller(seller);
        }

        public bool IsDifferentBankAccount(SellerGatewayViewModel other)
        {
            if (SellerReceivesMoipPayment)
                return !Moip.Equals(other.Moip);

            else if (SellerReceivesPagarmePayment)
                return !Pagarme.Equals(other.Pagarme);

            throw UnsupportedGatewayException();
        }

        public bool IsDifferentBankAccountWithSameDocument(SellerGatewayViewModel other)
        {
            if (SellerReceivesMoipPayment)
                return !Moip.Equals(other.Moip) && Moip.GetDocumentNumber() == other.Moip.GetDocumentNumber();

            else if (SellerReceivesPagarmePayment)
                return !Pagarme.Equals(other.Pagarme) && Pagarme.GetDocumentNumber() == other.Pagarme.GetDocumentNumber();

            throw UnsupportedGatewayException();
        }

        private Exception UnsupportedGatewayException()
        {
            return new NotImplementedException($"Esta operação não foi implementada para o gateway '{MaaSConfig.Current.Payment.ActiveGateway}'.");
        }
    }
}