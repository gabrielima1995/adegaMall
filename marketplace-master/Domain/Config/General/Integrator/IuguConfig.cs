namespace Domain.Config.General.Integrator
{
    public class IuguConfig
    {
        public string AccountId { get; set; }
        public string ApiKeyLive { get; set; }
        public string ApiKeyTest { get; set; }
        public bool IsLive { get; set; } = false;
    }
}
