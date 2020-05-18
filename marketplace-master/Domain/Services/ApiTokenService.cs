using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using System;

namespace Domain.Services
{
    public class ApiTokenService
    {
        private readonly IApiTokenRepository _apiTokenRepository;

        public ApiTokenService(
            IApiTokenRepository apiTokenRepository)

        {
            _apiTokenRepository = apiTokenRepository;
        }

        public OperationResult<ApiToken> GetById(Guid id)
        {
            var entity = _apiTokenRepository.GetById(id);

            if (entity == null)
            {
                return OperationResult<ApiToken>.Fail("", "Entity not found.");
            }

            if (entity.Deleted)
            {
                return OperationResult<ApiToken>.Fail("", "Token revoked.");
            }

            return entity;
        }

        public OperationResult Generate(string name, string sellerId = null)
        {
            return ApiToken
                .Generate(name, sellerId)
                .Then(result => _apiTokenRepository.Add(result, true));
        }

        public OperationResult Update(ApiTokenEditing model)
        {
            return GetById(model.Id)
                .Then(result => result.Update(model))
                .Then(result => _apiTokenRepository.Update(result, true));
        }

        public OperationResult Revoke(Guid id)
        {
            return GetById(id)
                .Then(result => result.Revoke())
                .Then(result => _apiTokenRepository.Update(result, true));
        }
    }
}