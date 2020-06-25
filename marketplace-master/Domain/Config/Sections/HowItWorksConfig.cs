using Newtonsoft.Json;
using System.Collections.Generic;

namespace Application.Config.Sections
{
    public class HowItWorksConfig
    {
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public bool IsVisible => Enabled && Items?.Count > 0;

        public IList<HowItWorksItemConfig> Items { get; set; } = new List<HowItWorksItemConfig>(3)
        {
            new HowItWorksItemConfig
            {
                ImageUrl = "/assets/images/howitworks-1.svg"
            },
            new HowItWorksItemConfig
            {
                ImageUrl = "/assets/images/howitworks-2.svg"
            },
            new HowItWorksItemConfig
            {
                ImageUrl = "/assets/images/howitworks-3.svg"
            }
        };
    }
}