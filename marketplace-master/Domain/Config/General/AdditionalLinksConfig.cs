namespace Domain.Config.General
{
    public class AdditionalLinksConfig
    {
        public bool Enabled { get; set; } = false;
        public string Url { get; set; } = "";
        public string Label { get; set; } = "";

        public AdditionalLinksConfig()
        {

        }

        public AdditionalLinksConfig(int index)
        {
            Label = $"ADDITIONAL_LINK_LABEL_{index}";
        }
    }
}
