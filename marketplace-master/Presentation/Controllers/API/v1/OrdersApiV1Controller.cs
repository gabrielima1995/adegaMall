using Application.Auth;
using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Web.Http;
using Infrastructure;

namespace Presentation.Controllers.API.v1
{
    [AuthorizeApiToken]
    [RoutePrefix("api/v1/orders")]
    public class OrdersApiV1Controller : ApiController
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApiTokenAuthorizedProvider _authorizedProvider;
        private readonly ErrorReportingService _errorReportingService;

        public OrdersApiV1Controller(
            IOrderRepository orderRepository,
            ApiTokenAuthorizedProvider authorizedProvider,
            ErrorReportingService errorReportingService)
        {
            _orderRepository = orderRepository;
            _authorizedProvider = authorizedProvider;
            _errorReportingService = errorReportingService;
        }

        [Route("", Name = "Orders list")]
        public IHttpActionResult Get(int start = 0, int limit = 20, DateTime? start_date = null, DateTime? end_date = null, string seller_id = null, string buyer_id = null)
        {
            try
            {
                start_date = start_date?.ToUniversalTime();
                end_date = end_date?.ToUniversalTime();
                start = Math.Max(start, 0);
                limit = Math.Max(limit, 1);

                if (_authorizedProvider.Auth.HasSeller)
                {
                    seller_id = _authorizedProvider.Auth.Seller_Id;
                }

                var paging = _orderRepository.ListForPage(start, limit, start_date, end_date, seller_id, buyer_id);

                return Ok(paging.ToPagingResult(
                    start,
                    limit,
                    Url.Link("Orders list", new
                    {
                        start_date = start_date?.ToIsoString(),
                        end_date = end_date?.ToIsoString(),
                        seller_id,
                        buyer_id
                    })));
            }
            catch (Exception ex)
            {
                _errorReportingService.ReportError(ex);

                return InternalServerError();
            }
        }

        [Route("{id:guid}", Name = "Order by id")]
        public IHttpActionResult Get(Guid id)
        {
            try
            {
                string sellerId = null;

                if (_authorizedProvider.Auth.HasSeller)
                {
                    sellerId = _authorizedProvider.Auth.Seller_Id;
                }

                var result = _orderRepository.GetForPage(id, sellerId);

                if (result != null)
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _errorReportingService.ReportError(ex);

                return InternalServerError();
            }
        }
    }
}