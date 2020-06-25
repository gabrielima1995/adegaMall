using System;

namespace V2.Application.Modules.StoreModule.AppCommands
{
    public class DeleteLockAppCommand
    {
        public Guid LockId { get; set; }

        public string SellerId { get; set; }
    }
}