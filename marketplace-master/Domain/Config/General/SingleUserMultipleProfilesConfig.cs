using Application.Config.Common;

namespace Domain.Config.General
{
    public class SingleUserMultipleProfilesConfig : StatusConfig
    {
        private SingleUserMultipleProfilesConfig() { }
        public SingleUserMultipleProfilesConfig(bool enabled) : base(enabled) { }
    }
}
