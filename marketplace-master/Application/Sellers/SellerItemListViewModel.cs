using Domain.Entities;
using System;

namespace Application.Sellers
{
    public class SellerItemListViewModel
    {
        public string Id { get; set; }

        public string FullName { get; }

        public string Nickname { get; }

        public string Email { get; }

        public DateTime CreatedAt { get; }

        public string ProfileUrl { get; }

        public string SubscriptionStatus { get; }

        public string SubscriptionMode { get; }

        public DateTime? SubscriptionDate { get; }

        public int TotalQuotes { get; }

        public string SubscriptionStarted { get; set; }

        public bool Enabled { get; set; }

        public SellerItemListViewModel(Seller seller, int totalQuotes)
        {
            Id = seller.Id;
            FullName = seller.FullName;
            Nickname = seller.Nickname;
            Email = seller.Email;
            CreatedAt = seller.CreatedAt;
            ProfileUrl = seller.GetProfileUrl();
            SubscriptionStatus = seller.HasSubscription ? seller.Subscription.Status.GetDescription() : "Sem assinatura";
            SubscriptionMode = seller.HasSubscription ? seller.Subscription.Plan.Name : "";
            SubscriptionDate = seller.HasSubscription ? seller.Subscription.StartDate : default(DateTime?);
            TotalQuotes = totalQuotes;
            SubscriptionStarted = seller.SubscriptionStarted ? "Sim" : "Não";
            Enabled = seller.Enabled;
        }
    }
}