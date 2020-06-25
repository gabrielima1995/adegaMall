using PagarMe;
using PaymentIntegrator.Core.Customers;
using System;

namespace PaymentIntegrator.PagarMe.Customers
{
    /// <summary>
    /// Representa um cliente já salvo no Pagar.me.
    /// </summary>
    public class CustomerResponse : ICustomer
    {
        public string Email { get; private set; }

        public string Name { get; private set; }

        public string DocumentNumber { get; private set; }

        public string PhoneNumber { get; private set; }

        public string GatewayId { get; private set; }

        public DateTime? DateOfBirth { get; private set; }

        public string SubscriberId => throw new NotImplementedException();

        public string AccountableDocument => throw new NotImplementedException();

        public void SetGatewayId(string id)
        {
            GatewayId = id;
        }

        public void SetSubscriberId(string id)
        {
            throw new NotImplementedException();
        }

        public CustomerResponse(Customer customer)
        {
            Email = customer.Email;
            Name = customer.Name;
            DocumentNumber = customer.DocumentNumber;

            if (customer.Phone != null)
            {
                PhoneNumber = customer.Phone.Ddd + " " + customer.Phone.Number;
            }

            GatewayId = customer.Id;
        }
    }
}