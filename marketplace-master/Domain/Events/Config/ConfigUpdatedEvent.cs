using Application.Config;

namespace Domain.Events.Config
{
    public class ConfigUpdatedEvent : DomainEvent
    {
        public MaaSConfig NewConfig { get; protected set; }
        public MaaSConfig OldConfig { get; protected set; }

        public ConfigUpdatedEvent(MaaSConfig newConfig, MaaSConfig oldConfig)
        {
            NewConfig = newConfig;
            OldConfig = oldConfig;
        }
    }
}