using Framework;

namespace PaymentIntegrator.Iugu.Common
{
    public interface IValidatable
    {
        ErrorDictionary Validate();
    }
}