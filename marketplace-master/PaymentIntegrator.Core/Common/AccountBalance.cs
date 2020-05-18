namespace PaymentIntegrator.Core.Common
{
    public class AccountBalance
    {
        public AccountBalance()
        {
        }

        public AccountBalance(int available, int unavailable, int future)
        {
            AvailableBalance = available;
            UnavailableBalance = unavailable;
            FutureBalance = future;
        }

        public int AvailableBalance { get; private set; }
        public int UnavailableBalance { get; private set; }
        public int FutureBalance { get; private set; }

        public bool HasAnyBalance => AvailableBalance > 0 || UnavailableBalance > 0 || FutureBalance > 0;
    }
}
