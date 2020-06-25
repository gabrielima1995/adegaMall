using Framework;

namespace PaymentIntegrator.Iugu.Common
{
    public class IuguErrorContent : IErrorContent
    {
        public string Errors { get; set; }

        public void MergeErrors(OperationResult validation)
        {
            validation.AddError("error", Errors);
        }
    }
}