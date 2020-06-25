using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using System;

namespace Application.Sellers.Pagarme
{
    public class PagarmeSupplier : ISupplier
    {
        public string Name { get; private set; }
        public string AlternativeName { get; private set; }
        public string Email { get; private set; }
        public string BusinessDescription { get; private set; }
        public string DocumentNumber { get; private set; }
        public string Phone { get; private set; }
        public string GatewayId { get; private set; }
        public string GatewayLiveApiToken { get; private set; }
        public string GatewayTestApiToken { get; private set; }
        public string GatewayUserToken { get; private set; }
        public DateTime? DateOfBirth { get; private set; }

        public bool IsCompany()
        {
            return DocumentNumber?.Length == 14;
        }

        public void SetGatewayTokens(string id, string liveApiToken, string testApiToken, string userToken)
        {
            GatewayId = id;
            GatewayLiveApiToken = liveApiToken;
            GatewayTestApiToken = testApiToken;
            GatewayUserToken = userToken;
        }

        public PagarmeSupplier(SellerPagarmeViewModel vm)
        {
            Name = vm.LegalName;
            DocumentNumber = vm.DocumentNumber;
        }
    }

    public class PagarmeBankAccount : IBankAccount
    {
        public Bank Bank { get; private set; }
        public AccountType AccountType { get; private set; }
        public string Branch { get; private set; }
        public string Number { get; private set; }

        public PagarmeBankAccount(SellerPagarmeViewModel ba)
        {
            Bank = ba.BankCode ?? default(Bank);
            AccountType = ba.Type ?? default(AccountType);
            Branch = ba.Agencia;

            if (!string.IsNullOrWhiteSpace(ba.AgenciaDv))
            {
                Branch += "-" + ba.AgenciaDv;
            }

            Number = ba.Conta + "-" + ba.ContaDv;
        }
    }

    public class PagarmeAccountable : IAccountable
    {
        public string Name { get; private set; }
        public string DocumentNumber { get; private set; }
        public string PhoneNumber { get; private set; }

        public PagarmeAccountable(SellerPagarmeViewModel vm)
        {
            Name = vm.LegalName;
            DocumentNumber = vm.DocumentNumber;
        }
    }
}