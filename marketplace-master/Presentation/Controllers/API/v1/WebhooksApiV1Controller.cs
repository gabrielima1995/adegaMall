using Application.Auth;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Framework;
using Infra.Crosscutting.WebHelpers.WebApi;
using System;
using System.Net;
using System.Web.Http;

namespace Presentation.Controllers.API.v1
{
    [AuthorizeApiToken]
    [RoutePrefix("api/v1/webhooks")]
    public class WebhooksApiV1Controller : ApiController
    {
        private readonly IWebhookRepository _webhookRepository;
        private readonly WebhookService _webhookService;
        private readonly ApiTokenAuthorizedProvider _authorizedProvider;

        public WebhooksApiV1Controller(
            IWebhookRepository webhookRepository,
            WebhookService webhookService,
            ApiTokenAuthorizedProvider authorizedProvider)
        {
            _webhookRepository = webhookRepository;
            _webhookService = webhookService;
            _authorizedProvider = authorizedProvider;
        }

        [Route("", Name = "Webhooks list")]
        public IHttpActionResult Get(int start = 0, int limit = 20)
        {
            string seller_id = null;

            start = Math.Max(start, 0);
            limit = Math.Max(limit, 1);

            if (_authorizedProvider.Auth.HasSeller)
            {
                seller_id = _authorizedProvider.Auth.Seller_Id;
            }

            var paging = _webhookRepository.ListForPage(start, limit, seller_id);

            return Ok(paging.ToPagingResult(
                start,
                limit,
                Url.Link("Webhooks list", new { })));
        }

        [Route("{id:guid}", Name = "Webhook by id")]
        public IHttpActionResult Get([FromUri]Guid id)
        {
            return this.Result(_webhookService
                .GetById(id, _authorizedProvider.Auth)
                .Then<Webhook, WebhookListing>(result => new WebhookListing(result)));
        }

        [Route("")]
        public IHttpActionResult Post([FromBody] WebhookCreating model)
        {
            return this.Result(_webhookService.Create(model, _authorizedProvider.Auth), HttpStatusCode.Created);
        }

        [Route("{id:guid}")]
        public IHttpActionResult Put([FromUri]Guid id, [FromBody] WebhookEditing model)
        {
            model.Id = id;

            return this.Result(_webhookService.Update(model, _authorizedProvider.Auth));
        }

        [Route("{id:guid}")]
        public IHttpActionResult Delete([FromUri]Guid id)
        {
            return this.Result(_webhookService.Delete(id, _authorizedProvider.Auth));
        }
    }
}