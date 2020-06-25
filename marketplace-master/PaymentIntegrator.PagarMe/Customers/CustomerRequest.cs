using Framework;
using PagarMe;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.PagarMe.Common;

namespace PaymentIntegrator.PagarMe.Customers
{
    /// <summary>
    /// Representa um comprador/cliente para salvar no Pagar.me.
    /// </summary>
    public class CustomerRequest : Customer, IValidatable
    {
        public CustomerRequest(PagarMeClient client, ICustomer customer, IAddress address)
            : base(client.GetService())
        {
            Id = customer.GatewayId;
            DocumentNumber = customer.DocumentNumber?.Replace(".", "")?.Replace("-", "")?.Replace("/", "");
            Name = customer.Name;
            Email = customer.Email;

            if (address != null)
            {
                Address = new Address
                {
                    Street = address.Street,
                    StreetNumber = address.StreetNumber,
                    Neighborhood = address.Neighborhood,
                    City = address.City,
                    State = address.State,
                    Zipcode = address.ZipCode?.Replace(".", "")?.Replace("-", "")
                };
            }
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(DocumentNumber))
            {
                errors.Add(nameof(DocumentNumber), nameof(DocumentNumber) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Name))
            {
                errors.Add(nameof(Name), nameof(Name) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Email))
            {
                errors.Add(nameof(Email), nameof(Email) + " é obrigatório.");
            }

            if (Address != null)
            {
                if (string.IsNullOrEmpty(Address.Street))
                {
                    errors.Add(nameof(Address.Street), nameof(Address.Street) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(Address.StreetNumber))
                {
                    errors.Add(nameof(Address.StreetNumber), nameof(Address.StreetNumber) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(Address.Neighborhood))
                {
                    errors.Add(nameof(Address.Neighborhood), nameof(Address.Neighborhood) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(Address.Zipcode))
                {
                    errors.Add(nameof(Address.Zipcode), nameof(Address.Zipcode) + " é obrigatório.");
                }
            }

            return errors;
        }
    }
}