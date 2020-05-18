using System;

namespace V2.Application.Modules.Subscriptions.Model
{
    public class SubscriptionListItemForAdminDto
    {
        public string SellerId { get; set; }

        public string SellerName { get; set; }

        public DateTime StartDate { get; set; }

        public string PlanName { get; set; }

        public decimal Amount { get; set; }

        public int Status { get; set; }
    }
}