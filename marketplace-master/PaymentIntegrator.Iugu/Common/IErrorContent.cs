using Framework;

namespace PaymentIntegrator.Iugu.Common
{
    public interface IErrorContent
    {
        void MergeErrors(OperationResult validation);
    }
}