using Domain.Core;
using Domain.Entities;
using Domain.StoreModule.Entities;
using Framework;
using System;

namespace Domain.StoreModule.Commands
{
    public class DeleteLockCommand : Command
    {
        public Guid LockId { get; set; }

        public string SellerId { get; set; }

        public DeleteLockCommand(RentalLock rentalLock, Seller seller)
            : base(rentalLock.Id.ToString())
        {
            Guard.Ensure.NotNull(rentalLock, nameof(rentalLock));
            Guard.Ensure.NotNull(seller, nameof(seller));
            Guard.Ensure.Rule(() => seller.Id == rentalLock.Seller_Id, nameof(seller), "Seller invalid");

            LockId = rentalLock.Id;
            SellerId = seller.Id;
        }
    }
}