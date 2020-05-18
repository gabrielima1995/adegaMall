namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeRecipientTransferSettings
    {
        public PagarmeRecipientTransferSettings(PagarmeTransferInterval interval, bool enabled)
        {
            Interval = interval;
            Enabled = enabled;
        }

        public static PagarmeRecipientTransferSettings Default()
        {
            return new PagarmeRecipientTransferSettings(PagarmeTransferInterval.Default(), true);
        }

        public PagarmeTransferInterval Interval { get; }
        public bool Enabled { get; }
    }
}
