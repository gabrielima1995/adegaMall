using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.PagarMe.Common;
using PaymentIntegrator.PagarMe.Customers;
using PaymentIntegrator.PagarMe.PaymentMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using PagarMeSdk = PagarMe;

namespace PaymentIntegrator.PagarMe.Payments
{
    /// <summary>
    /// Representa uma transação para salvar no Pagar.me.
    /// </summary>
    public class TransactionRequest : PagarMeSdk.TransactionCustom, IValidatable
    {
        public IList<IOrder> Orders { get; }

        public TransactionRequest(PagarMeClient client, IOrder order, ICustomer customer, IAddress address, IPaymentOptions paymentOptions)
            : this(client, new List<IOrder>(1) { order }, customer, address, paymentOptions)
        {
        }

        public TransactionRequest(PagarMeClient client, IList<IOrder> orders, ICustomer customer, IAddress address, IPaymentOptions paymentOptions)
            : base(client.GetService())
        {
            switch (paymentOptions.Method)
            {
                case Core.PaymentMethods.PaymentMethod.CreditCard:
                    Async = false;
                    Card = paymentOptions.CreditCardResult.AsPagarmeCard(client);
                    PaymentMethod = PagarMeSdk.PaymentMethod.CreditCard;
                    Installments = paymentOptions.Installments;
                    break;

                case Core.PaymentMethods.PaymentMethod.Boleto:
                    Async = false;
                    PaymentMethod = PagarMeSdk.PaymentMethod.Boleto;
                    Installments = 1;
                    PostbackUrl = paymentOptions.PostbackUrl;
                    break;

                default:
                    throw new InvalidOperationException($"Payment method {paymentOptions.Method} is not handled.");
            }

            Orders = orders;
            Amount = (int)(Orders.GetAmount() * 100);
            SoftDescriptor = PagarMeConfig.SoftDescriptor;
            Metadata = new PaymentMetadataRequest(client.GetService(), Orders);
            Customer = customer.CreateToPagarMe(address, client);
            SplitRules = Orders.GenerateSplitRulesOrNull();
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (Amount <= 0)
                errors.Add(nameof(Amount), nameof(Amount) + " é obrigatório.");

            if ((PaymentMethod == PagarMeSdk.PaymentMethod.CreditCard || PaymentMethod == PagarMeSdk.PaymentMethod.DebitCard) && string.IsNullOrEmpty(Card?.Id))
                errors.Add(nameof(Card), nameof(Card) + " é obrigatório.");

            if (PaymentMethod == PagarMeSdk.PaymentMethod.Boleto && string.IsNullOrEmpty(PostbackUrl))
                errors.Add(nameof(PostbackUrl), nameof(PostbackUrl) + " é obrigatório.");

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

                if (string.IsNullOrEmpty(Customer.DocumentNumber))
                {
                    errors.Add(nameof(Customer) + "." + nameof(Customer.DocumentNumber), nameof(Customer) + "." + nameof(Customer.DocumentNumber) + " é obrigatório.");
                }

                if (string.IsNullOrEmpty(Customer.Email))
                {
                    errors.Add(nameof(Customer) + "." + nameof(Customer.Email), nameof(Customer) + "." + nameof(Customer.Email) + " é obrigatório.");
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

            if (Orders == null || !Orders.Any())
            {
                errors.Add(nameof(Orders), nameof(Orders) + " é obrigatório.");
            }
            else
            {
                foreach (var item in Orders)
                    errors.Merge(item.Validate());
            }

            return errors;
        }
    }
}