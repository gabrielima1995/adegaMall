using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Iugu.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentIntegrator.Iugu.Payments
{
    public class ChargeRequest : IValidatable
    {
        public string Method { get; set; }

        public string Token { get; }

        [JsonProperty("customer_payment_method_id")]
        public string CustomerPaymentMethodId { get; set; }

        public string Email { get; }

        public string Months { get; set; }

        public Payer Payer { get; set; }

        public IList<InvoiceItem> Items { get; }

        public ChargeRequest(IPaymentItem paymentItem, ICustomer customer, IAddress customerAddress, IPaymentOptions paymentOptions)
            : this(new List<IPaymentItem> { paymentItem }, customer, customerAddress, paymentOptions)
        {
        }

        public ChargeRequest(IList<IPaymentItem> paymentItems, ICustomer customer, IAddress customerAddress, IPaymentOptions paymentOptions)
        {
            switch (paymentOptions.Method)
            {
                case Core.PaymentMethods.PaymentMethod.CreditCard:
                    CustomerPaymentMethodId = paymentOptions.CreditCardResult.ReferenceId;
                    Months = paymentOptions.Installments.ToString();

                    if (string.IsNullOrEmpty(CustomerPaymentMethodId))
                        Token = paymentOptions.CreditCardResult.Token;

                    break;

                case Core.PaymentMethods.PaymentMethod.Boleto:
                    Method = "bank_slip";

                    if (customerAddress == null)
                        throw new ArgumentException("When the payment method is bank slip, the customer address is mandatory.");

                    break;

                default:
                    throw new InvalidOperationException($"Payment method {paymentOptions.Method} is not supported.");
            }

            Email = customer.Email;
            Items = paymentItems.Select(i => new InvoiceItem(i)).ToList();

            string phonePrefix, phoneNumber;
            PhoneNumberHelper.BreakPhoneNumber(customer.PhoneNumber, out phonePrefix, out phoneNumber);

            Payer = new Payer
            {
                CpfCnpj = customer.DocumentNumber,
                Email = customer.Email,
                Name = customer.Name,
                PhonePrefix = phonePrefix,
                Phone = phoneNumber,
                Address = new PayerAddress
                {
                    Street = customerAddress?.Street,
                    Number = customerAddress?.StreetNumber,
                    Complement = customerAddress?.Complementary,
                    District = customerAddress?.Neighborhood,
                    City = customerAddress?.City,
                    State = customerAddress?.State,
                    ZipCode = customerAddress?.ZipCode
                }
            };
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (string.IsNullOrEmpty(Email))
                errors.Add(nameof(Email), nameof(Email) + " é obrigatório.");

            foreach (var item in Items)
                errors.Merge(item.Validate());

            return errors;
        }
    }
}