using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Framework;
using PaymentIntegrator.Core.PaymentMethods;
using System;
using System.Globalization;

namespace Domain.Services.StoreModule
{
    public class CreditCardService
    {
        private readonly IPaymentService _paymentService;

        public CreditCardService(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        public OperationResult<ICreditCardResult> CreateOnGateway(Buyer buyer, CreditCard creditCard)
        {
            var errors = new ErrorDictionary();

            errors.Merge(ValidateModel(buyer));
            errors.Merge(ValidateModel(creditCard));

            if (errors.HasErrors)
                return new OperationResult<ICreditCardResult>(errors);

            return _paymentService.CreateCreditCard(buyer, creditCard);
        }

        private ErrorDictionary ValidateModel(Buyer buyer)
        {
            var errors = new ErrorDictionary();
            var requiredStr = CultureInfo.CurrentCulture.GetStringLocalized("REQUIRED_ERROR");

            if (buyer == null)
            {
                errors.Add("", new ArgumentNullException(nameof(buyer)).Message);
            }
            else
            {
                if (buyer.DocumentNumber == null)
                    errors.Add(nameof(buyer.DocumentNumber), string.Format(requiredStr, nameof(buyer.DocumentNumber)));

                if (buyer.Email == null)
                    errors.Add(nameof(buyer.DocumentNumber), string.Format(requiredStr, nameof(buyer.Email)));

                if (buyer.Name == null)
                    errors.Add(nameof(buyer.DocumentNumber), string.Format(requiredStr, nameof(buyer.Name)));

                if (buyer.PhoneNumber == null)
                    errors.Add(nameof(buyer.DocumentNumber), string.Format(requiredStr, nameof(buyer.PhoneNumber)));
            }

            return errors;
        }

        private ErrorDictionary ValidateModel(CreditCard creditCard)
        {
            var errors = new ErrorDictionary();
            var requiredStr = CultureInfo.CurrentCulture.GetStringLocalized("REQUIRED_ERROR");

            if (creditCard == null)
            {
                errors.Add("", new ArgumentNullException(nameof(creditCard)).Message);
            }
            else
            {
                if (creditCard.CVV == null)
                    errors.Add(nameof(creditCard.CVV), string.Format(requiredStr, nameof(creditCard.CVV)));

                if (creditCard.FullName == null)
                    errors.Add(nameof(creditCard.FullName), string.Format(requiredStr, nameof(creditCard.FullName)));

                if (creditCard.Month == 0)
                    errors.Add(nameof(creditCard.Month), string.Format(requiredStr, nameof(creditCard.Month)));

                if (creditCard.Number == null)
                    errors.Add(nameof(creditCard.Number), string.Format(requiredStr, nameof(creditCard.Number)));

                if (creditCard.Year == 0)
                    errors.Add(nameof(creditCard.Year), string.Format(requiredStr, nameof(creditCard.Year)));
            }

            return errors;
        }
    }
}