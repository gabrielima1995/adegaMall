using System;
namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class DeleteLockForProductAppComand
    {
        public Guid ProductId { get; set; }

        public string SellerId { get; set; }
    }
}
