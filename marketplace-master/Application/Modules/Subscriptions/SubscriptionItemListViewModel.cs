using Domain.Entities;
using Domain.Entities.SubscriptionModule;
using System;

namespace Application.Modules.Subscriptions
{
    public class SubscriptionItemListViewModel
    {
        public string Mode { get; }

        public decimal Amount { get; }

        public string SellerEmail { get; }

        public DateTime StartDate { get; }

        public string Status { get; }

        public SubscriptionItemListViewModel(Subscription subscription, Seller seller)
        {
            Mode = subscription.Plan.Name;
            Amount = subscription.Amount;
            SellerEmail = seller.Email;
            StartDate = subscription.StartDate;
            Status = subscription.Status.GetDescription();
        }
    }
}