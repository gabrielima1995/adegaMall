using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using System;

namespace Domain.Services
{
    public class WebhookService
    {
        private readonly IWebhookRepository _webhookRepository;

        public WebhookService(
            IWebhookRepository webhookRepository)
        {
            _webhookRepository = webhookRepository;
        }

        public OperationResult<Webhook> GetById(Guid id, ApiToken apiToken)
        {
            var entity = _webhookRepository.GetById(id);

            if (entity == null || entity.Deleted)
            {
                return OperationResult<Webhook>.Fail("", "Entity not found.");
            }

            if (apiToken.Id != entity.ApiToken_Id)
            {
                return OperationResult<Webhook>.Fail("", "Unauthorized.");
            }

            return entity;
        }

        public OperationResult<WebhookListing> Create(WebhookCreating model, ApiToken apiToken)
        {
            return Webhook
                .Create(model, apiToken)
                .Then(result => _webhookRepository
                    .Add(result, true)
                    .Then<WebhookListing>(() => new WebhookListing(result)));
        }

        public OperationResult<WebhookListing> Update(WebhookEditing model, ApiToken apiToken)
        {
            return GetById(model.Id, apiToken)
                .Then(result => result.Update(model))
                .Then(result => _webhookRepository
                    .Update(result, true)
                    .Then<WebhookListing>(() => new WebhookListing(result)));
        }

        public OperationResult Delete(Guid id, ApiToken apiToken)
        {
            return GetById(id, apiToken)
                .Then(result => result.Delete())
                .Then(result => _webhookRepository.Update(result, true));
        }
    }
}