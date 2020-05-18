using Framework;

namespace Domain.Modules.Finance.Interfaces
{
    public interface IGatewayRegistrationLogRepository
    {
        OperationResult Add(GatewayRegistrationLog registrationLog);
        bool HasAnyLogsForSeller(string sellerId);
    }
}
