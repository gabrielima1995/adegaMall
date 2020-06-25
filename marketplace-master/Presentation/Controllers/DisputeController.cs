using Application.Modules.Products.Orders;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.DisputeModule;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.StoreModule.Interfaces;
using Infra.Crosscutting.Globalization;
using Microsoft.AspNet.Identity;
using Presentation.Models.DisputeViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.Disputes;
using V2.Application.Modules.SellerAssistantModule;

namespace Presentation.Controllers
{
    public class DisputeController : BaseController
    {
        private readonly IDisputeAppService disputeAppService;
        private readonly IDisputeRepository disputeRepository;
        private readonly OrderAppService orderAppService;
        private readonly IOrderRepository orderRepository;
        private readonly IPersonRepository personRepository;
        private readonly IScheduledTransferRepository scheduledTransferRepository;

        public DisputeController(
            IDisputeAppService disputeAppService,
            IDisputeRepository disputeRepository,
            OrderAppService orderAppService,
            IOrderRepository orderRepository,
            IPersonRepository personRepository,
            IScheduledTransferRepository scheduledTransferRepository)
        {
            this.disputeAppService = disputeAppService;
            this.disputeRepository = disputeRepository;
            this.orderAppService = orderAppService;
            this.orderRepository = orderRepository;
            this.personRepository = personRepository;
            this.scheduledTransferRepository = scheduledTransferRepository;
        }

        [AuthorizeEnabled]
        [RequireAssistantPermissions(SellerAssistantPermissions.DisputesView)]
        public ActionResult Details(string orderId)
        {
            if (orderId == null)
                return HttpNotFound();

            var order = orderRepository.GetDetails(new Guid(orderId));
            if (order == null || order.Dispute_Id == null)
                return HttpNotFound();

            var dispute = disputeRepository.GetById(order.Dispute_Id.Value);
            if (dispute == null)
                return HttpNotFound();

            var personId = GetPersonId();
            if (personId != dispute.BuyerId && personId != dispute.SellerId && !GetLoginProfile().IsAdminOrGestao())
                return HttpNotFound();

            var messages = new List<DisputeCommentViewModel>();
            foreach (var comment in dispute.DisputeComments)
            {
                var senderPerson = comment.SenderId == null ? null : personRepository.GetById(comment.SenderId);
                var message = new DisputeCommentViewModel(comment, senderPerson);
                messages.Add(message);
            }
            messages = messages.OrderBy(x => x.DatePosted).ToList();

            var ViewModel = new DisputeViewModel(order, dispute, messages)
            {
                PaymentTransferred = scheduledTransferRepository.Get(x => x.OrderId == orderId && x.TransferAuthorized) != null
            };

            return View("Details", ViewModel);
        }

        [AuthorizeEnabled]
        [RequireAssistantPermissions(SellerAssistantPermissions.DisputesEdit)]
        public ActionResult OpenDispute(string orderId)
        {
            var existingDispute = disputeRepository.GetByOrderId(orderId);
            if (existingDispute != null)
                return Details(orderId);

            var order = orderRepository.GetDetails(new Guid(orderId));
            order.ExtendAutoCompletionDateForDispute();

            if (order == null || order.Dispute_Id != null || order.Status == OrderStatus.Completed)
                return HttpNotFound();

            var person = personRepository.GetById(GetPersonId());
            var dispute = Dispute.Start(order.Id, order.Buyer.Id, order.Seller.Id, person);
            order.SetDispute(dispute);

            disputeRepository.Add(dispute, true);
            orderRepository.Update(order, true);

            return RedirectToAction("Details", new { orderId });
        }

