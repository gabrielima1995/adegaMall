using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Infrastructure.Logging
{
    public class WebhookLogService
    {
        private readonly IWebhookCalledApiLogRepository _webhookLogRepository;
        private readonly ICloudStorage _cloudStorage;

        public WebhookLogService(
            IWebhookCalledApiLogRepository webhookLogRepository,
            ICloudStorage cloudStorage)
        {
            _webhookLogRepository = webhookLogRepository;
            _cloudStorage = cloudStorage;
        }

        public void Save(WebhookLogEntry entry)
        {
            var log = WebhookCalledApiLog.New(
                entry.WebhookLogEntryId,
                entry.Request.Token,
                entry.Seller,
                entry.Request.Timestamp,
                entry.Request.Uri,
                entry.EventType,
                entry.ResourceId,
                entry.Request.Method,
                entry.Response.StatusCode.ToString(),
                entry.Attempt);

            // Salva no banco de dados
            _webhookLogRepository.Add(log, true);

            // Salva na nuvem
            _cloudStorage.UploadFile("logs", entry.CreateFilename(), "application/json", entry.ConvertToStream());
        }
    }
}