using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Crosscutting.PayPal
{
    public interface IPayPalConfig
    {
        string MerchantId { get; }

        bool IsSandbox { get; }

        string NotifyUrl { get; }
    }
}