using Application.Config;
using Application.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.DisputeModule;
using Domain.Entities.StoreModule;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;

namespace Presentation.Models.DisputeViewModels
{
    public class DisputeViewModel
    {
        public DisputeStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DisputeCommentViewModel> MessageList { get; set; }

        public string OrderId { get; set; }
        public string DisputeId { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public string SellerAvatar { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAvatar { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public decimal OrderAmount { get; set; }
        public bool PaymentTransferred { get; set; }
        public bool MediationRequested { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime AutoTransferDeadline { get; set; }

        public DisputeViewModel(Order order, Dispute dispute, List<DisputeCommentViewModel> messageList)
        {
            MessageList = messageList ?? new List<DisputeCommentViewModel>();

            DisputeId = dispute.Id.ToString();
            CreatedAt = dispute.CreatedAt;
            Status = dispute.Status;

            SellerId = order.Seller.Id;
            SellerName = order.Seller.Name;
            SellerAvatar = ImgHelper.Avatar(order.Seller.Avatar, 80);

            BuyerId = order.Buyer.Id;
            BuyerName = order.Buyer.Name;
            BuyerAvatar = ImgHelper.Avatar(order.Buyer.Avatar, 80);

            OrderId = order.Id.ToString();
            OrderAmount = order.Amount;
            OrderStatus = order.Status;

            MediationRequested = dispute.MediationRequested;
            AutoTransferDeadline = order.AutoCompletionDate.Value;
            Deadline = order.Deadline.GetValueOrDefault();
        }

        public string GetDisputeStatusLocalized() => Status.DisplayDescription();

        public string GetTransferStatusLocalized()
        {
            return GlobalizationHelper.GetString(PaymentTransferred ? "AFFIRMATIVE_LABEL" : "NEGATIVE_LABEL");
        }

        public string GetOrderUrl(LoginProfile profile)
        {
            var baseUrl = MaaSConfig.Current.Environment.AppUrl;

            if (profile.IsBuyer())
                return baseUrl + $"myorders/details/{OrderId}/";

            if (profile.IsSellerOrSellerAssistant())
                return baseUrl + $"mysales/details/{OrderId}/";

            if (profile.IsAdminOrGestao())
                return baseUrl + $"admin/orders/details/{OrderId}/";

            throw new InvalidOperationException($"Método não trata casos em que usuário não se encaixa em uma das funções: Buyer, Seller, Admin, GestaoINA");
        }

        public bool IsPaymentReturned()
        {
            return OrderStatus == OrderStatus.Canceled ||
                   OrderStatus == OrderStatus.Rejected ||
                   OrderStatus == OrderStatus.Returned;
        }

        public bool IsTransferMade() => OrderStatus == OrderStatus.Completed;
        public bool IsCancellable() => !IsTransferMade() && !IsPaymentReturned();

        public bool CanAuthorizePayment()
        {
            switch (OrderStatus)
            {
                default:
                    return false;
                case OrderStatus.CompletedPayment:
                case OrderStatus.AwaitingDelivery:
                case OrderStatus.DeliveryOnTheWay:
                case OrderStatus.Delivered:
                case OrderStatus.AwaitingPickup:
                    return true;
            }
        }
    }

    public class DisputeCommentViewModel
    {
        public string DisputeId { get; set; }
        public string Message { get; set; }

        public bool IsSystemMessage { get; set; }
        public DateTime DatePosted { get; set; }

        public string SenderName { get; set; }
        public string SenderRole { get; set; }
        public string SenderAvatar { get; set; }

        public DisputeCommentViewModel()
        {
        }

        public DisputeCommentViewModel(string disputeId)
        {
            DisputeId = disputeId;
        }

        public DisputeCommentViewModel(DisputeComment comment, Person senderPerson)
        {
            Message = comment.Message;
            DatePosted = comment.CreatedAt;
            IsSystemMessage = comment.IsSystemMessage;

            SenderName = senderPerson?.Name ?? GlobalizationHelper.GetString("DISPUTE_AUTOMATIC_MESSAGE_LABEL");
            SenderAvatar = ImgHelper.Avatar(senderPerson?.Avatar, 100);
            SenderRole = "System";

            if (senderPerson != null)
            {
                SenderRole =
                    senderPerson.PersonType == PersonType.Buyer ? "Buyer" :
                    senderPerson.PersonType.In(PersonType.Seller, PersonType.SellerAssistant) ? "Seller" :
                    senderPerson.PersonType == PersonType.AdminPerson ? "Admin" : SenderRole;
            }
        }
    }
}