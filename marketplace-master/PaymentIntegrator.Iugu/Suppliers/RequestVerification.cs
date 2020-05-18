using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Iugu.Common;
using System;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class RequestVerification : IValidatable
    {
        /// <summary>
        /// Valor máximo da venda('Até R$ 100,00', 'Entre R$ 100,00 e R$ 500,00', 'Mais que R$ 500,00')
        /// </summary>
        [JsonProperty("price_range")]
        public string PriceRange { get; set; }

        /// <summary>
        /// Vende produtos físicos
        /// </summary>
        [JsonProperty("physical_products")]
        public bool PhysicalProducts { get; set; }

        /// <summary>
        /// Descrição do negócio
        /// </summary>
        [JsonProperty("business_type")]
        public string BusinessType { get; set; }

        /// <summary>
        /// 'Pessoa Física' ou 'Pessoa Jurídica'
        /// </summary>
        [JsonProperty("person_type")]
        public string PersonType { get; set; }

        /// <summary>
        /// Saque automático(Recomendamos que envie 'true')
        /// </summary>
        [JsonProperty("automatic_transfer")]
        public bool AutomaticTransfer { get; set; }

        /// <summary>
        /// CNPJ caso Pessoa Jurídica
        /// </summary>
        [JsonProperty("cnpj")]
        public string CNPJ { get; set; }

        /// <summary>
        /// Nome da Empresa, caso Pessoa Jurídica
        /// </summary>
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        /// <summary>
        /// CPF caso Pessoa Física
        /// </summary>
        [JsonProperty("cpf")]
        public string CPF { get; set; }

        /// <summary>
        /// Nome caso Pessoa Física
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Endreço Completo
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Cep
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Telefone comercial
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// Nome do Responsável, caso Pessoa Jurídica
        /// </summary>
        [JsonProperty("resp_name")]
        public string RespName { get; set; }

        /// <summary>
        /// CPF do Responsável, caso Pessoa Jurídica
        /// </summary>
        [JsonProperty("resp_cpf")]
        public string RespCPF { get; set; }

        /// <summary>
        /// Nome da instituição bancária
        /// Suportados : 'Itaú', 'Bradesco', 'Caixa Econômica', 'Banco do Brasil', 'Santander'
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// Agência da Conta
        /// Formatos com validação automática(9999-D, 9999)
        /// </summary>
        [JsonProperty("bank_ag")]
        public string BankAg { get; set; }

        /// <summary>
        /// Tipo da conta: 'Corrente', 'Poupança'
        /// </summary>
        [JsonProperty("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        /// Número da Conta
        /// Formatos com validação automática(99999999-D, XXX99999999-D onde X é Operação, 	9999999-D, 99999-D)
        /// </summary>
        [JsonProperty("bank_cc")]
        public string BankCC { get; set; }

        public RequestVerification()
        {
        }

        public RequestVerification(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable)
        {
            PriceRange = "Mais que R$ 500,00";
            BusinessType = supplier.BusinessDescription;
            PersonType = !supplier.IsCompany()
                ? "Pessoa Física"
                : "Pessoa Jurídica";
            AutomaticTransfer = true;
            Address = $"{address.Street}, {address.StreetNumber} - {address.Neighborhood} - {address.City} - {address.State}";
            Cep = address.ZipCode;
            City = address.City;
            State = address.State;
            Telephone = supplier.Phone;
            Bank = GetBankName(bankAccount.Bank);
            BankAg = bankAccount.Branch;
            AccountType = bankAccount.AccountType == Core.Common.AccountType.ContaCorrente
                ? "Corrente"
                : "Poupança";
            BankCC = bankAccount.Number;

            if (supplier.IsCompany())
            {
                if (accountable == null)
                    throw new ArgumentException("If the supplier represents a company, data for an accountable person must be supplied.");

                CNPJ = supplier.DocumentNumber;
                CompanyName = supplier.Name;
                RespName = accountable.Name;
                RespCPF = accountable.DocumentNumber;
            }
            else
            {
                CPF = supplier.DocumentNumber;
                Name = supplier.Name;
            }
        }

        private string GetBankName(Bank bank)
        {
            switch (bank)
            {
                case Core.Common.Bank.ItauUnibanco:
                    return "Itaú";

                case Core.Common.Bank.Bradesco:
                    return "Bradesco";

                case Core.Common.Bank.CaixaEconomicaFederal:
                    return "Caixa Econômica";

                case Core.Common.Bank.BancodoBrasil:
                    return "Banco do Brasil";

                case Core.Common.Bank.BancoSantander:
                    return "Santander";

                case Core.Common.Bank.BancodoEstadodoRioGrandedoSul:
                    return "Banrisul";

                case Core.Common.Bank.BancoCooperativoSicrediBANSICREDI:
                    return "Sicredi";

                case Core.Common.Bank.BancoCooperativodoBrasilBANCOOB:
                    return "SICOOB";
            }

            return "";
        }

        public bool IsPessoaJuridica()
        {
            return PersonType == "Pessoa Jurídica";
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(PriceRange))
            {
                errors.Add(nameof(PriceRange), nameof(PriceRange) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(BusinessType))
            {
                errors.Add(nameof(BusinessType), nameof(BusinessType) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(PersonType))
            {
                errors.Add(nameof(PersonType), nameof(PersonType) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Address))
            {
                errors.Add(nameof(Address), nameof(Address) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Cep))
            {
                errors.Add(nameof(Cep), nameof(Cep) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(City))
            {
                errors.Add(nameof(City), nameof(City) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(State))
            {
                errors.Add(nameof(State), nameof(State) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Bank))
            {
                errors.Add(nameof(Bank), nameof(Bank) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(BankAg))
            {
                errors.Add(nameof(BankAg), nameof(BankAg) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(AccountType))
            {
                errors.Add(nameof(AccountType), nameof(AccountType) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(BankCC))
            {
                errors.Add(nameof(BankCC), nameof(BankCC) + " é obrigatório.");
            }

            if (IsPessoaJuridica())
            {
                if (string.IsNullOrEmpty(CNPJ))
                {
                    errors.Add(nameof(CNPJ), nameof(CNPJ) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(CompanyName))
                {
                    errors.Add(nameof(CompanyName), nameof(CompanyName) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(RespName))
                {
                    errors.Add(nameof(RespName), nameof(RespName) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(RespCPF))
                {
                    errors.Add(nameof(RespCPF), nameof(RespCPF) + " é obrigatório.");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(CPF))
                {
                    errors.Add(nameof(CPF), nameof(CPF) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(Name))
                {
                    errors.Add(nameof(Name), nameof(Name) + " é obrigatório.");
                }
            }

            return errors;
        }
    }
}