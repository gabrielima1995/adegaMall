using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Suppliers;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Suppliers
{
    public class RequestVerificationRequest : IValidatable
    {
        public RequestVerification Data { get; }

        public bool AutomaticValidation { get; }

        public RequestVerificationRequest(ISupplier supplier, IAddress address, IBankAccount bankAccount, IAccountable accountable)
        {
            AutomaticValidation = true;

            Data = new RequestVerification(supplier, address, bankAccount, accountable);
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            errors.Merge(Data.Validate());

            return errors;
        }
    }
}