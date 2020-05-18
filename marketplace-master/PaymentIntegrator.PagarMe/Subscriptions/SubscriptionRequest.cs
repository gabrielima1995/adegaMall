using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.PagarMe.Common;
using PaymentIntegrator.PagarMe.Customers;
using PaymentIntegrator.PagarMe.PaymentMethods;
using PaymentIntegrator.PagarMe.Plans;
using PagarMeSdk = PagarMe;

namespace PaymentIntegrator.PagarMe.Subscriptions
{
    /// <summary>
    /// Representa uma assinatura para salvar no Pagar.me.
    /// </summary>
    public class SubscriptionRequest : PagarMeSdk.Subscription, IValidatable
    {
        public SubscriptionRequest(PagarMeClient client, IPlan plan, ICustomer customer, IAddress address,
           ICreditCardResult creditCardStatus, string postbackUrl)
        {
            Plan = plan.CreateToPagarMe(client);

            Card = creditCardStatus.AsPagarmeCard(client);

            PostbackUrl = postbackUrl;
            PaymentMethod = PagarMeSdk.PaymentMethod.CreditCard;

            Customer = customer.CreateToPagarMe(address, client);
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Plan?.Id))
            {
                errors.Add(nameof(Plan), nameof(Plan) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(Card?.Id))
            {
                errors.Add(nameof(Card), nameof(Card) + " é obrigatório.");
            }

            if (string.IsNullOrEmpty(PostbackUrl))
            {
                errors.Add(nameof(PostbackUrl), nameof(PostbackUrl) + " é obrigatório.");
            }

            #region Customer

            if (Customer == null)
            {
                errors.Add(nameof(Customer), nameof(Customer) + " é obrigatório.");
            }
            else
            {
                if (string.IsNullOrEmpty(Customer.Name))
                {
                    errors.Add(nameof(Customer) + "." + nameof(Customer.Name), nameof(Customer) + "." + nameof(Customer.Name) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(Customer.Email))
                {
                    errors.Add(nameof(Customer) + "." + nameof(Customer.Email), nameof(Customer) + "." + nameof(Customer.Email) + " é obrigatório.");
                }

                if (PagarMeConfig.AntiFraudEnabled)
                {
                    if (string.IsNullOrEmpty(Customer.DocumentNumber))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.DocumentNumber), nameof(Customer) + "." + nameof(Customer.DocumentNumber) + " é obrigatório.");
                    }
                }
            }

            #endregion Customer

            if (PagarMeConfig.AntiFraudEnabled)
            {
                #region Address

                if (Customer?.Address == null)
                {
                    errors.Add(nameof(Customer) + "." + nameof(Customer.Address), nameof(Customer) + "." + nameof(Customer.Address) + " é obrigatório.");
                }
                else
                {
                    if (string.IsNullOrEmpty(Customer.Address.Street))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.Street), nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.Street) + " é obrigatório.");
                    }

                    if (string.IsNullOrEmpty(Customer.Address.StreetNumber))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.StreetNumber), nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.StreetNumber) + " é obrigatório.");
                    }

                    if (string.IsNullOrEmpty(Customer.Address.City))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.City), nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.City) + " é obrigatório.");
                    }

                    if (string.IsNullOrEmpty(Customer.Address.State))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.State), nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.State) + " é obrigatório.");
                    }

                    if (string.IsNullOrEmpty(Customer.Address.Neighborhood))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.Neighborhood), nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.Neighborhood) + " é obrigatório.");
                    }

                    if (string.IsNullOrEmpty(Customer.Address.Zipcode))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.Zipcode), nameof(Customer) + "." + nameof(Customer.Address) + "." + nameof(Customer.Address.Zipcode) + " é obrigatório.");
                    }
                }

                #endregion Address

                #region Phone

                if (Customer?.Phone == null)
                {
                    errors.Add(nameof(Customer) + "." + nameof(Customer.Phone), nameof(Customer) + "." + nameof(Customer.Phone) + " é obrigatório.");
                }
                else
                {
                    if (string.IsNullOrEmpty(Customer.Phone.Ddd))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Phone) + "." + nameof(Customer.Phone.Ddd), nameof(Customer) + "." + nameof(Customer.Phone) + "." + nameof(Customer.Phone.Ddd) + " é obrigatório.");
                    }

                    if (string.IsNullOrEmpty(Customer.Phone.Number))
                    {
                        errors.Add(nameof(Customer) + "." + nameof(Customer.Phone) + "." + nameof(Customer.Phone.Number), nameof(Customer) + "." + nameof(Customer.Phone) + "." + nameof(Customer.Phone.Number) + " é obrigatório.");
                    }
                }

                #endregion Phone
            }

            return errors;
        }
    }
}