using Domain.Entities;
using System;

namespace Domain.DTO
{
    public enum AntifraudOrderStatusCode
    {
        Approved,
        Denied,
        AnalisysPending
    }

    public class AntifraudOrderStatus
    {
        public Guid OrderId { get; private set; }
        public AntifraudOrderStatusCode Status { get; private set; }
        public string Message { get; private set; }
        public AntifraudOrderStatus(Guid orderId, AntifraudOrderStatusCode status, string message = null)
        {
            OrderId = orderId;
            Status = status;
            Message = message;
        }
        public AntifraudTransaction AntifraudTransaction { get; set; }
    }
}
