using System;

namespace Domain.Events
{
    public class PersonUpdatedEventArgs : EventArgs
    {
        public bool IsBuyer { get; set; }
        public bool IsSeller { get; set; }
        public string UserId { get; set; }

        public PersonUpdatedEventArgs(string userId, bool isBuyer, bool isSeller)
        {
            UserId = userId;
            IsBuyer = isBuyer;
            IsSeller = isSeller;
        }
    }
}