        [HttpPost]
        [AuthorizeEnabled]
        [RequireAssistantPermissions(SellerAssistantPermissions.DisputesEdit)]
        public ActionResult AddComment(DisputeCommentViewModel commentVM)
        {
            var disputeId = commentVM.DisputeId;

            if (string.IsNullOrWhiteSpace(commentVM.Message))
                return new EmptyResult();

            DisputeComment comment = new DisputeComment()
            {
                Message = commentVM.Message,
                SenderId = GetPersonId(),
                IsSystemMessage = false,
            };

            if (disputeRepository.AddComment(disputeId, comment).OK)
                return RedirectToAction("Details", new { orderId = disputeRepository.GetById(new Guid(disputeId)).OrderId });

            return HttpNotFound();
        }

        [HttpPost]
        [AuthorizeBuyerOrAdmin]
        public ActionResult AuthorizePayment(string orderId)
        {
            if (orderId == null)
                return HttpNotFound();

            var order = orderRepository.GetDetails(new Guid(orderId));
            var personId = GetPersonId();

            if (order.Status.In(OrderStatus.Completed, OrderStatus.Rejected) || (personId != order.Buyer.Id && !GetLoginProfile().IsAdminOrGestao()))
                return HttpNotFound();

            var actingPerson = personRepository.GetById(GetPersonId());
            var result = orderAppService.AuthorizePayment(order, actingPerson, GetLoginProfile().IsAdminOrGestao() ? "Admin" : "Buyer");
            if (result.IsFailure)
                return HttpNotFound();

            order = result.Result;
            orderRepository.Update(order, true);

            return order.Dispute_Id != null
                ? RedirectToAction("Details", new { orderId })
                : RedirectToAction("Details", "MyOrders", new { id = new Guid(orderId) });
        }

        [HttpPost]
        [AuthorizeEnabled]
        [RequireAssistantPermissions(SellerAssistantPermissions.DisputesEdit)]
        public ActionResult ReturnPayment(string orderId)
        {
            var order = orderRepository.GetDetails(new Guid(orderId));
            var personId = User.Identity.GetPersonId();
            var dispute = disputeRepository.GetById(order.Dispute_Id);

            if (order.Status.In(OrderStatus.Completed, OrderStatus.Rejected) || (personId != dispute.SellerId && !GetLoginProfile().IsAdminOrGestao()))
                return HttpNotFound();

            var actingPerson = personRepository.GetById(GetPersonId());
            order.ChangeStatus(OrderStatus.Canceled, actingPerson);
            var result = orderRepository.Update(order, true);

            if (result.OK)
            {
                dispute.SetStatus(GetLoginProfile().IsAdminOrGestao() ? DisputeStatus.ClosedByAdmin : DisputeStatus.ClosedBySeller);
                disputeRepository.Update(dispute, true);

                string message = GlobalizationHelper.GetString("DISPUTE_SYSMESSAGE_ORDER_CANCELED");
                disputeAppService.AddSystemMessage(dispute, message);
            }

            return RedirectToAction("Details", new { orderId });
        }

        [HttpGet]
        [AuthorizeBuyerOrSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.DisputesEdit)]
        public ActionResult RequestMediation(string disputeId)
        {
            var dispute = disputeRepository.GetById(new Guid(disputeId));
            var personId = User.Identity.GetPersonId();

            if (dispute == null || (personId != dispute.BuyerId && personId != dispute.SellerId))
                return HttpNotFound();

            dispute.SetStatus(GetLoginProfile().IsBuyer()
                ? DisputeStatus.MediationRequestedByBuyer
                : DisputeStatus.MediationRequestedBySeller);

            var authorName = User.Identity.GetLoginProfile().Name;
            var message = GlobalizationHelper.FormatString("DISPUTE_SYSMESSAGE_MEDIATION_REQUESTED", authorName);
            disputeAppService.AddSystemMessage(dispute, message);
            disputeRepository.Update(dispute, true);

            var order = orderRepository.GetDetails(new Guid(dispute.OrderId));
            order.AddHistoryMessage(message);
            orderRepository.Update(order, saveChanges: true);

            return RedirectToAction("Details", new { orderId = dispute.OrderId });
        }
    }
}