using System;

namespace PaymentIntegrator.PagarMe.Common
{
    public class PagarmeTransferInterval
    {
        public PagarmeTransferInterval(int day, IntervalOption option)
        {
            Day = day;
            Option = option;
        }

        public static PagarmeTransferInterval From(int option) => new PagarmeTransferInterval(0, (IntervalOption)option);
        public static PagarmeTransferInterval From(string option) => new PagarmeTransferInterval(0, (IntervalOption)Enum.Parse(typeof(IntervalOption), option, true));
        public static PagarmeTransferInterval Default() => From(PagarMeConfig.RecipientTransferDay);

        public int Day { get; }
        public IntervalOption Option { get; }
        public string OptionString => Option.ToString().ToLower();

        public enum IntervalOption
        {
            Daily = 0, Weekly = 1, Monthly = 2
        }
    }
}
