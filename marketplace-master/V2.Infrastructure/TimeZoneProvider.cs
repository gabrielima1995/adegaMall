using Domain.Interfaces;
using Framework;

namespace V2.Infrastructure
{
    public class TimeZoneProvider : ITimeZoneProvider
    {
        private readonly IUserAuthProvider authProvider;

        public TimeZoneProvider(IUserAuthProvider authProvider)
        {
            this.authProvider = authProvider;
        }

        public MarketplaceTimeZone GetTimeZone()
        {
            var user = authProvider.GetLoginProfile();
            return user == null ? TimeZones.Default : TimeZones.Find(user.PreferredTimeZoneId);
        }
    }
}