using Domain.Core;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class MarkRentalAsReturnedCommand : Command
    {
        public Guid ProductId { get; private set; }

        public Guid OrderId { get; private set; }

        public DateTime ReturnedAt { get; private set; }

        public MarkRentalAsReturnedCommand(Guid productId, Guid orderId, DateTime returnedAt) : base(productId.ToString())
        {
            Guard.Ensure.NotEmpty(productId, nameof(productId));
            Guard.Ensure.NotEmpty(orderId, nameof(orderId));
            Guard.Ensure.NotEmpty(returnedAt, nameof(returnedAt));

            ProductId = productId;
            OrderId = orderId;
        }
    }
}
