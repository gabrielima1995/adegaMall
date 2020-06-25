using Domain.Interfaces.Repositories;
using Domain.StoreModule.Interfaces;
using Presentation.Models.DisputeViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class DisputesController : Controller
    {
        private readonly IDisputeRepository _disputeRepository;
        private readonly IOrderRepository _orderRepository;

        public DisputesController(
            IDisputeRepository disputeRepository,
            IOrderRepository orderRepository)
        {
            _disputeRepository = disputeRepository;
            _orderRepository = orderRepository;
        }

        public ActionResult Index()
        {
            var items = _disputeRepository.List();

            var modelList = new List<DisputeViewModel>();

            foreach (var dispute in items)
            {
                var order = _orderRepository.GetDetails(new Guid(dispute.OrderId));
                modelList.Add(new DisputeViewModel(order, dispute, null));
            }

            return View("Index", modelList);
        }

        //public ActionResult Details(Guid? id)
        //{
        //    if (id == null) return HttpNotFound();
        //    var order = _orderService.GetDetails(id.Value);
        //    if (order == null) return HttpNotFound();

        //    return View(order);
        //}
    }
}