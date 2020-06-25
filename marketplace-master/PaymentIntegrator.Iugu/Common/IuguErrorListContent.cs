using Framework;
using System.Collections.Generic;

namespace PaymentIntegrator.Iugu.Common
{
    public class IuguErrorListContent : IErrorContent
    {
        public Dictionary<string, IList<string>> Errors { get; set; }

        public void MergeErrors(OperationResult validation)
        {
            if (Errors != null && Errors.Count > 0)
            {
                foreach (var error in Errors)
                {
                    foreach (var value in error.Value)
                    {
                        validation.AddError(error.Key, value);
                    }
                }
            }
        }
    }
}