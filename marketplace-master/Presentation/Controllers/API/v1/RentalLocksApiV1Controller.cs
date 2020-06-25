using Domain.DTO;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.Services.StoreModule;
using System;
using System.Web.Http;
using V2.Application.Modules.StoreModule.AppCommands;

namespace Presentation.Controllers.API.v1
{
    [AuthorizeApiToken]
    [RoutePrefix("api/v1/rentallocks")]
    public class RentalLocksApiV1Controller : ApiController
    {
        private readonly NewLockAppCommandHandler _newLockAppCommandHandler;
        private readonly ProductService _productService;
        private readonly ITimeZoneProvider _timeZoneProvider;

        public RentalLocksApiV1Controller(NewLockAppCommandHandler newLockAppCommandHandler, ProductService productService, ITimeZoneProvider timeZoneProvider)
        {
            _newLockAppCommandHandler = newLockAppCommandHandler;
            _productService = productService;
            _timeZoneProvider = timeZoneProvider;
        }

        [HttpPost]
        [Route("{id:guid}")]
        public IHttpActionResult Post([FromUri]Guid id, [FromBody]LockProductRentalDto lockProductRentalDto)
        {
            var product = _productService.GetById(id);
            var newRentalLock = CreateNewLockAppCommand(product, lockProductRentalDto);

            return Ok(_newLockAppCommandHandler.Handle(newRentalLock));
        }

        [HttpPost]
        [Route("ref:{ref}")]
        public IHttpActionResult PostReferenceId([FromUri]string @ref, [FromBody]LockProductRentalDto lockProductRentalDto)
        {
            var product = _productService.GetByRef(@ref);
            var newRentalLock = CreateNewLockAppCommand(product, lockProductRentalDto);

            return Ok(_newLockAppCommandHandler.Handle(newRentalLock));
        }

        private NewLockAppCommand CreateNewLockAppCommand(Product product, LockProductRentalDto lockProductRentalDto)
        {
            return  new NewLockAppCommand()
            {
                SellerId = product.Store_SellerId,
                Type = Domain.StoreModule.Entities.LockType.ByDate,
                StartDate = lockProductRentalDto.LockStartUtc,
                DayOfWeek = lockProductRentalDto.LockStartUtc.DayOfWeek,
                EndDate = lockProductRentalDto.LockEndUtc,
                StartTime = lockProductRentalDto.LockStartUtc.TimeOfDay,
                EndTime = lockProductRentalDto.LockEndUtc.TimeOfDay,
                TimeZone = _timeZoneProvider.GetTimeZone(),
                ProductId = product.Id,
                Mode = Domain.StoreModule.Entities.LockMode.FullyUnavailable
            };
        }

    }
}