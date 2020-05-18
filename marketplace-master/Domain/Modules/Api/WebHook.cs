using Domain.DTO;
using Framework;
using System;

namespace Domain.Entities
{
    public class Webhook : SoftDeletableEntity<Guid>
    {
        public WebhookEvent EventType { get; private set; }

        public string Url { get; private set; }

        public Guid ApiToken_Id { get; private set; }

        public ApiToken ApiToken { get; private set; }

        private Webhook()
        {
            Id = Guid.NewGuid();
        }

        private Webhook(WebhookEvent eventType, string url, Guid apiTokenId)
            : this()
        {
            EventType = eventType;
            Url = url;
            ApiToken_Id = apiTokenId;
        }

        private OperationResult<Webhook> Validate()
        {
            StringHelper.Sanitize(this);

            if (string.IsNullOrWhiteSpace(Url))
            {
                return OperationResult<Webhook>.Fail("Url", "Url required.");
            }

            if (ApiToken == null && ApiToken_Id == Guid.Empty)
            {
                return OperationResult<Webhook>.Fail("ApiToken", "ApiToken required.");
            }

            return this;
        }

        public static OperationResult<Webhook> New(WebhookEvent eventType, string url, Guid apiTokenId)
        {
            return new Webhook(eventType, url, apiTokenId)
                .Validate();
        }

        public static OperationResult<Webhook> Create(WebhookCreating model, ApiToken apiToken)
        {
            return New(model.EventType, model.Url, apiToken.Id);
        }

        public OperationResult<Webhook> Update(WebhookEditing model)
        {
            EventType = model.EventType;
            Url = model.Url;

            return Validate();
        }

        public OperationResult<Webhook> Delete()
        {
            Deleted = true;
            DeletedAt = DateTime.UtcNow;

            return this;
        }
    }
}