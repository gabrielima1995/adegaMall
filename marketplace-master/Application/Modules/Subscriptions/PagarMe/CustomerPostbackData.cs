using PaymentIntegrator.Core.Customers;

namespace Application.Modules.Subscriptions.PagarMe
{
    /// <summary>
    /// Dados do objeto Customer no postback enviado pelo Pagar.me.
    /// </summary>
    public class CustomerPostbackData
    {
        public string id { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public CustomerPostbackData()
        {
        }

        public CustomerPostbackData(ICustomer customer)
        {
            id = customer.GatewayId;
            name = customer.Name;
            email = customer.Email;
        }
    }
}