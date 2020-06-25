using Newtonsoft.Json;
using System.Collections.Generic;

namespace Application.Config.Sections
{
    public class MiscAreaConfig
    {
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public bool IsVisible => Enabled && Items?.Count > 0;

        public IList<MiscAreaItemConfig> Items { get; set; } = new List<MiscAreaItemConfig>(5)
        {
            new MiscAreaItemConfig(),
            new MiscAreaItemConfig(),
            new MiscAreaItemConfig(),
            new MiscAreaItemConfig(),
            new MiscAreaItemConfig()
        };
    }
}