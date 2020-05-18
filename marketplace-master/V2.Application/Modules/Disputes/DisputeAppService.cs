using Domain.Entities.DisputeModule;
using Domain.StoreModule.Interfaces;
using Framework;

namespace V2.Application.Modules.Disputes
{
    public class DisputeAppService : IDisputeAppService
    {
        private readonly IDisputeRepository _disputeRepository;

        public DisputeAppService(IDisputeRepository disputeRepository) : base()
        {
            _disputeRepository = disputeRepository;
        }

        public OperationResult AddSystemMessage(Dispute dispute, string message)
        {
            var disputeId = dispute.Id.ToString();

            if (!string.IsNullOrWhiteSpace(message))
            {
                DisputeComment comment = new DisputeComment()
                {
                    Message = message,
                    SenderId = null,
                    IsSystemMessage = true,
                };

                return _disputeRepository.AddComment(disputeId, comment);
            }

            return OperationResult.Fail("Invalid message");
        }

    }
}
