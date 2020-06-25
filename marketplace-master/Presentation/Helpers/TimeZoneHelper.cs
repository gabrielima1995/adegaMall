using Framework;
using System.Web.Mvc;

namespace Presentation.Helpers
{
    public static class TimeZoneHelper
    {
        public static SelectList GetTimeZoneSelectList(string selected)
        {
            return new SelectList(TimeZones.All,
                nameof(MarketplaceTimeZone.TimeZoneId), nameof(MarketplaceTimeZone.Description),
                selected ?? TimeZones.Default.TimeZoneId);
        }
    }
}