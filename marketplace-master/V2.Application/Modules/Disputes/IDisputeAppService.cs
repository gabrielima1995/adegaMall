using Domain.Entities.DisputeModule;
using Framework;

namespace V2.Application.Modules.Disputes
{
    public interface IDisputeAppService
    {
        OperationResult AddSystemMessage(Dispute dispute, string message);
    }
}
