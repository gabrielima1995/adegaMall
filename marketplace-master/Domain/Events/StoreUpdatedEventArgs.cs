using System;

namespace Domain.Events
{
    public class StoreUpdatedEventArgs : EventArgs
    {
        public string SellerId { get; private set; }

        public StoreUpdatedEventArgs(string sellerId)
        {
            SellerId = sellerId;
        }
    }
}