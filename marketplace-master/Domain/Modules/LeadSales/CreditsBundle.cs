using Application.Config;
using System;

namespace Domain.Entities
{
    public class CreditsBundle : BaseEntity<Guid>
    {
        public DateTime ExpirationDate { get; private set; }

        public int InitialCredits { get; private set; }

        public int CurrentCredits { get; private set; }

        public int AvailableCredits
        {
            get
            {
                if (MaaSConfig.Current.Quote.LeadSales.CreditsExpire)
                    return (DateTime.UtcNow > ExpirationDate) ? 0 : CurrentCredits;

                return CurrentCredits;
            }
        }

        public int ExpiredCredits
        {
            get
            {
                if (MaaSConfig.Current.Quote.LeadSales.CreditsExpire)
                    return (DateTime.UtcNow < ExpirationDate) ? 0 : CurrentCredits;

                return 0;
            }
        }

        public CreditsBundle(int initialCredits, DateTime expirationDate)
        {
            Id = Guid.NewGuid();
            InitialCredits = initialCredits;
            CurrentCredits = initialCredits;
            ExpirationDate = expirationDate;
        }

        public CreditsBundle()
        {

        }

        public void RemoveCredits(int amount)
        {
            CurrentCredits -= amount;
            CurrentCredits = Math.Max(0, CurrentCredits);
        }
    }
}
