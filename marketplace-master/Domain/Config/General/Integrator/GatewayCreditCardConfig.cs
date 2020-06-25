namespace Domain.Config.General.Integrator
{
    public class GatewayCreditCardConfig
    {
        public bool Enabled { get; set; } = false;

        public GatewayCreditCardConfig(bool enabled = false)
        {
            Enabled = enabled;
        }
    }
}
