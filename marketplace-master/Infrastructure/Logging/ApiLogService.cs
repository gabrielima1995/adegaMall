using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;

namespace Infrastructure.Logging
{
    public class ApiLogService
    {
        private readonly IRequestReceivedApiLogRepository _apiLogRepository;
        private readonly ICloudStorage _cloudStorage;

        public ApiLogService(
            IRequestReceivedApiLogRepository apiLogRepository,
            ICloudStorage cloudStorage)
        {
            _apiLogRepository = apiLogRepository;
            _cloudStorage = cloudStorage;
        }

        public void Save(ApiLogEntry entry)
        {
            var log = RequestReceivedApiLog.New(
                entry.ApiLogEntryId,
                entry.Request.IpAddress,
                entry.Request.Token,
                entry.Request.Timestamp,
                entry.Request.Uri,
                entry.Request.Method,
                entry.Response.StatusCode.ToString());

            // Salva no banco de dados
            _apiLogRepository.Add(log, true);

            // Salva na nuvem
            _cloudStorage.UploadFile("logs", entry.CreateFilename(), "application/json", entry.ConvertToStream());
        }
    }
}