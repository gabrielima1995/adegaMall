using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using System;

namespace Application.Sellers.Moip
{
    public class MoipSupplier : ISupplier
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

        public MoipSupplier(SellerMoipViewModel vm)
        {
            Name = vm.HasCompany ? vm.Company.BusinnessName : vm.Person.FullName;
            AlternativeName = vm.HasCompany ? vm.Company.Name : string.Empty;
            Email = vm.EmailAddress;
            DocumentNumber = vm.HasCompany ? vm.Company.TaxDocumentNumber : vm.Person.TaxDocumentNumber;
            Phone = vm.HasCompany ? vm.Company.Phone : vm.Person.Phone;
            DateOfBirth = vm.Person.BirthDate;
        }
    }

    public class MoipAddress : IAddress
    {
        public string Street { get; private set; }
        public string StreetNumber { get; private set; }
        public string Complementary { get; private set; }
        public string Neighborhood { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }

        public MoipAddress(SellerMoipViewModel vm)
        {
            var address = vm.HasCompany && !vm.Company.SamePersonAddress
                ? vm.Company.Address
                : vm.Person.Address;

            Street = address.Street;
            StreetNumber = address.StreetNumber;
            Complementary = address.Complement;
            Neighborhood = address.District;

            var aux = address.CityAndState.Split('-');

            City = aux[0].Trim();
            State = aux[1].Trim();

            ZipCode = address.ZipCode;
        }
    }

    public class MoipBankAccount : IBankAccount
    {
        public Bank Bank { get; private set; }
        public AccountType AccountType { get; private set; }
        public string Branch { get; private set; }
        public string Number { get; private set; }

        public MoipBankAccount(SellerMoipViewModel vm)
        {
            var ba = vm.BankAccount;

            Bank = ba.Bank ?? default(Bank);
            AccountType = ba.Type ?? default(AccountType);
            Branch = ba.AgencyNumber;

            if (!string.IsNullOrWhiteSpace(ba.AgencyCheckNumber))
            {
                Branch += "-" + ba.AgencyCheckNumber;
            }

            Number = ba.AccountNumber + "-" + ba.AccountCheckNumber;
        }
    }

    public class MoipAccountableAddress : IAddress
    {
        public string Street { get; private set; }
        public string StreetNumber { get; private set; }
        public string Complementary { get; private set; }
        public string Neighborhood { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }

        public MoipAccountableAddress(SellerMoipViewModel vm)
        {
            var address = vm.Person.Address;

            Street = address.Street;
            StreetNumber = address.StreetNumber;
            Complementary = address.Complement;
            Neighborhood = address.District;

            var aux = address.CityAndState.Split('-');

            City = aux[0].Trim();
            State = aux[1].Trim();

            ZipCode = address.ZipCode;
        }
    }

    public class MoipAccountable : IAccountable
    {
        public string Name { get; private set; }
        public string DocumentNumber { get; private set; }
        public string PhoneNumber { get; private set; }

        public MoipAccountable(SellerMoipViewModel vm)
        {
            Name = vm.Person.FullName;
            DocumentNumber = vm.Person.TaxDocumentNumber;
            PhoneNumber = vm.Person.Phone;
        }
    }
}