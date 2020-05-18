using PagarMe;

namespace PaymentIntegrator.PagarMe.Payments
{
    internal class CustomTransfer : Transfer
    {
        public CustomTransfer(PagarMeService service) : base(service)
        {
        }

        public CustomTransfer() : base()
        {
        }

        public void SetPayableIdMetadata(string id)
        {
            var data = new { id };
            SetAttribute("metadata", data);
        }

        public object GetMetadata()
        {
            return GetAttribute<object>("metadata");
        }
    }

    public static class TransferExtensions
    {
        public static bool IsAssociatedToPayable(this Transfer transfer, string payableId)
        {
            var dict = transfer.ToDictionary(PagarMeUtils.FullSerialization);

            if (dict.ContainsKey("metadata"))
            {
                dynamic metadataValue = dict["metadata"];

                if (metadataValue.ContainsKey("id") && metadataValue["id"] != null)
                {
                    var transferPayableId = metadataValue["id"].ToString();
                    return transferPayableId == payableId;
                }
            }

            return false;
        }
    }
}
