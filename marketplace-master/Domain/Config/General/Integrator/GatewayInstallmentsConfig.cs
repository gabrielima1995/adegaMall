namespace Application.Config.General.Integrator
{
    public class GatewayInstallmentsConfig
    {
        public int MaxInstallments { get; set; } = 1;

        public decimal MinInstallmentAmount { get; set; } = 0M;

        public decimal MinOrderAmount { get; set; } = 0M;
    }
}
