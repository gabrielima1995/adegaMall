using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Moip.Common.Models;
using PaymentIntegrator.Moip.Suppliers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Application.Sellers.Moip
{
    public class SellerMoipViewModel : IEquatable<SellerMoipViewModel>
    {
        public bool Enabled { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_HASCOMPANY_LABEL")]
        public bool HasCompany { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_EMAILADDRESS_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [EmailAddressLocalized]
        [StringLengthLocalized(100)]
        public string EmailAddress { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_PERSON_LABEL")]
        public MoipPersonViewModel Person { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_COMPANY_LABEL")]
        public MoipCompanyViewModel Company { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_BANKACCOUNT_LABEL")]
        public MoipBankAccountViewModel BankAccount { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_TERMS1_LABEL")]
        [MustBeTrueIfLocalized("Enabled")]
        public bool Terms1 { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_TERMS2_LABEL")]
        [MustBeTrueIfLocalized("Enabled")]
        public bool Terms2 { get; set; }

        public static SellerMoipViewModel Enable()
        {
            return new SellerMoipViewModel
            {
                Enabled = true,
                Person = new MoipPersonViewModel
                {
                    Enabled = true,
                    Address = new MoipAddressViewModel
                    {
                        Enabled = true
                    }
                },
                Company = new MoipCompanyViewModel
                {
                    SamePersonAddress = true,
                    Address = new MoipAddressViewModel()
                },
                BankAccount = new MoipBankAccountViewModel
                {
                    Enabled = true
                }
            };
        }

        public static SellerMoipViewModel Load(MoipAccountAndBankAccount model)
        {
            if (model == null)
                return new SellerMoipViewModel();

            return new SellerMoipViewModel
            {
                Enabled = true,
                HasCompany = model.Account.Company != null,
                EmailAddress = model.Account.Email.Address,
                Person = new MoipPersonViewModel(model.Account.Person),
                Company = new MoipCompanyViewModel(model.Account.Company, model.Account.Person),
                BankAccount = new MoipBankAccountViewModel(model.BankAccount),
                Terms1 = true,
                Terms2 = true
            };
        }

        public string GetDocumentNumber()
        {
            return Company.TaxDocumentNumber ?? Person.TaxDocumentNumber;
        }

        public bool Equals(SellerMoipViewModel other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public IEnumerable<object> GetEqualityComponents()
        {
            yield return HasCompany;
            yield return EmailAddress;

            if (Person.Enabled)
            {
                if (Person.Address.Enabled)
                {
                    yield return Person.Address.CityAndState;
                    yield return Person.Address.Complement;
                    yield return Person.Address.Country;
                    yield return Person.Address.District;
                    yield return Person.Address.Street;
                    yield return Person.Address.StreetNumber;
                    yield return Person.Address.ZipCode;
                }
                yield return Person.BirthDate;
                yield return Person.FullName;
                yield return Person.LastName;
                yield return Person.Name;
                yield return Person.Phone;
                yield return Person.TaxDocumentNumber;
            }

            if (Company.Enabled)
            {
                if (Company.Address.Enabled)
                {
                    yield return Company.Address.CityAndState;
                    yield return Company.Address.Complement;
                    yield return Company.Address.Country;
                    yield return Company.Address.District;
                    yield return Company.Address.Street;
                    yield return Company.Address.StreetNumber;
                    yield return Company.Address.ZipCode;
                }
                yield return Company.BusinnessName;
                yield return Company.Name;
                yield return Company.Phone;
            }

            if (BankAccount.Enabled)
            {
                yield return BankAccount.AccountCheckNumber;
                yield return BankAccount.AccountNumber;
                yield return BankAccount.AgencyCheckNumber;
                yield return BankAccount.AgencyNumber;
                yield return BankAccount.Bank;
                yield return BankAccount.Type;
            }
        }

        public bool IsFilled()
        {
            return Enabled &&
                EmailAddress != null &&
                (Person?.IsFilled() ?? false) &&
                ((Company?.IsFilled() ?? false) || !HasCompany) &&
                BankAccount.IsFilled() &&
                Terms1 && Terms2;
        }
    }

    public class MoipPersonViewModel
    {
        private string _taxDocumentNumber;

        public bool Enabled { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_PERSON_NAME_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(1024)]
        public string Name { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_PERSON_LASTNAME_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(1024)]
        public string LastName { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_PERSON_TAXDOCUMENTNUMBER_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [DocumentNumberLocalized("MOIP_SELLER_PERSON_TAXDOCUMENTNUMBER_ERROR", forceCheck: true, canBeCpf: true, canBeCnpj: false)]
        public string TaxDocumentNumber
        {
            get => _taxDocumentNumber;
            set => _taxDocumentNumber = value?.ExtractDigits();
        }

        [DisplayNameLocalized("MOIP_SELLER_PERSON_BIRTHDATE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_PERSON_PHONE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [PhoneNumberLocalized]
        public string Phone { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_PERSON_ADDRESS_LABEL")]
        public MoipAddressViewModel Address { get; set; }

        public string FullName => Name + " " + LastName;

        public MoipPersonViewModel()
        {
        }

        public MoipPersonViewModel(Person person)
        {
            Enabled = true;
            Name = person.Name;
            LastName = person.LastName;
            TaxDocumentNumber = person?.TaxDocument.Number.ExtractDigits();
            BirthDate = DateTime.Parse(person.BirthDate, CultureInfo.GetCultureInfo("en-us"));
            Phone = person.Phone.AreaCode + " " + person?.Phone.Number;
            Address = new MoipAddressViewModel(person.Address);
        }

        public bool IsFilled()
        {
            return Enabled &&
                Name != null &&
                TaxDocumentNumber != null &&
                BirthDate != null &&
                Phone != null &&
                Address.IsFilled();
        }
    }

    public class MoipCompanyViewModel
    {
        private string _taxDocumentNumber;

        public bool Enabled { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_COMPANY_NAME_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(1024)]
        public string Name { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_COMPANY_BUSINESSNAME_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(1024)]
        public string BusinnessName { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_COMPANY_TAXDOCUMENTNUMBER_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [DocumentNumberLocalized("MOIP_SELLER_COMPANY_TAXDOCUMENTNUMBER_ERROR", forceCheck: true, canBeCpf: false, canBeCnpj: true)]
        public string TaxDocumentNumber
        {
            get => _taxDocumentNumber;
            set => _taxDocumentNumber = value?.ExtractDigits();
        }

        [DisplayNameLocalized("MOIP_SELLER_COMPANY_PHONE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [PhoneNumberLocalized]
        public string Phone { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_COMPANY_SAMEPERSONADDRESS_LABEL")]
        public bool SamePersonAddress { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_COMPANY_ADDRESS_LABEL")]
        public MoipAddressViewModel Address { get; set; }

        public MoipCompanyViewModel()
        {
        }

        public MoipCompanyViewModel(Company company, Person person)
        {
            if (company == null)
                Address = new MoipAddressViewModel();

            else
            {
                Enabled = true;
                Name = company.Name;
                BusinnessName = company.BusinessName;
                TaxDocumentNumber = company.TaxDocument.Number.ExtractDigits();
                Phone = company.Phone.AreaCode + " " + company.Phone.Number;
                SamePersonAddress =
                    company.Address.Complement == person.Address.Complement &&
                    company.Address.ZipCode == person.Address.ZipCode &&
                    company.Address.Street == person.Address.Street &&
                    company.Address.State == person.Address.State &&
                    company.Address.StreetNumber == person.Address.StreetNumber &&
                    company.Address.District == person.Address.District &&
                    company.Address.Country == person.Address.Country &&
                    company.Address.City == person.Address.City;
                Address = new MoipAddressViewModel(company.Address);
            }
        }

        public bool IsFilled()
        {
            return !Enabled || (
                Name != null &&
                BusinnessName != null &&
                TaxDocumentNumber != null &&
                Phone != null &&
                Address.IsFilled());
        }
    }

    public class MoipAddressViewModel
    {
        public bool Enabled { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_ADDRESS_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(1024)]
        public string Street { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_ADDRESS_NUMBER_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(20)]
        public string StreetNumber { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_ADDRESS_COMPLEMENT_LABEL")]
        [StringLengthLocalized(30)]
        public string Complement { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_ADDRESS_DISTRICT_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(100)]
        public string District { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_ADDRESS_ZIPCODE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(15)]
        public string ZipCode { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_ADDRESS_CITYSTATE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        [StringLengthLocalized(100)]
        public string CityAndState { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_ADDRESS_COUNTRY_LABEL")]
        public string Country { get; set; } = "Brasil";

        public MoipAddressViewModel()
        {
        }

        public MoipAddressViewModel(Address address)
        {
            Enabled = true;
            Street = address.Street;
            StreetNumber = address.StreetNumber;
            Complement = address.Complement;
            District = address.District;
            ZipCode = address.ZipCode;
            CityAndState = address.City + " - " + address.State;
        }

        public bool IsFilled()
        {
            return Enabled &&
                Street != null &&
                StreetNumber != null &&
                ZipCode != null &&
                CityAndState != null;
        }
    }

    public class MoipBankAccountViewModel
    {
        public bool Enabled { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_BANKACCOUNT_BANK_LABEL")]
        [RequiredIfLocalized("Enabled")]
        public Bank? Bank { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_BANKACCOUNT_TYPE_LABEL")]
        [RequiredIfLocalized("Enabled")]
        public AccountType? Type { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_BANKACCOUNT_AGENCYNUMBER_LABEL")]
        [StringLengthLocalized(10)]
        [RequiredIfLocalized("Enabled")]
        public string AgencyNumber { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_BANKACCOUNT_AGENCYCHECKNUMBER_LABEL")]
        [StringLengthLocalized(4)]
        public string AgencyCheckNumber { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_BANKACCOUNT_ACCOUNTNUMBER_LABEL")]
        [StringLengthLocalized(10)]
        [RequiredIfLocalized("Enabled")]
        public string AccountNumber { get; set; }

        [DisplayNameLocalized("MOIP_SELLER_BANKACCOUNT_ACCOUNTCHECKNUMBER_LABEL")]
        [StringLengthLocalized(10)]
        [RequiredIfLocalized("Enabled")]
        public string AccountCheckNumber { get; set; }

        public MoipBankAccountViewModel()
        {
        }

        public MoipBankAccountViewModel(BankAccount bankAccount)
        {
            Enabled = true;
            Bank = (Bank)Enum.Parse(typeof(Bank), bankAccount.BankNumber);
            Type = bankAccount.Type == "SAVING" ? AccountType.Poupanca : AccountType.ContaCorrente;
            AgencyNumber = bankAccount.AgencyNumber;
            AgencyCheckNumber = bankAccount.AgencyCheckNumber;
            AccountNumber = bankAccount.AccountNumber;
            AccountCheckNumber = bankAccount.AccountCheckNumber;
        }

        public bool IsFilled()
        {
            return Enabled &&
                Bank != null &&
                Type != null &&
                AgencyNumber != null &&
                AccountNumber != null;
        }
    }
}