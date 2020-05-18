using Application.Config.Common;

namespace Application.Config.Modules
{
    public class StockConfig
    {
        public StatusConfig HideCurrentStock { get; set; } = new StatusConfig(enabled: false);
    }
}