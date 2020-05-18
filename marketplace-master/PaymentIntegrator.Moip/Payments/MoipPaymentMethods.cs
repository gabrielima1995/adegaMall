using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Moip.Helpers;
using System;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip.Payments
{
    public static class MoipPaymentMethods
    {
        public static object CreatePaymentMethod(IPaymentOptions options, ICustomer customer)
        {
            switch (options.Method)
            {
                case Core.PaymentMethods.PaymentMethod.CreditCard:
                    return CreateCreditCardPayment(options, customer);

                case Core.PaymentMethods.PaymentMethod.Boleto:
                    return CreateBankslipPayment(options, customer);

                case Core.PaymentMethods.PaymentMethod.Offline:
                default:
                    throw new InvalidOperationException("Unsupported payment method");
            }
        }

        private static object CreateCreditCardPayment(IPaymentOptions options, ICustomer customer)
        {
            Regex areaCodeRegex = new Regex(@"\(?([1-9]{2})\)?\s?([0-9\-]*)$");
            var match = areaCodeRegex.Match(customer.PhoneNumber);
            string areaCode = match.Groups[1].Value;
            string number = match.Groups[2].Value;
            string document = options.CreditCard.DocumentNumber.Replace("-", "").Replace("/", "").Replace(".", "").Replace(" ", "").Replace("\\", "");

            var obj = new
            {
                method = "CREDIT_CARD",
                creditCard = new
                {
                    hash = EncryptionHelper.EncryptCard(new
                    {
                        number = options.CreditCard.Number,
                        expirationMonth = options.CreditCard.Month,
                        expirationYear = options.CreditCard.Year,
                        cvc = options.CreditCard.CVV,
                    }),
                    holder = new
                    {
                        fullname = options.CreditCard.FullName,
                        birthdate = options.CreditCard.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                        phone = new
                        {
                            countryCode = "55",
                            areaCode,
                            number,
                        },
                        taxDocument = new
                        {
                            type = document.Length == 11 ? "CPF" : "CNPJ",
                            number = DocumentHelper.FormatDocument(document),
                        }
                    }
                },
            };

            return obj;
        }

        private static object CreateBankslipPayment(IPaymentOptions options, ICustomer customer)
        {
            var obj = new
            {
                method = "BOLETO",
                boleto = new
                {
                    expirationDate = DateTime.UtcNow.AddDays(2).GetBusinessDay().ToString("yyyy-MM-dd"),
                },
            };

            return obj;
        }
    }
}
