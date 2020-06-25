using Newtonsoft.Json;
using System.Collections.Generic;

namespace Application.Config.Sections
{
    public class HelpQuestionsConfig
    {
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public bool IsVisible => Enabled && Items?.Count > 0;

        public IList<HelpQuestionsItemConfig> Items { get; set; } = new List<HelpQuestionsItemConfig>(3)
        {
            new HelpQuestionsItemConfig(),
            new HelpQuestionsItemConfig(),
            new HelpQuestionsItemConfig()
        };
    }
}