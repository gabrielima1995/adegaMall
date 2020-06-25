using PagarMe;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.PagarMe.Common;

namespace PaymentIntegrator.PagarMe.Customers
{
    public static class CustomerHelper
    {
        public static Customer CreateToPagarMe(this ICustomer customer, IAddress address, PagarMeClient client)
        {
            if (customer == null) return null;

            var customerPagarMe = new Customer(client.GetService())
            {
                Id = customer.GatewayId,
                Name = customer.Name,
                DocumentNumber = customer.DocumentNumber,
                Email = customer.Email
            };

            if (!string.IsNullOrEmpty(customer.PhoneNumber))
            {
                var phone = customer.PhoneNumber.ExtractDigits();

                if (phone.StartsWith("55"))
                    phone = phone.Substring(2);
                if (phone.StartsWith("0"))
                    phone = phone.Substring(1);

                customerPagarMe.Phone = new Phone(client.GetService())
                {
                    Ddd = phone.Length > 9 ? phone.Substring(0, 2) : "",
                    Number = phone.Length > 9 ? phone.Substring(2, phone.Length - 2) : phone
                };
            }

            customerPagarMe.Address = address.CreateToPagarMe(client);

            return customerPagarMe;
        }
    }
}