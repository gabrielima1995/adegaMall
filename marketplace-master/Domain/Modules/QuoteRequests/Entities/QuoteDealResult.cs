using Infra.Crosscutting.Globalization.Attributes;

namespace Domain.QuoteRequests
{
    public enum QuoteDealResult
    {
        [DescriptionLocalized("QUOTE_CLOSED_DEAL_WON")]
        Won = 0,

        [DescriptionLocalized("QUOTE_CLOSED_DEAL_LOST")]
        Lost = 1,

        [DescriptionLocalized("QUOTE_CLOSED_DEAL_UNKNOWN")]
        Unknown = 2
    }
}
