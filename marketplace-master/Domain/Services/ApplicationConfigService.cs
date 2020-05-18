using Application.Config;
using Domain.Interfaces.Repositories;
using Framework;
using System;

namespace Domain.Services
{
    public class ApplicationConfigService
    {
        private readonly IApplicationConfigRepository _appConfigRepository;

        public ApplicationConfigService(IApplicationConfigRepository appConfigRepository)
        {
            _appConfigRepository = appConfigRepository;
        }

        public ApplicationConfig Get()
        {
            var appConfig = _appConfigRepository.Get();
            return appConfig ?? new ApplicationConfig();
        }

        public OperationResult Update(ApplicationConfig appConfig)
        {
            var current = Get();
            current.MaaSConfig = appConfig.MaaSConfig;

            Sanitize(current);

            if (current.Id == 0)
            {
                var result = _appConfigRepository.Add(current, saveChanges: true);
                return result;
            }
            else
            {
                var result = _appConfigRepository.Update(current, saveChanges: true);
                return result;
            }
        }

        private void Sanitize(ApplicationConfig appConfig)
        {
            var maasConfig = appConfig.MaaSConfig.FromJson<MaaSConfig>();
            maasConfig.Sanitize();
        }
    }
}
