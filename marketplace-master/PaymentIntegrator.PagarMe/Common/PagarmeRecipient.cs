namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeRecipient
    {
        public PagarmeRecipient(PagarmeRecipientTransferSettings transferSettings, PagarmeBankAccount bankAccount)
        {
            TransferSettings = transferSettings;
            BankAccount = bankAccount;
        }

        public PagarmeRecipientTransferSettings TransferSettings { get; }
        public PagarmeBankAccount BankAccount { get; }

        public object AsJson()
        {
            return new
            {
                transfer_interval = TransferSettings.Interval.OptionString,
                transfer_day = TransferSettings.Interval.Day,
                transfer_enabled = TransferSettings.Enabled,
                bank_account = BankAccount.AsJson()
            };
        }
    }
}
