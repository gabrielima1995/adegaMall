using Newtonsoft.Json;

namespace PaymentIntegrator.Moip.Orders
{
    public class MoipReceiver
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("feePayor")]
        public bool MoipFeePayor { get; set; }

        [JsonProperty("moipAccount")]
        public object MoipAccount { get; set; }

        [JsonProperty("amount")]
        public object Amount { get; set; }

        private MoipReceiver(string type, bool moipFeePayor, string moipAccountId, int? fixedAmount, int? percentual)
        {
            Type = type;
            MoipFeePayor = moipFeePayor;
            MoipAccount = new { id = moipAccountId };
            Amount = fixedAmount > 0
                ? new { @fixed = fixedAmount.ToString() }
                : new { percentual = percentual.ToString() } as object;
        }

        public static MoipReceiver WithFixedAmount(string type, bool moipFeePayor, string moipAccountId, int fixedAmount) => new MoipReceiver(type, moipFeePayor, moipAccountId, fixedAmount, null);
        public static MoipReceiver WithPercentualAmount(string type, bool moipFeePayor, string moipAccountId, int percentual) => new MoipReceiver(type, moipFeePayor, moipAccountId, null, percentual);

        public MoipReceiver()
        {
        }
    }
}