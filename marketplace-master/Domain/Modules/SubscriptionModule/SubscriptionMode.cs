using System.ComponentModel;

namespace Domain.Entities.SubscriptionModule
{
    /// <summary>
    /// Tipo de assinatura
    /// </summary>
    public enum SubscriptionMode
    {
        [Description("Mensal")]
        Monthly = 0,

        [Description("Trimestral")]
        Quarterly = 1,

        [Description("Anual")]
        Annual = 2
    }
}