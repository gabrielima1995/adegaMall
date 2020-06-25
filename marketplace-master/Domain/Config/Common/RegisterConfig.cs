using Newtonsoft.Json;

namespace Application.Config.Common
{
    public class RegisterConfig : StatusConfig
    {
        public bool Mandatory { get; set; }

        [JsonIgnore]
        public bool IsRequired => Enabled && Mandatory;

        public RegisterConfig(bool enabled = true, bool mandatory = true)
            : base(enabled)
        {
            Mandatory = mandatory;
        }
    }
}