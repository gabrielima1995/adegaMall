using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Customers
{
    public class CustomerRequest : IValidatable
    {
        public string Email { get; }

        public string Name { get; }

        [JsonProperty("cpf_cnpj")]
        public string CpfCnpj { get; }

        public CustomerRequest(ICustomer customer)
        {
            Email = customer.Email;
            Name = customer.Name;
            CpfCnpj = customer.DocumentNumber;
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Email))
            {
                errors.Add(nameof(Email), nameof(Email) + " é obrigatório.");
            }

            return errors;
        }
    }
}