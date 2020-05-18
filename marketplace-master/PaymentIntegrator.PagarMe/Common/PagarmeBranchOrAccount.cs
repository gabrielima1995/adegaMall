namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeBranchOrAccount
    {
        private PagarmeBranchOrAccount(string main, string digit)
        {
            Main = main;
            Digit = digit;
        }

        public string Main { get; }
        public string Digit { get; }

        public static PagarmeBranchOrAccount From(string branchOrAccount)
        {
            var parts = branchOrAccount.Trim().Split('-');
            var digit = parts.Length > 1 ? parts[1] : null;
            return new PagarmeBranchOrAccount(parts[0], digit);
        }

        public override string ToString() => Digit == null ? Main : $"{Main}-{Digit}";
    }
}
